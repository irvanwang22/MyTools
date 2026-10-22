using TMPro;
using UnityEngine;

namespace MyTools.Math
{
    public class BigNumWPrecDisplayer : TextMeshProUGUI
    {
        [SerializeField] private int standardLimit = 9;
        [SerializeField] private int significantDigits = 4;
        [SerializeField] private bool withComma = true;

        public bool displayingValue { get; private set; }
        public BigNumWPrec value { get; private set; }

        public override string text
        {
            get => base.text;
            set
            {
                base.text = value;
                displayingValue = false;
            }
        }

        public void DisplayNumber(BigNumWPrec value)
        {
            this.value = value;
            displayingValue = true;
            text = value.ToAutoDisplayNotation(standardLimit, significantDigits, withComma);
        }
    }
}
