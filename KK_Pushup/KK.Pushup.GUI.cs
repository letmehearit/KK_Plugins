﻿using BepInEx;
using ChaCustom;
using KKAPI.Maker;
using KKAPI.Maker.UI;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KK_Plugins
{
    [BepInPlugin(GUID, PluginName, Version)]
    [BepInDependency(Pushup.GUID)]
    [BepInProcess(Constants.MainGameProcessName)]
    [BepInProcess(Constants.MainGameProcessNameSteam)]
    public class PushupGUI : BaseUnityPlugin
    {
        public const string GUID = Pushup.GUID + ".gui";
        public const string PluginName = Pushup.PluginName + " GUI";
        public const string Version = Pushup.Version;

        //Sliders and toggles
        internal static MakerToggle EnablePushUpToggle;

        internal static PushUpSlider FirmnessSlider;
        internal static PushUpSlider LiftSlider;
        internal static PushUpSlider PushTogetherSlider;
        internal static PushUpSlider SqueezeSlider;
        internal static PushUpSlider CenterSlider;

        internal static MakerToggle FlattenNippleToggle;

        internal static MakerToggle AdvancedModeToggle;

        internal static PushUpSlider PushSizeSlider;
        internal static PushUpSlider PushVerticalPositionSlider;
        internal static PushUpSlider PushHorizontalAngleSlider;
        internal static PushUpSlider PushHorizontalPositionSlider;
        internal static PushUpSlider PushVerticalAngleSlider;
        internal static PushUpSlider PushDepthSlider;
        internal static PushUpSlider PushRoundnessSlider;

        internal static PushUpSlider PushSoftnessSlider;
        internal static PushUpSlider PushWeightSlider;

        internal static PushUpSlider PushAreolaDepthSlider;
        internal static PushUpSlider PushNippleWidthSlider;
        internal static PushUpSlider PushNippleDepthSlider;

        internal static MakerRadioButtons SelectButtons;

        private Pushup.PushupInfo _pushUpInfo;

        private Pushup.PushupController _pushUpController;
        private SliderManager _sliderManager;

        private Pushup.ClothData _activeClothData;

        internal void Start()
        {
            MakerAPI.RegisterCustomSubCategories += RegisterCustomSubCategories;
            MakerAPI.ReloadCustomInterface += (sender, args) => ReLoadPushUp();
            MakerAPI.MakerExiting += MakerExiting;
            MakerAPI.MakerFinishedLoading += MakerFinishedLoading;
        }

        private void MakerFinishedLoading(object sender, EventArgs e)
        {
            ReLoadPushUp();

            GameObject tglBreast = GameObject.Find("CustomScene/CustomRoot/FrontUIGroup/CustomUIGroup/CvsMenuTree/01_BodyTop/tglBreast");
            var tglBreastTrigger = tglBreast.GetOrAddComponent<EventTrigger>();
            var tglBreastEntry = new EventTrigger.Entry();
            tglBreastEntry.eventID = EventTriggerType.PointerEnter;
            tglBreastEntry.callback.AddListener(x => SliderManager.SlidersActive = true);
            tglBreastTrigger.triggers.Add(tglBreastEntry);

            GameObject tglPushup = GameObject.Find("CustomScene/CustomRoot/FrontUIGroup/CustomUIGroup/CvsMenuTree/03_ClothesTop/tglPushup");
            var tglPushupTrigger = tglPushup.GetOrAddComponent<EventTrigger>();
            var tglPushupEntry = new EventTrigger.Entry();
            tglPushupEntry.eventID = EventTriggerType.PointerEnter;
            tglPushupEntry.callback.AddListener(x => SliderManager.SlidersActive = false);
            tglPushupTrigger.triggers.Add(tglPushupEntry);
        }

        private void ReLoadPushUp()
        {
            _sliderManager = new SliderManager();

            _pushUpController = GetMakerController();
            _pushUpInfo = _pushUpController.CurrentInfo;
            _activeClothData = SelectButtons.Value == 0 ? _pushUpInfo.Bra : _pushUpInfo.Top;

            _sliderManager.InitSliders(_pushUpController);

            UpdateToggleSubscription(EnablePushUpToggle, _activeClothData.EnablePushUp, b => { _activeClothData.EnablePushUp = b; });

            UpdateSliderSubscription(PushSizeSlider, _activeClothData.Size, f => { _activeClothData.Size = f; });
            UpdateSliderSubscription(PushVerticalPositionSlider, _activeClothData.VerticalPosition, f => { _activeClothData.VerticalPosition = f; });
            UpdateSliderSubscription(PushHorizontalAngleSlider, _activeClothData.HorizontalAngle, f => { _activeClothData.HorizontalAngle = f; });
            UpdateSliderSubscription(PushHorizontalPositionSlider, _activeClothData.HorizontalPosition, f => { _activeClothData.HorizontalPosition = f; });
            UpdateSliderSubscription(PushVerticalAngleSlider, _activeClothData.VerticalAngle, f => { _activeClothData.VerticalAngle = f; });
            UpdateSliderSubscription(PushDepthSlider, _activeClothData.Depth, f => { _activeClothData.Depth = f; });
            UpdateSliderSubscription(PushRoundnessSlider, _activeClothData.Roundness, f => { _activeClothData.Roundness = f; });

            UpdateSliderSubscription(PushSoftnessSlider, _activeClothData.Softness, f => { _activeClothData.Softness = f; });
            UpdateSliderSubscription(PushWeightSlider, _activeClothData.Weight, f => { _activeClothData.Weight = f; });

            UpdateSliderSubscription(PushAreolaDepthSlider, _activeClothData.AreolaDepth, f => { _activeClothData.AreolaDepth = f; });
            UpdateSliderSubscription(PushNippleWidthSlider, _activeClothData.NippleWidth, f => { _activeClothData.NippleWidth = f; });
            UpdateSliderSubscription(PushNippleDepthSlider, _activeClothData.NippleDepth, f => { _activeClothData.NippleDepth = f; });

            UpdateSliderSubscription(FirmnessSlider, _activeClothData.Firmness, f => { _activeClothData.Firmness = f; });
            UpdateSliderSubscription(LiftSlider, _activeClothData.Lift, f => { _activeClothData.Lift = f; });
            UpdateSliderSubscription(PushTogetherSlider, _activeClothData.PushTogether, f => { _activeClothData.PushTogether = f; });
            UpdateSliderSubscription(SqueezeSlider, _activeClothData.Squeeze, f => { _activeClothData.Squeeze = f; });
            UpdateSliderSubscription(CenterSlider, _activeClothData.CenterNipples, f => { _activeClothData.CenterNipples = f; });

            UpdateToggleSubscription(FlattenNippleToggle, _activeClothData.FlattenNipples, b => { _activeClothData.FlattenNipples = b; });

            UpdateToggleSubscription(AdvancedModeToggle, _activeClothData.UseAdvanced, b => { _activeClothData.UseAdvanced = b; });
        }

        private void UpdateToggleSubscription(MakerToggle toggle, bool value, Action<bool> action)
        {
            var pushObserver = Observer.Create<bool>(b =>
            {
                action(b);
                _pushUpController.RecalculateBody();
            });

            toggle.ValueChanged.Subscribe(pushObserver);
            toggle.SetValue(value);
        }

        private void UpdateSliderSubscription(PushUpSlider slider, float value, Action<float> action)
        {
            slider.onUpdate = f =>
            {
                action(f);
                _pushUpController.RecalculateBody();
            };

            var pushObserver = Observer.Create<float>(f => slider.Update(f));

            slider.MakerSlider.ValueChanged.Subscribe(pushObserver);
            slider.MakerSlider.SetValue(value);
        }

        private void MakerExiting(object sender, EventArgs e)
        {
            _pushUpInfo = null;
            _pushUpController = null;
            _sliderManager = null;
        }

        private static Pushup.PushupController GetMakerController() => MakerAPI.GetCharacterControl().gameObject.GetComponent<Pushup.PushupController>();

        private void RegisterCustomSubCategories(object sender, RegisterSubCategoriesEvent ev)
        {
            MakerCategory category = new MakerCategory("03_ClothesTop", "tglPushup", MakerConstants.Clothes.Bra.Position + 1, "Pushup");

            //Bra or top
            SelectButtons = ev.AddControl(new MakerRadioButtons(category, this, "Type", "Bra", "Top"));
            SelectButtons.ValueChanged.Subscribe(i => ReLoadPushUp());

            //Basic mode
            EnablePushUpToggle = new MakerToggle(category, "Enabled", true, this);
            ev.AddControl(EnablePushUpToggle);

            FirmnessSlider = MakeSlider(category, "Firmness", ev, Pushup.ConfigFirmnessDefault.Value);
            LiftSlider = MakeSlider(category, "Lift", ev, Pushup.ConfigLiftDefault.Value);
            PushTogetherSlider = MakeSlider(category, "Push Together", ev, Pushup.ConfigPushTogetherDefault.Value);
            SqueezeSlider = MakeSlider(category, "Squeeze", ev, Pushup.ConfigSqueezeDefault.Value);
            CenterSlider = MakeSlider(category, "Center Nipples", ev, Pushup.ConfigNippleCenteringDefault.Value);

            FlattenNippleToggle = new MakerToggle(category, "Flatten Nipples", true, this);
            ev.AddControl(FlattenNippleToggle);

            //Advanced mode
            ev.AddControl(new MakerSeparator(category, this));

            AdvancedModeToggle = new MakerToggle(category, "Advanced Mode", false, this);
            ev.AddControl(AdvancedModeToggle);

            var copyBodyButton = new MakerButton("Copy Body to Advanced", category, this);
            ev.AddControl(copyBodyButton);
            copyBodyButton.OnClick.AddListener(CopyBodyToSliders);

            var copyBasicButton = new MakerButton("Copy Basic to Advanced", category, this);
            ev.AddControl(copyBasicButton);
            copyBasicButton.OnClick.AddListener(CopyBasicToSliders);

            PushSizeSlider = MakeSlider(category, "Size", ev, Singleton<CustomBase>.Instance.defChaInfo.custom.body.shapeValueBody[Pushup.PushupConstants.IndexSize]);
            PushVerticalPositionSlider = MakeSlider(category, "Vertical Position", ev, Singleton<CustomBase>.Instance.defChaInfo.custom.body.shapeValueBody[Pushup.PushupConstants.IndexVerticalPosition]);
            PushHorizontalAngleSlider = MakeSlider(category, "Horizontal Angle", ev, Singleton<CustomBase>.Instance.defChaInfo.custom.body.shapeValueBody[Pushup.PushupConstants.IndexHorizontalAngle]);
            PushHorizontalPositionSlider = MakeSlider(category, "Horizontal Position", ev, Singleton<CustomBase>.Instance.defChaInfo.custom.body.shapeValueBody[Pushup.PushupConstants.IndexHorizontalPosition]);
            PushVerticalAngleSlider = MakeSlider(category, "Vertical Angle", ev, Singleton<CustomBase>.Instance.defChaInfo.custom.body.shapeValueBody[Pushup.PushupConstants.IndexVerticalAngle]);
            PushDepthSlider = MakeSlider(category, "Depth", ev, Singleton<CustomBase>.Instance.defChaInfo.custom.body.shapeValueBody[Pushup.PushupConstants.IndexDepth]);
            PushRoundnessSlider = MakeSlider(category, "Roundness", ev, Singleton<CustomBase>.Instance.defChaInfo.custom.body.shapeValueBody[Pushup.PushupConstants.IndexRoundness]);

            PushSoftnessSlider = MakeSlider(category, "Softness", ev, Singleton<CustomBase>.Instance.defChaInfo.custom.body.bustSoftness);
            PushWeightSlider = MakeSlider(category, "Weight", ev, Singleton<CustomBase>.Instance.defChaInfo.custom.body.bustWeight);

            PushAreolaDepthSlider = MakeSlider(category, "Areola Depth", ev, Singleton<CustomBase>.Instance.defChaInfo.custom.body.shapeValueBody[Pushup.PushupConstants.IndexAreolaDepth]);
            PushNippleWidthSlider = MakeSlider(category, "Nipple Width", ev, Singleton<CustomBase>.Instance.defChaInfo.custom.body.shapeValueBody[Pushup.PushupConstants.IndexNippleWidth]);
            PushNippleDepthSlider = MakeSlider(category, "Nipple Depth", ev, Singleton<CustomBase>.Instance.defChaInfo.custom.body.shapeValueBody[Pushup.PushupConstants.IndexNippleDepth]);

            ev.AddSubCategory(category);
        }

        private void CopyBodyToSliders() => copyToSliders(_pushUpController.CurrentInfo.BaseData);

        private void CopyBasicToSliders()
        {
            _pushUpInfo.CalculatePushFromClothes(_activeClothData, false);
            copyToSliders(_pushUpController.CurrentInfo.PushupData);
        }

        private void copyToSliders(Pushup.BodyData infoBase)
        {
            PushSoftnessSlider.MakerSlider.SetValue(infoBase.Softness);
            PushWeightSlider.MakerSlider.SetValue(infoBase.Weight);

            PushSizeSlider.MakerSlider.SetValue(infoBase.Size);
            PushVerticalPositionSlider.MakerSlider.SetValue(infoBase.VerticalPosition);
            PushHorizontalAngleSlider.MakerSlider.SetValue(infoBase.HorizontalAngle);
            PushHorizontalPositionSlider.MakerSlider.SetValue(infoBase.HorizontalPosition);
            PushVerticalAngleSlider.MakerSlider.SetValue(infoBase.VerticalAngle);
            PushDepthSlider.MakerSlider.SetValue(infoBase.Depth);
            PushRoundnessSlider.MakerSlider.SetValue(infoBase.Roundness);
            PushAreolaDepthSlider.MakerSlider.SetValue(infoBase.AreolaDepth);
            PushNippleWidthSlider.MakerSlider.SetValue(infoBase.NippleWidth);
            PushNippleDepthSlider.MakerSlider.SetValue(infoBase.NippleDepth);
        }

        private PushUpSlider MakeSlider(MakerCategory category, string sliderName, RegisterSubCategoriesEvent e, float defaultValue)
        {
            var slider = new MakerSlider(category, sliderName, 0f, 1f, defaultValue, this);
            e.AddControl(slider);
            var pushUpSlider = new PushUpSlider();
            pushUpSlider.MakerSlider = slider;

            return pushUpSlider;
        }
    }

    public class PushUpSlider
    {
        public MakerSlider MakerSlider;
        public Action<float> onUpdate;

        public void Update(float f) => onUpdate(f);
    }
}