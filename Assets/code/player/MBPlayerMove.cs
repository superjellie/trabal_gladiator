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
    }
    
    public void MovePerformed(InputAction.CallbackContext context) {
        Debug.Log(context);
        Vector2 inputVector = context.ReadValue<Vector2>();
        this.rigidbody2d.velocity = 
            new Vector2(inputVector.x, inputVector.y) * this.moveSpeed;
    }

}