using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using FARVR;
using UnityEngine.UI;

public class CreateFurniture : MonoBehaviour {

	// A list of available furnitures that are currently in the environment
	List<Furniture> FurnitureList = new List<Furniture>();

    Dictionary<string, float> stoolparam = new Dictionary<string, float>();

	public GameObject prefab;

	private int counter = 0;

	//Slider for modifying stool height
	public Slider Height;

	//Slider for modifying stool radius
	public Slider Radius;

	//Slider for modifying stool legs
	public Slider Legs;

	// Use this for initialization
	void Start () {
		MakeDictionary ();
		Create (stoolparam, "stool", counter++);
	}
	
	// Update is called once per frame
	void Update () {
		if (CheckParam()) {
			FurnitureList [0].UpdateFurniture (stoolparam);
			FurnitureList [0].Display ();
		}
	}

	// Custom function to create an object
	// To be called in the actual script where the object is to be created
	void Create(Dictionary<string, float> parameters, string type, int id) {
		GameObject gameobj = Instantiate (prefab) as GameObject;
		Furniture furnitureobj = gameobj.GetComponent<Furniture> ();
		furnitureobj.MakeFurniture (parameters, type, id);
		furnitureobj.Display ();
		RegisterFurniture (furnitureobj);
	}

	// A subsidary function to log and record all the furnitures currently in the environment
	void RegisterFurniture(Furniture furniture) {
		FurnitureList.Add (furniture);
	}

	// Make fake data for dictionaries
	private void MakeDictionary() {
		stoolparam.Add("height", Height.value);
		stoolparam.Add("legs", Legs.value);
		stoolparam.Add("radius", Radius.value);
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
}
