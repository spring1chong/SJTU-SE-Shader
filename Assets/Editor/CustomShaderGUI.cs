using UnityEngine;
using UnityEditor;
using System;

public class CustomShaderGUI : ShaderGUI
{
    MaterialEditor editor;
    MaterialProperty[] properties;
    Material target;
    enum SpecularChoice
    {
        True, False
    }

    enum ShaderType
    {
        NORMAL_ONLY,
        BLINN_PHONG
    }

    public override void OnGUI(MaterialEditor editor, MaterialProperty[] properties)
    {
        this.editor = editor;
        this.properties = properties;
        this.target = editor.target as Material;

        ShaderType shaderType = target.IsKeywordEnabled("MODE_BLINN_PHONG")
            ? ShaderType.BLINN_PHONG
            : ShaderType.NORMAL_ONLY;

        shaderType = (ShaderType)EditorGUILayout.EnumPopup("Shader Type", shaderType);
        SetShaderType(shaderType);

        if (shaderType == ShaderType.NORMAL_ONLY)
        {
            DrawNormalOnlyUI();
        }
        else if (shaderType == ShaderType.BLINN_PHONG)
        {
            DrawBlinnPhongUI();
        }
    }

    private void SetShaderType(ShaderType type)
    {
        if (type == ShaderType.NORMAL_ONLY)
        {
            target.DisableKeyword("MODE_BLINN_PHONG");
            target.EnableKeyword("MODE_NORMAL_ONLY");
        }
        else if (type == ShaderType.BLINN_PHONG)
        {
            target.DisableKeyword("MODE_NORMAL_ONLY");
            target.EnableKeyword("MODE_BLINN_PHONG");
        }
    }

    private void DrawNormalOnlyUI()
    {
        EditorGUILayout.LabelField("Normal Shader", EditorStyles.boldLabel);
    }

    private void DrawBlinnPhongUI()
    {
        EditorGUILayout.LabelField("Blinn Phong Shader:", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        DrawMainTextureGUI();

        DrawSpecularGUI();

    }

    private void DrawMainTextureGUI()
    {
        MaterialProperty mainTexProp = FindProperty("_MainTex", properties);
        GUIContent mainTexLabel = new GUIContent(mainTexProp.displayName);
        editor.TextureProperty(mainTexProp, mainTexLabel.text);
    }

    private void DrawSpecularGUI()
    {
        SpecularChoice specularChoice = target.IsKeywordEnabled("USE_SPECULAR")
            ? SpecularChoice.True
            : SpecularChoice.False;
        EditorGUI.BeginChangeCheck();

        specularChoice = (SpecularChoice)EditorGUILayout.EnumPopup(
            new GUIContent("Use Specular?"), specularChoice
        );
        if (EditorGUI.EndChangeCheck())
        {
            SetSpecularKeyword(specularChoice);
        }
        if (specularChoice == SpecularChoice.True)
        {
            EditorGUI.indentLevel++;
            DrawSpecularFactorGUI();
            DrawSpecularColorGUI();
        }
    }

    private void SetSpecularKeyword(SpecularChoice choice)
    {
        if (choice == SpecularChoice.True)
        {
            target.EnableKeyword("USE_SPECULAR");
        }
        else
        {
            target.DisableKeyword("USE_SPECULAR");
        }
    }

    private void DrawSpecularFactorGUI()
    {
        MaterialProperty shininess = FindProperty("_Shininess", properties);
        GUIContent shininessLabel = new GUIContent(shininess.displayName);
        editor.FloatProperty(shininess, "Specular Factor");
    }

    private void DrawSpecularColorGUI()
    {
        MaterialProperty specColorProp = FindProperty("_SpecColor", properties);
        GUIContent specColorLabel = new GUIContent(specColorProp.displayName);
        editor.ColorProperty(specColorProp, specColorLabel.text);
    }
}
