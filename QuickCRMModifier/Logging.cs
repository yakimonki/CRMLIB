using System.IO;

namespace CRMLib
{
    public class Logger
    {
        public static void Main(string Faild, string path)
        {
            //StreamWriter sw = new StreamWriter(EnvPath);
            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine(Faild);
                }
            }

            // This text is always added, making the file longer over time
            // if it is not deleted.
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(Faild);
            }

        }

    }
}
