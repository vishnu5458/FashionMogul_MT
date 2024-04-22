using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviour
{
    public List<ClothStack> cloths = new List<ClothStack>();
    [SerializeField] GameObject[] clothPref;
    [SerializeField] Animator anim;
    [SerializeField] Transform clothStackParent;
    [SerializeField] GameObject fillOBJ;
    [SerializeField] Image fillImage;
    [SerializeField] Rigidbody rb;

    int currShelfType = 0;
    bool IsCollecting = false;
    float fillTimer = 0;

    // input variables
    private float _zMoveInput;
    private float _xMoveInput;
    private float moveSpeed;
    private bool IsMove = false;
    private Transform _cameraTransform;
    [SerializeField] float walkSpeed = 2;
    [SerializeField] float runSpeed = 4;
    [SerializeField] private float _smoothFacing = 0.75f;

    // Start is called before the first frame update
    void Start()
    {
        _cameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();
        if (IsCollecting)
        {
            fillTimer += Time.deltaTime;
            fillImage.fillAmount = (fillTimer / 2);
            if (fillTimer > 2)
            {
                fillTimer = 0;
                fillImage.fillAmount = 0;
                AddClothToStack(currShelfType);
            }
            fillOBJ.transform.LookAt(fillOBJ.transform.position + _cameraTransform.rotation * Vector3.forward, _cameraTransform.rotation * Vector3.up);
        }
    }

    private void PlayerMovement()
    {
        _zMoveInput = CrossPlatformInputManager.GetAxis("Vertical");
        _xMoveInput = CrossPlatformInputManager.GetAxis("Horizontal");
        if (Mathf.Abs(_zMoveInput) > 0.1f || Mathf.Abs(_zMoveInput) > 0.1f || Mathf.Abs(_xMoveInput) > 0.1f || Mathf.Abs(_xMoveInput) > 0.1f && !IsMove)
        {
            if (cloths.Count > 0)
            {
                moveSpeed = walkSpeed;
                anim.SetInteger("State", 1);
            }
            else
            {
                moveSpeed = runSpeed;
                anim.SetInteger("State", 0);
            }
            anim.SetBool("Move", true);
            IsMove = true;
        }
        else if (_zMoveInput < 0.1f && IsMove)
        {
            IsMove = false;
            anim.SetBool("Move", false);
        }
        Move(moveSpeed);
        HandleFacingRotation(_smoothFacing);
    }

    void Move(float moveSpeed)
    {
        rb.velocity = new Vector3(
            RelativeDirection().x * moveSpeed,
            rb.velocity.y,
            RelativeDirection().z * moveSpeed);
    }
    Vector3 RelativeDirection()
    {
        Vector3 zDir = _cameraTransform.forward;
        Vector3 xDir = _cameraTransform.right;
        zDir.y = 0;
        xDir.y = 0;

        return (zDir * _zMoveInput + xDir * _xMoveInput).normalized;
    }

    void HandleFacingRotation(float smoothRotation)
    {
        if (RelativeDirection().magnitude > 0)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(RelativeDirection()),smoothRotation * 10f * Time.deltaTime);
        }
    }

    public void RemoveClothFromStack(int index)
    {
        GameObject stack = cloths[index].gameObject;
        cloths.RemoveAt(index);
        Destroy(stack);
        for (int i = 0; i < cloths.Count; i++)
            cloths[i].transform.localPosition = new Vector3(0, (i * 0.05f), 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ClothShelf>())
        {
            currShelfType = other.GetComponent<ClothShelf>().shelfID;
            IsCollecting = true;
            fillOBJ.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<ClothShelf>())
        {
            IsCollecting = false;
            fillTimer = 0;
            fillImage.fillAmount = 0;
            fillOBJ.SetActive(false);
        }
    }

    void AddClothToStack(int _id)
    {
        GameObject go = Instantiate(clothPref[_id], clothStackParent);
        cloths.Add(go.GetComponent<ClothStack>());
        go.transform.localPosition = new Vector3(0, ((cloths.Count-1)*0.05f), 0);
    }
}
