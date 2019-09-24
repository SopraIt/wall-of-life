using UnityEngine;
using Windows.Kinect;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;

public class KinectManager : MonoBehaviour
{
    public Camera myCam;
    private KinectSensor _sensor;
    private BodyFrameReader _bodyFrameReader;
    private Body[] _bodies = null;
    private float closestDistance = 0;
    public bool setupBody = false;
    public bool waveSegment1;
    //public bool waveSegment2;
    //public bool waveSegment3;
    public bool waveComplete;

    public bool IsAvailable;
    public string PlayerHand;
    public Windows.Kinect.HandState HandState;
    public bool HandClosed;
    public bool HandOpen;
    public JointType MainHand = (JointType)11;
    private JointType MainElbow = (JointType)9;


    private CameraSpacePoint ShakePositionStart;
    public bool HandAwaken = false;
    private int ShakeCounter;
    private int rapidMovementCounter;
    private int slowMovementCounter;
    //Reducing hand trembling
    public float TremblingThreshold;
    public float defaultTremblingThreshold = 0.15f;
    //Reducing Erratic Behaviour of Kinect
    public float AcceptableMovementArea;
    public float defaultAcceptableMovementArea = 3;
    //Responsiveness
    public float RapidCounterThreshold;
    public float defaultRapidCounterThreshold = 1;

    public bool checkWave;
    //private bool CheckShake = false;
    public int wavesToDo = 2;
    public int wavesComplete = 0;

    public CameraSpacePoint pos;
    public Vector2 HandPosition;
    public static KinectManager instance = null;

    private bool created = false;

    Body body = null;
    // index for the currently tracked body
    public int bodyIndex;
    // flag to asses if a body is currently tracked
    public bool bodyTracked = false;

    public int sensitivityX;
    public int sensitivityY;
    public int defaultSensitivityX = 25;
    public int defaultSensitivityY = 45;

    public Body[] GetBodies()
    {
        return _bodies;
    }

    public Body GetBody() {
        return body;
    }
    
    void Awake()
    {

        if (!created)
        {
            DontDestroyOnLoad(this.gameObject);
            created = true;
        }
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        sensitivityX = PlayerPrefs.GetInt("sensitivityX");
        if (sensitivityX == 0) {
            sensitivityX = defaultSensitivityX;
        }

        sensitivityY = PlayerPrefs.GetInt("sensitivityY");
        if (sensitivityY == 0)
        {
            sensitivityY = defaultSensitivityY;
        }

        var defaultOptions = new List<string>
        {
            "TremblingThreshold",
            "AcceptableMovementArea",
            "RapidCounterThreshold"
        };
        InitOptions(defaultOptions);

        _sensor = KinectSensor.GetDefault();
        if (_sensor != null)
        {
            IsAvailable = _sensor.IsAvailable;

            _bodyFrameReader = _sensor.BodyFrameSource.OpenReader();
            if (!_sensor.IsOpen)
            {
                _sensor.Open();
            }
            _bodies = new Body[_sensor.BodyFrameSource.BodyCount];
        }
    }

    void InitOptions(List<string> options) {
        foreach (string option in options)
        {
            float defaultOption = (float)typeof(KinectManager).GetField("default" + option).GetValue(instance);
            float playerOption = PlayerPrefs.GetFloat(option);
            FieldInfo optionField = typeof(KinectManager).GetField(option);
            if (playerOption == 0)
            {
                optionField.SetValue(instance, defaultOption);
            }
            else {
                optionField.SetValue(instance, playerOption);
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        UpdateMainHand();
    }

    // Update is called once per frame
    void Update()
    {
        if (myCam == null)
        {
            myCam = Camera.main;
        }
        if (_sensor != null)
        {
            IsAvailable = _sensor.IsAvailable;
        }
        if (_bodyFrameReader != null)
        {
            var frame = _bodyFrameReader.AcquireLatestFrame();

            if (frame != null)
            {
                frame.GetAndRefreshBodyData(_bodies);

                
                if (bodyTracked)
                {
                    if (_bodies[bodyIndex].IsTracked)
                    {
                        body = _bodies[bodyIndex];
                    }
                    else
                    {
                        bodyTracked = false;
                    }
                }
                if (setupBody || !bodyTracked)
                {
                    closestDistance = 0;

                    for (int i = 0; i < _bodies.Length; ++i)
                    {
                        float thisDistance = _bodies[i].Joints[JointType.Head].Position.Z;

                        if (closestDistance == 0 || thisDistance != 0 && thisDistance <= closestDistance)
                        {
                            closestDistance = thisDistance;
                            bodyIndex = i;
                        }
                    }

                    if (_bodies[bodyIndex].IsTracked)
                    {
                        bodyTracked = true;
                    }
                }

                if (body != null && bodyTracked && body.IsTracked)
                {
                    IsAvailable = true;

                    if (PlayerHand.Equals("HandRight"))
                    {
                        HandState = body.HandRightState;
                    }
                    else
                    {
                        HandState = body.HandLeftState;
                    }
                    HandClosed = (HandState == HandState.Closed);
                    HandOpen = (HandState == HandState.Open);

                    CameraSpacePoint newPos = body.Joints[MainHand].Position;
                    newPos.X = newPos.X * sensitivityX;
                    newPos.Y = newPos.Y * sensitivityY;
                    MoveHand(newPos);

                    if (!HandAwaken)
                    {
                        ShakeHand();
                    }

                    if (checkWave)
                    {
                        WaveSegments();
                    }


                }
            
                frame.Dispose();
                frame = null;
            }
            //foreach (var body in _bodies.Where(b => b.IsTracked))
            //  {
                
            //        //if (body.HandRightConfidence == TrackingConfidence.High && body.HandRightState == HandState.Lasso)
            //    }
        }

    }

    bool PointRangeCheck(CameraSpacePoint pointToCheck, CameraSpacePoint pointToCompare, float range) {
        return (TestRange(pointToCheck.X, pointToCompare.X, range) && TestRange(pointToCheck.Y, pointToCompare.Y, range));
    }

    bool TestRange(float numberToCheck, float numberToCompare, float range)
    {
        return (numberToCheck >= (numberToCompare - range) && numberToCheck <= (numberToCompare + range));
    }

    private void MoveHand(CameraSpacePoint newPos) {
        //If movement is too little and not continuous, do nothing (is involuntary)
        if (PointRangeCheck(newPos, pos, TremblingThreshold) && slowMovementCounter < 1)
        {
            slowMovementCounter++;
            //Debug.Log("Trembling");
        }
        //If movement is in a certain area, move the hand;
        //If it is outside, check the counter to understand if it is voluntary and not a glitch
        else if (PointRangeCheck(newPos, pos, AcceptableMovementArea)
             || (!PointRangeCheck(newPos, pos, AcceptableMovementArea) && (rapidMovementCounter > RapidCounterThreshold)))
        {
            pos = newPos;
            HandPosition = new Vector2(pos.X, pos.Y);
            rapidMovementCounter = 0;
            slowMovementCounter = 0;
        }
        else
        {
            rapidMovementCounter++;
        }
    }


    private void ShakeHand() {
        CameraSpacePoint ShakePositionNow = body.Joints[MainHand].Position;
        float nx = ShakePositionNow.X * sensitivityX;
        float ny = ShakePositionNow.Y * sensitivityY;

        if (!TestRange(nx, ShakePositionStart.X, 1) || !TestRange(ny, ShakePositionStart.Y, 1)) {
            ShakeCounter++;
            ShakePositionStart = pos;
        }
        if (ShakeCounter >= 3) {
            HandAwaken = true;
            //CheckShake = false;
            ShakeCounter = 0;
        }
    }

    public void AwakeCheckStart() {
        ShakeCounter = 0;
        HandAwaken = false;
        //CheckShake = true;
        ShakePositionStart = pos;
    }

    public void WaveSegments()
    {
        int idx = -1;
        for (int i = 0; i < _sensor.BodyFrameSource.BodyCount; i++)
        {
            if (_bodies[i].IsTracked)
            {
                idx = i;
            }
        }
        if (idx > -1)
        {
            // Hand above elbow
            if (HandAboveElbow(idx))
            {
                if (WaveLeft(idx))
                {
                    waveSegment1 = true;
                }
                else if (waveSegment1 && WaveRight(idx)) {
                    waveSegment1 = false;
                    wavesComplete++;
                }

                if (wavesComplete >= wavesToDo) {
                    waveComplete = true;
                }
            }
        }
    }

    bool HandAboveElbow(int bodyN) {
        return (_bodies[bodyN].Joints[MainHand].Position.Y >
                _bodies[bodyN].Joints[MainElbow].Position.Y);
    }
    bool WaveLeft(int bodyN)
    {
        return (_bodies[bodyN].Joints[MainHand].Position.X <
                _bodies[bodyN].Joints[MainElbow].Position.X);
    }
    bool WaveRight(int bodyN)
    {
        return (_bodies[bodyN].Joints[MainHand].Position.X >
                _bodies[bodyN].Joints[MainElbow].Position.X);
    }

    public void UpdateMainHand() {
        PlayerHand = PlayerPrefs.GetString("hand");

        if (PlayerHand == "HandRight")
        {
            MainHand = (JointType)11;
            MainElbow = (JointType)9;
        }
        else
        {
            MainHand = (JointType)7;
            MainElbow = (JointType)5;
        }
    }

    static float RescalingToRangesB(float scaleAStart, float scaleAEnd, float scaleBStart, float scaleBEnd, float valueA)
    {
        return (((valueA - scaleAStart) * (scaleBEnd - scaleBStart)) / (scaleAEnd - scaleAStart)) + scaleBStart;
    }

    void OnApplicationQuit()
    {
        if (_bodyFrameReader != null)
        {
            _bodyFrameReader.IsPaused = true;
            _bodyFrameReader.Dispose();
            _bodyFrameReader = null;
        }

        if (_sensor != null)
        {
            if (_sensor.IsOpen)
            {
                _sensor.Close();
            }
            _sensor = null;
        }
    }
}