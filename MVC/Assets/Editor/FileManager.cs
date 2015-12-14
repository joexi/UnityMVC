using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class FileManager
{
	public static string StreamingAssetsPath {
		get {
			if (Application.platform == RuntimePlatform.Android) {
				return "jar:file://" + Application.dataPath + "!/assets/";
			} else if (Application.platform == RuntimePlatform.IPhonePlayer) {
				return Application.dataPath + "/Raw/";
			} else {
				return "file://" + Application.dataPath + "/StreamingAssets/";
			}
		}
	}


	private static FileManager instance = new FileManager ();

	public static FileManager Instance {
		get {
			return instance;
		}
	}

	public string LoadFile (string fileName, bool encryption = false)
	{
		string result = "";
		string path = GetCachePath ();
		path += fileName;
		return LoadFileAbsolute (path, encryption);
	}

	public string LoadFileAbsolute (string path, bool encryption = false)
	{
		string result = "";
		using (StreamReader sw = new StreamReader (path, System.Text.UTF8Encoding.UTF8)) {
			result = sw.ReadToEnd ();
			sw.Close ();
			sw.Dispose ();
		}
		return result;
	}

	public void SaveFile (string fileName, string content, bool append = false, bool encryption = false)
	{
		string path = GetCachePath ();
		Directory.CreateDirectory (path);
		path += fileName;
		using (StreamWriter sw = new StreamWriter (path, append)) {
			sw.Write (content);
			sw.Close ();
			sw.Dispose ();
		}
	}

	public void SaveFileAbsolute (string path, string content, bool append = false, bool encryption = false)
	{
		using (StreamWriter sw = new StreamWriter (path, append)) {
			sw.Write (content);
			sw.Close ();
			sw.Dispose ();
		}
	}

	public void WriteBytesToFile (string fileName, byte[] bytes, bool encryption = false)
	{
		string path = GetCachePath ();
		Directory.CreateDirectory (path);
		path += fileName;
		WriteBytesToFileAbsolute (path, bytes, encryption);
	}

	public void WriteBytesToFileAbsolute (string path, byte[] bytes, bool encryption = false)
	{
        Directory.CreateDirectory (Path.GetDirectoryName(path));
		System.IO.FileStream writer = new System.IO.FileStream (path,
			                              System.IO.FileMode.Create,
			                              System.IO.FileAccess.Write);
		writer.Write (bytes, 0, bytes.Length);
		writer.Dispose ();
		writer.Close ();
	}

	public byte[] LoadBytesToFileAbsolute (string path, bool encryption = false)
	{
		using (FileStream fsSource = new FileStream(path,
			FileMode.Open, FileAccess.Read))
		{
			byte[] bytes = new byte[fsSource.Length];
			int numBytesToRead = (int)fsSource.Length;
			int numBytesRead = 0;
			while (numBytesToRead > 0)
			{
				int n = fsSource.Read(bytes, numBytesRead, numBytesToRead);
				if (n == 0)
					break;
				numBytesRead += n;
				numBytesToRead -= n;
			}
			return bytes;
		}

	}


	public string GetCachePath ()
	{
		string path = "";
		if (Application.platform == RuntimePlatform.Android) {
			path = Application.persistentDataPath + "/Cache/";
		} else if (Application.platform == RuntimePlatform.IPhonePlayer) {
			path = System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal) + "/Cache/";
		} else {
			path = Application.persistentDataPath + "/Cache/";
		}
		return path;
	}

	public bool ExistFile (string fileName)
	{
		string savePath = GetCachePath ();
		if (File.Exists (savePath + fileName)) {
			return true;
		}
		return false;
	}
}
