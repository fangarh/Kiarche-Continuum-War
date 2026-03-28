# Создание префаба юнита

## Инструкция

Префаб юнита нужно создать в Unity Editor:

1. **В Unity Editor:**
   - Кликни правой кнопкой в окне Project
   - Выбери `Create > Empty Prefab`
   - Назови `UnitPrefab`

2. **Настрой префаб:**
   - Открой префаб для редактирования (двойной клик)
   - Добавь компоненты через `Add Component`:
     - `Capsule Collider`
       - Radius: 0.5
       - Height: 2
       - Center: (0, 1, 0)
     - `Rigidbody`
       - Constraints: Freeze Rotation X, Z
       - Mass: 1
     - `Unit` (скрипт из `Assets/Scripts/Units/`)
       - Move Speed: 5
       - Health: 100
       - Damage: 10
       - Attack Range: 2
       - Attack Cooldown: 1
     - `3D Object > Cylinder` (как дочерний объект)
       - Позиция: (0, 1, 0)
       - Назови "Visual"

3. **Сохраните префаб:**
   - Нажми `Save` вверху окна сцены

## Альтернативно через код

Можно создать программно через меню Unity:

```csharp
using UnityEngine;
using UnityEditor;
using KiarcheContinuumWar.Units;

public class CreateUnitPrefab : EditorWindow
{
    [MenuItem("Tools/KCW/Create Unit Prefab")]
    static void CreatePrefab()
    {
        GameObject unitObj = new GameObject("UnitPrefab");
        
        // Добавить компоненты
        unitObj.AddComponent<Unit>();
        CapsuleCollider collider = unitObj.AddComponent<CapsuleCollider>();
        collider.radius = 0.5f;
        collider.height = 2f;
        collider.center = new Vector3(0, 1, 0);
        
        Rigidbody rb = unitObj.AddComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        
        // Визуализация
        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        visual.transform.SetParent(unitObj.transform);
        visual.transform.localPosition = new Vector3(0, 1, 0);
        visual.name = "Visual";
        DestroyImmediate(visual.GetComponent<Collider>());
        
        // Сохранить как префаб
        string path = "Assets/Prefabs/UnitPrefab.prefab";
        PrefabUtility.SaveAsPrefabAsset(unitObj, path);
        DestroyImmediate(unitObj);
        
        Debug.Log($"Prefab created at {path}");
    }
}
```
