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
		// A Material used to display on the Prefab Object
		public Material MaterialPrefab;

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
					{"radius", 25},
					{"angle", 90} /* Temporarily due to bug we found with the radius */
				}},
			{"table", new Dictionary<string, float>() {
					
				}}
		};

		/// <summary>
		/// Photon Update the Furniture
		/// </summary>
		/// <returns>The furniture.</returns>
		/// <param name="localparameters">Localparameters.</param>
		/// <param name="ftype">Ftype.</param>
		/// <param name="id">Identifier.</param>
		/// <param name="location">Location.</param>
		/// <param name="rotation">Rotation.</param>
		/// <param name="scale">Scale.</param>
		[PunRPC]
		void PUNMakeFurniture(Dictionary<string, float> localparameters, string ftype, int id, Vector3 location, Quaternion rotation, Vector3 scale) {
			// Hold the name of the furniture as the type of furniture
			type = ftype;

			// Hold the id of the furniture
			ID = id;

			// Hold the parameters of the furniture
			parameters = localparameters;

			gameObject.name = type + ID.ToString ();
			gameObject.AddComponent<MeshFilter> ();
			gameObject.AddComponent<MeshRenderer> ();

			// TODO: Add Physics collision to object
			// furniture.gameObject.AddComponent<MeshCollider> ();
			Mesh[] meshes = GetSTL ();
			if (meshes == null) {
				Debug.Log ("Failed to get mesh");
				return;
			} else {
				gameObject.GetComponent<MeshFilter>().mesh = meshes [0];
				gameObject.AddComponent<MeshCollider> ();
				gameObject.GetComponent<MeshCollider> ().sharedMesh = meshes [0];
				gameObject.GetComponent<MeshCollider> ().convex = true;
				gameObject.GetComponent<MeshCollider> ().sharedMaterial = Resources.Load ("Assets/FARVR/Prefabs/FurniturePhy") as PhysicMaterial;
				gameObject.GetComponent<MeshCollider> ().cookingOptions = MeshColliderCookingOptions.InflateConvexMesh;
				Renderer rend = gameObject.GetComponent<Renderer> ();
				rend.material = MaterialPrefab;

				/* Transforms the furniture based on what we have initialized it to be */
				gameObject.GetComponent<PhotonView> ().RPC ("PUNTransformFurniture", PhotonTargets.AllBufferedViaServer, location, rotation, scale);

				/* Update: Added a Collider tag that allows us to distinguish whether an Existing Furniture was hit or not */
				gameObject.tag = "Furniture";
			}
			return;
		}

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

			if (PhotonNetwork.inRoom && gameObject.GetComponent<PhotonView> ().viewID > 0) {
				Debug.Log ("Calling Connected Function");
				gameObject.GetComponent<PhotonView> ().RPC ("PUNMakeFurniture", PhotonTargets.AllBufferedViaServer, localparameters, ftype, id, location, rotation, scale);
				if (gameObject.GetComponent<MeshFilter> () == null) {
					return 1;
				} else {
					return 0;
				}
			}

			// Hold the name of the furniture as the type of furniture
			type = ftype;

			// Hold the id of the furniture
			ID = id;

			// Hold the parameters of the furniture
			parameters = localparameters;

			gameObject.name = type + ID.ToString ();
			gameObject.AddComponent<MeshFilter> ();
			gameObject.AddComponent<MeshRenderer> ();

			// TODO: Add Physics collision to object
			// furniture.gameObject.AddComponent<MeshCollider> ();
			Mesh[] meshes = GetSTL ();
			if (meshes == null) {
				Debug.Log ("Failed to get mesh");
				return 1;
			} else {
				gameObject.GetComponent<MeshFilter>().mesh = meshes [0];
				gameObject.AddComponent<MeshCollider> ();
				gameObject.GetComponent<MeshCollider> ().sharedMesh = meshes [0];
				gameObject.GetComponent<MeshCollider> ().convex = true;
				gameObject.GetComponent<MeshCollider> ().sharedMaterial = Resources.Load ("Assets/FARVR/Prefabs/FurniturePhy") as PhysicMaterial;
				gameObject.GetComponent<MeshCollider> ().cookingOptions = MeshColliderCookingOptions.InflateConvexMesh;
				Renderer rend = gameObject.GetComponent<Renderer> ();
				rend.material = MaterialPrefab;

				/* Transforms the furniture based on what we have initialized it to be */
				TransformFurniture(location, rotation, scale);

				/* Update: Added a Collider tag that allows us to distinguish whether an Existing Furniture was hit or not */
				gameObject.tag = "Furniture";
			}
			return 0;
		}

		// The following are overloaded functions that act as an alternative to default parameters
		// Optional rotation AND scale
		public int MakeFurniture(Dictionary<string, float> localparameters, string ftype, int id, Vector3 location) {
			if (PhotonNetwork.inRoom && gameObject.GetComponent<PhotonView> ().viewID > 0) {
				Debug.Log ("Calling Connected Function");
				gameObject.GetComponent<PhotonView> ().RPC ("PUNMakeFurniture", PhotonTargets.AllBufferedViaServer, localparameters, ftype, id, location, Quaternion.identity, new Vector3 (1f, 1f, 1f));
				if (gameObject.GetComponent<MeshFilter> () == null) {
					return 1;
				} else {
					return 0;
				}
			} else {
				return MakeFurniture (localparameters, ftype, id, location, Quaternion.identity, new Vector3 (1f, 1f, 1f));
			}
		}

		// Optional rotation
		public int MakeFurniture(Dictionary<string, float> localparameters, string ftype, int id, Vector3 location, Vector3 scale) {
			if (PhotonNetwork.inRoom && gameObject.GetComponent<PhotonView> ().viewID > 0) {
				gameObject.GetComponent<PhotonView> ().RPC ("PUNMakeFurniture", PhotonTargets.AllBufferedViaServer, localparameters, ftype, id, location, Quaternion.identity, scale);
				if (gameObject.GetComponent<MeshFilter> () == null) {
					return 1;
				} else {
					return 0;
				}
			} else {
				return MakeFurniture (localparameters, ftype, id, location, Quaternion.identity, scale);
			}
		}

		// Optional scale
		public int MakeFurniture(Dictionary<string, float> localparameters, string ftype, int id, Vector3 location, Quaternion rotate) {
			if (PhotonNetwork.inRoom && gameObject.GetComponent<PhotonView> ().viewID > 0) {
				gameObject.GetComponent<PhotonView>().RPC("PUNMAkeFurniture", PhotonTargets.AllBufferedViaServer, localparameters, ftype, id, location, rotate, new Vector3(1f, 1f, 1f));
				if (gameObject.GetComponent<MeshFilter> () == null) {
					return 1;
				} else {
					return 0;
				}
			} else {
				return MakeFurniture (localparameters, ftype, id, location, rotate, new Vector3 (1f, 1f, 1f));
			}
		}

		//Make furniture using catalog and predefined parameters
		/// <summary>
		/// Generates a furniture object by setting all the parameters in the object. Returns the parameters for the furniture
		/// </summary>
		/// <returns>Dictionary of the parameters that were used to generate the furniture; NULL = Invalid parameters</returns>
		/// <param name="ftype">The name of the furniture that we are to generate</param>
		/// <param name="id">A unique ID for the furniture.</param>
		public Dictionary<string, float> MakeFurniture (string ftype, int id, Vector3 location, Quaternion rotation, Vector3 scale) {
			//Verify that the furniture is valid
			if (!VerifyFurniture (ftype)) {
				Debug.Log ("Failed to find valid furniture");
				return null;
			} else {
				if (PhotonNetwork.inRoom && gameObject.GetComponent<PhotonView> ().viewID > 0) {
					Debug.Log ("Calling Connected Function");
					gameObject.GetComponent<PhotonView> ().RPC ("PUNMakeFurniture", PhotonTargets.AllBuffered, FurnitureCatalog[ftype], ftype, id, location, rotation, scale);
					if (gameObject.GetComponent<MeshRenderer> () == null) {
						return null;
					} else {
						return parameters;
					}
				}

				//If the furniture is valid
				parameters = FurnitureCatalog [ftype];

				type = ftype;
				ID = id;

				gameObject.name = type + ID.ToString ();
				gameObject.AddComponent<MeshFilter> ();
				gameObject.AddComponent<MeshRenderer> ();

				// TODO: Add Physics collision to object
				// furniture.gameObject.AddComponent<MeshCollider> ();
				Mesh[] meshes = GetSTL ();
				if (meshes == null) {
					Debug.Log ("Failed to get mesh");
					return null;
				} else {
					gameObject.GetComponent<MeshFilter>().mesh = meshes [0];
					gameObject.AddComponent<MeshCollider> ();
					gameObject.GetComponent<MeshCollider> ().sharedMesh = meshes [0];
					gameObject.GetComponent<MeshCollider> ().convex = true;
					gameObject.GetComponent<MeshCollider> ().sharedMaterial = Resources.Load ("Assets/FARVR/Prefabs/FurniturePhy") as PhysicMaterial;
					gameObject.GetComponent<MeshCollider> ().cookingOptions = MeshColliderCookingOptions.WeldColocatedVertices;
					Renderer rend = gameObject.GetComponent<Renderer> ();
					rend.material = MaterialPrefab;

					/*  Transform the furniture based on provided parameters */
					TransformFurniture (location, rotation, scale);

					/* Update: Added a Collider tag that allows us to distinguish whether an Existing Furniture was hit or not */
					gameObject.tag = "Furniture";
				}
			}
			return parameters;
		}

		// The following are overloaded functions that act as an alternative to default parameters
		// Optional rotation AND scale
		public Dictionary<string, float> MakeFurniture(string ftype, int id, Vector3 location) {
			if (PhotonNetwork.inRoom && gameObject.GetComponent<PhotonView> ().viewID > 0) {
				Debug.Log ("Calling Connected Function");
				gameObject.GetComponent<PhotonView> ().RPC ("PUNMakeFurniture", PhotonTargets.AllBufferedViaServer, FurnitureCatalog[ftype], ftype, id, location, Quaternion.identity, new Vector3(1f, 1f, 1f));
				if (gameObject.GetComponent<MeshFilter> () == null) {
					return null;
				} else {
					return parameters;
				}
			} else {
				return MakeFurniture (ftype, id, location, Quaternion.identity, new Vector3 (1f, 1f, 1f));
			}
		}

		// Optional rotation
		public Dictionary<string, float> MakeFurniture(string ftype, int id, Vector3 location, Vector3 scale) {
			if (PhotonNetwork.inRoom && gameObject.GetComponent<PhotonView> ().viewID > 0) {
				Debug.Log ("Calling Connected Function");
				gameObject.GetComponent<PhotonView> ().RPC ("PUNMakeFurniture", PhotonTargets.AllBufferedViaServer, FurnitureCatalog [ftype], ftype, id, location, Quaternion.identity, scale);
				if (gameObject.GetComponent<MeshFilter> () == null) {
					return null;
				} else {
					return parameters;
				}
			} else {
				return MakeFurniture (ftype, id, location, Quaternion.identity, scale);
			}
		}

		// Optional scale
		public Dictionary<string, float> MakeFurniture(string ftype, int id, Vector3 location, Quaternion rotate) {
			if (PhotonNetwork.inRoom && gameObject.GetComponent<PhotonView> ().viewID > 0) {
				Debug.Log ("Calling Connected Function");
				gameObject.GetComponent<PhotonView> ().RPC ("PUNMakeFurniture", PhotonTargets.AllBufferedViaServer, FurnitureCatalog [ftype], ftype, id, location, rotate, new Vector3 (1f, 1f, 1f));
				if (gameObject.GetComponent<MeshFilter> () == null) {
					return null;
				} else {
					return parameters;
				}
			} else {
				return MakeFurniture (ftype, id, location, rotate, new Vector3 (1f, 1f, 1f));
			}
		}
			
		/// <summary>
		/// Updates the furniture. Returns the corresponding mesh of the object and updates the furniture object.
		/// </summary>
		/// <returns>RETURN CODE: 0 = Successfully updated furniture; 1 = Failed to retrieve mesh</returns>
		/// <param name="local">Local parameters of the given furniture.</param>
		/// <param name="Furniture">The GameObject corresponding to the furniture</param>
		public int UpdateFurniture(Dictionary<string, float> localparameters) 
        { 
			if (PhotonNetwork.inRoom && gameObject.GetComponent<PhotonView>().viewID > 0) {
				Debug.Log ("Calling Connected Function");
				gameObject.GetComponent<PhotonView> ().RPC ("PUNUpdateFurniture", PhotonTargets.AllBufferedViaServer, localparameters);
				/* Because we can't check anything in the function, we assume it is successful */
				return 0;
			}
			parameters = localparameters;
            // Assuming we need to update the furniture
			Mesh[] meshes = GetSTL ();
			meshes [0].RecalculateNormals ();
			if (meshes == null) {
				Debug.Log ("Failed to get mesh");
				return 1;
			} else {
				gameObject.GetComponent<MeshFilter> ().mesh = meshes [0];
				gameObject.GetComponent<MeshCollider> ().sharedMesh = meshes [0];
				gameObject.GetComponent<MeshCollider> ().convex = true;
				gameObject.GetComponent<MeshCollider> ().sharedMaterial = Resources.Load ("Assets/FARVR/Prefabs/FurniturePhy") as PhysicMaterial;
				gameObject.GetComponent<MeshCollider> ().cookingOptions = MeshColliderCookingOptions.WeldColocatedVertices;

				MeshRenderer rend = gameObject.GetComponent<MeshRenderer> ();
				rend.material = MaterialPrefab;
			}
			return 0;
        }

		[PunRPC]
		void PUNUpdateFurniture(Dictionary<string, float> localparameters) {
			parameters = localparameters;
			// Assuming we need to update the furniture
			Mesh[] meshes = GetSTL ();
			meshes [0].RecalculateNormals ();
			if (meshes == null) {
				Debug.Log ("Failed to get mesh");
			} else {
				gameObject.GetComponent<MeshFilter> ().mesh = meshes [0];
				gameObject.GetComponent<MeshCollider> ().sharedMesh = meshes [0];
				gameObject.GetComponent<MeshCollider> ().convex = true;
				gameObject.GetComponent<MeshCollider> ().sharedMaterial = Resources.Load ("Assets/FARVR/Prefabs/FurniturePhy") as PhysicMaterial;
				gameObject.GetComponent<MeshCollider> ().cookingOptions = MeshColliderCookingOptions.WeldColocatedVertices;

				MeshRenderer rend = gameObject.GetComponent<MeshRenderer> ();
				rend.material = MaterialPrefab;
			}
		}

		/// <summary>
		/// Transforms the furniture.
		/// </summary>
		/// <returns>RETURN CODE: 0 = Successful update of transformation; 1 = Invalid parameters</returns>
		/// <param name="translate">Vector3 for new object position.</param>
		/// <param name="rotate">Quarternion rotation for new object rotation.</param>
		/// <param name="scale">Vector3 for new object localscale.</param>
		public int TransformFurniture(Vector3 translate, Quaternion rotate, Vector3 scale) {
			if (PhotonNetwork.inRoom && gameObject.GetComponent<PhotonView> ().viewID > 0) {
				Debug.Log ("Calling Connected Function");
				gameObject.GetComponent<PhotonView> ().RPC ("PUNTransformFurniture", PhotonTargets.AllBufferedViaServer, translate, rotate, scale);
				return 0;
			}
			if (translate != gameObject.transform.position) {
				gameObject.transform.position = translate;
			}

			if (rotate != gameObject.transform.rotation) {
				gameObject.transform.rotation = rotate;
			}

			Vector3 scaler = new Vector3 (0.01f * scale.x, 0.01f * scale.y, 0.01f * scale.z);

			if (scaler != gameObject.transform.localScale) {
				gameObject.transform.localScale = scaler;
			}

			return 0;
		}

		[PunRPC]
		void PUNTransformFurniture(Vector3 translate, Quaternion rotate, Vector3 scale) {
			if (translate != gameObject.transform.position) {
				gameObject.transform.position = Vector3.Lerp (gameObject.transform.position, translate, 1.0f);
			}

			if (rotate != gameObject.transform.rotation) {
				gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, rotate, 1.0f);
			}

			Vector3 scaler = new Vector3 (0.01f * scale.x, 0.01f * scale.y, 0.01f * scale.z);

			if (scaler != gameObject.transform.localScale) {
				gameObject.transform.localScale = scaler;
			}
		}


		// To obtain transformations of furniture
		public Vector3 GetPosition() {
			return gameObject.transform.position;
		}

		public Vector3 GetScale() {
			Vector3 scale = gameObject.transform.localScale;
			return (new Vector3 (scale.x / 0.01f, scale.y / 0.01f, scale.z / 0.01f));
		}

		public Quaternion GetRotation() {
			return gameObject.transform.rotation;
		}

		public string GetID() {
			return (gameObject.name);
		}

		public Dictionary<string, float> GetParameters() {
			return parameters;
		}

		public string GetFType() {
			return type;
		}

		public Dictionary<string, float> GetDefaults(string ftype) {
			return FurnitureCatalog [ftype];
		}

		public GameObject GetFurniture() {
			return gameObject;
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

		public void RemoveFurniture() {
			Destroy(gameObject);
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
			foreach (KeyValuePair<string, Dictionary<string, float>> entry in FurnitureCatalog)  {
				if (entry.Key == type) {
					parameters = entry.Value;
					return VerifyFurniture(type, parameters);
				}
			}

			return false;
		}

		// The single function we will use to get an STL binary file
		private Mesh[] GetSTL(){
		//Store Mesh
		Mesh[] holder = null;

		/* If you wish to use a local server, please use the localhost url; otherwise, please use the given url */
		string url = "http://ayeaye.ee.ucla.edu:5000/{0}.stl?{1}";
		//string url = "http://localhost:5000/{0}.stl?{1}";

		//Stage 1: Get the STL Binary from the server
		string param = LinkParam(parameters);

		url = string.Format(url, type, param);

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

			garr [0] = gameObject;

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

			Vector3[] vertices = meshes [0].vertices;
			Vector2[] UVArr = new Vector2[vertices.Length];
			for (int i = 0; i < UVArr.Length; i++) {
				UVArr [i] = new Vector2 (vertices [i].x * 0.01f, vertices [i].z * 0.01f);
			}

			meshes [0].uv = UVArr;

			meshes [0].RecalculateNormals ();
			meshes [0].RecalculateBounds ();

			// In this case, the importer automatically merges the meshes together, so the only mesh in the array is at index 0
			return meshes;
		}

		// A function used to produce the strings for the parameters for the url
		private string LinkParam(Dictionary<string, float> parameters) {
			string result = "";

			foreach(KeyValuePair<string, float> entry in parameters) {
				// Case where we have to input a discrete value
				if (entry.Key == "legs" || entry.Key == "angle") {
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


