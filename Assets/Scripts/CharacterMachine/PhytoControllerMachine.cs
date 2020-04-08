using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using KinematicCharacterController;

public class PhytoControllerMachine : CharacterMachine
{
    public struct PhytoCharacterInputs
    {
        public float MoveAxisForward;
        public float MoveAxisRight;
        public Quaternion CameraRotation;
        public bool JumpDown;
        public bool JumpPressed;
    }
    public PhytoCharacterInputs inputs;

    public Animator CharacterAnimator;
    private Vector3 _rootMotionPositionDelta = Vector3.zero;
    private Quaternion _rootMotionRotationDelta = Quaternion.identity;

    public const int Idle = 1;
    public const int Walking = 2;
    public const int Jumping = 3;
    public const int Climbing = 4;

    [System.Serializable]
    public class DoubleJumpEvent : UnityEvent<float>
    {

    }
    public DoubleJumpEvent DoubleJump;

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

    public const float PRESSURE_SENSITIVE_JUMP_BUFFER = 0.2f;

    [Header("Misc")]
    public List<Collider> IgnoredColliders = new List<Collider>();
    public bool OrientTowardsGravity = false;
    public Vector3 Gravity = new Vector3(0, -30f, 0);
    public Transform MeshRoot;
    public Transform CameraFollowPoint;

    //Used for probing character collisions - 8 is an arbitrary number of colliders to track during simulation
    private Collider[] _probedColliders = new Collider[8];
    private RaycastHit[] _raycastHits = new RaycastHit[8];
    public LayerMask DefaultLayer, ClimbingLayer;

    private float PressureSensitiveJumpPower;
    private bool _jumpBuffered = false;
    private bool _jumpConsumed = false, _doubleJumpConsumed = false;
    private bool _jumpedThisFrame = false;

    private float _timeSinceLastAbleToJump = 0f;
    private Vector3 _internalVelocityAdd = Vector3.zero;
    
    private Vector3 lastInnerNormal = Vector3.zero;
    private Vector3 lastOuterNormal = Vector3.zero;

    // Start is called before the first frame update
    protected void Start()
    {
        states.Add(Base, new BaseState());
        states.Add(Idle, new PhytoIdleState(this));
        states.Add(Walking, new PhytoWalkingState(this));
        states.Add(Jumping, new PhytoJumpingState(this));
        //states.Add(Climbing, new PhytoClimbingState(this));

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

    /*protected void OnAnimatorMove()
    {
        // Accumulate rootMotion deltas between character updates 
        _rootMotionPositionDelta += CharacterAnimator.deltaPosition;
        _rootMotionRotationDelta = CharacterAnimator.deltaRotation * _rootMotionRotationDelta;
    }*/

    public void SetInputs(ref PhytoCharacterInputs inputs)
    {
        this.inputs = inputs;
        // Clamp input
        Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(inputs.MoveAxisRight, 0f, inputs.MoveAxisForward), 1f);
        this.GetComponentInChildren<Animator>().SetFloat("input", moveInputVector.magnitude);

        // Calculate camera direction and rotation on the character plane
        Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.forward, Motor.CharacterUp).normalized;
        if (cameraPlanarDirection.sqrMagnitude == 0f)
        {
            cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.up, Vector3.up).normalized;
        }
        Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, Vector3.up);

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
            _jumpBuffered = true;
        }

        if(_jumpBuffered)
        {
            _timeSinceJumpRequested += Time.deltaTime;
            if (inputs.JumpPressed)
            {
                if (_timeSinceJumpRequested > PRESSURE_SENSITIVE_JUMP_BUFFER)
                {
                    _jumpRequested = true;
                    _jumpBuffered = false;
                    PressureSensitiveJumpPower = 1f;
                }
            }
            else
            {
                PressureSensitiveJumpPower = _timeSinceJumpRequested / PRESSURE_SENSITIVE_JUMP_BUFFER;
                _jumpRequested = true;
                _jumpBuffered = false;
            }
        }
    }

    public override void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        /*if(currentState.Key != Climbing)
        {
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
        }*/
        currentState.UpdateRotation(ref currentRotation, deltaTime);
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

    //Universal post-jumping logic for reseting the relevant state variables
    public void ResetJumpLogic(float deltaTime)
    {
        
    }

    private class PhytoIdleState : CharacterState
    {
        PhytoControllerMachine machine;

        private bool jump_transition = false;
        public PhytoIdleState(PhytoControllerMachine m)
        {
            machine = m;
            KeyValue = PhytoControllerMachine.Idle;
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
            //Debug.Log("Idling");
        }

        public override void BeforeCharacterUpdate(float deltaTime)
        {
            /*int num_overlaps = machine.Motor.CharacterOverlap(machine.Motor.TransientPosition, machine.Motor.TransientRotation, machine._probedColliders, machine.DefaultLayer, QueryTriggerInteraction.Collide);
            if (num_overlaps > 0)
            {
                for (int i = 0; i < num_overlaps; i++)
                {
                    Parametric_L_System p_l_sys = machine._probedColliders[i].GetComponent<Parametric_L_System>();
                    if (p_l_sys != null)
                    {
                        p_l_sys.GetComponentInChildren<Parametric_Turtle>().TurtleAnalysis(Time.deltaTime);
                        p_l_sys.Age += (Time.deltaTime);
                    }
                }
            }*/
        }

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
        { }

        public override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            Vector3 effectiveGroundNormal = machine.Motor.GroundingStatus.GroundNormal;

            if (machine._jumpRequested)
            {
                // See if we actually are allowed to jump
                if (!machine._jumpConsumed && ((machine.AllowJumpingWhenSliding ? machine.Motor.GroundingStatus.FoundAnyGround : machine.Motor.GroundingStatus.IsStableOnGround) || machine._timeSinceLastAbleToJump <= machine.JumpPostGroundingGraceTime))
                {
                    jump_transition = true;
                }
            }
        }

        public override void AfterCharacterUpdate(float deltaTime)
        {
            //machine.ResetJumpLogic(deltaTime);
            if (jump_transition)
            {
                machine.StateChange(KeyValue, PhytoControllerMachine.Jumping);
                jump_transition = false;
                return;
            }
            if (machine.Motor.Velocity.magnitude > 0f || machine._moveInputVector.magnitude > 0f)
            {
                machine.StateChange(KeyValue, PhytoControllerMachine.Walking);
                return;
            }
        }

        public override void StateExit()
        { }

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

    private class PhytoWalkingState : CharacterState
    {
        PhytoControllerMachine machine;
        bool jump_transition = false;

        public PhytoWalkingState(PhytoControllerMachine m)
        {
            machine = m;
            KeyValue = PhytoControllerMachine.Walking;
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
                    jump_transition = true;
                }
            }
        }

        public override void AfterCharacterUpdate(float deltaTime)
        {
            machine.ResetJumpLogic(deltaTime);

            if (jump_transition)
            {
                machine.StateChange(KeyValue, PhytoControllerMachine.Jumping);
                jump_transition = false;
                return;
            }
            if (machine.Motor.Velocity.magnitude == 0)
            {
                machine.StateChange(KeyValue, PhytoControllerMachine.Idle);
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

    private class PhytoJumpingState : CharacterState
    {
        PhytoControllerMachine machine;

        bool can_climb = false;
        float double_jump_rotation_angle = 0f;
        float peak_vertical_velocity = 0f;
        float start_euler_y = 0f;
        Vector3 jump_direction;

        public PhytoJumpingState(PhytoControllerMachine m)
        {
            machine = m;
            KeyValue = PhytoControllerMachine.Jumping;
            sourceKey = CharacterMachine.Base;
            source = machine.states[sourceKey];
        }

        public override void StateEnter()
        {
            machine.GetComponentInChildren<Animator>().SetBool("jump", true);
            can_climb = false;
        }

        public override void BeforeCharacterUpdate(float deltaTime)
        {
            if(machine._jumpRequested && !machine._jumpConsumed)
            {
                // Calculate jump direction before ungrounding
                jump_direction = machine.Motor.CharacterUp;
                if (machine.Motor.GroundingStatus.FoundAnyGround && !machine.Motor.GroundingStatus.IsStableOnGround)
                {
                    jump_direction = machine.Motor.GroundingStatus.GroundNormal;
                }
                // Makes the character skip ground probing/snapping on its next update. 
                // If this line weren't here, the character would remain snapped to the ground when trying to jump. Try commenting this line out and see.
                machine.Motor.ForceUnground();
            }
        }

        public override void PostGroundingUpdate(float deltaTime)
        {}

        public override void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            /*if (machine._lookInputVector.sqrMagnitude > 0f && machine.OrientationSharpness > 0f)
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
            }*/
            //currentRotation = machine._rootMotionRotationDelta * currentRotation;
            if(machine._doubleJumpConsumed)
            {
                currentRotation = Quaternion.Euler(currentRotation.eulerAngles.x, start_euler_y + Mathf.Lerp(0, 360f, 1 - machine.Motor.Velocity.y/peak_vertical_velocity), currentRotation.eulerAngles.z);
            }
        }

        public override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            if(machine._jumpRequested)
            {
                if (!machine._jumpConsumed)
                {
                    // Add to the return velocity and reset jump state
                    currentVelocity += (jump_direction * machine.JumpUpSpeed * machine.PressureSensitiveJumpPower) - Vector3.Project(currentVelocity, machine.Motor.CharacterUp);
                    currentVelocity += (machine._moveInputVector * machine.JumpScalableForwardSpeed);
                    machine._jumpRequested = false;
                    machine._jumpConsumed = true;
                    machine._jumpedThisFrame = true;
                }
                else if (!machine._doubleJumpConsumed)
                {
                    machine.GetComponentInChildren<Animator>().SetTrigger("double_jump");
                    Vector3 jumpDirection = machine.Motor.CharacterUp;
                    currentVelocity += (jump_direction * machine.JumpUpSpeed * machine.PressureSensitiveJumpPower) - Vector3.Project(currentVelocity, machine.Motor.CharacterUp);
                    currentVelocity += (machine._moveInputVector * machine.JumpScalableForwardSpeed);
                    
                    machine._jumpRequested = false;
                    machine._doubleJumpConsumed = true;
                    machine._jumpedThisFrame = true;

                    peak_vertical_velocity = currentVelocity.y;
                    double_jump_rotation_angle = 360f;// / (peak_vertical_velocity / (machine.Drag * machine.Gravity.y));
                    start_euler_y = machine.Motor.TransientRotation.eulerAngles.y;

                    //Invoke the double jump event with the total time before the character peaks
                    machine.DoubleJump.Invoke(peak_vertical_velocity);
                }
                else
                {
                    // Handle jumping
                    machine._jumpedThisFrame = false;
                    machine._timeSinceJumpRequested += deltaTime;
                }
            }
            
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
            machine.GetComponent<Animator>().SetFloat("vertical", currentVelocity.y / peak_vertical_velocity);
        }

        public override void AfterCharacterUpdate(float deltaTime)
        {
            // Handle jumping pre-ground grace period
            if (machine._jumpRequested && machine._timeSinceJumpRequested > machine.JumpPreGroundingGraceTime)
            {
                machine._jumpRequested = false;
                machine._jumpBuffered = false;
            }

            if (machine.Motor.GroundingStatus.IsStableOnGround && !machine._jumpRequested)
            {
                // If we're on a ground surface, reset jumping values
                machine._jumpConsumed = false;
                machine._doubleJumpConsumed = false;
                machine._timeSinceLastAbleToJump = 0f;

                machine.StateChange(KeyValue, Idle);
            }
            //else if (can_climb)
            //    machine.StateChange(KeyValue, Climbing);
            else
                machine._timeSinceLastAbleToJump += deltaTime;
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
            if (hitCollider.gameObject.layer == LayerMask.NameToLayer("Plant"))
            {
                if(Vector3.Angle(hitNormal, Vector3.up) > machine.Motor.MaxStableDenivelationAngle)
                {
                    machine.climbing_surface = hitCollider;
                    machine.surface_normal = hitNormal;
                    can_climb = true;
                }
            }

        }

        public override void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
        {
        }

        public override void OnDiscreteCollisionDetected(Collider hitCollider)
        {
        }
    }

    private Collider climbing_surface;
    private Vector3 surface_normal;
    private Vector3 surface_point;
    private class PhytoClimbingState : CharacterState
    {
        PhytoControllerMachine machine;
        private bool hanging = false;
        private bool jump_transition = false;
        private bool standing_transition = false;
        private bool hanging_transition = false;

        private bool lost_surface = false;

        private const float CLIMB_SPEED = 1.0f;

        private Vector3 capsule_point;

        public PhytoClimbingState(PhytoControllerMachine m)
        {
            machine = m;
            KeyValue = PhytoControllerMachine.Climbing;
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
            jump_transition = false;
            standing_transition = false;

            machine._timeSinceLastAbleToJump = 0f;  //Reset our jump buffer
            machine._jumpConsumed = false;

            machine.GetComponentInChildren<Animator>().SetBool("climbing", true);
            machine.climbing_surface = null;
            
        }

        public override void BeforeCharacterUpdate(float deltaTime)
        {
            Vector3 cast_direction;
            Vector3 cast_postion = machine.Motor.TransientPosition;
            if (machine.climbing_surface == null)
            {
                if(hanging)
                {
                    cast_direction = machine.Motor.CharacterUp;
                }
                else
                    cast_direction = machine.Motor.CharacterForward;
            }
            else
                cast_direction = -machine.surface_normal;
            
            //This is likely totally unnecessary (as a method for retrieving the normal vector)
            int hits = machine.Motor.CharacterSweep(cast_postion, machine.Motor.TransientRotation,
                cast_direction, 0.05f, out RaycastHit closestHit, machine._raycastHits, 
                machine.ClimbingLayer, QueryTriggerInteraction.Collide);

            if(hits < 1)
            {
                //Need a mechanism to clamp to our surface
                lost_surface = true;
                Debug.Log("No surface");
            }
            else
            {
                lost_surface = false;
                machine.climbing_surface = closestHit.collider;
                machine.surface_normal = closestHit.normal;
                machine.surface_point = closestHit.point;
                capsule_point = machine.GetComponent<CapsuleCollider>().ClosestPointOnBounds(machine.surface_point);
            }
            
            /*if (hits > 0)
            {
                for (int i = 0; i < hits; i++)
                {
                    Debug.DrawRay(machine._raycastHits[i].point, machine._raycastHits[i].normal, Color.red, 30f);

                }
                Debug.DrawRay(machine.Motor.TransientPosition + machine.Motor.CharacterTransformToCapsuleCenter,
                    machine.surface_normal, Color.white, 30f);
            }
            else
                Debug.Log(machine.climbing_surface);*/
            
            float nivellationAngle = Vector3.Angle(Vector3.up, machine.surface_normal);
            Debug.Log(nivellationAngle);
            if (nivellationAngle > 0)
            {
                if (nivellationAngle < machine.Motor.MaxStableDenivelationAngle)
                {
                    standing_transition = true;

                }
                else if (nivellationAngle > 180 - machine.Motor.MaxStableDenivelationAngle)
                {
                    Debug.Log("Climbing to hang");
                    hanging = true;
                    machine.GetComponentInChildren<Animator>().SetBool("hanging", true);
                }
                else if (hanging)
                {
                    Debug.Log("Hanging to climb");
                    hanging = false;
                    hanging_transition = true;

                    machine.GetComponentInChildren<Animator>().SetBool("hanging", false);
                    Debug.DrawRay(machine.climbing_surface.ClosestPoint(machine.Motor.TransientPosition), machine.surface_normal, Color.gray, 10f);
                    //Debug.Break();
                }
                else if (hanging_transition)
                {
                    if(nivellationAngle < 90 + machine.Motor.MaxStableDenivelationAngle)
                    {
                        hanging_transition = false;
                    }
                }

                Debug.DrawRay(machine.climbing_surface.transform.position, machine.surface_normal, Color.gray, 15f);
            }
            else    //Typically this means we've exited the surface, which we need to prevent
            { }
        }

        public override void PostGroundingUpdate(float deltaTime)
        {

        }

        public override void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            if(hanging)
            {
                if (machine._lookInputVector.sqrMagnitude > 0f && machine.OrientationSharpness > 0f)
                {
                    Debug.DrawRay(machine.Motor.TransientPosition, machine._lookInputVector, Color.red, 10f);
                    // Smoothly interpolate from current to target look direction
                    Vector3 smoothedLookInputDirection = Vector3.Slerp(machine.Motor.CharacterForward, machine._lookInputVector, 1 - Mathf.Exp(-machine.OrientationSharpness * deltaTime)).normalized;

                    // Set the current rotation (which will be used by the KinematicCharacterMotor)
                    currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, Vector3.up);
                }
                //currentRotation = Quaternion.LookRotation(machine.Motor.CharacterForward, Vector3.up);
            }
            else if(hanging_transition)
            {
                //Ultimately we would want to play an animation here
            }
            else
            {
                currentRotation = Quaternion.LookRotation(-machine.surface_normal, machine.Motor.CharacterUp);
            }
        }

        public override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            machine._jumpedThisFrame = false;
            machine._timeSinceJumpRequested += deltaTime;
            if (machine._jumpRequested)
            {
                // See if we actually are allowed to jump - ignore logic related to grounding
                if (!machine._jumpConsumed && (machine._timeSinceLastAbleToJump <= machine.JumpPostGroundingGraceTime))
                {
                    // Calculate jump direction before ungrounding
                    Vector3 jumpDirection = machine.surface_normal;

                    // Makes the character skip ground probing/snapping on its next update. 
                    // If this line weren't here, the character would remain snapped to the ground when trying to jump. Try commenting this line out and see.
                    machine.Motor.ForceUnground();
                    jump_transition = true;

                    // Add to the return velocity and reset jump state
                    currentVelocity += (machine.Motor.CharacterUp * machine.JumpUpSpeed) - Vector3.Project(currentVelocity, machine.Motor.CharacterUp);
                    currentVelocity += (jumpDirection * machine.JumpScalableForwardSpeed);
                    machine._jumpRequested = false;
                    machine._jumpConsumed = true;
                    machine._jumpedThisFrame = true;
                }
            }
            else
            {
                if (!hanging_transition)
                {
                    float surfaceOrient = -Vector3.Angle(machine.surface_normal, machine.Motor.CharacterUp);
                    //if (Mathf.Abs(surfaceOrient) < -90)
                    //    surfaceOrient = -Vector3.Angle(-machine.surface_normal, machine.Motor.CharacterUp);
                    Vector3 planeVec = Quaternion.AngleAxis(surfaceOrient, machine.Motor.CharacterRight) * machine._moveInputVector;
                    currentVelocity = planeVec.normalized * CLIMB_SPEED;
                }

                if (lost_surface)
                {
                    currentVelocity += machine.Motor.GetVelocityForMovePosition(machine.Motor.TransientPosition,
                            machine.Motor.TransientPosition+Vector3.up, deltaTime);
                }
            } 
        }

        public override void AfterCharacterUpdate(float deltaTime)
        {
            //machine.ResetJumpLogic(deltaTime);
            if(jump_transition)
            {
                Debug.Log("Attempting to jump from climb");
                machine.StateChange(KeyValue, Jumping);
            }
            else if(standing_transition)
            {
                Debug.Log("Mounting stable surface from climb");
                machine.StateChange(KeyValue, Idle);
            }
                
        }

        public override void StateExit()
        {
            machine.GetComponentInChildren<Animator>().SetBool("climbing", false);
        }

        public override bool IsColliderValidForCollisions(Collider coll)
        {
            return true;
        }

        public override void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
            //This would be a good place to implement logic for stepping off a climbing surface
            Debug.Log("Ground Hit");
            standing_transition = true;
        }

        public override void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
            Debug.Log("Hit");
        }

        public override void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
        {
        }

        public override void OnDiscreteCollisionDetected(Collider hitCollider)
        {
        }
    }


}
