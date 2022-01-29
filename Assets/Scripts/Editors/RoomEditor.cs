#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

[CustomEditor(typeof(Room))]
public class RoomEditor : Editor
{
    private BoxBoundsHandle _boundsHandle = new BoxBoundsHandle();

    // the OnSceneGUI callback uses the Scene view camera for drawing handles by default
    protected virtual void OnSceneGUI()
    {
        Room room = (Room)target;

        // copy the target object's data to the handle
        _boundsHandle.center = room.Center + (Vector2)room.transform.position;
        _boundsHandle.size = room.Size;

        // draw the handle
        EditorGUI.BeginChangeCheck();
        _boundsHandle.DrawHandle();
        if (EditorGUI.EndChangeCheck())
        {
            // record the target object before setting new values so changes can be undone/redone
            Undo.RecordObject(room, "Change Room Size");

            // copy the handle's updated data back to the target object
            room.Center = _boundsHandle.center - room.transform.position;
            room.Size = _boundsHandle.size;
        }
        
        Vector2 newEntrancePosition = Handles.PositionHandle(room.transform.position + (Vector3)room.EntrancePosition, Quaternion.identity);
        if(newEntrancePosition != room.EntrancePosition)
        {
            Undo.RecordObject(room, "Move Room Entrance");
            room.EntrancePosition = newEntrancePosition - (Vector2)room.transform.position;
            room.OnValidate();
        }
    }
}

#endif