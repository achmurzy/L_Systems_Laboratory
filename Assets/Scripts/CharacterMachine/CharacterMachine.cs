using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;

public class CharacterMachine : MonoBehaviour, ICharacterController
{
    public enum OrientationMethod
    {
        TowardsCamera,
        TowardsMovement,
    }

    //These are interpreted across states in context-dependent ways
    public struct PlayerCharacterInputs
    {
        public float MoveAxisForward;
        public float MoveAxisRight;
        public Quaternion CameraRotation;
        public bool JumpDown;
        public bool CrouchDown;
        public bool CrouchUp;
    }
    public struct AICharacterInputs
    {
        public Vector3 MoveVector;
        public Vector3 LookVector;
    }

    public KinematicCharacterMotor Motor;

    protected Dictionary<int, CharacterState> states;   //States are encoded as named constant integers unique to the
    protected CharacterState currentState, lastState;   //particular machine. Transitions occur by passing the named
                                                        //index from a state to the machine, which indexes the dict.
    protected const int NoTransition = -1;
    protected const int Base = 0;           //Most of these are totally useless to us and exist on rigidbodies. Consider removal

    protected Vector3 _moveInputVector;
    protected Vector3 _lookInputVector;
    public OrientationMethod Orientation_Method = OrientationMethod.TowardsCamera;

    protected float _timeSinceJumpRequested = Mathf.Infinity;
    protected bool _jumpRequested = false;

    protected void Awake()
    {
        states = new Dictionary<int, CharacterState>();

    }
    protected void Start()
    {
        
    }

    /// <summary>
    /// This is called every frame by the AI script in order to tell the character what its inputs are
    /// </summary>
    public void SetInputs(ref AICharacterInputs inputs)
    {
        _moveInputVector = inputs.MoveVector;
        _lookInputVector = inputs.LookVector;
    }

    /// <summary>
    /// This is called every frame by ExamplePlayer in order to tell the character what its inputs are
    /// </summary>
    public void SetInputs(ref PlayerCharacterInputs inputs)
    {
        // Clamp input
        Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(inputs.MoveAxisRight, 0f, inputs.MoveAxisForward), 1f);

        // Calculate camera direction and rotation on the character plane
        Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.forward, Motor.CharacterUp).normalized;
        if (cameraPlanarDirection.sqrMagnitude == 0f)
        {
            cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.up, Motor.CharacterUp).normalized;
        }
        Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, Motor.CharacterUp);

        // Move and look inputs
        _moveInputVector = cameraPlanarRotation * moveInputVector;

        switch (Orientation_Method)
        {
            case OrientationMethod.TowardsCamera:
                _lookInputVector = cameraPlanarDirection;
                break;
            case OrientationMethod.TowardsMovement:
                _lookInputVector = _moveInputVector.normalized;
                break;
        }

        // Jumping input
        if (inputs.JumpDown)
        {
            _timeSinceJumpRequested = 0f;
            _jumpRequested = true;
        }
    }

    #region KinematicCharacterController callbacks
    /// <summary>
    /// (Called by KinematicCharacterMotor during its update cycle)
    /// This is called before the character begins its movement update
    /// </summary>
    public virtual void BeforeCharacterUpdate(float deltaTime)
    {
        currentState.BeforeCharacterUpdate(deltaTime);
    }

    /// <summary>
    /// (Called by KinematicCharacterMotor during its update cycle)
    /// This is where you tell your character what its rotation should be right now. 
    /// This is the ONLY place where you should set the character's rotation
    /// </summary>
    public virtual void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        currentState.UpdateRotation(ref currentRotation, deltaTime);
    }

    /// <summary>
    /// (Called by KinematicCharacterMotor during its update cycle)
    /// This is where you tell your character what its velocity should be right now. 
    /// This is the ONLY place where you can set the character's velocity
    /// </summary>
    public virtual void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        currentState.UpdateVelocity(ref currentVelocity, deltaTime);
    }

    public virtual void PostGroundingUpdate(float deltaTime)
    {
        currentState.PostGroundingUpdate(deltaTime);
    }

    /// <summary>
    /// (Called by KinematicCharacterMotor during its update cycle)
    /// This is called after the character has finished its movement update
    /// This is where all character state transitions should occur
    /// </summary>
    public virtual void AfterCharacterUpdate(float deltaTime)
    {
        currentState.AfterCharacterUpdate(deltaTime);
    }

    public virtual bool IsColliderValidForCollisions(Collider coll)
    {
        return currentState.IsColliderValidForCollisions(coll);
    }

    public virtual void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
        currentState.OnGroundHit(hitCollider, hitNormal, hitPoint, ref hitStabilityReport);
    }

    public virtual void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
        currentState.OnMovementHit(hitCollider, hitNormal, hitPoint, ref hitStabilityReport);
    }

    public virtual void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
    {
        currentState.ProcessHitStabilityReport(hitCollider, hitNormal, hitPoint, atCharacterPosition, atCharacterRotation, ref hitStabilityReport);
    }

    public virtual void OnDiscreteCollisionDetected(Collider hitCollider)
    {
        currentState.OnDiscreteCollisionDetected(hitCollider);
    }
    #endregion


    //Finds the common root between two states in the hierarchy in order to iteratively perform state transitions during state change
    private int findCommonRoot(int sourceState, int targetState, ref List<int> list2)
    {
        List<int> list1;
        list1 = new List<int>();

        int i = sourceState;
        while (i != 0)
        {
            list1.Insert(0, i);
            i = states[i].sourceKey;
        }
        list1.Insert(0, 0);

        i = targetState;
        while (i != 0)
        {
            list2.Insert(0, i);
            i = states[i].sourceKey;
        }
        list2.Insert(0, 0);

        i = 0;
        while (list1[i] == list2[i])    //elements different or last element of one list
        {
            if (i == list1.Count - 1 || i == list2.Count - 1)
                return list1[i];
            i++;
        }
        return list1[i - 1];
    }

    protected void StateChange(int source, int target)
    {
        if (target != NoTransition)
        {
            if (source != target)
            {
                List<int> targetList = new List<int>();
                int rootKey = findCommonRoot(source, target, ref targetList);
                while (currentState.Key != rootKey)
                {
                    lastState = currentState;
                    lastState.StateExit();
                    currentState = states[currentState.sourceKey];
                    if (currentState.Key == target)
                    {
                        currentState.StateEnter();          //Allows habitation of super-states by catching our 
                        return;                             //target state along the hierarchy towards the root
                    }
                }
                rootKey = targetList.IndexOf(rootKey) + 1;
                currentState = states[targetList[rootKey]];
                while (currentState.Key != target)
                {
                    currentState.StateEnter();
                    lastState = currentState;
                    rootKey++;
                    Debug.Log(targetList.Count);
                    Debug.Log(rootKey);
                    Debug.Log(states.Keys.Count);
                    currentState = states[targetList[rootKey]];
                }
            }
            else
            {
                currentState.StateExit();
            }
            currentState.StateEnter();
        }
    }

    //CharacterStates implement all of the callbacks of the kinematic controller so that each state is well-defined for the physical simulation
    protected abstract class CharacterState
    {
        protected CharacterMachine machine;
        protected CharacterState source;
        protected int KeyValue;
        public int Key { get { return KeyValue; } }
        public int sourceKey;

        protected CharacterState() { }
        protected CharacterState(CharacterMachine m)
        {
            machine = m;
        }
        public abstract void StateEnter();                  //When we change states, we must call each entry and exit method of each state						
        public abstract void StateExit();                   //traversed along the hierarchy to get to the new state. So if we are walking and

        //ICharacterController callbacks, implemented for each state and listed in the order of the simulation loop
        public abstract void BeforeCharacterUpdate(float deltaTime);
        public abstract void PostGroundingUpdate(float deltaTime);
        public abstract void UpdateRotation(ref Quaternion currentRotation, float deltaTime);
        public abstract void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime);
        public abstract void AfterCharacterUpdate(float deltaTime);     //State changes always happen here

        public abstract bool IsColliderValidForCollisions(Collider coll);
        public abstract void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport);
        public abstract void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport);
        public abstract void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport);
        public abstract void OnDiscreteCollisionDetected(Collider hitCollider);
    }

    protected class BaseState : CharacterState
    {
        public BaseState() { }
        public override void AfterCharacterUpdate(float deltaTime)
        {
            throw new System.NotImplementedException();
        }

        public override void BeforeCharacterUpdate(float deltaTime)
        {
            throw new System.NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool IsColliderValidForCollisions(Collider coll)
        {
            throw new System.NotImplementedException();
        }

        public override void OnDiscreteCollisionDetected(Collider hitCollider)
        {
            throw new System.NotImplementedException();
        }

        public override void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
            throw new System.NotImplementedException();
        }

        public override void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
            throw new System.NotImplementedException();
        }

        public override void PostGroundingUpdate(float deltaTime)
        {
            throw new System.NotImplementedException();
        }

        public override void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
        {
            throw new System.NotImplementedException();
        }

        public override void StateEnter()
        {
            throw new System.NotImplementedException();
        }

        public override void StateExit()
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            throw new System.NotImplementedException();
        }

        public override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            throw new System.NotImplementedException();
        }
    }
}
