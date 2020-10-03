using Godot;
using System;
using EventCallback;

public class PlayerBody : KinematicBody
{
    //= Grappling hook variables ==================================================================
    //The point where the grappling hook has hooked
    Vector3 hookPoint = new Vector3();
    //If a hook point is returned
    bool hasHookPoint = false;
    //The raycast that picks up if the player has the ceiling above it
    RayCast ceilingRaycast;
    //The grounds raycast
    RayCast groundRay;
    //The raycast for thte grapple 
    RayCast grappleRay;
    //The emediate geometryto draw the grapple line
    DrawGrappleLine grappleLine;
    //=============================================================================================
    //= Crouching variables =======================================================================
    //Movement varaibles below in the movement variables code, only other crouch spisific variables here

    //The collision shape for the capsule, modified for the crouching behaviour 
    CollisionShape bodyCollShape;
    //=============================================================================================
    //= New movement variables ====================================================================
    //The direction of travel
    Vector3 direction = new Vector3();
    //The velocity at whitch the player is traveling
    Vector3 velocity = new Vector3();
    //The spatiol object repereseting the gimble of the camera
    Spatial cameraGimbal;
    //The instance of the camera
    Camera camera;
    //= The speeds for all movement ===============================================================
    //The force of the jump
    float jumpSpeed = 5f;
    //Force of a jump if the grapple is in use
    float grappleJumpSpeed = 7f;
    //How hard the gravity pulls down on the player
    float gravity = 9.8f;
    //The vector for the gravity calculations
    Vector3 gravityVec = new Vector3();
    //The speed at witch the object picks up speed
    float acceleration = 5f;
    //acceleration for the grapple
    float grappleAcceleration = 16;
    //The speed at witch the the object losses speed
    float deacceleration = 16f;
    //The maximum slope that can be moved up
    float maxSlopeAngle = 45;
    //How hight the player characters camera displayes when standing
    float defualtHeight = 1.5f;
    //How high the players character camera is when it is crouvhed
    float crouchHeight = .5f;
    //The sensitivity of the mouse, how fast it turns
    float mouseSensitivity = 0.1f;
    //The maximum walk speed
    float maxWalkSpeed = 7;
    //How sensitive the movement of the mouse is
    float maxSprintSpeed = 15;
    //The maximum crouch movement speed the player has
    float maxCrouchingSpeed = 4;
    //The speed at with the player crouches, goes into a crouching position
    float maxCrouchSpeed = 3;
    //The speed at with the player is pulle to the hook of the grapple
    float maxGrappleSpeed = 5;
    //=============================================================================================
    //= Statuses of the movement of the player ====================================================
    //If the player is sprinting
    bool isSprinting = false;
    //If the player is crouching
    bool isCrouching = false;
    //The player can jump now
    bool canJump = false;
    //Check if the player has pressed the jump button once already
    bool hasJumped = false;
    //If we are gliding it is set to true
    bool isGliding = false;
    //If the grappling hook has been fired
    bool grappleActive = false;
    //If the player has reached the hook point for the grappling hook
    bool reachedHookPoint = false;
    //If the players head is touching the cieling, used for crouch stuff
    bool isCollidingWithCeiling = false;
    //Is the ground raycast in contact with the ground
    bool groundContact = false;
    //=============================================================================================

    bool moveForward = false, moveBackward = false, strafeLeft = false, strafeRight = false, sprint = false, jump = false, crouch = false, graple = false;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        InputHandleEvent.RegisterListener(grabInput);
        //Make the mouse invisible and not able to leave the window to capture it's movement
        Input.SetMouseMode(Input.MouseMode.Captured);
        //The shape of thte players collision shape
        bodyCollShape = GetNode<CollisionShape>("BodyCollisionShape");

        //The cameras movement gimbal, used for looking mechanics
        grappleRay = GetNode<RayCast>("CameraGimbal/Camera/GrappleRay");
        //Set the ground ray to the raycast in scene
        groundRay = GetNode<RayCast>("GroundRay");
        //The raycast on the player body that detects cieling collisions
        ceilingRaycast = GetNode<RayCast>("CeilingCollisionRay");
        //Grab the refference to the cameras gimbals
        cameraGimbal = GetNode<Spatial>("CameraGimbal");
        //Grab the refference to the camera
        camera = GetNode<Camera>("CameraGimbal/Camera");
        //Set the grappel line to the emediate geometry object on the player
        grappleLine = GetNode<DrawGrappleLine>("GrappleLine");
    }

    private void grabInput(InputHandleEvent ihei)
    {
        moveForward = ihei.upPressed;
        moveBackward = ihei.downPressed;
        strafeLeft = ihei.leftPressed;
        strafeRight = ihei.rightPressed;
        sprint = ihei.sprintPressed;
        jump = ihei.jumpPressed;
        crouch = ihei.crouchPressed;
        graple = ihei.abilityPressed;
    }
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(float delta)
    {
        //Grabs the input 
        ProcessInput(delta);
        //Processes the input and moves the charecter
        PrecessMovement(delta);
    }

    private void ProcessInput(float delta)
    {
        //Check if the head ray collider is active
        if (ceilingRaycast.IsColliding()) isCollidingWithCeiling = true;
        //Check if the ground ray is colliding
        if (groundRay.IsColliding()) groundContact = true;
        else groundContact = false;
        //The initial move direction for the player
        direction = new Vector3();
        //The reference to the cameras global transformw
        Transform camTransform = camera.GlobalTransform;
        //Get the movement vector here 
        Vector2 inputMovementVector = new Vector2();
        //Check what movement buttons are pressed
        if (Input.IsActionPressed("MoveForward"))
            inputMovementVector.y += 1;
        else if (Input.IsActionPressed("MoveBackward"))
            inputMovementVector.y -= 1;
        if (Input.IsActionPressed("StrafeLeft"))
            inputMovementVector.x -= 1;
        else if (Input.IsActionPressed("StrafeRight"))
            inputMovementVector.x += 1;
        //Normalize the input vector to not get faster movement when moving diagonally
        inputMovementVector = inputMovementVector.Normalized();
        //set the direction using the cameras transform basis multiplied with the input values
        direction += -camTransform.basis.z * inputMovementVector.y;
        direction += camTransform.basis.x * inputMovementVector.x;
        //If the player is on the floor then set the hasjumped and isGliding checks to false
        //Can't think of a better way to do this now than brute forcing it just before the Jump button check
        if (IsOnFloor())
        {
            hasJumped = false;
            isGliding = false;
            reachedHookPoint = false;
        }
        //If we press jump this method is called once
        if (Input.IsActionJustPressed("Jump"))
        {
            //If the player is on the floor
            if ((IsOnFloor() || reachedHookPoint || groundRay.IsColliding()) && !isGliding)
            {
                //Set jump to true so the player jumps in the process fynction
                jump = true;
                //We set the hasJumped to true
                hasJumped = true;
                //Disable the hook point from the graple
                grappleActive = false;
                hasHookPoint = false;
                reachedHookPoint = false;
            }
            else
            {
                //If the player is not on thet floow and is gliding and 
                if (hasJumped)
                {
                    //Togle gliding on or of
                    isGliding = !isGliding;
                }
            }
        }
        //  Capturing/Freeing the cursor
        if (Input.IsActionJustPressed("ui_cancel"))
        {
            if (Input.GetMouseMode() == Input.MouseMode.Visible)
                Input.SetMouseMode(Input.MouseMode.Captured);
            else
                Input.SetMouseMode(Input.MouseMode.Visible);
        }
        //If the sprint button is pressed we set sprinting to true
        if (Input.IsActionPressed("Sprint")) isSprinting = true;
        else isSprinting = false;
        //If the player is crouching and the collision shape is a capsule then 
        if (Input.IsActionPressed("Crouch") && bodyCollShape.Shape is CapsuleShape capShape && !hasHookPoint && !isGliding)
        {
            capShape.Height -= maxCrouchSpeed * delta;
            isCrouching = true;
        }
        else if (!isCollidingWithCeiling)
        {
            ((CapsuleShape)bodyCollShape.Shape).Height += maxCrouchSpeed * delta;
            isCrouching = false;
        }
        //Clamp the max and min height for crouching when it is being modified
        ((CapsuleShape)bodyCollShape.Shape).Height = Mathf.Clamp(((CapsuleShape)bodyCollShape.Shape).Height, crouchHeight, defualtHeight);
        //If the player presses the ability key for hte grapple
        if (Input.IsActionJustPressed("Ability"))
        {
            //If the grapple ray is colliding with an object
            if (grappleRay.IsColliding())
            {
                //If the grapple is off
                if (!grappleActive)
                {
                    //Set the grapple active
                    grappleActive = true;
                }
            }
            //If the grapple is active and the the hook point is there then we cut the grapple 
            if (grappleActive && hasHookPoint)
            {
                grappleActive = false;
                hasHookPoint = false;
                reachedHookPoint = false;
                hookPoint = new Vector3();
            }
        }
    }

    private void PrecessMovement(float delta)
    {

        //Set the directions y to zero as the jump physics will be added later
        direction.y = 0;
        //Normalize the direction
        direction = direction.Normalized();
        //The lift calculated that will be applied to the gravity calculations
        float lift = 0;
        //===============================================================================================================
        //Grapling script to work on later
        if (grappleActive)
        {
            //Check if we already have a hook point for the grapple
            if (!hasHookPoint)
            {
                //If we don't have a hook point yet then we check if the raycast is colliding with something valid
                //If the raycast is colliding we set the hookPoint that point 
                hookPoint = grappleRay.GetCollisionPoint();
                //We tell the function we now have a hook point so we don't get a new one in the next interation of the loop
                hasHookPoint = true;
            }
        }
        //We check if we are colliding with hte ceiling
        if (ceilingRaycast.IsColliding())
        {
            //If we hit the ceiling with our head we set the graplpling to false to disconect the grapple line
            grappleActive = false;
            //We reset the hookPoint 
            hookPoint = new Vector3();
            //We set the hookPointGet
            hasHookPoint = false;
            GlobalTranslate(new Vector3(0, -1, 0));
            hookPoint = new Vector3();
        }
        //===============================================================================================================

        //If we were gliding but are now on the ground we disable the isGliding
        if (isGliding)
        {
            //We get the magnitude of the vector as a float (speed) to add toe the gravity calculations
            lift = new Vector2(velocity.x, velocity.z).Length();
            //Clamp the lift float to not give to much lift
            lift = Mathf.Clamp(lift, 0, 14);
        }
        //We add the gravity to the velocities y axis
        //velocity.y += delta * (gravity + lift);

        if (!IsOnFloor())
        {
            gravityVec += Vector3.Down * gravity * delta;
        }
        else if (IsOnFloor() && groundContact)
        {
            gravityVec = -GetFloorNormal() * gravity;
        }
        else
        {
            gravityVec = -GetFloorNormal();
        }

        if (jump)
        {
            if (reachedHookPoint)
            {
                //We set the velocity to the jump velocity puls a little bit extra to get over a ledge if the graple hook was used
                //velocity.y = grappleJumpSpeed;
                gravityVec = Vector3.Up * grappleJumpSpeed;
            }
            else
            {
                //We set the velocity to the jump velocity
                //velocity.y = jumpSpeed;
                gravityVec = Vector3.Up * jumpSpeed;
            }
            //Set to false after we have jumped
            jump = false;
        }
        //we set the velocity to a temporary velocity hvel to add some pysics work
        Vector3 hvel = velocity;
        //We make sure that the tem horizontal velocities y axis for jumping is set to zero; 
        hvel.y = 0;
        //we create another temporary vector3 to us for the movement speed calculations
        Vector3 target = direction;
        //If the grapple has hooked a valid surface
        if (hasHookPoint)
        {
            //Set gravity to 0 to travel ot hooked point smoothly
            velocity.y = 0;
            Transform = new Transform(Transform.basis, Transform.origin.LinearInterpolate(hookPoint, maxGrappleSpeed * delta));
            //Check the distance from the players position to the hook point of the grapple
            //GD.Print("Distance to hook point = " + Transform.origin.DistanceTo(hookPoint));
            if (Transform.origin.DistanceTo(hookPoint) < 1.5f)
            {
                reachedHookPoint = true;
            }

            //grappleLine.DrawLine(Transform.origin, hookPoint);

        }
        //We set the maximum movement speed her, later more max move speeds will be added for crouching, sprinting and gliding
        if (isSprinting) target *= maxSprintSpeed;
        else if (isCrouching) target *= maxCrouchSpeed;
        else if (hasHookPoint) target *= maxGrappleSpeed;
        else target *= maxWalkSpeed;
        //Create the aceleration variable to be used
        float accel;
        //Check if the dot product for the direction vec3 is greater than zero, if it is we set the accel to acceleration else we set it to deaccelerate
        if (direction.Dot(hvel) > 0) accel = acceleration;
        else accel = deacceleration;
        //We then linear interpolate the horizontal velocity with the target by the accel amount
        hvel = hvel.LinearInterpolate(target, accel * delta);
        //We set the velocity to the newly interpolated hvel vec3
        velocity.x = hvel.x + gravityVec.x;
        velocity.z = hvel.z + gravityVec.z;
        velocity.y = gravityVec.y;
        //We then call the move and slide method with the new velocity values
        //velocity = MoveAndSlide(velocity, Vector3.Up, true, 4, Mathf.Deg2Rad(maxSlopeAngle));
        MoveAndSlide(velocity, Vector3.Up, true, 4, Mathf.Deg2Rad(maxSlopeAngle));
    }
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion eventMouseMotion && Input.GetMouseMode() == Input.MouseMode.Captured)
        {
            //RotateY(Mathf.Deg2Rad(-@event.relative.x * mouseSensitivity)) 
            cameraGimbal.RotateX(Mathf.Deg2Rad(-eventMouseMotion.Relative.y * mouseSensitivity));
            RotateY(Mathf.Deg2Rad(-eventMouseMotion.Relative.x * mouseSensitivity));

            //cameraGimbal.Rotation = new Vector3(Mathf.Clamp(cameraGimbal.Rotation.x, Mathf.Deg2Rad(-90), Mathf.Deg2Rad(90)), cameraGimbal.Rotation.y, cameraGimbal.Rotation.z);
            Vector3 camRotation = cameraGimbal.RotationDegrees;
            camRotation.x = Mathf.Clamp(camRotation.x, -80f, 80f);
            cameraGimbal.RotationDegrees = camRotation;
        }
    }
}
