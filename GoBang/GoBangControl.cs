using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GoBangControl : MonoBehaviour
{
    public GameObject lefttop,righttop,leftbottom,rightbottom;//棋盘上的四个顶点
    public Camera camera;//镜头
    private Vector3 ltpos, rtpos, lbpos, rbpos, position;//四个顶点的位置和鼠标点击的位置

    public Button back,regame;
    public GameObject blackchessprefab, whitechessprefab;
    public Image blackwin, whitewin;

    private bool isblackturn;//是否是黑棋的回合
    private float gridwidth, gridheight, gridmin;//单位宽度，单位高度，两者中的较小值
    private Vector2[,] chesspos;//棋盘上每个可下棋点的坐标
    private int[,] ischessexist;//无为0，黑为1，白为-1
    private int iswin;//无为0，黑为1，白为-1
    private List<Chess> blackchesses,whitechesses;//黑棋记录，白棋记录

    private int bestx = 0;
    private int besty = 0;
    private int bestscore = 0;

    void Start()
    {
        back.onClick.AddListener(Back);
        regame.onClick.AddListener(ReGame);

        chesspos = new Vector2[15, 15];
        ischessexist = new int[15, 15];
        isblackturn = true;
        iswin = 0;
        blackchesses = new List<Chess>();
        whitechesses = new List<Chess>();

        ltpos = camera.WorldToScreenPoint(lefttop.transform.position);
        rtpos = camera.WorldToScreenPoint(righttop.transform.position);
        lbpos = camera.WorldToScreenPoint(leftbottom.transform.position);
        rbpos = camera.WorldToScreenPoint(rightbottom.transform.position);

        gridwidth = (rtpos.x - ltpos.x) / 14;
        gridheight = (ltpos.y - lbpos.y) / 14;
        gridmin = gridwidth < gridheight ? gridwidth : gridheight;

        bestx = 0;
        besty = 0;
        bestscore = 0;

        for (int i = 0; i < 15; i++)
        {
            for (int j = 0; j < 15; j++)
            {
                chesspos[i, j] = new Vector2(lbpos.x + gridwidth * i, lbpos.y + gridheight * j);
                ischessexist[i,j] = 0;
            }
        }
    }


    void Update()
    {
        if (iswin==0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                position = Input.mousePosition;
                for (int i = 0; i < 15; i++)
                {
                    for (int j = 0; j < 15; j++)
                    {
                        if (Distance(position, chesspos[i, j]) < gridmin / 2)
                        {
                            if (ischessexist[i, j] == 0)
                            {
                                Vector3 realposition = camera.ScreenToWorldPoint(new Vector3(chesspos[i, j].x, chesspos[i, j].y, 10));
                                GameObject chess = isblackturn ? Instantiate(blackchessprefab, realposition, transform.rotation) : Instantiate(whitechessprefab, realposition, transform.rotation);
                                ischessexist[i, j] = isblackturn ? 1 : -1;
                                Chess newchess = isblackturn ? new Chess(i, j, 1) : new Chess(i, j, -1);
                                if (isblackturn)
                                {
                                    blackchesses.Add(newchess);
                                }
                                else
                                {
                                    newchess.Color = -1;
                                    whitechesses.Add(newchess);
                                }
                                iswin = IsWin(i,j);
                                if (iswin == 1)
                                {
                                    blackwin.gameObject.SetActive(true);
                                    regame.gameObject.SetActive(true);
                                }
                                else if(iswin == -1)
                                {
                                    whitewin.gameObject.SetActive(true);
                                    regame.gameObject.SetActive(true);
                                }
                                else
                                {
                                    isblackturn = !isblackturn;
                                    AIPutChess();
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    int IsWin(int x,int y)
    {
        int flag = isblackturn ? 1 : -1;
        for (int i = 0;i<11;i++)//横向是否有五子
        {
            if (ischessexist[i, y]==flag&& ischessexist[i + 1, y] == flag&& ischessexist[i + 2, y] == flag&& ischessexist[i + 3, y] == flag&& ischessexist[i + 4, y] == flag)
            {
                return flag;
            }
        }
        for (int j = 0; j < 11; j++)//纵向是否有五子
        {
            if (ischessexist[x, j] == flag && ischessexist[x, j + 1] == flag && ischessexist[x, j + 2] == flag && ischessexist[x, j + 3] == flag && ischessexist[x, j + 4] == flag)
            {
                return flag;
            }
        }
        if (x < y)//判断横坐标和纵坐标哪个小，就以它为顶点，判断两个斜方向是否有五子
        {
            for (int i = 0, j = y - x; j < 11; i++,j++)
            {
                if (ischessexist[i, j] == flag && ischessexist[i + 1, j + 1] == flag && ischessexist[i + 2, j + 2] == flag && ischessexist[i + 3, j + 3] == flag && ischessexist[i + 4, j + 4] == flag)
                {
                    return flag;
                }
            }
            for (int i = 0, j = y + x; i < 11 && j > 4; i++, j--)
            {
                if (j < 15)
                {
                    if (ischessexist[i, j] == flag && ischessexist[i + 1, j - 1] == flag && ischessexist[i + 2, j - 2] == flag && ischessexist[i + 3, j - 3] == flag && ischessexist[i + 4, j - 4] == flag)
                    {
                        return flag;
                    }
                }
            }
        }
        else
        {
            for (int i = x - y, j = 0; i < 11; i++, j++)
            {
                if (ischessexist[i, j] == flag && ischessexist[i + 1, j + 1] == flag && ischessexist[i + 2, j + 2] == flag && ischessexist[i + 3, j + 3] == flag && ischessexist[i + 4, j + 4] == flag)
                {
                    return flag;
                }
            }
            for (int i = x + y, j = 0; i > 4 && j < 11; i--, j++)
            {
                if (i < 15)
                {
                    if (ischessexist[i, j] == flag && ischessexist[i - 1, j + 1] == flag && ischessexist[i - 2, j + 2] == flag && ischessexist[i - 3, j + 3] == flag && ischessexist[i - 4, j + 4] == flag)
                    {
                        return flag;
                    }
                }
            }
        }
        return 0;
    }

    public void Back()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ReGame()
    {
        blackwin.gameObject.SetActive(false);
        whitewin.gameObject.SetActive(false);
        regame.gameObject.SetActive(false);
        iswin = 0;
        for (int i = 0; i < 15; i++)
        {
            for (int j = 0; j < 15; j++)
            {
                ischessexist[i, j] = 0;
            }
        }
        foreach (GameObject chess in GameObject.FindGameObjectsWithTag("chess"))
        {
            Destroy(chess);
        }
    }

    public float Distance(Vector3 mPos, Vector2 gridPos)
    {
        return Mathf.Sqrt(Mathf.Pow(mPos.x - gridPos.x, 2) + Mathf.Pow(mPos.y - gridPos.y, 2));
    }

    public void AIPutChess()
    {
        bestx = 7;
        besty = 7;
        bestscore = 0;
        for (int i = 0; i < blackchesses.ToArray().Length; i++)//遍历每一个黑方下过的棋子
        {
            int x = blackchesses[i].X;
            int y = blackchesses[i].Y;
            //判断每一个已下棋子的周围八格是否为空
            if (y < 14 && ischessexist[x, y + 1] == 0)//上
            {
                getScore(x, y + 1, 1);
            }
            if (x < 14 && y < 14 && ischessexist[x + 1, y + 1] == 0)//右上
            {
                getScore(x + 1, y + 1, 1);
            }
            if (x < 14 && ischessexist[x + 1, y] == 0)//右
            {
                getScore(x + 1, y, 1);
            }
            if (x < 14 && y > 0 && ischessexist[x + 1, y - 1] == 0)//右下
            {
                getScore(x + 1, y - 1, 1);
            }
            if (y > 0 && ischessexist[x, y - 1] == 0)//下
            {
                getScore(x, y - 1, 1);
            }
            if (x > 0 && y > 0 && ischessexist[x - 1, y - 1] == 0)//左下
            {
                getScore(x - 1, y - 1, 1);
            }
            if (x > 0 && ischessexist[x - 1, y] == 0)//左
            {
                getScore(x - 1, y, 1);
            }
            if (x > 0 && y < 14 && ischessexist[x - 1, y + 1] == 0)//左上
            {
                getScore(x - 1, y + 1, 1);
            }
        }
        for (int i=0;i<whitechesses.ToArray().Length;i++)//遍历白方每一个下过的棋子
        {
            int x = blackchesses[i].X;
            int y = blackchesses[i].Y;
            //判断每一个已下棋子的周围八格是否为空
            if (y<14 && ischessexist[x,y + 1] == 0)//上
            {
                getScore(x,y + 1,-1);
            }
            if (x < 14 && y < 14 && ischessexist[x + 1, y + 1] == 0)//右上
            {
                getScore(x + 1, y + 1, -1);
            }
            if (x < 14 && ischessexist[x + 1, y] == 0)//右
            {
                getScore(x + 1, y, -1);
            }
            if (x < 14 && y > 0 && ischessexist[x + 1, y - 1] == 0)//右下
            {
                getScore(x + 1, y - 1, -1);
            }
            if (y > 0 && ischessexist[x, y - 1] == 0)//下
            {
                getScore(x, y - 1, -1);
            }
            if (x > 0 && y > 0 && ischessexist[x - 1, y - 1] == 0)//左下
            {
                getScore(x - 1, y - 1, -1);
            }
            if (x > 0 && ischessexist[x - 1, y] == 0)//左
            {
                getScore(x - 1, y, -1);
            }
            if (x > 0 && y < 14 && ischessexist[x - 1, y + 1] == 0)//左上
            {
                getScore(x - 1, y + 1, -1);
            }
        }
        Vector3 realposition = camera.ScreenToWorldPoint(new Vector3(chesspos[bestx, besty].x, chesspos[bestx, besty].y, 10));
        GameObject chess = isblackturn ? Instantiate(blackchessprefab, realposition, transform.rotation) : Instantiate(whitechessprefab, realposition, transform.rotation);
        ischessexist[bestx, besty] = isblackturn ? 1 : -1;
        Chess newchess = isblackturn ? new Chess(bestx, besty, 1) : new Chess(bestx, besty, -1);
        if (isblackturn)
        {
            blackchesses.Add(newchess);
        }
        else
        {
            newchess.Color = -1;
            whitechesses.Add(newchess);
        }
        iswin = IsWin(bestx, besty);
        if (iswin == 1)
        {
            blackwin.gameObject.SetActive(true);
            regame.gameObject.SetActive(true);
        }
        else if (iswin == -1)
        {
            whitewin.gameObject.SetActive(true);
            regame.gameObject.SetActive(true);
        }
        else
        {
            isblackturn = !isblackturn;
            return;
        }
    }

    public void getScore(int x,int y,int flag)//判断该空位的行列斜上的棋子数
    {
        int score = 0;
        for (int m = x, n = y; n < 15; n++)//上
        {
            if (ischessexist[m, n] == flag)
            {
                score++;
            }
        }
        for (int m = x, n = y; m < 15 && n < 15; m++, n++)//右上
        {
            if (ischessexist[m, n] == flag)
            {
                score++;
            }
        }
        for (int m = x, n = y; m < 15; m++)//右
        {
            if (ischessexist[m, n] == flag)
            {
                score++;
            }
        }
        for (int m = x, n = y; m < 15 && n >= 0; m++, n--)//右下
        {
            if (ischessexist[m, n] == flag)
            {
                score++;
            }
        }
        for (int m = x, n = y; n >= 0; n--)//下
        {
            if (ischessexist[m, n] == flag)
            {
                score++;
            }
        }
        for (int m = x, n = y; m >= 0 && n >= 0; m--, n--)//左下
        {
            if (ischessexist[m, n] == flag)
            {
                score++;
            }
        }
        for (int m = x, n = y; m >= 0; m--)//左
        {
            if (ischessexist[m, n] == flag)
            {
                score++;
            }
        }
        for (int m = x, n = y; m >= 0 && n < 15; m--, n++)//左上
        {
            if (ischessexist[m, n] == flag)
            {
                score++;
            }
        }
        if (score > bestscore)
        {
            bestscore = score;
            bestx = x;
            besty = y;
        }
    }
}
