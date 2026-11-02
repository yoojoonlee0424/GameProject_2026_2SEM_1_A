using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;

public class SimpleInventory : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private TextMeshProUGUI[] slotText;

    private List<inventoryitem> items = new List<inventoryitem>();

    private PlayerContoller playerContoller;
    private PlayerPickup playerPickup;
    private ThirdPersonCamera thirdPersonCamera;
    private bool isOpen;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        playerContoller = GetComponent<PlayerContoller>();
        playerPickup = GetComponent<PlayerPickup>();

        if(Camera.main != null)
        {
            thirdPersonCamera = Camera.main.GetComponent<ThirdPersonCamera>();
        }

        inventoryPanel.SetActive(false);
    }

    void Start()
    {
        UpdateUi();
    }

    // Update is called once per frame
    void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null) return;

        if(keyboard.iKey.wasPressedThisFrame)
        {
            ToggleInventory();
        }
    }

    private void ToggleInventory()
    {
        if(isOpen == false && playerContoller.IsNormalState() == false)
        {
            return;
        }

        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);

        if(isOpen)
        {
            playerContoller.ChangeState(PlayerState.inventory);
            playerPickup.enabled = false;

            if(thirdPersonCamera != null)
            {
                thirdPersonCamera.enabled = false;
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            playerContoller.ChangeState(PlayerState.Normal);
            playerPickup.enabled = true;

            if (thirdPersonCamera != null)
            {
                thirdPersonCamera.enabled = true;
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

    }

    private void UpdateUi()
    {
        for(int i = 0; i < slotText.Length; i++)
        {
            if(i < items.Count)
            {
                slotText[i].text = items[i].itemName + "\n" + items[i].amount + "개";
            }
            else
            {
                slotText[i].text = "비어 있음";
            }
        }
    }

    public bool Additem(string itemName, int amount)
    {
        foreach(inventoryitem item in items)
        {
            if(item.itemName == itemName)
            {
                item.amount += amount;
                UpdateUi();
                return true;
            }
        }

        if(items.Count >= slotText.Length)
        {
            Debug.Log("인벤토리가 가득 찼습니다.");
            return false;
        }

        inventoryitem newitem = new inventoryitem();
        newitem.itemName = itemName;
        newitem.amount = amount;

        items.Add(newitem);
        UpdateUi();

        return true;

    }


}
