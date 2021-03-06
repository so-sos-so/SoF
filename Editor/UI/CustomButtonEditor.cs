using System;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Editor
{
    [CustomEditor(typeof(CustomButton))]
    public class CustomButtonEditor : ButtonEditor
    {
        private SerializedProperty singleClickIntervalTime;
        private SerializedProperty doubleClickIntervalTime;
        private SerializedProperty longClickTime;
        
        GUIStyle m_caption;
        GUIStyle caption 
        {
            get
            {
                if(m_caption == null)
                {
                    m_caption = new GUIStyle { richText = true, alignment = TextAnchor.MiddleCenter };
                }
                return m_caption;
            }
        }

        [MenuItem("GameObject/UI/CustomButton", false, -10)]
        private static void Create()
        {
            EditorApplication.ExecuteMenuItem("GameObject/UI/Button - TextMeshPro");
            var obj = Selection.activeGameObject;
            DestroyImmediate(obj.GetComponent<Button>());
            obj.AddComponent<CustomButton>();
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();
            singleClickIntervalTime = serializedObject.FindProperty("singleClickIntervalTime");
            doubleClickIntervalTime = serializedObject.FindProperty("doubleClickIntervalTime");
            longClickTime = serializedObject.FindProperty("longClickTime");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("<b><color=white>Additional configs</color></b>", caption);
            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(singleClickIntervalTime,new GUIContent("单击间隔时间(s)"));
            EditorGUILayout.PropertyField(doubleClickIntervalTime,new GUIContent("多少秒内算双击(s)"));
            EditorGUILayout.PropertyField(longClickTime,new GUIContent("超过多久算长按(s)"));
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("<b><color=white>For original ScrollRect</color></b>", caption);
            base.OnInspectorGUI();
        }
    }
}