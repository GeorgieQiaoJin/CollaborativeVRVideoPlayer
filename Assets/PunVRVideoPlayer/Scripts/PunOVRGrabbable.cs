﻿using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

//
//Custom grabbable script which checks if the grabber "is mine" to actually grab
//

namespace Networking.Pun2
{
    [RequireComponent(typeof(PhotonView))]
    public class PunOVRGrabbable : OVRGrabbable
    {
        public UnityEvent onGrab;
        public UnityEvent onRelease;

        public bool GrabStatus;

        [SerializeField] bool hideHandOnGrab;
        PhotonView pv;
        Rigidbody _rb;

        protected override void Start()
        {
            base.Start();
            GrabStatus = false;
            pv = GetComponent<PhotonView>();
            _rb = gameObject.GetComponent<Rigidbody>();
        }

        public void Update()
        {
           // if()
        }

        public bool getGrabStatus()
        {
            return GrabStatus;
        }

        override public void GrabBegin(OVRGrabber hand, Collider grabPoint)
        {
            m_grabbedBy = hand;
            m_grabbedCollider = grabPoint;

            Debug.Log("GrabBegin");
           

            pv.TransferOwnership(PhotonNetwork.LocalPlayer);

            if (pv.IsMine)
            {
                GrabStatus = true;
                pv.RPC("SetKinematicTrue", RpcTarget.All); //changes the kinematic state of the object to all players when its grabbed
                if (onGrab != null)
                    onGrab.Invoke();
                if (hideHandOnGrab)
                    m_grabbedBy.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
            }
        }

        override public void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
        {
            
            //If the grabbed object is mine, release it
            if (pv.IsMine)
            {
                GrabStatus = false;
                _rb.isKinematic = m_grabbedKinematic;
                pv.RPC("SetKinematicFalse", RpcTarget.All);
                _rb.velocity = linearVelocity;
                _rb.angularVelocity = angularVelocity;
                if (hideHandOnGrab)
                    m_grabbedBy.GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
                m_grabbedBy = null;
                m_grabbedCollider = null;
                if (onRelease != null)
                    onRelease.Invoke();
            }
        }

        public Collider[] grabPoints
        {
            get { return m_grabPoints; }
            set { grabPoints = value; }
        }

        virtual public void CustomGrabCollider(Collider[] collider)
        {
            m_grabPoints = collider;
        }

        [Photon.Pun.PunRPC]
        public void SetKinematicTrue()
        {
            
            Debug.Log("SetKinematicTrue");

            gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }

        [Photon.Pun.PunRPC]
        public void SetKinematicFalse()
        {
            
            _rb.isKinematic = m_grabbedKinematic;
        }
    }
}

