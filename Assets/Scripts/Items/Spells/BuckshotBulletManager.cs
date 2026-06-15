using CartoonFX;
using UnityEngine;

public class BuckshotBulletManager : BulletManager
{

    [Header("Buckshot Attributes")]
    public int pelletCount = 5;
    public float spreadInDegrees = 15f;
    public float maxAudioPlayers = 3;
    public float maxScreenShakes = 3;

    [Header("Pellet Prefab")]
    GameObject pelletPrefab;

    public override void InitializeBullet(CharacterManager characterCausingDamage, ElementalDamageType currentHighestElementalDamageType, float projectileLifetimeInSeconds, Transform spawnTransform, float projectileUpwardVelocityMultiplier, float projectileForwardVelocityMultiplier, bool hasGravity = true)
    {
        base.InitializeBullet(characterCausingDamage, currentHighestElementalDamageType, projectileLifetimeInSeconds, spawnTransform, projectileUpwardVelocityMultiplier, projectileForwardVelocityMultiplier, hasGravity);

        if (pelletCount > 0)
        {
            pelletPrefab = characterCausingDamage.characterWeaponManager.GetEquippedWeapon(true).GetComponent<WeaponScript>().gunProjectile;
            //Instantiate 1 pellet instance per pellet, minus 1 since the initial pellet spawns the others
            InstantiatePellets(pelletCount, projectileLifetimeInSeconds, this.spawnTransform, characterCausingDamage, projectileUpwardVelocityMultiplier, projectileForwardVelocityMultiplier);
        }

    }

    public void InstantiatePellets(int currentPelletCount, float projectileLifetimeInSeconds, Transform spawnTransform, CharacterManager characterCausingDamage, float projectileUpwardVelocityMultiplier, float projectileForwardVelocityMultiplier)
    {
        float masterSpeed = fireBallRigidBody.velocity.magnitude;
        if (masterSpeed < 0.1f) masterSpeed = 20f; // Default if not moving yet

        //Temp variable that stores the currently randomized spread of the new pellet
        Vector3 staggeredSpawn;
        Quaternion spreadRotation;


        //currentPelletCount - 1 is used because the initial BuckshotBulletManager counts for the total
        for (int i = 0; i < currentPelletCount - 1; i++)
        {

            //Nudge each pellet slightly forward so they aren't exactly on top of each other
            staggeredSpawn = spawnTransform.position + (transform.forward * (i * 0.1f));
            spreadRotation = spawnTransform.rotation *
                 Quaternion.Euler(0, -135, 0) *
                 Quaternion.Euler(Random.Range(-spreadInDegrees, spreadInDegrees),
                                  Random.Range(-spreadInDegrees, spreadInDegrees), 0);


            GameObject newBuckshotPellet = Instantiate(pelletPrefab, staggeredSpawn, spreadRotation);
            BuckshotBulletManager newBuckshotBulletManager = newBuckshotPellet.GetComponent<BuckshotBulletManager>();

            //If projectiles start colliding and acting weird, uncomment this
            // //1. Get all colliders on the new pellet
            // Collider[] pelletColliders = newBuckshotPellet.GetComponentsInChildren<Collider>();
            // //2. Get all colliders on the character who shot it
            // Collider[] shooterColliders = damageCollider.characterCausingDamage.GetComponentsInChildren<Collider>();

            // Collider masterBulletCollider = GetComponent<Collider>();
            // Collider masterBulletDamageCollider = newBuckshotBulletManager.damageCollider.damageCollider;
            // //3. Tell Unity to ignore collisions between them
            // foreach (var pelletCollider in pelletColliders)
            // {
            //     //4. Tell Pellets to ignore collisions with master bullet
            //     if (masterBulletCollider != null) Physics.IgnoreCollision(pelletCollider, masterBulletCollider);
            //     if (masterBulletDamageCollider != null) Physics.IgnoreCollision(pelletCollider, masterBulletDamageCollider);

            //     foreach (var shooterCollider in shooterColliders)
            //     {
            //         Physics.IgnoreCollision(pelletCollider, shooterCollider);
            //     }
            // }


            //Initialize Pellet Location and Scale
            newBuckshotPellet.transform.parent = null;
            newBuckshotPellet.transform.localScale = Vector3.one;

            newBuckshotBulletManager.pelletCount = 0;
            newBuckshotBulletManager.InitializeBullet(damageCollider.characterCausingDamage, highestElementalDamageType, projectileLifetimeInSeconds, newBuckshotPellet.transform, projectileUpwardVelocityMultiplier, projectileForwardVelocityMultiplier, fireBallRigidBody.useGravity);

            Rigidbody bulletRigidbody = newBuckshotPellet.GetComponent<Rigidbody>();

            if (characterCausingDamage.isLockedOn)
            {
                //If locked on, point at target
                newBuckshotPellet.transform.LookAt(characterCausingDamage.characterCombatManager.LockOnTransform.transform.position);
                //Apply the spread rotation to the LookAt rotation
                newBuckshotPellet.transform.rotation *= Quaternion.Euler(Random.Range(-spreadInDegrees, spreadInDegrees), Random.Range(-spreadInDegrees, spreadInDegrees), 0);
                newBuckshotPellet.transform.rotation *= Quaternion.Euler(0, -225, 0);
            }
            else
            {
                newBuckshotPellet.transform.rotation = spreadRotation;
            }

            Vector3 upwardVelocity = newBuckshotPellet.transform.up * projectileUpwardVelocityMultiplier;
            Vector3 forwardVelocity = newBuckshotPellet.transform.forward * projectileForwardVelocityMultiplier;
            bulletRigidbody.velocity = upwardVelocity + forwardVelocity;

            //Limit number of sounds playing simultaneously for performance
            if (i > (maxAudioPlayers - 2))
            {
                AudioSource[] audioSources = newBuckshotPellet.GetComponentsInChildren<AudioSource>();
                foreach (var audioSource in audioSources)
                {
                    audioSource.enabled = false;
                }
            }

            //Limit number of Screen Shakes playing simultaneously for performance
            if (i > (maxScreenShakes - 2))
            {
                CFXR_Effect[] screenShakes = newBuckshotPellet.GetComponentsInChildren<CFXR_Effect>();
                foreach (var screenShake in screenShakes)
                {
                    screenShake.cameraShake.enabled = false;
                }
            }
        }
    }

}
