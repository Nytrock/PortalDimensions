using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Reflection;
using Object = UnityEngine.Object;

[CustomEditor(typeof(Character), true)]
[CanEditMultipleObjects]
public class CharacterEditor : Editor
{
    private Character item { get { return (target as Character); } }
    public List<Texture2D> overlayIcons = new List<Texture2D>();

    public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
    {
        Texture2D texture = AssetPreview.GetAssetPreview(item.prefab);
        int num = 3;

        Color pixelsColor = new();
        switch (item.rarity) {
           case CharacterInterface.RarityLevel.Common: pixelsColor = Color.white; break; 
           case CharacterInterface.RarityLevel.Rare: pixelsColor = new Color(104f / 256f, 151f / 256f, 229f / 256f, 1f); break; 
           case CharacterInterface.RarityLevel.Epic: pixelsColor = new Color(137f / 256f, 71f / 256f, 253f / 256f, 1f); break; 
           case CharacterInterface.RarityLevel.Legendary: pixelsColor = new Color(231f / 256f, 196f / 256f, 0f, 1f); break; 
           case CharacterInterface.RarityLevel.Unique: pixelsColor = new Color(235f / 256f, 76f / 256f, 74f / 256f, 1f); break; 
        }

        for (int x = 0; x < texture.width; x++) {
            for (int y = 0; y < texture.height; y++) {
                if (x < num || y < num || x + num > texture.width - 1 || y + num > texture.height - 1)
                    texture.SetPixel(x, y, pixelsColor);
            }
        }
        texture.Apply();

        Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        if (sprite != null)
        {
            Type t = GetType("UnityEditor.SpriteUtility");
            if (t != null)
            {
                MethodInfo method = t.GetMethod("RenderStaticPreview", new Type[] { typeof(Sprite), typeof(Color), typeof(int), typeof(int) });
                if (method != null)
                {
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

        if (TypeName.Contains("."))
        {
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
        foreach (var assemblyName in referencedAssemblies)
        {
            var assembly = Assembly.Load(assemblyName);
            if (assembly != null)
            {
                type = assembly.GetType(TypeName);
                if (type != null)
                    return type;
            }
        }
        return null;
    }
}