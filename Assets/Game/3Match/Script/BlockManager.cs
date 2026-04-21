using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    public static int BLOCK_MAX_X = 9;
    public static int BLOCK_MAX_Y = 9;

    public float SLIDE_TIME = 0.8f;
    public float VANISH_TIME = 1.0f;     // 매치 후 사라질 때 까지의 시간
    public float FALL_TIME = 0.5f;

    public GameObject BlockPrefab = null;
    public BlockControl[,] blocks;

    public Match3GameManager gameMgr = null;
    public ColorDataManager colorDataMgr = null;

    protected bool is_vanishing_prev = false;

    private RaycastHit hit;
    private BlockControl grabbedBlock = null;

    void Start()
    {
        colorDataMgr.LoadBlockColor();
    }

    void Update()
    {
        if (!gameMgr.isGamePlay())
            return;

        if (!is_able_block_click())
            return;

        // 잡고 있는 블럭이 없을 때
        if (grabbedBlock == null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    GameObject gameObj = hit.transform.gameObject;
                    grabbedBlock = gameObj.GetComponent<BlockControl>();
                    grabbedBlock.beginGrab();
                    return;
                }
            }
        }
        else // 블럭 잡고 있을 때
        {
            if (Input.GetMouseButtonUp(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    GameObject gameObj = hit.transform.gameObject;
                    BlockControl outBlock = gameObj.GetComponent<BlockControl>();
                    Block.DIR4 slide_dir = calcSlideDir(grabbedBlock, outBlock);
                    if (slide_dir == Block.DIR4.NONE)
                    {
                        grabbedBlock.endGrab();
                        grabbedBlock = null;
                        return;
                    }

                    swapBlockcheck(grabbedBlock, slide_dir);
                    grabbedBlock = null;
                }
                else
                {
                    grabbedBlock.endGrab();
                    grabbedBlock = null;
                    return;
                }
            }
        }


        int ignite_count = 0;
        foreach (BlockControl block in blocks)
        {
            if (!block.isIdle())
                continue;

            if (checkConnection(block))
                ++ignite_count;
        }

        if (ignite_count > 0)
        {
            int block_count = 0;
            foreach (BlockControl block in blocks)
            {
                if (block.isVanishing())
                {
                    block.rewindVanishTimer(VANISH_TIME);
                    ++block_count;
                }
            }

            gameMgr.UpdateMatchCount(ignite_count);
            gameMgr.UpdateScore(block_count * 10);
        }

        do
        {
            for (int x = 0; x < BLOCK_MAX_X; ++x)
            {
                if (is_has_sliding_block_in_column(x))
                    continue;

                for (int y = 0; y < BLOCK_MAX_Y - 1; ++y)
                {
                    if (!blocks[x, y].isVacant())
                        continue;

                    for (int y1 = y + 1; y1 < BLOCK_MAX_Y; ++y1)
                    {
                        if (blocks[x, y1].isVacant())
                            continue;

                        fallBlock(blocks[x, y], Block.DIR4.UP, blocks[x, y1]);
                        break;
                    }

                }
            }

            for (int x = 0; x < BLOCK_MAX_X; ++x)
            {
                int fall_start_y = BLOCK_MAX_Y;
                for (int y = 0; y < BLOCK_MAX_Y; ++y)
                {
                    if (!blocks[x, y].isVacant())
                        continue;

                    blocks[x, y].beginRespawn(fall_start_y);
                    fall_start_y++;
                }
            }


        } while (false);

        bool is_vanishing = is_has_vanishing_block();
        is_vanishing_prev = is_vanishing;
    }

    public void InitBlocks()
    {
        ClearBlocks();

        blocks = new BlockControl[BLOCK_MAX_X, BLOCK_MAX_Y];

        int color_index = 0;
        for (int y = 0; y < BLOCK_MAX_Y; y++)
        {
            for (int x = 0; x < BLOCK_MAX_X; x++)
            {
                // 블록 생성 및 그리드에 저장
                GameObject obj = Instantiate(BlockPrefab);
                BlockControl block = obj.GetComponent<BlockControl>();
                blocks[x, y] = block;

                // 블록의 위치 정보 설정
                block.i_pos.x = x;
                block.i_pos.y = y;

                block.blockMgr = this;
                Vector3 position = BlockManager.calcBlockPosition(block.i_pos);
                block.transform.position = position;

                color_index = Random.Range(0, (int)Block.COLOR.NORMAL_COLOR_NUM);
                block.SetColor((Block.COLOR)color_index);

                block.name = "block(" + block.i_pos.x.ToString() + "," + block.i_pos.y.ToString() + ")";
            }
        }
    }

    public void ClearBlocks()
    {
        if (blocks == null) return;

        for (int y = 0; y < BLOCK_MAX_Y; y++)
        {
            for (int x = 0; x < BLOCK_MAX_X; x++)
            {
                if (blocks[x, y] != null)
                {
                    Destroy(blocks[x, y].gameObject);
                    blocks[x, y] = null;
                }
            }
        }
        blocks = null;
    }

    public static Vector3 calcBlockPosition(Block.iPosition i_pos)
    {
        Vector3 position = new Vector3(-(BLOCK_MAX_X / 2.0f - 0.5f), -(BLOCK_MAX_Y / 2.0f - 0.5f), 0.0f);

        position.x += (float)i_pos.x * Block.COLLISION_SIZE;
        position.y += (float)i_pos.y * Block.COLLISION_SIZE;

        return position;
    }

    public static Vector3 getDirVector(Block.DIR4 dir)
    {
        Vector3 v = Vector3.zero;
        switch (dir)
        {
            case Block.DIR4.RIGHT:
                v = Vector3.right;
                break;
            case Block.DIR4.LEFT:
                v = Vector3.left;
                break;
            case Block.DIR4.UP:
                v = Vector3.up;
                break;
            case Block.DIR4.DOWN:
                v = Vector3.down;
                break;
        }

        v *= Block.COLLISION_SIZE;
        return v;
    }
    
    public static Block.DIR4 getOppositDir(Block.DIR4 dir)
    {
        Block.DIR4 opposit = dir;
        switch (dir)
        {
            case Block.DIR4.RIGHT:
                opposit = Block.DIR4.LEFT;
                break;
            case Block.DIR4.LEFT:
                opposit = Block.DIR4.RIGHT;
                break;
            case Block.DIR4.UP:
                opposit = Block.DIR4.DOWN;
                break;
            case Block.DIR4.DOWN:
                opposit = Block.DIR4.UP;
                break;
        }

        return opposit;
    }

    public Block.DIR4 calcSlideDir(Vector2 startPos, Vector2 endPos)
    {
        Block.DIR4 dir = Block.DIR4.NONE;
        Vector2 v = endPos - startPos;

        if (v.magnitude < 0.1f)
            return Block.DIR4.NONE;

        v = v.normalized;

        if (v.x > 0)
        {
            if (v.y <= 1 && v.y > 0.45)
                dir = Block.DIR4.UP;
            else if (v.y <= 0.45 && v.y > -0.45)
                dir = Block.DIR4.RIGHT;
            else
                dir = Block.DIR4.DOWN;
        }
        else
        {
            if (v.y <= 1 && v.y > 0.45)
                dir = Block.DIR4.UP;
            else if (v.y <= 0.45 && v.y > -0.45)
                dir = Block.DIR4.LEFT;
            else
                dir = Block.DIR4.DOWN;
        }

        return dir;
    }

    public Block.DIR4 calcSlideDir(BlockControl startBlock, BlockControl endBlock)
    {
        Vector2 startPos = new Vector2(startBlock.transform.position.x, 
                                        startBlock.transform.position.y);
        Vector2 endPos = new Vector2(endBlock.transform.position.x, 
                                        endBlock.transform.position.y);

        Block.DIR4 dir = Block.DIR4.NONE;
        Vector2 v = endPos - startPos;

        if (v.magnitude < 0.1f)
            return Block.DIR4.NONE;

        v = v.normalized;

        if (v.x > 0)
        {
            if (v.y <= 1 && v.y > 0.45)
                dir = Block.DIR4.UP;
            else if (v.y <= 0.45 && v.y > -0.45)
                dir = Block.DIR4.RIGHT;
            else
                dir = Block.DIR4.DOWN;
        }
        else
        {
            if (v.y <= 1 && v.y > 0.45)
                dir = Block.DIR4.UP;
            else if (v.y <= 0.45 && v.y > -0.45)
                dir = Block.DIR4.LEFT;
            else
                dir = Block.DIR4.DOWN;
        }

        return dir;
    }

    public BlockControl getNextBlock(BlockControl block, Block.DIR4 dir)
    {
        BlockControl next_block = null;
        switch (dir)
        {
            case Block.DIR4.RIGHT:
                if (block.i_pos.x < BLOCK_MAX_X - 1)
                    next_block = blocks[block.i_pos.x + 1, block.i_pos.y];
                break;
            case Block.DIR4.LEFT:
                if (block.i_pos.x > 0)
                    next_block = blocks[block.i_pos.x - 1, block.i_pos.y];
                break;
            case Block.DIR4.UP:
                if (block.i_pos.y < BLOCK_MAX_Y - 1)
                    next_block = blocks[block.i_pos.x, block.i_pos.y + 1];
                break;
            case Block.DIR4.DOWN:
                if (block.i_pos.y > 0)
                    next_block = blocks[block.i_pos.x, block.i_pos.y - 1];
                break;
        }

        return next_block;
    }

    public void swapBlockcheck(BlockControl grabbed_block, Block.DIR4 slide_dir)
    {
        BlockControl swap_target = getNextBlock(grabbed_block, slide_dir);
        if (swap_target == null)
        {
            grabbed_block.endGrab();
            return;
        }

        swapBlock(grabbed_block, slide_dir, swap_target);
        grabbed_block.endGrab();
    }

    public void swapBlock(BlockControl block0, Block.DIR4 dir, BlockControl block1)
    {
        Block.COLOR color0 = block0.color;
        Block.COLOR color1 = block1.color;

        Vector3 scale0 = block0.transform.localScale;
        Vector3 scale1 = block1.transform.localScale;

        float vanish_timer0 = block0.vanish_timer;
        float vanish_timer1 = block1.vanish_timer;

        Vector3 offset0 = BlockManager.getDirVector(dir);
        Vector3 offset1 = BlockManager.getDirVector(BlockManager.getOppositDir(dir));

        block0.SetColor(color1);
        block1.SetColor(color0);

        block0.transform.localScale = scale1;
        block1.transform.localScale = scale0;

        block0.vanish_timer = vanish_timer1;
        block1.vanish_timer = vanish_timer0;

        block0.beginSlide(offset0);
        block1.beginSlide(offset1);
    }

    public bool checkConnection(BlockControl start)
    {
        bool ret = false;
        int normal_block_num = 0;

        if (!start.isVanishing())
            normal_block_num = 1;

        // 가로 세로 3매치 확인
        int rx = start.i_pos.x;
        int lx = start.i_pos.x;

        for (int x = lx - 1; x > 0; --x)
        {
            BlockControl next_block = blocks[x, start.i_pos.y];
            if (next_block.color != start.color)
                break;

            if (next_block.step == Block.STEP.FALL || 
                next_block.next_step == Block.STEP.FALL || 
                next_block.step == Block.STEP.SLIDE || 
                next_block.next_step == Block.STEP.SLIDE)
                break;

            if (!next_block.isVanishing())
                ++normal_block_num;

            lx = x;
        }

        for (int x = rx + 1; x < BLOCK_MAX_X; ++x)
        {
            BlockControl next_block = blocks[x, start.i_pos.y];
            if (next_block.color != start.color)
                break;

            if (next_block.step == Block.STEP.FALL || next_block.next_step == Block.STEP.FALL)
                break;

            if (next_block.step == Block.STEP.SLIDE || next_block.next_step == Block.STEP.SLIDE)
                break;

            if (!next_block.isVanishing())
                normal_block_num++;

            rx = x;
        }

        do
        {
            if (rx - lx + 1 < 3)
                break;

            if (normal_block_num == 0)
                break;

            for (int x = lx; x < rx + 1; ++x)
            {
                blocks[x, start.i_pos.y].toVanishing(VANISH_TIME);
                ret = true;
            }
        } while (false);

        normal_block_num = 0;
        if (!start.isVanishing())
            normal_block_num = 1;


        // 위아래 3매치 확인
        int uy = start.i_pos.y;
        int dy = start.i_pos.y;

        // 블록 위 
        for (int y = dy - 1; y > 0; --y)
        {
            BlockControl next_block = blocks[start.i_pos.x, y];
            if (next_block.color != start.color)
                break;

            if (next_block.step == Block.STEP.FALL || next_block.next_step == Block.STEP.FALL)
                break;

            if (next_block.step == Block.STEP.SLIDE || next_block.next_step == Block.STEP.SLIDE)
                break;

            if (!next_block.isVanishing())
                normal_block_num++;

            dy = y;
        }

        // 블록 아래 검사
        for (int y = uy + 1; y < BLOCK_MAX_Y; ++y)
        {
            BlockControl next_block = blocks[start.i_pos.x, y];
            if (next_block.color != start.color)
                break;

            if (next_block.step == Block.STEP.FALL || next_block.next_step == Block.STEP.FALL)
                break;

            if (next_block.step == Block.STEP.SLIDE || next_block.next_step == Block.STEP.SLIDE)
                break;

            if (!next_block.isVanishing())
                normal_block_num++;

            uy = y;
        }

        // 3매치 확인
        do
        {
            if (uy - dy + 1 < 3)
                break;

            if (normal_block_num == 0)
                break;

            for (int y = dy; y < uy + 1; y++)
            {
                blocks[start.i_pos.x, y].toVanishing(VANISH_TIME);
                ret = true;
            }
        } while (false);


        return ret;
    }

    public void fallBlock(BlockControl block0, Block.DIR4 dir, BlockControl block1)
    {
        Block.COLOR color0 = block0.color;
        Block.COLOR color1 = block1.color;

        Vector3 scale0 = block0.transform.localScale;
        Vector3 scale1 = block1.transform.localScale;

        float vanish_timer0 = block0.vanish_timer;
        float vanish_timer1 = block1.vanish_timer;

        bool visible0 = block0.isVisible();
        bool visible1 = block1.isVisible();

        Block.STEP step0 = block0.step;
        Block.STEP step1 = block1.step;

        // 속성 교체
        block0.SetColor(color1);
        block1.SetColor(color0);

        block0.transform.localScale = scale1;
        block1.transform.localScale = scale0;

        block0.vanish_timer = vanish_timer1;
        block1.vanish_timer = vanish_timer0;

        block0.SetVisible(visible1);
        block1.SetVisible(visible0);

        block0.step = step1;
        block1.step = step0;

        block0.beginFall(block1);
    }

    public bool is_able_block_click()
    {
        bool ret = true;

        if (!gameMgr.isGamePlay())
            ret = false;

        if (is_has_falling_block())
            ret = false;

        if (is_has_sliding_block())
            ret = false;

        if (is_has_vanishing_block())
            ret = false;

        return ret;
    }

    private bool is_has_vanishing_block()
    {
        foreach (BlockControl block in blocks)
        {
            if (block.vanish_timer > 0.0f)
                return true;
        }

        return false;
    }

    private bool is_has_sliding_block()
    {
        foreach (BlockControl block in blocks)
        {
            if (block.step == Block.STEP.SLIDE)
                return true;
        }

        return false;
    }

    private bool is_has_falling_block()
    {
        foreach (BlockControl block in blocks)
        {
            if (block.step == Block.STEP.FALL)
                return true;
        }

        return false;
    }

    private bool is_has_sliding_block_in_column(int x)
    {
        bool ret = false;
        for (int y = 0; y < BLOCK_MAX_Y; y++)
        {
            if (blocks[x, y].isSlidng())
            {
                ret = true;
                break;
            }
        }

        return ret;
    }

    public Color GetColorRGB(int index)
    {
        return colorDataMgr.GetColor(index);
    }
}
