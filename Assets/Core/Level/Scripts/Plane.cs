using UnityEngine;

public class Plane : MonoBehaviour
{
    [SerializeField]
    Vector2 moveDir;

    [SerializeField]
    float moveSpeed = 60;

    void Update()
    {
        transform.position += new Vector3(moveDir.x, moveDir.y, 0).normalized * Time.deltaTime * moveSpeed;
    }
}
