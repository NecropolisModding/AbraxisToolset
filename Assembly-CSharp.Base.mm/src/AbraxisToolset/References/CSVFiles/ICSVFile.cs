using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbraxisToolset.CSVFiles {
    public interface ICSVFile {
        void PatchFromFile(string path);
        void ReadFromFile(string path);
        void WriteToFile(string path);
    }
}
