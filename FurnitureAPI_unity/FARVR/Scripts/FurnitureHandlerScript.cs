using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using System.IO;
using STLReader;
using STLImporter;
using UnityEngine.UI;

public class FurnitureHandlerScript : MonoBehaviour {
	//Slider for modifying stool height
	public Slider Height;

	//Slider for modifying stool radius
	public Slider Radius;

	//Slider for modifying stool legs
	public Slider Legs;

	//GameObject to hold the stool
	private GameObject furniture;

	// The following are several generic test parameters to generate furniture
	// Generic Dictionary to hold parameters for stool
	private Dictionary<string, float> stoolparam = new Dictionary<string, float>();

	// Base string for STL object requests
	private string url = "http://ayeaye.ee.ucla.edu:5000/{0}.stl?";

	// Names of the object(s) we are trying to get
	string stool = "stool";

	// Use this for initialization
	void Start () {
		furniture = new GameObject("Furniture");
		furniture.AddComponent<MeshFilter>();
		furniture.AddComponent<MeshRenderer>();
		MakeDictionary();
		Mesh[] meshes = GetSTL (stool, stoolparam);
		if (meshes == null) {
			Debug.Log ("Failed to get mesh");
		} else {
			furniture.GetComponent<MeshFilter> ().mesh = meshes [0];
		}
    }

	// Make fake data for dictionaries
	private void MakeDictionary() {
		stoolparam.Add("height", Height.value);
		stoolparam.Add("legs", Legs.value);
		stoolparam.Add("radius", Radius.value);
	}

	// The single function we will use to get an STL binary file
	public Mesh[] GetSTL(string name, Dictionary<string, float> arguments){
		//Store Mesh
		Mesh[] holder = null;

		//Stage 1: Get the STL Binary from the server
		string parameters = LinkParam(arguments);

		Debug.Log(parameters);

		url = string.Format(url, name);

		string output = url + parameters;

		Debug.Log(output);
		
		using (UnityWebRequest www = UnityWebRequest.Get(output))
		{
		    www.SendWebRequest();
		    while (!www.isDone) ;

		    if (www.isNetworkError || www.isHttpError)
		    {
				Debug.Log(www.error);
		    }
		    else
		    {
				// For Debugging purposes
				// Show results as text
				Debug.Log(www.downloadHandler.text);

				// Or retrieve results as binary data
				byte[] results = www.downloadHandler.data;
				Debug.Log(results);

				// Retrieve results converted to string
				Debug.Log(Convert.ToBase64String(results));

				holder = MakeFurniture(results);
		    }
		}

		return holder;
	}

	// A function to read the bytes into furniture object
	private Mesh[] MakeFurniture(byte[] data) {
		//Stage 2: Transform the STL into a working Mesh
		MemoryStream stream = new MemoryStream(data);

		Mesh[] meshes = pb_Stl_Importer.ImportBinary(stream);

		Debug.Log(meshes);
		Debug.Log("size of meshes are: " + meshes.Length.ToString());

		// The combine functions are only used if there are multiple meshes
		// CombineInstance[] combine = new CombineInstance[meshes.Length];


		// Combine the meshes together- only if there are more than 1
		/* 
		int i = 0;
		while (i < meshFilters.Length) {
		    combine[i].mesh = meshFilters[i].sharedMesh;
		    combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
		    meshFilters[i].gameObject.active = false;
		    i++;
		} 
		*/

		// Add the mesh to a new furniture object
		// In this case, the importer automatically merges the meshes together, so the only mesh in the array is at index 0
		return meshes;
	}

	// A function used to produce the strings for the parameters for the url
	private string LinkParam(Dictionary<string, float> parameters) {
		string result = "";

		foreach(KeyValuePair<string, float> entry in parameters) {
			// Case where we have to input a discrete value
			if (entry.Key == "legs") {
				int valuenum = (int) entry.Value;
				result += (entry.Key + "=" + valuenum.ToString() + "&");
				continue;
			}

			// Case where we can input a decimal value
			result += (entry.Key + "=" + entry.Value.ToString() + "&");
		}

		// For the last string we have an extra ampersand sign at the end
		result = result.Substring(0, result.Length - 1);

		Debug.Log(result);

		return result;
	}

	// Check if any update is required
	private bool CheckParam() {
		if (stoolparam["height"] != Height.value) {
			stoolparam["height"] = Height.value;
			return true;
		}

		if (stoolparam["legs"] != Legs.value) {
			stoolparam["legs"] = Legs.value;
			return true;
		}

		if (stoolparam["radius"] != Radius.value) {
			stoolparam["radius"] = Radius.value;
			return true;
		}

		return false;
	}

	// Update is called once per frame
	// TLDR: When the slider value changes, then the stool parameters change
	//	 When the stool parameters change, the STL file changes
	void Update () {

		if (CheckParam()) {
			Mesh[] meshes = GetSTL (stool, stoolparam);
			if (meshes == null) {
				Debug.Log ("Failed to get mesh");
			} else {
				furniture.GetComponent<MeshFilter> ().mesh = meshes [0];
			}
		}
	}
}
