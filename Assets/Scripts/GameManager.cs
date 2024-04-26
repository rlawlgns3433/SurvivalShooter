using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // �̱��� ���ٿ� ������Ƽ
    public static GameManager instance
    {
        get
        {
            // ���� �̱��� ������ ���� ������Ʈ�� �Ҵ���� �ʾҴٸ�
            if (m_instance == null)
            {
                // ������ GameManager ������Ʈ�� ã�� �Ҵ�
                m_instance = FindObjectOfType<GameManager>();
            }

            // �̱��� ������Ʈ�� ��ȯ
            return m_instance;
        }
    }

    private static GameManager m_instance; // �̱����� �Ҵ�� static ����

    public int Score { get; set; } = 0; // ���� ���� ����
    public bool isGameover { get; private set; } // ���� ���� ����
    public bool isResume { get; private set; }

    //public AudioMixer musicAudioMixer;
    //public AudioMixer effectAudioMixer;

    public AudioSource bgmPlayer;
    public AudioClip backgroundMusic;

    private void Awake()
    {
        // ���� �̱��� ������Ʈ�� �� �ٸ� GameManager ������Ʈ�� �ִٸ�
        if (instance != this)
        {
            // �ڽ��� �ı�
            Destroy(gameObject);
        }
        isGameover = false;
    }

    private void Start()
    {
        // �÷��̾� ĳ������ ��� �̺�Ʈ �߻��� ���� ����
        //FindObjectOfType<PlayerHealth>().onDeath += EndGame;

        var pos = Random.insideUnitSphere * 10f;
        if (NavMesh.SamplePosition(pos, out var hit, 10f, NavMesh.AllAreas))
        {
            pos = hit.position;
        }
        else
        {
            pos = Vector3.zero;
        }

        pos.y += 0.1f;

        //UIManager.instance.musicSlider.onValueChanged.AddListener(SetMusicVolume);
        //UIManager.instance.effectSlider.onValueChanged.AddListener(SetEffectVolume);

        bgmPlayer.clip = backgroundMusic;
        bgmPlayer.Play();
    }

    private void Update()
    {
        if (isResume)
        {
            AudioSource[] effects = FindObjectsOfType<AudioSource>();

            foreach (var effect in effects)
            {
                if (effect == backgroundMusic) continue;
                effect.volume = UIManager.instance.effectSlider.value;
            }

            bgmPlayer.volume = UIManager.instance.musicSlider.value;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isResume = !UIManager.instance.pauseUI.activeInHierarchy;
            UIManager.instance.SetActivePauseUI(isResume);
        }
    }

    // ������ �߰��ϰ� UI ����
    public void AddScore(int newScore)
    {
        // ���� ������ �ƴ� ���¿����� ���� ���� ����
        if (!isGameover)
        {
            // ���� �߰�
            Score += newScore;
            // ���� UI �ؽ�Ʈ ����
            UIManager.instance.UpdateScoreText(Score);
        }
    }

    // ���� ���� ó��
    public void EndGame()
    {
        // ���� ���� ���¸� ������ ����
        isGameover = true;
        // ���� ���� UI�� Ȱ��ȭ
        //UIManager.instance.SetActiveGameoverUI(true);
    }

    //public void SetMusicVolume(float volume)
    //{
    //    musicAudioMixer.SetFloat("musicVol", Mathf.Log10(volume) * 20);
    //}
    //public void SetEffectVolume(float volume)
    //{
    //    effectAudioMixer.SetFloat("Player", Mathf.Log10(volume) * 20);
    //    effectAudioMixer.SetFloat("Enemies", Mathf.Log10(volume) * 20);
    //    effectAudioMixer.SetFloat("Gunshots", Mathf.Log10(volume) * 20);
    //}
}