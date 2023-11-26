using Cinemachine;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class PlayerController : MonoBehaviour
    {

        [SerializeField] public InputManager inputManager;

        [SerializeField] private CharacterController _controller;

        [SerializeField] private Animator _animator;
        //[Range(0, 1f)]
        //public float StartAnimTime = 0.3f;
        //[Range(0, 1f)]
        //public float StopAnimTime = 0.15f;
        [SerializeField] public GameObject FirstPersonFollowTarget;
        [SerializeField] public GameObject ThirdPersonFollowTarget;
        [SerializeField] public CinemachineVirtualCamera FirstPersonCamera;
        [SerializeField] public CinemachineVirtualCamera ThirdPersonCamera;

        //private AudioSource Audio;
        [SerializeField] public float WalkSpeed;
        [SerializeField] public float RunSpeed;
        [SerializeField] public float SneakSpeed;
        [SerializeField] public float RotateSpeed;
        [SerializeField] public float JumpSpeed;
        [SerializeField] public float JumpPenalty;


        //public LayerMask groundLayer;
        //[SerializeField] private Transform feetTransform;
        //[SerializeField] private float goundCheckRadius = 0.01f;

        private float _moveSpeed;
        private float _verticalSpeed;
        private bool _isFirstPerson;
        private bool _isGrounded;
        private bool _isWalking;
        private bool _isRunning;
        private bool _isSneaking;
        private float _verticalRotation;
        private Vector3 _velocity;
        private Vector3 _slopeDirection;


        private void Awake()
        {
            //Audio = GetComponent<AudioSource>();
            _controller = GetComponent<CharacterController>();
            _animator = GetComponent<Animator>();
            _verticalSpeed = -10.0f;
            _moveSpeed = WalkSpeed;
        }

        private void Start()
        {
            inputManager.OnJumpPerformed += PlayerInput_OnJumpPerformed;
            inputManager.OnInteractPerformed += PlayerInput_OnInteractPerformed;
            inputManager.OnSwitchCamera += PlayerInput_OnSwitchCamera;
            inputManager.OnRunPerformed += PlayerInput_OnRunPerformed;
            GameManager.ShowMouse(false);
            HandleMovement();
            HandleGravity();
            _controller.Move(_velocity * Time.deltaTime);
        }

        private void PlayerInput_OnInteractPerformed(object sender, System.EventArgs e)
        {
            Vector3 origin = transform.position;
            origin.y += 0.5f;
            if (Physics.Raycast(origin, transform.forward, out RaycastHit hit, 1f))

            {
                if (hit.collider.gameObject.tag == "Door")
                {
                    //hit.collider.gameObject.GetComponent<Door>().SetFlag(true);
                }
            }
        }

        private void PlayerInput_OnJumpPerformed(object sender, System.EventArgs e)
        {

            if (_isGrounded)
            {
                if (!_isWalking)
                {
                    _animator.SetTrigger("jump");
                }
                _verticalSpeed = JumpSpeed;
                if (_slopeDirection != Vector3.up && (Vector3.Angle(_slopeDirection, Vector3.up) > _controller.slopeLimit))

                {
                    _verticalSpeed *= (90 - Vector3.Angle(_slopeDirection, Vector3.up)) / 90;
                }
            }

        }

        //run
        private void PlayerInput_OnRunPerformed(object sender, System.EventArgs e)
        {
            Debug.Log("run: "+ _isRunning);
            if (_isWalking)
            {
                _isRunning = !_isRunning;
                _moveSpeed = _isRunning ? RunSpeed : WalkSpeed;
            }
        }

        private void PlayerInput_OnSneakPerformed(object sender, System.EventArgs e)
        {
            _isSneaking = !_isSneaking;
            _moveSpeed = _isSneaking ? SneakSpeed : WalkSpeed;
        }

        private void PlayerInput_OnSwitchCamera(object sender, System.EventArgs e)
        {
            _isFirstPerson = !_isFirstPerson;
            FirstPersonCamera.gameObject.SetActive(_isFirstPerson);
            ThirdPersonCamera.gameObject.SetActive(!_isFirstPerson);
            Camera.main.cullingMask = _isFirstPerson ? ~(1 << LayerMask.NameToLayer("Player")) : -1;

        }




        private void Update()
        {
            Vector3 origin = transform.position;
            origin.y += 1f;
            HandleCamera();
            HandleGroundCheck();
            HandleMovement();
            HandleGravity();
            HandleSlope();
            _controller.Move((_velocity + _velocityOffset) * Time.deltaTime);
            HandleAnimator();

        }

        private void HandleAnimator()
        {
            _animator.SetBool("isWalking", _isWalking);
            _animator.SetBool("isRunning", _isRunning);
            _controller.radius = _isRunning ? 0.5f : 0.3f;

            if (_animator.GetCurrentAnimatorStateInfo(2).IsName("Attack"))
            {
                if (_isRunning || _isWalking)
                {
                    _animator.SetLayerWeight(1, 0.8f);
                    _animator.SetLayerWeight(2, 0.2f);
                }
            }
            else
            {
                _animator.SetLayerWeight(1, 0);
                _animator.SetLayerWeight(2, 1);
            }
            _animator.SetFloat("speed", _isGrounded ? 1f : 0.5f);

        }


        private void HandleGroundCheck()
        {
            // Check whether Player is on the ground
            //isGrounded = Physics.CheckSphere(feetTransform.position, goundCheckRadius, groundLayer);
            _isGrounded = _controller.isGrounded;
        }


        private void HandleMovement()
        {
            Vector2 inputVector = inputManager.GetCameraRotate();
            transform.Rotate(Vector3.up * inputVector.x * RotateSpeed * Time.deltaTime);

            inputVector = inputManager.GetMovementVectorNormalized();
            _isWalking = inputVector.magnitude > 0.1f;
            //if (!_isWalking)
            //{
            //    _isRunning = false;
            //    _isSneaking = false;
            //}
            _velocity = new Vector3(inputVector.x * 0.6f, 0, inputVector.y);
            if (_isRunning && _velocity == Vector3.zero)
            {
                _isRunning = false;
                _moveSpeed = WalkSpeed;
            }
            _velocity = transform.TransformDirection(_velocity) * _moveSpeed;

            if (!_isGrounded)
            {
                Vector3 velocity = new Vector3(_controller.velocity.x, 0, _controller.velocity.z);
                _velocity = inputVector.y != 0 ? Vector3.Lerp(velocity, _velocity, JumpPenalty) : velocity;

            }
        }

        private void HandleGravity()
        {
            _velocity.y += _verticalSpeed;
            if (!_isGrounded)
            {
                _verticalSpeed += 2 * Physics.gravity.y * Time.deltaTime;
            }
        }

        private Vector3 _velocityOffset = Vector3.zero;
        private void HandleSlope()
        {
            if (_isGrounded && _slopeDirection != Vector3.up)
            {
                Vector3 velocity = Vector3.ProjectOnPlane(new Vector3(_velocity.x, 0, _velocity.z), _slopeDirection);
                _velocity = velocity + Vector3.up * _velocity.y;
                if (Vector3.Angle(_slopeDirection, Vector3.up) > _controller.slopeLimit)
                {

                    _velocity *= _velocity.y > 0 ? 0.2f : 1.1f;
                    _velocityOffset += Vector3.ProjectOnPlane(Vector3.up, _slopeDirection) * Physics.gravity.y * Time.deltaTime * 0.5f;
                    _velocityOffset += _velocityOffset.normalized * 0.2f;
                }
            }
            else
            {
                _velocityOffset = Vector3.zero;
            }
        }

        private void HandleCamera()
        {
            Vector2 inputVector = inputManager.GetCameraRotate();
            _verticalRotation -= inputVector.y * RotateSpeed * Time.deltaTime;
            _verticalRotation = Mathf.Clamp(_verticalRotation, _isFirstPerson ? -90f : -60f, _isFirstPerson ? 90f : 60f);
            FirstPersonFollowTarget.transform.localRotation = Quaternion.Euler(_verticalRotation, 0, 0);
            ThirdPersonFollowTarget.transform.localRotation = Quaternion.Euler(_verticalRotation, 0, 0);
        }

        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.collider.CompareTag("Ground"))
            {
                _slopeDirection = hit.normal;
                if (_controller.velocity.y < -10f)
                {
                    //_impulseSource.GenerateImpulse(controller.velocity * 0.5f);
                }
            }

            Rigidbody body = hit.collider.attachedRigidbody;
            if (body == null || body.isKinematic)
            {
                return;
            }
            if (hit.moveDirection.y < -0.3)
            {
                return;
            }
            Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
            body.velocity = pushDir * 1;
        }


    }
}
