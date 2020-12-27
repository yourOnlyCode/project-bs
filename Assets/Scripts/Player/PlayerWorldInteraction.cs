using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerWorldInteraction : NetworkBehaviour
{

    override public void OnStartAuthority()
    {
        enabled = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        
    }
}
