using UnityEngine;

public class FishMovement : MonoBehaviour
{
    public float speed = 2f;
    public float changeDirectionTime = 2f;
    public float moveRadius = 10f;
    private Vector3 _centerPoint;
    private Vector3 _targetDirection;
    private float _timer;

    public void SetCenter(Vector3 center, float radius)
    {
        _centerPoint = center;
        moveRadius = radius;
        PickNewDirection();
    }

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer > changeDirectionTime)
        {
            PickNewDirection();
            _timer = 0;
        }

        Vector3 nextPosition = transform.position + _targetDirection * speed * Time.deltaTime;

        if (Vector3.Distance(_centerPoint, nextPosition) < moveRadius)
            transform.position = nextPosition;
        else
            PickNewDirection(); // Stay inside radius
    }

    void PickNewDirection()
    {
        _targetDirection = Random.insideUnitSphere;
        _targetDirection.y = Mathf.Clamp(_targetDirection.y, -0.5f, 0.5f); // Optional: limit vertical motion
        _targetDirection.Normalize();
    }
}
