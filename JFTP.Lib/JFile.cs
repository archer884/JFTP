using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JFTP.Lib
{
    public class JFile
    {
        public string Path { get; set; }
        public string Name
        {
            get { return System.IO.Path.GetFileName(Path); }
        }
        
        private string _token;
        public string Token
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_token))
                    _token = GetToken();
                return _token;
            }
        }

        public JFile()
        {
        }

        public JFile(string path)
        {
            Path = path;
        }

        private string GetToken()
        {
            return ((long)Path.GetHashCode() + 4500000000).EncodeBase36();
        }
    }
}
