using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using System.Collections.Generic;
using LevelTweaks.Configuration;
using BS_Utils.Utilities;
using System.Collections;
using IPA.Utilities;
using UnityEngine;
using System.Linq;
using HMUI;
using System;

namespace LevelTweaks.UI
{
    public class LTUI : NotifiableSingleton<LTUI> // *fuck*
    {
        public StandardLevelDetailView detailView;

        public TweakCell selectedTweak = new TweakCell(new TweakData() { Name = "Loading...", NJS = -999f, Offset = -999f });
        public int selectedIndex;

        [UIComponent("tweak-list")]
        public CustomListTableData tweakList;

        [UIParams]
        public BSMLParserParams parserParams;

        //"if it works its not stupid" ~Caeden117
        private float _ypos = 0f;
        [UIValue("y-pos")]
        public float YPos
        {
            get => _ypos;
            set
            {
                _ypos = value;
                NotifyPropertyChanged();
            }
        }

        //"if it works its not stupid" ~Caeden117
        private float _y2pos = 3f;
        [UIValue("y2-pos")]
        public float Y2Pos
        {
            get => _y2pos;
            set
            {
                _y2pos = value;
                NotifyPropertyChanged();
            }
        }

        private bool _canDelete = false;
        [UIValue("can-delete")]
        public bool CanDelete
        {
            get => _canDelete;
            set
            {
                _canDelete = value;
                NotifyPropertyChanged();
            }
        }

        public string _message = "Level Tweaks";
        [UIValue("message")]
        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                NotifyPropertyChanged();
            }
        }

        public float _scuff = -999f;
        [UIValue("name")]
        public string Name
        {
            get => selectedTweak == null ? _scuff.ToString() : selectedTweak.data.Name;
            set
            {
                if (selectedIndex != 0)
                    selectedTweak.data.Name = value;
                NotifyPropertyChanged();
            }
        }

        [UIValue("njs")]
        public float NJS
        {
            get => selectedTweak == null ? _scuff : selectedTweak.data.NJS;
            set
            {
                if (selectedIndex != 0)
                    selectedTweak.data.NJS = (float)Math.Round(value, 2);
                NotifyPropertyChanged();
            }
        }

        [UIValue("offset")]
        public float Offset
        {
            get => selectedTweak == null ? _scuff : selectedTweak.data.Offset;
            set
            {
                if (selectedIndex != 0)
                    selectedTweak.data.Offset = (float)Math.Round(value, 2);
                NotifyPropertyChanged();
            }
        }

        [UIAction("new")]
        public void New()
        {
            var selected = detailView.selectedDifficultyBeatmap;
            var charact = detailView.GetField<BeatmapCharacteristicSegmentedControlController, StandardLevelDetailView>("_beatmapCharacteristicSegmentedControlController");
            var newTweak = new TweakCell(new TweakData()
            {
                Selected = true,
                LevelInfo = new TweakData.HashDifMode()
                {
                    Difficulty = selected.difficulty.ToString(),
                    Hash = selected.level.levelID,
                    Mode = charact.selectedBeatmapCharacteristic.serializedName
                },
                Name = "Custom",
                NJS = selected.noteJumpMovementSpeed,
                Offset = selected.noteJumpStartBeatOffset
            });
            tweakList.data.Add(newTweak);
            Configuration.Config.Instance.Tweaks.Add(newTweak.data);

            tweakList.tableView.ReloadData();
            tweakList.tableView.SelectCellWithIdx(tweakList.data.Count - 1, true);
        }

        [UIAction("delete")]
        public void Delete()
        {
            Configuration.Config.Instance.Tweaks.Remove(selectedTweak.data);
            tweakList.data.Remove(selectedTweak);
            tweakList.tableView.ReloadData();

            tweakList.tableView.SelectCellWithIdx(selectedIndex - 1, true);
        }

        [UIAction("#post-parse")]
        public void Parsed()
        {
            detailView = Resources.FindObjectsOfTypeAll<StandardLevelDetailView>().FirstOrDefault();
            Plugin.levelFilteringNavigationController.didSelectAnnotatedBeatmapLevelCollectionEvent -= LevelSelectionChanged;
            Plugin.levelFilteringNavigationController.didSelectAnnotatedBeatmapLevelCollectionEvent += LevelSelectionChanged;
            Message = "Please select a level.";
        }

        [UIAction("cell-clicked")]
        public void CellClicked(TableView _, int pos)
        {
            foreach (var c in tweakList.data)
                (c as TweakCell).data.Selected = false;
            var cell = tweakList.data[pos] as TweakCell;
            cell.data.Selected = true;
            CanDelete = !cell.isDefault;

            selectedTweak = cell;
            selectedIndex = pos;

            Name = selectedTweak.data.Name;
            NJS = selectedTweak.data.NJS;
            Offset = selectedTweak.data.Offset;

            parserParams.EmitEvent("set");
        }

        [UIAction("reload")]
        public void Reload(object o)
        {
            StartCoroutine(QuickReload());
        }

        public IEnumerator QuickReload()
        {
            yield return new WaitForSeconds(.05f);
            int index = selectedIndex;
            for (int i = 0; i < tweakList.data.Count; i++)
            {
                (tweakList.data[i] as TweakCell).UpdateText();
            }
            tweakList.tableView.ReloadData();
            tweakList.tableView.SelectCellWithIdx(index, false);
        }

        private static bool IsEqualToOne(float value) => Math.Abs(value - 1) < 0.00001f;
        

        public IEnumerator SetupTweakPage()
        {
            tweakList.data.Clear();
            tweakList.tableView.ReloadData();
            CanDelete = false;
            Y2Pos = -100f;
            Message = "Loading...";
            var prevMap = detailView.selectedDifficultyBeatmap;
            yield return new WaitUntil(() => prevMap != detailView.selectedDifficultyBeatmap);
            Y2Pos = 3f;
            var selected = detailView.selectedDifficultyBeatmap;
            if (selected == null)
            {
                Message = "Error! Please click off and back on.";
                Y2Pos = -100f;
                yield break;
            }
            if (!selected.level.levelID.ToLower().Contains("custom_level"))
            {
                Message = "This mod does not work with OSTs.";
                Y2Pos = -100f;
                yield break;
            }
            Message = "";
            var charact = detailView.GetField<BeatmapCharacteristicSegmentedControlController, StandardLevelDetailView>("_beatmapCharacteristicSegmentedControlController");
            tweakList.data.Add(new TweakCell(new TweakData()
            {
                LevelInfo = new TweakData.HashDifMode()
                {
                    Difficulty = selected.difficulty.ToString(),
                    Hash = selected.level.levelID,
                    Mode = charact.selectedBeatmapCharacteristic.serializedName
                },
                Name = "Default",
                NJS = selected.noteJumpMovementSpeed,
                Offset = selected.noteJumpStartBeatOffset,
                Selected = true
            }, true));
            Plugin.lastSelectedMode = charact.selectedBeatmapCharacteristic.serializedName;
            List<TweakData> tweakData = Configuration.Config.Instance.Tweaks.Where(t => t.LevelInfo.Equals(selected, charact.selectedBeatmapCharacteristic.serializedName)).ToList();
            List<TweakCell> cells = new List<TweakCell>();
            foreach (var t in tweakData)
                cells.Add(new TweakCell(t));
            tweakList.data.AddRange(cells);
            tweakList.tableView.ReloadData();

            if (tweakData.Count() == 0 || !tweakData.Any(x => x.Selected))
                tweakList.tableView.SelectCellWithIdx(0, true);
            else
            {
                int index = -1;
                for (int i = 0; i < tweakData.Count; i++)
                {
                    if (tweakData[i].Selected)
                    {
                        index = i + 1;
                        break;
                    }
                }
                tweakList.tableView.SelectCellWithIdx(index, true);
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
            BSEvents.characteristicSelected += LevelSelectionChanged;
            BSEvents.difficultySelected += LevelSelectionChanged;
            BSEvents.levelSelected += LevelSelectionChanged;
        }

        private void LevelSelectionChanged(bool forceHide = false) //Don't ask.
        {
            YPos = forceHide ? -100f : 0;
            if (!forceHide)
            {
                StartCoroutine(SetupTweakPage());
            }
        }

        private void LevelSelectionChanged(object arg1, object arg2, object arg3, object arg4) => LevelSelectionChanged(true);
        private void LevelSelectionChanged(object arg1, object arg2) => LevelSelectionChanged();

        public void OnDisable()
        {
            BSEvents.characteristicSelected -= LevelSelectionChanged;
            BSEvents.difficultySelected -= LevelSelectionChanged;
            Plugin.levelFilteringNavigationController.didSelectAnnotatedBeatmapLevelCollectionEvent -= LevelSelectionChanged;
            BSEvents.levelSelected -= LevelSelectionChanged;
        }
    }
}
