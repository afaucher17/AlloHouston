﻿using CRI.HelloHouston.Experience.Actions;
using System.Linq;
using UnityEngine;
using System;
using CRI.HelloHouston.Calibration;
using CRI.HelloHouston.Audio;
using CRI.HelloHouston.Translation;
using UnityEngine.SceneManagement;
using CRI.HelloHouston.Settings;
using CRI.HelloHouston.GameElements;
using CRI.HelloHouston.GameElements;

namespace CRI.HelloHouston.Experience
{
    public enum GameState
    {
        Default,
        Alarm,
        Dark,
    }

    public class GameManager : MonoBehaviour, ISource, ILangManager
    {
        private static int s_randomSeed;

        public static int randomSeed
        {
            get
            {
                return s_randomSeed;
            }
        }

        private static GameManager s_instance;

        public static GameManager instance
        {
            get
            {
                if (!s_instance)
                    s_instance = GameObject.FindObjectOfType<GameManager>();
                if (!s_instance)
                    s_instance = new GameObject("GameManager").AddComponent<GameManager>();
                return s_instance;
            }
        }

        public delegate void GameManagerEvent();
        public static GameManagerEvent onExperienceChange;
        /// <summary>
        /// The Game action controller.
        /// </summary>
        public GameActionController gameActionController { get; private set; }
        [SerializeField]
        private AppSettings _appSettings = null;
        [SerializeField]
        private XPMainSettings _mainSettings = null;
        /// <summary>
        /// The Log controller.
        /// </summary>
        public LogManager logManager { get; private set; }
        /// <summary>
        /// The log general controller.
        /// </summary>
        public LogGeneralController logGeneralController { get { return logManager.logGeneralController; } }
        /// <summary>
        /// globalSoundManager
        /// </summary>
        public SoundManager globalSoundManager { get; private set; }
        /// <summary>
        /// Language manager.
        /// </summary>
        public LangManager langManager { get; protected set; }
        /// <summary>
        /// Text manager.
        /// </summary>
        public TextManager textManager
        {
            get
            {
                return langManager.textManager;
            }
        }
        /// <summary>
        /// Experience list.
        /// </summary>
        public XPManager[] xpManagers { get; private set; }
        /// <summary>
        /// All the available game actions.
        /// </summary>
        public GameAction[] actions
        {
            get
            {
                return _mainSettings.actions;
            }
        }
        private float _startTime;
        /// <summary>
        /// Time since the game start (in seconds).
        /// </summary>
        public float timeSinceGameStart {
            get
            {
                return Time.time - _startTime;
            }
        }

        public int xpTimeEstimate { get; private set; }

        private VirtualRoom _room;
        private RoomAnimator _roomAnimator;
        private HologramManager _hologramManager;
        private UIComScreen _comScreen;
        private Holocube _holocube;
        private GameState _currentState;
        private GameState _previousState;

        public string sourceName
        {
            get
            {
                return "Main";
            }
        }

        private void Awake()
        {
            InitGameManager();
        }

        private void InitGameManager()
        {
            globalSoundManager = GetComponent<SoundManager>();
            gameActionController = new GameActionController(this);
            logManager = new LogManager(this);
            langManager = new LangManager(_appSettings.langSettings);
        }

        public XPManager[] InitGame(XPContext[] xpContexts, VirtualRoom room, int timeEstimate)
        {
            return InitGame(xpContexts, room, timeEstimate, UnityEngine.Random.Range(0, int.MaxValue));
        }

        public XPManager[] InitGame(XPContext[] xpContexts, VirtualRoom room, int timeEstimate, int seed)
        {
            s_randomSeed = seed;
            this.xpTimeEstimate = timeEstimate;
            _room = room;
            _roomAnimator = room.GetComponent<RoomAnimator>();
            _hologramManager = room.GetComponentInChildren<HologramManager>(true);
            _holocube = room.GetComponentInChildren<Holocube>(true);
            // Managers init
            xpManagers = xpContexts.Select(xpContext => xpContext.InitManager(logManager.logExperienceController, room.GetZones().Where(zone => zone.xpContext == xpContext).ToArray(), s_randomSeed)).ToArray();
            // Change tube init
            _room.GetComponentInChildren<ChangeTubeHologram>(true).Init(this, xpManagers, _room.GetZones<VirtualWallTopZone>());
            _startTime = Time.time;
            logGeneralController.AddLog(string.Format("Random Seed: {0}", randomSeed), this, Log.LogType.Important);
            _comScreen = room.GetComponentInChildren<UIComScreen>(true);
            _comScreen.Init(this, xpManagers);
            _comScreen.gameObject.SetActive(false);
            return xpManagers;
        }

        public GameHint[] GetAllCurrentHints()
        {
            return _mainSettings.hints.Select(hint => new GameHint(hint, this)).Concat(xpManagers.Where(x => x.state == XPState.InProgress).SelectMany(x => x.xpContext.hints)).ToArray();
        }

        /// <summary>
        /// Unloads an xp from a wall top zone.
        /// </summary>
        /// <param name="wallTopIndex">The index of the wall top zone from which the experience will be unloaded.</param>
        public bool UnloadXP(int wallTopIndex, Action onXPUnloaded = null)
        {
            var zone = _room.GetZones<VirtualWallTopZone>().FirstOrDefault(x => x.index == wallTopIndex);
            if (zone != null)
                return UnloadXP(zone, onXPUnloaded);
            return false;
        }

        /// <summary>
        /// Unloads an xp from a wall top zone.
        /// </summary>
        /// <param name="zone">The wall top zone from which the experience will be unloaded.</param>
        public bool UnloadXP(VirtualWallTopZone zone, Action onXPUnloaded = null)
        {
            XPManager manager = zone.manager;
            if (manager == null)
                return false;
            var vhzone = _room.GetZones<VirtualHologramZone>().FirstOrDefault(x => x.index == zone.index && x.xpZone == null);
            if (vhzone == null)
                vhzone = _room.GetZones<VirtualHologramZone>().FirstOrDefault(x => x.xpZone == null);
            logGeneralController.AddLog(string.Format("XP Unloaded: {0}", manager.xpContext.contextName), this, Log.LogType.Important);
            _roomAnimator.UninstallTubex(zone.index, manager, () => {
                manager.Hide();
                if (onXPUnloaded != null)
                    onXPUnloaded.Invoke();
            });
            return true;
        }

        /// <summary>
        /// Loads an experience into a wall top zone.
        /// </summary>
        /// <param name="managerIndex">The index of the experience that will be loaded.</param>
        /// <param name="wallTopIndex">The index of the zone that will be loaded.</param>
        public bool LoadXP(int managerIndex, int wallTopIndex, Action onXPLoaded = null)
        {
            var zone = _room.GetZones<VirtualWallTopZone>().FirstOrDefault(x => x.index == wallTopIndex);
            XPManager[] managers = xpManagers;
            XPManager manager = managerIndex < managers.Length ? managers[managerIndex] : null;
            if (manager != null && zone != null)
                return LoadXP(manager, zone, onXPLoaded);
            return false;
        }

        /// <summary>
        /// Loads an experience into a wall top zone.
        /// </summary>
        /// <param name="manager">The manager of the experience that will be loaded.</param>
        /// <param name="zone">The wall top zone in which the experience will be loaded.</param>
        public bool LoadXP(XPManager manager, VirtualWallTopZone zone, Action onXPLoaded = null)
        {
            // No need to load the experiment if it's already there or if the manager is already loaded elsewhere.
            if (zone.manager != null || manager.visibility == XPVisibility.Visible)
                return false;
            var vhzone = _room.GetZones<VirtualHologramZone>().FirstOrDefault(x => x.index == zone.index && x.xpZone == null);
            if (vhzone == null)
                vhzone = _room.GetZones<VirtualHologramZone>().FirstOrDefault(x => x.xpZone == null);
            logGeneralController.AddLog(string.Format("XP Loaded: {0}", manager.xpContext.contextName), this, Log.LogType.Important);
            _roomAnimator.InstallTubex(zone.index, manager, () => {
                manager.Show(zone, vhzone);
                if (onXPLoaded != null)
                    onXPLoaded.Invoke();
            });
            return true;
        }

        /// <summary>
        /// Swaps the current active hologram to the indexed one.
        /// </summary>
        /// <param name="index">The index of the hologram to swap to. If there's none, nothing happens.</param>
        public void SwapHologram(int index)
        {
            _hologramManager.Enable();
            _hologramManager.SwapHologram(index);
        }

        /// <summary>
        /// Hides the current active hologram.
        /// </summary>
        public void HideHologram()
        {
            _hologramManager.HideHologram();
        }

        /// <summary>
        /// Changes the state of the game.
        /// </summary>
        /// <param name="state"></param>
        public void SetGameState(GameState state)
        {
            _currentState = state;
            switch (state)
            {
                case GameState.Default:
                    if (_previousState != GameState.Default)
                        _roomAnimator.ActivateAllLights();
                    if (_previousState == GameState.Alarm)
                        _roomAnimator.DeactivateAlarm();
                    if (_previousState == GameState.Dark)
                    {
                        _hologramManager.Enable();
                        _holocube.PowerUp();
                        _comScreen.gameObject.SetActive(true);
                    }
                    break;
                case GameState.Alarm:
                    if (_previousState != GameState.Alarm)
                        _roomAnimator.ActivateAlarm();
                    if (_previousState == GameState.Default)
                        _roomAnimator.DeactivateAllLights();
                    if (_previousState == GameState.Dark)
                    {
                        _hologramManager.Enable();
                        _holocube.PowerUp();
                        _comScreen.gameObject.SetActive(true);
                    }
                    break;
                case GameState.Dark:
                    if (_previousState == GameState.Alarm)
                        _roomAnimator.DeactivateAlarm();
                    if (_previousState != GameState.Dark)
                    {
                        _roomAnimator.DeactivateAllLights();
                        _hologramManager.Disable();
                        _holocube.PowerDown();
                        _comScreen.gameObject.SetActive(false);
                    }
                    break;
            }
            _previousState = _currentState;
        }

        public void SendHintToPlayers(string hint)
        {
            logGeneralController.AddLog(string.Format("Hint sent to players: \"{0}\"", hint), this, Log.LogType.Automatic);
            Debug.Log(hint);
        }

        public void StartGame(bool[] starting)
        {
            for (int i = 0; i < starting.Length; i++)
            {
                if (starting[i] && xpManagers[i].state == XPState.Inactive)
                {
                    xpManagers[i].Activate();
                }
            }
        }

        public void TurnLightOn()
        {
            logGeneralController.AddLog("Turn Light On", this, Log.LogType.Automatic);
        }

        public void TurnLightOff()
        {
            logGeneralController.AddLog("Turn Light Off", this, Log.LogType.Automatic);
        }

        public void PlaySound(PlayableSound source)
        {
            globalSoundManager.Play(source);
            logGeneralController.AddLog(string.Format("Play Sound <{0}>", source.clip.name), this, Log.LogType.Automatic);
        }

        public void PlayMusic(PlayableMusic source)
        {
            globalSoundManager.Play(source);
            logGeneralController.AddLog(string.Format("Play Music <{0}>", source.clip.name), this, Log.LogType.Automatic);
        }

        public void StopMusic()
        {
            globalSoundManager.StopAllMusic();
            logGeneralController.AddLog(string.Format("Stop Music"), this, Log.LogType.Automatic);
        }
    }
}
