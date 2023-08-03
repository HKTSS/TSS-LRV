using OpenBveApi.Runtime;
using OpenBveApi.Colors;
using System;

namespace Plugin {
    /// <summary>The interface to be implemented by the plugin.</summary>
    public partial class Plugin : IRuntime {
        internal static int[] Panel = null;
        
        internal static bool DoorOpened;
        internal static bool DoorOpened2 = true;
        internal static Util.LRVType LRVGeneration = Util.LRVType.P4;
        internal static int LastBrakeNotch = 5;
        internal static bool LockTreadBrake = false;
        internal static VehicleSpecs specs;
        internal static string Language = "en-us";
        internal static SpeedMode CurrentSpeedMode = SpeedMode.Normal;
        internal int SpeedLimit = 60;
        private double currentSpeed;
        private bool Crashed;
        private bool iSPSDoorLock;
        private bool DoorBrake;
        private bool Ready;
        private PAManager PAManager = new PAManager();
        public static IndicatorLight DirectionLight = IndicatorLight.None;

        /// <summary>Is called when the plugin is loaded.</summary>
        public bool Load(LoadProperties prop) {
            /* Initialize MessageManager, so we can print message on the top-left screen later. */
            MessageManager.Initialise(prop.AddMessage);
            /* Initialize an empty array with 256 elements, used for Panel Indicator. */
            Panel = new int[256];
            /* Initialize Sound, used to play sound later on. */
            SoundManager.Initialise(prop.PlaySound, prop.PlayCarSound, 256);
            prop.Panel = Panel;
            prop.FailureReason = "LRV plugin failed to load, some functions will be unavailable.";
            prop.AISupport = AISupport.Basic;

            return Config.LoadConfig(prop, LRVGeneration);
        }

        public void Unload() {
        }

        /// <summary>Is called after loading to inform the plugin about the specifications of the train.</summary>
        public void SetVehicleSpecs(VehicleSpecs specs) {
            Plugin.specs = specs;
            SetPanel(PanelIndices.FirstCarNumber, Util.CarNumPanel(Config.carNum1));
            SetPanel(PanelIndices.SecondCarNumber, Util.CarNumPanel(Config.carNum2));
        }

        /// <summary>Is called when the plugin should initialize, reinitialize or jumping stations.</summary>
        public void Initialize(InitializationModes mode) {
            PAManager.Load();
            ResetLRV(ResetType.JumpStation);
        }

        /// <summary>This is called every frame. If you have 60fps, then this method is called 60 times in 1 second</summary>
        public void Elapse(ElapseData data) {
            /* Get system language, used for displaying train settings dialog later. */
            Ready = true;
            Language = data.CurrentLanguageCode;
            currentSpeed = data.Vehicle.Speed.KilometersPerHour;

            CameraManager.Update(data);
            StationManager.Update(data);
            ReporterLED.Update(data);
            AIManager.Elapse(data);
            PAManager.Elapse(data);

            /* Lock the door above 2 km/h */
            if (currentSpeed > 2 && Config.doorlockEnabled) {
                data.DoorInterlockState = DoorInterlockStates.Locked;
            } else {
                data.DoorInterlockState = DoorInterlockStates.Unlocked;
            }

            if (currentSpeed > SpeedLimit - 5) {
                SetPanel(PanelIndices.iSPSOverSpeed, 1);
            } else {
                SetPanel(PanelIndices.iSPSOverSpeed, 0);
            }

            if ((DoorBrake && Config.doorApplyBrake) || (iSPSDoorLock && Config.iSPSEnabled)) {
                data.Handles.PowerNotch = 0;
                data.Handles.BrakeNotch = specs.B67Notch;
            }

            if (DirectionLight == IndicatorLight.Left) {
                SetPanel(PanelIndices.Indicator, 1);
            } else if (DirectionLight == IndicatorLight.Right) {
                SetPanel(PanelIndices.Indicator, 2);
            } else if (DirectionLight == IndicatorLight.Both) {
                SetPanel(PanelIndices.Indicator, 3);
            } else {
                SetPanel(PanelIndices.Indicator, 0);
            }

            if (Config.tutorialMode) {
                if (Language.StartsWith("zh")) {
                    Panel[PanelIndices.TutorialModeChin] = 1;
                } else {
                    Panel[PanelIndices.TutorialModeEng] = 1;
                }
            } else {
                Panel[PanelIndices.TutorialModeChin] = 0;
                Panel[PanelIndices.TutorialModeEng] = 0;
            }

            /* Clamp the power notch to P1 on slow mode. */
            if (CurrentSpeedMode == SpeedMode.Slow && data.Handles.PowerNotch > 1) {
                data.Handles.PowerNotch = 1;
            } else if (CurrentSpeedMode != SpeedMode.Fast && data.Handles.PowerNotch == specs.PowerNotches) {
                /* We reserved the last notch for the fast (aka "Elephant") mode. If the current speed mode is not fast and current power notch is the last notch: Clamp it to last notch - 1 */
                data.Handles.PowerNotch = specs.PowerNotches - 1;
            }

            if (data.PrecedingVehicle != null) {
                if (Config.crashEnabled && Crashed == false && data.PrecedingVehicle.Distance < 0.1 && data.PrecedingVehicle.Distance > -4) {
                    /* Crash Sounds */
                    SoundManager.Play(SoundIndices.Crash, 1.0, 1.0, false);

                    if (Math.Abs(data.PrecedingVehicle.Speed.KilometersPerHour - currentSpeed) > 10) {
                        Panel[213] = 1;
                        data.HeadlightState = 1;

                        if (Math.Abs(data.PrecedingVehicle.Speed.KilometersPerHour - currentSpeed) > 17) {
                            SetPanel(PanelIndices.SpeedometerLight, 1);
                            DirectionLight = IndicatorLight.None;
                            SetPanel(PanelIndices.Indicator, 0);
                        }
                    }
                    Crashed = true;
                }
            }

            if (StationManager.approachingStation && currentSpeed < 0.1 && DoorOpened2 == false) {
                Panel[202] = 1;
                /* If the reverser is Forward */
                if (data.Handles.Reverser == 1) {
                    iSPSDoorLock = true;
                } else {
                    if (Config.allowReversingInStations) {
                        iSPSDoorLock = false;
                    }
                }
            }

            if (currentSpeed > 10 && Panel[202] == 1) {
                Panel[202] = 0;
            }

            /* Turn signal sound in cab */
            if (DirectionLight != IndicatorLight.None) {
                if (CameraManager.isInCab()) {
                    SoundManager.Play(SoundIndices.CabDirIndicator, 1.0, 1.0, true);
                } else {
                    SoundManager.Stop(SoundIndices.CabDirIndicator);
                }
            } else {
                SoundManager.Stop(SoundIndices.CabDirIndicator);
            }

            SetPanel(PanelIndices.TrainStatus, Config.trainStatus);
        }

        public void SetReverser(int reverser) {
        }

        public void SetPower(int notch) {
            if(notch % 2 == 0 && CameraManager.isInCab() && Ready) {
                SoundManager.Play(SoundIndices.powerHandleClick, 1.0, 1.0, false);
            }
        }

        public void SetBrake(int notch) {
            if (LastBrakeNotch == 0 && notch > 0 && currentSpeed > 15) {
                SoundManager.PlayAllCar(SoundIndices.StartBrake, 1.0, 1.0, false);
            }

            if (notch % 2 == 0 && CameraManager.isInCab() && Ready) {
                SoundManager.Play(SoundIndices.powerHandleClick, 1.0, 1.0, false);
            }

            LastBrakeNotch = notch;
        }

        /// <summary>Is called when a virtual key is pressed.</summary>
        public void KeyDown(VirtualKeys key) {
            VirtualKeys virtualKey = key;

            switch (virtualKey) {
                /* GearDown = Ctrl + G */
                case VirtualKeys.GearDown:
                    ConfigForm.LaunchForm();
                    break;
                case VirtualKeys.A1:
                    ResetLRV(0);
                    break;
                case VirtualKeys.A2:
                    if (CameraManager.isInCab()) SoundManager.Play(SoundIndices.Click, 1.0, 1.0, false);
                    if ((int)CurrentSpeedMode == 2) CurrentSpeedMode = SpeedMode.Normal;
                    else CurrentSpeedMode++; Panel[PanelIndices.SpeedModeSwitch] = (int)CurrentSpeedMode;
                    break;
                case VirtualKeys.B1:
                    Panel[PanelIndices.TreadBrake] = (Panel[PanelIndices.TreadBrake] + 1) % 2;
                    break;
                case VirtualKeys.D:
                    ToggleDirectionIndicator(IndicatorLight.Left);
                    break;
                case VirtualKeys.E:
                    ToggleDirectionIndicator(IndicatorLight.Right);
                    break;
                case VirtualKeys.F:
                    SetPanel(PanelIndices.DestinationLED, Panel[PanelIndices.DestinationLED]+1);
                    break;
                case VirtualKeys.G:
                    Panel[PanelIndices.Digit1]++;
                    break;
                case VirtualKeys.H:
                    Panel[PanelIndices.Digit2]++;
                    break;
                case VirtualKeys.I:
                    Panel[PanelIndices.Digit3]++;
                    break;
                case VirtualKeys.K:
                    Panel[PanelIndices.CabDoor] ^= 1;
                    break;
                case VirtualKeys.L:
                    Panel[PanelIndices.SpeedometerLight] ^= 1;
                    SoundManager.PlayCabPanelClickSound();
                    break;
                case VirtualKeys.J:
                    Panel[PanelIndices.LightToggle] ^= 1;
                    SoundManager.PlayCabPanelClickSound();
                    break;
                case VirtualKeys.WiperSpeedUp:
                    SoundManager.PlayCabPanelClickSound();
                    Panel[PanelIndices.WiperMode] = Math.Min(Panel[PanelIndices.WiperMode] + 1, Enum.GetNames(typeof(WiperMode)).Length - 1);
                    break;
                case VirtualKeys.WiperSpeedDown:
                    SoundManager.PlayCabPanelClickSound();
                    Panel[PanelIndices.WiperMode] = Math.Max(Panel[PanelIndices.WiperMode] - 1, 0);
                    break;
                case VirtualKeys.LeftDoors:
                    PAManager.KeyDown();
                    if (DoorOpened && Config.mtrBeeping) {
                        if (SoundManager.IsPlaying(SoundIndices.MTRBeep)) {
                            SoundManager.Stop(SoundIndices.MTRBeep);
                        } else {
                            SoundManager.PlayCar(SoundIndices.MTRBeep, 2.0, 1.0, false, 0);
                            if (specs.Cars == 2) SoundManager.PlayCar(SoundIndices.MTRBeep, 2.0, 1.0, false, 1);
                        }
                    }
                    break;
                case VirtualKeys.MainBreaker:
                    ToggleDirectionIndicator(IndicatorLight.Both);
                    break;
                case VirtualKeys.Headlights:
                    SoundManager.PlayCabPanelClickSound();
                    break;
            }
        }

        /// <summary>Is called when a virtual key is released.</summary>
        public void KeyUp(VirtualKeys key) {
        }

        public void HornBlow(HornTypes type) {
        }

        /// <summary>Is called when the state of the doors changes.</summary>
        public void DoorChange(DoorStates oldState, DoorStates newState) {
            /* Door is opened */
            if (oldState == DoorStates.None & newState != DoorStates.None) {
                DoorOpened = true;
                DoorOpened2 = true;
                DoorBrake = true;
                /* Door is closed */
            } else if (oldState != DoorStates.None & newState == DoorStates.None) {
                DoorOpened = false;
                Panel[204] = 0;
                StationManager.approachingStation = false;
                iSPSDoorLock = false;
                DoorBrake = false;
            }
        }

        public void SetSignal(SignalData[] signal) {
        }

        /// <summary>Is called when the train passes a beacon.</summary>
        /// <param name="beacon">The beacon data.</param>
        public void SetBeacon(BeaconData beacon) {
            AIManager.SetBeacon(beacon);
            switch (beacon.Type) {
                case BeaconIndices.SpeedLimit:
                    if (beacon.Optional > 0) SpeedLimit = beacon.Optional;
                    break;
            }
        }

        public void PerformAI(AIData data) {
            AIManager.PerformAI(data);
        }

        public static void SetPanel(int index, int val) {
            Panel[index] = val;
        }

        public static void ToggleDirectionIndicator(IndicatorLight direction) {
            if(DirectionLight == direction) {
                SetDirectionIndicator(IndicatorLight.None);
            } else {
                if (direction == IndicatorLight.Both) {
                    if (DirectionLight == IndicatorLight.Left || DirectionLight == IndicatorLight.Right)
                    {
                        MessageManager.PrintMessage(Messages.getTranslation("gameMsg.turnOffTurnSignal"), MessageColor.Orange, 5.0);
                        return;
                    }
                }
                SetDirectionIndicator(direction);
            }
        }

        public static void SetDirectionIndicator(IndicatorLight newDirection) {
            DirectionLight = newDirection;

            if (newDirection == IndicatorLight.Both) {
                SetPanel(PanelIndices.DirBoth, 1);
            } else {
                SetPanel(PanelIndices.DirBoth, 0);
            }

            if (newDirection == IndicatorLight.Left) {
                SetPanel(PanelIndices.Indicator, 1);
            } else if (newDirection == IndicatorLight.Right) {
                SetPanel(PanelIndices.Indicator, 2);
            } else if (newDirection == IndicatorLight.Both) {
                SetPanel(PanelIndices.Indicator, 3);
            } else {
                SetPanel(PanelIndices.Indicator, 0);
            }

            SoundManager.PlayCabPanelClickSound();
        }

        internal void ResetLRV(ResetType mode) {
            if (mode == ResetType.JumpStation) {
                DoorOpened2 = true;
                DoorBrake = true;
                iSPSDoorLock = false;
                if (Crashed) {
                    Crashed = false;
                    SetPanel(PanelIndices.SpeedometerLight, 0);
                    Panel[213] = 0;
                }
            }
            AIManager.ResetLRV(mode);
            StationManager.approachingStation = false;
            iSPSDoorLock = false;
            DoorBrake = false;
            Panel[202] = 0;
            Panel[203] = 0;
        }

        internal static void ChangeCarNumber(int car, int states) {
            if (car == 1) {
                Panel[PanelIndices.FirstCarNumber] = states;
            } else {
                Panel[PanelIndices.SecondCarNumber] = states;
            }
        }
    }

    public enum ResetType {
        JumpStation,
        ManualReset,
    }

    public enum IndicatorLight {
        Left,
        Right,
        Both,
        None
    }

    enum SpeedMode {
        Normal,
        Fast,
        Slow
    }

    enum WiperMode { 
        Stopped,
        Normal,
        Fast
    }
}