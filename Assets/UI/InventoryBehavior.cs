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
    public SlotBehavior containerSlotPrefab;
    public Texture[] ContainerImage;

    public ItemBehavior itemPrefab;
    public CursorBehavior cursor;

    private UIBehavior ui;
    private Lasergun gun;
    private RawImage containerUI;
    private ButtonBehavior craftButton;

    private SlotBehavior[] InventorySlot=new SlotBehavior[40];
    private SlotBehavior[] AmmoSlot = new SlotBehavior[5];
    private int activeAmmo = 0;
    private SlotBehavior[] ActiveSlot = new SlotBehavior[5];
    private int activeActive = 0;
    private SlotBehavior[] CraftingSlot = new SlotBehavior[6];
    private SlotBehavior CursorSlot;
    private SlotBehavior[] ContainerSlot = new SlotBehavior[12];

    private ContainerBehavior activeContainer;

    // Start is called before the first frame update
    void Start()
    {
        Item.loadSprites();
        Item.loadModels();
        gun = GameObject.FindGameObjectWithTag("Gun").GetComponent<Lasergun>();
        ui = GetComponent<UIBehavior>();
        containerUI = GameObject.Find("UI_Container").GetComponent<RawImage>();
        containerUI.gameObject.SetActive(false);
        craftButton = GameObject.Find("UI_CraftButton").GetComponent<ButtonBehavior>();
        craftButton.clickEvent = clickButton;
        Crafting.loadTest();

        GameObject InventoryUI = GameObject.Find("UI_Top");
        GameObject ActiveUI = GameObject.Find("UI_Left");
        GameObject AmmoUI = GameObject.Find("UI_Right");

        CursorSlot = cursor;
        CursorSlot.useType = Item.useType.GENERIC;
       
        //crate inventory slots
        for(int i = 0; i < InventorySlot.Length; i++) {
            InventorySlot[i] = Instantiate(slotPrefab, InventoryUI.transform);
            InventorySlot[i].transform.localPosition = new Vector3(100 * (i % 10) - 631 + ((int)i / 10) * 34, 508 - ((int)i/10) * 61,0);
            InventorySlot[i].useType = Item.useType.GENERIC;
        }
        //create ammo slots
        for (int i = 0; i < AmmoSlot.Length; i++) {
            AmmoSlot[i] = Instantiate(usableSlotPrefab, AmmoUI.transform);
            AmmoSlot[i].transform.localPosition = new Vector3(-(14 * i) + 746, -(90 * i) - 97, 0);
            AmmoSlot[i].mirror();
            AmmoSlot[i].useType = Item.useType.AMMO;
        }
        AmmoSlot[0].addItem(Item.createItem(Item.Type.LaserBlue, -1, new Vector3(0, 0, 0)));
        AmmoSlot[0].accessible = SlotBehavior.AccesType.VIEWONLY;
        gun.ammo = (AmmoBehavior)AmmoSlot[0].viewItem();
        //create active slots
        for (int i = 0; i < ActiveSlot.Length; i++) {
            ActiveSlot[i] = Instantiate(usableSlotPrefab, ActiveUI.transform);
            ActiveSlot[i].transform.localPosition = new Vector3((14 * i) - 746, -(90 * i) - 97, 0);
            ActiveSlot[i].useType = Item.useType.ACTIVE;
        }
        //create container slots
        for(int i = 0; i < ContainerSlot.Length; i++) {
            ContainerSlot[i] = Instantiate(containerSlotPrefab, containerUI.transform);
        }
        //create crafting slots
        CraftingSlot[0] = Instantiate(craftingResultPrefab, InventoryUI.transform);
        CraftingSlot[0].transform.localPosition = new Vector3(500, 325, 0);
        CraftingSlot[0].useType = Item.useType.GENERIC;
        CraftingSlot[0].accessible = SlotBehavior.AccesType.TAKEONLY;
        CraftingSlot[1] = Instantiate(craftingSlotPrefab, InventoryUI.transform);
        CraftingSlot[1].transform.localPosition = new Vector3(428, 415, 0);
        CraftingSlot[1].useType = Item.useType.GENERIC;
        CraftingSlot[2] = Instantiate(craftingSlotPrefab, InventoryUI.transform);
        CraftingSlot[2].transform.localPosition = new Vector3(572, 415, 0);
        CraftingSlot[2].useType = Item.useType.GENERIC;
        CraftingSlot[2].mirror();
        CraftingSlot[3] = Instantiate(slotPrefab, InventoryUI.transform);
        CraftingSlot[3].transform.localPosition = new Vector3(385, 476, 0);
        CraftingSlot[3].useType = Item.useType.GENERIC;
        CraftingSlot[4] = Instantiate(slotPrefab, InventoryUI.transform);
        CraftingSlot[4].transform.localPosition = new Vector3(615, 476, 0);
        CraftingSlot[4].useType = Item.useType.GENERIC;
        CraftingSlot[4].mirror();
        CraftingSlot[5] = Instantiate(craftingExtraPrefab, InventoryUI.transform);
        CraftingSlot[5].transform.localPosition = new Vector3(500, 476, 0);
        CraftingSlot[5].useType = Item.useType.GENERIC;
        CraftingSlot[5].accessible = SlotBehavior.AccesType.CLOSED;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) {
            giveItem(Item.Type.Battery,20);
            giveItem(Item.Type.Flashlight, 20);
            giveItem(Item.Type.Ironplate, 20);
            giveItem(Item.Type.Nut, 20);
        }
        if (Input.GetKeyDown(KeyCode.O)) {
            Item.createItem(Item.Type.LaserRed, 10, GameObject.FindGameObjectWithTag("Player").transform.position + new Vector3(0, 0, 1));
        }

        //switch ammo
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            switchAmmo(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            switchAmmo(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            switchAmmo(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4)) {
            switchAmmo(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5)) {
            switchAmmo(4);
        }
        if (Input.mouseScrollDelta.y != 0) {
            scrollAmmo((int)Input.mouseScrollDelta.y);
        }
        AmmoSlot[activeAmmo].updateSlot();
        ActiveSlot[activeActive].updateSlot();
    }

    private void positionAmmoSlots() {
        for (int i = 0; i < AmmoSlot.Length; i++) {
            int p = (i + activeAmmo) % 5;
            AmmoSlot[p].transform.localPosition = new Vector3(-(14 * i) + 746, -(90 * i) - 97, 0);
        }
    }

    private void positionActiveSlots() {
        for (int i = 0; i < ActiveSlot.Length; i++) {
            int p = (i + activeActive) % 5;
            ActiveSlot[p].transform.localPosition = new Vector3((14 * i) - 746, -(90 * i) - 97, 0);
        }
    }

    /* 
    public void SetItem(ItemBehavior item,int slot) {
        InventorySlot[slot].setItem(item);
    }
    */

    public ItemBehavior takeItem(int slot) {
        return InventorySlot[slot].takeItem();
    }
    

    public void switchCursor(SlotBehavior slot) {
        CursorSlot.addItem(slot.switchItem(CursorSlot.takeItem(),true));
    }

    public void splitCursor(SlotBehavior slot) {
        if (CursorSlot.viewItem() == null) {
            CursorSlot.addItem(slot.splitItem(-1));
        } else {
            if (slot.viewItem()==null || slot.viewItem().type==CursorSlot.viewItem().type) {
                slot.addItem(CursorSlot.splitItem(1));
            }
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

    public ItemBehavior scrollAmmo(int count) {
        if (count >= 0) {
            for (int i = 0; i < count; i++) {
                activeAmmo++;
                if (activeAmmo >= 5) {
                    activeAmmo = 0;
                } else {
                    if (AmmoSlot[activeAmmo].viewItem() == null) {
                        i--;
                    }
                }
            }
        } else {
            for (int i = 0; i > count; i--) {
                activeAmmo--;
                if (activeAmmo < 0) {
                    activeAmmo = 4;
                }
                if (AmmoSlot[activeAmmo].viewItem() == null) {
                    i++;
                }
            }
        }
        gun.ammo = (AmmoBehavior)AmmoSlot[activeAmmo].viewItem();
        ui.setCrosshairType(((AmmoBehavior)AmmoSlot[activeAmmo].viewItem()).crosshairType);
        positionAmmoSlots();
        return AmmoSlot[activeAmmo].viewItem();
    }

    public ItemBehavior switchAmmo(int num) {
        num = num % 5;
        if (AmmoSlot[num].viewItem() != null) {
            activeAmmo = num;
            ui.setCrosshairType(((AmmoBehavior)AmmoSlot[activeAmmo].viewItem()).crosshairType);
            gun.ammo = (AmmoBehavior)AmmoSlot[activeAmmo].viewItem();
            positionAmmoSlots();
        }
        return AmmoSlot[activeAmmo].viewItem();
    }

    public ItemBehavior scrollActive(int count) {
        if (count >= 0) {
            for (int i = 0; i < count; i++) {
                activeActive++;
                if (activeActive >= 5) {
                    activeActive = 0;
                } else {
                    if (ActiveSlot[activeActive].viewItem() == null) {
                        i--;
                    }
                }
            }
        } else {
            for (int i = 0; i > count; i--) {
                activeActive--;
                if (activeActive < 0) {
                    activeActive = 4;
                }
                if (ActiveSlot[activeActive].viewItem() == null) {
                    i++;
                }
            }
        }
        positionActiveSlots();
        return ActiveSlot[activeActive].viewItem();
    }

    public ItemBehavior switchActive(int num) {
        num = num % 5;
        if (ActiveSlot[num].viewItem() != null) {
            activeActive = num;
        }
        positionActiveSlots();
        return ActiveSlot[activeActive].viewItem();
    }

    public void giveItem(Item.Type type, int amount) {
        Item.createItem(type, amount, GameObject.FindGameObjectWithTag("Player").transform.position + new Vector3(0, 0, 0));
    }

    public void closeInventory() {
        if (CursorSlot.viewItem() != null) {
            pickUpItem(CursorSlot.takeItem());
        }
        closeContainer();
    }

    public void closeContainer() {
        if (activeContainer != null) {
            for (int i = 0; i < activeContainer.content.Length; i++) {
                activeContainer.content[i] = ContainerSlot[i].viewItem();
                ContainerSlot[i].setItem(null);
            }
            activeContainer.updateContainer();
            activeContainer = null;
            containerUI.gameObject.SetActive(false);
        }
        
    }

    public void openContainer(ContainerBehavior container) {
        activeContainer = container;
        this.containerUI.texture = ContainerImage[(int)container.type];
        switch (container.type) {
            case ContainerBehavior.ContainerType.Tiny:
                for (int i = 0; i < 4; i++) {
                    ContainerSlot[i].gameObject.SetActive(true);
                    ContainerSlot[i].setItem(container.content[i]);
                    ContainerSlot[i].transform.localPosition = new Vector3(96 * (i % 2) - 48, 32 - ((int)i / 2) * 62, 0);
                }
                for (int i = 4; i < ContainerSlot.Length; i++) {
                    ContainerSlot[i].gameObject.SetActive(false);
                    ContainerSlot[i].setItem(null);
                }
                break;

            case ContainerBehavior.ContainerType.Medium:
                for (int i = 0; i < 8; i++) {
                    ContainerSlot[i].gameObject.SetActive(true);
                    ContainerSlot[i].setItem(container.content[i]);
                    ContainerSlot[i].transform.localPosition = new Vector3(96 * (i % 4) - 144, 32 - ((int)i / 4) * 62, 0);
                }
                for (int i = 8; i < ContainerSlot.Length; i++) {
                    ContainerSlot[i].gameObject.SetActive(false);
                    ContainerSlot[i].setItem(null);
                }
                break;

            case ContainerBehavior.ContainerType.Large:
                for (int i = 0; i < 12; i++) {
                    ContainerSlot[i].gameObject.SetActive(true);
                    ContainerSlot[i].setItem(container.content[i]);
                }
                for (int i = 12; i < ContainerSlot.Length; i++) {
                    ContainerSlot[i].gameObject.SetActive(false);
                    ContainerSlot[i].setItem(null);
                }
                break;

            case ContainerBehavior.ContainerType.Olli:
                for (int i = 0; i < 5; i++) {
                    ContainerSlot[i].gameObject.SetActive(true);
                    ContainerSlot[i].setItem(container.content[i]);
                }
                for (int i = 5; i < ContainerSlot.Length; i++) {
                    ContainerSlot[i].gameObject.SetActive(false);
                    ContainerSlot[i].setItem(null);
                }
                ContainerSlot[0].transform.localPosition = new Vector3(-96, 32, 0);
                ContainerSlot[1].transform.localPosition = new Vector3(96, 32, 0);
                ContainerSlot[2].transform.localPosition = new Vector3(-48, -30, 0);
                ContainerSlot[3].transform.localPosition = new Vector3(48, -30, 0);
                ContainerSlot[4].transform.localPosition = new Vector3(0, 32, 0);
                break;
        }
        containerUI.gameObject.SetActive(true);
    }

    public ItemBehavior getActiveAmmo() {
        return AmmoSlot[activeAmmo].viewItem();
    }

    public ItemBehavior getActiveActive() {
        return ActiveSlot[activeActive].viewItem();
    }

    public void craft() {
        ItemBehavior[] ingredients = new ItemBehavior[] { CraftingSlot[1].viewItem() , CraftingSlot[2].viewItem() , CraftingSlot[3].viewItem() , CraftingSlot[4].viewItem() };
        CraftingSlot[0].setItem(Crafting.craft(ingredients, Crafting.CraftingStationType.NONE));
        CraftingSlot[1].updateSlot();
        CraftingSlot[2].updateSlot();
        CraftingSlot[3].updateSlot();
        CraftingSlot[4].updateSlot();
    }

    public void clickButton(string message) {
        craft();
    }
}
