using Assets.Scripts.AI.Bosses.Bat;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Character
{
    public class PlayerCollisions : MonoBehaviour
    {
        private Player player;

        private const int arbitraryDamageValue = 5;
        private const float arbitraryKnockbackForce = 0.2f;

        public AudioClip damageSound;
        public Vector3 soundPos;
        public GameObject standardizedCam;

        private void Start()
        {
            player = GetComponent<Player>();
            standardizedCam = GameObject.FindGameObjectWithTag("StandardizedCamera");
        }

        private void Update()
        {
            soundPos = standardizedCam.transform.position;
        }

        private void OnTriggerEnter(Collider _collider)
        {
            var colliderObject = _collider.gameObject;
            var collidingTag = colliderObject.tag;

            if (collidingTag.Equals("Untagged") || collidingTag.Contains("Player")) return;

            //Debug.Log("Player Collisions: Colliding with Tag: " + collidingTag + "  Self is: " + gameObject.tag);

            if (collidingTag.Equals("Bat Minion") && player.isDashing)
            {
                var batMinionController = colliderObject.GetComponent<BatMinionController>();

                if (!batMinionController.invulnerable)
                {
                    batMinionController.targetPit.hasMinionAttention = false;
                    PlayerReactToEnemy(colliderObject, false);
                }
            }
            else if (collidingTag.Equals("Bat Bomb Projectile"))
            {
                // TODO: Should we check what part of the player is hit?
                if (!player.isBlocking)
                {
                    RemovePlayerHealth();
                }
            }
            else if (collidingTag.Equals("RockEnemy") || collidingTag.Equals("MushroomEnemy"))
            {
                PlayerReactToEnemy(colliderObject, false);
            }
            //else if (collidingTag.Equals("EnemyHeadTrigger"))
            //{
            //    CrushEnemy(colliderObject);
            //}
            else if (collidingTag.Equals("EnemyDashCollider") && player.isDashing)
            {
                // The coroutine behaviour with pushing away is far too clunky. Probably best to just use a particle system instead.
                //StartCoroutine(DashKillEnemyCoroutine(colliderObject));
                colliderObject.SetActive(false);
            }
            else if(collidingTag.Equals("RootsTrigger") && player.isDashing)
            {
                Destroy(colliderObject);
            }
        }

        //private IEnumerator DashKillEnemyCoroutine(GameObject colliderObject)
        //{
        //    PushEnemyAwayWithForce(colliderObject);

        //    yield return new WaitForSeconds(2);

        //    colliderObject.SetActive(false);
        //}

        private void CrushEnemy(GameObject colliderObject)
        {
            player.BounceOffEnemyHead();
            SetGameObjectInactive(colliderObject, false);
        }

        private void PushEnemyAwayWithForce(GameObject colliderObject)
        {
            var enemyPosition = colliderObject.transform.position;
            var playerPosition = colliderObject.transform.position;

            var direction = (enemyPosition - playerPosition).normalized;

            colliderObject.GetComponent<Rigidbody>().velocity = direction * arbitraryKnockbackForce;
        }

        private void PlayerReactToEnemy(GameObject colliderObject, bool isChildTrigger)
        {
            if (player.isDashing)
            {
                player.AddStamina(player.enemyKillStaminaIncrease);
                SetGameObjectInactive(colliderObject, isChildTrigger);
            }
            else
            {
                if (player.velocity.y < 0 && transform.position.y > colliderObject.transform.position.y)
                {
                    CrushEnemy(colliderObject);
                }
                else
                {
                    RemovePlayerHealth();
                }

                //RemovePlayerHealth();
                //StartCoroutine(ApplyPlayerKnockbackCoroutine(colliderObject));
            }
        }

        private void SetGameObjectInactive(GameObject obj, bool isChildTrigger)
        {
            var objectToSetInactive = isChildTrigger ? obj.transform.parent.gameObject : obj;
            objectToSetInactive.SetActive(false);
        }

        private IEnumerator ApplyPlayerKnockbackCoroutine(GameObject colliderObject)
        {
            var damagingEntityPosition = colliderObject.transform.position;
            var playerPosition = player.transform.position;

            var direction = (playerPosition - damagingEntityPosition).normalized;
            direction.z = 0;

            player.velocity = (direction * arbitraryKnockbackForce);

            player.controlsEnabled = false;

            yield return new WaitForSeconds(0.4f);

            player.controlsEnabled = true;
        }

        private void RemovePlayerHealth()
        {
            player.RemoveHealth(arbitraryDamageValue);
            AudioSource.PlayClipAtPoint(damageSound, soundPos, 0.70f);

        }

    }
}
