using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Idea Database")]
public class IdeaDatabase : ScriptableObject
{
    public List<IdeaData> ideas = new List<IdeaData>(); 
    private static Dictionary<string, IdeaData> ideaLookup = null;

    public void Initialize()
    {
        ideaLookup = new Dictionary<string, IdeaData>();

        foreach (IdeaData idea in ideas)
        {
            if (!ideaLookup.ContainsKey(idea.ideaId))
            {
                ideaLookup.Add(idea.ideaId, idea);
            }
            else
            {
                Debug.LogWarning($"Duplicate ideaId: {idea}");
            }
        }
    }
    public IdeaData GetIdea(string ideaId)
    {
        if (ideaLookup == null)
            Initialize();
        if (ideaLookup.ContainsKey(ideaId))
            return ideaLookup[ideaId];
        else
        {
            Debug.LogError("idea not found:" + ideaId + ". Creating idea");
            IdeaData idea = new IdeaData();
            idea.ideaId = ideaId;
            idea.ideaName = ideaId;
            return idea;
        }
    }
}
