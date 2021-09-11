using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace vomsProject
{
    public class FilteredFileProvider : IFileProvider
    {
        private IFileProvider FileProvider;
        Func<string, bool> Filter;
        public FilteredFileProvider(IFileProvider fileProvider, Func<string, bool> filter) 
        {
            FileProvider = fileProvider;
            Filter = filter;
        }
        public IFileInfo GetFileInfo(string subpath)
        {
            if (!Filter(subpath))
            {
                return new NotFoundFileInfo(subpath.Split('/').Last());
            }
            return FileProvider.GetFileInfo(subpath);
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            if (!Filter(subpath))
            {
                return new NotFoundDirectoryContents();
            }
            return FileProvider.GetDirectoryContents(subpath);
        }

        public IChangeToken Watch(string filter)
        {
            return FileProvider.Watch(filter);
        }
    }
}
