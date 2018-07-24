using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public WorldQueue universe;
    public LevelTimer timer;
    public GameObject[] menuObjects;

	void Start()
	{
        timer.gameObject.SetActive(false);	
	}

	public void StartGame()
    {
        foreach(var o in menuObjects)
        {
            o.SetActive(false);
        }

        timer.gameObject.SetActive(true);  
        universe.StartWorlds();
    }
}
