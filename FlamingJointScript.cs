using System;
using UnityEngine;

namespace DestructionEffects
{
	public class FlamingJointScript : MonoBehaviour
	{
		float maxCombineDistance = 0.6f;
		
		float shrinkRateSmoke = 1f;
		float shrinkRateFlame = 0.35f;
		
		float highestEnergy = 0;
		float destroyTimerStart = 0;
		GameObject destroyer;
		
		public void Start()
		{
			foreach(GameObject otherFlame in FlamingJoints.flameObjects)
			{
				if((gameObject.transform.position-otherFlame.transform.position).sqrMagnitude < (maxCombineDistance * maxCombineDistance))	
				{
					Debug.Log ("== Flame combined ==");
					Destroy(gameObject);
					return;
				}
			}
					
			foreach(var pe in gameObject.GetComponentsInChildren<KSPParticleEmitter>())
			{
				Color color = pe.material.color;
				color.a = color.a/2;
				pe.material.SetColor("_TintColor", color);
				pe.force = -FlightGlobals.getGeeForceAtPosition(transform.position)/3;
				if(pe.maxEnergy > highestEnergy)
				{
					destroyer = pe.gameObject;
					highestEnergy = pe.maxEnergy;
				}
			}
			FlamingJoints.flameObjects.Add(gameObject);
			
		}
		
		public void FixedUpdate()
		{
			foreach(var pe in gameObject.GetComponentsInChildren<KSPParticleEmitter>())
			{
				float shrinkRate = pe.gameObject.name.Contains("smoke") ? shrinkRateSmoke : shrinkRateFlame;
				pe.maxSize = Mathf.MoveTowards(pe.maxSize, 0, shrinkRate * Time.fixedDeltaTime);
				pe.minSize = Mathf.MoveTowards(pe.minSize, 0, shrinkRate * Time.fixedDeltaTime);
				if(pe.maxSize < 0.1f && pe.gameObject == destroyer && destroyTimerStart == 0)
				{
					destroyTimerStart = Time.time;	
				}
				
				if(pe.gameObject.GetComponent<Light>())
				{
					pe.gameObject.GetComponent<Light>().intensity = UnityEngine.Random.Range(0f, pe.maxSize/6);
				}
			}
			
			
			
			if(destroyTimerStart != 0 && Time.time-destroyTimerStart > highestEnergy)
			{
				GameObject.Destroy(gameObject);
			}
		}
		
		void OnDestroy()
		{
			if(FlamingJoints.flameObjects.Contains(gameObject))
			{
				FlamingJoints.flameObjects.Remove(gameObject);	
			}
		}
	}
}

