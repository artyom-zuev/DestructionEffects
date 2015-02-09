using System;
using UnityEngine;
using System.Collections.Generic;

namespace DestructionEffects
{
	public class FlamingJointScript : MonoBehaviour
	{
        private float maxCombineDistance = 0.6f;
        private float shrinkRateSmoke = 1f;
        private float shrinkRateFlame = 0.35f;
        private float highestEnergy = 0;
        private float destroyTimerStart = 0;
        private GameObject destroyer;
		
		public void Start ()
		{
            List<GameObject> flameObjects = FlamingJoints.flameObjects;
            for (int i = 0; i < flameObjects.Count; ++i)
            {
                if ((gameObject.transform.position - flameObjects[i].transform.position).sqrMagnitude < (maxCombineDistance * maxCombineDistance))	
				{
					if (FlamingJoints.log) 
                        Debug.Log ("DE | FlamingJointScript | Start () | Flame was combined");
					Destroy (gameObject);
					return;
				}
			}
            KSPParticleEmitter[] emitters = gameObject.GetComponentsInChildren<KSPParticleEmitter> ();
            for (int i = 0; i < emitters.Length; ++i)
            {
                Material material = emitters[i].material;
                Color color = material.color;
				color.a = color.a / 2f;
                material.SetColor ("_TintColor", color);

                emitters[i].force = -FlightGlobals.getGeeForceAtPosition (transform.position) / 3f;
                if (emitters[i].maxEnergy > highestEnergy)
				{
                    destroyer = emitters[i].gameObject;
                    highestEnergy = emitters[i].maxEnergy;
				}
			}
			FlamingJoints.flameObjects.Add (gameObject);
		}
		
		public void FixedUpdate ()
		{
            KSPParticleEmitter[] emitters = gameObject.GetComponentsInChildren<KSPParticleEmitter>();
            for (int i = 0; i < emitters.Length; ++i)
            {
                float shrinkRate = emitters[i].gameObject.name.Contains ("smoke") ? shrinkRateSmoke : shrinkRateFlame;
                emitters[i].maxSize = Mathf.MoveTowards (emitters[i].maxSize, 0, shrinkRate * Time.fixedDeltaTime);
                emitters[i].minSize = Mathf.MoveTowards (emitters[i].minSize, 0, shrinkRate * Time.fixedDeltaTime);
                if (emitters[i].maxSize < 0.1f && emitters[i].gameObject == destroyer && destroyTimerStart == 0)
                {
                    destroyTimerStart = Time.time;
                }
                Light light = emitters[i].gameObject.GetComponent<Light> ();
                if (light != null)
                {
                    light.intensity = UnityEngine.Random.Range (0f, emitters[i].maxSize / 6f);
                }
            }
			if (destroyTimerStart != 0 && Time.time - destroyTimerStart > highestEnergy)
			{
				GameObject.Destroy(gameObject);
			}
		}
		
		private void OnDestroy ()
		{
			if (FlamingJoints.flameObjects.Contains(gameObject))
			{
				FlamingJoints.flameObjects.Remove(gameObject);	
			}
		}
	}
}

