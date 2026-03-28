#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using KiarcheContinuumWar.Units;

namespace KiarcheContinuumWar.Editor
{
    /// <summary>
    /// Editor скрипт для создания префаба юнита.
    /// Меню: Tools > KCW > Create Unit Prefab
    /// </summary>
    public class CreateUnitPrefabEditor
    {
        [MenuItem("Tools/KCW/Create Unit Prefab")]
        public static void CreatePrefab()
        {
            // Создать GameObject
            GameObject unitObj = new GameObject("UnitPrefab");
            
            // Добавить Unit компонент
            Unit unit = unitObj.AddComponent<Unit>();
            unit.MoveSpeed = 5f;
            unit.Health = 100f;
            unit.Damage = 10f;
            unit.AttackRange = 2f;
            unit.AttackCooldown = 1f;
            
            // Добавить Collider
            CapsuleCollider collider = unitObj.AddComponent<CapsuleCollider>();
            collider.radius = 0.5f;
            collider.height = 2f;
            collider.center = new Vector3(0, 1, 0);
            
            // Добавить Rigidbody
            Rigidbody rb = unitObj.AddComponent<Rigidbody>();
            rb.mass = 1f;
            rb.linearDamping = 0f;
            rb.angularDamping = 0.05f;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            
            // Создать дочерний объект для визуализации
            GameObject visualObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            visualObj.transform.SetParent(unitObj.transform);
            visualObj.transform.localPosition = new Vector3(0, 1, 0);
            visualObj.transform.localScale = new Vector3(1f, 1f, 1f);
            visualObj.name = "Visual";
            
            // Удалить лишний collider с цилиндра
            Collider[] colliders = visualObj.GetComponents<Collider>();
            foreach (var c in colliders)
            {
                Object.DestroyImmediate(c);
            }
            
            // Сохранить как префаб
            string path = "Assets/Prefabs/UnitPrefab.prefab";
            PrefabUtility.SaveAsPrefabAsset(unitObj, path);
            
            // Удалить временный объект из сцены
            Object.DestroyImmediate(unitObj);
            
            Debug.Log($"Unit prefab created at {path}");
        }
    }
}
#endif
