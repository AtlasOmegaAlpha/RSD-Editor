using System.Text.RegularExpressions;
using System.IO;

namespace Util.Validation
{
    public class Validation
    {
        public static string ValidateFileName(string fileName)
        {
            Regex badName = new Regex("[" + Regex.Escape(new string(Path.GetInvalidPathChars())) + "]");
            if (!badName.IsMatch(fileName))
                return fileName;

            return badName.Replace(fileName, "");
        }
    }
}
