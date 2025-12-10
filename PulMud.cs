using System.Collections.Generic;
using BepInEx;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GorillaTag.SmoothClimb
{
    [BepInPlugin("com.lemons.smoothclimb", "Smooth Climb Optimized & Edited by казан", "3.0.0")]
    public class SmoothClimb : BaseUnityPlugin
    {
        private void Start()
        {
            LoadSettings();
            leftLast = Player.Instance.leftControllerTransform.position;
            rightLast = Player.Instance.rightControllerTransform.position;
            headLast = Player.Instance.headCollider.transform.position;
        }

        private void Update()
        {
            if (Keyboard.current.f10Key.wasPressedThisFrame)
                showMenu = !showMenu;

            Player p = Player.Instance;
            p.maxArmLength = armLength;

            Vector3 headVel = (p.headCollider.transform.position - headLast) / Time.deltaTime;

            Vector3 lPos = p.leftControllerTransform.position;
            Vector3 rPos = p.rightControllerTransform.position;
            Vector3 lVel = (lPos - leftLast) / Time.deltaTime - headVel;
            Vector3 rVel = (rPos - rightLast) / Time.deltaTime - headVel;

            CheckHand(true, lPos, lVel, p.leftHandCollider);
            CheckHand(false, rPos, rVel, p.rightHandCollider);

            leftLast = lPos;
            rightLast = rPos;
            headLast = p.headCollider.transform.position;

            if (leftBoost)
                p.leftControllerTransform.position = Vector3.Lerp(lPos, leftTarget, smooth);
            if (rightBoost)
                p.rightControllerTransform.position = Vector3.Lerp(rPos, rightTarget, smooth);
        }

        private void CheckHand(bool left, Vector3 pos, Vector3 vel, GorillaHandCollider hand)
        {
            bool gripping = !needWall || hand.wasColliding;
            float speed = vel.magnitude;
            float thresh = Player.Instance.scale < 0.25f ? tinyThresh : normalThresh;

            if (speed > thresh && gripping && ((left && useLeft) || (!left && useRight)))
            {
                if ((left && leftTimer <= 0f) || (!left && rightTimer <= 0f))
                {
                    Vector3 dir = hand.wasColliding
                        ? p.headCollider.transform.TransformDirection(
                            p.headCollider.transform.InverseTransformDirection(vel.normalized))
                        : p.headCollider.transform.forward;

                    float dist = Player.Instance.scale < 0.25f
                        ? (tinyRand ? Random.Range(tinyMin, tinyMax) : tinyDist)
                        : (doRand ? Random.Range(minDist, maxDist) : baseDist);

                    Vector3 pull = dir.normalized;
                    if (Vector3.Dot(pull, Vector3.down) > 0.4f)
                        pull = Vector3.Slerp(pull, Vector3.up, upBias).normalized;

                    if (left)
                    {
                        leftTarget = pos + pull * dist * power;
                        leftTimer = cooldown;
                        leftBoost = true;
                        p.leftControllerTransform.rotation = p.leftControllerTransform.rotation;
                    }
                    else
                    {
                        rightTarget = pos + pull * dist * power;
                        rightTimer = cooldown;
                        rightBoost = true;
                        p.rightControllerTransform.rotation = p.rightControllerTransform.rotation;
                    }
                }
            }

            if (left && leftTimer > 0f) leftTimer -= Time.deltaTime;
            if (!left && rightTimer > 0f) rightTimer -= Time.deltaTime;

            if (leftTimer <= 0f) leftBoost = false;
            if (rightTimer <= 0f) rightBoost = false;
        }

        private void OnGUI()
        {
            if (!showMenu) return;

            GUI.skin.box.normal.background = Texture2D.grayTexture;
            GUI.skin.box.normal.textColor = Color.white;
            GUI.skin.label.normal.textColor = new Color(0.9f, 0.9f, 1f);
            GUI.skin.button.normal.background = Texture2D.grayTexture;
            GUI.skin.button.hover.background = Texture2D.whiteTexture;
            GUI.skin.button.hover.textColor = Color.black;
            GUI.skin.button.normal.textColor = Color.white;
            GUI.skin.toggle.normal.textColor = Color.white;
            GUI.skin.horizontalSlider.normal.background = Texture2D.grayTexture;
            GUI.skin.horizontalSliderThumb.normal.background = Texture2D.whiteTexture;
            GUI.skin.textField.normal.background = Texture2D.grayTexture;
            GUI.skin.textField.normal.textColor = Color.white;

            win = GUI.Window(1337, win, Window, "");
        }

        private void Window(int id)
        {
            GUI.backgroundColor = new Color(0.1f, 0.1f, 0.15f, 0.98f);
            GUI.Box(new Rect(0, 0, win.width, win.height), "");

            GUI.Label(new Rect(10, 5, 340, 20), "smooth climb v3 - pull request by казан", GUI.skin.label);

            float y = 30f;
            showOnStart = GUI.Toggle(new Rect(10, y, 200, 20), showOnStart, "show on start");
            y += 25;

            GUI.Label(new Rect(10, y, 150, 20), $"normal thresh: {normalThresh:F2}");
            normalThresh = GUI.HorizontalSlider(new Rect(160, y, 180, 20), normalThresh, 4f, 35f);
            y += 25;

            GUI.Label(new Rect(10, y, 150, 20), $"tiny thresh: {tinyThresh:F2}");
            tinyThresh = GUI.HorizontalSlider(new Rect(160, y, 180, 20), tinyThresh, 6f, 55f);
            y += 25;

            GUI.Label(new Rect(10, y, 150, 20), $"distance: {baseDist:F2}");
            baseDist = GUI.HorizontalSlider(new Rect(160, y, 180, 20), baseDist, 0.1f, 3.5f);
            y += 25;

            doRand = GUI.Toggle(new Rect(10, y, 200, 20), doRand, "random dist");
            y += 25;

            if (doRand)
            {
                GUI.Label(new Rect(20, y, 150, 20), $"min: {minDist:F2}");
                minDist = GUI.HorizontalSlider(new Rect(170, y, 170, 20), minDist, 0.05f, 2f);
                y += 25;

                GUI.Label(new Rect(20, y, 150, 20), $"max: {maxDist:F2}");
                maxDist = GUI.HorizontalSlider(new Rect(170, y, 170, 20), maxDist, 0.2f, 5f);
                y += 25;
            }

            GUI.Label(new Rect(10, y, 150, 20), $"tiny dist: {tinyDist:F2}");
            tinyDist = GUI.HorizontalSlider(new Rect(160, y, 180, 20), tinyDist, 0.08f, 0.7f);
            y += 25;

            tinyRand = GUI.Toggle(new Rect(10, y, 200, 20), tinyRand, "tiny random");
            y += 25;

            if (tinyRand)
            {
                GUI.Label(new Rect(20, y, 150, 20), $"tiny min: {tinyMin:F2}");
                tinyMin = GUI.HorizontalSlider(new Rect(170, y, 170, 20), tinyMin, 0.03f, 0.6f);
                y += 25;

                GUI.Label(new Rect(20, y, 150, 20), $"tiny max: {tinyMax:F2}");
                tinyMax = GUI.HorizontalSlider(new Rect(170, y, 170, 20), tinyMax, 0.1f, 1f);
                y += 25;
            }

            GUI.Label(new Rect(10, y, 150, 20), $"power: {power:F2}");
            power = GUI.HorizontalSlider(new Rect(160, y, 180, 20), power, 0.6f, 5.5f);
            y += 25;

            GUI.Label(new Rect(10, y, 150, 20), $"cooldown: {cooldown:F2}");
            cooldown = GUI.HorizontalSlider(new Rect(160, y, 180, 20), cooldown, 0.15f, 1.2f);
            y += 25;

            GUI.Label(new Rect(10, y, 150, 20), $"smooth: {smooth:F2}");
            smooth = GUI.HorizontalSlider(new Rect(160, y, 180, 20), smooth, 0.05f, 0.95f);
            y += 25;

            GUI.Label(new Rect(10, y, 150, 20), $"arm length: {armLength:F2}");
            armLength = GUI.HorizontalSlider(new Rect(160, y, 180, 20), armLength, 1.6f, 2.9f);
            y += 25;

            GUI.Label(new Rect(10, y, 150, 20), $"up bias: {upBias:F2}");
            upBias = GUI.HorizontalSlider(new Rect(160, y, 180, 20), upBias, -0.3f, 0.7f);
            y += 25;

            useLeft = GUI.Toggle(new Rect(10, y, 200, 20), useLeft, "left hand");
            y += 25;

            useRight = GUI.Toggle(new Rect(10, y, 200, 20), useRight, "right hand");
            y += 25;

            needWall = GUI.Toggle(new Rect(10, y, 200, 20), needWall, "need wall");
            y += 30;

            if (GUI.Button(new Rect(10, y, 160, 30), "save"))
                SaveSettings();
            if (GUI.Button(new Rect(180, y, 160, 30), "load"))
                LoadSettings();

            GUI.DragWindow(new Rect(0, 0, win.width, 30));
        }

        private void SaveSettings()
        {
            PlayerPrefs.SetFloat("sc_n", normalThresh);
            PlayerPrefs.SetFloat("sc_t", tinyThresh);
            PlayerPrefs.SetFloat("sc_d", baseDist);
            PlayerPrefs.SetFloat("sc_p", power);
            PlayerPrefs.SetFloat("sc_c", cooldown);
            PlayerPrefs.SetFloat("sc_s", smooth);
            PlayerPrefs.SetFloat("sc_a", armLength);
            PlayerPrefs.SetFloat("sc_u", upBias);
            PlayerPrefs.SetFloat("sc_td", tinyDist);
            PlayerPrefs.SetFloat("sc_mind", minDist);
            PlayerPrefs.SetFloat("sc_maxd", maxDist);
            PlayerPrefs.SetFloat("sc_tmin", tinyMin);
            PlayerPrefs.SetFloat("sc_tmax", tinyMax);
            PlayerPrefs.SetInt("sc_rand", doRand ? 1 : 0);
            PlayerPrefs.SetInt("sc_trand", tinyRand ? 1 : 0);
            PlayerPrefs.SetInt("sc_l", useLeft ? 1 : 0);
            PlayerPrefs.SetInt("sc_r", useRight ? 1 : 0);
            PlayerPrefs.SetInt("sc_wall", needWall ? 1 : 0);
            PlayerPrefs.SetInt("sc_start", showOnStart ? 1 : 0);
            PlayerPrefs.Save();
        }

        private void LoadSettings()
        {
            normalThresh = PlayerPrefs.GetFloat("sc_n", 10.5f);
            tinyThresh = PlayerPrefs.GetFloat("sc_t", 16f);
            baseDist = PlayerPrefs.GetFloat("sc_d", 0.58f);
            power = PlayerPrefs.GetFloat("sc_p", 2.1f);
            cooldown = PlayerPrefs.GetFloat("sc_c", 0.38f);
            smooth = PlayerPrefs.GetFloat("sc_s", 0.22f);
            armLength = PlayerPrefs.GetFloat("sc_a", 2.15f);
            upBias = PlayerPrefs.GetFloat("sc_u", 0.32f);
            tinyDist = PlayerPrefs.GetFloat("sc_td", 0.19f);
            minDist = PlayerPrefs.GetFloat("sc_mind", 0.35f);
            maxDist = PlayerPrefs.GetFloat("sc_maxd", 1.1f);
            tinyMin = PlayerPrefs.GetFloat("sc_tmin", 0.11f);
            tinyMax = PlayerPrefs.GetFloat("sc_tmax", 0.42f);
            doRand = PlayerPrefs.GetInt("sc_rand", 1) == 1;
            tinyRand = PlayerPrefs.GetInt("sc_trand", 1) == 1;
            useLeft = PlayerPrefs.GetInt("sc_l", 1) == 1;
            useRight = PlayerPrefs.GetInt("sc_r", 1) == 1;
            needWall = PlayerPrefs.GetInt("sc_wall", 1) == 1;
            showOnStart = PlayerPrefs.GetInt("sc_start", 1) == 1;
            showMenu = showOnStart;
        }

        private Player p => Player.Instance;

        private float normalThresh = 10.5f;
        private float tinyThresh = 16f;
        private float baseDist = 0.58f;
        private float tinyDist = 0.19f;
        private float power = 2.1f;
        private float cooldown = 0.38f;
        private float smooth = 0.22f;
        private float armLength = 2.15f;
        private float upBias = 0.32f;
        private float minDist = 0.35f;
        private float maxDist = 1.1f;
        private float tinyMin = 0.11f;
        private float tinyMax = 0.42f;

        private bool doRand = true;
        private bool tinyRand = true;
        private bool useLeft = true;
        private bool useRight = true;
        private bool needWall = true;
        private bool showOnStart = true;

        private Vector3 leftLast, rightLast, headLast;
        private Vector3 leftTarget, rightTarget;
        private bool leftBoost, rightBoost;
        private float leftTimer, rightTimer;

        private bool showMenu = true;
        private Rect win = new Rect(30, 30, 360, 700);
    }
}
