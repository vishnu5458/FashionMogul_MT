using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerController : MonoBehaviour
{

    [SerializeField] CustomerMovement[] customers;
    [SerializeField] CustomerSpot[] shopSpots;
    [SerializeField] Transform exitSpot;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < customers.Length; i++)
            AssignCustomer(customers[i]);
    }


    public void AssignCustomer(CustomerMovement customer)
    {
        foreach (CustomerSpot spot in shopSpots)
        {
            if (!spot.IsVacant)
            {
                customer.SetShopPosition(spot, exitSpot.position);
                spot.IsVacant = true;
                return;
            }
        }
    }
}
