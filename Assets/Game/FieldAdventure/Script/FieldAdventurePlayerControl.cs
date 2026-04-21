using UnityEngine;

public class FieldAdventurePlayerControl : MonoBehaviour
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

    private FieldAdventureItemRoot item_root = null;
    private FieldAdventureEventRoot event_root = null;
    private FieldAdventureGameStatus game_status = null;

    void Start()
    {
    }

    public void FieldActionPlayerControlInit()
    {
        item_root = GameObject.Find("ItemRoot").GetComponent<FieldAdventureItemRoot>();

        event_root = GameObject.Find("GameManager").GetComponent<FieldAdventureEventRoot>();
        rocket_model = GameObject.Find("Rocket").transform.Find("model").gameObject;

        game_status = GameObject.Find("GameManager").GetComponent<FieldAdventureGameStatus>();
    }

    public void FieldActionPlayerInit()
    {
        step = STEP.NONE;
        next_step = STEP.MOVE;

        transform.position = player_start_pos;

        closest_item = null;
        if(carried_item != null)
        {
            Destroy(carried_item);
        }
        carried_item = null;
        closest_event = null;
    }

    void Update()
    {
        GetInput();
        step_timer += Time.deltaTime;

        StepUpdate();
        StepChangeUpdate();
        StepRepeatUpdate();

        var rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
    }

    void StepUpdate()
    {
        var eat_time = 0.5f;
        var repair_time = 1.0f;

        if (next_step == STEP.NONE)
        {
            switch (step)
            {
                case STEP.MOVE:
                    do
                    {
                        if (!key.action)
                        {
                            break;
                        }

                        if (closest_event != null)
                        {
                            if (!IsEventIgnitable())
                            {
                                break;
                            }

                            var ignitable_event = event_root.GetEventType(closest_event);
                            switch (ignitable_event)
                            {
                                case Event.TYPE.ROCKET:
                                    next_step = STEP.REPAIRING;
                                    break;
                            }
                            break;
                        }

                        if (carried_item != null)
                        {
                            var carried_item_type = item_root.GetItemType(carried_item);

                            switch (carried_item_type)
                            {
                                case Item.TYPE.APPLE:
                                case Item.TYPE.PLANT:
                                    next_step = STEP.EATING;
                                    break;
                            }
                        }

                    } while (false);

                    break;
                case STEP.EATING:
                    if (step_timer > eat_time)
                    {
                        next_step = STEP.MOVE;
                    }
                    break;
                case STEP.REPAIRING:
                    if (step_timer > repair_time)
                    {
                        next_step = STEP.MOVE;
                    }
                    break;
            }
        }
    }

    void StepChangeUpdate()
    {
        // 상태가 변했을 때 
        while (next_step != STEP.NONE)
        {
            step = next_step;
            next_step = STEP.NONE;

            switch (step)
            {
                case STEP.MOVE:
                    break;
                case STEP.EATING:
                    if (carried_item != null)
                    {
                        // 체력 회복
                        game_status.AddSatiety(item_root.GetRegainSatiety(carried_item));

                        GameObject.Destroy(carried_item);
                        carried_item = null;
                    }
                    break;
                case STEP.REPAIRING:
                    if (carried_item != null)
                    {
                        // 수리
                        game_status.AddRepairment(item_root.GetGainRepairment(carried_item));

                        GameObject.Destroy(carried_item);
                        carried_item = null;
                        closest_event = null;
                    }
                    break;
            }

            step_timer = 0.0f;
        }
    }
    
    void StepRepeatUpdate()
    {
        // 각 상황에서 반복할 것
        switch (step)
        {
            case STEP.MOVE:
                MoveControl();
                PickorDropControl();
                game_status.AlwaysSatiety();
                break;
            case STEP.REPAIRING:
                rocket_model.transform.localRotation *= Quaternion.AngleAxis(360.0f / 10.0f * Time.deltaTime, Vector3.up);
                break;
        }

    }

    public float GetPlayerMoveSpeed()
    {
        var speed = MOVE_SPEED;
        var carried_item_type = item_root.GetItemType(carried_item);

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
        key.up = false;
        key.down = false;
        key.right = false;
        key.left = false;

        key.up |= Input.GetKey(KeyCode.UpArrow);
        key.down |= Input.GetKey(KeyCode.DownArrow);
        key.right |= Input.GetKey(KeyCode.RightArrow);
        key.left |= Input.GetKey(KeyCode.LeftArrow);
        
        key.pick = Input.GetKeyDown(KeyCode.Z);
        key.action = Input.GetKeyDown(KeyCode.X);
    }

    private void MoveControl()
    {
        var move_vector = Vector3.zero;
        var position = transform.position;        
        var is_moved = false;

        if(key.right)
        {
            move_vector += Vector3.right;
            is_moved = true;
        }

        if (key.left)
        {
            move_vector += Vector3.left;
            is_moved = true;
        }

        if (key.up)
        {
            move_vector += Vector3.forward;
            is_moved = true;
        }

        if (key.down)
        {
            move_vector += Vector3.back;
            is_moved = true;
        }

        move_vector.Normalize();
        move_vector *= PlayerMoveSpeed * Time.deltaTime;

        position += move_vector;
        position.y = 0.0f;

        if(position.magnitude > FieldAdventureGameManager.MOVE_AREA_RADIUS)
        {
            position.Normalize();
            position *= FieldAdventureGameManager.MOVE_AREA_RADIUS;
        }

        position.y = transform.position.y;
        transform.position = position;

        if(move_vector.magnitude > 0.01f)
        {
            var q = Quaternion.LookRotation(move_vector, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, q, 0.2f);
        }

        if(is_moved)
        {
            var consume = item_root.GetConsumeSatiety(carried_item);
            game_status.AddSatiety(-consume * Time.deltaTime);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        var other_obj = other.gameObject;

        if (other_obj.layer == LayerMask.NameToLayer("Item"))
        {
            if (closest_item == null)
            {
                if (IsOtherInView(other_obj))
                {
                    closest_item = other_obj;
                }
                else if (closest_item == other_obj)
                {
                    if (!IsOtherInView(other_obj))
                    {
                        closest_item = null;
                    }
                }
            }
        }
        else if (other_obj.layer == LayerMask.NameToLayer("Event"))
        {
            if(closest_event == null)
            {
                if(IsOtherInView(other_obj))
                {
                    closest_event = other_obj;
                }
            }
            else if(closest_event == other_obj)
            {
                if(!IsOtherInView(other_obj))
                {
                    closest_event = null;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(closest_item == other.gameObject)
        {
            closest_item = null;
        }
    }

    private void PickorDropControl()
    {
        if (!key.pick)
        {
            return;
        }

        if (carried_item == null)
        {
            if (closest_item == null)
            {
                return;
            }

            carried_item = closest_item;
            carried_item.transform.parent = transform;
            carried_item.transform.localPosition = Vector3.up * 2.0f;
            closest_item = null;
        }
        else
        {
            var obj = carried_item;
            obj.transform.parent = GameObject.Find("ItemRoot").transform;

            var pos = transform.position + (Vector3.forward * 1.5f);
            pos.y = 1.5f;
            obj.transform.position = pos;

            carried_item = null;
        }
    }

    private bool IsOtherInView(GameObject other)
    {
        var ret = false;

        Vector3 heading = transform.TransformDirection(Vector3.forward);
        Vector3 to_other = other.transform.position - transform.position;

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
        if (closest_event == null)
        {
            return ret;
        }

        var carried_item_type = item_root.GetItemType(carried_item);
        if (!event_root.IsEventIgnitable(carried_item_type, closest_event))
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
        if(carried_item != null)
        {
            GUI.Label(new Rect(x, y, 200.0f, 20.0f), "Z : 버린다", guistyle);
            do
            {
                if(IsEventIgnitable())
                {
                    break;
                }

                if(item_root.GetItemType(carried_item) == Item.TYPE.IRON)
                {
                    break;
                }

                GUI.Label(new Rect(x, y - 27.0f, 200.0f, 20.0f), "X : 먹는다", guistyle);

            } while (false);

        }
        else
        {
            // 주목하는 아이템이 있다면
            if(closest_item != null)
            {
                GUI.Label(new Rect(x, y, 200.0f, 20.0f), "Z : 줍는다", guistyle);
            }
        }
        

        switch (step)
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

        if (IsEventIgnitable())
        {
            var message = event_root.GetIIgnitableMessage(closest_event);
            GUI.Label(new Rect(x, y - 27.0f, 200.0f, 20.0f), $"X : {message}" , guistyle);
        }

    }
}
