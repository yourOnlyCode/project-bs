using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMovementController : NetworkBehaviour
{
    [SerializeField] private float _movementSpeed = 5f;
    [SerializeField] private Rigidbody2D _controller = null;
    [SerializeField] private PlayerAnimationController _animationController = null;

    private Vector2 _previousInput;

    [SyncVar] private Vector2 _serverPosition;

    private Controls _controls;

    private Controls Controls
    {
        get
        {
            if(_controls != null) { return _controls; }

            return _controls = new Controls();
        }
    }

    public override void OnStartAuthority()
    {
        enabled = true;

        CmdInitializeServerPosition(_controller.position);

        Controls.Player.Move.performed += ctx => SetMovement(ctx.ReadValue<Vector2>());
        Controls.Player.Move.canceled += ctx => ResetMovement();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (hasAuthority)
        {
            // If this is the local player then enable controls.
            Controls.Enable();
        }
        else
        {
            // Otherwise disable the controls.
            Controls.Disable();
        }
    }

    [ClientCallback]

    private void OnEnable()
    {

    }

    [ClientCallback]

    private void OnDisable()
    {
        Controls.Disable();
    }

    [ClientCallback]

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Debug.DrawRay(_serverPosition, _previousInput.normalized, Color.red);
        if (GetComponent<NetworkIdentity>().isServer)
        {

            _controller.position += _previousInput.normalized * _movementSpeed * Time.fixedDeltaTime;
            _serverPosition = _controller.position;
            int facingDirection = getDirection(_previousInput.normalized);
            // _controller.position = _serverPosition;
            if(_previousInput.sqrMagnitude > 0)
            {
                _animationController.setAnimation(PlayerAnimationController.WALK_ANIMATION, facingDirection);
            }
            else
            {
                _animationController.setAnimation(PlayerAnimationController.IDLE_ANIMATION, facingDirection);
            }

            if (_previousInput.x < 0)
            {
                _controller.transform.localScale = new Vector3(-1f, _controller.transform.localScale.y);
            }
            else if(_previousInput.x > 0)
            {

                _controller.transform.localScale = new Vector3(1f, _controller.transform.localScale.y);
            }

        } else
        if (GetComponent<NetworkIdentity>().isClient)
        {

            if (hasAuthority) // If this is the local player.
            {
                Move(); // This will move locally then move on the server to try and reduce lag.
            }
            else // This is not the local player.
            {
                // Check to see how far the player has moved.
                Vector2 difference = _serverPosition - _controller.position;
                // If the difference is large than smooth the movement, otherwise just set the new position.
                if (difference.magnitude <= .2)
                {
                    // Set new position.
                    _controller.position = _serverPosition;
                }
                else
                {
                    // Smooth movement
                    _controller.position += difference * _movementSpeed * Time.fixedDeltaTime;
                }

                int facingDirection = getDirection(difference);


                if (difference.sqrMagnitude > 0)
                {
                    _animationController.setAnimation(PlayerAnimationController.WALK_ANIMATION, facingDirection);
                } else
                {
                    _animationController.setAnimation(PlayerAnimationController.IDLE_ANIMATION, facingDirection);
                }

                if (difference.x < 0)
                {
                    _controller.transform.localScale = new Vector3(-1f, _controller.transform.localScale.y);
                }
                else if (difference.x > 0)
                {

                    _controller.transform.localScale = new Vector3(1f, _controller.transform.localScale.y);
                }

            }
        }
    }

    [Client]
    private void Move()
    {
        // This will make the local movement look smoother.
        _controller.position += _previousInput.normalized * _movementSpeed * Time.fixedDeltaTime;

        // Over time, the local position might be different than the server position. This will track the difference.
        Vector2 deltaPosition = _controller.position - _serverPosition;
        // If the difference is too great, reset the local position to match the server position.
        if (deltaPosition.magnitude >= 3)
        {
            _controller.position = _serverPosition;
        }

        int facingDirection = getDirection(_previousInput);

        if (_previousInput.sqrMagnitude > 0)
        {
            _animationController.setAnimation(PlayerAnimationController.WALK_ANIMATION, facingDirection);
            if (_previousInput.x < 0)
            {
                _controller.transform.localScale = new Vector3(-1f, _controller.transform.localScale.y);
            }
            else if (_previousInput.x > 0)
            {

                _controller.transform.localScale = new Vector3(1f, _controller.transform.localScale.y);
            }
        }
        else
        {
            _animationController.setAnimation(PlayerAnimationController.IDLE_ANIMATION, facingDirection);
        }
    }

    [Client]
    private void SetMovement(Vector2 pMovement)
    {
        _previousInput = pMovement.normalized;
        CmdSetInput(pMovement);
    }

    [Client]
    private void ResetMovement()
    {
        _previousInput = Vector2.zero;
        CmdSetInput(Vector2.zero);
    }

    [Command]
    private void CmdInitializeServerPosition(Vector2 pTransform)
    {
        _serverPosition = pTransform;


        // Debug.Log(this.GetComponent<PlayerGameObject>().GetPlayerInformation().GetPlayerRole());
    }

    [Command]
    private void CmdSetInput(Vector2 pInput)
    {
        pInput = pInput.normalized;
        _previousInput = pInput;
    }

    private int getDirection(Vector2 direction)
    {
        int facingDirection;
        if (direction.sqrMagnitude == 0)
        {
            facingDirection = -1;
        }
        else if (direction.x * direction.x > direction.y * direction.y)
        {
            if (direction.x < 0)
            {
                facingDirection = PlayerAnimationController.LEFT;
            }
            else
            {
                facingDirection = PlayerAnimationController.RIGHT;
            }
        }
        else
        {
            if (direction.y < 0)
            {
                facingDirection = PlayerAnimationController.DOWN;
            }
            else
            {
                facingDirection = PlayerAnimationController.UP;
            }
        }

        return facingDirection;
    }

}
