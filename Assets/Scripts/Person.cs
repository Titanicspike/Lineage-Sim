using UnityEngine;
using System.Collections.Generic;
using System;

public class Person
{
    public enum Gender { Male, Female }
    public Gender gender;
    public string firstName;
    public string surname;
    public float isAlive;
    public Person father;
    public Person mother;
    public bool married;
    public Person spouse;
    public int numChildren;
    public List<Person> children;
    public Vector2 location;
    public float health;
    public float infantHealth;
    public float adolescentFactor;
    public int fertilityStartAge;
    public int fertilityEndAge;
    public double baseFertility;
    public double fertilityDesire;
    public int birthYear;
    public int marriageYear;
    public int deathYear;
    public int Age(int currentYear) => currentYear - birthYear;

    public bool Dies(System.Random rng, int age)
    {
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

    public bool GivesBirth(System.Random rng, int age)
    {
        if(age < fertilityStartAge || age > fertilityEndAge)
        {
            return false;
        }
        fertilityDesire = 
    }
}
