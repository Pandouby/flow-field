using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particle : MonoBehaviour
{
    public float _velocity;
    Renderer renderer;
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position += transform.forward * _velocity * Time.deltaTime;

        renderer.material.color = new Color(this.transform.rotation.x, this.transform.rotation.y, this.transform.rotation.z, 1);
    }

    public void ApplyRotation(Vector3 roation, float rotationSpeed)
    {
        Quaternion targetRotation = Quaternion.LookRotation(roation.normalized);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
