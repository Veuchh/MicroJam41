using Unity.Cinemachine;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [SerializeField] CinemachineCamera cinemachineCamera;

    private void Awake()
    {
        cinemachineCamera.transform.parent = null;
    }
}
