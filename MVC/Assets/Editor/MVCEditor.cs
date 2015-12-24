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

        result += generate<UILabel>(root.transform);
        result += generate<UISprite>(root.transform);
        result += generate<UITexture>(root.transform);
        result += generate<UIButton>(root.transform);

        text_model = text_model.Replace ("{0}", root.name);
        text_model = text_model.Replace ("{1}", result);

		text_model = generateButtonEvent (root.transform, text_model);

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
		foreach (T temp in trans.GetComponentsInChildren<T>(true))
        {
            string class_name = typeof(T).ToString();
			string obj_name = prefix<T>() + temp.name;
            int index = 0;
            while (go_dic.ContainsKey(obj_name))
            {
                index++;
                obj_name = temp.name + index.ToString();
            }
            go_dic[obj_name] = true;
            FieldInfo field = view.GetType().GetField(obj_name);
            if (field != null)
            {
                field.SetValue(view, temp);
            }
        }
    }

	static string generateButtonEvent(Transform trans, string result)
	{
		
		string registerResult = string.Empty;
		string eventResult = string.Empty;
		Dictionary <string ,bool> go_dic = new Dictionary<string, bool>();

		foreach (UIButton sp in trans.GetComponentsInChildren<UIButton>(true)) {
			string class_name = typeof(UIButton).ToString();
			string obj_name = sp.name;
			int index = 0;
			while (go_dic.ContainsKey(obj_name))
			{
				index++;
				obj_name = sp.name + index.ToString();
			}
			registerResult += string.Format("        UIEventListener.Get({0}.gameObject).onClick = {1}_onclick;\n", prefix<UIButton>() + obj_name, obj_name);
			eventResult += string.Format(@"    void {0}_onclick (GameObject go) {1}", obj_name, "{ \n\n\t} \n\n");
			go_dic[obj_name] = true;
		}
		result = result.Replace ("{2}", registerResult);
		result = result.Replace ("{3}", eventResult);
		return result;
	}

    static string generate<T>(Transform trans) where T : Component
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
			result += string.Format("    public {0} {1}; \n", class_name, prefix<T>() + obj_name);
            go_dic[obj_name] = true;
        }
        return result;
    }

	static string prefix<T>() {
		string type = typeof(T).ToString();
		switch(type) {
			case "UIButton":
				{
					return "btn_";
				}
			case "UISprite":
				{
					return "sp_";
				}
			case "UITexture":
				{
					return "tex_";
				}
			case "UILabel":
				{
					return "lbl_";
				}
		}
		return string.Empty;
	}
}
