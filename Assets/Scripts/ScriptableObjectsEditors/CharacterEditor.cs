using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using Object = UnityEngine.Object;

[CustomEditor(typeof(Character), true)]
[CanEditMultipleObjects]
public class CharacterEditor : Editor
{
    private Character item { get { return (target as Character); } }

    public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
    {
        var texture = PrefabUtility.GetIconForGameObject(item.prefab);
        Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        if (sprite != null) {
            Type t = GetType("UnityEditor.SpriteUtility");
            if (t != null) {
                MethodInfo method = t.GetMethod("RenderStaticPreview", new Type[] { typeof(Sprite), typeof(Color), typeof(int), typeof(int) });
                if (method != null) {
                    object ret = method.Invoke("RenderStaticPreview", new object[] { sprite, Color.white, width, height });
                    if (ret is Texture2D)
                        return ret as Texture2D;
                }
            }
        }
        return base.RenderStaticPreview(assetPath, subAssets, width, height);
    }

    private static Type GetType(string TypeName)
    {
        var type = Type.GetType(TypeName);
        if (type != null)
            return type;

        if (TypeName.Contains(".")) {
            var assemblyName = TypeName.Substring(0, TypeName.IndexOf('.'));
            var assembly = Assembly.Load(assemblyName);
            if (assembly == null)
                return null;
            type = assembly.GetType(TypeName);
            if (type != null)
                return type;
        }

        var currentAssembly = Assembly.GetExecutingAssembly();
        var referencedAssemblies = currentAssembly.GetReferencedAssemblies();
        foreach (var assemblyName in referencedAssemblies) {
            var assembly = Assembly.Load(assemblyName);
            if (assembly != null) {
                type = assembly.GetType(TypeName);
                if (type != null)
                    return type;
            }
        }
        return null;
    }
}