/*
 * Name: RTS Unit Movement
 * Author: James 'Sevion' Nhan, Erik 'Siretu', and Aron Granberg
 * Date: 21/07/2013
 * Version: 1.1.0.0
 * Description:
 *		This is a movement system that handles moving
 *		normal units inside the battlecruiser. If the unit
 *		has a parent or if the parent variable is set, it will
 *		move "inside" that parent with relative position
 *		and rotation.
 */

using UnityEngine;
using System.Collections;

using Pathfinding;

public class UnitMovement : MonoBehaviour {

    // Will default to the gameObject's direct parent if not set.
    public GameObject Parent;

    private UnitOrder OrderManager;


    private UnitOrder.Order CurrentOrder = UnitOrder.Order.Stop;
    private GameObject TargetObject;
    private Vector3 TargetPosition;
    private Vector3 OriginalParentPos;
    private Vector3 OriginalRelativeTarget;
    private Quaternion OriginalParentRot;
    private const float MOVESPEED = 10.0f;

    private Seeker seeker;
    private CharacterController controller;

    //The calculated path
    public Path path;

    //The waypoint we are currently moving towards
    private int currentWaypoint = 0;

    // Use this for initialization
    void Start() {
        OrderManager = GetComponent<UnitOrder>();
        seeker = GetComponent<Seeker>();

        
        if (Parent == null && gameObject.transform.parent != null) {
            Parent = gameObject.transform.parent.gameObject;
        }
    }

    // Update is called once per frame
    void Update() {
        //This needs fixing, GetCurrentOrder does not stay at the same order for consecutive frames
        //if (OrderManager.GetCurrentOrder() == RTSUnitOrder.Order.Move) {

        if (path == null) {
            //We have no path to move after yet
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count) {
            path = null;
            return;
        }

        Vector3 currPath = path.vectorPath[currentWaypoint];

        if (Parent != null) {
            currPath = gameObject.transform.parent.transform.localToWorldMatrix.MultiplyPoint3x4(currPath);
        }

        //currPath = m.MultiplyPoint3x4(currPath);
        // Make sure the target doesn't cause the unit to "sink" in to the ground.
        currPath.y = gameObject.transform.position.y;

        // This is the vector towards the new position with respect to time.
        // "1" is the distance at which to start slowing down
        Vector3 NewPosition = Vector3.ClampMagnitude(currPath - transform.position, 1) * MOVESPEED * Time.deltaTime;
        gameObject.transform.position += NewPosition;

        // Go on to the next waypoint.
        //sqrMagnitude is faster than Vector3.Distance
        if ((transform.position - currPath).sqrMagnitude <= 2 * 2) {
            if (currentWaypoint == path.vectorPath.Count - 1) {

                // Clamp when it gets close enough.
                if (Vector3.Distance(gameObject.transform.position, currPath) <= MOVESPEED * Time.deltaTime) {
                    gameObject.transform.position = currPath;
                    currentWaypoint++;
                }
            } else {
                currentWaypoint++;
            }
        }
        //}	
    }

    public void OnPathComplete(Path p) {
        if (!p.error) {
            path = p;
            // Reset the waypoint counter
            currentWaypoint = 0;
            // Reset the order
            //RTSUnitOrder.OrderStruct order = new RTSUnitOrder.OrderStruct(RTSUnitOrder.Order.Stop, gameObject.transform.localPosition);
            //gameObject.SendMessage("IssueOrder", order);
        }
    }

    public void Move(Vector3 pos) {
        Vector3 myStartPosition = transform.position;
        Vector3 myTargetPosition = pos;
        if (Parent != null) {
            myStartPosition = gameObject.transform.parent.transform.worldToLocalMatrix.MultiplyPoint3x4(transform.position);
            myTargetPosition = gameObject.transform.parent.transform.worldToLocalMatrix.MultiplyPoint3x4(pos);
            Debug.Log(myStartPosition);
            Debug.Log(myTargetPosition);
        }
        seeker.StartPath(myStartPosition, myTargetPosition, OnPathComplete);
    }
}
