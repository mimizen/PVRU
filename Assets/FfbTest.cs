using System.Collections;
using UnityEngine;

namespace UnityFFB
{
    public class FfbTest : MonoBehaviour
     {
        private UnityFFB ffb;

        // The interval in seconds for applying and stopping the force
        public float interval = 3f;

        // Force value to apply
        public int forceValue = 100;

        // Direction toggle
        private int direction = 1;

        private void Start()
        {
            // Find the UnityFFB instance in the scene
            ffb = UnityFFB.instance;

            if (ffb == null)
            {
                Debug.LogError("UnityFFB instance not found. Make sure the UnityFFB script is attached to a GameObject in the scene.");
                return;
            }

            // Start the coroutine to apply and stop force feedback periodically
            StartCoroutine(ApplyForceFeedback());
        }

        private IEnumerator ApplyForceFeedback()
        {
            while (true)
            {
                // Apply force feedback in one direction
                ffb.force = forceValue * direction;
                ffb.StartFFBEffects();
                Debug.Log("Force feedback applied. Direction: " + direction);

                // Wait for the specified interval
                yield return new WaitForSeconds(interval);

                // Stop force feedback
                ffb.StopFFBEffects();
                Debug.Log("Force feedback stopped.");

                // Wait for the specified interval before applying force feedback again
                yield return new WaitForSeconds(interval);

                // Toggle the direction
                direction *= -1;
            }
        }
    }
}