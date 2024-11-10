using UnityEngine;
using UnityEngine.UI;

// This class is used to assign the option menu music toogle programmatically
public class MainMenu : MonoBehaviour
{
   [SerializeField] private GameObject optionsMenu;
   private Toggle musicToggle;

   // Sets options menu active for a second to get toogle object assignment
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
