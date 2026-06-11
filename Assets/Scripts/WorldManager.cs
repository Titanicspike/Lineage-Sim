using UnityEngine;
using System;

public class WorldManager : MonoBehaviour
{

    System.Random rng = new System.Random();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool Dies(Person person)
    {
        int age = person.Age;
        float health = person.health;
        float infantHealth = person.infantHealth;
        float adolescentFactor = person.adolescentFactor;

        double x2 = age * age;

        double logNumerator =
            Math.Log(infantHealth) + x2 * Math.Log(adolescentFactor);

        double logDenominatorTerm =
            logNumerator + ((5.0 * age / health) - 5.0) * Math.Log(10);

        // Use log-sum-exp trick style stability
        double maxLog = Math.Max(0, logDenominatorTerm);

        double denom =
            Math.Exp(-maxLog) +
            Math.Exp(logDenominatorTerm - maxLog);

        double numer =
            Math.Exp(logNumerator - maxLog);

        return rng.NextDouble() < (numer / denom);
    }
}
