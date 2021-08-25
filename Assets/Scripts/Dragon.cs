using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dragon : MonoBehaviour
{
    public int ID;
    public bool isUp;
    public int col;
    public int row;
    private GridManager grid;
    private Vector2 tempPos;
    private float TargetY;
    private float TargetX;
    private Vector3 tempScale;

    //Animator 
   

    private void Awake()
    {
        isUp = false;
        grid = FindObjectOfType<GridManager>();
        TargetY = row * 2.1f;
    }
    private void Update()
    {
        if (grid.state == 1)
        {
            isUp = false;
        }
        StartCoroutine(changeScale());
        Move();
        gameObject.name = "(" + row + "," + col+")";
       /* if (transform.position.y > 5 * 2.1f)
        {
            this.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        }
        else this.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);*/
    }

    public IEnumerator changeScale()
    {
        if (isUp && this.transform.localScale.x < 0.45f)
        {
            Vector3 scale = this.transform.localScale;
            this.transform.localScale = new Vector3(scale.x + 1f * Time.deltaTime, scale.y + 1f * Time.deltaTime, 1);
        }
        else if (!isUp && this.transform.localScale.x > 0.3f)
        {
            Vector3 scale = this.transform.localScale;
            this.transform.localScale = new Vector3(scale.x - 1f * Time.deltaTime, scale.y - 1f * Time.deltaTime, 1);
        }
        yield return new WaitForSeconds(0.5f);
    }

    public void Trace ()
    {
        grid.canDestroy++;
        Dragon left = null;
        Dragon right = null;
        Dragon up = null;
        Dragon down = null;
        if (col + 1 < 5 && grid.dragonMatrix[col+1,row] != null)
        {
            right = grid.dragonMatrix[col + 1, row].GetComponent<Dragon>();
        }
        if (row + 1 < 5 && grid.dragonMatrix[col ,1 + row] != null)
        {
            up = grid.dragonMatrix[col , row+1].GetComponent<Dragon>();
        }
        if (col - 1 >= 0 && grid.dragonMatrix[col - 1, row] != null)
        {
            left =  grid.dragonMatrix[col - 1, row].GetComponent<Dragon>();
        }
        if (row - 1 >= 0 && grid.dragonMatrix[col , row-1] != null)
        {
            down = grid.dragonMatrix[col, row-1].GetComponent<Dragon>();
        }
        /*Debug.Log(left);
        Debug.Log(right);
        Debug.Log(up);
        Debug.Log(down);*/
        if (left != null && left.ID == this.ID  && !left.isUp)
        {
            //Debug.Log("(" + row + "," + col + ") left");
            left.isUp = true;
            left.Trace();
        }
        if (right != null && right.ID == this.ID && !right.isUp)
        {
            //Debug.Log("(" + row + "," + col + ") right");

            right.isUp = true;
            right.Trace();
        }
        if (up != null && up.ID == this.ID  && !up.isUp)
        {
            //Debug.Log("(" + row + "," + col + ") up");

            up.isUp = true;
            up.Trace();
        }
        if (down != null && down.ID == this.ID && !down.isUp)
        {
            //Debug.Log("(" + row + "," + col + ") down");

            down.isUp = true;
            down.Trace();
        }
    }

    private void Move()
    {
        TargetY = grid.distance2*row;
        TargetX = grid.distance1 * col;
        
        if (Mathf.Abs(TargetY-transform.position.x) > .1f)
        {
            tempPos = new Vector2(TargetX, TargetY);
            transform.position = Vector2.Lerp(transform.position, tempPos, 15f * Time.deltaTime);
        }
        else
        {
            tempPos = new Vector2(TargetX, TargetY);
            transform.position = tempPos;
        }
    }

    private void  DestroyHandle()
    {
        grid.positionDestroy =  new Vector2(this.transform.position.x ,this.transform.position.y+0.2f);
        grid.r = row;
        grid.c = col;
 
        GameObject ap = Instantiate(grid.dragons[ID], this.transform.position, Quaternion.identity);
        grid.maxInit = Mathf.Max(grid.maxInit, ID);
        ap.name = "(" + col + "," + row + ")";
        ap.GetComponent<Dragon>().col = col;
        ap.GetComponent<Dragon>().row = row;
        ap.transform.parent = grid.transform;
        grid.DestroyOnClick();
        grid.dragonMatrix[col, row] = ap;
        StartCoroutine(grid.DisminuirColumna());

    }

   

    private void OnMouseDown()
    {
        if (!GamePlay_UI.paused)
        {

            if (grid.state == 2)
            {

                if (isUp && grid.canDestroy > 1)
                {
                    DestroyHandle();
                    grid.score += (grid.canDestroy * 10);

                    // grid.canDestroy = 0;
                }
                grid.state = 1;

            }
            else if (grid.state == 1)
            {

                grid.state = 2;
                Trace();
                //StartCoroutine( checkIfcanUp());

                if (grid.canDestroy > 1f)
                    this.isUp = true;
                else grid.state = 1;
            }
        }
      
    }

    
}
