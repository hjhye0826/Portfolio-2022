using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldActionPlayerControl : MonoBehaviour
{
    public static float MOVE_SPEED = 7.0f;

    public enum STEP
    {
        NONE = -1,
        MOVE = 0,
        REPAIRING,  //수리
        EATING,     //먹기
        NUM,
    };

    public GUIStyle guistyle;    // 폰트 스타일
    public GUIStyle step_guistyle;

    public Vector3 player_start_pos = new Vector3(0.0f, 2.0f, 0.0f);

    public STEP step = STEP.NONE;
    public STEP next_step = STEP.NONE;
    public float step_timer = 0.0f;
     public float PlayerMoveSpeed { get { return GetPlayerMoveSpeed(); } }

    private struct Key
    {
        public bool up;
        public bool down;
        public bool right;
        public bool left;
        public bool pick;
        public bool action;
    }
    private Key key;

    private GameObject closest_item = null;     // 플레이어 정면에 있는 아이템
    private GameObject carried_item = null;     // 플레이어가 들어 올린 아이템
    private GameObject closest_event = null;    // 플레이어 정면에 있는 이벤트

    private GameObject rocket_model = null;

    private FieldActionItemRoot item_root = null;
    private FieldActionEventRoot event_root = null;
    private FieldActionGameStatus game_status = null;

    void Start()
    {
    }

    public void FieldActionPlayerControlInit()
    {
        this.item_root = GameObject.Find("ItemRoot").GetComponent<FieldActionItemRoot>();

        this.event_root = GameObject.Find("GameManager").GetComponent<FieldActionEventRoot>();
        this.rocket_model = GameObject.Find("Rocket").transform.Find("model").gameObject;

        this.game_status = GameObject.Find("GameManager").GetComponent<FieldActionGameStatus>();
    }

    public void FieldActionPlayerInit()
    {
        this.step = STEP.NONE;
        this.next_step = STEP.MOVE;

        this.transform.position = player_start_pos;

        this.closest_item = null;
        if(this.carried_item != null)
        {
            Destroy(this.carried_item);
        }
        this.carried_item = null;
        this.closest_event = null;
    }

    void Update()
    {
        this.GetInput();
        this.step_timer += Time.deltaTime;

        this.StepUpdate();
        this.StepChangeUpdate();
        this.StepRepeatUpdate();

        var rigidbody = this.GetComponent<Rigidbody>();
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
    }

    void StepUpdate()
    {
        var eat_time = 0.5f;
        var repair_time = 1.0f;

        if (this.next_step == STEP.NONE)
        {
            switch (this.step)
            {
                case STEP.MOVE:
                    do
                    {
                        if (!this.key.action)
                        {
                            break;
                        }

                        if (this.closest_event != null)
                        {
                            if (!this.IsEventIgnitable())
                            {
                                break;
                            }

                            var ignitable_event = this.event_root.GetEventType(this.closest_event);
                            switch (ignitable_event)
                            {
                                case Event.TYPE.ROCKET:
                                    this.next_step = STEP.REPAIRING;
                                    break;
                            }
                            break;
                        }

                        if (this.carried_item != null)
                        {
                            var carried_item_type = this.item_root.GetItemType(this.carried_item);

                            switch (carried_item_type)
                            {
                                case Item.TYPE.APPLE:
                                case Item.TYPE.PLANT:
                                    this.next_step = STEP.EATING;
                                    break;
                            }
                        }

                    } while (false);

                    break;
                case STEP.EATING:
                    if (this.step_timer > eat_time)
                    {
                        this.next_step = STEP.MOVE;
                    }
                    break;
                case STEP.REPAIRING:
                    if (this.step_timer > repair_time)
                    {
                        this.next_step = STEP.MOVE;
                    }
                    break;
            }
        }
    }

    void StepChangeUpdate()
    {
        // 상태가 변했을 때 
        while (this.next_step != STEP.NONE)
        {
            this.step = this.next_step;
            this.next_step = STEP.NONE;

            switch (this.step)
            {
                case STEP.MOVE:
                    break;
                case STEP.EATING:
                    if (this.carried_item != null)
                    {
                        // 체력 회복
                        this.game_status.AddSatiety(
                                                        this.item_root.GetRegainSatiety(this.carried_item));

                        GameObject.Destroy(this.carried_item);
                        this.carried_item = null;
                    }
                    break;
                case STEP.REPAIRING:
                    if (this.carried_item != null)
                    {
                        // 수리
                        this.game_status.AddRepairment(
                                                        this.item_root.GetGainRepairment(this.carried_item));

                        GameObject.Destroy(this.carried_item);
                        this.carried_item = null;
                        this.closest_event = null;
                    }
                    break;
            }

            this.step_timer = 0.0f;
        }
    }
    
    void StepRepeatUpdate()
    {
        // 각 상황에서 반복할 것
        switch (this.step)
        {
            case STEP.MOVE:
                this.MoveControl();
                this.PickorDropControl();
                this.game_status.AlwaysSatiety();
                break;
            case STEP.REPAIRING:
                this.rocket_model.transform.localRotation *= Quaternion.AngleAxis(360.0f / 10.0f * Time.deltaTime, Vector3.up);
                break;
        }

    }

    public float GetPlayerMoveSpeed()
    {
        var speed = MOVE_SPEED;
        var carried_item_type = this.item_root.GetItemType(this.carried_item);

        switch (carried_item_type)
        {
            case Item.TYPE.APPLE:
                speed = MOVE_SPEED * 0.9f;
                break;
            case Item.TYPE.PLANT:
                speed = MOVE_SPEED * 0.8f;
                break;
            case Item.TYPE.IRON:
                speed = MOVE_SPEED * 0.7f;
                break;
        }

        return speed;
    }

    private void GetInput()
    {
        this.key.up = false;
        this.key.down = false;
        this.key.right = false;
        this.key.left = false;

        this.key.up |= Input.GetKey(KeyCode.UpArrow);
        this.key.down |= Input.GetKey(KeyCode.DownArrow);
        this.key.right |= Input.GetKey(KeyCode.RightArrow);
        this.key.left |= Input.GetKey(KeyCode.LeftArrow);
        
        this.key.pick = Input.GetKeyDown(KeyCode.Z);
        this.key.action = Input.GetKeyDown(KeyCode.X);
    }

    private void MoveControl()
    {
        var move_vector = Vector3.zero;
        var position = this.transform.position;        
        var is_moved = false;

        if(this.key.right)
        {
            move_vector += Vector3.right;
            is_moved = true;
        }

        if (this.key.left)
        {
            move_vector += Vector3.left;
            is_moved = true;
        }

        if (this.key.up)
        {
            move_vector += Vector3.forward;
            is_moved = true;
        }

        if (this.key.down)
        {
            move_vector += Vector3.back;
            is_moved = true;
        }

        move_vector.Normalize();
        move_vector *= PlayerMoveSpeed * Time.deltaTime;

        position += move_vector;
        position.y = 0.0f;

        if(position.magnitude > FieldActionGameManager.MOVE_AREA_RADIUS)
        {
            position.Normalize();
            position *= FieldActionGameManager.MOVE_AREA_RADIUS;
        }

        position.y = this.transform.position.y;
        this.transform.position = position;

        if(move_vector.magnitude > 0.01f)
        {
            var q = Quaternion.LookRotation(move_vector, Vector3.up);
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, q, 0.2f);
        }

        if(is_moved)
        {
            var consume = this.item_root.GetConsumeSatiety(this.carried_item);
            this.game_status.AddSatiety(-consume * Time.deltaTime);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        var other_obj = other.gameObject;

        if (other_obj.layer == LayerMask.NameToLayer("Item"))
        {
            if (this.closest_item == null)
            {
                if (this.IsOtherInView(other_obj))
                {
                    this.closest_item = other_obj;
                }
                else if (this.closest_item == other_obj)
                {
                    if (!this.IsOtherInView(other_obj))
                    {
                        this.closest_item = null;
                    }
                }
            }
        }
        else if (other_obj.layer == LayerMask.NameToLayer("Event"))
        {
            if(this.closest_event == null)
            {
                if(this.IsOtherInView(other_obj))
                {
                    this.closest_event = other_obj;
                }
            }
            else if(this.closest_event == other_obj)
            {
                if(!this.IsOtherInView(other_obj))
                {
                    this.closest_event = null;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(this.closest_item == other.gameObject)
        {
            this.closest_item = null;
        }
    }

    private void PickorDropControl()
    {
        if (!this.key.pick)
        {
            return;
        }

        if (this.carried_item == null)
        {
            if (this.closest_item == null)
            {
                return;
            }

            this.carried_item = this.closest_item;
            this.carried_item.transform.parent = this.transform;
            this.carried_item.transform.localPosition = Vector3.up * 2.0f;
            this.closest_item = null;
        }
        else
        {
            var obj = this.carried_item;
            obj.transform.parent = GameObject.Find("ItemRoot").transform;

            var pos = this.transform.position + (Vector3.forward * 1.5f);
            pos.y = 1.5f;
            obj.transform.position = pos;

            this.carried_item = null;
        }
    }

    private bool IsOtherInView(GameObject other)
    {
        var ret = false;

        Vector3 heading = this.transform.TransformDirection(Vector3.forward);
        Vector3 to_other = other.transform.position - this.transform.position;

        heading.y = 0.0f;
        to_other.y = 0.0f;

        heading.Normalize();
        to_other.Normalize();

        var dp = Vector3.Dot(heading, to_other);
        if (dp < Mathf.Cos(45.0f))
        {
            return ret;
        }

        ret = true;
        return ret;
    }

    private bool IsEventIgnitable()
    {
        var ret = false;
        if (this.closest_event == null)
        {
            return ret;
        }

        var carried_item_type = this.item_root.GetItemType(this.carried_item);
        if (!this.event_root.IsEventIgnitable(carried_item_type, this.closest_event))
        {
            return ret;
        }

        ret = true;
        return ret;
    }

    private void OnGUI()
    {
        float x = 20.0f;
        float y = Screen.height - 40.0f;

        // 들고 있는 아이템이 있다면
        if(this.carried_item != null)
        {
            GUI.Label(new Rect(x, y, 200.0f, 20.0f), "Z : 버린다", guistyle);
            do
            {
                if(this.IsEventIgnitable())
                {
                    break;
                }

                if(item_root.GetItemType(this.carried_item) == Item.TYPE.IRON)
                {
                    break;
                }

                GUI.Label(new Rect(x, y - 27.0f, 200.0f, 20.0f), "X : 먹는다", guistyle);

            } while (false);

        }
        else
        {
            // 주목하는 아이템이 있다면
            if(this.closest_item != null)
            {
                GUI.Label(new Rect(x, y, 200.0f, 20.0f), "Z:줍는다", guistyle);
            }
        }
        

        switch (this.step)
        {
            case STEP.EATING:
                GUI.Box(new Rect((Screen.width / 2.0f) - 80.0f, (Screen.height / 2.0f) - 10.0f, 160.0f, 40.0f), "");
                GUI.Label(new Rect((Screen.width / 2.0f) - 20.0f, (Screen.height / 2.0f), 200.0f, 20.0f), "냠냠...", step_guistyle);
                break;
            case STEP.REPAIRING:
                GUI.Box(new Rect((Screen.width / 2.0f) - 80.0f, (Screen.height / 2.0f) - 10.0f, 160.0f, 40.0f), "");
                GUI.Label(new Rect((Screen.width / 2.0f) - 30.0f, (Screen.height / 2.0f), 200.0f, 20.0f), "수리중", step_guistyle);
                break;
        }

        if (this.IsEventIgnitable())
        {
            var message = this.event_root.GetIIgnitableMessage(this.closest_event);
            GUI.Label(new Rect(x, y - 27.0f, 200.0f, 20.0f), $"X : {message}" , guistyle);
        }

    }
}
