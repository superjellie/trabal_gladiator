using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D)),
 RequireComponent(typeof(MBEntity))]
public class MBPlayer : MonoBehaviour {

    [SerializeField]
    private float targetRange;
    
    [SerializeField]
    private Transform target;
    
    [SerializeField]
    private float moveSpeed;
    
    [SerializeField]
    private Camera myCamera;

    [SerializeField]
    private MBAbility defaultDodgePrefab;
    
    private MBAbility dodgeAbility;

    private Rigidbody2D rigidbody2d;
    private MBEntity entity;
    private Vector3 moveDirection;
   
    /* message */ void Awake() {
        this.rigidbody2d = this.GetComponent<Rigidbody2D>();
        this.entity = this.GetComponent<MBEntity>();
        this.defaultDodgePrefab.PlaceTo(ref this.dodgeAbility, this.transform);
    }
    
    public void OnMoveAction(InputAction.CallbackContext context) {
        Vector2 inputVector = context.ReadValue<Vector2>();
        this.moveDirection = new Vector3(inputVector.x, inputVector.y, 0f);
        this.rigidbody2d.velocity = inputVector * this.moveSpeed;
    }
    
    public void OnTargetAction(InputAction.CallbackContext context) {
        Vector2 inputVector = context.ReadValue<Vector2>();
        Ray ray = this.myCamera.ScreenPointToRay(
            new Vector3(inputVector.x, inputVector.y, 0f)
        );
        float t = - ray.origin.z / ray.direction.z;
        this.target.position = ray.origin + t * ray.direction;
    } 

    public void OnDodgeAction(InputAction.CallbackContext context) {
        if (!this.dodgeAbility.isRunning)
            this.dodgeAbility.UseAsync(this.entity, 
                this.moveDirection.magnitude > .001f ? 
                    this.transform.position + this.moveDirection
                    : this.target.position
            );
    }   
}