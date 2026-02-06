using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdeaDatabase : ScriptableObject
{
    List<string> ideas = new List<string>(); 
    private Dictionary<string, bool> ideaLookup;

    public void Initialize()
    {
        ideaLookup = new Dictionary<string, bool>();

        foreach (var idea in ideas)
        {
            if (!ideaLookup.ContainsKey(idea))
            {
                ideaLookup.Add(idea, false);
            }
            else
            {
                Debug.LogWarning($"Duplicate ideaId: {idea}");
            }
        }
    }

    public bool CheckHasIdea(string ideaId)
    {
        if (ideaLookup == null)
            Initialize();

        ideaLookup.TryGetValue(ideaId, out var idea);
        return idea;
    }
    public void SetHasObtained(string ideaID)
    {
        if (ideaLookup.ContainsKey(ideaID))
        {
            ideaLookup[ideaID] = true;
        }
        else
        {
            ideas.Add(ideaID);
            ideaLookup.Add(ideaID, true);
        }
    }
}
