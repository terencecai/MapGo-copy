using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillCache
{
    private static readonly string CACHE_KEY = "skills_cache";

    private static SkillCache _instance;
    public static SkillCache Instance 
    {
        get
        {
            if (_instance == null)
                _instance = new SkillCache();
            return _instance;
        }
    }

    public void Save(string json)
    {
        if (json.StartsWith("[")) {
            json = SkillCacheContainer.skillCacheString(json);
        } 

        PlayerPrefs.SetString(CACHE_KEY, json);
        PlayerPrefs.Save();
    }

    public void Save(SkillCacheContainer container)
    {
        PlayerPrefs.SetString(CACHE_KEY, JsonUtility.ToJson(container));
        PlayerPrefs.Save();
    }

    public void RemoveItem(Skill skill)
    {
        var cache = GetCache();
        if (cache.skills.Count <= 0)
            return;
        cache.skills.Remove(skill);
        Save(cache);
    }

    public SkillCacheContainer GetCache()
    {
        return JsonUtility.FromJson<SkillCacheContainer>(PlayerPrefs.GetString(CACHE_KEY, ""));
    }
}

[Serializable]
public class SkillCacheContainer
{
    public List<Skill> skills;

    public static SkillCacheContainer fromJsonList(string json)
    {
        return JsonUtility.FromJson<SkillCacheContainer>(skillCacheString(json));
    }

    public static string skillCacheString(string json)
    {
        return "{ \"skills\": " + json + "}";
    }
}