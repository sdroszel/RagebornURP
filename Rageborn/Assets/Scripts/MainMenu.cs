using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
   [SerializeField] private GameObject optionsMenu;
   private Toggle musicToggle;

   private void Start()
   {
      optionsMenu.SetActive(true);
      musicToggle = optionsMenu.transform.Find("MusicToggle").GetComponent<Toggle>();
      optionsMenu.SetActive(false);

      musicToggle.isOn = AudioManager.instance.isPlaying;

      musicToggle.onValueChanged.AddListener(OnMusicToggleChanged);
   }

   private void OnMusicToggleChanged(bool isOn)
   {
      AudioManager.instance.ToggleMusic(isOn);
   }
}
