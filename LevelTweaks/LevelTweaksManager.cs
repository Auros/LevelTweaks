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
    public class LevelTweaksManager : MonoBehaviour
    {
        public static LevelTweaksManager Instance { get; set; }

        //public IDifficultyBeatmap CurrentlySelectedDifficultyBeatmap { get; set; }
        public StandardLevelDetailView standardView;

        [UIParams] public BSMLParserParams parserParams;
        [UIComponent("LTB")] public Button lt_button;
        //[UIComponent("testtext")] public TextMeshProUGUI testtext;
        [UIComponent("list")] public CustomListTableData list;

        private bool newInteractable = false;
        [UIValue("new-interactable")]
        public bool NewInteractable
        {
            get => newInteractable;
            set
            {
                newInteractable = value;
                
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
                
            }
        }

        [UIValue("v_offset")]
        public float offset;

        [UIValue("v_njs")]
        public float njs;

        [UIAction("#post-parse")]
        public void Parsed()
        {
            list.data.Add(new LevelTweak(new LevelTweakInfo() { njs = 17f, offset = 0f, name = "Default" }));
            list.data.Add(new LevelTweak(new LevelTweakInfo() { njs = 18f, offset = 4f, name = "Custom" }));
            list.data.Add(new LevelTweak(new LevelTweakInfo() { njs = 16f, offset = -5f, name = "Custom" }));
            list.tableView.ReloadData();
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
            //testtext.text = standardView.selectedDifficultyBeatmap.level.songName;
            Logger.log.Info(standardView.selectedDifficultyBeatmap.level.songName);
        }
        
        public void ShowDialog()
        {
            
            parserParams.EmitEvent("open-modal");
        }
        public void HideDialog() => parserParams.EmitEvent("close-modal");
        
    }
}
