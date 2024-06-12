using UnityEngine;
using System.Collections;

namespace VolumetricFogAndMist {
				public class DemoSurroundingFog : MonoBehaviour {

								public Camera cam;


								void Update () {
												if (Input.GetKeyDown (KeyCode.C)) {
																cam.enabled = !cam.enabled;
												}
								}
				}
}