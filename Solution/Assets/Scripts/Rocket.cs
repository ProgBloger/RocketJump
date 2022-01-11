using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{

    [SerializeField] float flySpeed = 100;
    [SerializeField] float rotSpeed = 100;
    [SerializeField] AudioClip flySound;
    [SerializeField] AudioClip finishShound0;
    [SerializeField] AudioClip finishShound1;
    [SerializeField] AudioClip finishShound2;
    [SerializeField] AudioClip finishShound3;
    [SerializeField] AudioClip deathSound;
    [SerializeField] ParticleSystem boomParticles;
    [SerializeField] ParticleSystem finishParticles;
    [SerializeField] ParticleSystem flyParticles;

    AudioClip[] finishSounds;
    Rigidbody rigidBody; 
    AudioSource audioSource;

    enum State {Playing, Dead, NextLevel };
    
    State state = State.Playing;
    const int maxLevel = 16;
    const int minLevel = 1;
    bool collisionOn = true;
    
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        finishSounds = new AudioClip []{finishShound0, finishShound1, finishShound2, finishShound3};
    }

    // Update is called once per frame
    void Update()
    {
        Launch();
        Rotation();
        DebugKeys();
    }

    void DebugKeys()
    {
        if(Input.GetKeyDown(KeyCode.N))
        {
            LoadNextLevel();
        }
        else if(Input.GetKeyDown(KeyCode.P)){
            LoadPreviousLevel();
        }
        else if(Input.GetKeyDown(KeyCode.C)){
            collisionOn = !collisionOn;
            print($"collisionOn switched to {collisionOn}");
        }
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if(state == State.Dead || state == State.NextLevel)
        {
            return;
        }
 
        switch(collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                print("Switched to Finish");
                Finish();
                break;
            case "Battery":
                break;
            default:
                if(collisionOn)
                {
                    print($"collisionOn is {collisionOn} and you gonna Lose");
                    Lose();
                }
                break;
        }
    }

    void Lose(){
        
        state = State.Dead;
        audioSource.Stop();
        audioSource.PlayOneShot(deathSound);
        boomParticles.Play();
        Invoke("LoadPreviousLevel", 2f);
    }

    void Finish(){
        print("Entered Finish");
        state = State.NextLevel;
        var soundId = Random.Range(0,3);
        audioSource.Stop();
        audioSource.PlayOneShot(finishShound0);
        finishParticles.Play();
        print("LoadNextLevel about to execute");
        Invoke("LoadNextLevel", 2f);
    }

    void LoadNextLevel(){
        int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        int nextLevelIndex = currentLevelIndex;

        if(currentLevelIndex < maxLevel)
        {
            nextLevelIndex++;
        }

        print($"LoadingNextLevel current level {currentLevelIndex}, next level {nextLevelIndex}");

        SceneManager.LoadScene(nextLevelIndex);
    }

    void LoadPreviousLevel(){

        int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        int previousLevelIndex = currentLevelIndex;
        
        if(currentLevelIndex > minLevel)
        {
            previousLevelIndex--;
        }

        print($"LoadingNextLevel current level {currentLevelIndex}, prev level {previousLevelIndex}");

        SceneManager.LoadScene(previousLevelIndex);
        }

    void Rotation(){
        
        float rotationSpeed = rotSpeed * Time.deltaTime;

        rigidBody.freezeRotation = true;
        if(Input.GetKey(KeyCode.A))
        {
            // print("A pressed");
            transform.Rotate(Vector3.forward * rotationSpeed);
        }
        else if(Input.GetKey(KeyCode.D))
        {
            // print("D pressed");
            transform.Rotate(Vector3.back * rotationSpeed);
        }
            
        rigidBody.freezeRotation = true;
    }

    void Launch()
    {
        if(Input.GetKey(KeyCode.Space))
        {
            rigidBody.AddRelativeForce(Vector3.up * flySpeed * Time.deltaTime);
            if(audioSource.isPlaying == false && state == State.Playing)
            {
                audioSource.PlayOneShot(flySound);
            }
            if(!flyParticles.isPlaying)
                flyParticles.Play();
        }
        else if(state == State.Playing){
            audioSource.Pause();
            if(flyParticles.isPlaying)
                flyParticles.Stop();
        }
    }
}
