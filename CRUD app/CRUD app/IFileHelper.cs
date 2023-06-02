using System;
using System.Collections.Generic;
using System.Text;

namespace CRUD_app
{
    public interface IFileHelper
    {
        string GetLocalFilePath(string filename);
    }
}
