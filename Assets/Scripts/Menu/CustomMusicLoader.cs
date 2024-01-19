using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace NsfwMiniJam.Menu
{
    public class CustomMusicLoader : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _explanation;

        [SerializeField]
        private Transform _musicContainer;

        [SerializeField]
        private GameObject _musicPrefab;

        [SerializeField]
        private TMP_InputField _bpm;

        [SerializeField]
        private RuntimeAnimatorController _controller;

        private List<MusicLoaderData> _music = new();

        private void Awake()
        {
            string path = Path.Combine(Application.persistentDataPath, "Music");
            if (Application.platform != RuntimePlatform.WebGLPlayer)
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                _explanation.text = $"Put your music in {path} and restart the game!";
                var extensions = new[] { ".wav", ".mp3", ".ogg" };
                foreach (var f in Directory.GetFiles(path))
                {
                    var fi = new FileInfo(f);

                    if (extensions.Contains(fi.Extension))
                    {
                        var inst = Instantiate(_musicPrefab, _musicContainer);
                        inst.GetComponentInChildren<TMP_Text>().text = fi.Name;
                        var b = inst.GetComponent<Button>();
                        b.interactable = IsBPMValid;
                        b.onClick.AddListener(new(() =>
                        {
                            GlobalData.LevelIndex = -1;
                            GlobalData.CustomSongPath = fi.FullName;
                            GlobalData.CustomMusic = new()
                            {
                                HaveReadyUpAnim = false,
                                HaveSpeNoteAnim = false,
                                BlindOverrides = false,
                                Bpm = int.Parse(_bpm.text),
                                Controller = _controller,
                                DialogueInfo = new(),
                                HypnotisedOverrides = false,
                                Intro = null,
                                KeyOverrides = true,
                                MinesOverrides = false,
                                ModifierDescription = string.Empty,
                                ModifierName = string.Empty,
                                Music = null,
                                Name = fi.Name,
                                NoFailOverrides = false,
                                NoteCount = 100
                            };
                            GlobalData.CustomMusicAudioType = fi.Extension switch
                            {
                                ".wav" => AudioType.WAV,
                                ".ogg" => AudioType.OGGVORBIS,
                                ".mp3" => AudioType.MPEG,
                                _ => throw new System.NotImplementedException()
                            };

                            SceneManager.LoadScene("Main");
                        }));
                        _music.Add(new()
                        {
                            Button = b,
                            Path = path
                        });
                    }
                }
            }
            else
            {
                _explanation.text = $"This feature is not available on WebGL";
            }

        }

        private bool IsBPMValid => int.TryParse(_bpm.text, out int res) && res > 0;

        public void OnBPMEdit()
        {
            foreach (var m in _music)
            {
                m.Button.interactable = IsBPMValid;
            }
        }

        public class MusicLoaderData
        {
            public string Path;
            public Button Button;
        }
    }
}
