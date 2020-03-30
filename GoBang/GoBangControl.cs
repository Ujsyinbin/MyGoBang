using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GoBangControl : MonoBehaviour
{
    public GameObject lefttop,righttop,leftbottom,rightbottom;
    public Camera camera;
    private Vector3 ltpos, rtpos, lbpos, rbpos, position;

    public Button back;
    public GameObject blackchess, whitechess;
    public Image blackwin, whitewin;

    private bool isblackturn;
    private float gridwidth, gridheight, gridmin;
    private Vector2[,] chesspos;
    private int[,] ischessexist;//无为0，黑为1，白为-1
    private int iswin;//无为0，黑为1，白为-1

    void Start()
    {
        back.onClick.AddListener(Back);

        chesspos = new Vector2[15, 15];
        ischessexist = new int[15, 15];
        isblackturn = true;
        iswin = 0;

        ltpos = camera.WorldToScreenPoint(lefttop.transform.position);
        rtpos = camera.WorldToScreenPoint(righttop.transform.position);
        lbpos = camera.WorldToScreenPoint(leftbottom.transform.position);
        rbpos = camera.WorldToScreenPoint(rightbottom.transform.position);

        gridwidth = (rtpos.x - ltpos.x) / 14;
        gridheight = (ltpos.y - lbpos.y) / 14;
        gridmin = gridwidth < gridheight ? gridwidth : gridheight;

        for (int i = 0; i < 15; i++)
        {
            for (int j = 0; j < 15; j++)
            {
                chesspos[i, j] = new Vector2(lbpos.x + gridwidth * i, lbpos.y + gridheight * j);
            }
        }
        for (int i = 0; i < 15; i++)
        {
            for (int j = 0; j < 15; j++)
            {
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
                                GameObject chess = isblackturn ? Instantiate(blackchess, realposition, transform.rotation) : Instantiate(whitechess, realposition, transform.rotation);
                                ischessexist[i, j] = isblackturn ? 1 : -1;
                                iswin = IsWin(i,j);
                                if (iswin == 1)
                                {
                                    blackwin.gameObject.SetActive(true);
                                }
                                else if(iswin == -1)
                                {
                                    whitewin.gameObject.SetActive(true);
                                }
                                else
                                {
                                    isblackturn = !isblackturn;
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
        for (int i = 0;i<11;i++)
        {
            if (ischessexist[i, y]==flag&& ischessexist[i + 1, y] == flag&& ischessexist[i + 2, y] == flag&& ischessexist[i + 3, y] == flag&& ischessexist[i + 4, y] == flag)
            {
                return flag;
            }
        }
        for (int j = 0; j < 11; j++)
        {
            if (ischessexist[x, j] == flag && ischessexist[x, j + 1] == flag && ischessexist[x, j + 2] == flag && ischessexist[x, j + 3] == flag && ischessexist[x, j + 4] == flag)
            {
                return flag;
            }
        }
        if (x < y)
        {
            for (int i = 0, j = y - x; j < 11; i++,j++)
            {
                if (ischessexist[i, j] == flag && ischessexist[i + 1, j + 1] == flag && ischessexist[i + 2, j + 2] == flag && ischessexist[i + 3, j + 3] == flag && ischessexist[i + 4, j + 4] == flag)
                {
                    return flag;
                }
            }
            for (int i = 0, j = y + x; i < 15 && j > 4; i++, j--)
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
            for (int i = x + y, j = 0; i > 4 && j < 15; i--, j++)
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

    float Distance(Vector3 mPos, Vector2 gridPos)
    {
        return Mathf.Sqrt(Mathf.Pow(mPos.x - gridPos.x, 2) + Mathf.Pow(mPos.y - gridPos.y, 2));
    }
}
