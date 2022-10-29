using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    static public ParticleManager instance;

    private Dictionary<string, List<GameObject>> totalParticle = new Dictionary<string, List<GameObject>>();

    private void Awake()
    {
        instance = this;
    }

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
