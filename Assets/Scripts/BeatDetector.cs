using UnityEngine;
using Photon.Pun;
using Unity.XR.CoreUtils;
using Unity.VisualScripting;

public class ImprovedBeatDetector : MonoBehaviour
{
    public AudioSource audioSource;
    public float sensitivity = 1.5f;
    public float minThreshold = 0.0005f;
    public GameObject leftParticlePrefab;  // Assign in Inspector
    public GameObject rightParticlePrefab; // Assign in Inspector

    public GameObject parentObject;

    private float[] spectrumData = new float[256];
    private float previousAverage = 0.0f;

    void Start()
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }

    void Update()
    {
        if (audioSource.isPlaying)
        {
            DetectBeat();
        }
    }

    void DetectBeat()
    {
        audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);

        float currentAverage = 0f;
        foreach (float sample in spectrumData)
        {
            currentAverage += sample;
        }
        currentAverage /= spectrumData.Length;

        if (currentAverage > minThreshold && currentAverage > previousAverage * sensitivity)
        {
            OnBeat();
        }

        previousAverage = currentAverage;
    }

   void OnBeat()
{
    // Find all game objects with the tag "Player"
    /*
    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

    foreach (GameObject player in players)
    {
        // Check if the player GameObject is controlled by this client
        PhotonView photonView = player.GetComponent<PhotonView>();
        if (photonView != null && photonView.IsMine)
        {
            // Spawn particle effects with slight position variations
            SpawnParticleEffects(player.transform);
            Debug.Log("Beat detected and player is controlled by this client!");
        }
    }

    Debug.Log("Beat detected!");*/

    // go through all the children of the parent object
    foreach (Transform child in parentObject.transform)
    {
        // Check if the player GameObject is controlled by this client
        GameObject bar = child.gameObject.GetNamedChild("Barrier");
        // get the renderes of material and change to neon colors; 
        if (bar != null)
        {
            // Spawn particle effects with slight position variations

            Color RandomNeon = Random.ColorHSV(0f, 1f, 0.8f, 1f, 0.9f, 1f);
            // check if it is greenish and if yes reroll                    
            while (RandomNeon.g > 0.5f)
            {
                RandomNeon = Random.ColorHSV(0f, 1f, 0.8f, 1f, 0.9f, 1f);
            }
            // change opacity of the color
            RandomNeon.a = 0.25f;

           bar.GetComponent<Renderer>().material.color = RandomNeon;
            Debug.Log("Beat detected and player is controlled by this client!");
        }
    }


}
    void SpawnParticleEffects(Transform playerTransform)
    {
        Debug.Log("Spawning particle effects!");
        // Random position variation
        Vector3 leftOffset = new Vector3(-70.0f + Random.Range(-10f, 10f), 70.0f + Random.Range(-10f, 10f), 70);
        Vector3 rightOffset = new Vector3(70.0f + Random.Range(-10f, 10f), 70.0f + Random.Range(-10f, 10f), 70);

        // Instantiate particle effects
        GameObject leftParticle = Instantiate(leftParticlePrefab, playerTransform.position + leftOffset, playerTransform.rotation);
        GameObject rightParticle = Instantiate(rightParticlePrefab, playerTransform.position + rightOffset, playerTransform.rotation);

        // Set neon colors
        SetNeonColor(leftParticle);
        SetNeonColor(rightParticle);

        // Destroy particles after 2 seconds
        Destroy(leftParticle, 2f);
        Destroy(rightParticle, 2f);
    }

    void SetNeonColor(GameObject particleObject)
    {
        ParticleSystemRenderer particleRenderer = particleObject.GetComponent<ParticleSystemRenderer>();
        if (particleRenderer != null)
        {
            // Generate a neon color
            Color neonColor = new Color(Random.value, Random.value, Random.value) * 2f;

            // Set the color on the material of the renderer
            if (particleRenderer.material != null)
            {
                particleRenderer.material.color = neonColor;
            }

            // Optionally, if you want to ensure the material has a property for emission or a similar effect:
            if (particleRenderer.material.HasProperty("_EmissionColor"))
            {
                particleRenderer.material.SetColor("_EmissionColor", neonColor);
            }
        }
    }
}
