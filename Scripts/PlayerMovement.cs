using Godot;
using System;
using EventCallback;

public class PlayerMovement : KinematicBody
{
    //= Grappling hook variables ==================================================================
    //The point where the grappling hook has hooked
    Vector3 hookPoint = new Vector3();
    //If a hook point is returned
    bool hasHookPoint = false;
    //The raycast that picks up if the player has the ceiling above it
    RayCast ceilingRay;
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
    //The keyboards directional input only for the horizotal movement
    Vector3 inputDirection = new Vector3();
    //The horizontal direction of travel
    Vector3 horizontalVelocity = new Vector3();
    //The vertical direction of travel
    Vector3 verticalVelocity = new Vector3();
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
    //Decelleration when input controls released while in mid air
    float airAcceleration = 1;
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
    bool canJump = true;
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
    //If the escape key has been pressed we take note of it so the key signal does not get repeated and the action only happens once
    bool escapePressed = false;
    //Check for the graple button
    bool abilityPressed = false;
    //=============================================================================================

    //= Variables used for movement checks from the input manager =================================
    bool moveForward = false, moveBackward = false, strafeLeft = false, strafeRight = false, sprint = false, jump = false, crouch = false, ability = false, escape = false;
    //=============================================================================================

    //= Called when the node enters the scene tree for the first time. ============================
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
        groundRay = GetNode<RayCast>("GroundCollisionRay");
        //The raycast on the player body that detects cieling collisions
        ceilingRay = GetNode<RayCast>("CeilingCollisionRay");
        //Grab the refference to the cameras gimbals
        cameraGimbal = GetNode<Spatial>("CameraGimbal");
        //Grab the refference to the camera
        camera = GetNode<Camera>("CameraGimbal/Camera");
        //Set the grappel line to the emediate geometry object on the player
        grappleLine = GetNode<DrawGrappleLine>("GrappleLine");
    }
    // ============================================================================================

    //= Whenever the input manager is called these this function is called
    private void grabInput(InputHandleEvent ihei)
    {
        moveForward = ihei.upPressed;
        moveBackward = ihei.downPressed;
        strafeLeft = ihei.leftPressed;
        strafeRight = ihei.rightPressed;
        sprint = ihei.sprintPressed;
        jump = ihei.jumpPressed;
        crouch = ihei.crouchPressed;
        ability = ihei.abilityPressed;
        escape = ihei.escapePressed;
    }
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion eventMouseMotion && Input.GetMouseMode() == Input.MouseMode.Captured)
        {

            cameraGimbal.RotateX(Mathf.Deg2Rad(-eventMouseMotion.Relative.y * mouseSensitivity));
            RotateY(Mathf.Deg2Rad(-eventMouseMotion.Relative.x * mouseSensitivity));

            Vector3 camRotation = cameraGimbal.RotationDegrees;
            camRotation.x = Mathf.Clamp(camRotation.x, -80f, 80f);
            cameraGimbal.RotationDegrees = camRotation;
        }
    }
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(float delta)
    {
        //Grabs the input 
        ProcessInput(delta);
        //Processes the input and moves the charecter
        ProcessMovement(delta);
    }

    private void ProcessInput(float delta)
    {
        //The horizontal movement input from the keyboard, we reset it here so it does not build up a value indefenitely
        inputDirection = Vector3.Zero;
        //Check if the ground ray is colliding then sets the grounded value acordingly
        if (groundRay.IsColliding()) groundContact = true; else groundContact = false;

        //Explination for inputDirection below, the inpit direction below 
        //Check what movement buttons are pressed
        if (moveForward)
            inputDirection -= Transform.basis.z;
        else if (moveBackward)
            inputDirection += Transform.basis.z;
        if (strafeLeft)
            inputDirection -= Transform.basis.x;
        else if (strafeRight)
            inputDirection += Transform.basis.x;

        //Normalize the input vector to not get faster movement when moving diagonally
        inputDirection = inputDirection.Normalized();

        //If the sprint button is pressed we set sprinting to true
        if (sprint) isSprinting = true; else isSprinting = false;

        //If the player is crouching and the collision shape is a capsule then 
        if (crouch)
        {
            //Return out of the above if statement as you cannot crouch while gliding or hooked on a grapple hook point
            if (hasHookPoint || isGliding) return;
            //Set the new collision capsule height when crouching
            ((CapsuleShape)bodyCollShape.Shape).Height -= maxCrouchSpeed * delta;
            //Set is crouching to true 
            isCrouching = true;
        }
        else if (!isCollidingWithCeiling)
        {
            ((CapsuleShape)bodyCollShape.Shape).Height += maxCrouchSpeed * delta;
            isCrouching = false;
        }
        //Clamp the max and min height for crouching when it is being modified
        ((CapsuleShape)bodyCollShape.Shape).Height = Mathf.Clamp(((CapsuleShape)bodyCollShape.Shape).Height, crouchHeight, defualtHeight);

        //Check if the abillity button is being pressed 
        if (!ability) abilityPressed = false;
        //If the player presses the ability key for hte grapple
        if (ability && !abilityPressed)
        {
            abilityPressed = true;
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
                isGliding = false;
            }
        }
        //We check if we are colliding with hte ceiling
        if (ceilingRay.IsColliding())
        {
            //If we hit the ceiling with our head we set the graplpling to false to disconect the grapple line
            grappleActive = false;
            //We reset the hookPoint 
            hookPoint = new Vector3();
            //We set the hookPointGet
            hasHookPoint = false;
            GlobalTranslate(new Vector3(0, -.2f, 0));
            reachedHookPoint = false;
        }
        if(reachedHookPoint && groundRay.IsColliding())
        {
            //If we hit the ground we disconect the grapple line
            grappleActive = false;
            //We reset the hookPoint 
            hookPoint = new Vector3();
            //We set the hookPointGet
            hasHookPoint = false;
            reachedHookPoint = false;
        }

        if (!escape) escapePressed = false;
        //  Capturing/Freeing the cursor
        if (escape && !escapePressed)
        {
            escapePressed = true;
            if (Input.GetMouseMode() == Input.MouseMode.Visible)
                Input.SetMouseMode(Input.MouseMode.Captured);
            else
                Input.SetMouseMode(Input.MouseMode.Visible);
        }
    }
    private void ProcessMovement(float delta)
    {
        float accel;

        //Check if we are on the floor for the gravity calculations
        if (!IsOnFloor())
        {
            //If we are not on the floor and the charecter is gliding
            if (isGliding)
            {
                //If we are not on the floor we just add the gravity like normal we halve the gravity
                verticalVelocity += Vector3.Down * (gravity * 0.5f) * delta;
            }
            else
            {
                //If we are not on the floor we just add the gravity like normal
                verticalVelocity += Vector3.Down * gravity * delta;
            }
        }
        else if (IsOnFloor() && groundContact)
        {
            //If we are on a slope we add gravity in the oposite direction of the slopes normal vector to prevent a godot bug from allowing us to slowely slide down the slope
            verticalVelocity = -GetFloorNormal() * gravity;
            //Set the gliding to false
            isGliding = false;
        }
        else
        {
            // if we are not on the floor or on a slope we 
            verticalVelocity = -GetFloorNormal();
        }

        //Used to eliminate the key presses from repeating an action that only needs to be run on one press of the button
        //currently the keypress callback runs repeated key presses
        if (!jump) canJump = true;
        //If the jump button was pressed
        //This jump funtion does a normal low jump when the jump key is pressed or held in but once the
        //jump key is left the can jump goes to true that allows us to press it again, if the jump button is pressed again the 
        //player is not on the ground or on a grapple hook the gliding is set to true that reduces the gravity to .5 of its original strength
        if (jump)
        {
            //If we can jump and we are not gliding yet 
            if (canJump && !isGliding)
            {
                //We set the can jump to false
                canJump = false;    
                //If not on the floor or slope we set the gliding to true
                if (!IsOnFloor() && !groundContact && !grappleActive)
                {
                    //We are in the air and we can activate gliding
                    isGliding = true;
                }
                //If we are on the floor or in contact with a slope 
                if (IsOnFloor() || groundContact)
                {
                    //We add the jymp speed to the vertical velocity
                    verticalVelocity = Vector3.Up * jumpSpeed;
                }
                //If we are at a hook point for the grapple
                else if (reachedHookPoint)
                {
                    //We add a bit larger jump speed to clear the ledge that we are grappled to
                    verticalVelocity = Vector3.Up * grappleJumpSpeed;
                    //If we hit the ceiling with our head we set the graplpling to false to disconect the grapple line
                    grappleActive = false;
                    //We reset the hookPoint 
                    hookPoint = new Vector3();
                    //We set the hookPointGet
                    hasHookPoint = false;
                    //Reset the hook point reached
                    reachedHookPoint = false;
                }

            }

        }

        //= Grapple calculations for the grapple movement 
        if (hasHookPoint)
        {
            verticalVelocity.y = 0;
            Transform = new Transform(Transform.basis, Transform.origin.LinearInterpolate(hookPoint, maxGrappleSpeed * delta));
            grappleLine.DrawLine(Transform.origin, hookPoint);
            //Check the distance from the players position to the hook point of the grapple
            //GD.Print("Distance to hook point = " + Transform.origin.DistanceTo(hookPoint));
            if (Transform.origin.DistanceTo(hookPoint) < 1.5f)
            {
                reachedHookPoint = true;
                grappleLine.HideLine();
            }
        }

        //======================================================

        //= Last physics calculations before mave and slide is called
        //We set the maximum movement speed her, later more max move speeds will be added for crouching, sprinting and gliding
        if (isSprinting) inputDirection *= maxSprintSpeed;
        else if (isCrouching) inputDirection *= maxCrouchSpeed;
        else inputDirection *= maxWalkSpeed;

        //Check if the dot product for the direction vec3 is greater than zero, if it is we set the accel to acceleration else we set it to deaccelerate
        if (horizontalVelocity.Dot(velocity) > 0) accel = acceleration;
        else accel = deacceleration;

        horizontalVelocity = horizontalVelocity.LinearInterpolate(inputDirection, accel * delta);
        velocity.z = horizontalVelocity.z + verticalVelocity.z;
        velocity.x = horizontalVelocity.x + verticalVelocity.x;
        velocity.y = verticalVelocity.y;



        MoveAndSlide(velocity, Vector3.Up);
    }
}
