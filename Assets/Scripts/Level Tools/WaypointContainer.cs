/*************************************************************************
 * Author: MaKayla Elder
 * Date: 07.13.23
 * 
 * Description:
 * Waypoint Container for use with Waypoints. Each Waypoint is a stop on a route. A route is the result of
 * a filled in WaypointContainer
 * 
 */
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class WaypointContainer : MonoBehaviour  
{
    [Header("Waypoints")]
    [Tooltip("Waypoints in this route")]
    [ReadOnly]  [SerializeField] internal List<Waypoint> _Waypoints;

    // Start is called before the first frame update


    void Start()
    {
        //find all waypoints and add them to self if they belong to this container
        foreach (Waypoint currentFound in GetComponentsInChildren<Waypoint>())
        {           
            if (currentFound._ParentRoute.name == this.name)
            {
                if (!_Waypoints.Contains(currentFound))
                {
                    _Waypoints.Add(currentFound);

                }
            }
        }
    }

}


#if UNITY_EDITOR


/*[CustomEditor(typeof(WaypointContainer))]
public class WaypointContainerEditor : Editor
{

    SerializedProperty _Waypoints;

    private void OnEnable()
    {
        _Waypoints = serializedObject.FindProperty("_Waypoints");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        base.OnInspectorGUI();

        EditorGUI.BeginChangeCheck();
        //find all waypoints and add them to self if they belong to this container
        foreach (Waypoint currentFound in this.target.GetComponentsInChildren<Waypoint>())
        {

            List<Waypoint> _CopyWaypoints = (List<Waypoint>)_Waypoints.GetUnderlyingValue();
            if (!_CopyWaypoints.Contains(currentFound))
            {
                _CopyWaypoints.Add(currentFound);

            }
            if (currentFound._ParentRoute.name == this.target.name)
            {
               
            }
        }

        EditorGUI.EndChangeCheck();

        serializedObject.ApplyModifiedProperties();
    }

}*/

#endif

#region ReadOnlyAttribute - Custom Inspector Drawer

//look inside Waypoint Inspector Property Drawer below for details on this item
//public class ReadOnlyAttribute : PropertyAttribute { }

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class WaypointContainerInspectorReadOnlyDrawer : PropertyDrawer
{

    /// <summary>
    /// Allows us to render the Read Only Attribute which is marked by [Read Only] & [SerializeField]
    /// Make sure you mark read only properties with BOTH flags for rendering.
    /// Above ReadOnlyClass need not be filled, the flags do this job.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="property"></param>
    /// <param name="label"></param>
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Saving previous GUI enabled value
        bool previousGUIState = GUI.enabled;
        // Disabling edit for property
        GUI.enabled = false;

        //find all waypoints and add them to self if they belong to this container
        EditorGUI.BeginChangeCheck();
        Waypoint[] _FoundWaypoints = property.serializedObject.targetObject.GetComponentsInChildren<Waypoint>();
        List<Waypoint> _Waypoints = new List<Waypoint>();

        foreach (Waypoint currentFound in _FoundWaypoints)
        {
            if (currentFound._ParentRoute.name == property.serializedObject.targetObject.name)
            {
                if (!_Waypoints.Contains(currentFound))
                {
                    _Waypoints.Add(currentFound);
                }
            }
        }

        EditorGUI.EndChangeCheck();



        // Drawing Property
        EditorGUI.PropertyField(position, property, label);
        // Setting old GUI enabled value
        GUI.enabled = previousGUIState;
    }
}


#endif
#endregion
