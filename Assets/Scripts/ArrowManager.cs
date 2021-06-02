using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowManager : MonoBehaviour
{
    public float speed = 1f;

    private Rigidbody2D rbody;

    
    void Awake()
    {
        rbody = GetComponent<Rigidbody2D>();
        //5秒后不管打没打中，销毁
        Destroy(this.gameObject, 5f);  
    }


    void Start()
    {
    
    }


    //弓箭发射
    public void Shoot(Vector2 moveDirection, float moveSpeed)
    {  
        rbody.velocity = speed * moveDirection;
    }

    //碰到敌人
    private void OnTriggerEnter2D(Collider2D collider) {
        Debug.Log(collider.name);
        Destroy(this.gameObject, 0.1f);
    }

   
    // Update is called once per frame
    void Update()
    {
        // float distance = (transform.position - startPos).sqrMagnitude;
        // if(distance > destroyDistance) 
        // {
        //     Destroy(gameObject);
           
        // }
    }
}
