using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappler : MonoBehaviour
{

    private Spawned mySpawn;
    private LineRenderer myLine;
    private Vector2 endPoint;
    private Collider2D hitTarget;
    private Vector2 startPoint;

    [Header("Variables for the projectile half of grapple")]
    [Tooltip("What layer you can hit to dash to")] public LayerMask grappledLayer;
    public GameObject GrappledPrefab;
    [Tooltip("What layer you can hit to dash to")] public LayerMask grabLayer;

    private float endlessWobbleTime; //Make rope shake while unconected without straightening
    private bool wobbled; //inverse every other wibble
    
    public AnimationClip pulledAnimation;



    public AnimationCurve[] possibleRopeStarts;
    [HideInInspector] public AnimationCurve RopeCurve = null;
    public int linePoints = 20;
    public float curveAmp = 1f;


    [Header("Variables for the hooked and pulling half of grapple")]
    public GameObject straightLinePrefab;
    public float straightenTime = 1f;
    private float currentTime;
    [Range(1,10)] public int wobbles = 1;
    [Range(0,1)] public float wobbleDampen = 0.5f;
    [HideInInspector] public float inheritWobbleStart;


    [HideInInspector] public float keepAngle;
    private Aim myAim;


    
    [Tooltip("X is min dash speed at min range and Y is max")] public Vector2 grappleDashSpeed;
    [Tooltip("Does not set a max distance rather, any grapple longer than this range will no longer increase dash speed past max")] public float maxEffectiveRange;
    [Tooltip("X is initial dash time where movement is locked, Y is velocity fall off time after")] public Vector2 grappleDashDuration;
    
    private bool grappled = false;
    [HideInInspector] public bool startToTighten = false;



    // Start is called before the first frame update
    void Awake()
    {
        if (!mySpawn) { mySpawn = GetComponentInParent<Spawned>(); }
        if (mySpawn) 
        { 
            if (mySpawn.myCore) 
            {
                if (mySpawn.myCore.myAim) { myAim = mySpawn.myCore.myAim; } 
            }
        }

        if(!myLine) { myLine = GetComponent<LineRenderer>(); }

    }

    void OnEnable()
    {
        currentTime = 0f;
        grappled = false;
        endlessWobbleTime = 0f;

        if (myLine) { myLine.positionCount = 0;}

        if(possibleRopeStarts.Length > 0) 
        { 
            int r = Random.Range(0, possibleRopeStarts.Length);
            RopeCurve = possibleRopeStarts[r];
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (myLine && mySpawn)
        {
            if (myAim) { myAim.attackAngleOffset = mySpawn.angle; }

            if (mySpawn.source) { startPoint = mySpawn.source.position; }
            else { startPoint = mySpawn.transform.position; }

            
            if (hitTarget) { endPoint = hitTarget.ClosestPoint(transform.position); }
            else { endPoint = transform.position; }

            UpdateLinePoints();            
        }
    }

    void UpdateLinePoints()
    {
        if (!startToTighten) //Wobble the rope but don't tighten it and trigger end timmer
        { endlessWobbleTime += Time.deltaTime; }
        else if (currentTime < straightenTime) //Time until rope fully straightens
        { currentTime += Time.deltaTime; }
        
        if (currentTime >= straightenTime)
        {
            currentTime = straightenTime;
            if (!grappled) { GrappleDash(); }

            if (straightLinePrefab)
            {
                var line = PoolManager.Instance.Spawn(straightLinePrefab, transform.position, transform.rotation, mySpawn.source).GetComponent<LineHandler>();
                if (line) { line.endPoint = endPoint; }
            }
            gameObject.SetActive(false);

        }
        else //do all this extra math while line is curvy
        {
            //myLine.positionCount = linePoints;

            Vector2 dir = (endPoint - startPoint).normalized;
            var perpendicular = new Vector2(-dir.y, dir.x);


            float cycleTime = straightenTime / wobbles;
            float calculatedTime = endlessWobbleTime;
            if (startToTighten) { calculatedTime = currentTime + inheritWobbleStart; }

            float currentCyclePercent = calculatedTime % cycleTime;
            int currentCycleCount = Mathf.FloorToInt(calculatedTime / cycleTime);


            float t = currentCyclePercent / cycleTime;
            if (currentCycleCount % 2 != 0) //expand wobble every other cycle instead of straightening
            { t = 1f - t; }

            for (int i = 0; i < linePoints; i ++)
            {
                float percent = (float)i / (linePoints +1); //+1 because divide by 0 is bad
                Vector2 straightPosition = Vector2.Lerp(startPoint, endPoint, percent);


                float curveMultiplier = RopeCurve.Evaluate(percent);
                float currentCurvature = 0f;

                if (startToTighten)
                {
                    int dampenCycle = Mathf.FloorToInt((currentCycleCount + 1) / 2);
                    curveMultiplier *= Mathf.Pow(-wobbleDampen, dampenCycle);
                    currentCurvature = Mathf.Lerp(curveMultiplier, 0, t);
                }
                else
                {
                    if (endlessWobbleTime > straightenTime) { endlessWobbleTime -= straightenTime; wobbled = !wobbled; }
                    if (wobbled) { curveMultiplier *= -1; }
                    currentCurvature = Mathf.Lerp(curveMultiplier, -curveMultiplier, endlessWobbleTime / straightenTime); 
                }


                Vector2 curveOffset = perpendicular * currentCurvature * curveAmp;
                Vector2 pointPosition = straightPosition + curveOffset;

                myLine.positionCount = linePoints;
                myLine.SetPosition(i, pointPosition);
            }
            myLine.SetPosition(linePoints -1, endPoint); //Make sure end point is snapped to right spot
        }
    
    }

    void GrappleDash()
    {
        grappled = true; 
        if (mySpawn.myCore) 
        {
            var dir = (endPoint - startPoint).normalized;
            var dist = Vector3.Distance(startPoint, endPoint);

            if (dist > maxEffectiveRange) { dist = maxEffectiveRange; }
            dir *= Mathf.Lerp(grappleDashSpeed.x, grappleDashSpeed.y, dist / maxEffectiveRange);

            //mySpawn.myCore.Stun(grappleDashDuration.x, pulledAnimation);
            mySpawn.myCore.myMove.DoDash(dir, grappleDashDuration, pulledAnimation, true, true);
            
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if ((grappledLayer.value & (1 << other.transform.gameObject.layer)) > 0) 
        {
            if (GrappledPrefab && mySpawn) //Probably important that this prefab doesn't have its own grapple prefab that then spawns infinitely
            {
                var newG = PoolManager.Instance.Spawn(GrappledPrefab, other.ClosestPoint(transform.position), transform.rotation, mySpawn.source);
                var G = newG.GetComponent<Grappler>();
                if (G)
                {
                    G.RopeCurve = RopeCurve;
                    G.startToTighten = true;
                    G.inheritWobbleStart = endlessWobbleTime;
                    G.hitTarget = other;
                    G.pulledAnimation = pulledAnimation;
                    //G.grappledLayer = mySpawn.source.gameObject.layer; //the new rope will now check for the source layer to collide with
                }

            }

            gameObject.SetActive(false);
        }
    }


}
