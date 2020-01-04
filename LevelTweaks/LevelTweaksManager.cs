using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using HMUI;
using IPA.Utilities;
using Polyglot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LevelTweaks
{
    public class LevelTweaksManager : NotifiableSingleton<LevelTweaksManager>
    {
        public static LevelTweaksManager Instance { get; set; }

        //public IDifficultyBeatmap CurrentlySelectedDifficultyBeatmap { get; set; }
        public StandardLevelDetailView standardView;

        [UIParams] public BSMLParserParams parserParams;
        [UIComponent("LTB")] public Button lt_button;
        [UIComponent("modal")] public ModalView modal;
        [UIComponent("list")] public CustomListTableData list;

        private bool newInteractable = true;
        [UIValue("new-interactable")]
        public bool NewInteractable
        {
            get => newInteractable;
            set
            {
                newInteractable = value;
                NotifyPropertyChanged();

            }
        }

        private bool deleteInteractable = false;
        [UIValue("del-interactable")]
        public bool DeleteInteractable
        {
            get => deleteInteractable;
            set
            {
                deleteInteractable = value;
                NotifyPropertyChanged();
            }
        }

        [UIValue("v_offset")]
        public float offset {
            get => _offset;
            set
            {
                _offset = value;
                NotifyPropertyChanged();
                UpdateThingy();
            }
        }
        private float _offset;

        [UIValue("v_njs")]
        public float njs {
            get => _njs;
            set
            {
                _njs = value;
                NotifyPropertyChanged();
                UpdateThingy();
            }
        }
        private float _njs;

        public void UpdateThingy()
        {
            if (curSel != 0)
            {
                var cell = list.data[curSel] as LevelTweak;
                cell.njs = njs;
                cell.offset = offset;
            }
        }

        int curSel = 0;

        [UIAction("newclicked")]
        public void NewClicked()
        {
            var beatmap = standardView.selectedDifficultyBeatmap;
            list.data.Add(new LevelTweak(new LevelTweakInfo() { njs = beatmap.noteJumpMovementSpeed, offset = beatmap.noteJumpStartBeatOffset, name = "Custom" }));
            list.tableView.ReloadData();
            list.tableView.SelectCellWithIdx(list.data.Count - 1, true);
            UpdateSliders();
            DeleteInteractable = list.data.Count != 1;
        }

        [UIAction("delclicked")]
        public void DelClicked()
        {
            if ((list.data[curSel] as LevelTweak).text.ToLower().Contains("default"))
                return;
            list.data.Remove(list.data[curSel]);
            list.tableView.ReloadData();
            list.tableView.SelectCellWithIdx(0, true);
            UpdateSliders();
            DeleteInteractable = false;
        }

        [UIAction("#post-parse")]
        public void Parsed()
        {
            
        }

        [UIAction("s_list")]
        public void Selected(TableView view, int pos)
        {
            
            curSel = pos;
            var beatmap = standardView.selectedDifficultyBeatmap;
            if (pos == 0)
            {
                njs = beatmap.noteJumpMovementSpeed;
                offset = beatmap.noteJumpStartBeatOffset;
                DeleteInteractable = false;
                UpdateSliders();
                return;
            }
            DeleteInteractable = true;
            njs = (list.data[pos] as LevelTweak).njs;
            offset = (list.data[pos] as LevelTweak).offset;
            Logger.log.Info($"offset: {offset}, local offset: {(list.data[pos] as LevelTweak).offset}, name: {(list.data[pos] as LevelTweak).text}");
            UpdateSliders();
        }

        public void UpdateInfo()
        {
            var beatmap = standardView.selectedDifficultyBeatmap;
            njs = beatmap.noteJumpMovementSpeed;
            offset = beatmap.noteJumpStartBeatOffset;
            list.data.Clear();
            list.data.Add(new LevelTweak(new LevelTweakInfo() { njs = beatmap.noteJumpMovementSpeed, offset = beatmap.noteJumpStartBeatOffset, name = "Default" }));
            list.tableView.ReloadData();
            list.tableView.SelectCellWithIdx(0, true);
        }

        public void UpdateSliders()
        {
            var s = modal.GetComponentInChildren<TimeSlider>();
            s.value = offset;
            var s2 = modal.GetComponentsInChildren<TimeSlider>().Last();
            s2.value = njs;
        }

        void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }

        internal void SetupButton(StandardLevelDetailView view)
        {
            standardView = view;
            var standardLevel = Resources.FindObjectsOfTypeAll<StandardLevelDetailViewController>().First();
            SetupDialog(standardLevel.transform.Find("LevelDetail").gameObject);
            lt_button.transform.localScale *= .5f;

        }

        internal void SetupDialog(GameObject source) => BSMLParser.instance.Parse(BeatSaberMarkupLanguage.Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "LevelTweaks.Views.dialog.bsml"), source, this);
        
        [UIAction("button-clicked")]
        public void Show()
        {
            UpdateInfo();
            Logger.log.Info(standardView.selectedDifficultyBeatmap.level.songName);
        }
        
        public void ShowDialog()
        {
            
            parserParams.EmitEvent("open-modal");
        }
        public void HideDialog() => parserParams.EmitEvent("close-modal");
        
    }
}
