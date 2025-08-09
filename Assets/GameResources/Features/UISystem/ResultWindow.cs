namespace GameResources.Features.UISystem
{
    using UnityEngine;
    using UnityEngine.UI;

    public sealed class ResultWindow : BaseWindow
    {
        [Header("Buttons")]
        [field: SerializeField] public Button ButtonRestart { get; set; } = default;
        [field: SerializeField] public Button ButtonMenu { get; set; } = default;

        [Header("Views")]
        [SerializeField] private Text _scoreField = default;
        [SerializeField] private Text _recordField = default;

        private string _scoreFormat = "Score: {0}";
        private string _recordFormat = "Record: {0}";
        
        public void UpdateView(int score, int record)
        {
            _scoreField.gameObject.SetActive(record <= score);
            _recordField.gameObject.SetActive(record > score);
            
            _scoreField.text = string.Format(_scoreFormat, score.ToString());
            _recordField.text = string.Format(_recordFormat, record.ToString());
        }
    }
}