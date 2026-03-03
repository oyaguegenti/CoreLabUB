using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class PhysicalButton : MonoBehaviour
{
    public float max_y, min_y, max_x, max_z, min_z;

    private Rigidbody rb;

    private bool touchingSomething = false;

    [SerializeField] private UnityEvent programable_event;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        GetComponent<BoxCollider>().isTrigger = true;

        max_y = transform.position.y;
        min_y = transform.position.y - 0.08f;
        max_x = transform.position.x;
        max_z = transform.position.z;
    }

    void FixedUpdate()
    {
        if (!touchingSomething && transform.position.y < max_y)
        {
            rb.AddForce(transform.forward * 1.2f, ForceMode.Force);
        }
        if (touchingSomething && transform.position.y > min_y)
        {
            rb.AddForce(-transform.forward * 1.05f, ForceMode.Force);
        }
        if (transform.position.y <= min_y)
        {
            programable_event.Invoke();   
        }
        if (transform.position.y < min_y)
        {
            rb.velocity = Vector3.zero;
            transform.position = new Vector3(max_z, min_y, max_z);
        }
        if (transform.position.y > max_y)
        {
            rb.velocity = Vector3.zero;
            transform.position = new Vector3(max_z, max_y, max_z);
        }
        /*
        if (!touchingSomething && transform.position.y < max_y)
        {
            transform.Translate(transform.forward * Time.deltaTime);
            //rb.AddForce(transform.forward * 5, ForceMode.Force);
        }
        else if (transform.position.y > max_y)
            transform.position = new Vector3(transform.position.x, max_y, transform.position.z);
        else if (transform.position.y <= min_y)
        {
            Debug.Log("A");
            programable_event.Invoke();
            transform.position = new Vector3(transform.position.x, min_y, transform.position.z);
        }

        if (touchingSomething && transform.position.y > min_y)
            rb.AddForce(-transform.forward * 1.3f, ForceMode.Force);*/

        transform.position = new Vector3(max_x, transform.position.y, max_z);
    }

    private void OnTriggerEnter(Collider coll)
    {
        Debug.Log("a");
        touchingSomething = true;
    }
    private void OnTriggerExit(Collider coll)
    {
        Debug.Log("b");

        touchingSomething = false;
    }
}