using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
public class GameController : MonoBehaviour
{
    private CubePos nowCube = new CubePos(0, 1, 0), pastCube;
    private bool firstInput = true;
    private Coroutine showCubePlace;
    private Rigidbody allCubesRB;
    private float camMoveToYPosition, angelToRotate, camMoveToZPosition, speed = 2f, mHue, mSaturation, mValue;
    private Transform mainCam;
    private int totalMaxHor;
    public bool IsLose = false;
    private int maxX = 0, maxY = 0, maxZ = 0, maxHor;
    private Color colorToChange;
    private Transform maxCube;

    public float cubeChangePlaceSpeed = 0.5f, LoseRotate=0.3f;
    public Transform cubeToPlace;
    public GameObject cubeToCreate, allCubes, placePar;
    public GameObject restartButton;
    public GameObject[] canvasStartPage;
    public Transform rotator;
    
    private List<Vector3> allCubesPositions = new List<Vector3>{
        new Vector3(0,0,0),
        new Vector3(1,0,0),
        new Vector3(-1,0,0),
        new Vector3(0,1,0),
        new Vector3(0,0,1),
        new Vector3(0,0,-1),
        new Vector3(1,0,1),
        new Vector3(1,0,-1),
        new Vector3(-1,0,1),
        new Vector3(-1,0,-1),
        };
    private void Start()
    {      
        colorToChange = Camera.main.backgroundColor;
        camMoveToYPosition = nowCube.y - 1f;
        mainCam = Camera.main.transform;
        camMoveToZPosition = mainCam.localPosition.z;
        Color.RGBToHSV(Camera.main.backgroundColor,out mHue, out mSaturation, out mValue);
        
        allCubesRB = allCubes.GetComponent<Rigidbody>();
        showCubePlace = StartCoroutine(ShowCubePlace());    
    }
    private void Update()
    {
        if((Input.GetMouseButtonDown(0)|| Input.touchCount>0) && cubeToPlace != null && allCubes != null && !EventSystem.current.IsPointerOverGameObject())
        {
#if !UNITY_EDITOR
            if (Input.GetTouch(0).phase != TouchPhase.Began)
                return;
#endif
          
            if(firstInput==true) //Remove the menu after the first click
            {
                firstInput = false;
                foreach(GameObject obj in canvasStartPage)
                {
                    Destroy(obj);
                }
            }

            GameObject newCube = Instantiate(cubeToCreate, cubeToPlace.position, Quaternion.identity) as GameObject;
            newCube.transform.SetParent(allCubes.transform);
            pastCube.setVector(nowCube.getVector());
            nowCube.setVector(cubeToPlace.position);
            allCubesPositions.Add(nowCube.getVector());

            GameObject newVfx = Instantiate(placePar, newCube.transform.position, Quaternion.identity) as GameObject;
            Destroy(newVfx, placePar.GetComponent<ParticleSystem>().main.startLifetime.constantMax);

            allCubesRB.isKinematic = true;
            allCubesRB.isKinematic = false;

            SpawnPositions();
            MoveCameraChangeBG();
        }
        if(!IsLose && allCubesRB.velocity.magnitude > 0.04f)
        {
            Destroy(cubeToPlace.gameObject);
            IsLose = true;
            StopCoroutine(showCubePlace);
            restartButton.SetActive(true);
           // camMoveToZPosition = mainCam.localPosition.z - 3f;


            maxCube = allCubes.transform.GetChild(allCubes.transform.childCount-1);
            //rotator.SetParent(maxCube);
            

           // camMoveToYPosition = maxY + 4;
            angelToRotate = Mathf.Atan((maxY + 4) /Mathf.Abs(mainCam.localPosition.z)) * 40f;
            //Debug.Log(angelToRotate);
        } 
        
        if (!IsLose)
        {
            rotator.localPosition = Vector3.MoveTowards(rotator.localPosition,
                new Vector3(rotator.localPosition.x, camMoveToYPosition, rotator.localPosition.z), speed * Time.deltaTime);

            mainCam.localPosition = Vector3.MoveTowards(mainCam.localPosition,
                new Vector3(mainCam.localPosition.x, mainCam.localPosition.y, camMoveToZPosition), speed * Time.deltaTime);
        }
        if(Camera.main.backgroundColor != colorToChange)
        Camera.main.backgroundColor=Color.Lerp(Camera.main.backgroundColor, colorToChange, 2f*Time.deltaTime);
        
       
    }

    private void FixedUpdate()
    {
        if (IsLose)
        {
            rotator.position = Vector3.MoveTowards(rotator.position, maxCube.position, 16f * Time.deltaTime);
            mainCam.transform.LookAt(maxCube);


            /* mainCam.localPosition = Vector3.MoveTowards(mainCam.localPosition,
                 new Vector3(mainCam.localPosition.x, mainCam.localPosition.y, camMoveToZPosition), speed * Time.deltaTime);

             mainCam.localEulerAngles = Vector3.Lerp(mainCam.localEulerAngles,
             new Vector3(angelToRotate, mainCam.localEulerAngles.y, mainCam.localEulerAngles.z),
             6f * Time.deltaTime);*/


        }
    }
    IEnumerator ShowCubePlace()
    {
        while(true)
        {
            SpawnPositions();

            yield return new WaitForSeconds(cubeChangePlaceSpeed);
        }
    }
    private void SpawnPositions()
    {
        List<Vector3> positions = new List<Vector3>();
        if(IsPositionEmpty(new Vector3(nowCube.x + 1, nowCube.y, nowCube.z)) && nowCube.x + 1 != cubeToPlace.position.x)
            positions.Add(new Vector3(nowCube.x + 1, nowCube.y, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x - 1, nowCube.y, nowCube.z)) && nowCube.x - 1 != cubeToPlace.position.x)
            positions.Add(new Vector3(nowCube.x - 1, nowCube.y, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y + 1, nowCube.z)) && nowCube.y + 1 != cubeToPlace.position.y)
            positions.Add(new Vector3(nowCube.x, nowCube.y + 1, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y - 1, nowCube.z)) && nowCube.y - 1 != cubeToPlace.position.y)
            positions.Add(new Vector3(nowCube.x, nowCube.y - 1, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y, nowCube.z + 1)) && nowCube.z + 1 != cubeToPlace.position.z)
            positions.Add(new Vector3(nowCube.x, nowCube.y, nowCube.z + 1));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y, nowCube.z - 1)) && nowCube.z - 1 != cubeToPlace.position.z)
            positions.Add(new Vector3(nowCube.x, nowCube.y, nowCube.z - 1));

        if (positions.Count > 1)
            cubeToPlace.position = positions[UnityEngine.Random.Range(0, positions.Count)];
        else if (positions.Count == 0)
            IsLose = true;
        else
            cubeToPlace.position = positions[0];

    }
    private bool IsPositionEmpty(Vector3 targetPos)
    {
        if(targetPos.y==0)
            return false;

        foreach(Vector3 pos in allCubesPositions)
        {
            if (pos.x==targetPos.x &&pos.y==targetPos.y && pos.z==targetPos.z)
                return false;
        }
        return true;
    }
    
    private void MoveCameraChangeBG()
    {
       

        foreach (Vector3 pos in allCubesPositions)
        {
            if (Mathf.Abs(Convert.ToInt32(pos.x)) > maxX)
                maxX = Mathf.Abs(Convert.ToInt32(pos.x));
            if (Convert.ToInt32(pos.y) > maxY)
                maxY = Convert.ToInt32(pos.y);
            if (Mathf.Abs(Convert.ToInt32(pos.z)) > maxZ)
                maxZ = Mathf.Abs(Convert.ToInt32(pos.z));
        }
           
            camMoveToYPosition = nowCube.y-1f;
        maxHor = maxX >= maxZ ? maxX : maxZ;

        if(maxHor % 2==0 && maxHor!=totalMaxHor)
        {
            //mainCam.localPosition -= new Vector3(0, 0, 2.5f);
            camMoveToZPosition = mainCam.localPosition.z - 2.5f;
            totalMaxHor = maxHor;
        }

        if (nowCube.y == pastCube.y) return;
        if (nowCube.y > pastCube.y)  mHue += 0.1f; else mHue -= 0.1f;
        if (mHue >= 1f) mHue -= 1f;
        if (mHue <= 0) mHue += 1f;
        colorToChange = Color.HSVToRGB(mHue, mSaturation, mValue);
    }
}


struct CubePos
{
    public int x, y, z;
    public CubePos(int x,int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    public Vector3 getVector()
    {
        return new Vector3(x, y, z);
    }
    public void setVector(Vector3 pos)
    {
        x = Convert.ToInt32(pos.x);
        y = Convert.ToInt32(pos.y);
        z = Convert.ToInt32(pos.z);
    }
}
