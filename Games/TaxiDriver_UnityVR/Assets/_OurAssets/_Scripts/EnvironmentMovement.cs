using System.Collections;
using UnityEngine;

public class EnvironmentMovement : MonoBehaviour
{

    [SerializeField] private float speedSlow =  2f;
    [SerializeField] private float speedMedium =  4f;
    [SerializeField] private float speedFast =  8f;
     [SerializeField] private float speed = 8f ;
    [SerializeField] private Vector3 direction = Vector3.forward;
    [SerializeField] private bool isMoving = true;
    
    void Update()
    {
        if (!isMoving) return;
        
        Move();
    }

    private void Move()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    public void OnChangeSpeed(int index)
    {
        switch (index)
        {
            case 0: SetSpeed(speedSlow); 
                Debug.Log("Slow Speed Active"); break;
            case 1: SetSpeed(speedMedium);
                Debug.Log("medium Speed Active"); break;
            case 2: SetSpeed(speedFast);  
                Debug.Log("fast Speed Active"); break;
        }
    }
    private void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public void StopCar()
    {
        isMoving = false;
        Debug.Log($"Car is moving: {isMoving}");
    }

    public void StartCar()
    {
        isMoving = true;
    }
}
