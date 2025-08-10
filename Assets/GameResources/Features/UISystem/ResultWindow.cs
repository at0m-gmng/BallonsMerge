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
        private string _newRecordFormat = "New Record: {0}";
        
        public void UpdateView(int score, int record)
        {
            if (score != record)
            {
                _scoreField.gameObject.SetActive(true);
                _recordField.gameObject.SetActive(true);
                _scoreField.text = string.Format(_scoreFormat, score.ToString());
                _recordField.text = string.Format(_recordFormat, record.ToString());
            }
            else
            {
                _scoreField.gameObject.SetActive(false);
                _recordField.gameObject.SetActive(true);
                _recordField.text = string.Format(_newRecordFormat, record.ToString());
            }
        }
    }
}