using Assets.Scripts.Weapon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem.XR;

public class Pick : MonoBehaviour
{
    [SerializeField] public InputManager inputManager;
    [SerializeField] private Animator _animator;
    [SerializeField] public float AttackInterval;



    public GameObject Source;
    private GameObject _item;
    private float _lastAttackTime;
    private bool _attacked;


    private void Awake()
    {
        //Audio = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
    }
    void Start()
    {
        inputManager.OnInteractPerformed += PlayerInput_OnInteractPerformed;
        inputManager.OnAttackPerformed += PlayerInput_OnAttackPerformed;
    }
    void Update()
    {
        if (_item && !_attacked && Time.time - _lastAttackTime > 0.3)
        {
            _attacked = true;
            _item.GetComponent<MeleeWeapon>().Attack();
        }


        //if (Input.GetKeyDown(KeyCode.E) && !_item)
        //{
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    if (Physics.Raycast(ray, out RaycastHit hit))
        //    {
        //        Debug.Log(hit.transform.name);
        //        //get object
        //        _item = hit.transform.gameObject;
        //        if (_item.CompareTag("Equipment"))
        //        {
        //            Rigidbody rb = _item.GetComponent<Rigidbody>();
        //            rb.useGravity = false;
        //            rb.isKinematic = true;
        //            rb.isKinematic = false;
        //            _item.GetComponent<Collider>().enabled = false;
        //            ParentConstraint parentConstraint = _item.AddComponent<ParentConstraint>();
        //            ConstraintSource constraintSource = new ConstraintSource
        //            {
        //                sourceTransform = Source.transform,
        //                weight = 1
        //            };
        //            parentConstraint.AddSource(constraintSource);
        //            parentConstraint.constraintActive = true;
        //            parentConstraint.SetRotationOffset(0, new Vector3(45, 0, 0));

        //        }
        //        else
        //        {
        //            _item = null;
        //        }
        //    }
        //}
        //else
        if (Input.GetKeyDown(KeyCode.Q) && _item)
        {
            _item.GetComponent<Rigidbody>().useGravity = true;
            _item.GetComponent<Collider>().enabled = true;
            Destroy(_item.GetComponent<ParentConstraint>());
            _item = null;

        }
        else if (Input.GetKeyDown(KeyCode.G) && _item)
        {
            _item.GetComponent<Rigidbody>().useGravity = true;
            _item.GetComponent<Collider>().enabled = true;
            Destroy(_item.GetComponent<ParentConstraint>());
            _item.GetComponent<Rigidbody>().velocity = transform.forward * 10 + transform.up * 5;
            _item = null;
        }
    }
    private void PlayerInput_OnInteractPerformed(object sender, System.EventArgs e)
    {
        if (_item)
            return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 5f, ~(1 << LayerMask.NameToLayer("Player"))))
        {
            if (hit.transform.gameObject.CompareTag("Equipment"))
            {
                _item = hit.transform.gameObject;
                if (_item.GetComponent<MeleeWeapon>())
                {
                    _item.GetComponent<MeleeWeapon>().Initialize(gameObject, transform);
                }
                Rigidbody rb = _item.GetComponent<Rigidbody>();
                rb.useGravity = false;
                rb.isKinematic = true;
                rb.isKinematic = false;
                _item.GetComponent<Collider>().enabled = false;
                ParentConstraint parentConstraint = _item.AddComponent<ParentConstraint>();
                ConstraintSource constraintSource = new ConstraintSource
                {
                    sourceTransform = Source.transform,
                    weight = 1
                };
                parentConstraint.AddSource(constraintSource);
                parentConstraint.constraintActive = true;

            }
        }
    }
    private void PlayerInput_OnAttackPerformed(object sender, System.EventArgs e)
    {
        if (!_item)
            return;

        if (Time.time - _lastAttackTime < AttackInterval)
            return;
        if (_item.GetComponent<MeleeWeapon>())
        {
            _lastAttackTime = Time.time;
            _attacked = false;
            _animator.SetTrigger("attack");
        }
    }
}