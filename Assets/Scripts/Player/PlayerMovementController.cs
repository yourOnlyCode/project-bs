using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMovementController : NetworkBehaviour
{
    [SerializeField] private float _movementSpeed = 5f;
    [SerializeField] private Rigidbody2D _controller = null;
    [SerializeField] private PlayerAnimationController _animationController = null;
    [SerializeField] private GameObject _Equipment = null;

    private List<GameObject> _currentCollisions = null;
    private GameObject _equippedItem = null;
    private GameObject _carriedItem = null;

    private Vector2 _previousInput;
    [SyncVar] private Vector2 _facingDirection;
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

    public override void OnStartServer()
    {
        base.OnStartServer();
        _currentCollisions = new List<GameObject>();
    }

    public override void OnStartAuthority()
    {
        enabled = true;

        CmdInitializeServerPosition(_controller.position);
        _currentCollisions = new List<GameObject>();
        Controls.Player.Move.performed += ctx => SetMovement(ctx.ReadValue<Vector2>());
        Controls.Player.Move.canceled += ctx => ResetMovement();
        Controls.Player.Pickup.performed += ctx => CmdPickupItem();
        Controls.Player.Drop.performed += ctx => CmdDropAllItems();
        Controls.Player.Attack.performed += ctx => CmdUseEquipment();
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

        //Debug.Log(_currentCollisions.Count);
        Debug.DrawRay(_serverPosition, _previousInput.normalized, Color.red);
        if (GetComponent<NetworkIdentity>().isServer)
        {

            _controller.position += _previousInput.normalized * _movementSpeed * Time.fixedDeltaTime;
            _serverPosition = _controller.position;
            if(_previousInput.sqrMagnitude != 0)
            {
                _facingDirection = _previousInput.normalized;
            }
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(_currentCollisions != null && !_currentCollisions.Contains(collision.gameObject))
        {
            _currentCollisions.Add(collision.gameObject);
            if (GetComponent<NetworkIdentity>().isServer)
            {
                // Debug.Log(collision);
            }
            // Debug.Log("Entered");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Debug.Log(collision);
        if(_currentCollisions != null && _currentCollisions.Contains(collision.gameObject))
        {
            // Debug.Log("removed");
            _currentCollisions.Remove(collision.gameObject);
        }
    }

    [Command]
    private void CmdUseEquipment()
    {
        if(_equippedItem != null)
        {
            Collider2D[] hit2D = Physics2D.OverlapBoxAll(_controller.position + _facingDirection, new Vector2(1f, 1f), 0f);
            Debug.DrawRay(_controller.position, _facingDirection, Color.red);
            // Debug.Log(hit2D);
            _equippedItem.GetComponent<EquipableItems>().Swing(hit2D);
        }
    }

    [ClientRpc]
    private void RpcDropItem(bool pickup)
    {
        Debug.Log(pickup);
        Debug.Log(_carriedItem);
        Debug.Log(_equippedItem);
    }

    [Command]
    private void CmdPickupItem()
    {
        bool pickup = false;
        for (int i = 0; i < _currentCollisions.Count; i++)
        { 
            if (_currentCollisions[i].layer == 9) // Equipment
            {
                if (!_equippedItem)
                {
                    _currentCollisions[i].transform.parent = _Equipment.transform;
                    _equippedItem = _currentCollisions[i].gameObject;
                    _equippedItem.GetComponent<EquipableItems>().ServerPickup();
                    pickup = true;
                   
                    // RpcPickupItem(_equippedItem.GetComponent<NetworkIdentity>().netId);
                    break;
                }
            } else if(_currentCollisions[i].layer == 12) // Material
            {
                if (!_carriedItem)
                {
                    _currentCollisions[i].transform.parent = _Equipment.transform;
                    _carriedItem = _currentCollisions[i].gameObject;
                    _carriedItem.GetComponent<Material>().ServerPickup();
                    pickup = true;
                    break;
                }
            }
        }
        RpcDropItem(pickup);
    }

    [Command]
    private void CmdDropAllItems()
    {

        foreach(Transform child in _Equipment.transform)
        {
            child.parent = null;
            if(child.gameObject.layer == 10)
            {
                child.gameObject.GetComponent<EquipableItems>().ServerDrop();
            }
            else if (child.gameObject.layer == 13)
            {
                child.gameObject.GetComponent<Material>().ServerDrop();
                child.gameObject.GetComponent<Material>().SpawnItem(child.position, new Vector3(_facingDirection.x, _facingDirection.y).normalized*1.5f);
            }
        }
        _equippedItem = null;
        _carriedItem = null;
    }

}
