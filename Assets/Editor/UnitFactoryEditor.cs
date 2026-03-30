#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UnitFactory))]
public class UnitFactoryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        UnitFactory factory = (UnitFactory)target;

        if (GUILayout.Button("템플릿 자동 채우기"))
        {
            UnitTemplate[] loaded = Resources.LoadAll<UnitTemplate>("UnitTemplates");
            factory.SetTemplatesManually(loaded);
            EditorUtility.SetDirty(factory);
            Debug.Log("템플릿 SO 로드 완료 (" + loaded.Length + ")");
        }
    }
}
#endif