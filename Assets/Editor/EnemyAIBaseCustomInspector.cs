using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;




//this is WIP. Idea is to make random max delay time float field only available if random delay time bool has been set to true
//
#region CustomEditor
#if UNITY_EDITOR

[CustomEditor(typeof(EnemyAIBase))]
[CanEditMultipleObjects]
public class EnemyAIBaseCustomerInspector : Editor
{
    //create all the serialized properties to match our serialized fields to display
    #region Serialized Properties
         SerializedProperty _AttackType;
         SerializedProperty _BaseDamage;
         SerializedProperty _AttackDistance;
         SerializedProperty _PatrolType;
         SerializedProperty _PatrolPoints;
         SerializedProperty _PatrolDelay;
         SerializedProperty _bRandomDelayTime;
         SerializedProperty _RandomDelayMaxTime;
         SerializedProperty _RangedPerceptionDistance;
         SerializedProperty _GeneralPerceptionRadius;
        SerializedProperty _ChaseTime;

    #endregion


    private void OnEnable()
    {
        //assign the properties from the serializedObject. these are sensitive as they are found by STRING
        #region Set Serialized Properties
        _AttackType = serializedObject.FindProperty("_AttackType");
        _BaseDamage = serializedObject.FindProperty("_BaseDamage");
        _AttackDistance = serializedObject.FindProperty("_AttackDistance");
        _PatrolType = serializedObject.FindProperty("_PatrolType");
        _PatrolPoints = serializedObject.FindProperty("_PatrolPoints");
        _PatrolDelay = serializedObject.FindProperty("_PatrolDelay");
        _bRandomDelayTime = serializedObject.FindProperty("_bRandomDelayTime");
        _RandomDelayMaxTime = serializedObject.FindProperty("_RandomDelayMaxTime");
        _RangedPerceptionDistance = serializedObject.FindProperty("_RangedPerceptionDistance");
        _GeneralPerceptionRadius = serializedObject.FindProperty("_GeneralPerceptionRadius");
        _ChaseTime = serializedObject.FindProperty("_ChaseTime");

        #endregion
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        //display attack settings
        EditorGUILayout.PropertyField(_AttackType);
        EditorGUILayout.PropertyField(_BaseDamage);
        if (_PatrolType.GetUnderlyingValue().ToString() != "StaticEnemy")
        {
            EditorGUILayout.PropertyField(_AttackDistance);

        }

        //pick the patrol type
        EditorGUILayout.PropertyField(_PatrolType);
 
       
        //Patrol Random Delay Options
        if (_PatrolType.GetUnderlyingValue().ToString() == "Patrol" ||
            _PatrolType.GetUnderlyingValue().ToString() == "Roam")
        {
            EditorGUILayout.PropertyField(_PatrolPoints);

            _bRandomDelayTime.boolValue = EditorGUILayout.ToggleLeft("Randomize Patrol Delay?", _bRandomDelayTime.boolValue);

            if (_bRandomDelayTime.boolValue)
            {
                EditorGUILayout.PropertyField(_RandomDelayMaxTime);

            }
            else if (!_bRandomDelayTime.boolValue)
            {

                EditorGUILayout.PropertyField(_PatrolDelay);
            }
        }

        //ranged perception used on ALL enemy types
        EditorGUILayout.PropertyField(_RangedPerceptionDistance);

        if(_PatrolType.GetUnderlyingValue().ToString() != "StaticEnemy")
        {
            EditorGUILayout.PropertyField(_GeneralPerceptionRadius);
            EditorGUILayout.PropertyField(_ChaseTime);

        }

        serializedObject.ApplyModifiedProperties();
    }   
}
#endif
#endregion