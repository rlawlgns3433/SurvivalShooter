using UnityEngine;
using UnityEngine.SceneManagement; // �� ������ ���� �ڵ�
using UnityEngine.UI; // UI ���� �ڵ�
using TMPro;
using UnityEngine.Audio;

// �ʿ��� UI�� ��� �����ϰ� ������ �� �ֵ��� ����ϴ� UI �Ŵ���
public class UIManager : MonoBehaviour
{
    // �̱��� ���ٿ� ������Ƽ
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

    private static UIManager m_instance; // �̱����� �Ҵ�� ����

    public TextMeshProUGUI scoreText; // ���� ǥ�ÿ� �ؽ�Ʈ
    public GameObject pauseUI; // ���� ������ Ȱ��ȭ�� UI
    public Slider musicSlider;
    public Slider effectSlider;

    // ���� �ؽ�Ʈ ����
    public void UpdateScoreText(int newScore)
    {
        scoreText.text = "Score : " + newScore;
    }

    // ���� ���� UI Ȱ��ȭ
    public void SetActivePauseUI(bool active)
    {
        Time.timeScale = active ? 0f : 1f;
        pauseUI.SetActive(active);
    }

    // ���� �����
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