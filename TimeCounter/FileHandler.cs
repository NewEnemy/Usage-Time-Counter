using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeCounter
{
    class FileHandler
    {
        static string file_path = Path.Combine(Path.GetTempPath(), "temprecorddata");
        
        public FileHandler()
        {
           
        }
        public  void WriteRecord(Dictionary<string,long[]> Dic)
        {
            using (FileStream stream = new FileStream(file_path,FileMode.Create,FileAccess.Write)) {
                foreach(KeyValuePair<string,long[]> item in Dic){
                    string entry = string.Format("{0};{1};{2}\n", item.Key, item.Value[0], item.Value[1]);
                    stream.Write(Encoding.ASCII.GetBytes(entry), 0, Encoding.ASCII.GetBytes(entry).Length);
                }
                stream.Close();


            }

        }
    }
}
