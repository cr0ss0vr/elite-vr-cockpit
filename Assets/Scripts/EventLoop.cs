﻿using System;
using UnityEngine;
using Valve.VR;

namespace EVRC
{
    public class EventLoop : MonoBehaviour
    {
        public TrackedDevicePose_t[] poses = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];
        public TrackedDevicePose_t[] gamePoses = new TrackedDevicePose_t[0];

        void Update()
        {
            var vr = OpenVR.System;
            if (vr == null) return;

            VREvent_t ev = new VREvent_t();
            var size = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(VREvent_t));
            for (int limit = 64; limit > 0; --limit)
            {
                if (!vr.PollNextEvent(ref ev, size)) break;
                var type = (EVREventType)ev.eventType;
                SteamVR_Events.System(type).Send(ev);

                if (type == EVREventType.VREvent_Quit)
                {
                    enabled = false;
                    break;
                }
            }
        }

        void LateUpdate()
        {
            var compositor = OpenVR.Compositor;
            if (compositor == null) return;

            compositor.GetLastPoses(poses, gamePoses);
            SteamVR_Events.NewPoses.Send(poses);
            SteamVR_Events.NewPosesApplied.Send();
        }
    }
}
