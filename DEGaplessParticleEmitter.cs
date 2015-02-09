using System;
using UnityEngine;

namespace DestructionEffects
{
	public class DEGaplessParticleEmitter : MonoBehaviour
	{
		public KSPParticleEmitter pEmitter;	
		public float maxDistance = 1.1f;
		public bool emit = false;
		public Part part = null;
		public Rigidbody rb;
		
		private void Start ()
		{
			pEmitter = gameObject.GetComponent<KSPParticleEmitter>();	
			pEmitter.emit = false;
			if (part != null)
			{
                if (FlamingJoints.log) 
                    Debug.Log ("DE | DEGaplessParticleEmitter | Start () | Part " + part.partName + " explosionPotential: " + part.explosionPotential);	
			}
			maxDistance = pEmitter.minSize / 3f;
		}

        private void FixedUpdate ()
		{
			if (emit)
			{
				Vector3 velocity = (part == null) ? rb.velocity : part.rigidbody.velocity;
				Vector3 originalLocalPosition = gameObject.transform.localPosition;
				Vector3 originalPosition = gameObject.transform.position;
				Vector3 startPosition = gameObject.transform.position + (velocity * Time.fixedDeltaTime);
				float originalGapDistance = Vector3.Distance (originalPosition, startPosition);
				float intermediateSteps = originalGapDistance/maxDistance;

				pEmitter.EmitParticle ();
				gameObject.transform.position = Vector3.MoveTowards (gameObject.transform.position, startPosition, maxDistance);
				for (int i = 1; i < intermediateSteps; i++)
				{
					pEmitter.EmitParticle();
					gameObject.transform.position = Vector3.MoveTowards (gameObject.transform.position, startPosition, maxDistance);
				}
				gameObject.transform.localPosition = originalLocalPosition;
			}
			
		}
		
		public void EmitParticles ()
		{
			Vector3 velocity = (part == null) ? rb.velocity : part.rigidbody.velocity;
			Vector3 originalLocalPosition = gameObject.transform.localPosition;
			Vector3 originalPosition = gameObject.transform.position;
			Vector3 startPosition = gameObject.transform.position + (velocity * Time.fixedDeltaTime);

			float originalGapDistance = Vector3.Distance (originalPosition, startPosition);
			float intermediateSteps = originalGapDistance/maxDistance;
			for (int i = 0; i < intermediateSteps; i++)
			{
				pEmitter.EmitParticle();
				gameObject.transform.position = Vector3.MoveTowards (gameObject.transform.position, startPosition, maxDistance);
			}
			gameObject.transform.localPosition = originalLocalPosition;
		}
	}
}

