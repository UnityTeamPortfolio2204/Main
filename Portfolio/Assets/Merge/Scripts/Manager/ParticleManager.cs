using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 Add "ParticleManager.instance.CreateParticle();" in GameManager

 */
public class ParticleManager : MonoBehaviour
{
    static private ParticleManager _instance;

    static public ParticleManager instance
    {
        get
        {
            if(_instance == null)
            {
                GameObject gameObject = new GameObject("ParticleManager");
                _instance = gameObject.AddComponent<ParticleManager>();
            }

            return _instance;
        }
    }

    private Dictionary<string, List<GameObject>> totalParticle = new Dictionary<string, List<GameObject>>();

    public void CreateParticle()
    {
        AddParticle("Blood");
        AddParticle("Flame");
        AddParticle("Fire");
        AddParticle("Attack");
    }

    private void AddParticle(string key, int poolCount = 20)
    {
        GameObject prefab = Resources.Load<GameObject>("Particles/" + key);

        List<GameObject> particles = new List<GameObject>();

        for(int i = 0; i < poolCount; i++)
        {
            GameObject particle = Instantiate(prefab, transform);
            particle.SetActive(false);
            particle.name = key + "_" + i;

            particles.Add(particle);
        }

        totalParticle.Add(key, particles);
    }

    public void Play(string key, Vector3 pos, Quaternion rot)
    {
        if (!totalParticle.ContainsKey(key)) return;

        foreach(GameObject particle in totalParticle[key])
        {
            if(!particle.activeSelf)
            {
                particle.transform.position = pos;
                particle.transform.rotation = rot;
                particle.SetActive(true);
                return;
            }
        }
    }
}
