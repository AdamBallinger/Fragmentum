using Assets.Scripts.Objects;
using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.Character
{
    [RequireComponent(typeof(CharacterController))]
    public class Player : MonoBehaviour
    {
        public CharacterController Controller { get; private set; }

        public Animator Animator { get; private set; }

        [HideInInspector]
        public bool isDead;
        [HideInInspector]
        public bool alwaysGrounded = false;
        [HideInInspector]
        public int score = 0;
        [HideInInspector]
        public int currentHealth;
        [HideInInspector]
        public bool knockedBack = false;
        [HideInInspector]
        public float currentStamina;
        [HideInInspector]
        public Vector3 velocity;
        [HideInInspector]
        public bool isDashing;
        [HideInInspector]
        public bool isBlocking;

        public bool controlsEnabled = true;
        public bool invertMovement = false;

        [Tooltip("The reference to the gameobject containing the collider used for blocking.")]
        public GameObject blockingCollider;
        
        [Header("Player Stats")]
        public int maxHealth = 100;
        public int recoveryTime = 1500;

        public int maxStamina = 20;
        public float staminaTickTime = 1000;
        public float staminaRegenRate = 8.0f;
        public float dashStaminaReduction = 8;
        public float enemyKillStaminaIncrease = 6;

        [Header("Character Controller Physics")]
        [Range(-10.0f, 10.0f)]
        public float gravity = -1.0f;
        [Range(0.0f, 100.0f)]
        public float moveSpeed = 8.0f;
        [Range(1.0f, 100.0f)]
        public float jumpSpeed = 28.0f;
        [Range(0.0f, 100.0f)]
        public float dashSpeed = 18.0f;
        [Range(1.0f, 1000.0f)]
        public float dashLength = 300.0f;

        [Header("Player Audio Settings")]
        public AudioClip jumpSound;
        public AudioClip doubleJump;
        public AudioClip dashSound;
        public AudioClip playerHitSound;

        private Vector3 dashMotion;

        // Timers
        private float dashTimer;
        private float recoveryTimer;
        private float staminaTickCounter;
        
        // State flags
        private bool isDoubleJumping;
        private bool recoveringFromDamage;

        // Button states
        private bool jumpPressed;
        private bool dashPressed;

        // Cached object transform.
        private Transform _transform;

        private void Start()
        {
            _transform = transform;

            currentHealth = maxHealth;
            currentStamina = maxStamina;

            isDoubleJumping = false;
            isDashing = false;

            dashMotion = Vector3.zero;
            velocity = Vector3.zero;

            Controller = GetComponent<CharacterController>();
            Animator = GetComponent<Animator>();
        }

        private void FixedUpdate()
        {

#if UNITY_EDITOR
            // Debug instant death
            if (Input.GetKeyDown(KeyCode.K))
            {
                //Debug.Log(currentHealth + " / " + maxHealth);
                RemoveHealth(100);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                Respawn();
            }

            if(Input.GetKeyDown(KeyCode.H))
            {
                RemoveHealth(10);
            }
#endif
            var horizontalInput = 0.0f;

            if (controlsEnabled && !isDead)
            {
                horizontalInput = Input.GetAxis("Horizontal");
                Animator.SetFloat("animSpeedMod", Mathf.Abs(horizontalInput));
                Animator.SetBool("isRunning", (velocity.x != 0.0f || velocity.z != 0.0f) && !isDashing);
                Animator.SetBool("isGrounded", Grounded());
                Animator.SetBool("isDashing", isDashing);

                if (horizontalInput != 0.0f)
                {
                    var rawHorizontal = invertMovement ? Input.GetAxis("Horizontal") * -1 : Input.GetAxis("Horizontal");
                    if (rawHorizontal > 0.0f) rawHorizontal = 1.0f;
                    if (rawHorizontal < 0.0f) rawHorizontal = -1.0f;
                    _transform.rotation = Quaternion.Euler(0.0f, 90.0f * rawHorizontal, 0.0f);
                }

                isBlocking = Input.GetAxisRaw("Block") == 1.0f;
                Animator.SetBool("isBlocking", isBlocking);

                blockingCollider.SetActive(isBlocking);

                // Update "IFrames'
                UpdateDamageRecovery();
                // Update stamina
                UpdateStamina();

                // Handle jump and dash logic
                Jump();
                Dash();
            }

            // Gravity
            ApplyGravity();

            // Now all logic is done update the player position
            UpdatePosition(horizontalInput);
        }

        public void Die()
        {
            isDead = true;
            currentHealth = 0;
            velocity.x = 0.0f;
            velocity.z = 0.0f;
            Animator.SetBool("isDead", true);
        }

        public void Respawn()
        {
            isDead = false;
            currentHealth = maxHealth;
            controlsEnabled = true;
            Animator.SetBool("isDead", false);
        }

        public bool Grounded()
        {
            if (alwaysGrounded) return true;

            return !isDead && Controller.isGrounded;
        }

        public void BounceOffEnemyHead()
        {
            Animator.Play("Jump", 0, 0.0f);
            velocity.y = jumpSpeed / 1.35f * Time.deltaTime;
        }

        private void UpdateDamageRecovery()
        {
            if (recoveringFromDamage)
            {
                recoveryTimer += Time.deltaTime * 1000;
                if (recoveryTimer > recoveryTime)
                {
                    recoveringFromDamage = false;
                    recoveryTimer = 0.0f;
                }
            }
        }

        private void UpdateStamina()
        {
            if (currentStamina < maxStamina)
            {
                staminaTickCounter += Time.deltaTime * 1000;
                if (staminaTickCounter > staminaTickTime)
                {
                    AddStamina(staminaRegenRate);
                    staminaTickCounter = 0;
                }
            }
        }

        private void UpdatePosition(float _horizontalInput)
        {
            if (isDashing)
            {
                velocity = dashMotion * Time.deltaTime;
                dashTimer += Time.deltaTime * 1000;
                if (dashTimer > dashLength)
                {
                    isDashing = false;
                    dashTimer = 0.0f;
                }
            }
            else
            {
                var preservedYVel = velocity.y;

                if (controlsEnabled && _horizontalInput != 0.0f)
                {
                    var velocityChange = (isBlocking ? moveSpeed / 2.0f : moveSpeed * Mathf.Abs(_horizontalInput)) * Time.deltaTime;
                    velocity = _transform.forward * velocityChange;
                }
                else
                {
                    velocity = Vector3.zero;
                }

                velocity.y = preservedYVel;
            }

            Controller.Move(velocity);
        }

        private void ApplyGravity()
        {
            if (!isDashing && !Controller.isGrounded)
            {
                velocity.y += gravity * Time.deltaTime;
            }

            if (Controller.isGrounded)
            {
                isDoubleJumping = false;
            }
        }

        private void Dash()
        {
            if (Input.GetAxis("Dash") != 0.0f && !isDashing && !dashPressed)
            {
                dashPressed = true;

                if (currentStamina >= dashStaminaReduction)
                {
                    isDashing = true;
                    dashMotion = _transform.forward * dashSpeed;
                    currentStamina -= dashStaminaReduction;
                    GetComponent<AudioSource>().clip = dashSound;
                    GetComponent<AudioSource>().Play();
                }
            }
            if (Input.GetAxis("Dash") == 0.0f)
            {
                dashPressed = false;
            }
        }

        private void Jump()
        {
            if (Input.GetAxis("Jump") != 0 && Controller.isGrounded)
            {
                velocity.y = jumpSpeed * Time.deltaTime;
                jumpPressed = true;
                Animator.SetBool("isGrounded", false);
                Animator.Play("Jump", 0, 0.0f);
                GetComponent<AudioSource>().clip = jumpSound;
                GetComponent<AudioSource>().Play();
            }
            else if (Input.GetAxis("Jump") != 0 && !Controller.isGrounded)
            {
                if (!isDoubleJumping && !jumpPressed)
                {
                    isDoubleJumping = true;
                    velocity.y = jumpSpeed * Time.deltaTime;
                    Animator.Play("Jump", 0, 0.0f);
                    Animator.SetBool("isGrounded", false);
                    GetComponent<AudioSource>().clip = doubleJump;
                    GetComponent<AudioSource>().Play();
                }
            }
            if (Input.GetAxis("Jump") == 0)
            {
                jumpPressed = false;
            }
        }

        // TODO: Headbutting the ceiling is not stopping the player properly
        private void OnCollisionEnter(Collision _collision)
        {
            if ((Controller.collisionFlags & CollisionFlags.Above) != 0)
            {
                velocity.y = 0;
            }
        }

        private void OnControllerColliderHit(ControllerColliderHit _hit)
        {
            var pushableObj = _hit.gameObject.GetComponentInChildren<PushableObject>();

            if (pushableObj == null)
            {
                return;
            }

            var rb = _hit.collider.attachedRigidbody;

            if (rb == null || rb.isKinematic)
            {
                return;
            }

            if (_hit.moveDirection.y < -0.3f)
            {
                return;
            }

            var pushDir = new Vector3(_hit.moveDirection.x, 0.0f, _hit.moveDirection.z);
            rb.velocity += pushDir * pushableObj.pushPower;
        }

        public void Update()
        {
            // ONLY PAUSE IN UPDATE FUNCTION. Everything else in FixedUpdate above.
            // Update will still be called when Time.timeScale is 0 so the game can be unpaused using controller.
            Pause();
        }


        //Pause
        private void Pause()
        {
            if (Input.GetButtonDown("Pause"))
            {
                if (Time.timeScale == 0.0f && FindObjectOfType<GlobalUIController>().pauseUI.activeSelf)
                {
                    Time.timeScale = 1.0f;
                    FindObjectOfType<GlobalUIController>().pauseUI.SetActive(false);

                }
                else if (Time.timeScale == 1.0f)
                {
                    Time.timeScale = 0.0f;
                    FindObjectOfType<GlobalUIController>().pauseUI.SetActive(true);
                }
            }
        }

        // Stamina
        public void AddStamina(float _stamina)
        {
            if (currentStamina < maxStamina)
                currentStamina += _stamina;

            if (currentStamina > maxStamina)
                currentStamina = maxStamina;
        }

        public void RemoveStamina(float _stamina)
        {
            if (currentStamina > 0)
                currentStamina -= _stamina;

            if (currentStamina < 0)
                currentStamina = 0;
        }

        // Health
        public void AddHealth(int _health)
        {
            if (currentHealth < maxHealth)
                currentHealth += _health;

            if (currentHealth > maxHealth)
                currentHealth = maxHealth;
        }

        public void RemoveHealth(int _health)
        {
            if (isDead || _health <= 0) return;

            if(!recoveringFromDamage)
            {
                currentHealth -= _health;

                if(currentHealth > 0)
                {
                    recoveringFromDamage = true;
                    Animator.SetBool("takingDamage", true);

                    GetComponent<AudioSource>().clip = playerHitSound;
                    GetComponent<AudioSource>().Play();
                }
            }

            if(currentHealth <= 0)
            {
                Die();
            }
        }

        private void OnDamageAnimFinish()
        {
            Animator.SetBool("takingDamage", false);
        }
    }
}
