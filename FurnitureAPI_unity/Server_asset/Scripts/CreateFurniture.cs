using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using FurnitureTool;
using STLReader;
using STLImporter;

public class CreateFurniture : MonoBehaviour {

    Dictionary<string, float> stoolparam = new Dictionary<string, float>();

    Dictionary<string, float> globparam = new Dictionary<string, float>();

    private string url = "http://ayeaye.ee.ucla.edu:5000/{0}.stl?";

	// Use this for initialization
	void Start () {
        stoolparam.Add("height", 10);
        stoolparam.Add("legs", 2);
        stoolparam.Add("radius", 20);
        globparam.Add("x", 0);
        globparam.Add("y", 0);
        globparam.Add("z", 0);
        int counter = 0;

        Furniture object1 = gameObject.AddComponent<Furniture>();
        object1.initialize(stoolparam, globparam, "Furniture", counter++);
        StartCoroutine(GetSTL(object1.Property));
        object1.Display();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // The single function we will use to get an STL binary file
    IEnumerator GetSTL(Properties property)
    {
        //Stage 1: Get the STL Binary from the server
        string parameters = LinkParam(property.LocalParameters);

        Debug.Log(parameters);

        url = string.Format(url, property.Type);

        string output = url + parameters;

        Debug.Log(output);

        using (UnityWebRequest www = UnityWebRequest.Get(output))
        {
            yield return www.SendWebRequest();
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

                //Stage 2: Transform the STL into a working Mesh
                MemoryStream stream = new MemoryStream(results);

                Mesh[] meshes = pb_Stl_Importer.ImportBinary(stream);

                Debug.Log(meshes);
                Debug.Log("size of meshes are: " + meshes.Length.ToString());
                property.Displayer.GetComponent<MeshFilter>().mesh = meshes[0];
            }
        }
    }

    // A function used to produce the strings for the parameters for the url
    private string LinkParam(Dictionary<string, float> parameters)
    {
        string result = "";

        foreach (KeyValuePair<string, float> entry in parameters)
        {
            // Case where we have to input a discrete value
            if (entry.Key == "legs")
            {
                int valuenum = (int)entry.Value;
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
}
