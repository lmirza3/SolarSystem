﻿/// Sample Code for CS 491 Virtual And Augmented Reality Course - Fall 2017
/// written by Andy Johnson
/// 
/// makes use of various textures from the celestia motherlode - http://www.celestiamotherlode.net/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Valve.VR;


public class Planets : MonoBehaviour
{

    float panelHeight = 0.1F;
    float panelWidth = 30.0F;
    float panelDepth = 0.1F;

    float orbitWidth = 0.01F;
    float habWidth = 0.03F;

    float revolutionSpeed = 0.2F;

    float panelXScale = 2.0F;
    float orbitXScale = 2.0F;

	SteamVR_TrackedObject obj;
	public GameObject main;
	public bool buttonEnabled;

	GameObject allCenter;

	bool show3dGlobal;

	float speedCounter;
	float planetScaleCounter;
	float orbitScaleCounter;

	string[] sol = new string[5] { "695500", "Our Sun", "sol", "G2V" , "1.0"};

	string[,] solPlanets = new string[8, 5] {
		{   "57910000",  "2440",    "0.24", "mercury", "mercury" },
		{  "108200000",  "6052",    "0.62", "venus",   "venus" },
		{  "149600000",  "6371",    "1.00", "earthmap", "earth" },
		{  "227900000",  "3400",    "1.88", "mars",     "mars" },
		{  "778500000", "69911",   "11.86", "jupiter", "jupiter" },
		{ "1433000000", "58232",   "29.46", "saturn",   "saturn" },
		{ "2877000000", "25362",   "84.01", "neptune", "uranus" },
		{ "4503000000", "24622",  "164.80", "uranus", "neptune" }
	};

    //------------------------------------------------------------------------------------//

    void drawOrbit(string orbitName, float orbitRadius, Color orbitColor, float myWidth, GameObject myOrbits)
    {

        GameObject newOrbit;
        GameObject orbits;


        newOrbit = new GameObject(orbitName);
        newOrbit.AddComponent<Circle>();
        newOrbit.AddComponent<LineRenderer>();

        newOrbit.GetComponent<Circle>().xradius = orbitRadius;
        newOrbit.GetComponent<Circle>().yradius = orbitRadius;

        var line = newOrbit.GetComponent<LineRenderer>();
        line.startWidth = myWidth;
        line.endWidth = myWidth;
        line.useWorldSpace = false;

        newOrbit.GetComponent<LineRenderer>().material.color = orbitColor;

        orbits = myOrbits;
        newOrbit.transform.parent = orbits.transform;


    }

	// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - //
	void SolarSystemPlanets(string [,] planets, GameObject thesePlanets, GameObject theseOrbits){
		GameObject newPlanetCenter;
		GameObject newPlanet;

		GameObject sunRelated;

		Material planetMaterial;

		int planetCounter;

		for (planetCounter = 0; planetCounter < planets.GetLength(0); planetCounter++) {

			float planetDistance = float.Parse (planets [planetCounter, 0]) / 149600000.0F * 10.0F;
			float planetSize = float.Parse (planets [planetCounter, 1]) * 2.0F / 10000.0F;
			float planetSpeed = -1.0F / float.Parse (planets [planetCounter, 2]) * revolutionSpeed;
			string textureName = planets [planetCounter, 3];
			string planetName = planets [planetCounter, 4];

			newPlanetCenter = new GameObject (planetName + "Center");
			newPlanetCenter.AddComponent<rotate> ();

			newPlanet = GameObject.CreatePrimitive (PrimitiveType.Sphere);
			newPlanet.name = planetName;
			newPlanet.transform.position = new Vector3 (0, 0, planetDistance * orbitXScale);
			newPlanet.transform.localScale = new Vector3 (planetSize, planetSize, planetSize);
			newPlanet.transform.parent = newPlanetCenter.transform;

			newPlanetCenter.GetComponent<rotate> ().rotateSpeed = planetSpeed; 

			planetMaterial = new Material (Shader.Find ("Standard"));
			newPlanet.GetComponent<MeshRenderer> ().material = planetMaterial;
			planetMaterial.mainTexture = Resources.Load (textureName) as Texture;

			drawOrbit (planetName + " orbit", planetDistance * orbitXScale, Color.white, orbitWidth, theseOrbits);

			sunRelated = thesePlanets;
			newPlanetCenter.transform.parent = sunRelated.transform;
		}
	}

	void SolarSystemStar(string [] star, GameObject thisStar, GameObject theseOrbits){
		GameObject newSun, upperSun;
		Material sunMaterial;

		GameObject sunRelated;
		GameObject sunSupport;
		GameObject sunText;

		float sunScale = float.Parse(star [0]) / 100000F;
		float centerSunSize = 0.25F;

		// set the habitable zone based on the star's luminosity
		float innerHab = float.Parse (star[4]) * 9.5F;
		float outerHab = float.Parse (star[4]) * 14F;


		newSun = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		newSun.AddComponent<rotate> ();
		newSun.name = star[1];
		newSun.transform.position = new Vector3 (0, 0, 0);
		newSun.transform.localScale = new Vector3 (centerSunSize, centerSunSize, centerSunSize);

		sunRelated = thisStar;

		newSun.GetComponent<rotate> ().rotateSpeed = -0.25F; 

		sunMaterial = new Material (Shader.Find ("Unlit/Texture"));
		newSun.GetComponent<MeshRenderer> ().material = sunMaterial;
		sunMaterial.mainTexture = Resources.Load (star[2]) as Texture;

		newSun.transform.parent = sunRelated.transform;


		// copy the sun and make a bigger version above

		upperSun = Instantiate (newSun);
		upperSun.name = star[1] + " upper";
		upperSun.transform.localScale = new Vector3 (sunScale,sunScale,sunScale);
		upperSun.transform.position = new Vector3 (0, 10, 0);

		upperSun.transform.parent = sunRelated.transform;

		// draw the support between them
		sunSupport = GameObject.CreatePrimitive (PrimitiveType.Cube);
		sunSupport.transform.localScale = new Vector3 (0.1F, 10.0F, 0.1F);
		sunSupport.transform.position = new Vector3 (0, 5, 0);
		sunSupport.name = "Sun Support";

		sunSupport.transform.parent = sunRelated.transform;


		sunText = new GameObject();
		sunText.name = "Star Name";
		sunText.transform.position = new Vector3 (0, 5, 0);
		sunText.transform.localScale = new Vector3 (0.1F, 0.1F, 0.1F);
		var sunTextMesh = sunText.AddComponent<TextMesh>();
		sunTextMesh.text = star[1];
		sunTextMesh.fontSize = 200;

		sunText.transform.parent = sunRelated.transform;


		drawOrbit ("Habitable Inner Ring", innerHab * orbitXScale, Color.green, habWidth, theseOrbits);
		drawOrbit ("Habitable Outer Ring", outerHab * orbitXScale, Color.green, habWidth, theseOrbits);

	}

	void SolarSystemStarSide(string [] star, GameObject thisSide, GameObject theseOrbits){
		GameObject newSidePanel;
		GameObject newSideSun;
		GameObject sideSunText;

		GameObject habZone;

		Material sideSunMaterial, habMaterial;

		newSidePanel = GameObject.CreatePrimitive (PrimitiveType.Cube);
		newSidePanel.name = "Side " + star[1] + " Panel";
		newSidePanel.transform.position = new Vector3 (0, 0, 0);
		newSidePanel.transform.localScale = new Vector3 (panelWidth, panelHeight, panelDepth);
		newSidePanel.transform.parent = thisSide.transform;

		newSideSun = GameObject.CreatePrimitive (PrimitiveType.Cube);
		newSideSun.name = "Side " + star[1] + " Star";
		newSideSun.transform.position = new Vector3 (-0.5F * panelWidth - 0.5F, 0, 0);
		newSideSun.transform.localScale = new Vector3 (1.0F, panelHeight*40.0F, 2.0F * panelDepth);
		newSideSun.transform.parent = thisSide.transform;

		sideSunMaterial = new Material (Shader.Find ("Unlit/Texture"));
		newSideSun.GetComponent<MeshRenderer> ().material = sideSunMaterial;
		sideSunMaterial.mainTexture = Resources.Load (star[2]) as Texture;


		sideSunText = new GameObject();
		sideSunText.name = "Side Star Name";
		sideSunText.transform.position = new Vector3 (-0.47F * panelWidth, 22.0F * panelHeight, 0);
		sideSunText.transform.localScale = new Vector3 (0.1F, 0.1F, 0.1F);
		var sunTextMesh = sideSunText.AddComponent<TextMesh>();
		sunTextMesh.text = star[1];
		sunTextMesh.fontSize = 150;
		sideSunText.transform.parent = thisSide.transform;

		float innerHab = float.Parse (star[4]) * 9.5F;
		float outerHab = float.Parse (star[4]) * 14F;


		// need to take panelXScale into account for the hab zone

		habZone = GameObject.CreatePrimitive (PrimitiveType.Cube);
		habZone.name = "Hab";
		habZone.transform.position = new Vector3 ((-0.5F * panelWidth) + ((innerHab+outerHab) * 0.5F * panelXScale), 0, 0);
		habZone.transform.localScale = new Vector3 ((outerHab - innerHab)* panelXScale, 40.0F * panelHeight, 2.0F * panelDepth);
		habZone.transform.parent = thisSide.transform;

		habMaterial = new Material (Shader.Find ("Standard"));
		habZone.GetComponent<MeshRenderer> ().material = habMaterial;
		habMaterial.mainTexture = Resources.Load ("habitable") as Texture;


	}

	void SolarSystemPlanetsSide(string [,] planets, GameObject thisSide, GameObject theseOrbits){
		GameObject newPlanet;

		GameObject sunRelated;

		Material planetMaterial;

		int planetCounter;

		for (planetCounter = 0; planetCounter < planets.GetLength(0); planetCounter++) {

			float planetDistance = float.Parse (planets [planetCounter, 0]) / 149600000.0F * 10.0F;
			float planetSize = float.Parse (planets [planetCounter, 1]) * 1.0F / 10000.0F;
			string textureName = planets [planetCounter, 3];
			string planetName = planets [planetCounter, 4];

			// limit the planets to the width of the side view
			if ((panelXScale * planetDistance) < panelWidth) {

				newPlanet = GameObject.CreatePrimitive (PrimitiveType.Sphere);
				newPlanet.name = planetName;
				newPlanet.transform.position = new Vector3 (-0.5F * panelWidth + planetDistance * panelXScale, 0, 0);
				newPlanet.transform.localScale = new Vector3 (planetSize, planetSize, 5.0F * panelDepth);

				planetMaterial = new Material (Shader.Find ("Standard"));
				newPlanet.GetComponent<MeshRenderer> ().material = planetMaterial;
				planetMaterial.mainTexture = Resources.Load (textureName) as Texture;

				sunRelated = thisSide;
				newPlanet.transform.parent = sunRelated.transform;
			}
		}
	}

	void SolarSystem(string[] starInfo, string[,] planetInfo, Vector3 offset, GameObject allThings){
		GameObject SolarCenter;
		GameObject AllOrbits;
		GameObject SunStuff;
		GameObject Planets;

		SolarCenter = new GameObject();
		AllOrbits = new GameObject();
		SunStuff = new GameObject();
		Planets = new GameObject();

		SolarCenter.name = "SolarCenter" + " " + starInfo[1];
		AllOrbits.name = "All Orbits" + " " + starInfo[1];
		SunStuff.name = "Sun Stuff" + " " + starInfo[1];
		Planets.name = "Planets" + " " + starInfo[1];

			SolarCenter.transform.parent = allThings.transform;

			AllOrbits.transform.parent = SolarCenter.transform;
			SunStuff.transform.parent = SolarCenter.transform;
			Planets.transform.parent = SolarCenter.transform;

		SolarSystemStar(starInfo, SunStuff, AllOrbits);
		SolarSystemPlanets (planetInfo, Planets, AllOrbits);


		// need to do this last
		SolarCenter.transform.position = offset;


		// add in second 'flat' representation
		GameObject SolarSide;
		SolarSide = new GameObject();
		SolarSide.name = "Side View of" + starInfo[1];

		SolarSystemStarSide (starInfo, SolarSide, AllOrbits);
		SolarSystemPlanetsSide (planetInfo, SolarSide, AllOrbits);

		SolarSide.transform.position = new Vector3 (0, 8, 10.0F);
		SolarSide.transform.position += (offset * 0.15F);


	}

	// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - //

    //------------------------------------------------------------------------------------//

    void dealWithPlanets(string[,] planets, GameObject thesePlanets, GameObject theseOrbits)
    {
        GameObject newPlanetCenter;
        GameObject newPlanet;

        GameObject sunRelated;

        Material planetMaterial;

        int planetCounter;

        for (planetCounter = 0; planetCounter < planets.GetLength(0); planetCounter++)
        {

			float planetDistance = float.Parse(planets[planetCounter, 5]) * 5.0F;
            //Debug.Log(planets[planetCounter, 5]);
            float planetSize = float.Parse(planets[planetCounter, 9]) * 6371.0F *2.0F / 100000.0F;
            float planetSpeed = -1.0F / float.Parse(planets[planetCounter, 4]) / 365.0F * revolutionSpeed;
            string textureName = "mercury";
            string planetName = planets[planetCounter, 1];


            newPlanetCenter = new GameObject(planetName + "Center");
            newPlanetCenter.AddComponent<rotate>();

            newPlanet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            newPlanet.name = planetName;
            newPlanet.transform.position = new Vector3(0, 0, planetDistance * orbitXScale);
            newPlanet.transform.localScale = new Vector3(planetSize, planetSize, planetSize);
            newPlanet.transform.parent = newPlanetCenter.transform;

            newPlanetCenter.GetComponent<rotate>().rotateSpeed = planetSpeed;

            planetMaterial = new Material(Shader.Find("Standard"));
            newPlanet.GetComponent<MeshRenderer>().material = planetMaterial;
            planetMaterial.mainTexture = Resources.Load(textureName) as Texture;

            drawOrbit(planetName + " orbit", planetDistance * orbitXScale, Color.white, orbitWidth, theseOrbits);

            sunRelated = thesePlanets;
            newPlanetCenter.transform.parent = sunRelated.transform;
        }
    }

    //------------------------------------------------------------------------------------//

    void sideDealWithPlanets(string[,] planets, GameObject thisSide, GameObject theseOrbits)
    {
        GameObject newPlanet;

        GameObject sunRelated;

        Material planetMaterial;

        int planetCounter;

        for (planetCounter = 0; planetCounter < planets.GetLength(0); planetCounter++)
        {

            float planetDistance = float.Parse(planets[planetCounter, 5]) * 10.0F;
            float planetSize = float.Parse(planets[planetCounter, 9]) * 6371.0F * 2.0F / 100000.0F;
            string textureName = "mercury";
            string planetName = planets[planetCounter, 1];

            // limit the planets to the width of the side view
            if ((panelXScale * planetDistance) < panelWidth)
            {

                newPlanet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                newPlanet.name = planetName;
                newPlanet.transform.position = new Vector3(-0.5F * panelWidth + planetDistance * panelXScale, 0, 0);
                newPlanet.transform.localScale = new Vector3(planetSize, planetSize, 5.0F * panelDepth);

                planetMaterial = new Material(Shader.Find("Standard"));
                newPlanet.GetComponent<MeshRenderer>().material = planetMaterial;
                planetMaterial.mainTexture = Resources.Load(textureName) as Texture;

                sunRelated = thisSide;
                newPlanet.transform.parent = sunRelated.transform;
            }
        }
    }

    //------------------------------------------------------------------------------------//

    void sideDealWithStar(string[] star, GameObject thisSide, GameObject theseOrbits)
    {
        GameObject newSidePanel;
        GameObject newSideSun;
        GameObject sideSunText;

        GameObject habZone;

        Material sideSunMaterial, habMaterial;

        newSidePanel = GameObject.CreatePrimitive(PrimitiveType.Cube);
        newSidePanel.name = "Side " + star[0] + " Panel";
        newSidePanel.transform.position = new Vector3(0, 0, 0);
        newSidePanel.transform.localScale = new Vector3(panelWidth, panelHeight, panelDepth);
        newSidePanel.transform.parent = thisSide.transform;

        newSideSun = GameObject.CreatePrimitive(PrimitiveType.Cube);
        newSideSun.name = "Side " + star[0] + " Star";
        newSideSun.transform.position = new Vector3(-0.5F * panelWidth - 0.5F, 0, 0);
        newSideSun.transform.localScale = new Vector3(1.0F, panelHeight * 40.0F, 2.0F * panelDepth);
        newSideSun.transform.parent = thisSide.transform;

        sideSunMaterial = new Material(Shader.Find("Unlit/Texture"));
        newSideSun.GetComponent<MeshRenderer>().material = sideSunMaterial;
        sideSunMaterial.mainTexture = Resources.Load("gstar") as Texture;


        sideSunText = new GameObject();
        sideSunText.name = "Side Star Name";
        sideSunText.transform.position = new Vector3(-0.47F * panelWidth, 22.0F * panelHeight, 0);
        sideSunText.transform.localScale = new Vector3(0.1F, 0.1F, 0.1F);
        var sunTextMesh = sideSunText.AddComponent<TextMesh>();
        sunTextMesh.text = star[0];
        sunTextMesh.fontSize = 150;
        sideSunText.transform.parent = thisSide.transform;

        float innerHab = float.Parse(star[11]) * 9.5F;
        float outerHab = float.Parse(star[11]) * 14F;


        // need to take panelXScale into account for the hab zone

        habZone = GameObject.CreatePrimitive(PrimitiveType.Cube);
        habZone.name = "Hab";
        habZone.transform.position = new Vector3((-0.5F * panelWidth) + ((innerHab + outerHab) * 0.5F * panelXScale), 0, 0);
        habZone.transform.localScale = new Vector3((outerHab - innerHab) * panelXScale, 40.0F * panelHeight, 2.0F * panelDepth);
        habZone.transform.parent = thisSide.transform;

        habMaterial = new Material(Shader.Find("Standard"));
        habZone.GetComponent<MeshRenderer>().material = habMaterial;
        habMaterial.mainTexture = Resources.Load("habitable") as Texture;

    }

    //------------------------------------------------------------------------------------//

    void dealWithStar(string[] star, GameObject thisStar, GameObject theseOrbits)
    {

        GameObject newSun, upperSun;
        Material sunMaterial;

        GameObject sunRelated;
        GameObject sunSupport;
        GameObject sunText;

        float sunScale = float.Parse(star[8]) * 695500F / 100000F;
        float centerSunSize = 0.25F;

        // set the habitable zone based on the star's luminosity
        float innerHab = float.Parse(star[11]) * 9.5F;
        float outerHab = float.Parse(star[11]) * 14F;


        newSun = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        newSun.AddComponent<rotate>();
        newSun.name = star[0];
        newSun.transform.position = new Vector3(0, 0, 0);
        newSun.transform.localScale = new Vector3(centerSunSize, centerSunSize, centerSunSize);

        sunRelated = thisStar;

        newSun.GetComponent<rotate>().rotateSpeed = -0.25F;

        sunMaterial = new Material(Shader.Find("Unlit/Texture"));
        newSun.GetComponent<MeshRenderer>().material = sunMaterial;
        sunMaterial.mainTexture = Resources.Load("gstar") as Texture;

        newSun.transform.parent = sunRelated.transform;


        // copy the sun and make a bigger version above

        upperSun = Instantiate(newSun);
        upperSun.name = star[0] + " upper";
        upperSun.transform.localScale = new Vector3(sunScale, sunScale, sunScale);
        upperSun.transform.position = new Vector3(0, 10, 0);

        upperSun.transform.parent = sunRelated.transform;

        // draw the support between them
        sunSupport = GameObject.CreatePrimitive(PrimitiveType.Cube);
        sunSupport.transform.localScale = new Vector3(0.1F, 10.0F, 0.1F);
        sunSupport.transform.position = new Vector3(0, 5, 0);
        sunSupport.name = "Sun Support";

        sunSupport.transform.parent = sunRelated.transform;


        sunText = new GameObject();
        sunText.name = "Star Name";
        sunText.transform.position = new Vector3(0, 5, 0);
        sunText.transform.localScale = new Vector3(0.1F, 0.1F, 0.1F);
        var sunTextMesh = sunText.AddComponent<TextMesh>();
        sunTextMesh.text = star[0];
        sunTextMesh.fontSize = 200;

        sunText.transform.parent = sunRelated.transform;


        drawOrbit("Habitable Inner Ring", innerHab * orbitXScale, Color.green, habWidth, theseOrbits);
        drawOrbit("Habitable Outer Ring", outerHab * orbitXScale, Color.green, habWidth, theseOrbits);
    }

    //------------------------------------------------------------------------------------//

	void dealWithSystem(string[] starInfo, string[,] planetInfo, Vector3 offset, GameObject allThings, bool show3d)
    {

        GameObject SolarCenter;
        GameObject AllOrbits;
        GameObject SunStuff;
        GameObject Planets;

        SolarCenter = new GameObject();
        AllOrbits = new GameObject();
        SunStuff = new GameObject();
        Planets = new GameObject();

        SolarCenter.name = "SolarCenter" + " " + starInfo[1];
        AllOrbits.name = "All Orbits" + " " + starInfo[1];
        SunStuff.name = "Sun Stuff" + " " + starInfo[1];
        Planets.name = "Planets" + " " + starInfo[1];

		if (show3d == true) {
			SolarCenter.transform.parent = allThings.transform;

			AllOrbits.transform.parent = SolarCenter.transform;
			SunStuff.transform.parent = SolarCenter.transform;
			Planets.transform.parent = SolarCenter.transform;

			dealWithStar (starInfo, SunStuff, AllOrbits);
			dealWithPlanets (planetInfo, Planets, AllOrbits);
		}
        // need to do this last
        SolarCenter.transform.position = offset;


        // add in second 'flat' representation
        GameObject SolarSide;
        SolarSide = new GameObject();
        SolarSide.name = "Side View of" + starInfo[1];



        sideDealWithStar(starInfo, SolarSide, AllOrbits);
        sideDealWithPlanets(planetInfo, SolarSide, AllOrbits);

        SolarSide.transform.position = new Vector3(0, 8, 10.0F);
        SolarSide.transform.position += (offset * 0.15F);

    }

    //------------------------------------------------------------------------------------//

//	void Awake() {
//		obj = GetComponent<SteamVR_TrackedObject>();
//		buttonHolder.SetActive(false);
//		buttonEnabled = false;
//
//	}

    void Start()
    {
		allCenter = new GameObject();
		allCenter.name = "all systems";

		var systemOffset = new Vector3(0, 0, 0);
		var oneOffset = new Vector3(0, -30, 0);


//        //string[] sol = new string[5] { "695500", "Our Sun", "sol", "G2V", "1.0" };
//        string[] sol = new string[12] { "Our Sun", "", "", "8", "1", "3", "0", "70","695500","6","G2V", "1.0" };
//
//
//        string[,] solPlanets = new string[8, 12] {
//            {   "","mercury","","","0.0006575","0.38", "","","","0.382", "mercury",  "" },
//            {   "","venus","","","0.00169","0.72", "","","","0.949", "mercury",  "" },
//            {   "","earth","","","0.0027","1.0", "","","","1.0", "mercury",  "" },
//            {   "","mars","","","0.0051","1.52", "","","","0.533", "mercury",  "" },
//            {   "","jupiter","","","0.0324","5.20", "","","","10.97", "mercury",  "" },
//            {   "","saturn","","","0.0807","9.57", "","","","9.14", "mercury",  "" },
//            {   "","uranus","","","0.2301","19.2", "","","","3.98", "mercury",  "" },
//            {   "","neptune","","","0.4515","30.1", "","","","3.864", "mercury",  "" }
//        };

        /*
            {  "108200000",  "6052",    "0.62", "venus",   "venus" },
            {  "149600000",  "6371",    "1.00", "earthmap", "earth" },
            {  "227900000",  "3400",    "1.88", "mars",     "mars" },
            {  "778500000", "69911",   "11.86", "jupiter", "jupiter" },
            { "1433000000", "58232",   "29.46", "saturn",   "saturn" },
            { "2877000000", "25362",   "84.01", "neptune", "uranus" },
            { "4503000000", "24622",  "164.80", "uranus", "neptune" }
        };

    */




        string[,] solP = new string[259, 12];
        int colIndex = 0;
        int rowIndex = 0;
        string aLine;
        System.IO.StreamReader aFile = new System.IO.StreamReader(@"C:Assets/csv/planets.csv");

        while ((aLine = aFile.ReadLine()) != null)
        {
            colIndex = 0;
            string[] parts = aLine.Split(',');
            foreach (string part in parts)
            {
                if (part != "")
                {
                    solP[rowIndex, colIndex] = part;
                }
                else
                {
                    rowIndex--;
                    // Debug.Log(rowIndex);

                    break;
                }
                ++colIndex;
            }
            ++rowIndex;
        }
        aFile.Close();
        Debug.Log(rowIndex);

       
		SolarSystem(sol, solPlanets, systemOffset, allCenter);
        systemOffset += oneOffset;

		for (int i = 0; i < 258; i++)
        {
            string[] starH = new string[12];
            int amount = int.Parse(solP[i, 3]);
            string[,] sys = new string[amount, 12];
            for (int j = 0; j < amount; j++)
            {
                for (int k = 0; k < 12; k++)
                {
                    sys[j, k] = solP[i + j, k];
                    starH[k] = solP[i, k];

                }
            }

			// if i = n ; enable show3d
       
            dealWithSystem(starH, sys, systemOffset, allCenter, true);
            systemOffset += oneOffset;
            i = i + amount;
        }
			

        allCenter.transform.localScale = new Vector3(0.1F, 0.1F, 0.1F);
    }

	public void updateOrbitScale(float value){
		this.orbitScaleCounter = value;
	}
	public void updatePlanetScale(float value){
		this.planetScaleCounter = value;
	}
	public void updateOrbitSpeed(float value){
		this.speedCounter = value;
	}


    // Update is called once per frame
    void Update()
    {
		// if reset button is clicked
		// reset
//		var device = SteamVR_Controller.Input((int)obj.index);
//
//		if (device.GetPressDown (SteamVR_Controller.ButtonMask.ApplicationMenu)) {
//			if (buttonEnabled == false) {
//				Destroy (allCenter);
//				Start ();
//				buttonHolder.SetActive (true);
//				this.buttonEnabled = true;
//			} else if (buttonEnabled == true) {
//				buttonHolder.SetActive (false);
//				buttonEnabled = false;
//			}
//		}

		// if button is clicked
		// set global counter to appropriate button
		// reset

		// if orbit scale is clicked
		// set counter to multiply by number multiply by amount
		// reset

		// if planet scale is clicked
		// set counter to multiply by number multiply by amount
		// reset

		// if speed scale is clicked
		// set counter to multiply by number multiply by amount
		// reset


    }
}