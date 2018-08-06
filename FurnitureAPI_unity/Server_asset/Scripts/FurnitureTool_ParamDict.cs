using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;

namespace FurnitureTool
{
    public struct Properties
    {
        public Dictionary<string, float> LocalParameters;
        /*
            local parameters for chair: (int)id, heightm length, width, (int)legNum, legLength, topThickness, backHeight;
            local parameters for table: (int)id, heightm length, width, (int)legNum, legLength, topThickness;
            local parameters for shelf: (int)id, heightm length, width, (int)boardNum, boardThickness, sideThickness;
         */

        public string Type;
        /*
            current furniture type: chair, table, shelf
         */

        public int ID;

        public byte[] STLFile;

        public GameObject Displayer;

        public Properties(Dictionary<string, float> loc, string type, int id, byte[] STL, GameObject display)
        {
            LocalParameters = loc;
            Type = type;
            ID = id;
            STLFile = STL;
            Displayer = display;
        }
    };

    public class Furniture : MonoBehaviour
    {
        public Properties Property;

        // Creates a furniture object
        public Furniture(Dictionary<string, float> localparameters, Dictionary<string, float> globalparameters, string type, int id) 
        {
            // Put code here
            GameObject furniture = new GameObject(type);
            furniture.AddComponent<MeshFilter>();
            furniture.AddComponent<MeshRenderer>();
            Instantiate(furniture, new Vector3(globalparameters["x"], globalparameters["y"], globalparameters["z"]), new Quaternion(0, 0, 0, 0));
            Property = new Properties(localparameters, type, id, null, furniture);
        }

        // Creates a furniture object
        public void initialize(Dictionary<string, float> localparameters, Dictionary<string, float> globalparameters, string type, int id)
        {
            // Put code here
            GameObject furniture = new GameObject(type);
            furniture.AddComponent<MeshFilter>();
            furniture.AddComponent<MeshRenderer>();
            Instantiate(furniture, new Vector3(globalparameters["x"], globalparameters["y"], globalparameters["z"]), new Quaternion(0, 0, 0, 0));
            Property = new Properties(localparameters, type, id, null, furniture);
        }


        // Updates the furniture object
        public void UpdateFurniture(Dictionary<string, float> local) 
        { 
            // Assuming we need to update the furniture
            Property.LocalParameters = local;
        }


        // Display all the data about the current furniture
        public void Display()
        { 
            // This should be the same for all of the children, which is why it should be modified here
            // Log the type
            Debug.Log("Type: " + Property.Type);

            // Log the ID
            Debug.Log("ID: " + Property.ID.ToString());

            // Log the local param
            foreach (KeyValuePair<string, float> entry in Property.LocalParameters) {
                Debug.Log("Local Parameter: " + entry.Key + " = " + entry.Value.ToString());
            }
        }

        // Save and export the STL File from the object
        public void Export()
        { 
            // This should be the same for all of the children, which is why it should be modified here 
            // Export GameObject as STLFile
            
        }

        // Delete the current gameobject
        public void Delete()
        {
            // This should be the same for all of the children, which is why it should be modified here
            try
            {
                Destroy(Property.Displayer);
            }
            catch
            {
                Debug.Log("Fail to delete the object.");
            }
        }
    };

    public class Stool : Furniture 
    {
        // Creates a chair object
        public Stool (Dictionary<string, float> localparameters, Dictionary<string, float> globalparameters, string type, int id) : base (localparameters, globalparameters, type, id) 
        {
            Debug.Log("Created stool");
        }

    };

    public class Table : Furniture 
    { 
        // Creates a table object
        public Table (Dictionary<string, float> localparameters, Dictionary<string, float> globalparameters, string type, int id) : base (localparameters, globalparameters, type, id) 
        {
            Debug.Log("Created table");
        }
    };

    public class Shelf : Furniture 
    { 
        // Creates a shelf object
        public Shelf (Dictionary<string, float> localparameters, Dictionary<string, float> globalparameters, string type, int id) : base (localparameters, globalparameters, type, id) 
        {
            Debug.Log("Created shelf");
        }
    }

}


