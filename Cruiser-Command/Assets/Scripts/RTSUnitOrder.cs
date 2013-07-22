/*
 * Name: RTS Unit Order
 * Author: James 'Sevion' Nhan and Erik 'Siretu'
 * Date: 21/07/2013
 * Version: 1.0.0.3
 * Description:
 *		This is a simple RTS unit order system that
 *		handles orders on mouse clicks.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Pathfinding;

public class RTSUnitOrder : MonoBehaviour {

    // The possible orders
    public enum Order {
        Move,
        Stop,
        // Still need to implement:
        HoldPosition,
        Patrol,
        Attack,
        Smart
    };

    // OrderStruct for issuing orders
    public struct OrderStruct {
        public Order order;
        public Vector3 target;

        public OrderStruct(Order order, Vector3 target) {
            this.order = order;
            this.target = target;
        }
    };

    private Order CurrentOrder = Order.Stop;
    private GameObject TargetObject;
    private Vector3 TargetPosition;
	private RTSUnitMovement MovementManager;
    
	void Start() {
		MovementManager = GetComponent<RTSUnitMovement>();
	}

    // Issue an order to the unit
    public void IssueOrder(OrderStruct order) {
        // If it's either of the non-motion orders, there shouldn't be a target
        if (order.order == Order.Stop || order.order == Order.HoldPosition) {
            CurrentOrder = order.order;
            TargetPosition = gameObject.transform.position;
            TargetObject = gameObject;
        } else {
            CurrentOrder = order.order;
            TargetPosition = order.target;
			
			MovementManager.Move(TargetPosition);
        }
    }

    public Order GetCurrentOrder() {
        return CurrentOrder;
    }

    public Vector3 GetOrderPosition() {
        return TargetPosition;
    }

    public GameObject GetOrderTarget() {
        return TargetObject;
    }
}