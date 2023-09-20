using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiRISApp.Services
{
    public class TranslationService
    {
        #region SINGLETON

        private static TranslationService? instance = null;
        public static TranslationService Instance
        {
            get
            {
                instance ??= new TranslationService();
                return instance;
            }
        }

        private TranslationService() { }

        #endregion


        public string Translate(string text)
        {
            var app = (App)System.Windows.Application.Current;
            string? translatedText = app.LanguageDictionary[text]?.ToString();
            translatedText ??= text;
            
            return translatedText;
        }
    }
}
