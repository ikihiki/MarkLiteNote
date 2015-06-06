using Livet.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MarkdownCustom
{
    public class FileInfoViewModel:INotifyPropertyChanged
    {
        private readonly FileInfo fileInfo;
        private readonly MarkdownViewModel parent;

        public event PropertyChangedEventHandler PropertyChanged;

        public FileInfoViewModel(string path,MarkdownViewModel parent)
        {
            fileInfo = new FileInfo(path);
            this.parent = parent;
            OpenFileCommand = new ViewModelCommand(async () =>await parent.OpenFile(fileInfo));
            DeleteFileCommand = new ViewModelCommand(() => parent.DeleteFile(fileInfo));
        }

        public ICommand OpenFileCommand { get; }

        public ICommand DeleteFileCommand { get; }

        public string Name
        {
            get
            {
                return fileInfo.Name;
            }
        }

        public FileInfo RawData
        {
            get
            {
                return fileInfo;
            }
        }

    }
}
