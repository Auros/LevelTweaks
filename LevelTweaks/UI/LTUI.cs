using HMUI;
using System;
using Zenject;
using System.Linq;
using UnityEngine;
using IPA.Utilities;
using System.Collections;
using System.ComponentModel;
using BeatSaberMarkupLanguage;
using LevelTweaks.Configuration;
using System.Collections.Generic;
using Utilities = SiraUtil.Utilities;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.GameplaySetup;
using Config = LevelTweaks.Configuration.Config;
using LevelTweaks.HarmonyPatches;

namespace LevelTweaks.UI
{
    public class LTUI : IInitializable, IDisposable, INotifyPropertyChanged
    {
        private readonly Config _config;
        private readonly StandardLevelDetailView _detailView;
        private readonly StandardLevelDetailViewController _standardLevelDetailViewController;
        public event PropertyChangedEventHandler PropertyChanged;

        public TweakCell selectedTweak = new TweakCell(new TweakData() { Name = "Loading...", NJS = -999f, Offset = -999f }, 128);
        public int selectedIndex;

        private IDifficultyBeatmap selectedDifficultyBeatmap;

        public LTUI(Config config, StandardLevelDetailViewController standardLevelDetailViewController)
        {
            Y2Pos = -100f;
            _config = config;
            _standardLevelDetailViewController = standardLevelDetailViewController;
            _detailView = standardLevelDetailViewController.GetField<StandardLevelDetailView, StandardLevelDetailViewController>("_standardLevelDetailView");
        }

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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(YPos)));
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Y2Pos)));
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanDelete)));
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Message)));
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
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
                DisablesScoreSubmission = NJS != selectedDifficultyBeatmap.noteJumpMovementSpeed;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NJS)));
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Offset)));
            }
        }

        private bool _disablesScoreSubmission = false;
        [UIValue("disables-score-submission")]
        public bool DisablesScoreSubmission
        {
            get => _disablesScoreSubmission;
            set
            {
                _disablesScoreSubmission = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisablesScoreSubmission)));
            }
        }

        private bool _showEditor = false;
        [UIValue("show-editor")]
        public bool ShowEditor
        {
            get => _showEditor;
            set
            {
                _showEditor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowEditor)));
            }
        }

        [UIAction("new")]
        public void New()
        {
            var newTweak = new TweakCell(new TweakData()
            {
                Selected = true,
                LevelInfo = new TweakData.HashDifMode()
                {
                    Difficulty = selectedDifficultyBeatmap.difficulty.ToString(),
                    Hash = selectedDifficultyBeatmap.level.levelID,
                    Mode = selectedDifficultyBeatmap.parentDifficultyBeatmapSet.beatmapCharacteristic.serializedName
                },
                Name = "Custom",
                NJS = selectedDifficultyBeatmap.noteJumpMovementSpeed,
                Offset = selectedDifficultyBeatmap.noteJumpStartBeatOffset
            }, selectedDifficultyBeatmap.level.beatsPerMinute);
            tweakList.data.Add(newTweak);
            _config.Tweaks.Add(newTweak.data);

            tweakList.tableView.ReloadData();
            tweakList.tableView.SelectCellWithIdx(tweakList.data.Count - 1, true);
        }

        [UIAction("delete")]
        public void Delete()
        {
            _config.Tweaks.Remove(selectedTweak.data);
            tweakList.data.Remove(selectedTweak);
            tweakList.tableView.ReloadData();

            tweakList.tableView.SelectCellWithIdx(selectedIndex - 1, true);
        }

        [UIAction("#post-parse")]
        public void Parsed()
        {
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
            DisablesScoreSubmission = NJS != selectedDifficultyBeatmap.noteJumpMovementSpeed;
        }

        [UIAction("reload")]
        public async void Reload(object _)
        {
            await Utilities.PauseChamp;
            int index = selectedIndex;
            for (int i = 0; i < tweakList.data.Count; i++)
            {
                (tweakList.data[i] as TweakCell).UpdateText();
            }
            tweakList.tableView.ReloadData();
            tweakList.tableView.SelectCellWithIdx(index, false);
        }

        public IEnumerator SetupTweakPage(IDifficultyBeatmap taDifficultyBeatmap = null)
        {
            tweakList.data.Clear();
            tweakList.tableView.ReloadData();
            CanDelete = false;
            Y2Pos = -100f;
            Message = "Loading...";

            selectedDifficultyBeatmap = null;
            if (taDifficultyBeatmap != null)
            {
                selectedDifficultyBeatmap = taDifficultyBeatmap;
            }
            else
            {
                var prevMap = _detailView.selectedDifficultyBeatmap;
                yield return new WaitUntil(() => prevMap != _detailView.selectedDifficultyBeatmap || (_detailView.selectedDifficultyBeatmap != null && prevMap == _detailView.selectedDifficultyBeatmap));
                Y2Pos = 3f;
                selectedDifficultyBeatmap = _detailView.selectedDifficultyBeatmap;
            }

            if (selectedDifficultyBeatmap == null)
            {
                Message = "Error! Please try again.";
                Y2Pos = -100f;
                yield break;
            }
            if (!selectedDifficultyBeatmap.level.levelID.ToLower().Contains("custom_level"))
            {
                Message = "This mod does not work with OSTs.";
                Y2Pos = -100f;
                yield break;
            }
            Message = "";
            var charact = selectedDifficultyBeatmap.parentDifficultyBeatmapSet.beatmapCharacteristic;
            tweakList.data.Add(new TweakCell(new TweakData()
            {
                LevelInfo = new TweakData.HashDifMode()
                {
                    Difficulty = selectedDifficultyBeatmap.difficulty.ToString(),
                    Hash = selectedDifficultyBeatmap.level.levelID,
                    Mode = charact.serializedName
                },
                Name = "Default",
                NJS = selectedDifficultyBeatmap.noteJumpMovementSpeed,
                Offset = selectedDifficultyBeatmap.noteJumpStartBeatOffset,
                Selected = true
            }, selectedDifficultyBeatmap.level.beatsPerMinute, true));
            Plugin.lastSelectedMode = charact.serializedName;
            List<TweakData> tweakData = _config.Tweaks.Where(t => t.LevelInfo.Equals(selectedDifficultyBeatmap, charact.serializedName)).ToList();
            List<TweakCell> cells = new List<TweakCell>();
            foreach (var t in tweakData)
            {
                cells.Add(new TweakCell(t, selectedDifficultyBeatmap.level.beatsPerMinute));
            }
            tweakList.data.AddRange(cells);
            tweakList.tableView.ReloadData();

            DisablesScoreSubmission = NJS != selectedDifficultyBeatmap.noteJumpMovementSpeed;
            if (tweakData.Count() == 0 || !tweakData.Any(x => x.Selected))
            {
                tweakList.tableView.SelectCellWithIdx(0, true);
            }
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

        public void Initialize()
        {
            GameplaySetup.instance.AddTab("Level Tweaks", "LevelTweaks.UI.lt.bsml", this);
            _standardLevelDetailViewController.didChangeContentEvent += DidContentChange;
            _detailView.didChangeDifficultyBeatmapEvent += DidChange;
            TADifficultySelected.TADifficultyBeatmapSelected += DidSelectTADifficultyBeatmap;
            TADismissed.TARoomDismissed += TARoomDismissed;
        }

        private void LevelSelectionChanged(IDifficultyBeatmap taDifficultyBeatmap = null, bool forceHide = false)
        {
            YPos = forceHide ? -100f : 0;
            if (!forceHide)
            {
                SharedCoroutineStarter.instance.StartCoroutine(SetupTweakPage(taDifficultyBeatmap: taDifficultyBeatmap));
            }
        }

        public void Dispose()
        {
            if (GameplaySetup.IsSingletonAvailable && BSMLParser.IsSingletonAvailable)
            {
                GameplaySetup.instance.RemoveTab("Level Tweaks");
            }
            _standardLevelDetailViewController.didChangeContentEvent -= DidContentChange;
            _detailView.didChangeDifficultyBeatmapEvent -= DidChange;
            TADifficultySelected.TADifficultyBeatmapSelected -= DidSelectTADifficultyBeatmap;
            TADismissed.TARoomDismissed -= TARoomDismissed;
        }

        private void ClearContent()
        {
            tweakList.data.Clear();
            tweakList.tableView.ReloadData();
            CanDelete = false;
            Y2Pos = -100f;
            Message = "Loading...";
        }

        private void DidContentChange(StandardLevelDetailViewController arg1, StandardLevelDetailViewController.ContentType arg2)
        {
            if (arg2 == StandardLevelDetailViewController.ContentType.OwnedAndReady || arg2 == StandardLevelDetailViewController.ContentType.Buy)
            {
                LevelSelectionChanged();
            }
            else
            {
                ClearContent();
            }
        }

        private void DidChange(StandardLevelDetailView arg1, IDifficultyBeatmap arg2)
        {
            LevelSelectionChanged();
        }

        private void DidSelectTADifficultyBeatmap(IDifficultyBeatmap selectedDifficultyBeatmap)
        {
            LevelSelectionChanged(taDifficultyBeatmap: selectedDifficultyBeatmap);
        }

        private void TARoomDismissed()
        {
            ClearContent();
        }
    }
}