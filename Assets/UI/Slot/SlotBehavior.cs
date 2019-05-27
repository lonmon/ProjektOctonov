﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotBehavior : MonoBehaviour
{
    protected ItemBehavior item;
    public Item.useType useType = Item.useType.GENERIC;
    public enum AccesType {
        OPEN,
        TAKEONLY,
        VIEWONLY,
        CLOSED
    }
    public AccesType accessible = AccesType.OPEN;

    protected Text amount;
    protected RawImage slotImage;
    protected RawImage itemImage;

    // Start is called before the first frame update
    void Start()
    {
        amount = GetComponentInChildren<Text>();
        slotImage = GetComponent<RawImage>();
        itemImage = GetComponentsInChildren<RawImage>()[1];
        itemImage.gameObject.SetActive(false);
        updateSlot();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void updateSlot() {
        if (slotImage != null) {
            if (accessible == AccesType.CLOSED) {
                slotImage.color = new Color(1f, 1f, 1f, 0.5f);
            } else {
                slotImage.color = new Color(1, 1, 1, 1);
            }

            if (item == null) {
                amount.text = "";
                itemImage.texture = null;
                itemImage.gameObject.SetActive(false);
            } else {
                if (item.amount == 0) {
                    item = null;
                    amount.text = "";
                    itemImage.texture = null;
                    itemImage.gameObject.SetActive(false);
                } else {
                    if (item.amount == -1) {
                        amount.text = "∞";
                    } else {
                        amount.text = "" + item.amount;
                    }

                    itemImage.texture = Item.getSprite(item.type);
                    itemImage.gameObject.SetActive(true);
                }
            }
        }
    }

    public bool addItem(ItemBehavior neu) {
        if (accessible == AccesType.OPEN) {
            if (neu == null) {
                return true;
            }
            if (item == null) {
                if (useType == Item.useType.GENERIC || useType == neu.useType) {
                    item = neu;
                    neu.take();
                    updateSlot();
                    return true;
                } else {
                    return false;
                }
            }
            if (item.type == neu.type) {
                item.amount += neu.amount;
                Destroy(neu.gameObject);
                updateSlot();
                return true;
            }
        }
        return false;
    }

    public ItemBehavior switchItem(ItemBehavior neu,bool stack) {
        if (accessible == AccesType.OPEN) {
            if (stack && item!=null && neu!=null && neu.type == item.type) {
                item.amount += neu.amount;
                updateSlot();
                return null;
            } else {
                ItemBehavior ret = item;
                item = neu;
                updateSlot();
                return ret;
            }
        } else {
            return neu;
        }
    }

    public ItemBehavior splitItem(int takeAmount) {
        if ((accessible == AccesType.OPEN || accessible == AccesType.TAKEONLY) && item != null) {
            ItemBehavior ret = item.split(takeAmount);
            updateSlot();
            return ret;
        }
        return null;
    }

    public void setItem(ItemBehavior neu) {
        item = neu;
        updateSlot();
    }

    public ItemBehavior viewItem() {
        return item;
    }

    public ItemBehavior takeItem() {
        if ((accessible== AccesType.OPEN || accessible == AccesType.TAKEONLY) && item!=null) { 
            ItemBehavior ret = item;
            item = null;
            updateSlot();
            return ret;
        } else {
            return null;
        }
    }

    public void mirror() {
        transform.localScale = new Vector3(transform.localScale.x * - 1, transform.localScale.y , 1);
        GetComponentInChildren<Text>().transform.localScale = new Vector3(GetComponentInChildren<Text>().transform.localScale.x * -1, GetComponentInChildren<Text>().transform.localScale.y, 1);
        GetComponentsInChildren<RawImage>()[1].transform.localScale = new Vector3(GetComponentsInChildren<RawImage>()[1].transform.localScale.x * -1, GetComponentsInChildren<RawImage>()[1].transform.localScale.y, 1);
    }
}
