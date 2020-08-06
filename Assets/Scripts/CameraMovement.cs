using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private GameObject m_Target;

    [SerializeField] private float m_CameraSensitivity;
    private float m_CameraSensitivityBase;
    
    [SerializeField] private Vector3 m_CameraOffset;
    
    [Header("Camera height options")]
    [SerializeField] private float m_YRotationMinimum;
    [SerializeField] private float m_YRotationMaximum;

    [Header("Zoom distance options")]
    [SerializeField] private float m_MinimumCameraZoom;
    [SerializeField] private float m_MaximumCameraZoom;
    

    [SerializeField] private LayerMask m_CollisionLayers = default;
   
    //Private
    private float m_DesiredCameraX = 270;
    private float m_DesiredCameraY = 0;
    private float m_DesiredCameraZoom = 0;
    private float m_CameraX = 360;
    private float m_CameraY = 0;
    private float m_CameraZoom;
    private float m_CameraXSmoothVelocity;
    private float m_CameraYSmoothVelocity;
    private float m_CameraZoomVelocity;
    private Vector3 m_CameraLocation;
    private const string k_MouseX = "Mouse X";
    private const string k_MouseY = "Mouse Y";
    private const string k_MouseWheel = "Mouse ScrollWheel";

    private void Awake()
    {
        m_CameraSensitivityBase = m_CameraSensitivity;
        SetCameraLocation();
    }

    private void Update()
    {
        SetCameraLocation();
        UpdateCameraTarget(new Vector3(Input.GetAxis(k_MouseX), Input.GetAxis(k_MouseY), Input.GetAxis(k_MouseWheel)));
    }


    internal void UpdateCameraTarget(Vector3 mouseScrollDelta)
    {
        m_CameraSensitivity = m_CameraSensitivityBase;
        m_DesiredCameraX -= mouseScrollDelta.x * m_CameraSensitivity * Time.deltaTime;
        m_DesiredCameraY -= mouseScrollDelta.y * m_CameraSensitivity * Time.deltaTime;
        m_DesiredCameraZoom -= mouseScrollDelta.z * 100 * Time.deltaTime;

        m_CameraX = Mathf.SmoothDamp(m_CameraX, m_DesiredCameraX, ref m_CameraXSmoothVelocity, 0);

        m_DesiredCameraY = Mathf.Clamp(m_DesiredCameraY, m_YRotationMinimum, m_YRotationMaximum);
        m_CameraY = Mathf.SmoothDamp(m_CameraY, m_DesiredCameraY, ref m_CameraYSmoothVelocity, 0);
        
        m_DesiredCameraZoom = Mathf.Clamp(m_DesiredCameraZoom, m_MinimumCameraZoom, m_MaximumCameraZoom);
        m_CameraZoom = Mathf.SmoothDamp(m_CameraZoom, m_DesiredCameraZoom, ref m_CameraZoomVelocity, 0);

        SetCameraLocation();
    }

    private void SetCameraLocation()
    {
        float zoom = m_CameraOffset.z - m_CameraZoom;
        Vector3 targetPosition = m_Target.transform.position;
        Vector2 rotationXNormal = new Vector2(Mathf.Cos(m_CameraX * Mathf.Deg2Rad), Mathf.Sin(m_CameraX * Mathf.Deg2Rad));
        Vector2 rotationYNormal = new Vector2(Mathf.Cos(m_CameraY * Mathf.Deg2Rad), -Mathf.Sin(m_CameraY * Mathf.Deg2Rad));
        Vector3 newDirection = new Vector3(rotationXNormal.x * rotationYNormal.x, rotationYNormal.y, rotationXNormal.y * rotationYNormal.x).normalized;

        float collisionOffset = 0;

        if (Physics.Linecast(m_Target.transform.position, m_Target.transform.position + newDirection * zoom, out RaycastHit hit, m_CollisionLayers))
        {
            collisionOffset = zoom + hit.distance;
        }

        newDirection *= zoom - collisionOffset;

        m_CameraLocation = new Vector3(newDirection.x + m_CameraOffset.x, newDirection.y + m_CameraOffset.y, newDirection.z) + targetPosition;

        transform.position = m_CameraLocation;
        transform.LookAt(new Vector3(targetPosition.x + m_CameraOffset.x, targetPosition.y + m_CameraOffset.y, targetPosition.z), Vector3.up);
    }
}