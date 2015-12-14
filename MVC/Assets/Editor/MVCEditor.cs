using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

public class MVCEditor : ScriptableObject
{

    [MenuItem("MVC/Create")]
    public static void Create()
    {
        GameObject root = Selection.activeGameObject;
        parseModel(root);
        parseView(root);
        parseController(root);
        Reimport(Application.dataPath + "/Scripts/" + root.name + "/");

    }

    [MenuItem("MVC/Assemble")]
    public static void Assemble()
    {
        GameObject root = Selection.activeGameObject;
        string class_name = root.name;
        string controller_name = class_name + "Controller";
        string view_name = class_name + "View";
        ControllerBase controller =  root.AddComponent(controller_name) as ControllerBase;
        ViewBase view = root.AddComponent(view_name) as ViewBase;
        controller.View = view;

        Connect<UILabel>(root.transform, view);
        Connect<UISprite>(root.transform, view);
        Connect<UITexture>(root.transform, view);
        Connect<UIButton>(root.transform, view);
    }


    static void Reimport(string path) {
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ImportRecursive);
        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
    }

    static void parseModel(GameObject root)
    {
        string module_model_path = Application.dataPath + "/Editor/ModuleModel.txt";
        string text_model = FileManager.Instance.LoadFileAbsolute(module_model_path);
        Debug.Log(text_model);
        string result = "";
        text_model = text_model.Replace ("{0}", root.name);
        text_model = text_model.Replace ("{1}", result);
        string out_put = text_model;
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(out_put);
        string output_path = Application.dataPath + "/Scripts/" + root.name + "/" + root.name + "Model.cs";
        FileManager.Instance.WriteBytesToFileAbsolute(output_path, bytes);
    }

    static void parseView(GameObject root)
    {
        string module_model_path = Application.dataPath + "/Editor/ModuleView.txt";
        string text_model = FileManager.Instance.LoadFileAbsolute(module_model_path);


        string result = "";

        result += print<UILabel>(root.transform);
        result += print<UISprite>(root.transform);
        result += print<UITexture>(root.transform);
        result += print<UIButton>(root.transform);

        text_model = text_model.Replace ("{0}", root.name);
        text_model = text_model.Replace ("{1}", result);
        string out_put = text_model;

        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(out_put);
        string output_path = Application.dataPath + "/Scripts/" + root.name + "/" + root.name + "View.cs";
        FileManager.Instance.WriteBytesToFileAbsolute(output_path, bytes);
    }

    static void parseController(GameObject root)
    {
        string module_model_path = Application.dataPath + "/Editor/ModuleController.txt";
        string text_model = FileManager.Instance.LoadFileAbsolute(module_model_path);

        string result = "";
        text_model = text_model.Replace ("{0}", root.name);
        text_model = text_model.Replace ("{1}", result);
        string out_put = text_model;

        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(out_put);
        string output_path = Application.dataPath + "/Scripts/" + root.name + "/" + root.name + "Controller.cs";
        FileManager.Instance.WriteBytesToFileAbsolute(output_path, bytes);

    }

    static void Connect<T>(Transform trans, ViewBase view) where T : Component
    {
        Dictionary <string ,bool> go_dic = new Dictionary<string, bool>();
        foreach (T sp in trans.GetComponentsInChildren<T>(true))
        {
            string class_name = typeof(T).ToString();
            string obj_name = sp.name;
            int index = 0;
            while (go_dic.ContainsKey(obj_name))
            {
                index++;
                obj_name = sp.name + index.ToString();
            }
            go_dic[obj_name] = true;
            FieldInfo field = view.GetType().GetField(obj_name);
            if (field != null)
            {
                field.SetValue(view, sp);
            }
        }
    }

    static string print<T>(Transform trans) where T : Component
    {
        string result = string.Empty;
        Dictionary <string ,bool> go_dic = new Dictionary<string, bool>();
        foreach (T sp in trans.GetComponentsInChildren<T>(true))
        {
            string class_name = typeof(T).ToString();
            string obj_name = sp.name;
            int index = 0;
            while (go_dic.ContainsKey(obj_name))
            {
                index++;
                obj_name = sp.name + index.ToString();
            }
            result += string.Format("    public {0} {1}; \n", class_name, obj_name);
            go_dic[obj_name] = true;
        }
        return result;
    }
}
