using UnityEngine;
using UnityEngine.SceneManagement;

namespace CoreDev.Examples
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private int sceneIndex = 0;

        [ContextMenu("LoadScene")]
        private void LoadScene()
        {
            SceneManager.LoadScene(sceneIndex);
        }
    }
}