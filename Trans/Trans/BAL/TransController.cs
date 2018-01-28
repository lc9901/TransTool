using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trans.BAL;
using Trans.Exceptions;

namespace Trans.Business
{
    public class TransController
    {
        private string ErroMessage = "Please input correct content!!!";
        public string GetTargetTesult(string inputContent, string inputLanguage, string targetLanguage)
        {
            string result = string.Empty;
            BusinessLogic bl = new BusinessLogic();
            try
            {
                result = bl.GetTargetLanguage(inputContent, inputLanguage, targetLanguage);
            }
            catch (InputException)
            {
                throw new Exception(ErroMessage);
            }

            return result;
        }
    }
}
