using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리자 관련 코드
using UnityEngine.UI; // UI 관련 코드
using TMPro;
using UnityEngine.Audio;

// 필요한 UI에 즉시 접근하고 변경할 수 있도록 허용하는 UI 매니저
public class UIManager : MonoBehaviour
{
    // 싱글톤 접근용 프로퍼티
    public static UIManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<UIManager>();
            }

            return m_instance;
        }
    }

    private static UIManager m_instance; // 싱글톤이 할당될 변수

    public TextMeshProUGUI scoreText; // 점수 표시용 텍스트
    public GameObject pauseUI; // 게임 오버시 활성화할 UI
    public Slider musicSlider;
    public Slider effectSlider;

    // 점수 텍스트 갱신
    public void UpdateScoreText(int newScore)
    {
        scoreText.text = "Score : " + newScore;
    }

    // 게임 오버 UI 활성화
    public void SetActivePauseUI(bool active)
    {
        Time.timeScale = active ? 0f : 1f;
        pauseUI.SetActive(active);
    }

    // 게임 재시작
    public void GameRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }

    public void Resume()
    {
        SetActivePauseUI(!pauseUI.activeInHierarchy);
        Time.timeScale = 1f;
    }
}