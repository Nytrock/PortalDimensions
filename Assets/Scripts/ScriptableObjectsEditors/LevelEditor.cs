using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Reflection;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
[CustomEditor(typeof(Level), true)]
[CanEditMultipleObjects]
public class LevelEditor : Editor
{
    private Level item { get { return (target as Level); } }

    public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
    {
        Texture2D texture = new(item.world.icon.texture.width, item.world.icon.texture.height);
        texture.SetPixels(item.world.icon.texture.GetPixels());
        texture.Apply();
        texture = Resize(texture, (int)(texture.width * 1.6f), (int)(texture.height * 1.6f));

        List<Texture2D> settingTextures = new();
        int num = item.id + 1;
        if (num == 0)
            settingTextures.Add((Texture2D)Resources.Load("NumberTextures/0"));
        while (num != 0) {
            var integer = (num % 10).ToString();
            settingTextures.Add((Texture2D)Resources.Load("NumberTextures/" + integer));
            num /= 10;
        }
        settingTextures.Reverse();

        int offset = 12;
        foreach (Texture2D addingTexture in settingTextures) {
            for (int x = 0; x < addingTexture.width; x++) {
                for (int y = 0; y < addingTexture.height; y++) {
                    if (addingTexture.GetPixel(x, y).a != 0) {
                        texture.SetPixel(x + offset, y + 1, Color.white);
                    }
                }
            }
            offset += 31;
        }
        texture.Apply();

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

    Texture2D Resize(Texture2D texture2D, int targetX, int targetY)
    {
        RenderTexture rt = new RenderTexture(targetX, targetY, 24);
        RenderTexture.active = rt;
        Graphics.Blit(texture2D, rt);
        Texture2D result = new Texture2D(targetX, targetY);
        result.ReadPixels(new Rect(0, 0, targetX, targetY), 0, 0);
        result.Apply();
        return result;
    }
}
#endif