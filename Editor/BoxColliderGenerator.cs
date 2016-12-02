using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(AutoBoxCollider))]
// ReSharper disable once CheckNamespace
public class BoxColliderGenerator : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Add Bounding Box Collider"))
        {
            AddBoundingBoxCollider();
        }

        if (GUILayout.Button("Remove Bounding Box Collider"))
        {
            RemoveBoundingBoxCollider();
        }
    }

    private void AddBoundingBoxCollider()
    {
        var autoBoxCollider = target as AutoBoxCollider;
        if (autoBoxCollider != null)
        {
            var mainObject = autoBoxCollider.gameObject;

            var existingBoxCollider = autoBoxCollider.gameObject.GetComponent<BoxCollider>();
            if (existingBoxCollider != null)
            {
                Debug.Log("This game object already has a box collider! If you want to recalculate, first remove the existing collider.");
                GUIUtility.ExitGUI();
            }

            Vector3? minExtent = null;
            Vector3? maxExtent = null;

            // Remember the current active state of each object in the hierarchy
            var listActiveStates = GetActiveStatesInHierarchy(mainObject);
            // Make everything active (adding a box collider will only auto-fit correctly if the object is active in the hierarchy)
            MakeAllActiveInHierarchy(mainObject);

            foreach (var meshFilter in mainObject.GetComponentsInChildren<MeshFilter>(true))
            {
                // Add the box collider (which will auto-fit the mesh)
                var boxCollider = meshFilter.gameObject.AddComponent<BoxCollider>();

                // BoxCollider bounds are in world space (no transform required)
                if (minExtent == null)
                {
                    minExtent = boxCollider.bounds.min;
                    maxExtent = boxCollider.bounds.max;
                }
                else
                {
                    minExtent = Vector3.Min(minExtent.Value, boxCollider.bounds.min);
                    maxExtent = Vector3.Max(maxExtent.Value, boxCollider.bounds.max);
                }

                DestroyImmediate(boxCollider);
            }

            // Set everything back to the way it was
            RestoreActiveStates(listActiveStates);

            if (minExtent.HasValue)
            {
                var mainCollider = mainObject.AddComponent<BoxCollider>();
                var size = maxExtent.Value - minExtent.Value;
                // Convert world space bounds back to local space for the collider
                mainCollider.center = mainCollider.transform.InverseTransformPoint(minExtent.Value + size * 0.5f);
                mainCollider.size = size;
            }
        }
    }

    private void RemoveBoundingBoxCollider()
    {
        var autoBoxCollider = target as AutoBoxCollider;
        if (autoBoxCollider != null)
        {
            var boxCollider = autoBoxCollider.gameObject.GetComponent<BoxCollider>();
            if (boxCollider != null)
            {
                DestroyImmediate(boxCollider);
                // Immediately exit from this render pass of the GUI , 
                // so that Unity doesn't try to draw the removed boxCollider property inspector
                GUIUtility.ExitGUI();
            }
        }
    }

    private List<GameObjectActiveState> GetActiveStatesInHierarchy(GameObject gameObject)
    {
        var activeStates = new List<GameObjectActiveState>
        {
            new GameObjectActiveState(gameObject)
        };

        foreach (Transform trans in gameObject.transform)
        {
            activeStates.AddRange(GetActiveStatesInHierarchy(trans.gameObject));
        }

        return activeStates;
    }

    private void MakeAllActiveInHierarchy(GameObject gameObject)
    {
        gameObject.SetActive(true);
        foreach (Transform trans in gameObject.transform)
        {
            MakeAllActiveInHierarchy(trans.gameObject);
        }
    }

    private void RestoreActiveStates(List<GameObjectActiveState> listActiveStates)
    {
        foreach (var gameObjectActiveState in listActiveStates)
        {
            gameObjectActiveState.GameObject.SetActive(gameObjectActiveState.IsActive);
        }
    }

    private class GameObjectActiveState
    {
        public GameObject GameObject { get; private set; }
        public bool IsActive { get; private set; }

        public GameObjectActiveState(GameObject gameObject)
        {
            GameObject = gameObject;
            IsActive = gameObject.activeSelf;
        }
    }
}