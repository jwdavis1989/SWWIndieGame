using UnityEngine;
[CreateAssetMenu(menuName = "Character Effects/Instant Effects/Apply Slow Effect")]
public class ApplySlowEffect : InstantCharacterEffect
{
    public float slowMultiplier;
    public float duration;
}

public class TimedCharacterEffect
{
    public float remaingDuration;
    public bool started = false;
    public bool finished = false;
    public virtual void ApplyEffect(CharacterManager character)
    {
        if(!started) 
            started = true;
        remaingDuration -= Time.deltaTime;
        if(remaingDuration <= 0 )
            finished = true;
    }
}
public class ActiveSlowCharacterEffect : TimedCharacterEffect
{
    public float slowMultiplier; 
    //note: could have starting and final slow multiplier
    public override void ApplyEffect(CharacterManager character)
    {
        base.ApplyEffect(character);
        //Note: could make slow increase or decrease over time here
    }
}