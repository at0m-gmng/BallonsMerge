namespace GameResources.Features.UISystem
{
    using UnityEngine;
    using UnityEngine.UI;

    public sealed class GameWindow : BaseWindow
    {
        [Header("Buttons")]
        [field: SerializeField] public Button ButtonPause { get; set; } = default;
        [field: SerializeField] public RawImage GraphicsTexture { get; set; } = default;
        
        [Header("Views")]
        [SerializeField] private Text _scoreField = default;

        public void UpdateView(int score) => _scoreField.text = score.ToString();
    }
}