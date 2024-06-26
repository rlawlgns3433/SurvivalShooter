using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // 싱글톤 접근용 프로퍼티
    public static GameManager instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (m_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                m_instance = FindObjectOfType<GameManager>();
            }

            // 싱글톤 오브젝트를 반환
            return m_instance;
        }
    }

    private static GameManager m_instance; // 싱글톤이 할당될 static 변수

    public int Score { get; set; } = 0; // 현재 게임 점수
    public bool isGameover { get; private set; } // 게임 오버 상태
    public bool isResume { get; private set; }

    //public AudioMixer musicAudioMixer;
    //public AudioMixer effectAudioMixer;

    public AudioSource bgmPlayer;
    public AudioClip backgroundMusic;

    private float bgmVolume;
    private float effectVolume;
    private bool isAllSoundPlaying = true;

    private void Awake()
    {
        // 씬에 싱글톤 오브젝트가 된 다른 GameManager 오브젝트가 있다면
        if (instance != this)
        {
            // 자신을 파괴
            Destroy(gameObject);
        }
        isGameover = false;
    }

    private void Start()
    {
        // 플레이어 캐릭터의 사망 이벤트 발생시 게임 오버
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
        bgmPlayer.loop = true;
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

    // 점수를 추가하고 UI 갱신
    public void AddScore(int newScore)
    {
        // 게임 오버가 아닌 상태에서만 점수 증가 가능
        if (!isGameover)
        {
            // 점수 추가
            Score += newScore;
            // 점수 UI 텍스트 갱신
            UIManager.instance.UpdateScoreText(Score);
        }
    }

    // 게임 오버 처리
    public void EndGame()
    {
        // 게임 오버 상태를 참으로 변경
        isGameover = true;
        // 게임 오버 UI를 활성화
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

    public void AllSound()
    {
        AudioSource[] effects = FindObjectsOfType<AudioSource>();
        if (isAllSoundPlaying)
        {
            effectVolume = UIManager.instance.effectSlider.value;
            bgmVolume = UIManager.instance.musicSlider.value;

            UIManager.instance.effectSlider.value = 0;
            UIManager.instance.musicSlider.value = 0;
            isAllSoundPlaying = !isAllSoundPlaying;
        }
        else
        {
            UIManager.instance.effectSlider.value = effectVolume;
            UIManager.instance.musicSlider.value = bgmVolume;
            isAllSoundPlaying = !isAllSoundPlaying;
        }
    }
}