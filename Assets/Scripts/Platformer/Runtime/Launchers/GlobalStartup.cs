using UnityEngine;
using UnityEngine.SceneManagement;

namespace Local.Launchers {

    public sealed class GlobalStartup : MonoBehaviour {

        [SerializeField] [Range(1, 240)] private int _targetFrameRate = 120;
        [SerializeField] private string _startScene;

        private void Awake() {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Application.targetFrameRate = _targetFrameRate;
            SceneManager.LoadScene(_startScene, LoadSceneMode.Additive);
        }

        private void OnValidate() {
            Application.targetFrameRate = _targetFrameRate;
        }
    }

}
