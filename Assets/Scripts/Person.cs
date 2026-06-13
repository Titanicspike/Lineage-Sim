using UnityEngine;
using System.Collections.Generic;
using System;
public class Person
{
    public enum Gender { Male, Female }
    public Gender gender;
    public string firstName;
    public string surname;
    public bool isAlive;
    public Person father;
    public Person mother;
    public bool married;
    public bool wantsToMarry = true;
    public bool goingToMarry = false;
    public Person spouse;
    public int numChildren;
    public List<Person> children;
    public Vector2Int location;
    public double health;
    public double infantHealth;
    public double adolescentFactor;
    public int fertilityStartAge;
    public int fertilityEndAge;
    public int childExhaustion;
    public double exhaustionFactor;
    public double baseFertility;
    public double fertilityDesire;
    public int birthYear;
    public int marriageYear;
    public int deathYear;
    public int Age(int currentYear) => currentYear - birthYear;

    public Person(int currentYear, Gender _gender, string _firstName, string _surname, Person _father, Person _mother, Vector2Int _location, double _health = 0, double _infantHealth = 0, double _adolescentFactor = 0, int _fertilityStartAge = 0, int _fertilityEndAge = 0, int _childExhaustion = 0, double _exhaustionFactor = 0, double _baseFertility = 0)
    {
        isAlive = true;
        children = new List<Person>();
        birthYear = currentYear;
        gender = _gender;
        firstName = _firstName;
        surname = _surname;
        father = _father;
        mother = _mother;
        location = _location;
        health = _health;
        infantHealth = _infantHealth;
        adolescentFactor = _adolescentFactor;
        fertilityStartAge = _fertilityStartAge;
        fertilityEndAge = _fertilityEndAge;
        childExhaustion = _childExhaustion;
        exhaustionFactor = _exhaustionFactor;
        baseFertility = _baseFertility;
    }

    public bool Dies(System.Random rng, int age)
    {
        const double Divisor = 100000.0; // 10^5

        double xSquared = age * age;

        // First term: 10^(5x/h) / 10^5
        double term1 = Math.Pow(10, 5.0 * age / health) / Divisor;

        // Second term: 1 / (i_l^(x^2) * i_h)
        double term2 = 1.0 / (Math.Pow(adolescentFactor, xSquared) * infantHealth);

        return rng.NextDouble() < (term1 + term2);
    }

    public bool GivesBirth(System.Random rng, int age)
    {
        if(age < fertilityStartAge || age > fertilityEndAge)
        {
            return false;
        }
        fertilityDesire = Math.Min(fertilityDesire + baseFertility + Math.Min(0, childExhaustion - numChildren) * exhaustionFactor, 1);
        bool hasChild =  rng.NextDouble() < fertilityDesire;
        if(hasChild)
        {
            fertilityDesire = 0;
        }
        return hasChild;
    }

    public override string ToString()
    {        
        return $"{firstName} {surname}, Age: {Age(WorldManager.Instance.currentYear)}, Father: {(father != null ? father.firstName : "Unknown")}, Mother: {(mother != null ? $"{mother.firstName} {mother.surname}" : "Unknown")}, Children: {numChildren}";
    }
}