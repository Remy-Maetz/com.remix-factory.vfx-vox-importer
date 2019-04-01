using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.VFX.Utils;
using UnityEngine.Experimental.VFX;
using com.RemixFactory.VFXVox;

namespace com.RemixFactoryEditor.VFXVox
{

    [CustomEditor(typeof(VoxShape))]
    public class VoxShape_Editor : Editor
    {
        VoxShape typedTarget;
        PointCacheAsset pointCache;

        void OnEnable()
        {
            typedTarget = (VoxShape)target;
            pointCache = AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath(typedTarget.pointCacheGUID), typeof(PointCacheAsset) ) as PointCacheAsset;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            pointCache = EditorGUILayout.ObjectField("Point Cache", pointCache, typeof(PointCacheAsset), false ) as PointCacheAsset;
            if ( EditorGUI.EndChangeCheck () )
            {
                typedTarget.pointCacheGUID = AssetDatabase.AssetPathToGUID( AssetDatabase.GetAssetPath(pointCache) );
            }

            EditorGUILayout.IntField("Count", pointCache.PointCount);
            
            EditorGUILayout.Vector2Field(
                "Surfaces dimension",
                new Vector2(
                    pointCache.surfaces[0].width,
                    pointCache.surfaces[0].height
                    )
                );
            
            EditorGUILayout.ObjectField("Palette", typedTarget.palette, typeof(Texture2D), false);
            
            foreach (var s in pointCache.surfaces)
            {
                EditorGUILayout.ObjectField(s.name, s,  typeof(Texture2D), false);
            }

            if (GUILayout.Button("Apply to VFX"))
            {
                var vfx = typedTarget.gameObject.GetComponent<VisualEffect>();
                
                if (vfx == null) return;
                
                vfx.SetInt("Points Count", pointCache.PointCount);
                vfx.SetInt("Data Width", pointCache.surfaces[0].width);
                vfx.SetInt("Data Height", pointCache.surfaces[0].height);
                vfx.SetTexture("Palette", typedTarget.palette);
                vfx.SetTexture("Position_Index", pointCache.surfaces[0]);
            }
        }
    }
}