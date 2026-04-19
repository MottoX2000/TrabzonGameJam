using UnityEngine;

public class ZombieSoundController : MonoBehaviour // FATÝH
{
    [Header("Ses Ýsimleri (SoundManager'daki isimler)")]
    //[SerializeField] private string[] idleSounds = { "ZombieIdle1", "ZombieIdle2", "ZombieIdle3" };
    [SerializeField] private string[] hurtSounds = { "ZombieHurt1", "ZombieHurt2", "ZombieHurt3" };
    [SerializeField] private string[] walkSounds = { "ZombieWalk1", "ZombieWalk2", "ZombieWalk3" };

    [Header("Ayarlar")]
    [SerializeField] private float minIdleDelay = 3f;
    [SerializeField] private float maxIdleDelay = 7f;
    [SerializeField] private float walkSoundDelay = 0.6f;

    //private float _nextIdleTime;
    private float _walkTimer;
    private string _assignedWalkSound;

    private Zombie _zombie;

    void Start()
    {
        _zombie = GetComponent<Zombie>();
        //SetNextIdleTime();

        if (walkSounds != null && walkSounds.Length > 0)
        {
            _assignedWalkSound = walkSounds[Random.Range(0, walkSounds.Length)];
        }
    }

    /*void Update()
    {
        if (_zombie.IsDead) return;

        if (Time.time >= _nextIdleTime)
        {
            PlayRandomSound(idleSounds);
            SetNextIdleTime();
        }
    }*/

    public void PlayWalkSound()
    {
        if (_zombie.IsDead || string.IsNullOrEmpty(_assignedWalkSound)) return;

        _walkTimer -= Time.deltaTime;

        if (_walkTimer <= 0)
        {
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySFX(_assignedWalkSound);
            }
            _walkTimer = walkSoundDelay;
        }
    }
    public void PlayHurtSound()
    {
        PlayRandomSound(hurtSounds);
    }

    public void PlayDeathSound()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySFX("ZombieDeath");
    }

    public void PlayAttackSound()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySFX("ZombieAttack");
    }

    private void PlayRandomSound(string[] soundList)
    {
        if(_zombie.IsDead) return;
        if (soundList == null || soundList.Length == 0) return;

        int randomIndex = Random.Range(0, soundList.Length);
        string soundToPlay = soundList[randomIndex];

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(soundToPlay);
        }
    }

    /*private void SetNextIdleTime()
    {
        _nextIdleTime = Time.time + Random.Range(minIdleDelay, maxIdleDelay);
    }*/
}