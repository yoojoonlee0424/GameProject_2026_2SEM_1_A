using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.Rendering.Universal;


public class PlayerPickup : MonoBehaviour
{
    [Header("직접 연결")]
    [SerializeField]private Animator animator;
    [SerializeField]private PlayerContoller contoller;

    [Header("아이템 탑색")]
    [SerializeField]private float detectionRadius = 2f;

    [Header("줍기 모션")]
    [SerializeField]private float pickupDuration = 1.2f;
    [SerializeField]private float pickupMoment = 0.55f;

    private PickUpItem targetItem;
    private bool isPickingUp;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isPickingUp)
        {
            return;
        }

        targetItem = FindClosestItem();

        if(targetItem == null)
        {
            return;
        }

        Keyboard keyboard = Keyboard.current;

        if(keyboard == null)
        {
            return;
        }

        if(keyboard.eKey.wasPressedThisFrame)
        {
            StartCoroutine(PickupAnimation());
        }


    }

    private PickUpItem FindClosestItem()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, ~0, QueryTriggerInteraction.Collide); //구체 충돌체
        PickUpItem closestItem = null;
        float closestDistance = Mathf.Infinity;

        foreach(Collider itemCollider in colliders) //구체에 들어온 모든걸 검사
        {
            PickUpItem item = itemCollider.GetComponentInParent<PickUpItem>();  //컴포넌트가 PickupItem 검사

            if (item == null) continue;

            float distance = Vector3.Distance(transform.position, item.transform.position);

            if(distance < closestDistance)
            {
                closestDistance = distance;
                closestItem = item;
            }

        }
        return closestItem;
    }

    private IEnumerator PickupAnimation()
    {
        isPickingUp = true;

        PickUpItem itemToCollect = targetItem;

        Vector3 itemDirection = itemToCollect.transform.position - transform.position;
        itemDirection.y = 0;

        if(itemDirection.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(itemDirection);
        }

        contoller.ChangeState(PlayerState.Pickup);
        // 전환선 없이 PickUp01 상태로 이동
        animator.CrossFade("PickUp01", 0.1f);

        yield return new WaitForSeconds(pickupMoment);

        if(itemToCollect != null)
        {
            itemToCollect.Collect();
        }

        yield return new WaitForSeconds(pickupDuration - pickupMoment);

        //Blend Tree로 돌아간다
        animator.CrossFade("Blend Tree", 0.1f);

        contoller.ChangeState(PlayerState.Normal);

        targetItem = null;
        isPickingUp = false;
    }


}
