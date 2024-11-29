using UnityEngine;

namespace ArcheryGame
{
    public class CameraCloseUpController : MonoBehaviour
    {
        [SerializeField] private Transform _hitTarget;
        [SerializeField] private Vector3 _closeUpOffset = new Vector3(0, 0, -10);
        [SerializeField] private float _closeUpSize = 2f;
        [SerializeField] private float _defaultSize = 5f;

        private Camera _mainCamera;
        private Vector3 _defaultPosition;

        private void Start()
        {
            _mainCamera = Camera.main;
            _defaultPosition = _mainCamera.transform.position;
            _defaultSize = _mainCamera.orthographicSize;
        }
   
        public void ShowCloseUp()
        {
            _mainCamera.transform.position = _hitTarget.position + _closeUpOffset;
            _mainCamera.orthographicSize = _closeUpSize;
        }

        public void ShowDefaultView()
        {
            _mainCamera.transform.position = _defaultPosition;
            _mainCamera.orthographicSize = _defaultSize;
        }
    }
}
