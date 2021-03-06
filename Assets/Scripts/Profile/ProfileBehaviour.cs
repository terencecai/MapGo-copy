﻿using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UniRx;

public class ProfileBehaviour : MonoBehaviour
{

    [SerializeField] public Text ProfileName;
    [SerializeField] public Text CharType;
    [SerializeField] public Text Level;
    [SerializeField] public Text Exp;
    [SerializeField] public Button BackButton;
    [SerializeField] public Button EditButton;
    [SerializeField] public Button ProfileAvatar;
    [SerializeField] public Image AutorityBar;
    [SerializeField] public Image CompassionBar;
    [SerializeField] public Image IntelligenceBar;


    void Start()
    {
        var i = 0;
        ProfileAvatar.onClick.AddListener(() =>
        {
            if (i < 4)
            {
                i++;
            }
            else
            {
                PlayerPrefs.DeleteAll();
                SceneManager.LoadSceneAsync("TestScene");
            }
        });
        EditButton.onClick.AddListener(() => SceneManager.LoadScene("ProfileEdit", LoadSceneMode.Additive));
        BackButton.onClick.AddListener(() => gameObject.SetActive(false));        
    }

    void OnEnable()
    {
        loadProfile();
    }

    private void loadProfile()
    {
        var token = PlayerPrefs.GetString("token", "");
        Observable.WhenAll(RestClient.getMySkills(token), RestClient.getAllSkills(token), RestClient.getProfile(token))
            .Subscribe(
                x =>
                {
                    var p = JsonUtility.FromJson<Profile>(x[2]);
                    p.skills = ApplicationLoadController.convertSkills(x[0]);
                    p.allSkills = ApplicationLoadController.convertSkills(x[1]);
                    ProfileRepository.Instance.SaveProfileJson(p);
                    fillData(p);

                },
                e => Debug.Log(e)
            );
    }

    private void fillData(Profile profile)
    {
        ProfileName.text = profile.nickName;
        CharType.text = profile.chosenValue.name;

        AutorityBar.fillAmount = profile.values[0].level / 100.0f;
        CompassionBar.fillAmount = profile.values[1].level / 100.0f;
        IntelligenceBar.fillAmount = profile.values[2].level / 100.0f;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
