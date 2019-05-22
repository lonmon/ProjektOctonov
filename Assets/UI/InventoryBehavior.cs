﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;

public class InventoryBehavior : MonoBehaviour
{

    public SlotBehavior slotPrefab;
    public SlotBehavior usableSlotPrefab;
    public SlotBehavior craftingResultPrefab;
    public SlotBehavior craftingSlotPrefab;
    public SlotBehavior craftingExtraPrefab;
    public ItemBehavior itemPrefab;
    public CursorBehavior cursor;
    

    private SlotBehavior[] InventorySlot=new SlotBehavior[40];
    private SlotBehavior[] AmmoSlot = new SlotBehavior[5];
    private SlotBehavior[] ActiveSlot = new SlotBehavior[5];
    private SlotBehavior[] CraftingSlot = new SlotBehavior[6];
    private SlotBehavior CursorSlot;

    // Start is called before the first frame update
    void Start()
    {
        Item.loadSprites();
        Item.loadMaterial();

        GameObject InventoryUI = GameObject.Find("UI_Top");
        GameObject ActiveUI = GameObject.Find("UI_Left");
        GameObject AmmoUI = GameObject.Find("UI_Right");

        CursorSlot = cursor;

        if (CursorSlot == null) {
            Debug.Log(""+ GetComponentsInChildren<CursorBehavior>().Length);
        }
        //crate inventory slots
        for(int i = 0; i < InventorySlot.Length; i++) {
            InventorySlot[i] = Instantiate(slotPrefab, InventoryUI.transform);
            InventorySlot[i].transform.localPosition = new Vector3(100 * (i % 10) - 631 + ((int)i / 10) * 34, 508 - ((int)i/10) * 61,0); 
        }
        //create ammo slots
        for (int i = 0; i < AmmoSlot.Length; i++) {
            AmmoSlot[i] = Instantiate(usableSlotPrefab, AmmoUI.transform);
            AmmoSlot[i].transform.localPosition = new Vector3(-(14 * i) + 746, -(90 * i) - 97, 0);
            AmmoSlot[i].mirror(true);
        }
        //create active slots
        for (int i = 0; i < ActiveSlot.Length; i++) {
            ActiveSlot[i] = Instantiate(usableSlotPrefab, ActiveUI.transform);
            ActiveSlot[i].transform.localPosition = new Vector3((14 * i) - 746, -(90 * i) - 97, 0);
        }
        //create crafting slots
        CraftingSlot[0] = Instantiate(craftingResultPrefab, InventoryUI.transform);
        CraftingSlot[0].transform.localPosition = new Vector3(500, 325, 0);
        CraftingSlot[1] = Instantiate(craftingSlotPrefab, InventoryUI.transform);
        CraftingSlot[1].transform.localPosition = new Vector3(428, 415, 0);
        CraftingSlot[2] = Instantiate(craftingSlotPrefab, InventoryUI.transform);
        CraftingSlot[2].transform.localPosition = new Vector3(572, 415, 0);
        CraftingSlot[2].mirror(true);
        CraftingSlot[3] = Instantiate(slotPrefab, InventoryUI.transform);
        CraftingSlot[3].transform.localPosition = new Vector3(385, 476, 0);
        CraftingSlot[4] = Instantiate(slotPrefab, InventoryUI.transform);
        CraftingSlot[4].transform.localPosition = new Vector3(615, 476, 0);
        CraftingSlot[4].mirror(true);
        CraftingSlot[5] = Instantiate(craftingExtraPrefab, InventoryUI.transform);
        CraftingSlot[5].transform.localPosition = new Vector3(500, 476, 0);


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) {
            giveItem(Item.Type.BATTERY, 20);
        }
        if (Input.GetKeyDown(KeyCode.O)) {
            Item.createItem(Item.Type.BATTERY, GameObject.FindGameObjectWithTag("Player").transform.position);
        }
    }

    public void SetItem(ItemBehavior item,int slot) {
        InventorySlot[slot].setItem(item);
    }

    public ItemBehavior takeItem(int slot) {
        return InventorySlot[slot].takeItem();
    }

    public void switchCursor(SlotBehavior slot) {
        if (CursorSlot.viewItem() != null && slot.viewItem() != null && CursorSlot.viewItem().type == slot.viewItem().type) {
            slot.addItem(CursorSlot.takeItem());
        } else {
            ItemBehavior temp = CursorSlot.takeItem();
            CursorSlot.setItem(slot.takeItem());
            slot.setItem(temp);
        }
    }

    public bool pickUpItem(ItemBehavior item) {
        for (int i = 0; i < InventorySlot.Length; i++) {
            if (InventorySlot[i].viewItem() != null && InventorySlot[i].viewItem().type == item.type) {
                return InventorySlot[i].addItem(item);
            }
        }

        for (int i = 0; i < InventorySlot.Length; i++) {
            if (InventorySlot[i].viewItem()==null) {
                return InventorySlot[i].addItem(item);
            }
        }
        return false;
    }

    public void giveItem(Item.Type type, int amount) {
        ItemBehavior neu = Instantiate(itemPrefab);
        neu.type = type;
        neu.amount = amount;
        pickUpItem(neu);
    }

    public void closeInventory() {
        if (CursorSlot.viewItem() != null) {
            pickUpItem(CursorSlot.takeItem());
        }
    }
}