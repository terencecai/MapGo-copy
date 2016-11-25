﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CoPlayerManager : MonoBehaviour {

    private ArrayList playersOnScreen = new ArrayList();
    [SerializeField]
    private CoPlayer coPlayerPrefab;
    private SocketManager sockManager;
    private float nextActionTime = 0.0f;
    public float period = 1f;

    void Start () {
        sockManager = GetComponent<SocketManager>();        
	}  

    void Update () {
        if(Time.time > nextActionTime) {
            nextActionTime += period;
            UpdatePlayers(sockManager.GetUsers());
        }
	}

    public void UpdatePlayers(Users users) {
       

        if(playersOnScreen.Count == 0) {
            foreach(User user in users.nearest_players) {
                AddNewCoPlayer(user.GetEmail(), user.GetLocation());
            }
            
        } else {
            Dictionary<string, Location> newUsers = new Dictionary<string, Location>();
            foreach(User user in users.nearest_players) {
                newUsers.Add(user.GetEmail(), user.GetLocation());
            }
            foreach(CoPlayer coPlayer in playersOnScreen) {
                Debug.Log("debug1");
                string email = coPlayer.GetEmail();
                if(newUsers.ContainsKey(email)) {
                    coPlayer.SetPosition(newUsers[email]);
                    newUsers.Remove(email);
                } else {
                    playersOnScreen.Remove(coPlayer);
                    Destroy(coPlayer.gameObject);
                }
            }
            foreach(KeyValuePair<string, Location> user in newUsers) {
                Debug.Log("debug2");
                AddNewCoPlayer(user.Key, user.Value);
            }
        }
    }

    private void AddNewCoPlayer(string email, Location location) {
        CoPlayer coPlayer = Instantiate(coPlayerPrefab, transform.position, transform.rotation) as CoPlayer;
        coPlayer.SetEmail(email);
        coPlayer.SetPosition(location);
        playersOnScreen.Add(coPlayer);
    }
}
