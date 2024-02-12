﻿//-----------------------------------------------------------------------
// <copyright file="SettingsEditor.cs" company="Google">
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

using UnityEditor;
using UnityEngine;

namespace Google.Android.PerformanceTuner.Editor
{
    public class SettingsEditor
    {
        readonly ProjectData m_ProjectData;
        readonly SetupConfig m_SetupConfig;

        static readonly string k_InfoText =
            "Settings are auto-saved into " + Paths.androidAssetsPathName + "/tuningfork_settings.bin.\n" +
            "To get better frame rate in your game turn on frame pacing optimization in" +
            " Player->Resolution and Presentation->Optimized Frame Pacing.";


        public SettingsEditor(ProjectData projectData, SetupConfig setupConfig)
        {
            m_ProjectData = projectData;
            m_SetupConfig = setupConfig;
        }

        public void OnGUI()
        {
            LoadStyles();
            RenderPluginStatus();
            GUILayout.Space(10);
            GUILayout.Label(k_InfoText, Styles.info);
            GUILayout.Space(10);
            m_ProjectData.apiKey = EditorGUILayout.TextField("API key", m_ProjectData.apiKey);
        }

        StatusContent m_PluginStatus;
        StatusContent m_ApiStatus;
        StatusContent m_FidelityModeStatus;
        StatusContent m_AnnotationModeStatus;
        StatusContent m_LoadingAnnotationStatus;
        GUIContent m_NoFidelityMessages;

        void LoadStyles()
        {
            Texture textureDone = (Texture) Resources.Load("ic_done");
            Texture textureError = (Texture) Resources.Load("ic_error_outline");

            m_PluginStatus = new StatusContent(
                new GUIContent("Android Performance Tuner is enabled", textureDone),
                new GUIContent("Android Performance Tuner is disabled", textureError));

            m_ApiStatus = new StatusContent(
                new GUIContent("API key is not set", textureError),
                new GUIContent("API key is set", textureDone));

            m_FidelityModeStatus = new StatusContent(
                new GUIContent("Using custom fidelity", textureDone),
                new GUIContent("Using default fidelity", textureDone));

            m_AnnotationModeStatus = new StatusContent(
                new GUIContent("Using custom annotation", textureDone),
                new GUIContent("Using default annotation", textureDone));

            m_LoadingAnnotationStatus = new StatusContent(
                new GUIContent("Remove loading state from custom annotation", textureError,
                    Names.removeLoadingStateTooltip),
                new GUIContent("Remove loading state from default annotation", textureError,
                    Names.removeLoadingStateTooltip));

            m_NoFidelityMessages = new GUIContent("Number of quality levels is 0", textureError);
        }

        void RenderPluginStatus()
        {
            GUILayout.Space(10);
            using (var group = new EditorGUI.ChangeCheckScope())
            {
                m_SetupConfig.pluginEnabled = EditorGUILayout.ToggleLeft(m_PluginStatus[m_SetupConfig.pluginEnabled],
                    m_SetupConfig.pluginEnabled);
                if (group.changed) EditorUtility.SetDirty(m_SetupConfig);
            }

            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(m_ApiStatus[string.IsNullOrEmpty(m_ProjectData.apiKey)]);
            if (GUILayout.Button("Get API Key", Styles.button, GUILayout.ExpandWidth(false)))
            {
                Application.OpenURL("https://developer.android.com/games/sdk/performance-tuner/unity/enable-api#steps");
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField(m_FidelityModeStatus[m_SetupConfig.useAdvancedFidelityParameters]);
            EditorGUILayout.LabelField(m_AnnotationModeStatus[m_SetupConfig.useAdvancedAnnotations]);

            if (m_ProjectData.hasLoadingState)
            {
                EditorGUILayout.LabelField(m_LoadingAnnotationStatus[m_SetupConfig.useAdvancedAnnotations]);
            }

            if (m_SetupConfig.useAdvancedFidelityParameters && m_ProjectData.messages.Count == 0)
            {
                EditorGUILayout.LabelField(m_NoFidelityMessages);
            }
        }

        class StatusContent
        {
            readonly GUIContent m_ContentOn;
            readonly GUIContent m_ContentOff;

            public StatusContent(GUIContent contentOn, GUIContent contentOff)
            {
                m_ContentOn = contentOn;
                m_ContentOff = contentOff;
            }

            public GUIContent this[bool on]
            {
                get { return on ? m_ContentOn : m_ContentOff; }
            }
        }
    }
}