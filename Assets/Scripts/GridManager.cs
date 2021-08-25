using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    public GameObject tile1;
    public GameObject tile2;
    private GameObject[,] tile;
    public GameObject[] dragons;
    public float distance1;
    public float distance2;
    
    public GameObject[,] dragonMatrix;

    // 
    public int state;
    public int canDestroy;
    public bool gameIsOver;

    public int maxInit = 4;

    public int score;


    // UI
    public Animator ScoretextAnim;
    public Text scoreaAnimText;

    //GDX 
    public GameObject BroomAnimation;
    public Vector2 positionDestroy;
    public GameObject smokeAnimation;
    public int r, c;

    //Audio 
    public AudioSource audio;

    private void Start()
    {
        tile = new GameObject[5, 5];
        dragonMatrix = new GameObject[5, 5];
        distance1 = tile1.GetComponent<SpriteRenderer>().bounds.size.x-0.1f;
        distance2 = tile1.GetComponent<SpriteRenderer>().bounds.size.y-0.4f;
        Debug.Log(distance1 + " ***" + distance2);
        state = 1;
        setup();
        canDestroy = 0;
        gameIsOver = false;
        score = 0;
    }
    private void Update()
    {
        if (state == 1)
            canDestroy = 0;
        PlayerPrefs.SetInt("highscore", Mathf.Max(PlayerPrefs.GetInt("highscore"),score));
        
    }

    private void setup ()
    {
        GameObject go;
        for (int i = 0; i < 5 ; i++)
        {
            for (int j = 0; j  < 5 ; j++)
            {
                if ((i + j) % 2 == 1)
                    go = Instantiate(tile1, new Vector2(i * distance1, j * distance2), Quaternion.identity) as GameObject;
                else
                    go = Instantiate(tile2, new Vector2(i * distance1, j * distance2), Quaternion.identity) as GameObject;
                go.GetComponent<SpriteRenderer>().sortingOrder = 4-j;
                go.transform.parent = this.transform;
                go.name = "(" + i + "," + j + ")";
                //
                int idRandom = Random.Range(0, 4);
                GameObject ob = Instantiate(dragons[idRandom], go.transform.position, Quaternion.identity);
                ob.GetComponent<Dragon>().col = i; 
                ob.GetComponent<Dragon>().row = j; 
                ob.transform.parent = this.transform;
                ob.name = "(" + i + "," + j + ")";
                dragonMatrix[i, j] = ob.gameObject;
                //
                GameObject gp = Instantiate(smokeAnimation, go.transform.position, Quaternion.identity);
                //go.transform.parent = this.transform;
                tile[i,j] = gp ;
                gp.GetComponent<Animator>().SetTrigger("Trigger");
          
            }
        }
        
    }

    public void DestroyOnClick()
    {
        scoreaAnimText.text = "x " + canDestroy.ToString();
        ScoretextAnim.SetTrigger("PlayScore");
        
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                if (dragonMatrix[i, j] != null && dragonMatrix[i, j].GetComponent<Dragon>().isUp)
                {
                    Destroy(dragonMatrix[i, j]);
                    if (r != j || c != i)
                    {

                        tile[i, j].GetComponent<Animator>().SetTrigger("Trigger");
                        
                    }
                    dragonMatrix[i, j] = null;
                }
            }

        }
    }

    public IEnumerator DisminuirColumna()
    {
                
        for (int i = 0; i < 5; i++)
        {
            for (int j = 1; j < 5; j++)
            {
                if (dragonMatrix[i, j] == null) continue;
                GameObject dra = dragonMatrix[i, j];
                int p = dra.GetComponent<Dragon>().row;
                while ( true )
                {
                    
                    p = dra.GetComponent<Dragon>().row;
                    if ( p-1 >=0 &&  dragonMatrix[i, p - 1] == null)
                    {
                        dra.GetComponent<Dragon>().row--;
                        dragonMatrix[i, p - 1] = dra;
                        dragonMatrix[i, p] = null;
                    }
                    else break;
                }
                if ( j == r && i == c)
                {

                    StartCoroutine(animationPlay(dra,p ));
                }
                
            }
        }
        audio.Play();
        StartCoroutine(AgregarDragon());
        yield return new WaitForSeconds(.5f);
        
    }

    

    public IEnumerator animationPlay(GameObject dra ,int p )
    {
        yield return new WaitForSeconds(0.25f);
        BroomAnimation.transform.position = new Vector2(dra.transform.position.x, p * 2.2f);
        
        BroomAnimation.GetComponent<Animator>().SetTrigger("Trigger");
    }

    public IEnumerator AgregarDragon()
    {
       
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                if (dragonMatrix[i, j] == null)
                {
                    int p = (int)Random.Range(0, maxInit+1);
                    Vector2 pos = new Vector2(i * 2.2f, (5 + j) * 2.1f);
                    GameObject ap = Instantiate(dragons[p], pos, Quaternion.identity);
                    ap.GetComponent<Dragon>().row = j;
                    ap.GetComponent<Dragon>().col = i;
                    ap.transform.parent = transform;
                    dragonMatrix[i, j] = ap;
                }
            }
        }
        checkGameOver();
        yield return new WaitForSeconds(.5f);
    }



    public IEnumerator waitSecondFinish()
    {
        yield return new WaitForSeconds(4f);
        
        gameIsOver = true;
    }
    
    public void checkGameOver ()
    {
        for (int i = 0; i < 5; i++)
        {
            for(int j = 0; j < 5; j++)
            {

                Dragon dra = dragonMatrix[i, j].GetComponent<Dragon>();
                Dragon left = null;
                Dragon right = null;
                Dragon up = null;
                Dragon down = null;
                if (i + 1 < 5 && dragonMatrix[i + 1, j] != null)
                {
                    right = dragonMatrix[i + 1, j].GetComponent<Dragon>();
                }
                if (j + 1 < 5 && dragonMatrix[i, 1 + j] != null)
                {
                    up = dragonMatrix[i, j + 1].GetComponent<Dragon>();
                }
                if (i - 1 >= 0 && dragonMatrix[i - 1, j] != null)
                {
                    left = dragonMatrix[i - 1, j].GetComponent<Dragon>();
                }
                if (j - 1 >= 0 && dragonMatrix[i, j - 1] != null)
                {
                    down = dragonMatrix[i, j - 1].GetComponent<Dragon>();
                }
               
                if (left != null && left.ID == dra.ID && !left.isUp)
                {
                    gameIsOver = false;
                    return;
                  
                }
                if (right != null && right.ID == dra.ID && !right.isUp)
                {
                    gameIsOver = false;
                    return;
                }
                if (up != null && up.ID == dra.ID && !up.isUp)
                {
                    gameIsOver = false;
                    return;
                }
                if (down != null && down.ID == dra.ID && !down.isUp)
                {
                    gameIsOver = false;
                    return;

                }
            }
        }
        StartCoroutine(waitSecondFinish());
        
    }

}
