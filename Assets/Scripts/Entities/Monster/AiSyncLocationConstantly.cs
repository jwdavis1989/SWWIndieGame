using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiSyncLocationConstantly : MonoBehaviour
{

    private GameObject targetObject;
    private Transform targetObjectTransform;
    public bool hasBeenInitialized = false;


    // Update is called once per frame
    void LateUpdate()
    {
        if (targetObjectTransform && targetObject.gameObject.activeInHierarchy && hasBeenInitialized)
        {
            transform.position = targetObjectTransform.transform.position;
            transform.rotation = targetObjectTransform.transform.rotation;
        }
    }

    public void InitializeAiSync(GameObject initializerGameObject)
    {
        targetObject = initializerGameObject;
        targetObjectTransform = targetObject.transform;
        hasBeenInitialized = true;
    }

    public void EnableIsFalling()
    {
        //Compatibility Stub
    }

    public void DisableIsFalling()
    {
        //Compatibility Stub
    }

    public virtual void DisableBoosting()
    {
        //Does nothing, this is to prevent an error from using the humanoid animation events.
    }

    public void PlayLandingSFX()
    {
        //Does nothing, this is to prevent an error from using the humanoid animation events.
    }

    public void CallDrainStaminaBasedOnAttack()
    {
        //Does nothing, this is to prevent an error from using the humanoid animation events.
    }

    public void EnableCanRotate()
    {
        //Does nothing, this is to prevent an error from using the humanoid animation events.
    }

    public void DisableCanRotate()
    {
        //Does nothing, this is to prevent an error from using the humanoid animation events.
    }

    public void CallOpenDamageCollider()
    {
        //Does nothing, this is to prevent an error from using the humanoid animation events.
    }

    public void CallCloseDamageCollider()
    {
        //Does nothing, this is to prevent an error from using the humanoid animation events.
    }

    public void EnableCanDoCombo()
    {
        //Does nothing, this is to prevent an error from using the humanoid animation events.
    }

}
