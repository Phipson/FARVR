using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using System.IO;
using STLImporter;
using Parabox.STL;

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
		/// <returns>RETURN CODE: 0 = Successful creation of Furniture; 1 = Failed to retrieve mesh from server; 2 = Invalid parameters</returns>
		/// <param name="localparameters">Parameters used to generate a mesh</param>
		/// <param name="ftype">The name of the furniture that we are to generate</param>
		/// <param name="id">A unique ID for the furniture.</param>
		public int MakeFurniture(Dictionary<string, float> localparameters, string ftype, int id, Vector3 location, Quaternion rotation, Vector3 scale)
        {
			if (!VerifyFurniture (ftype, localparameters)) {
				return 2;
			}

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
				return 1;
			} else {
				DisplayObject.GetComponent<MeshFilter>().mesh = meshes [0];
				DisplayObject.AddComponent<MeshCollider> ();
				DisplayObject.GetComponent<MeshCollider> ().sharedMesh = meshes [0];
				DisplayObject.GetComponent<MeshCollider> ().convex = true;
				DisplayObject.GetComponent<MeshCollider> ().sharedMaterial = Resources.Load ("Assets/FARVR/Prefabs/FurniturePhy") as PhysicMaterial;
				DisplayObject.GetComponent<MeshCollider> ().cookingOptions = MeshColliderCookingOptions.InflateConvexMesh;
				TransformFurniture(location, rotation, scale);
			}
			return 0;
		}

		// The following are overloaded functions that act as an alternative to default parameters
		// Optional rotation AND scale
		public int MakeFurniture(Dictionary<string, float> localparameters, string ftype, int id, Vector3 location) {
			return MakeFurniture (localparameters, ftype, id, location, Quaternion.identity, new Vector3 (0.01f, 0.01f, 0.01f));
		}

		// Optional rotation
		public int MakeFurniture(Dictionary<string, float> localparameters, string ftype, int id, Vector3 location, Vector3 scale) {
			return MakeFurniture (localparameters, ftype, id, location, Quaternion.identity, scale);
		}

		// Optional scale
		public int MakeFurniture(Dictionary<string, float> localparameters, string ftype, int id, Vector3 location, Quaternion rotate) {
			return MakeFurniture (localparameters, ftype, id, location, rotate, new Vector3 (0.01f, 0.01f, 0.01f));
		}

		//Make furniture using catalog and predefined parameters
		/// <summary>
		/// Generates a furniture object by setting all the parameters in the object. Returns the parameters for the furniture
		/// </summary>
		/// <returns>Dictionary of the parameters that were used to generate the furniture; NULL = Invalid parameters</returns>
		/// <param name="ftype">The name of the furniture that we are to generate</param>
		/// <param name="id">A unique ID for the furniture.</param>
		public Dictionary<string, float> MakeFurniture (string ftype, int id, Vector3 location, Quaternion rotation, Vector3 scale) {
			type = ftype;
			ID = id;

			//Verify that the furniture is valid
			if (!VerifyFurniture (ftype)) {
				Debug.Log ("Failed to find valid furniture");
				return null;
			} else {
				//If the furniture is valid
				parameters = FurnitureCatalog [ftype];

				type = ftype;
				ID = id;

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
					return null;
				} else {
					DisplayObject.GetComponent<MeshFilter>().mesh = meshes [0];
					DisplayObject.AddComponent<MeshCollider> ();
					DisplayObject.GetComponent<MeshCollider> ().sharedMesh = meshes [0];
					DisplayObject.GetComponent<MeshCollider> ().convex = true;
					DisplayObject.GetComponent<MeshCollider> ().sharedMaterial = Resources.Load ("Assets/FARVR/Prefabs/FurniturePhy") as PhysicMaterial;
					DisplayObject.GetComponent<MeshCollider> ().cookingOptions = MeshColliderCookingOptions.InflateConvexMesh;
					TransformFurniture (location, rotation, scale);
				}
			}
			return parameters;
		}

		// The following are overloaded functions that act as an alternative to default parameters
		// Optional rotation AND scale
		public Dictionary<string, float> MakeFurniture(string ftype, int id, Vector3 location) {
			return MakeFurniture (ftype, id, location, Quaternion.identity, new Vector3 (0.01f, 0.01f, 0.01f));
		}

		// Optional rotation
		public Dictionary<string, float> MakeFurniture(string ftype, int id, Vector3 location, Vector3 scale) {
			return MakeFurniture (ftype, id, location, Quaternion.identity, scale);
		}

		// Optional scale
		public Dictionary<string, float> MakeFurniture(string ftype, int id, Vector3 location, Quaternion rotate) {
			return MakeFurniture (ftype, id, location, rotate, new Vector3 (0.01f, 0.01f, 0.01f));
		}
			
		/// <summary>
		/// Updates the furniture. Returns the corresponding mesh of the object and updates the furniture object.
		/// </summary>
		/// <returns>RETURN CODE: 0 = Successfully updated furniture; 1 = Failed to retrieve mesh</returns>
		/// <param name="local">Local parameters of the given furniture.</param>
		/// <param name="Furniture">The GameObject corresponding to the furniture</param>
		public int UpdateFurniture(Dictionary<string, float> localparameters) 
        { 
			parameters = localparameters;
            // Assuming we need to update the furniture
			Mesh[] meshes = GetSTL ();
			if (meshes == null) {
				Debug.Log ("Failed to get mesh");
				return 1;
			} else {
				DisplayObject.GetComponent<MeshFilter> ().mesh = meshes [0];
				DisplayObject.GetComponent<MeshCollider> ().sharedMesh = meshes [0];
				DisplayObject.GetComponent<MeshCollider> ().convex = true;
				DisplayObject.GetComponent<MeshCollider> ().sharedMaterial = Resources.Load ("Assets/FARVR/Prefabs/FurniturePhy") as PhysicMaterial;
				DisplayObject.GetComponent<MeshCollider> ().cookingOptions = MeshColliderCookingOptions.InflateConvexMesh;
			}
			return 0;
        }

		/// <summary>
		/// Transforms the furniture.
		/// </summary>
		/// <returns>RETURN CODE: 0 = Successful update of transformation; 1 = Invalid parameters</returns>
		/// <param name="translate">Vector3 for new object position.</param>
		/// <param name="rotate">Quarternion rotation for new object rotation.</param>
		/// <param name="scale">Vector3 for new object localscale.</param>
		public int TransformFurniture(Vector3 translate, Quaternion rotate, Vector3 scale) {
			if (translate != DisplayObject.transform.position) {
				DisplayObject.transform.position = translate;
			}

			if (rotate != DisplayObject.transform.rotation) {
				DisplayObject.transform.rotation = rotate;
			}

			Vector3 scaler = new Vector3 (0.01f * scale.x, 0.01f * scale.y, 0.01f * scale.z);

			if (scaler != DisplayObject.transform.localScale) {
				DisplayObject.transform.localScale = scaler;
			}

			return 0;
		}

		// To obtain transformations of furniture
		public Vector3 GetPosition() {
			return DisplayObject.transform.position;
		}

		public Vector3 GetScale() {
			Vector3 scale = DisplayObject.transform.localScale;
			return (new Vector3 (scale.x / 0.01f, scale.y / 0.01f, scale.z / 0.01f));
		}

		public Quaternion GetRotation() {
			return DisplayObject.transform.rotation;
		}

		/// <summary>
		/// Display all the data about the current furniture. For Debugging Purposes.
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
		/// <summary>
		/// Export this instance.
		/// </summary>
		/// <returns>RETURN CODE: 0 = Successfully exported STL File; 1 = Failed to export STL file</returns>
        public int Export()
        { 
            // This should be the same for all of the children, which is why it should be modified here 
			if (!ExportSTL ()) {
				return 1;
			} else {
				return 0;
			}
        }

		// Verify Furniture parameters via FurnitureCatalog
		private bool VerifyFurniture(string type, Dictionary<string, float> parameter) {
			foreach (KeyValuePair<string, Dictionary<string, float>> entry in FurnitureCatalog) {
				// Check if furniture type exists
				if (entry.Key == type) {
					// If the type exists, we verify the parameters are valid
					// Search the dictionary from FurnitureCatalog and the parameters to find similarity
					foreach (KeyValuePair<string, float> requirement in entry.Value) {
						// To verify all parameters, we ensure we can find one of each value entry
						bool match = false;
						foreach (KeyValuePair<string, float> comparer in parameter) {
							//Once we find a match, we don't have to search exhaustively
							if (comparer.Key == requirement.Key) {
								match = true;
								break;
							}
						}

						// If we do not find a match after searching through all the parameters, then we can confirm that parameters are invalid
						if (!match) {
							return false;
						}
					}

					// If we search through entire parameter and all match, then it is valid
					return true;
				}
			}

			// If we can't find the correct type, then it is invalid
			return false;
		}

		// Overloaded version of VerifyFurniture that only takes type
		private bool VerifyFurniture(string type) {
			return VerifyFurniture(type, parameters);
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

				holder = MakeMesh(results);
			}
		}
		return holder;
		}

		//Exports STL into a .stl file for printing and manufacturing
		private bool ExportSTL() {
			GameObject[] garr = new GameObject[1];

			garr [0] = DisplayObject;

			string fileName = Application.persistentDataPath + "/" + type + ID + ".stl"; 



			if (pb_Stl_Exporter.Export(fileName, garr, FileType.Binary)) {
				return true;
			} else {
				return false;
			}

		}

		// A function to read the bytes into furniture object
		private Mesh[] MakeMesh(byte[] data) {
			//Stage 2: Transform the STL into a working Mesh
			MemoryStream stream = new MemoryStream(data);

			Mesh[] meshes = Importer.ImportBinary(stream);

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

			return result;
		}
    };
}


