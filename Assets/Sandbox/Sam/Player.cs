using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private CharacterController controller = null;

    private Vector2 previousInput;

    private Controls controls;
    private Controls Controls
    {
        get
        {
            if (controls != null)
            {
                return controls;
            }
            return controls = new Controls();
        }
    }

    public override void OnStartAuthority()
    {
        enabled = true;

        Controls.Player.Move.performed += ctx =>
        {
            CmdMoveInput(ctx.ReadValue<Vector2>());
        };
        Controls.Player.Move.canceled += ctx =>
        {
            CmdMoveInput(Vector2.zero);
        };

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    [ClientCallback]
    void Update()
    {
        
    }

    [ClientCallback]

    private void OnEnable()
    {
        Controls.Enable();
    }

    [ClientCallback]

    private void OnDisable()
    {
        Controls.Disable();
    }

    [Server]

    private void FixedUpdate()
    {
        RpcMove();
    }

    [Command]  // This is run on the Server
    private void CmdMove()
    {

        RpcMove();

    }

    [Command]
    private void CmdMoveInput(Vector2 pInput)
    {
        previousInput = pInput;
    }

    [ClientRpc]  // This is run on the Client
    private void RpcMove()
    {
        Vector3 right = new Vector3(1f, 0f, 0f);
        Vector3 up = new Vector3(0f, 1f, 0f);

        Vector3 movement = right * previousInput.x + up * previousInput.y;

        controller.Move(movement * movementSpeed * Time.deltaTime);
    }
}
