using UnityEngine;

public class Item
{
    public enum TYPE
    {
        NONE = -1,
        IRON = 0,
        APPLE,
        PLANT,
        NUM
    };

}

public class FieldAdventureItemRoot : MonoBehaviour
{

    public Item.TYPE GetItemType(GameObject obj)
    {
        Item.TYPE type = Item.TYPE.NONE;
        if(obj != null)
        {
            switch(obj.tag)
            {
                case "Iron" : type = Item.TYPE.IRON; break;
                case "Apple": type = Item.TYPE.APPLE; break;
                case "Plant": type = Item.TYPE.PLANT; break;
            }
        }

        return (type);
    }

    public float GetGainRepairment(GameObject item_obj)
    {
        var gain = 0.0f;
        if(item_obj == null)
        {
            gain = 0.0f;
        }
        else
        {
            var type = this.GetItemType(item_obj);
            switch(type)
            {
                case Item.TYPE.IRON:
                    gain = FieldAdventureGameStatus.GAIN_REPAIRMENT_IRON;
                    break;
                case Item.TYPE.PLANT:
                    gain = FieldAdventureGameStatus.GAIN_REPAIRMENT_PLANT;
                    break;
            }
        }

        return gain;
    }

    public float GetConsumeSatiety(GameObject item_obj)
    {
        var consume = 0.0f;
        if(item_obj == null)
        {
            consume = 0.0f;
        }
        else
        {
            var type = this.GetItemType(item_obj);
            switch(type)
            {
                case Item.TYPE.IRON:
                    consume = FieldAdventureGameStatus.CONSUME_SATIETY_IRON;
                    break;
                case Item.TYPE.APPLE:
                    consume = FieldAdventureGameStatus.CONSUME_SATIETY_APPLE;
                    break;
                case Item.TYPE.PLANT:
                    consume = FieldAdventureGameStatus.CONSUME_SATIETY_PLANT;
                    break;
            }
        }

        return consume;
    }

    public float GetRegainSatiety(GameObject item_obj)
    {
        var regain = 0.0f;
        if(item_obj == null)
        {
            regain = 0.0f;
        }
        else
        {
            var type = this.GetItemType(item_obj);
            switch(type)
            {
                case Item.TYPE.APPLE:
                    regain = FieldAdventureGameStatus.REGAIN_SATIETY_APPLE;
                    break;
                case Item.TYPE.PLANT:
                    regain = FieldAdventureGameStatus.REGAIN_SATIETY_PLANT;
                    break;
            }
        }

        return regain;
    }
}
