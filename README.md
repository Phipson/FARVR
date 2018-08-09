# FARVR
By LEMUR at UCLA (Cheuk Yin Phipson Lee and Qiong (June) Hu)

The following is a repository containing a library used to optimize and simplify the manufacturing process of furniture design. FARVR is a furniture design API made by undergraduates at the UCLA LEMUR Research Lab. Beginning on June 2018, we have implemented this API to interact with furniture on Augmented Reality and Virtual Reality utilities on Unity. The ultimate goal is to design a collaborative Mixed Reality environment where multiple designers can interact in a common space and design manufacturable furniture together.

## Table of Content
1. **Software Requirements**
2. **Installation Process**
3. **Documentation of API**
	1. The Furniture Class
	2. Furniture Objects
	3. Creating a Furniture
	4. Updating a Furniture
	5. Displaying Information about a Furniture
	6. Deleting a Furniture
	7. Exporting a Furniture
	8. Transforming a Furniture
	9. Adding New Furniture Types
4. **Example Scenes and Reference Material**
5. **Update Log and Current Work in Progress**

### Software Requirements
The original Furniture API was developed using Unity 2017 f2.0b. The API has also been tested on Unity 2018 f2.2b, Unity 2017 f4.3b and Unity 2017 f3.0b. We have developed it such that it should be compatible with all versions of Unity with a minimum requirement of 2017 2.0+. If there are any known issues with the API, please report the error with details about the **Hardware and Software that you are using, as well as the error message and context in which this error occurs** at lemurarvrfurniture@gmail.com. 

If you want to make this project accessible across all different versions of Unity, please refer to the [Unity Hub](https://blogs.unity3d.com/2018/01/24/streamline-your-workflow-introducing-unity-hub-beta/) for documentation.

### Installation Process
By default, the Furniture API has been implemented as a Unity Asset, so to access the API as an asset:

1. Create a Unity Project (or open an existing Unity Project) that has the minimum software requirements.
2. Download the Furniture_API Folder from this Github Repository.
3. Drag the nested "FARVR" Folder into the Assets folder of your Unity Project. If you have issues finding the location of your Assets folder, simply right click the "Assets" folder inside your Unity Editor, and click on the option that says "Reveal in folder" (or "Reveal in Finder", depending on your Operation System).
4. Once the folder has been copied into the Assets, Unity will take some time to import packages. Let it run and update the Assets before proceeding.
5. Ignore any warnings that may occur. If there are any errors with the file, it is best to reload the package by saving the project and then restarting Unity. This will allow Unity to link all the dlls and recompile the Asset library.
6. To use the Unity project, create an Empty GameObject and drag the corresponding CreateFurniture.cs script into it as a component, found in our **Scripts** folder.
7. If you are struggling to figure out how the API works, you can also choose to duplicate our existing **APIScene** found in our **Scenes** folder.
8. In the **Furniture.cs** file located in our **Script** folder, on line 330 to 331 (in the function **GetSTL()**), change the **url** according to whether you are running your own server, or whether you are running the project using the given *ayeaye* server.
	1. If you choose to run the project using a **localhost** server, please use `string url = "http://localhost:5000/{0}.stl?{1}";`
	2. Otherwise, please use `string url = "http://ayeaye.ee.ucla.edu:5000/{0}.stl?{1}";`

**For Users Using our APIScene:**
1. In our GameObject named **FurnitureAPI**, you may find that there is a missing script. Please drag the script **CreateFurniture.cs** from our **Scripts** folder to replace the missing script. For now, drag the sliders from the **Canvas** GameObject into the corresponding inputs for the script, and then drag the **Furniture.prefab** object from our **Prefabs** folder into the supplied Prefab object.

2. In the **Furniture.prefab** object, you may also find that there is a missing script. Please drag the **Furniture.cs** script from our **Scripts** folder to replace the missing script. Once you do so, you will be asked to also provide a Prefab. In this case, drag the **DisplayObject.prefab** object into that Prefab box.

Once you do that, you should be able to run our APIScene.

### Documentation of API
The following documentation is exclusive to Unity, and is written in C# code. We currently do not have any plans for designing the API across other game engines such as Unreal, but may look to do so if there are sufficient demands for it in future.

#### The Furniture Class
The entire Furniture API is built using the **Furniture.cs**, found in our **Scripts** folder. It employs a namespace known as FARVR, which distinguishes it from other classes that may use similar names. To use the class in a script, please use the following syntax

`using FARVR;`

at the top of your file. This will enable your script to recognize the furniture class from the .cs script we have provided.

The following diagram illustrates the lifecycle of a Furniture object, and how data is manipulated. Details will be provided for each stage in the following subsections.

![Lifecycle of Furniture Object](https://github.com/Phipson/FARVR/blob/master/FurnitureLifeCycle.png)

The furniture object is first created under a Prefab known as Furniture.prefab, found in our **Prefabs** folder. From there, users can manually retrieve a Furniture object from that GameObject, which allows them to construct a Furniture using the `MakeFurniture()` function. As they are manipulating the furniture, they can get instant, real-time feedback of their modifications using the `UpdateFurniture()` function, which retrieves data from a generative backend compiler that passes an STL binary to the Unity environment. If users wish to move the furniture, while also preserving the internal parameters of the object, they can call the function `TransformFurniture()`, which translates, rotates, and scales the furniture according to the user's liking.

Once furnitures have been defined, users are also free to obtain and visualize the Furniture's parameters using the `Display()` function. If users are satisfied with the Furniture, they can proceed to manufacture it by calling the `Export()` function, which saves the STL binary data as an STL file stored in the application. By default, STL files have become the standard for Computer-Aided Manufacturing, which is why the produced file will be compatible for most manufacturing utilities such as 3D Printers.

**Note:** It is the user's responsibility to determine **when** to update the furniture. Additionally, while this API provides mechanisms to pass a unique ID to the furniture, users have to define the ID and the naming system. 

To control what types of Furnitures that can be generated, please refer to **3.i. Furniture Objects**. To request for a Furniture object, please email lemurarvrfurniture@gmail.com with the following details: **1. The type of furniture you wish to make; 2. The way you would like to define that furniture; 3. Examples of this furniture that we can use as a reference point.**

----

#### Furniture Objects
Upon creation, each Furniture object consists of the following private subcomponents:

Name  | Type | Description
--------------- | -------------- | ------------------------
type  | String | A string used to distinguish the type of furniture that is generated (e.g. stools, tables, shelves, etc.). Used with **ID** to distinguish the furniture object and assign it a name.
ID  | int | An integer value that is concatenated with the string to display a unique furniture identifier when a furniture is created. Used with **type** to generate a unique ID for the furniture, and distinguish furnitures of the same **type**.
parameters | Dictionary<string, float> | A dictionary that holds all the parameter names and corresponding parameter values used to define the particular type of furniture, as specified by **type**. The string key holds the parameter name, and the float value holds the corresponding decimal value of a given parameter. Used to obtain and request an STL file that holds the furniture.
DisplayObject | GameObject | The virtual object that Unity uses to display a mesh. By default, the DisplayObject will hold a mesh and render the corresponding furniture STL file that we will retrieve using the parameters and type. It will also define the collision and physics interactions with other objects in the virtual environment to simulate a real furniture.
FurnitureCatalog | Dictionary<string, Dictionary<string, float>> | A private, constant Dictionary that defines what types of furnitures we can generate and produce. It acts as a sanity test to verify that all the **parameters** passed by the user is valid and meets the requirements of a given furniture type, specified by the string **type**. The string key holds the name of the furniture **type**, and the Dictionary value holds the corresponding **parameters** that can be used to generate a valid furniture of that type. The **parameters** are initialized to default values, so they cannot be changed upon creation.
Prefab | Object | A template GameObject used to propagate the **DisplayObject** of the Furniture

The **type** and **ID** are used together to distinguish multiple Furnitures from each other. All parameters of a Furniture at a given instance is held by the **parameters** and is verified using the **FurnitureCatalog**. Using the **parameters** we display the **DisplayObject** in our environment. Due to Unity's syntax, **DisplayObject** has to be instantiated using **Prefab**.

In detail, the **DisplayObject** holds the following components:

Name | Description
--------------- | --------------
MeshFilter | Used to hold the geometric values of the mesh that we produce using the STL file
MeshRenderer | Used to render the mesh with a set of material and UV Mapping
RigidBody | Simulate the physics of the Furniture
MeshCollider | Takes the mesh from our **MeshFilter** and produces a collision object that detects when the Furniture hits a virtual object

Additionally, the **FurnitureCatalog** currently supports the following Furniture objects:

Name | Parameters : Default Value
---- | ----------
stool | height : 10; radius : 20; legs : 2

----

#### Creating a Furniture
The following two overloaded functions are used to generate a Furniture:

`public int MakeFurniture(Dictionary<string, float> localparameters, string ftype, int id, Vector3 location, Quaternion rotation = DisplayObject.transform.rotation, Vector3 scale = DisplayObject.transform.localScale)`

**Definition**: A call to make a furniture with user-defined parameters. Generates a GameObject that holds the mesh of the furniture with corresponding physics interactions and collision detection.

Return Code | Description
----------- | -----------
0 | Successful creation of Furniture
1 | Failed to retrieve mesh from server
2 | Invalid parameters

Parameter | Type | Description
--------- | ---- | -----------
localparameters | Dictinoary<string, float> | Parameters passed by the user to generate a specified furniture
ftype | string | A string that holds the **type** of Furniture that the user wants to generate
id | int | An identifier that the user can define to distinguish this particular Furniture
location | Vector3 | Specifies the location at which the Furniture will spawn
rotation *(optional)* | Quaternion | Specifies the rotation of the Furniture when it spawns; Default is Quaternion Identity (1, 1, 1, 1)
scale *(optional)* | Vector3 | Specifies the scale of the Furniture when it spawns; Default is (1f, 1f, 1f)

`public Dictionary<string, float> MakeFurniture (string ftype, int id, Vector3 location, Quaternion rotation = DisplayObject.transform.rotation, Vector3 scale = DisplayObject.transform.localScale)`

**Definition**: A call to make a furniture without user-defined parameters. By default, the Furniture will consult the **FurnitureCatalog** for the default values, and generates an object using those default values. Generates a GameObject that holds the mesh of the furniture with corresponding physics interactions and collision detection. The parameters that were used are returned as a Dictionary<string, float>, which the user can reference and make modifications to.


Return Code | Description
----------- | -----------
null | Failed to retrieve mesh from server
valid Dictionary<string, float> | Successful retrieval of furniture object

Parameter | Type | Description
--------- | ---- | -----------
ftype | string | A string that holds the **type** of Furniture that the user wants to generate
id | int | An identifier that the user can define to distinguish this particular Furniture
location | Vector3 | Specifies the location at which the Furniture will spawn
rotation *(optional)* | Quaternion | Specifies the rotation of the Furniture when it spawns; Default is Quaternion Identity (1, 1, 1, 1)
scale *(optional)* | Vector3 | Specifies the scale of the Furniture when it spawns; Default is (0.01f, 0.01f, 0.01f)

In both cases, the inherent code is identical, but the parameters and return results are different. The first iteration of MakeFurniture allows users to have more fine-grained control of the Furniture parameters when they create a Furniture. The second method of MakeFurniture simplifies the process by creating a default Furniture first, and then passing the corresponding values that users can modify later. The second method is preferred only when users have no idea what type of furniture they want to make. 

Unlike a typical constructor that one would call in object-oriented programming, to create a furniture in your actual script, you have to follow the steps below:

1. Instantiate a GameObject in your MonoBehavior script using our Furniture.prefab as a reference. You can do this either directly dragging the .prefab file into the script by setting up a public UnityEnginer.Object variable, or use the following:

```
public UnityEngine.Object Prefab;

void Awake() {
	Prefab = Resources.Load("path/to/prefab");
}
```

After loading the prefab, you can instantiate it using the following code:

`GameObject furniture_object = Instantiate(Prefab) as GameObject;`

This will allow you to link the prefab to a predefined object that we have made for you. The prefab has no influence on the display of the object and is only used as a template for creating the furniture. Hence, you do not need to modify it.

2. Extract the Furniture script component that we have attached to the provided prefab. This can be done using the following code  (assuming you have already included the FARVR namespace):

`Furniture stool = furniture_object.GetComponent<Furniture>();`

3. Now call the `FARVR.Furniture.MakeFurniture()` function. You may need to define the parameters, type and id ahead of time.

```
stool.MakeFurniture("stool", 1);

//or

stool.MakeFurniture(stoolparameters, "stool", 1);

/* Note that making multiple furnitures in one single furniture class, while it is legal, is not suggested. This would mean that multiple furnitures are sharing the same id and are instantiated in the same location */
```

----

#### Updating a Furniture
Furnitures can only be updated once it has been created. To update the furniture, users must call the following function:

`public int UpdateFurniture(Dictionary<string, float> localparameters)`

**Definition**: A call to update an existing Furniture object. Users must pass parameters that match with the existing **type** of furniture that has been created. The **parameters** will be updated to the **local** argument, and a new mesh will be generated corresponding to the new Furniture object.

Return Code | Description
----------- | -----------
0 | Successful creation of Furniture
1 | Failed to retrieve mesh from server

Parameter | Type | Description
--------- | ---- | -----------
localparameters | Dictinoary<string, float> | Parameters passed by the user to update **parameters** of a specified furniture

Update furniture can be called only after a furniture object has been created. In this case, the new parameters will be stored as the old parameters, if and only if there are differences between the existing parameters and the new parameters. If there is an error, or if the parameters have not changed, no update will be made.

Similar to our previous instance of `MakeFurniture()`, you can call `FARVR.Furniture.UpdateFurniture()` using the following syntax:

```
/* Either in your start or update function */

stool.UpdateParameter(newstoolparameters);
```

----

#### Displaying information about a Furniture
Furniture information can be accessed using the `FARVR.Furniture.Display()` Function, which logs all the information regarding the furniture object.

`public void Display();`

**Definition**: A call to display all information about the current Furniture, include its real-world coordinates, the local parameters, the **type** and the **ID**. Currently logs the information in the console, but will look to display it as text later on.

Similar to before, you can call `FARVR.Furniture.Display()` as shown:

```
/* Either in your start or update function */

stool.Display();
```

Display can only be called after a furniture has been created. It cannot be called once the furniture has been destroyed.

----

#### Deleting a Furniture
Because the Furniture is an object, by default, users simply have to delete the furniture by calling the following function:

`Destroy(FurnitureObject);`

This permanently deletes the instance of the furniture from the game. For more information, see [the Unity Documentation](https://docs.unity3d.com/ScriptReference/Object.Destroy.html).

Destroy can only be called after a Furniture has been created.

----

#### Exporting a Furniture
To export a furniture, simply call the function `FARVR.Furniture.Export()`, which saves the resulting mesh as an STL file. By default, the STL file is saved in the folder at which the application's cache or downloaded items are stored. For more information, see [Unity Application DataPath](https://docs.unity3d.com/ScriptReference/Application-dataPath.html).

`public int Export()`

**Definition**: Takes all existing **parameters** in the Furniture object and then sends it over to the server to generate an STL file. The bytes from the STL file will then be transferred into a .stl file that is saved with the application path.

Return Code | Description
----------- | -----------
0 | Successful export of Furniture
1 | Failed to retrieve STL from server

Similar to before, you can call `FARVR.Furniture.Export()` as shown:

```
/* Either in your start or update function */

stool.Export();
```

Export can only be called after a furniture has been created. It cannot be called once the furniture has been destroyed.

----

### Transforming a Furniture
To translate, rotate, or scale the furniture, call the function `FARVR.Furniture.TransformFurniture()`, which translates, rotates, and scales the **DisplayObject** all at once. 

`public int TransformFurniture(Vector3 translate, Quaternion rotate, Vector3 scale)`

**Definition**: Translates, rotates, and/or scales the **DisplayObject** that visualizes the Furniture object. Ignores any transformations if they are identical to the corresponding transformation of the current Furniture.

Return Code | Description
----------- | -----------
0 | Successful transformation
1 | Invalid parameters

Parameter | Type | Description
--------- | ---- | -----------
location | Vector3 | Specifies the location at which the Furniture will spawn
rotation | Quaternion | Specifies the rotation of the Furniture when it spawns
scale | Vector3 | Specifies the scale of the Furniture when it spawns

Corresponding to this function, we also offer additional functions to get the current Position, Rotation, and/or Scale of the Furniture. This can be accessed as follows:

`public Vector3 GetPosition()`

**Definition**: Returns the current position of the Furniture using a Vector3, as represented by **DisplayObject**.

`public Vector3 GetScale()`

**Definition**: Returns the current scale of the Furniture using a Vector3, as represented by **DisplayObject**.

`public Quaternion GetRotation()`

**Definition**: Returns the current rotation of the Furniture using a Quaternion, as represented by **DisplayObject**.

Unlike `MakeFurniture()`, `TransformFurniture()` **_requires_** users to specify all 3 forms of transformations of the **DisplayObject**. In the case where users wish to only manipulate one or some of the arguments, they can pass the current transformation of the Furniture as follows:

```
Furniture FurnitureObj;

/* Initialize the Furniture and update it as needed */

FurnitureObj.TransformFurniture(FurnitureObj.GetPosition(), FurnitureObj.GetRotation(), FurnitureObj.GetScale());
```

----

### Adding New Furniture Types/Modifying the Furniture Catalog
To add new Furniture types, users have to manually update and modify the **FurnitureCatalog** that is found in our **Furniture.cs** class. Please note that this will only work provided the server a) recognizes the Furniture **type**, b) has exactly the paramaeters it needs from **parameters** and c) the parameters fit a certain range and value that is defined by the compiler.

To add an entry in the furniture type, simply follow the following format:

```
/* This is existing code in the Furniture class */
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

		}}, /* Add a comma for every new entry you want to add */
	{"Name_of_type_of_furniture", new Dictionary<string, float>() {
	/* Input all viable parameters and values here */
		}} /* etc. */
};
```

Once this has been added, all new Furnitures should be identifiable through the FurnitureCatalog.

### Example Scenes and Reference Material
To play around with the server and understand how STL files are generated through the compiler, please refer to [oldroco](https://git.uclalemur.com/mehtank/oldroco). You can also check out its implementation on Unity using the **ServerScene** found in the **Scenes** folder of our API.

To play around with the Furniture API, please refer to the **APIScene** found in the **Scenes** folder of our API.

A thorough demonstration of the API at work is shown through [the following youtube video](https://www.youtube.com/watch?v=QbHisCfoSfE&feature=youtu.be).

**_If you have video demonstrations or examples implementing this API or library, please send us your videos or work at lemurarvrfurniture@gmail.com. We would love to hear from you!_**

### Update Log and Current Work in Progress
- [x] Designed and documented preliminary API
- [x] Tested API in VR Environment
- [ ] Tested API in AR Environment
- [ ] Implemented Materials and Rendering Mesh into Furniture
- [x] Generate Exit Codes that detect and handle errors
- [x] Offer example scenes for users
- [x] Offer transformation functions for moving, rotating, and scaling GameObject
