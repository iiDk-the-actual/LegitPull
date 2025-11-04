using System;
using System.Collections.Generic;
using BepInEx;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GorillaTag.Pull
{
	[BepInPlugin("org.bablo.gtag.legitpull", "Legit Pull", "1.4.1")]
	public class PulMud : BaseUnityPlugin
	{
		public static void MainEntry()
		{
			PulMud pulMud = FindFirstObjectByType<PulMud>();
			if (pulMud == null)
			{
				GameObject gameObject = new GameObject("DCRunModGO");
				gameObject.AddComponent<PulMud>();
			}
		}

		private void Start()
		{
			LoadGuiOnStartVisibility();
			LoadGuiVisibility();
			LoadSettings();
			LoadPresetList();
			GTPlayer instance = GTPlayer.Instance;
			instance.maxArmLength = maxArmLength;
			leftPrevPos = instance.GetControllerTransform(true).position;
			rightPrevPos = instance.GetControllerTransform(false).position;
			prevHeadPos = instance.headCollider.transform.position;
			leftDCpedPos = leftPrevPos;
			rightDCpedPos = rightPrevPos;
		}

		private void Update()
		{
			bool wasPressedThisFrame = Keyboard.current.qKey.wasPressedThisFrame;
			if (wasPressedThisFrame)
			{
				guiVisible = !guiVisible;
			}
			float num = GTPlayer.Instance.scale;
			bool flag = num < 0.01f;
			if (flag)
			{
				num = 0.01f;
			}
			GTPlayer instance = GTPlayer.Instance;
			instance.maxArmLength = maxArmLength;
			Transform transform = instance.headCollider.transform;
			Vector3 vector = (transform.position - prevHeadPos) / Time.deltaTime;
			bool flag2 = !requireCollision || (useLeftHand && instance.LeftHand.wasColliding);
			bool flag3 = !requireCollision || (useRightHand && instance.RightHand.wasColliding);
			Vector3 position = instance.GetControllerTransform(true).position;
			Vector3 vector2 = (position - leftPrevPos) / Time.deltaTime - vector;
			float num2 = Vector3.Dot(vector2.normalized, Vector3.down);
			float num3 = vector2.magnitude;
			float num4 = speedThreshold;
			bool flag4 = num < 0.2f;
			if (flag4)
			{
				num3 = vector2.magnitude * (1f / num);
				num4 = tinySpeedThreshold;
			}
			bool flag5 = num2 > 0.6f;
			if (flag5)
			{
				num3 *= 0.4f;
			}
			bool flag6 = !requireCollision && !instance.LeftHand.wasColliding;
			if (flag6)
			{
				num4 = 35f;
			}
			bool flag7 = useLeftHand && !leftDCped && num3 > num4 && flag2;
			if (flag7)
			{
				leftDCped = true;
				leftDCTimer = ((!requireCollision && !instance.LeftHand.wasColliding) ? (recoveryTime * 4f) : recoveryTime);
				bool flag8 = !instance.LeftHand.wasColliding && !requireCollision;
				Vector3 vector3;
				if (flag8)
				{
					vector3 = transform.forward;
				}
				else
				{
					Vector3 vector4 = transform.InverseTransformDirection(vector2.normalized);
					vector3 = transform.TransformDirection(vector4);
				}
				bool flag9 = num < 0.2f;
				float num5 = (flag9 ? tinyPullDistance : PullDistance);
				float num6 = ((requireCollision || instance.LeftHand.wasColliding) ? (flag9 ? (tinyRandomizeDistance ? global::UnityEngine.Random.Range(tinyMinRandomDistance, tinyMaxRandomDistance) : tinyPullDistance) : (randomizeDistance ? global::UnityEngine.Random.Range(minRandomDistance, maxRandomDistance) : PullDistance)) : 4.5f);
				float num7 = ((num < 0.2f) ? tinyPullOffset : PullOffset);
				Vector3 vector5 = vector3.normalized;
				float num8 = Vector3.Dot(vector5, Vector3.down);
				bool flag10 = num8 > 0.3f;
				if (flag10)
				{
					float num9 = Mathf.Clamp01(num7);
					vector5 = Vector3.Slerp(vector5, Vector3.up, num9).normalized;
				}
				leftDCpedPos = position + vector5 * num6 * PullForce;
				leftLockedRot = instance.GetControllerTransform(true).rotation;
			}
			bool flag11 = leftDCped;
			if (flag11)
			{
				leftDCTimer -= Time.deltaTime;
				bool flag12 = leftDCTimer <= 0f;
				if (flag12)
				{
					leftDCped = false;
				}
			}
			Vector3 position2 = instance.GetControllerTransform(false).position;
			Vector3 vector6 = (position2 - rightPrevPos) / Time.deltaTime - vector;
			float num10 = Vector3.Dot(vector6.normalized, Vector3.down);
			float num11 = vector6.magnitude;
			float num12 = speedThreshold;
			bool flag13 = num < 0.2f;
			if (flag13)
			{
				num11 = vector6.magnitude * (1f / num);
				num12 = tinySpeedThreshold;
			}
			bool flag14 = num10 > 0.6f;
			if (flag14)
			{
				num11 *= 0.4f;
			}
			bool flag15 = !requireCollision && !instance.RightHand.wasColliding;
			if (flag15)
			{
				num12 = 35f;
			}
			bool flag16 = useRightHand && !rightDCped && num11 > num12 && flag3;
			if (flag16)
			{
				rightDCped = true;
				rightDCTimer = ((!requireCollision && !instance.RightHand.wasColliding) ? (recoveryTime * 4f) : recoveryTime);
				bool flag17 = !instance.RightHand.wasColliding && !requireCollision;
				Vector3 vector7;
				if (flag17)
				{
					vector7 = transform.forward;
				}
				else
				{
					Vector3 vector8 = transform.InverseTransformDirection(vector6.normalized);
					vector7 = transform.TransformDirection(vector8);
				}
				bool flag18 = num < 0.2f;
				float num13 = (flag18 ? tinyPullDistance : PullDistance);
				float num14 = ((requireCollision || instance.RightHand.wasColliding) ? (flag18 ? (tinyRandomizeDistance ? global::UnityEngine.Random.Range(tinyMinRandomDistance, tinyMaxRandomDistance) : tinyPullDistance) : (randomizeDistance ? global::UnityEngine.Random.Range(minRandomDistance, maxRandomDistance) : PullDistance)) : 4.5f);
				float num15 = ((num < 0.2f) ? tinyPullOffset : PullOffset);
				Vector3 vector9 = vector7.normalized;
				float num16 = Vector3.Dot(vector9, Vector3.down);
				bool flag19 = num16 > 0.3f;
				if (flag19)
				{
					float num17 = Mathf.Clamp01(num15);
					vector9 = Vector3.Slerp(vector9, Vector3.up, num17).normalized;
				}
				rightDCpedPos = position2 + vector9 * num14 * PullForce;
				rightLockedRot = instance.GetControllerTransform(false).rotation;
			}
			bool flag20 = rightDCped;
			if (flag20)
			{
				rightDCTimer -= Time.deltaTime;
				bool flag21 = rightDCTimer <= 0f;
				if (flag21)
				{
					rightDCped = false;
				}
			}
			leftPrevPos = position;
			rightPrevPos = position2;
			prevHeadPos = transform.position;
			instance.GetControllerTransform(true).position = (leftDCped ? (Vector3.Lerp(position, leftDCpedPos, SmoothingAmount) + Vector3.down * (gravit - 1f) * Time.deltaTime) : position);
			instance.GetControllerTransform(false).position = (rightDCped ? (Vector3.Lerp(position2, rightDCpedPos, SmoothingAmount) + Vector3.down * (gravit - 1f) * Time.deltaTime) : position2);
			bool flag22 = leftDCped;
			if (flag22)
			{
				instance.GetControllerTransform(true).rotation = leftLockedRot;
			}
			bool flag23 = rightDCped;
			if (flag23)
			{
				instance.GetControllerTransform(false).rotation = rightLockedRot;
			}
		}

		private void OnGUI()
		{
			bool flag = !guiVisible;
			if (!flag)
			{
				GUI.backgroundColor = new Color(0.4f, 0.2f, 0.6f);
				GUI.contentColor = Color.white;
				GUI.skin.label.fontSize = 18;
				GUI.skin.button.fontSize = 18;
				GUI.skin.toggle.fontSize = 18;
				GUI.skin.textField.fontSize = 18;
				windowRect = GUI.Window(1985, windowRect, new GUI.WindowFunction(DrawWindow), "Legit Pull");
				bool flag2 = showPresetsWindow;
				if (flag2)
				{
					presetsWindowRect = GUI.Window(1986, presetsWindowRect, new GUI.WindowFunction(DrawPresetsWindow), "Presets");
				}
			}
		}

		private void DrawWindow(int id)
		{
			GUI.Box(new Rect(0f, 0f, windowRect.width, windowRect.height), GUIContent.none);
			GUI.backgroundColor = new Color(0.6f, 0.3f, 0.8f);
			GUI.Box(new Rect(10f, 25f, windowRect.width - 20f, windowRect.height - 35f), GUIContent.none);
			GUILayout.BeginArea(new Rect(15f, 30f, windowRect.width - 30f, windowRect.height - 45f));
			scrollPos = GUILayout.BeginScrollView(scrollPos, new GUILayoutOption[]
			{
				GUILayout.Width(windowRect.width - 40f),
				GUILayout.Height(windowRect.height - 100f)
			});
			bool flag = GUILayout.Toggle(guiVisibleOnStart, "Show GUI on Start", Array.Empty<GUILayoutOption>());
			bool flag2 = flag != guiVisibleOnStart;
			if (flag2)
			{
				guiVisibleOnStart = flag;
				guiVisible = guiVisibleOnStart;
				SaveGuiOnStartVisibility();
				SaveGuiVisibility();
			}
			DrawSliderWithInput("Speed Threshold", ref speedThreshold, 1f, 50f, "DCRun_Speed");
			DrawSliderWithInput("Tiny Threshold", ref tinySpeedThreshold, 1f, 50f, "DCRun_TinySpeed");
			DrawSliderWithInput("Fake DC", ref recoveryTime, 0.05f, 2f, "DCRun_Recovery");
			DrawSliderWithInput("Pull Distance", ref PullDistance, 0.05f, 5f, "DCRun_Distance");
			bool flag3 = GUILayout.Toggle(randomizeDistance, "Randomize Distance", Array.Empty<GUILayoutOption>());
			bool flag4 = flag3 != randomizeDistance;
			if (flag4)
			{
				randomizeDistance = flag3;
			}
			bool flag5 = randomizeDistance;
			if (flag5)
			{
				DrawSliderWithInput("Min Pull Distance", ref minRandomDistance, 0.01f, 5f, "DCRun_MinDist");
				DrawSliderWithInput("Max Pull Distance", ref maxRandomDistance, 0.01f, 5f, "DCRun_MaxDist");
			}
			DrawSliderWithInput("Smoothing", ref SmoothingAmount, 0f, 1f, "DCRun_Smooth");
			DrawSliderWithInput("Lerp", ref LerpAmount, 0f, 1f, "DCRun_Lerp");
			DrawSliderWithInput("Max Arm Length", ref maxArmLength, 1.5f, 3f, "DCRun_MaxArm");
			DrawSliderWithInput("Pull Force", ref PullForce, 1f, 5f, "DCRun_Force");
			DrawSliderWithInput("Gravit", ref gravit, 1f, 3f, "DCRun_Gravit");
			DrawSliderWithInput("Pull Offset", ref PullOffset, -0.5f, 0.1f, "DCRun_Offset");
			GUILayout.Label("Tiny Pull Distance", Array.Empty<GUILayoutOption>());
			DrawSliderWithInput("Tiny Pull Distance", ref tinyPullDistance, 0.1f, 0.5f, "DCRun_TinyPullDistance");
			GUILayout.Space(5f);
			GUILayout.Label("Tiny Randomized Distance", Array.Empty<GUILayoutOption>());
			bool flag6 = GUILayout.Toggle(tinyRandomizeDistance, "Randomize Tiny Distance", Array.Empty<GUILayoutOption>());
			bool flag7 = flag6 != tinyRandomizeDistance;
			if (flag7)
			{
				tinyRandomizeDistance = flag6;
			}
			bool flag8 = tinyRandomizeDistance;
			if (flag8)
			{
				DrawSliderWithInput("Tiny Min Pull Distance", ref tinyMinRandomDistance, 0.01f, 1f, "DCRun_TinyMinDist");
				DrawSliderWithInput("Tiny Max Pull Distance", ref tinyMaxRandomDistance, tinyMinRandomDistance, 1f, "DCRun_TinyMaxDist");
			}
			bool flag9 = GUILayout.Toggle(useLeftHand, "Enable Left Hand", Array.Empty<GUILayoutOption>());
			bool flag10 = flag9 != useLeftHand;
			if (flag10)
			{
				useLeftHand = flag9;
			}
			bool flag11 = GUILayout.Toggle(useRightHand, "Enable Right Hand", Array.Empty<GUILayoutOption>());
			bool flag12 = flag11 != useRightHand;
			if (flag12)
			{
				useRightHand = flag11;
			}
			bool flag13 = GUILayout.Toggle(requireCollision, "Require Collision", Array.Empty<GUILayoutOption>());
			bool flag14 = flag13 != requireCollision;
			if (flag14)
			{
				requireCollision = flag13;
			}
			GUILayout.Space(15f);
			bool flag15 = GUILayout.Button("Open Presets", Array.Empty<GUILayoutOption>());
			if (flag15)
			{
				showPresetsWindow = !showPresetsWindow;
			}
			GUILayout.EndScrollView();
			GUILayout.Space(10f);
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			bool flag16 = GUILayout.Button("Save Settings", Array.Empty<GUILayoutOption>());
			if (flag16)
			{
				SaveSettings();
			}
			bool flag17 = GUILayout.Button("Load Settings", Array.Empty<GUILayoutOption>());
			if (flag17)
			{
				LoadSettings();
			}
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
			GUI.DragWindow(new Rect(0f, 0f, 10000f, 20000f));
		}

		private void SaveGuiOnStartVisibility()
		{
			PlayerPrefs.SetInt("DCRun_GUI_VisibleOnStart", guiVisibleOnStart ? 1 : 0);
			PlayerPrefs.Save();
		}

		private void LoadGuiOnStartVisibility()
		{
			guiVisibleOnStart = PlayerPrefs.GetInt("DCRun_GUI_VisibleOnStart", 1) == 1;
		}

		private void DrawPresetsWindow(int id)
		{
			GUI.backgroundColor = new Color(0.6f, 0.3f, 0.8f);
			GUI.Box(new Rect(0f, 0f, presetsWindowRect.width, presetsWindowRect.height), GUIContent.none);
			GUILayout.BeginArea(new Rect(10f, 25f, presetsWindowRect.width - 20f, presetsWindowRect.height - 35f));
			presetScrollPos = GUILayout.BeginScrollView(presetScrollPos, Array.Empty<GUILayoutOption>());
			for (int i = 0; i < presetNames.Count; i++)
			{
				GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
				GUI.backgroundColor = new Color(0.4f, 0.2f, 0.6f);
				bool flag = GUILayout.Button(presetNames[i], Array.Empty<GUILayoutOption>());
				if (flag)
				{
					selectedPresetIndex = i;
					LoadCustomPreset(presetNames[i]);
				}
				bool flag2 = GUILayout.Button("X", new GUILayoutOption[] { GUILayout.Width(30f) });
				if (flag2)
				{
					DeletePreset(presetNames[i]);
					break;
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndScrollView();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			newPresetName = GUILayout.TextField(newPresetName, new GUILayoutOption[] { GUILayout.Width(presetsWindowRect.width - 110f) });
			bool flag3 = GUILayout.Button("Save", new GUILayoutOption[] { GUILayout.Width(80f) }) && !string.IsNullOrWhiteSpace(newPresetName);
			if (flag3)
			{
				SaveCustomPreset(newPresetName.Trim());
				newPresetName = "";
			}
			GUILayout.EndHorizontal();
			bool flag4 = GUILayout.Button("Close", Array.Empty<GUILayoutOption>());
			if (flag4)
			{
				showPresetsWindow = false;
			}
			GUILayout.EndArea();
			GUI.backgroundColor = Color.white;
			GUI.contentColor = Color.white;
			GUI.DragWindow(new Rect(0f, 0f, 10000f, 20000f));
		}

		private void SaveGuiVisibility()
		{
			PlayerPrefs.SetInt("DCRun_GUI_Visible", guiVisible ? 1 : 0);
			PlayerPrefs.Save();
		}

		private void LoadGuiVisibility()
		{
			guiVisible = PlayerPrefs.GetInt("DCRun_GUI_Visible", 1) == 1;
		}

		private void SaveCustomPreset(string name)
		{
			PulMud.DCPreset dcpreset = new PulMud.DCPreset
			{
				tinyPullDistance = tinyPullDistance,
				tinyMinRandomDistance = tinyMinRandomDistance,
				tinyMaxRandomDistance = tinyMaxRandomDistance,
				tinyRandomizeDistance = tinyRandomizeDistance,
				gravit = gravit,
				speedThreshold = speedThreshold,
				recoveryTime = recoveryTime,
				PullDistance = PullDistance,
				LerpAmount = LerpAmount,
				maxArmLength = maxArmLength,
				useLeftHand = useLeftHand,
				useRightHand = useRightHand,
				requireCollision = requireCollision,
				PullForce = PullForce,
				PullOffset = PullOffset,
				tinySpeedThreshold = tinySpeedThreshold,
				tinyPullOffset = tinyPullOffset,
				randomizeDistance = randomizeDistance,
				maxRandomDistance = maxRandomDistance,
				minRandomDistance = minRandomDistance,
				SmoothingAmount = SmoothingAmount
			};
			string text = JsonUtility.ToJson(dcpreset);
			PlayerPrefs.SetString("DCRun_Preset_" + name, text);
			bool flag = !presetNames.Contains(name);
			if (flag)
			{
				presetNames.Add(name);
				SavePresetList();
			}
			PlayerPrefs.Save();
			UpdatePresetArray();
		}

		private void LoadCustomPreset(string name)
		{
			string @string = PlayerPrefs.GetString("DCRun_Preset_" + name, null);
			bool flag = !string.IsNullOrEmpty(@string);
			if (flag)
			{
				PulMud.DCPreset dcpreset = JsonUtility.FromJson<PulMud.DCPreset>(@string);
				tinyPullDistance = dcpreset.tinyPullDistance;
				tinyMinRandomDistance = dcpreset.tinyMinRandomDistance;
				tinyMaxRandomDistance = dcpreset.tinyMaxRandomDistance;
				tinyRandomizeDistance = dcpreset.tinyRandomizeDistance;
				gravit = dcpreset.gravit;
				speedThreshold = dcpreset.speedThreshold;
				recoveryTime = dcpreset.recoveryTime;
				PullDistance = dcpreset.PullDistance;
				LerpAmount = dcpreset.LerpAmount;
				maxArmLength = dcpreset.maxArmLength;
				PullForce = dcpreset.PullForce;
				useLeftHand = dcpreset.useLeftHand;
				useRightHand = dcpreset.useRightHand;
				requireCollision = dcpreset.requireCollision;
				PullOffset = dcpreset.PullOffset;
				tinySpeedThreshold = dcpreset.tinySpeedThreshold;
				tinyPullOffset = dcpreset.tinyPullOffset;
				randomizeDistance = dcpreset.randomizeDistance;
				maxRandomDistance = dcpreset.maxRandomDistance;
				minRandomDistance = dcpreset.minRandomDistance;
				SmoothingAmount = dcpreset.SmoothingAmount;
				SaveSettings();
			}
		}

		private void DeletePreset(string name)
		{
			PlayerPrefs.DeleteKey("DCRun_Preset_" + name);
			presetNames.Remove(name);
			SavePresetList();
			UpdatePresetArray();
		}

		private void SavePresetList()
		{
			string text = string.Join("|", presetNames);
			PlayerPrefs.SetString("DCRun_PresetList", text);
			PlayerPrefs.Save();
		}

		private void LoadPresetList()
		{
			string @string = PlayerPrefs.GetString("DCRun_PresetList", null);
			bool flag = !string.IsNullOrEmpty(@string);
			if (flag)
			{
				presetNames = new List<string>(@string.Split(new char[] { '|' }));
			}
			UpdatePresetArray();
		}

		private void UpdatePresetArray()
		{
			presetArray = presetNames.ToArray();
		}

		private void DrawSliderWithInput(string label, ref float value, float min, float max, string playerPrefKey)
		{
			GUILayout.Label(string.Format("{0}: {1:F2}", label, value), Array.Empty<GUILayoutOption>());
			float num = GUILayout.HorizontalSlider(value, min, max, Array.Empty<GUILayoutOption>());
			bool flag = num != value;
			if (flag)
			{
				value = num;
			}
			string text = GUILayout.TextField(value.ToString("F2"), Array.Empty<GUILayoutOption>());
			float num2;
			bool flag2 = float.TryParse(text, out num2);
			if (flag2)
			{
				num2 = Mathf.Clamp(num2, min, max);
				bool flag3 = num2 != value;
				if (flag3)
				{
					value = num2;
				}
			}
		}

		private void LoadSettings()
		{
			tinyRandomizeDistance = PlayerPrefs.GetInt("DCRun_TinyRandomDist", 0) == 1;
			tinyPullDistance = PlayerPrefs.GetFloat("DCRun_TinyPullDistance", 0.15f);
			tinyMinRandomDistance = PlayerPrefs.GetFloat("DCRun_TinyMinDist", 0.05f);
			tinyMaxRandomDistance = PlayerPrefs.GetFloat("DCRun_TinyMaxDist", 0.3f);
			tinySpeedThreshold = PlayerPrefs.GetFloat("DCRun_TinySpeed", tinySpeedThreshold);
			tinyPullOffset = PlayerPrefs.GetFloat("DCRun_TinyOffset", tinyPullOffset);
			speedThreshold = PlayerPrefs.GetFloat("DCRun_Speed", speedThreshold);
			recoveryTime = PlayerPrefs.GetFloat("DCRun_Recovery", recoveryTime);
			PullDistance = PlayerPrefs.GetFloat("DCRun_Distance", PullDistance);
			randomizeDistance = PlayerPrefs.GetInt("DCRun_RandomDist", 0) == 1;
			minRandomDistance = PlayerPrefs.GetFloat("DCRun_MinDist", 0.1f);
			maxRandomDistance = PlayerPrefs.GetFloat("DCRun_MaxDist", 1f);
			LerpAmount = PlayerPrefs.GetFloat("DCRun_Lerp", LerpAmount);
			maxArmLength = PlayerPrefs.GetFloat("DCRun_MaxArm", maxArmLength);
			PullForce = PlayerPrefs.GetFloat("DCRun_Force", PullForce);
			PullOffset = PlayerPrefs.GetFloat("DCRun_Offset", PullOffset);
			gravit = PlayerPrefs.GetFloat("DCRun_Gravit", gravit);
			useLeftHand = PlayerPrefs.GetInt("DCRun_Left", useLeftHand ? 1 : 0) == 1;
			useRightHand = PlayerPrefs.GetInt("DCRun_Right", useRightHand ? 1 : 0) == 1;
			requireCollision = PlayerPrefs.GetInt("DCRun_Collision", requireCollision ? 1 : 0) == 1;
			SmoothingAmount = PlayerPrefs.GetFloat("DCRun_Smooth", 0.1f);
		}

		private void SaveSettings()
		{
			PlayerPrefs.SetInt("DCRun_TinyRandomDist", tinyRandomizeDistance ? 1 : 0);
			PlayerPrefs.SetFloat("DCRun_TinyPullDistance", tinyPullDistance);
			PlayerPrefs.SetFloat("DCRun_TinyMinDist", tinyMinRandomDistance);
			PlayerPrefs.SetFloat("DCRun_TinyMaxDist", tinyMaxRandomDistance);
			PlayerPrefs.SetFloat("DCRun_TinySpeed", tinySpeedThreshold);
			PlayerPrefs.SetFloat("DCRun_TinyOffset", tinyPullOffset);
			PlayerPrefs.SetFloat("DCRun_Offset", PullOffset);
			PlayerPrefs.SetFloat("DCRun_Speed", speedThreshold);
			PlayerPrefs.SetFloat("DCRun_Recovery", recoveryTime);
			PlayerPrefs.SetFloat("DCRun_Distance", PullDistance);
			PlayerPrefs.SetInt("DCRun_RandomDist", randomizeDistance ? 1 : 0);
			PlayerPrefs.SetFloat("DCRun_MinDist", minRandomDistance);
			PlayerPrefs.SetFloat("DCRun_MaxDist", maxRandomDistance);
			PlayerPrefs.SetFloat("DCRun_Lerp", LerpAmount);
			PlayerPrefs.SetFloat("DCRun_MaxArm", maxArmLength);
			PlayerPrefs.SetFloat("DCRun_Force", PullForce);
			PlayerPrefs.SetFloat("DCRun_Gravit", gravit);
			PlayerPrefs.SetInt("DCRun_Left", useLeftHand ? 1 : 0);
			PlayerPrefs.SetInt("DCRun_Right", useRightHand ? 1 : 0);
			PlayerPrefs.SetInt("DCRun_Collision", requireCollision ? 1 : 0);
			PlayerPrefs.SetFloat("DCRun_Smooth", SmoothingAmount);
			PlayerPrefs.Save();
		}

		private bool IsVerticalSurface(Vector3 normal)
		{
			return Mathf.Abs(Vector3.Dot(normal, Vector3.up)) < 0.3f;
		}

		private float speedThreshold = 2f;

		private float recoveryTime = 0.5f;

		private float PullDistance = 0.3f;

		private float LerpAmount = 0f;

		private float maxArmLength = 1.5f;

		private bool useLeftHand = true;

		private bool useRightHand = true;

		private bool requireCollision = true;

		public float PullForce = 1f;

		private Rect windowRect = new Rect(20f, 20f, 350f, 500f);

		private Rect presetsWindowRect = new Rect(400f, 20f, 300f, 400f);

		private Vector2 scrollPos;

		private Vector2 presetScrollPos;

		private string newPresetName = "";

		private List<string> presetNames = new List<string>();

		private string[] presetArray = new string[0];

		private int selectedPresetIndex = 0;

		private bool showDeleteConfirm = false;

		private bool showPresetsWindow = false;

		public float PullOffset = 0f;

		private float gravit = 1f;

		private Quaternion leftLockedRot;

		private Quaternion rightLockedRot;

		private Vector3 leftPrevPos;

		private Vector3 rightPrevPos;

		private Vector3 prevHeadPos;

		private Vector3 leftDCpedPos;

		private Vector3 rightDCpedPos;

		private bool leftDCped;

		private bool rightDCped;

		private float leftDCTimer = 0f;

		private float rightDCTimer = 0f;

		private float tinySpeedThreshold = 3f;

		private float tinyPullOffset = 0.5f;

		private bool randomizeDistance = false;

		private float minRandomDistance = 0.1f;

		private float maxRandomDistance = 1f;

		private bool guiVisible = true;

		private bool guiVisibleOnStart = true;

		private float SmoothingAmount = 0.1f;

		private bool showPredWindow = false;

		private float pred = 1f;

		private float xPred = 1f;

		private float yPred = 1f;

		private float predSmoothing = 0.5f;

		private Vector3 leftVelocity = Vector3.zero;

		private Vector3 rightVelocity = Vector3.zero;

		private Vector3 leftPrevLocal = Vector3.zero;

		private Vector3 rightPrevLocal = Vector3.zero;

		private Vector3 leftHandOffset = Vector3.zero;

		private Vector3 rightHandOffset = Vector3.zero;

		private bool tinyRandomizeDistance = false;

		private float tinyPullDistance = 0.15f;

		private float tinyMinRandomDistance = 0.05f;

		private float tinyMaxRandomDistance = 0.3f;

		private class DCPreset
		{
			public float tinyPullDistance;

			public float SmoothingAmount;

			public float speedThreshold;

			public float recoveryTime;

			public float PullDistance;

			public float LerpAmount;

			public float maxArmLength;

			public bool useLeftHand;

			public bool useRightHand;

			public bool requireCollision;

			public float PullForce;

			public float PullOffset;

			public float gravit;

			public float tinySpeedThreshold;

			public float tinyPullOffset;

			public bool randomizeDistance;

			public float minRandomDistance;

			public float maxRandomDistance;

			public float tinyMinRandomDistance;

			public float tinyMaxRandomDistance;

			public bool tinyRandomizeDistance;
		}
	}
}
