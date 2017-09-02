﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilePicker
{
    public interface IFileService
    {
        IList<PathModel> GetFileInfos(string path = null);
        string GetBaseDirectory();
        byte[] ReadAllBytes(string path);
        void OpenFile(string filePath);
    }
}