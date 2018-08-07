using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using System.IO;
using STLReader;
using STLImporter;

namespace FARVR
{
	public class Furniture : MonoBehaviour
    {
		// A prefab to generate the Display Object
		public UnityEngine.Object Prefab;

		//The following are a list of private variables that make up the furniture object
		/// <summary>
		/// The GameObject that Displays the furniture
		/// </summary>
		private GameObject DisplayObject;

		/// <summary>
		/// The ID of the furniture
		/// </summary>
		private int ID;

		/// <summary>
		/// The type of furniture that will be displayed
		/// </summary>
		private string type;

		/// <summary>
		/// The parameters defining the furniture that is generated
		/// </summary>
		private Dictionary<string, float> parameters;


		//The following are existing predefined furniture objects used to verify the object created
		/// <summary>
		/// The furniture catalog. Contains all available furnitures in current class that users can generate
		/// </summary>
		private Dictionary<string, Dictionary<string, float>> FurnitureCatalog = new Dictionary<string, Dictionary<string, float>> () {
			{"stool", new Dictionary<string, float>() {
					{"height", 10},
					{"legs", 2},
					{"radius", 20}
				}},
			{"table", new Dictionary<string, float>() {
					
				}}
		};

        // Creates a furniture object
		/// <summary>
		/// Generates a furniture object by setting all the parameters in the object.
		/// </summary>
		/// <param name="localparameters">Parameters used to generate a mesh</param>
		/// <param name="ftype">The name of the furniture that we are to generate</param>
		/// <param name="id">A unique ID for the furniture.</param>
		public void MakeFurniture(Dictionary<string, float> localparameters, string ftype, int id)
        {
			//TODO: Include the Position Vector and Rotation Vector in initialization
			// Hold the name of the furniture as the type of furniture
			type = ftype;

			// Hold the id of the furniture
			ID = id;

			// Hold the parameters of the furniture
			parameters = localparameters;

			//Make new GameObject;
			DisplayObject = Instantiate(Prefab) as GameObject;

			DisplayObject.name = type + ID.ToString ();
			DisplayObject.AddComponent<MeshFilter> ();
			DisplayObject.AddComponent<MeshRenderer> ();

			// TODO: Add Physics collision to object
			// furniture.DisplayObject.AddComponent<MeshCollider> ();
			Mesh[] meshes = GetSTL ();
			if (meshes == null) {
				Debug.Log ("Failed to get mesh");
			} else {
				DisplayObject.GetComponent<MeshFilter>().mesh = meshes [0];
				DisplayObject.AddComponent<MeshCollider> ();
				DisplayObject.GetComponent<MeshCollider> ().sharedMesh = meshes [0];
				DisplayObject.GetComponent<MeshCollider> ().convex = true;
				DisplayObject.GetComponent<MeshCollider> ().sharedMaterial = Resources.Load ("Assets/FARVR/Prefabs/FurniturePhy") as PhysicMaterial;
				DisplayObject.GetComponent<MeshCollider> ().cookingOptions = MeshColliderCookingOptions.InflateConvexMesh;
			}
		}

		//Make furniture using catalog and predefined parameters
		/// <summary>
		/// Generates a furniture object by setting all the parameters in the object. Returns the parameters for the furniture
		/// </summary>
		/// <param name="ftype">The name of the furniture that we are to generate</param>
		/// <param name="id">A unique ID for the furniture.</param>
		public Dictionary<string, float> MakeFurniture (string ftype, int id) {
			type = ftype;
			ID = id;
			// Consult FurnitureCatalog for suitable furniture
			foreach(KeyValuePair<string, Dictionary<string, float>> entry in FurnitureCatalog) {
				if (entry.Key == ftype) {
					parameters = entry.Value;

					// Generate object only if we have correct parameters 
					//Make new GameObject;
					DisplayObject = Instantiate(Prefab) as GameObject;

					DisplayObject.name = type + ID.ToString ();
					DisplayObject.AddComponent<MeshFilter> ();
					DisplayObject.AddComponent<MeshRenderer> ();

					// TODO: Add Physics collision to object
					// furniture.DisplayObject.AddComponent<MeshCollider> ();
					Mesh[] meshes = GetSTL ();
					if (meshes == null) {
						Debug.Log ("Failed to get mesh");
					} else {
						DisplayObject.GetComponent<MeshFilter>().mesh = meshes [0];
						DisplayObject.AddComponent<MeshCollider> ();
						DisplayObject.GetComponent<MeshCollider> ().sharedMesh = meshes [0];
						DisplayObject.GetComponent<MeshCollider> ().convex = true;
						DisplayObject.GetComponent<MeshCollider> ().sharedMaterial = Resources.Load ("Assets/FARVR/Prefabs/FurniturePhy") as PhysicMaterial;
						DisplayObject.GetComponent<MeshCollider> ().cookingOptions = MeshColliderCookingOptions.InflateConvexMesh;
					}
				}
			}

			Debug.Log ("Failed to find valid furniture");
			return parameters;
		}
			
		/// <summary>
		/// Updates the furniture. Returns the corresponding mesh of the object and updates the furniture object.
		/// </summary>
		/// <returns>A mesh corresponding to the updated furniture.</returns>
		/// <param name="local">Local parameters of the given furniture.</param>
		/// <param name="Furniture">The GameObject corresponding to the furniture</param>
		public void UpdateFurniture(Dictionary<string, float> local) 
        { 
			parameters = local;
            // Assuming we need to update the furniture
			Mesh[] meshes = GetSTL ();
			if (meshes == null) {
				Debug.Log ("Failed to get mesh");
			} else {
				DisplayObject.GetComponent<MeshFilter> ().mesh = meshes [0];
				DisplayObject.GetComponent<MeshCollider> ().sharedMesh = meshes [0];
				DisplayObject.GetComponent<MeshCollider> ().convex = true;
				DisplayObject.GetComponent<MeshCollider> ().sharedMaterial = Resources.Load ("Assets/FARVR/Prefabs/FurniturePhy") as PhysicMaterial;
				DisplayObject.GetComponent<MeshCollider> ().cookingOptions = MeshColliderCookingOptions.InflateConvexMesh;
			}
        }

		/// <summary>
		/// Display all the data about the current furniture
		/// </summary>
		public void Display()
        {
            // Log the type
			Debug.Log("Type: " + type);

			// Log the ID
			Debug.Log("ID: " + ID.ToString());

            // Log the local param
			// Because we do not know what type the furniture is beforehand, we can't simply call the key
			foreach (KeyValuePair<string, float> entry in parameters) {
                Debug.Log("Local Parameter: " + entry.Key + " = " + entry.Value.ToString());
            }
        }

        // Save and export the STL File from the object
        public void Export()
        { 
            // This should be the same for all of the children, which is why it should be modified here 
			ExportSTL();
        }

		// The single function we will use to get an STL binary file
		private Mesh[] GetSTL(){
		//Store Mesh
		Mesh[] holder = null;

		string url = "http://ayeaye.ee.ucla.edu:5000/{0}.stl?{1}";

		//Stage 1: Get the STL Binary from the server
		string param = LinkParam(parameters);

		url = string.Format(url, name, param);

		using (UnityWebRequest www = UnityWebRequest.Get(url))
		{
			www.SendWebRequest();
			while (!www.isDone) ;

			if (www.isNetworkError || www.isHttpError)
			{
				Debug.Log(www.error);
			}
			else
			{
				// Or retrieve results as binary data
				byte[] results = www.downloadHandler.data;

				holder = MakeFurniture(results);
			}
		}

		return holder;
		}

		private void ExportSTL() {
			string url = "http://ayeaye.ee.ucla.edu:5000/{0}.stl?{1}";

			//Stage 1: Get the STL Binary from the server
			string param = LinkParam(parameters);

			url = string.Format(url, name, param);

			using (UnityWebRequest www = UnityWebRequest.Get(url))
			{
				www.SendWebRequest();
				while (!www.isDone) ;

				if (www.isNetworkError || www.isHttpError)
				{
					Debug.Log(www.error);
				}
				else
				{
					string fileName = Application.persistentDataPath + "/" + type + ID + ".stl"; 

					System.IO.File.WriteAllBytes(fileName, www.downloadHandler.data);

					if (System.IO.File.Exists ("file://" + Application.persistentDataPath + type + ID + ".stl")) { 
						print ("File does exist"); 
					} else {
						print ("File does not exist");
					}
				}
			}

		}

		// A function to read the bytes into furniture object
		private Mesh[] MakeFurniture(byte[] data) {
			//Stage 2: Transform the STL into a working Mesh
			MemoryStream stream = new MemoryStream(data);

			Mesh[] meshes = pb_Stl_Importer.ImportBinary(stream);

			Debug.Log(meshes);
			Debug.Log("size of meshes are: " + meshes.Length.ToString());

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
    };
}


