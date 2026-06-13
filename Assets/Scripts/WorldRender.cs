using UnityEngine;
using System.Collections.Generic;
using VidTools.Vis;

public class WorldRender : MonoBehaviour
{

    public static WorldRender Instance{ get; private set; }

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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RenderWorld()
    {
        RenderPeople();
    }
    public void RenderPeople()
    {
        foreach(Person person in WorldManager.Instance.people)
        {
            if(person.isAlive)
            {
                Draw.Point(WorldToGame(person.location), 100f, Color.white);
                // Render the person at their location
                // This is a placeholder for actual rendering code
                Debug.Log($"Rendering {person.firstName} {person.surname} at location {person.location}");
            }
        }
    }

    public Vector3 WorldToGame(Vector2Int worldPosition)
    {
        // Convert world coordinates to screen coordinates
        // This is a placeholder for actual conversion logic
        return new Vector3(worldPosition.x/10.0f, worldPosition.y/10.0f, 0);
    }
}
