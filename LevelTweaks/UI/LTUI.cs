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

namespace LevelTweaks.UI
{
    public class LTUI : IInitializable, IDisposable, INotifyPropertyChanged
    {
        private readonly Config _config;
        private readonly StandardLevelDetailView _detailView;
        private readonly StandardLevelDetailViewController _standardLevelDetailViewController;
        private readonly BeatmapCharacteristicSegmentedControlController _beatmapCharacteristicSegmentedControlController;
        public event PropertyChangedEventHandler PropertyChanged;

        public TweakCell selectedTweak = new TweakCell(new TweakData() { Name = "Loading...", NJS = -999f, Offset = -999f }, 128);
        public int selectedIndex;

        public LTUI(Config config, StandardLevelDetailViewController standardLevelDetailViewController)
        {
            Y2Pos = -100f;
            _config = config;
            _standardLevelDetailViewController = standardLevelDetailViewController;
            _detailView = standardLevelDetailViewController.GetField<StandardLevelDetailView, StandardLevelDetailViewController>("_standardLevelDetailView");
            _beatmapCharacteristicSegmentedControlController = _detailView.GetField<BeatmapCharacteristicSegmentedControlController, StandardLevelDetailView>("_beatmapCharacteristicSegmentedControlController");
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
                DisablesScoreSubmission = NJS != _detailView.selectedDifficultyBeatmap.noteJumpMovementSpeed;
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
            var selected = _detailView.selectedDifficultyBeatmap;
            var newTweak = new TweakCell(new TweakData()
            {
                Selected = true,
                LevelInfo = new TweakData.HashDifMode()
                {
                    Difficulty = selected.difficulty.ToString(),
                    Hash = selected.level.levelID,
                    Mode = _beatmapCharacteristicSegmentedControlController.selectedBeatmapCharacteristic.serializedName
                },
                Name = "Custom",
                NJS = selected.noteJumpMovementSpeed,
                Offset = selected.noteJumpStartBeatOffset
            }, selected.level.beatsPerMinute);
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
            DisablesScoreSubmission = NJS != _detailView.selectedDifficultyBeatmap.noteJumpMovementSpeed;
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

        public IEnumerator SetupTweakPage()
        {
            tweakList.data.Clear();
            tweakList.tableView.ReloadData();
            CanDelete = false;
            Y2Pos = -100f;
            Message = "Loading...";
            var prevMap = _detailView.selectedDifficultyBeatmap;
            yield return new WaitUntil(() => prevMap != _detailView.selectedDifficultyBeatmap || (_detailView.selectedDifficultyBeatmap != null && prevMap == _detailView.selectedDifficultyBeatmap));
            Y2Pos = 3f;
            var selected = _detailView.selectedDifficultyBeatmap;
            if (selected == null)
            {
                Message = "Error! Please try again.";
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
            var charact = _detailView.GetField<BeatmapCharacteristicSegmentedControlController, StandardLevelDetailView>("_beatmapCharacteristicSegmentedControlController");
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
            }, selected.level.beatsPerMinute, true));
            Plugin.lastSelectedMode = charact.selectedBeatmapCharacteristic.serializedName;
            List<TweakData> tweakData = _config.Tweaks.Where(t => t.LevelInfo.Equals(selected, charact.selectedBeatmapCharacteristic.serializedName)).ToList();
            List<TweakCell> cells = new List<TweakCell>();
            foreach (var t in tweakData)
            {
                cells.Add(new TweakCell(t, selected.level.beatsPerMinute));
            }
            tweakList.data.AddRange(cells);
            tweakList.tableView.ReloadData();

            DisablesScoreSubmission = NJS != selected.noteJumpMovementSpeed;
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
        }

        private void LevelSelectionChanged(bool forceHide = false)
        {
            YPos = forceHide ? -100f : 0;
            if (!forceHide)
            {
                SharedCoroutineStarter.instance.StartCoroutine(SetupTweakPage());
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
        }

        private void DidContentChange(StandardLevelDetailViewController arg1, StandardLevelDetailViewController.ContentType arg2)
        {
            if (arg2 == StandardLevelDetailViewController.ContentType.OwnedAndReady || arg2 == StandardLevelDetailViewController.ContentType.Buy)
            {
                LevelSelectionChanged();
            }
            else
            {
                tweakList.data.Clear();
                tweakList.tableView.ReloadData();
                CanDelete = false;
                Y2Pos = -100f;
                Message = "Loading...";
            }
        }

        private void DidChange(StandardLevelDetailView arg1, IDifficultyBeatmap arg2)
        {
            LevelSelectionChanged();
        }
    }
}