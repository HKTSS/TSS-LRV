using OpenBveApi.Runtime;
using OpenBveApi.Colors;
using Plugin.Managers;
using System;

namespace Plugin {
    /// <summary>The interface to be implemented by the plugin.</summary>
    public partial class Plugin : IRuntime {
        private double currentSpeed;
        private bool iSPSDoorLock;
        private bool DoorBrake;
        private bool Ready;
        private System.Reflection.MethodInfo setHeadLightMethod = typeof(ElapseData).GetMethod("set_HeadlightState");
        internal static bool DoorOpened;
        internal static bool DoorOpened2 = true;
        internal static Util.LRVType LRVGeneration = Util.LRVType.P4;
        internal static int LastBrakeNotch = 5;
        internal static bool LockTreadBrake;
        internal static VehicleSpecs specs;
        internal static string Language = "en-us";
        internal static SpeedMode CurrentSpeedMode = SpeedMode.Normal;
        internal static IndicatorLight DirectionLight = IndicatorLight.None;
        internal static CrashState currentCrashState = CrashState.None;
        internal static int SpeedLimit = 60;

        /// <summary>Is called when the plugin is loaded.</summary>
        public bool Load(LoadProperties prop) {
            /* Initialize MessageManager, so we can print message on the top-left screen later. */
            MessageManager.Initialize(prop.AddMessage);
            /* Initialize an empty array with 256 elements, used for Panel Indicator. */
            /* Initialize Sound, used to play sound later on. */
            SoundManager.Initialize(prop.PlaySound, prop.PlayCarSound, 256);
            int[] Panel = new int[256];
            PanelManager.Initialize(Panel);
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
        }

        /// <summary>Is called when the plugin should initialize, reinitialize or jumping stations.</summary>
        public void Initialize(InitializationModes mode) {
            PAManager.Initialize();
            PanelManager.Set(PanelIndices.FirstCarNumber, Util.CarNumPanel(Config.carNum1));
            PanelManager.Set(PanelIndices.SecondCarNumber, Util.CarNumPanel(Config.carNum2));
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
                PanelManager.Set(PanelIndices.iSPSOverSpeed, 1);
            } else {
                PanelManager.Set(PanelIndices.iSPSOverSpeed, 0);
            }

            if ((DoorBrake && Config.doorApplyBrake) || (iSPSDoorLock && Config.iSPSEnabled)) {
                data.Handles.PowerNotch = 0;
                data.Handles.BrakeNotch = specs.B67Notch;
            }

            if (DirectionLight == IndicatorLight.Left) {
                PanelManager.Set(PanelIndices.Indicator, 1);
            } else if (DirectionLight == IndicatorLight.Right) {
                PanelManager.Set(PanelIndices.Indicator, 2);
            } else if (DirectionLight == IndicatorLight.Both) {
                PanelManager.Set(PanelIndices.Indicator, 3);
            } else {
                PanelManager.Set(PanelIndices.Indicator, 0);
            }

            if (Config.tutorialMode) {
                if (Language.StartsWith("zh")) {
                    PanelManager.Set(PanelIndices.TutorialModeChin, 1);
                } else {
                    PanelManager.Set(PanelIndices.TutorialModeEng, 1);
                }
            } else {
                PanelManager.Set(PanelIndices.TutorialModeChin, 0);
                PanelManager.Set(PanelIndices.TutorialModeEng, 0);
            }

            /* Clamp the power notch to P1 on slow mode. */
            if (CurrentSpeedMode == SpeedMode.Slow && data.Handles.PowerNotch > 1) {
                data.Handles.PowerNotch = 1;
            } else if (CurrentSpeedMode != SpeedMode.Fast && data.Handles.PowerNotch == specs.PowerNotches) {
                /* We reserved the last notch for the fast (aka "Elephant") mode. If the current speed mode is not fast and current power notch is the last notch: Clamp it to last notch - 1 */
                data.Handles.PowerNotch = specs.PowerNotches - 1;
            }

            switch(currentCrashState) {
                case CrashState.Medium:
                    PanelManager.Set(PanelIndices.GlassCracked, 1);
                    SetHeadlightState(data, 2);
                    break;
                case CrashState.Severe:
                    PanelManager.Set(PanelIndices.GlassCracked, 1);
                    PanelManager.Set(PanelIndices.NoPower, 1);
                    DirectionLight = IndicatorLight.None;
                    PanelManager.Set(PanelIndices.Indicator, 0);
                    break;
            }

            // Handle collision with front train
            if (data.PrecedingVehicle != null && Config.crashEnabled) {
                if (currentCrashState == CrashState.None && data.PrecedingVehicle.Distance < 0.1 && data.PrecedingVehicle.Distance > -4) {
                    /* Crash Sounds */
                    SoundManager.Play(SoundIndices.Crash, 1.0, 1.0, false);
                    double collisionSpeed = Math.Abs(data.PrecedingVehicle.Speed.KilometersPerHour - currentSpeed);

                    if(collisionSpeed > 17) {
                        currentCrashState = CrashState.Severe;
                    } else if(collisionSpeed > 10) {
                        currentCrashState = CrashState.Medium;
                    } else {
                        currentCrashState = CrashState.Minor;
                    }
                }
            }

            if (StationManager.approachingStation && currentSpeed < 0.1 && DoorOpened2 == false) {
                PanelManager.Set(PanelIndices.DoorLockBlink, 1);
                /* If the reverser is Forward */
                if (data.Handles.Reverser == 1) {
                    iSPSDoorLock = true;
                } else {
                    if (Config.allowReversingInStations) {
                        iSPSDoorLock = false;
                    }
                }
            }

            if (currentSpeed > 10 && PanelManager.Get(PanelIndices.DoorLockBlink) == 1) {
                PanelManager.Set(PanelIndices.DoorLockBlink, 0);
            }

            /* Turn signal sound in cab */
            if (DirectionLight != IndicatorLight.None) {
                if (CameraManager.InCab()) {
                    SoundManager.Play(SoundIndices.CabDirIndicator, 1.0, 1.0, true);
                } else {
                    SoundManager.Stop(SoundIndices.CabDirIndicator);
                }
            } else {
                SoundManager.Stop(SoundIndices.CabDirIndicator);
            }

            PanelManager.Set(PanelIndices.TrainStatus, Config.trainStatus);
        }

        private void SetHeadlightState(ElapseData data, int state) {
            //HACK: Use reflection to call data.HeadlightState = 2
            //This is required or our plugin will throw an error in older OpenBVE Version
            //Because you know there will always be someone running this in older version despite being warned against so...
            if (setHeadLightMethod != null)
            {
                setHeadLightMethod.Invoke(data, new object[] { 2 });
            }
        }

        public void SetReverser(int reverser) {
        }

        public void SetPower(int notch) {
            if(notch % 2 == 0 && CameraManager.InCab() && Ready) {
                SoundManager.Play(SoundIndices.powerHandleClick, 1.0, 1.0, false);
            }
        }

        public void SetBrake(int notch) {
            if (LastBrakeNotch == 0 && notch > 0 && currentSpeed > 15) {
                SoundManager.PlayAllCar(SoundIndices.StartBrake, 1.0, 1.0, false);
            }

            if (notch % 2 == 0 && CameraManager.InCab() && Ready) {
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
                    if (CameraManager.InCab()) SoundManager.Play(SoundIndices.Click, 1.0, 1.0, false);

                    if ((int)CurrentSpeedMode == 2) {
                        CurrentSpeedMode = SpeedMode.Normal;
                    } else {
                        CurrentSpeedMode++;
                    }

                    PanelManager.Set(PanelIndices.SpeedModeSwitch, (int)CurrentSpeedMode);
                    break;
                case VirtualKeys.B1:
                    PanelManager.Set(PanelIndices.TreadBrake, (PanelManager.Get(PanelIndices.TreadBrake) + 1) % 2);
                    break;
                case VirtualKeys.D:
                    ToggleDirectionIndicator(IndicatorLight.Left);
                    break;
                case VirtualKeys.E:
                    ToggleDirectionIndicator(IndicatorLight.Right);
                    break;
                case VirtualKeys.F:
                    PanelManager.Increment(PanelIndices.DestinationLED);
                    break;
                case VirtualKeys.G:
                    PanelManager.Increment(PanelIndices.Digit1);
                    break;
                case VirtualKeys.H:
                    PanelManager.Increment(PanelIndices.Digit2);
                    break;
                case VirtualKeys.I:
                    PanelManager.Increment(PanelIndices.Digit3);
                    break;
                case VirtualKeys.K:
                    PanelManager.Toggle(PanelIndices.CabDoor);
                    break;
                case VirtualKeys.L:
                    PanelManager.Toggle(PanelIndices.SpeedometerLight);
                    SoundManager.PlayCabPanelClickSound();
                    break;
                case VirtualKeys.J:
                    PanelManager.Toggle(PanelIndices.LightToggle);
                    SoundManager.PlayCabPanelClickSound();
                    break;
                case VirtualKeys.WiperSpeedUp:
                    SoundManager.PlayCabPanelClickSound();
                    PanelManager.Increment(PanelIndices.WiperMode, Enum.GetNames(typeof(WiperMode)).Length - 1);
                    break;
                case VirtualKeys.WiperSpeedDown:
                    SoundManager.PlayCabPanelClickSound();
                    PanelManager.Decrement(PanelIndices.WiperMode, 0);
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
                PanelManager.Set(204, 0);
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
                PanelManager.Set(PanelIndices.DirBoth, 1);
            } else {
                PanelManager.Set(PanelIndices.DirBoth, 0);
            }

            if (newDirection == IndicatorLight.Left) {
                PanelManager.Set(PanelIndices.Indicator, 1);
            }
            if (newDirection == IndicatorLight.Right) {
                PanelManager.Set(PanelIndices.Indicator, 2);
            }
            if (newDirection == IndicatorLight.Both) {
                PanelManager.Set(PanelIndices.Indicator, 3);
            } else {
                PanelManager.Set(PanelIndices.Indicator, 0);
            }

            SoundManager.PlayCabPanelClickSound();
        }

        internal void ResetLRV(ResetType mode) {
            if (mode == ResetType.JumpStation) {
                DoorOpened2 = true;
                DoorBrake = true;
                iSPSDoorLock = false;
                if (currentCrashState != CrashState.None) {
                    currentCrashState = CrashState.None;
                    PanelManager.Set(PanelIndices.GlassCracked, 0);
                    PanelManager.Set(PanelIndices.SpeedometerLight, 0);
                    PanelManager.Set(PanelIndices.NoPower, 0);
                }
            }

            AIManager.ResetLRV(mode);
            StationManager.approachingStation = false;
            iSPSDoorLock = false;
            DoorBrake = false;
            PanelManager.Set(202, 0);
            PanelManager.Set(203, 0);
        }

        internal static void ChangeCarNumber(int car, int states) {
            if (car == 1) {
                PanelManager.Set(PanelIndices.FirstCarNumber, states);
            } else {
                PanelManager.Set(PanelIndices.SecondCarNumber, states);
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

    public enum SpeedMode {
        Normal,
        Fast,
        Slow
    }

    public enum WiperMode { 
        Stopped,
        Normal,
        Fast
    }

    public enum CrashState {
        None,
        Minor,
        Medium,
        Severe
    }
}