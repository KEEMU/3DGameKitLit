using UnityEngine;
using System.Collections;
using System;


    public class SphereRotation : MonoBehaviour
    {
        [HideInInspector] public bool destroyIt;

        public GameObject Mount;
        public GameObject Swivel;

        private Vector3 defaultDir;
        private Quaternion defaultRot;

        private Transform headTransform;
        private Transform barrelTransform;

        public float HeadingTrackingSpeed = 2f;
        public float ElevationTrackingSpeed = 2f;

        private Vector3 targetPos;
        [HideInInspector] public Vector3 headingVetor;

        private float curHeadingAngle;
        private float curElevationAngle;

        public Vector2 HeadingLimit;
        public Vector2 ElevationLimit;

        public bool smoothControlling;

        public bool DebugDraw;

        public Transform DebugTarget;

        private bool fullAccess;
        public Animator[] Animators;



        private void Awake()
        {
            headTransform = Swivel.GetComponent<Transform>();
            barrelTransform = Mount.GetComponent<Transform>();
        }

        // Use this for initialization
        private void Start()
        {
            targetPos = headTransform.transform.position + headTransform.transform.forward * 100f;
            defaultDir = Swivel.transform.forward;
            defaultRot = Quaternion.FromToRotation(transform.forward, defaultDir);
            if (HeadingLimit.y - HeadingLimit.x >= 359.9f)
                fullAccess = true;
        }

        // Autotrack
        public void SetNewTarget(Vector3 _targetPos)
        {
            targetPos = _targetPos;
        }

        // Angle between mount and target
        public float GetAngleToTarget()
        {
            return Vector3.Angle(Mount.transform.forward, targetPos - Mount.transform.position);
        }

        private void Update()
        {
            CheckForTurn();
            if (DebugTarget != null)
                targetPos = DebugTarget.transform.position;

            if (!smoothControlling)
            {
                if (barrelTransform != null)
                {
                    /////// Heading
                    headingVetor =
                        Vector3.Normalize(ProjectVectorOnPlane(headTransform.up,
                            targetPos - headTransform.position));
                    float headingAngle =
                        SignedVectorAngle(headTransform.forward, headingVetor, headTransform.up);
                    float turretDefaultToTargetAngle = SignedVectorAngle(defaultRot * headTransform.forward,
                        headingVetor, headTransform.up);
                    float turretHeading = SignedVectorAngle(defaultRot * headTransform.forward,
                        headTransform.forward, headTransform.up);

                    float headingStep = HeadingTrackingSpeed * Time.deltaTime;

                    // Heading step and correction
                    // Full rotation
                    if (HeadingLimit.x <= -180f && HeadingLimit.y >= 180f)
                        headingStep *= Mathf.Sign(headingAngle);
                    else // Limited rotation
                        headingStep *= Mathf.Sign(turretDefaultToTargetAngle - turretHeading);

                    // Hard stop on reach no overshooting
                    if (Mathf.Abs(headingStep) > Mathf.Abs(headingAngle))
                        headingStep = headingAngle;

                    // Heading limits
                    if (curHeadingAngle + headingStep > HeadingLimit.x &&
                        curHeadingAngle + headingStep < HeadingLimit.y ||
                        HeadingLimit.x <= -180f && HeadingLimit.y >= 180f || fullAccess)
                    {
                        curHeadingAngle += headingStep;
                        headTransform.rotation = headTransform.rotation * Quaternion.Euler(0f, headingStep, 0f);
                    }

                    /////// Elevation
                    Vector3 elevationVector =
                        Vector3.Normalize(ProjectVectorOnPlane(headTransform.right,
                            targetPos - barrelTransform.position));
                    float elevationAngle =
                        SignedVectorAngle(barrelTransform.forward, elevationVector, headTransform.right);

                    // Elevation step and correction
                    float elevationStep = Mathf.Sign(elevationAngle) * ElevationTrackingSpeed * Time.deltaTime;
                    if (Mathf.Abs(elevationStep) > Mathf.Abs(elevationAngle))
                        elevationStep = elevationAngle;

                    // Elevation limits
                    if (curElevationAngle + elevationStep < ElevationLimit.y &&
                        curElevationAngle + elevationStep > ElevationLimit.x)
                    {
                        curElevationAngle += elevationStep;
                        barrelTransform.rotation = barrelTransform.rotation * Quaternion.Euler(elevationStep, 0f, 0f);
                    }
                }
            }
            else
            {
                Transform barrelX = barrelTransform;
                Transform barrelY = Swivel.transform;

                //finding position for turning just for X axis (down-up)
                Vector3 targetX = targetPos - barrelX.transform.position;
                Quaternion targetRotationX = Quaternion.LookRotation(targetX, headTransform.up);

                barrelX.transform.rotation = Quaternion.Slerp(barrelX.transform.rotation, targetRotationX,
                    HeadingTrackingSpeed * Time.deltaTime);
                barrelX.transform.localEulerAngles = new Vector3(barrelX.transform.localEulerAngles.x, 0f, 0f);

                //checking for turning up too much
                if (barrelX.transform.localEulerAngles.x >= 180f &&
                    barrelX.transform.localEulerAngles.x < (360f - ElevationLimit.y))
                {
                    barrelX.transform.localEulerAngles = new Vector3(360f - ElevationLimit.y, 0f, 0f);
                }

                //down
                else if (barrelX.transform.localEulerAngles.x < 180f &&
                         barrelX.transform.localEulerAngles.x > -ElevationLimit.x)
                {
                    barrelX.transform.localEulerAngles = new Vector3(-ElevationLimit.x, 0f, 0f);
                }

                //finding position for turning just for Y axis
                Vector3 targetY = targetPos;
                targetY.y = barrelY.position.y;

                Quaternion targetRotationY = Quaternion.LookRotation(targetY - barrelY.position, barrelY.transform.up);

                barrelY.transform.rotation = Quaternion.Slerp(barrelY.transform.rotation, targetRotationY,
                    ElevationTrackingSpeed * Time.deltaTime);
                barrelY.transform.localEulerAngles = new Vector3(0f, barrelY.transform.localEulerAngles.y, 0f);

                if (!fullAccess)
                {
                    //checking for turning left
                    if (barrelY.transform.localEulerAngles.y >= 180f &&
                        barrelY.transform.localEulerAngles.y < (360f - HeadingLimit.y))
                    {
                        barrelY.transform.localEulerAngles = new Vector3(0f, 360f - HeadingLimit.y, 0f);
                    }

                    //right
                    else if (barrelY.transform.localEulerAngles.y < 180f &&
                             barrelY.transform.localEulerAngles.y > -HeadingLimit.x)
                    {
                        barrelY.transform.localEulerAngles = new Vector3(0f, -HeadingLimit.x, 0f);
                    }
                }
            }

            if (DebugDraw)
                Debug.DrawLine(barrelTransform.position,
                    barrelTransform.position +
                    barrelTransform.forward * Vector3.Distance(barrelTransform.position, targetPos), Color.red);
        }
    //Projects a vector onto a plane. The output is not normalized.
    public static Vector3 ProjectVectorOnPlane(Vector3 planeNormal, Vector3 vector)
    {
        return vector - (Vector3.Dot(vector, planeNormal) * planeNormal);
    }

    public static float SignedVectorAngle(Vector3 referenceVector, Vector3 otherVector, Vector3 normal)
    {
        Vector3 perpVector;
        float angle;

        //Use the geometry object normal and one of the input vectors to calculate the perpendicular vector
        perpVector = Vector3.Cross(normal, referenceVector);

        //Now calculate the dot product between the perpendicular vector (perpVector) and the other input vector
        angle = Vector3.Angle(referenceVector, otherVector);
        angle *= Mathf.Sign(Vector3.Dot(perpVector, otherVector));

        return angle;
    }
    RaycastHit hitInfo; // Raycast structure

    void CheckForTurn()
    {
        // Construct a ray pointing from screen mouse position into world space
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Raycast
        if (Physics.Raycast(cameraRay, out hitInfo, 500f))
        {
            SetNewTarget(hitInfo.point);
            Debug.Log(hitInfo.point);
        }
    }
}
