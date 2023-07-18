/*************************************************************************
 * Author: MaKayla Elder
 * Date: 07.18.23
 * 
 * Description:
 * A required class in order to use the [ReadOnly] flag on properties with custom inspectors.
 * You would use this flag in combination with a Property Drawer, as an example.
 * 
 */


using UnityEngine;

//does not require any further configuration after deriving from parent class
public class ReadOnlyAttribute : PropertyAttribute { }