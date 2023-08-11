using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D)),
 RequireComponent(typeof(MBEntity))]
public class MBPlayer : MonoBehaviour {

    [SerializeField]
    private int targetRange;
    
    [SerializeField]
    private Transform target;
    
    [SerializeField]
    private int moveSpeed;
    
    [SerializeField]
    private Camera myCamera;

    [SerializeField]
    private GameObject dodgeAbilityPrefab;
    
    private MBAbility dodgeAbility;

    private Rigidbody2D rigidbody2d;
    private MBEntity entity;
   
    /* message */ void Awake() {
        this.rigidbody2d = this.GetComponent<Rigidbody2D>();
        this.entity = this.GetComponent<MBEntity>();
        GameObject dodgeAbilityInstance = GameObject.Instantiate(
            dodgeAbilityPrefab, this.transform
        );
        this.dodgeAbility = dodgeAbilityInstance.GetComponent<MBAbility>();
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
       
        Ray ray = this.myCamera.ScreenPointToRay(
            new Vector3(inputVector.x, inputVector.y, 0f)
        );
        float t = - ray.origin.z / ray.direction.z;
        this.target.position = ray.origin + t * ray.direction;
    } 

    public void OnDodgeAction(InputAction.CallbackContext context) {
        Debug.Log("Dodge");
        this.StartCoroutine(
            this.dodgeAbility.Use(this.entity, this.target.position)
        );
    }   
}