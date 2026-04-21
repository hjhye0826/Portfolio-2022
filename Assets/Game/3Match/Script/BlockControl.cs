using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockControl : MonoBehaviour
{
    public BlockManager blockMgr = null;

    public Block.COLOR color = (Block.COLOR)0;
    public Block.iPosition i_pos;

    public float vanish_timer = -1.0f;
    public float step_timer = 0.0f;

    public Material opague_material;
    public Material transparent_material;

    public Block.STEP step = Block.STEP.NONE;
    public Block.STEP next_step = Block.STEP.NONE;

    public Vector3 position_offset = Vector3.zero;
    private Vector3 position_offset_initial = Vector3.zero;

    private struct StepFall
    {
        public float velocity;
    }
    private StepFall fall;

    void Start()
    {
        this.SetColor(this.color);
        this.next_step = Block.STEP.IDLE;
    }

    void Update()
    {
        if (this.vanish_timer >= 0.0f)
        {
            this.vanish_timer -= Time.deltaTime;
            if (this.vanish_timer < 0.0f)
            {
                if (this.step != Block.STEP.SLIDE)
                {
                    this.vanish_timer = -1.0f;
                    this.next_step = Block.STEP.VACANT;
                }
                else
                {
                    this.vanish_timer = 0.0f;
                }
            }
        }

        this.step_timer += Time.deltaTime;

        // 상태 정보 없음
        if (this.next_step == Block.STEP.NONE)
        {
            switch (this.step)
            {
                case Block.STEP.SLIDE:
                    if (this.step_timer >= blockMgr.SLIDE_TIME)
                    {
                        if (this.vanish_timer == 0.0f)
                        {
                            this.next_step = Block.STEP.VACANT;
                        }
                        else
                        {
                            this.next_step = Block.STEP.IDLE;
                        }
                    }
                    break;

                case Block.STEP.IDLE:
                    this.GetComponent<Renderer>().enabled = true;
                    break;

                case Block.STEP.FALL:
                    if (this.position_offset.y <= 0.0f)
                    {
                        this.next_step = Block.STEP.IDLE;
                        this.position_offset.y = 0.0f;
                    }
                    break;
            }
        }
        else
        {
            this.step = this.next_step;
            this.next_step = Block.STEP.NONE;

            switch (this.step)
            {
                case Block.STEP.IDLE:
                    this.position_offset = Vector3.zero;
                    this.transform.localScale = Vector3.one * 1.0f;
                    break;
                case Block.STEP.GRABBED:
                    this.transform.localScale = Vector3.one * 1.2f;
                    break;
                case Block.STEP.RELERASED:
                    this.position_offset = Vector3.zero;
                    this.transform.localScale = Vector3.one * 1.0f;
                    break;
                case Block.STEP.VACANT:
                    this.position_offset = Vector3.zero;
                    this.SetVisible(false);
                    break;
                case Block.STEP.RESPAWN:
                    int color_index = Random.Range(0, (int)Block.COLOR.NORMAL_COLOR_NUM);
                    this.SetColor((Block.COLOR)color_index);
                    this.next_step = Block.STEP.IDLE;
                    break;
                case Block.STEP.FALL:
                    this.SetVisible(true);
                    this.fall.velocity = 0.0f;
                    break;
            }

            this.step_timer = 0.0f;
        }

        switch (this.step)
        {
            case Block.STEP.GRABBED:
                break;
            case Block.STEP.SLIDE:
                float rate = this.step_timer / blockMgr.SLIDE_TIME;
                rate = Mathf.Min(rate, 1.0f);
                rate = Mathf.Sin(rate * Mathf.PI / 2.0f);
                this.position_offset = Vector3.Lerp(
                    this.position_offset_initial, Vector3.zero, rate);
                break;
            case Block.STEP.FALL:
                this.fall.velocity += Physics.gravity.y * Time.deltaTime * blockMgr.FALL_TIME;
                this.position_offset.y += this.fall.velocity * Time.deltaTime;
                if (this.position_offset.y < 0.0f)
                    this.position_offset.y = 0.0f;

                break;
        }

        Vector3 position = BlockManager.calcBlockPosition(this.i_pos) + this.position_offset;
        this.transform.position = position;

        this.SetColor(this.color);

        if (this.vanish_timer > -0.0f)
        {
            Color color0 = Color.Lerp(this.GetComponent<Renderer>().material.color, Color.white, 0.5f);
            Color color1 = Color.Lerp(this.GetComponent<Renderer>().material.color, Color.black, 0.5f);

            if (this.vanish_timer < blockMgr.VANISH_TIME / 2.0f)
            {
                color0.a = this.vanish_timer / (blockMgr.VANISH_TIME / 2.0f);
                color1.a = color0.a;

                this.GetComponent<Renderer>().material = this.transparent_material;
            }

            float rate = 1.0f - this.vanish_timer / blockMgr.VANISH_TIME;
            this.GetComponent<Renderer>().material.color = Color.Lerp(color0, color1, rate);
        }
    }

    public void SetColor(Block.COLOR color)
    {
        this.color = color;
        Color color_value = blockMgr.GetColorRGB((int)this.color);
        this.GetComponent<Renderer>().material.color = color_value;
    }

    public bool isGrabbable()
    {
        // 잡을 수 있는 상태인지
        bool is_grabbable = false;
        switch (this.step)
        {
            case Block.STEP.IDLE:
                is_grabbable = true;
                break;
        }

        return is_grabbable;
    }

    public void beginSlide(Vector3 offset)
    {
        // 이동 시작을 알리는 메서드
        this.position_offset_initial = offset;
        this.position_offset = this.position_offset_initial;
        this.next_step = Block.STEP.SLIDE;
    }

    public void beginGrab()
    {
        this.next_step = Block.STEP.GRABBED;
    }
    public void endGrab()
    {
        // 놓았을 때 호출
        this.next_step = Block.STEP.IDLE;
    }

    public void toVanishing(float time)
    {
        this.vanish_timer = time;
    }

    public bool isVanishing()
    {
        bool is_vanishing = (this.vanish_timer > 0.0f);
        return is_vanishing;
    }

    public void rewindVanishTimer(float time)
    {
        this.vanish_timer = time;
    }

    public bool isVisible()
    {
        bool is_visible = this.GetComponent<Renderer>().enabled;
        return is_visible;
    }

    public void SetVisible(bool is_visible)
    {
        this.GetComponent<Renderer>().enabled = is_visible;
    }

    public bool isIdle()
    {
        bool is_idle = false;
        if (this.step == Block.STEP.IDLE && this.next_step == Block.STEP.NONE)
            is_idle = true;

        return is_idle;
    }

    public void beginFall(BlockControl start)
    {
        this.next_step = Block.STEP.FALL;
        this.position_offset.y = (float)(start.i_pos.y - this.i_pos.y) * Block.COLLISION_SIZE;
    }

    public void beginRespawn(int start_ipos_y)
    {
        this.position_offset.y = (float)(start_ipos_y - this.i_pos.y) * Block.COLLISION_SIZE;
        this.next_step = Block.STEP.FALL;
        int color_index = Random.Range((int)Block.COLOR.FIRST, (int)Block.COLOR.LAST + 1);
        this.SetColor((Block.COLOR)color_index);
    }

    public bool isVacant()
    {
        bool is_vacant = false;
        if (this.step == Block.STEP.VACANT && this.next_step == Block.STEP.NONE)
            is_vacant = true;

        return is_vacant;
    }

    public bool isSlidng()
    {
        return this.position_offset.x != 0.0f;
    }
}
public class Block
{
    public static float COLLISION_SIZE = 1.0f;  // 블록 충돌 크기

    public struct iPosition
    {
        public int x;
        public int y;
    }

    public enum COLOR
    {
        NONE = -1,
        PINK = 0,
        BLUE,
        YELLOW,
        GREEN,
        MAGENTA,
        ORANGE,
        GRAY,
        NUM,
        FIRST = PINK,
        LAST = ORANGE,
        NORMAL_COLOR_NUM = GRAY,
    };

    public enum DIR4
    {
        NONE = -1,
        RIGHT,
        LEFT,
        UP,
        DOWN,
        NUM,
    };

    public enum STEP
    {
        NONE = -1,
        IDLE = 0,   // 대기중
        GRABBED,    // 잡혀 있음
        RELERASED,  // 떨어진 순간
        SLIDE,      // 슬라이드 중
        VACANT,     // 소멸 중
        RESPAWN,    // 재생성 중
        FALL,       // 낙하 중
        LONG_SLIDE, // 크게 슬라이드 중
        NUM,
    };
}