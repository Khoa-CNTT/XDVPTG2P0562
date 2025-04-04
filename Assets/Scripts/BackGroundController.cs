using UnityEngine;

public class BackGroundController : MonoBehaviour
{
    private float startPos, length;
    public GameObject Camera;
    public float parallaxEffect;
    void Start()
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float distance = Camera.transform.position.x * parallaxEffect;
        float movement = Camera.transform.position.x * (1 - parallaxEffect);

        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);

        if(movement > startPos + length){
            startPos += length;
        }else if(movement < startPos - length){
            startPos -= length;
        }
    }
}
