using UnityEngine;

/// <summary>
/// 작성자 : 김동건
/// 오브젝트 회전을 구현하기 위한 스크립트
/// </summary>
public class ObjectRotator : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 50f;
    
    void FixedUpdate()
    {
        transform.Rotate(Vector3.forward * (rotationSpeed * Time.deltaTime));
    }
}