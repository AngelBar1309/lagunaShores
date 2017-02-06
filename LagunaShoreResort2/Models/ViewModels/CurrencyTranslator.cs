using System;
using System.Text;

namespace LagunaShoreResort2.Models.ViewModels
{
    public class CurrencyTranslator
    {
        // Array of sting to hold the words from one to nineteen 
        private string[] _arrayOfOnes = { string.Empty, "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", 
        "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", 
        "Nineteen" };
        // Array of string to hold the words of tens - 10, 20,.., 90
        private string[] _arrayOfTens = { string.Empty , "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", 
        "Seventy", "Eighty", "Ninety"};

        // Converts the decimal to currency
        public string TranslateCurrency(decimal currencyValue)
        {
            string numericCurrency = currencyValue.ToString().Trim();
            //Check for the max capacity limit of the input 
            if (numericCurrency.Length > 18)
                return "Invalid input format";

            string leftValue;
            //Right align the charecters with padding of "0" to length the of 18 charecters
            if (numericCurrency.IndexOf(".") >= 0)
            {
                leftValue = numericCurrency.Substring(0, numericCurrency.IndexOf(".")).PadLeft(18, Convert.ToChar("0"));
                //decimalWord = numericCurrency.Substring(numericCurrency.IndexOf(".") + 1).PadRight(2, Convert.ToChar("0"));
                //decimalWord = (decimalWord.Length > 2 ? decimalWord.Substring(0, 2) : decimalWord);
            }
            else
            {
                leftValue = numericCurrency.PadLeft(18, Convert.ToChar("0"));
                //decimalWord = "0";
            }

            string quadrillionWord, trillionWord, billionWord, millionWord, thousandWord, hundredWord;
            quadrillionWord = TranslateHundreds(Convert.ToInt32(leftValue.Substring(0, 3))); // One Quadrillion - Number of zeros 15
            trillionWord = TranslateHundreds(Convert.ToInt32(leftValue.Substring(3, 3)));    // One Trillion - Number of zeros 12
            billionWord = TranslateHundreds(Convert.ToInt32(leftValue.Substring(6, 3)));     // One Billion - Number of zeros 9
            millionWord = TranslateHundreds(Convert.ToInt32(leftValue.Substring(9, 3)));    // One Million - Number of zeros 6
            thousandWord = TranslateHundreds(Convert.ToInt32(leftValue.Substring(12, 3)));
            hundredWord = TranslateHundreds(Convert.ToInt32(leftValue.Substring(15, 3)));
            //decimalWord = TranslateDecimal(decimalWord);

            // Start building the currency
            StringBuilder currencyInEnglish = new StringBuilder();
            currencyInEnglish.Append((quadrillionWord.Trim() == string.Empty ? string.Empty : quadrillionWord + " Quadrillion "));
            currencyInEnglish.Append((trillionWord.Trim() == string.Empty ? string.Empty : trillionWord + " Trillion "));
            currencyInEnglish.Append((billionWord.Trim() == string.Empty ? string.Empty : billionWord + " Billion "));
            currencyInEnglish.Append((millionWord.Trim() == string.Empty ? string.Empty : millionWord + " Million "));
            currencyInEnglish.Append((thousandWord.Trim() == string.Empty ? string.Empty : thousandWord + " Thousand "));
            currencyInEnglish.Append((hundredWord.Trim() == string.Empty ? string.Empty : hundredWord));

            //currencyInEnglish.Append(currencyInEnglish.ToString() == string.Empty ? "Zero Dollars " : " Dollars");
            if (currencyInEnglish.ToString().StartsWith("One Dollars"))
            {
                currencyInEnglish.Replace("One Dollars", "One Dollar");
            }
            //currencyInEnglish.Append((decimalWord));
            return currencyInEnglish.ToString();
        }


        #region Private Functions


        private string TranslateHundreds(int value)
        {
            string result;
            // If the value is less than hundred then convert it as tens 
            if (value <= 99)
            {
                result = (TranslateTens(value));
            }
            else
            {
                result = _arrayOfOnes[Convert.ToInt32(Math.Floor(Convert.ToDecimal(value / 100)))];
                // Math.Floor method is used to get the largest integer from the decial value
                result += " Hundred ";
                // The rest of the decimal is converted into tens
                if (value - Math.Floor(Convert.ToDecimal((value / 100) * 100)) == 0)
                {
                    result += string.Empty;
                }
                else
                {
                    result += string.Empty + TranslateTens(Convert.ToInt32(value - Math.Floor(Convert.ToDecimal((value / 100) * 100))));
                }
            }
            return result;
        }


        private string TranslateTens(int value)
        {
            string result;
            // If the value is less than 20 then get the word directly from the array
            if (value < 20)
            {
                result = _arrayOfOnes[value];
                value = 0;
            }
            else
            {
                result = _arrayOfTens[(int)Math.Floor(Convert.ToDecimal(value / 10))];
                value -= Convert.ToInt32(Math.Floor(Convert.ToDecimal((value / 10) * 10)));
            }
            result = result + (_arrayOfOnes[value].Trim() == string.Empty ? string.Empty : "-" + _arrayOfOnes[value]);
            return result;
        }

        // Translates the decimal part of the currency to words
        private string TranslateDecimal(string value)
        {
            string result = string.Empty;
            // Check whether the decimal part starts with a zero. Example : 12.05
            if (value != "0")
            {
                if (value.StartsWith("0"))
                {
                    result = "Zero ";
                    result += _arrayOfOnes[Convert.ToInt32(value.Substring(1, 1))];
                }
                else
                {
                    result = TranslateTens(Convert.ToInt32(value));
                }

            }
            return result;
        }
        #endregion
    }
}