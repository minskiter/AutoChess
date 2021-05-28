using UnityEngine;
using System.Collections;
using UnityEditor;

public class ZST_Editor : MonoBehaviour {

	[UnityEditor.Callbacks.DidReloadScripts]
	private static void OnDidReloadScripts() {
		ZST_MapManager.SharedInstance.HardReloadTiles();
	}
}
