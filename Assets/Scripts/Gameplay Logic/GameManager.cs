// Ethan Le (8/6/2026):
using UnityEngine;

/** 
 * Global static script for managing game logic:
**/ 
public class GameManager : MonoBehaviour 
{
    public static GameManager Instance { get; private set; }

    // Variable for determining who is the imposter in this playthrough: 
    int imposterIndex; 

    // SerializedFields of the 6 different characters (excluding captain) who can potentially be imposters: 
    // We retrieve their bool isImposter flags and set the according one to true. 
    [SerializeField] NavigationOfficer navOfficer; 
    [SerializeField] Cook cook;
    [SerializeField] Engineer engineer;
    [SerializeField] Doctor doctor;
    [SerializeField] RichGuy guy;
    [SerializeField] RichGirl girl; 

    void Awake() 
    {
        if (Instance != null) // If instance already exists, destroy newly created copy. 
        {
            Destroy(gameObject); 
            return; 
        }

        // Otherwise, set this first instance to be the static instance:
        Instance = this; 
        DontDestroyOnLoad(gameObject); 

        StartGame(); 
    }

    // Pressing "Start" on the title screen should call this function to choose an imposter: 
    void StartGame()
    {
        imposterIndex = Random.Range(0, 6); 

        Debug.Log("Imposter index is: " + imposterIndex); 

        SetImposter(imposterIndex); // Set the respective character's imposter flag to true. 
    }

    // Helper function to set the imposter's flag to true: 
    void SetImposter(int imposter)
    {
        switch (imposter)
        {
            case 0: 
                navOfficer.isImposter = true; 
                break;
            case 1:
                cook.isImposter = true; 
                break;
            case 2:
                engineer.isImposter = true; 
                break; 
            case 3:
                doctor.isImposter = true;
                break; 
            case 4:
                guy.isImposter = true;
                break;
            case 5:
                girl.isImposter = true;
                break; 
            default:
                navOfficer.isImposter = true;
                break; 
        }

        Debug.Log("Navigation Officer imposter? " + navOfficer.isImposter);
        Debug.Log("Cook imposter? " + cook.isImposter);
        Debug.Log("Engineer imposter? " + engineer.isImposter);
        Debug.Log("Doctor imposter? " + doctor.isImposter);
        Debug.Log("Rich Guy imposter? " + guy.isImposter);
        Debug.Log("Rich Girl imposter? " + girl.isImposter);
    }
}