﻿//-----------------------------------------------------------------------
// <copyright file="FrameTracer.cs" company="Google">
//
// Copyright 2020 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

using System;
using UnityEngine;

namespace Google.Android.PerformanceTuner
{
    public class FrameTracer : MonoBehaviour
    {
        public Action onAppInBackground;
        public Action<LifecycleState> onLifecycleChanged;

        LifecycleState currentLifecycleState
        {
            set
            {
                if (onLifecycleChanged != null) onLifecycleChanged(value);
            }
        }

        void Awake()
        {
            currentLifecycleState = LifecycleState.OnCreate;
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (onAppInBackground != null && pauseStatus) onAppInBackground();
            currentLifecycleState = pauseStatus ? LifecycleState.OnStop : LifecycleState.OnStart;
        }

        void OnDestroy()
        {
            currentLifecycleState = LifecycleState.OnDestroy;
        }
    }
}