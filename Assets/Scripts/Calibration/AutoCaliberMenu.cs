﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace VRCalibrationTool
{
    /// <summary>
    /// Takes the coordonnates of the selected object stored in the XML to auto calibrate it.
    /// </summary>
    public class AutoCaliberMenu : MonoBehaviour 
	{
		[SerializeField] private GameObject _buttonPrefab;				//Prefab for the autocalibrate button	
		[SerializeField] private GameObject _panelToAttachButtonsTo;	//Panel to attach the autocalibrate button to

		void Start()
		{
			for (int i = 0; i < XMLManager.ins.itemDB.list.Count; i++) 
			{
				CreateButton (XMLManager.ins.itemDB.list[i].type);
			}
		}

		/// <summary>
		/// If button clicked, autocalibrates the selected object.
		/// </summary>
		void OnClick()
		{
			string objectCalibrateName = EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetComponent<Text>().text;
			GameObject objectCalibrate = (GameObject)Resources.Load (objectCalibrateName);

			ViveControllerManager ViveControllerManager = GameObject.Find ("ViveManager").GetComponent<ViveControllerManager> ();

            ViveControllerManager.CreatePositionTag ();
			ViveControllerManager.CreatePositionTag ();
			ViveControllerManager.CreatePositionTag ();

            int objectCounter = 0;

            if (XMLManager.ins.itemDB.list.Count != 0)
            {
                for (int i = 0; i < XMLManager.ins.itemDB.list.Count; i++)
                {
                    if(XMLManager.ins.itemDB.list[i].type == objectCalibrateName)
                    {
                        objectCounter = i;
                        break;
                    }
                }
            }

            ViveControllerManager._PositionTags [0].transform.position = XMLManager.ins.itemDB.list[objectCounter].point1.Vector3;
			ViveControllerManager._PositionTags [1].transform.position = XMLManager.ins.itemDB.list[objectCounter].point2.Vector3;
			ViveControllerManager._PositionTags [2].transform.position = XMLManager.ins.itemDB.list[objectCounter].point3.Vector3;

			GameObject objectInstantiated = (GameObject) Instantiate (objectCalibrate);

            //objectInstantiated.GetComponent<VirtualObject> ().Calibrate (ViveControllerManager._PositionTags);
            ViveControllerManager.CalibrateVR(objectInstantiated);

			Debug.Log("Object calibrated: "+objectCalibrate.name);
		}

		/// <summary>
		/// Creates an autocalibrate button.
		/// </summary>
		/// <param name="name">Name.</param>
		private void CreateButton(string name) 
		{
			GameObject button = (GameObject)Instantiate(_buttonPrefab);
			button.transform.SetParent(_panelToAttachButtonsTo.transform);
			button.GetComponent<Button>().onClick.AddListener(OnClick);
			button.transform.GetChild(0).GetComponent<Text>().text = name;
		}
	}
}