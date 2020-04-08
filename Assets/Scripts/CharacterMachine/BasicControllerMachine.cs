using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;

public class BasicControllerMachine : CharacterMachine
{
    public const int Idle = 1;
    public const int Walking = 2;
    public const int Aerial = 3;

    [Header("Stable Movement")]
    public float MaxStableMoveSpeed = 10f;
    public float StableMovementSharpness = 15f;
    public float OrientationSharpness = 10f;

    [Header("Air Movement")]
    public float MaxAirMoveSpeed = 100f;
    public float AirAccelerationSpeed = 15f;
    public float Drag = 0.1f;

    [Header("Jumping")]
    public bool AllowJumpingWhenSliding = false;
    public float JumpUpSpeed = 10f;
    public float JumpScalableForwardSpeed = 10f;
    public float JumpPreGroundingGraceTime = 0f;
    public float JumpPostGroundingGraceTime = 0f;

    [Header("Misc")]
    public List<Collider> IgnoredColliders = new List<Collider>();
    public bool OrientTowardsGravity = false;
    public Vector3 Gravity = new Vector3(0, -30f, 0);
    public Transform MeshRoot;
    public Transform CameraFollowPoint;

    private Collider[] _probedColliders = new Collider[8];
    
    private bool _jumpConsumed = false;
    private bool _jumpedThisFrame = false;
    
    private float _timeSinceLastAbleToJump = 0f;
    private Vector3 _internalVelocityAdd = Vector3.zero;
    private bool _shouldBeCrouching = false;
    private bool _isCrouching = false;

    private Vector3 lastInnerNormal = Vector3.zero;
    private Vector3 lastOuterNormal = Vector3.zero;

    // Start is called before the first frame update
    protected void Start()
    {
        states.Add(Base, new BaseState());
        states.Add(Idle, new BasicIdleState(this));
        states.Add(Walking, new BasicWalkingState(this));
        states.Add(Aerial, new BasicAerialState(this));

        // Handle initial state
        //TransitionToState(CharacterState.Default);
        currentState = states[Idle];

        // Assign the characterController to the motor
        Motor.CharacterController = this;
    }

    public void Update()
    {
        //Debug.Log(currentState);
    }

    public override void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        base.UpdateRotation(ref currentRotation, deltaTime);
        if (_lookInputVector.sqrMagnitude > 0f && OrientationSharpness > 0f)
        {
            // Smoothly interpolate from current to target look direction
            Vector3 smoothedLookInputDirection = Vector3.Slerp(Motor.CharacterForward, _lookInputVector, 1 - Mathf.Exp(-OrientationSharpness * deltaTime)).normalized;

            // Set the current rotation (which will be used by the KinematicCharacterMotor)
            currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, Motor.CharacterUp);
        }
        if (OrientTowardsGravity)
        {
            // Rotate from current up to invert gravity
            currentRotation = Quaternion.FromToRotation((currentRotation * Vector3.up), -Gravity) * currentRotation;
        }
    }
    public override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        currentState.UpdateVelocity(ref currentVelocity, deltaTime);
        // Take into account additive velocity
        if (_internalVelocityAdd.sqrMagnitude > 0f)
        {
            currentVelocity += _internalVelocityAdd;
            _internalVelocityAdd = Vector3.zero;
        }
    }
    public override bool IsColliderValidForCollisions(Collider col)
    {
        if (IgnoredColliders.Count == 0)
        {
            return true;
        }

        if (IgnoredColliders.Contains(col))
        {
            return false;
        }
        return currentState.IsColliderValidForCollisions(col);
    }

    public void AddVelocity(Vector3 velocity)
    {
        _internalVelocityAdd += velocity;
    }

    private class BasicIdleState : CharacterState
    {
        BasicControllerMachine machine;

        private bool jump_transition = false;
        public BasicIdleState(BasicControllerMachine m)
        {
            machine = m;
            KeyValue = BasicControllerMachine.Idle;
            sourceKey = CharacterMachine.Base;
            source = machine.states[sourceKey];
        }

        protected void OnLanded()
        {
        
        }

        protected void OnLeaveStableGround()
        {
            
        }

        public override void StateEnter()
        {
        }

        public override void BeforeCharacterUpdate(float deltaTime)
        {}

        public override void PostGroundingUpdate(float deltaTime)
        {
            // Handle landing and leaving ground - most important thing here is to learn the Motor's API for grounding/stability 
            /*if (machine.Motor.GroundingStatus.IsStableOnGround && !machine.Motor.LastGroundingStatus.IsStableOnGround)
            {
                OnLanded();
            }
            else if (!machine.Motor.GroundingStatus.IsStableOnGround && machine.Motor.LastGroundingStatus.IsStableOnGround)
            {
                OnLeaveStableGround();
            }*/
        }

        public override void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {}

        public override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            Vector3 effectiveGroundNormal = machine.Motor.GroundingStatus.GroundNormal;

            // Handle jumping
            machine._jumpedThisFrame = false;
            machine._timeSinceJumpRequested += deltaTime;
            if (machine._jumpRequested)
            {
                // See if we actually are allowed to jump
                if (!machine._jumpConsumed && ((machine.AllowJumpingWhenSliding ? machine.Motor.GroundingStatus.FoundAnyGround : machine.Motor.GroundingStatus.IsStableOnGround) || machine._timeSinceLastAbleToJump <= machine.JumpPostGroundingGraceTime))
                {
                    // Calculate jump direction before ungrounding
                    Vector3 jumpDirection = machine.Motor.CharacterUp;
                    if (machine.Motor.GroundingStatus.FoundAnyGround && !machine.Motor.GroundingStatus.IsStableOnGround)
                    {
                        jumpDirection = machine.Motor.GroundingStatus.GroundNormal;
                    }

                    // Makes the character skip ground probing/snapping on its next update. 
                    // If this line weren't here, the character would remain snapped to the ground when trying to jump. Try commenting this line out and see.
                    machine.Motor.ForceUnground();
                    jump_transition = true;

                    // Add to the return velocity and reset jump state
                    currentVelocity += (jumpDirection * machine.JumpUpSpeed) - Vector3.Project(currentVelocity, machine.Motor.CharacterUp);
                    currentVelocity += (machine._moveInputVector * machine.JumpScalableForwardSpeed);
                    machine._jumpRequested = false;
                    machine._jumpConsumed = true;
                    machine._jumpedThisFrame = true;
                }
            }
        }

        public override void AfterCharacterUpdate(float deltaTime)
        {
            // Handle jump-related values
            {
                // Handle jumping pre-ground grace period
                if (machine._jumpRequested && machine._timeSinceJumpRequested > machine.JumpPreGroundingGraceTime)
                {
                    machine._jumpRequested = false;
                }

                if (machine.AllowJumpingWhenSliding ? machine.Motor.GroundingStatus.FoundAnyGround : machine.Motor.GroundingStatus.IsStableOnGround)
                {
                    // If we're on a ground surface, reset jumping values
                    if (!machine._jumpedThisFrame)
                    {
                        machine._jumpConsumed = false;
                    }
                    machine._timeSinceLastAbleToJump = 0f;
                }
                else
                {
                    // Keep track of time since we were last able to jump (for grace period)
                    machine._timeSinceLastAbleToJump += deltaTime;
                }
            }

            // Handle uncrouching
            if (machine._isCrouching && !machine._shouldBeCrouching)
            {
                // Do an overlap test with the character's standing height to see if there are any obstructions
                machine.Motor.SetCapsuleDimensions(0.5f, 2f, 1f);
                if (machine.Motor.CharacterOverlap(
                    machine.Motor.TransientPosition,
                    machine.Motor.TransientRotation,
                    machine._probedColliders,
                    machine.Motor.CollidableLayers,
                    QueryTriggerInteraction.Ignore) > 0)
                {
                    // If obstructions, just stick to crouching dimensions
                    machine.Motor.SetCapsuleDimensions(0.5f, 1f, 0.5f);
                }
                else
                {
                    // If no obstructions, uncrouch
                    machine.MeshRoot.localScale = new Vector3(1f, 1f, 1f);
                    machine._isCrouching = false;
                }
            }

            if (jump_transition)
            {
                machine.StateChange(KeyValue, BasicControllerMachine.Aerial);
                jump_transition = false;
                return;
            }
            if (machine.Motor.Velocity.magnitude > 0f || machine._moveInputVector.magnitude > 0f)
            {
                machine.StateChange(KeyValue, BasicControllerMachine.Walking);
                return;
            }
        }

        public override void StateExit()
        {}

        public override bool IsColliderValidForCollisions(Collider coll)
        {
            return true;
        }

        public override void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
        }

        public override void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
        }

        public override void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
        {
        }

        public override void OnDiscreteCollisionDetected(Collider hitCollider)
        {
        }
    }

    private class BasicWalkingState : CharacterState
    {
        BasicControllerMachine machine;
        bool jump_transition = false;

        public BasicWalkingState(BasicControllerMachine m)
        {
            machine = m;
            KeyValue = BasicControllerMachine.Walking;
            sourceKey = CharacterMachine.Base;
            source = machine.states[sourceKey];
        }

        public override void StateEnter()
        {
            machine.GetComponentInChildren<Animator>().SetBool("walk", true);
        }

        public override void BeforeCharacterUpdate(float deltaTime)
        {
        }

        public override void PostGroundingUpdate(float deltaTime)
        {

        }

        public override void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            if (machine._lookInputVector.sqrMagnitude > 0f && machine.OrientationSharpness > 0f)
            {
                // Smoothly interpolate from current to target look direction
                Vector3 smoothedLookInputDirection = Vector3.Slerp(machine.Motor.CharacterForward, machine._lookInputVector, 1 - Mathf.Exp(-machine.OrientationSharpness * deltaTime)).normalized;

                // Set the current rotation (which will be used by the KinematicCharacterMotor)
                currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, machine.Motor.CharacterUp);
            }
            if (machine.OrientTowardsGravity)
            {
                // Rotate from current up to invert gravity
                currentRotation = Quaternion.FromToRotation((currentRotation * Vector3.up), -machine.Gravity) * currentRotation;
            }
        }

        public override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            float currentVelocityMagnitude = currentVelocity.magnitude;

            Vector3 effectiveGroundNormal = machine.Motor.GroundingStatus.GroundNormal;

            if (machine.Motor.GroundingStatus.SnappingPrevented)
            {
                // Take the normal from where we're coming from
                Vector3 groundPointToCharacter = machine.Motor.TransientPosition - machine.Motor.GroundingStatus.GroundPoint;
                if (Vector3.Dot(currentVelocity, groundPointToCharacter) >= 0f)
                {
                    effectiveGroundNormal = machine.Motor.GroundingStatus.OuterGroundNormal;
                }
                else
                {
                    effectiveGroundNormal = machine.Motor.GroundingStatus.InnerGroundNormal;
                }
            }
                   
            // Reorient velocity on slope
            currentVelocity = machine.Motor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) * currentVelocityMagnitude;

            // Calculate target velocity
            Vector3 inputRight = Vector3.Cross(machine._moveInputVector, machine.Motor.CharacterUp);
            Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized * machine._moveInputVector.magnitude;
            Vector3 targetMovementVelocity = reorientedInput * machine.MaxStableMoveSpeed;

            // Smooth movement Velocity
            currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1f - Mathf.Exp(-machine.StableMovementSharpness * deltaTime));

            // Handle jumping
            machine._jumpedThisFrame = false;
            machine._timeSinceJumpRequested += deltaTime;
            if (machine._jumpRequested)
            {
                // See if we actually are allowed to jump
                if (!machine._jumpConsumed && ((machine.AllowJumpingWhenSliding ? machine.Motor.GroundingStatus.FoundAnyGround : machine.Motor.GroundingStatus.IsStableOnGround) || machine._timeSinceLastAbleToJump <= machine.JumpPostGroundingGraceTime))
                {
                    // Calculate jump direction before ungrounding
                    Vector3 jumpDirection = machine.Motor.CharacterUp;
                    if (machine.Motor.GroundingStatus.FoundAnyGround && !machine.Motor.GroundingStatus.IsStableOnGround)
                    {
                        jumpDirection = machine.Motor.GroundingStatus.GroundNormal;
                    }

                    // Makes the character skip ground probing/snapping on its next update. 
                    // If this line weren't here, the character would remain snapped to the ground when trying to jump. Try commenting this line out and see.
                    machine.Motor.ForceUnground();

                    // Add to the return velocity and reset jump state
                    currentVelocity += (jumpDirection * machine.JumpUpSpeed) - Vector3.Project(currentVelocity, machine.Motor.CharacterUp);
                    currentVelocity += (machine._moveInputVector * machine.JumpScalableForwardSpeed);
                    machine._jumpRequested = false;
                    machine._jumpConsumed = true;
                    machine._jumpedThisFrame = true;

                    jump_transition = true;
                }
            }
        }

        public override void AfterCharacterUpdate(float deltaTime)
        {
            // Handle jump-related values
            {
                // Handle jumping pre-ground grace period
                if (machine._jumpRequested && machine._timeSinceJumpRequested > machine.JumpPreGroundingGraceTime)
                {
                    machine._jumpRequested = false;
                }

                if (machine.AllowJumpingWhenSliding ? machine.Motor.GroundingStatus.FoundAnyGround : machine.Motor.GroundingStatus.IsStableOnGround)
                {
                    // If we're on a ground surface, reset jumping values
                    if (!machine._jumpedThisFrame)
                    {
                        machine._jumpConsumed = false;
                    }
                    machine._timeSinceLastAbleToJump = 0f;
                }
                else
                {
                    // Keep track of time since we were last able to jump (for grace period)
                    machine._timeSinceLastAbleToJump += deltaTime;
                }
            }

            if(jump_transition)
            {
                machine.StateChange(KeyValue, BasicControllerMachine.Aerial);
                jump_transition = false;
                return;
            }
            if(machine.Motor.Velocity.magnitude == 0)
            {
                machine.StateChange(KeyValue, BasicControllerMachine.Idle);
                return;
            }
        }

        public override void StateExit()
        {
            machine.GetComponentInChildren<Animator>().SetBool("walk", false);
        }

        public override bool IsColliderValidForCollisions(Collider coll)
        {
            if (machine.IgnoredColliders.Count == 0)
            {
                return true;
            }

            if (machine.IgnoredColliders.Contains(coll))
            {
                return false;
            }
            return true;
        }

        public override void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
        }

        public override void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
        }

        public override void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
        {
        }

        public override void OnDiscreteCollisionDetected(Collider hitCollider)
        {
        }
    }

    private class BasicAerialState : CharacterState
    {
        BasicControllerMachine machine;
        public BasicAerialState(BasicControllerMachine m)
        {
            machine = m;
            KeyValue = BasicControllerMachine.Aerial;
            sourceKey = CharacterMachine.Base;
            source = machine.states[sourceKey];
        }

        public override void StateEnter()
        {
            machine.GetComponentInChildren<Animator>().SetBool("jump", true);
        }

        public override void BeforeCharacterUpdate(float deltaTime)
        {
        }

        public override void PostGroundingUpdate(float deltaTime)
        {
        }

        public override void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            if (machine._lookInputVector.sqrMagnitude > 0f && machine.OrientationSharpness > 0f)
            {
                // Smoothly interpolate from current to target look direction
                Vector3 smoothedLookInputDirection = Vector3.Slerp(machine.Motor.CharacterForward, machine._lookInputVector, 1 - Mathf.Exp(-machine.OrientationSharpness * deltaTime)).normalized;

                // Set the current rotation (which will be used by the KinematicCharacterMotor)
                currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, machine.Motor.CharacterUp);
            }
            if (machine.OrientTowardsGravity)
            {
                // Rotate from current up to invert gravity
                currentRotation = Quaternion.FromToRotation((currentRotation * Vector3.up), -machine.Gravity) * currentRotation;
            }
        }

        public override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            // Add move input
            if (machine._moveInputVector.sqrMagnitude > 0f)
            {
                Vector3 addedVelocity = machine._moveInputVector * machine.AirAccelerationSpeed * deltaTime;

                // Prevent air movement from making you move up steep sloped walls
                if (machine.Motor.GroundingStatus.FoundAnyGround)
                {
                    Vector3 perpenticularObstructionNormal = Vector3.Cross(Vector3.Cross(machine.Motor.CharacterUp, machine.Motor.GroundingStatus.GroundNormal), machine.Motor.CharacterUp).normalized;
                    addedVelocity = Vector3.ProjectOnPlane(addedVelocity, perpenticularObstructionNormal);
                }

                // Limit air movement from inputs to a certain maximum, without limiting the total air move speed from momentum, gravity or other forces
                Vector3 resultingVelOnInputsPlane = Vector3.ProjectOnPlane(currentVelocity + addedVelocity, machine.Motor.CharacterUp);
                if (resultingVelOnInputsPlane.magnitude > machine.MaxAirMoveSpeed && Vector3.Dot(machine._moveInputVector, resultingVelOnInputsPlane) >= 0f)
                {
                    addedVelocity = Vector3.zero;
                }
                else
                {
                    Vector3 velOnInputsPlane = Vector3.ProjectOnPlane(currentVelocity, machine.Motor.CharacterUp);
                    Vector3 clampedResultingVelOnInputsPlane = Vector3.ClampMagnitude(resultingVelOnInputsPlane, machine.MaxAirMoveSpeed);
                    addedVelocity = clampedResultingVelOnInputsPlane - velOnInputsPlane;
                }

                currentVelocity += addedVelocity;
            }

            // Gravity
            currentVelocity += machine.Gravity * deltaTime;

            // Drag
            currentVelocity *= (1f / (1f + (machine.Drag * deltaTime)));
            machine.GetComponentInChildren<Animator>().SetBool("falling", currentVelocity.y <= 0);
        }

        public override void AfterCharacterUpdate(float deltaTime)
        {
            // Handle jump-related values
            {
                // Handle jumping pre-ground grace period
                if (machine._jumpRequested && machine._timeSinceJumpRequested > machine.JumpPreGroundingGraceTime)
                {
                    machine._jumpRequested = false;
                }

                if (machine.AllowJumpingWhenSliding ? machine.Motor.GroundingStatus.FoundAnyGround : machine.Motor.GroundingStatus.IsStableOnGround)
                {
                    // If we're on a ground surface, reset jumping values
                    if (!machine._jumpedThisFrame)
                    {
                        machine._jumpConsumed = false;
                    }
                    machine._timeSinceLastAbleToJump = 0f;
                }
                else
                {
                    // Keep track of time since we were last able to jump (for grace period)
                    machine._timeSinceLastAbleToJump += deltaTime;
                }
            }
            if (machine.Motor.GroundingStatus.IsStableOnGround)
                machine.StateChange(KeyValue, Idle);
        }

        public override void StateExit()
        {
            machine.GetComponentInChildren<Animator>().SetBool("jump", false);
        }

        public override bool IsColliderValidForCollisions(Collider coll)
        {
            if (machine.IgnoredColliders.Count == 0)
            {
                return true;
            }

            if (machine.IgnoredColliders.Contains(coll))
            {
                return false;
            }
            return true;
        }

        public override void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
        }

        public override void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
        }

        public override void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
        {
        }

        public override void OnDiscreteCollisionDetected(Collider hitCollider)
        {
        }
    }


}
