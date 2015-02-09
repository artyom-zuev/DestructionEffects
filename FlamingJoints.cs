using System;
using UnityEngine;
using System.Collections.Generic;

namespace DestructionEffects
{
	[KSPAddon(KSPAddon.Startup.Flight, false)]
	public class FlamingJoints : MonoBehaviour
	{
		public static List<GameObject> flameObjects = new List<GameObject>();
        public static bool log = false;
		
		public void Start ()
		{
			GameEvents.onPartJointBreak.Add(onPartJointBreak);
		}
		
		public void onPartJointBreak (PartJoint partJoint)
		{
			if (partJoint.Target != null && partJoint.Target.PhysicsSignificance != 1)
			{
				Part part = partJoint.Target;
				bool attachFlames = false;
				if 
                (
                    part.partInfo.title.Contains("Wing") || 
					part.partInfo.title.Contains("Fuselage") || 
					part.FindModuleImplementing<ModuleEngines>() || 
					part.FindModuleImplementing<ModuleEnginesFX>()
                )
				{
					attachFlames = true;	
				}
				else
				{
                    PartResourceList resources = part.Resources;
                    for (int i = 0; i < resources.Count; ++i)
                    {
                        if (resources[i].resourceName.Contains ("Fuel") || resources[i].resourceName.Contains ("Ox"))
                        {
                            attachFlames = true;
                        }
                    }
				}
				if (attachFlames)
				{
					GameObject flameObject2 = (GameObject) GameObject.Instantiate(GameDatabase.Instance.GetModel("DestructionEffects/Models/FlameEffect/model"), partJoint.transform.position, Quaternion.identity);
					flameObject2.SetActive(true);
					flameObject2.transform.parent = partJoint.Target.transform;
					flameObject2.AddComponent<FlamingJointScript>();

                    KSPParticleEmitter[] emitters = flameObject2.GetComponentsInChildren<KSPParticleEmitter> ();
                    for (int i = 0; i < emitters.Length; ++i)
					{
                        if (emitters[i].useWorldSpace)	
						{
                            DEGaplessParticleEmitter gpe = emitters[i].gameObject.AddComponent<DEGaplessParticleEmitter> ();	
							gpe.part = partJoint.Target;
							gpe.emit = true;
						}
					}
				}
			}
    	}
	}
}

