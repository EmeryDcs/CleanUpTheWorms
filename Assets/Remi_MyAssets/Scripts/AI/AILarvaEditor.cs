using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AILarva))]
public class AILarvaEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AILarva larva = (AILarva)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Stop Behavior"))
        {
            if (Application.isPlaying)
            {
                larva.StopBehaviorAndMakeKinematic();
            }
        }

        if (GUILayout.Button("Restart Behavior"))
        {
            if (Application.isPlaying)
            {
                larva.RestartBehaviorAndMakeDynamic();
            }
        }
    }
}