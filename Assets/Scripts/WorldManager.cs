using UnityEngine;
using System;
using System.Collections.Generic;

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance{ get; private set; }
    System.Random rng = new System.Random();

    public List<Person> people = new List<Person>();
    public Dictionary<(int, int), HashSet<Person>> grid = new Dictionary<(int, int), HashSet<Person>>();
    public float gridCellSize = 10000.0f;
    public float ticksPerSecond = 1.0f;
    public int currentYear = 0;

    public int initialPopulation = 1000;

    private void Awake()
    {
        // Enforce the Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Delete duplicate instances
            return;
        }

        Instance = this;

        // Optional: Keep this object alive across scene transitions
        // DontDestroyOnLoad(gameObject); 
    }


    float tickTimer = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        NameGenerator.Initialize();
        for(int i = 0; i < initialPopulation; i++)
        {
            Vector2Int location = new Vector2Int(rng.Next(-5000, 5000), rng.Next(-5000, 5000));
            Person p = CreatePerson1500s(location);
            people.Add(p);
            Move(p, location);
            AddToGrid(p);
        }
    }

    // Update is called once per frame
    void Update()
    {
        tickTimer += Time.deltaTime;
        if(tickTimer >= 1.0f / ticksPerSecond)
        {
            Tick();
            tickTimer = 0.0f;
        }
    }

    public void Tick()
    {
        currentYear++;
        foreach(Person person in people)
        {
            if(person.isAlive)
            {
                if(person.Dies(rng, person.Age(currentYear)))
                    Kill(person);
            }
        }
        List<Person> willGiveBirth = new List<Person>();
        foreach(Person person in people)
        {
            if(person.isAlive && person.gender == Person.Gender.Female && person.married)
            {
                if(person.GivesBirth(rng, person.Age(currentYear)))
                {
                    willGiveBirth.Add(person);
                }
                if(rng.NextDouble() < 0.01)
                {
                    Kill(person);
                }
            }
            if(person.isAlive && !person.married && person.wantsToMarry && (person.Age(currentYear) - 10)/40.0 > rng.NextDouble())
            {
                person.goingToMarry = true;
            }
        }
        foreach(Person person in people)
        {
            if(person.goingToMarry)
            {
                Person match = GetRandomPersonToMarry(person, 1000);
                if(match != null)
                {
                    Marry(person, match);
                    //print(person + " married " + match);
                }

            }
        }
        foreach(Person mother in willGiveBirth)
        {
            Person child = new Person(currentYear, RandGender(), NameGenerator.GenerateFirstName(mother), "", mother.spouse, mother, mother.location);
            people.Add(child);
            grid[GetGridPosition(child.location)].Add(child);
            AttributeVariation(mother, mother.spouse, child);
            mother.numChildren++;
            mother.spouse.numChildren++;
            if(rng.NextDouble() < 0.001)
            {
                child.surname = NameGenerator.GenerateLastName();
            }
            else
            {
                child.surname = mother.spouse.surname;
            }
        }
        WorldRender.Instance.RenderWorld();
    }

    public void Kill(Person person)
    {
        //print("Died: " + person);
        person.isAlive = false;
        person.deathYear = currentYear;
        if(person.married)
        {
            person.spouse.married = false;
            person.spouse.spouse = null;
            person.spouse.wantsToMarry = false;
        }
        grid[GetGridPosition(person.location)].Remove(person);
    }

    public void Marry(Person person1, Person person2)
    {
        person1.married = true;
        person1.spouse = person2;
        person2.married = true;
        person2.spouse = person1;
        person1.marriageYear = currentYear;
        person2.marriageYear = currentYear;
        person1.wantsToMarry = false;
        person2.wantsToMarry = false;
        person1.goingToMarry = false;
        person2.goingToMarry = false;
    }

    public void Move(Person person, Vector2Int newLocation)
    {
        var oldGridPos = GetGridPosition(person.location);
        var newGridPos = GetGridPosition(newLocation);
        if(oldGridPos != newGridPos)
        {
            if(grid.ContainsKey(oldGridPos))
            {
                grid[oldGridPos].Remove(person);
            }
            AddToGrid(person);
        }
        person.location = newLocation;
    }

    public Person CreatePerson1500s(Vector2Int location)
    {
        // Gender
        Person.Gender gender = RandGender();

        // Names
        string firstName = NameGenerator.GenerateFirstName(gender);
        string surname = NameGenerator.GenerateLastName();

        // Core survival stats (1500s baseline tuning)
        double health = Math.Max(1, Normal(70, 25));
        double infantHealth = Math.Max(1, Normal(5, 1));
        double adolescentFactor = Math.Max(1.05, Normal(1.1, 0.01));

        // Fertility tuning (higher but unstable population conditions)
        int fertilityStart = gender == Person.Gender.Female
            ? rng.Next(14, 18)
            : rng.Next(13, 16);

        int fertilityEnd = gender == Person.Gender.Female
            ? rng.Next(35, 46)
            : rng.Next(40, 55);

        int childExhaustion = rng.Next(2, 6);
        double exhaustionFactor = rng.NextDouble() * 0.15 + 0.05;
        double baseFertility = rng.NextDouble() * 0.25 + 0.10;

        // Create person
        Person p = new Person(
            currentYear,
            gender,
            firstName,
            surname,
            null,
            null,
            location,
            health,
            infantHealth,
            adolescentFactor,
            fertilityStart,
            fertilityEnd,
            childExhaustion,
            exhaustionFactor,
            baseFertility
        );

        // Initialize runtime fields (IMPORTANT)
        p.fertilityDesire = rng.NextDouble() * 0.2;
        p.numChildren = 0;
        p.married = false;
        p.wantsToMarry = true;
        p.isAlive = true;
        p.deathYear = -1;
        p.marriageYear = -1;

        return p;
    }

    // --- helpers ---

    private double Clamp01(double v)
    {
        return Math.Max(0.0, Math.Min(1.0, v));
    }

    // Box–Muller Gaussian
    private double Normal(double mean, double stdDev)
    {
        double u1 = 1.0 - rng.NextDouble();
        double u2 = 1.0 - rng.NextDouble();
        double randStdNormal =
            Math.Sqrt(-2.0 * Math.Log(u1)) *
            Math.Sin(2.0 * Math.PI * u2);

        return mean + stdDev * randStdNormal;
    }

    public Person.Gender RandGender()
    {
        return rng.Next(2) == 0 ? Person.Gender.Male : Person.Gender.Female;
    }

    public void AttributeVariation(Person mother, Person father, Person child)
    {
        if(mother == null)
            print("Warning: Mother is null in AttributeVariation");
        if(father == null)
            print("Warning: Father is null in AttributeVariation");
        child.health = Math.Min((mother.health + father.health) / 2 + (rng.NextDouble() * 10 - 5), 150);
        child.infantHealth = (mother.infantHealth + father.infantHealth) / 2 + (rng.NextDouble() * 10 - 5);
        child.adolescentFactor = Math.Max((mother.adolescentFactor + father.adolescentFactor) / 2 + (rng.NextDouble() * 0.02 - 0.01), 1);
        child.fertilityStartAge = Math.Max((mother.fertilityStartAge + father.fertilityStartAge) / 2 + rng.Next(-2, 3), 9);
        child.fertilityEndAge = Math.Min((mother.fertilityEndAge + father.fertilityEndAge) / 2 + rng.Next(-2, 3), 51);
        child.childExhaustion = (mother.childExhaustion + father.childExhaustion) / 2 + rng.Next(-1, 2);
        child.exhaustionFactor = (mother.exhaustionFactor + father.exhaustionFactor) / 2 + (rng.NextDouble() * 0.02 - 0.01);
        child.baseFertility = (mother.baseFertility + father.baseFertility) / 2 + (rng.NextDouble() * 0.02 - 0.01);
    }

    public (int, int) GetGridPosition(Vector2Int location)
    {
        return (Mathf.FloorToInt(location.x / gridCellSize), Mathf.FloorToInt(location.y / gridCellSize));
    }

    public void AddToGrid(Person person)
    {
        var gridPos = GetGridPosition(person.location);
        if(!grid.ContainsKey(gridPos))
        {
            grid[gridPos] = new HashSet<Person>();
        }
        grid[gridPos].Add(person);
    }
    public Person GetRandomPersonToMarry(Person person, float radius)
    {
        Vector2Int center = person.location;
        Person chosen = null;
        int count = 0;
        float radiusSqr = radius * radius;

        int minX = Mathf.FloorToInt((center.x - radius) / gridCellSize);
        int maxX = Mathf.FloorToInt((center.x + radius) / gridCellSize);
        int minY = Mathf.FloorToInt((center.y - radius) / gridCellSize);
        int maxY = Mathf.FloorToInt((center.y + radius) / gridCellSize);

        for(int x = minX; x <= maxX; x++)
        {
            for(int y = minY; y <= maxY; y++)
            {
                var gridPos = (x, y);
                if(grid.ContainsKey(gridPos))
                {
                    foreach(var match in grid[gridPos])
                    {
                        if(match.goingToMarry)
                        {
                            Vector2Int diff = match.location - center;
                            float distSqr = diff.x * diff.x + diff.y * diff.y;
                            if(distSqr <= radiusSqr)
                            {
                                count++;
                                if(UnityEngine.Random.Range(0, count) == 0 && match.gender != person.gender)
                                {
                                    chosen = match;
                                }
                            }
                        }
                    }
                }
            }
        }

        return chosen;
    }

    
}