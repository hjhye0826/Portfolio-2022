using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event
{
    public enum TYPE
    {
        NONE = -1,
        ROCKET =0,
        NUM,
    }
}

public class FieldActionEventRoot : MonoBehaviour
{
    public Event.TYPE GetEventType(GameObject event_obj)
    {
        var type = Event.TYPE.NONE;
        if(event_obj != null)
        {
            if(event_obj.tag == "Rocket")
            {
                type = Event.TYPE.ROCKET;
            }
        }

        return type;
    }

    // 이벤트 가능여부 확인
    public bool IsEventIgnitable(Item.TYPE carried_item, GameObject event_obj)
    {
        var ret = false;
        var type = Event.TYPE.NONE;

        if(event_obj != null)
        {
            type = this.GetEventType(event_obj);
        }

        switch(type)
        {
            case Event.TYPE.ROCKET:
                if(carried_item == Item.TYPE.IRON)
                {
                    ret = true;
                }
                if(carried_item == Item.TYPE.PLANT)
                {
                    ret = true;
                }
                break;
        }

        return ret;
    }

    public string GetIIgnitableMessage(GameObject event_obj)
    {
        var messaage = "";
        var type = Event.TYPE.NONE;
        if(event_obj != null)
        {
            type = this.GetEventType(event_obj);
        }

        switch(type)
        {
            case Event.TYPE.ROCKET:
                messaage = "수리한다";
                break;
        }

        return messaage;
    }

}
