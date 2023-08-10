using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]

public class MBPlayerMove : MonoBehaviour
{
    [SerializeField]
    private int moveSpeed;
    
    private Rigidbody2D rigidbody2d;
    
    public void Awake() {
        this.rigidbody2d = this.GetComponent<Rigidbody2D>();
        //playerInput.onActionTriggered += PlayerInput_onActionTriggered; 
        /*Tribal_gladiator tribalGladiator  = new Tribal_gladiator();
        tribalGladiator.Player.Enable();
        tribalGladiator.Player.Move.performed += Move_performed;*/
    }
    
    public void Move_performed(InputAction.CallbackContext context) {
        Debug.Log(context);
        Vector2 inputVector = context.ReadValue<Vector2>();
        rigidbody2d.velocity = new Vector2(inputVector.x, inputVector.y) * moveSpeed;
    }

}