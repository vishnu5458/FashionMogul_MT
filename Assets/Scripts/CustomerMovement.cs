using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class CustomerMovement : MonoBehaviour
{
    enum CustomerTask { shopeWalk, wait, exitWalk, nothing}
    CustomerTask customerTask;
    Camera camera;
    [SerializeField] CustomerSpot customerSpot;
    public Vector3 custSport;
    public Vector3 exitSport;
    public int clothType;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;

    [SerializeField] Renderer rend;
    [SerializeField] Material[] materials;

    [SerializeField] GameObject clothParent;
    [SerializeField] Image wantedClothImage;
    [SerializeField] Sprite[] clothSprites;
    [SerializeField] Transform playerHeadTrans;

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
    }

    float dist = 0;
    // Update is called once per frame
    void Update()
    {
        if (customerTask == CustomerTask.shopeWalk)
        {
            dist = Vector3.Distance(custSport, transform.position);
            if (dist < 0.1f)
            {
                anim.SetBool("Walking", false);
                customerSpot.IsVacant = false;
                transform.SetPositionAndRotation(customerSpot.transform.position, customerSpot.transform.rotation);
                wantedClothImage.sprite = clothSprites[clothType];
                clothParent.SetActive(true);
                customerTask = CustomerTask.wait;
            }
        }
        else if (customerTask == CustomerTask.exitWalk)
        {
            dist = Vector3.Distance(exitSport, transform.position);
            if (dist < 0.1f)
            {
                anim.SetBool("Walking", false);
                customerTask = CustomerTask.nothing;
            }
        }
        else if (customerTask == CustomerTask.wait)
            clothParent.transform.LookAt(transform.position + camera.transform.rotation * Vector3.forward, camera.transform.rotation * Vector3.up);
    }

    public void SetShopPosition(CustomerSpot spot, Vector3 _exit)
    {
        customerTask = CustomerTask.shopeWalk;
        customerSpot = spot;
        custSport = spot.transform.position;
        exitSport = _exit;
        agent.destination = custSport;
        anim.SetBool("Walking", true);
        clothType = Random.Range(0, 2);
    }

    public void SetExitPosition(Vector3 _pos)
    {
        customerTask = CustomerTask.exitWalk;
        clothParent.SetActive(false);
        exitSport = _pos;
        agent.destination = exitSport;
        anim.SetBool("Walking", true);
    }

    PlayerController playerController;
    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            playerController = other.GetComponent<PlayerController>();
            for (int i = 0; i < playerController.cloths.Count; i++)
            {
                if (clothType == playerController.cloths[i].ID)
                {
                    playerController.RemoveClothFromStack(i);
                    rend.material = materials[clothType];
                    SetExitPosition(exitSport);
                }
            }
        }
    }
}
