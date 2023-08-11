using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]

public class MBPlayerMove : MonoBehaviour
{
    [SerializeField]
    private int targetRange;
    
    [SerializeField]
    private Transform target;
    
    [SerializeField]
    private int moveSpeed;
    
    [SerializeField]
    private Camera camera;
    
    private Rigidbody2D rigidbody2d;
    
   
    
    public void Awake() {
        this.rigidbody2d = this.GetComponent<Rigidbody2D>();
    }
    
    public void OnMoveAction(InputAction.CallbackContext context) {
        // Debug.Log(context);
        Vector2 inputVector = context.ReadValue<Vector2>();
        this.rigidbody2d.velocity = 
            new Vector2(inputVector.x, inputVector.y) * this.moveSpeed;
    }
    
     public void OnTargetAction(InputAction.CallbackContext context) {
        // Debug.Log(context);
        Vector2 inputVector = context.ReadValue<Vector2>();
       
        Ray ray = camera.ScreenPointToRay(
            new Vector3(inputVector.x, inputVector.y, 0f)
        );
        float t = - ray.origin.z / ray.direction.z;
        this.target.position = ray.origin + t * ray.direction;
    }
    
}