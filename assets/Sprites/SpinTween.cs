using UnityEngine;
using System.Collections;


public class SpinTween : MonoBehaviour {
    public Vector3 direction;
    public float moveSpeed = 625f;
    public float spinSpeed = 300f;
    public float gravity = .63f;
    public float gravityOffset = -5.5f;
    public float upwardBias = 1.15f;
    public float velocityMult = 114.98f;

    private float timeAlive;
    public float accelDuration = 0.12f;


    private Material material;
    private Rigidbody rBody;

    void Start()
    {
        material = gameObject.GetComponent<Renderer>().material;
        rBody = gameObject.GetComponent<Rigidbody>();

        timeAlive = 0.0f;
        direction = Random.insideUnitCircle;
        if (direction.y < 0)
            direction.y *= -1;
        direction.y += upwardBias;
    }
    void Update()
    {
        timeAlive += Time.deltaTime;

        if (System.Math.Abs((decimal)rBody.velocity.y) > 10.0m)
            rBody.AddRelativeTorque(Vector3.down * spinSpeed, ForceMode.Impulse);

        if (timeAlive < accelDuration)
        {


            
            rBody.AddForce(direction * moveSpeed * Time.deltaTime);

            /*
            float downwardForce = (gravity * (timeAlive - gravityOffset)) * Time.deltaTime;
            direction.y -= downwardForce;
            
            transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime * 20);
            if (downwardForce < 0)
                transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
                */

        } else if (timeAlive > 4f)
        {
            Disintergrate();
        }
        else {
            rBody.AddForce(Vector3.down * velocityMult * Time.deltaTime);
            //Destroy(this.gameObject);
        }
        
    }

    void Disintergrate()
    {
        Color newColor = material.color;
        newColor.a -= 1f * Time.deltaTime;
        material.color = newColor;

        if (newColor.a < .05)
        {
            Destroy(this.gameObject);
        }
    }

}



