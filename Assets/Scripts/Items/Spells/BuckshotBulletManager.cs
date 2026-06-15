using UnityEngine;

public class BuckshotBulletManager : BulletManager
{

    [Header("Buckshot Attributes")]
    public int pelletCount = 5;
    public float spreadInDegrees = 15f;

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

        Debug.Log(characterCausingDamage + "|" + currentHighestElementalDamageType + "|" + projectileLifetimeInSeconds + "|" + hasGravity);
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

            // Nudge each pellet slightly forward so they aren't exactly on top of each other
            staggeredSpawn = spawnTransform.position + (transform.forward * (i * 0.1f));
            //spreadRotation = spawnTransform.rotation * Quaternion.Euler(Random.Range(-spreadInDegrees, spreadInDegrees), Random.Range(-spreadInDegrees, spreadInDegrees), 0);
            spreadRotation = spawnTransform.rotation *
                 Quaternion.Euler(0, -90, 0) *
                 Quaternion.Euler(Random.Range(-spreadInDegrees, spreadInDegrees),
                                  Random.Range(-spreadInDegrees, spreadInDegrees), 0);


            GameObject newBuckshotPellet = Instantiate(pelletPrefab, staggeredSpawn, spreadRotation);
            BuckshotBulletManager newBuckshotBulletManager = newBuckshotPellet.GetComponent<BuckshotBulletManager>();

            //1. Get all colliders on the new pellet
            Collider[] pelletColliders = newBuckshotPellet.GetComponentsInChildren<Collider>();
            //2. Get all colliders on the character who shot it
            Collider[] shooterColliders = damageCollider.characterCausingDamage.GetComponentsInChildren<Collider>();

            Collider masterBulletCollider = GetComponent<Collider>();
            Collider masterBulletDamageCollider = newBuckshotBulletManager.damageCollider.damageCollider;
            //3. Tell Unity to ignore collisions between them
            foreach (var pelletCollider in pelletColliders)
            {
                //4. Tell Pellets to ignore collisions with master bullet
                if (masterBulletCollider != null) Physics.IgnoreCollision(pelletCollider, masterBulletCollider);
                if (masterBulletDamageCollider != null) Physics.IgnoreCollision(pelletCollider, masterBulletDamageCollider);

                foreach (var shooterCollider in shooterColliders)
                {
                    Physics.IgnoreCollision(pelletCollider, shooterCollider);
                }
            }

            //Force the Rigid Body location update
            Rigidbody newRigidBody = newBuckshotPellet.GetComponent<Rigidbody>();
            if (newRigidBody != null)
            {
                newRigidBody.position = staggeredSpawn;
                newRigidBody.rotation = spreadRotation;
                newRigidBody.velocity = spreadRotation * Vector3.forward * masterSpeed;

                // Vector3 currentVelocity = newBuckshotBulletManager.fireBallRigidBody.velocity;
                newBuckshotBulletManager.fireBallRigidBody.position = staggeredSpawn;
                newBuckshotBulletManager.fireBallRigidBody.rotation = spreadRotation;
                newBuckshotBulletManager.fireBallRigidBody.velocity = spreadRotation * Vector3.forward * masterSpeed;
            }

            //Initialize Pellet Location and Scale
            newBuckshotPellet.transform.parent = null;
            newBuckshotPellet.transform.localScale = Vector3.one;

            newBuckshotBulletManager.pelletCount = 0;
            Debug.Log("Highest Element: " + highestElementalDamageType);
            newBuckshotBulletManager.InitializeBullet(damageCollider.characterCausingDamage, highestElementalDamageType, projectileLifetimeInSeconds, newBuckshotPellet.transform, projectileUpwardVelocityMultiplier, projectileForwardVelocityMultiplier, fireBallRigidBody.useGravity);

            if (characterCausingDamage.isLockedOn)
            {
                newBuckshotPellet.transform.LookAt(characterCausingDamage.characterCombatManager.currentTarget.transform.position);
            }
            else
            {
                //instantiatedSpellProjectileFX.transform.forward = this.characterCausingThisDamagetransform.forward;
                Vector3 newForward = characterCausingDamage.transform.forward + new UnityEngine.Vector3(0, 0, 0);
                newBuckshotPellet.transform.forward = newForward;
            }

            //6. Set the projectile's velocity
            Rigidbody bulletRigidbody = newBuckshotPellet.GetComponent<Rigidbody>();
            Vector3 upwardVelocity = newBuckshotPellet.transform.up * projectileUpwardVelocityMultiplier;
            Vector3 forwardVelocity = newBuckshotPellet.transform.forward * projectileForwardVelocityMultiplier;
            Vector3 totalVelocity = upwardVelocity + forwardVelocity;
            bulletRigidbody.velocity = totalVelocity;

        }
    }

}
