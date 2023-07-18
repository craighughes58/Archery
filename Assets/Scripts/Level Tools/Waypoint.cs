/*************************************************************************
 * Author: MaKayla Elder
 * Date: 07.13.23
 * 
 * Description:
 * Waypoints for use with Waypoint Containers. Each Waypoint is a stop on a route. A route is the result of
 * a filled in WaypointContainer
 * 
 */
using UnityEditor;
using UnityEngine;

public class Waypoint : MonoBehaviour
{

    [ReadOnly] [SerializeField] internal WaypointContainer _ParentRoute;

    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnEnable()
    {
          _ParentRoute = GetComponentInParent<WaypointContainer>();
    }


    internal Vector3 GetPosition()
    {
        return transform.position;
    }
}


#region ReadOnlyAttribute - Custom Inspector Drawer


#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class WaypointInspectorReadOnlyDrawer : PropertyDrawer
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
        // Drawing Property
        EditorGUI.PropertyField(position, property, label);
        // Setting old GUI enabled value
        GUI.enabled = previousGUIState;
    }
}


#endif
#endregion