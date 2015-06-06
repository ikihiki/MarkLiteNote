using Livet.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MarkdownCustom
{
    public class MarkdownViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private MarkdownConvertor convertor;

        private string markdown;
        private PropertyChangedEventArgs markdownProperty = new PropertyChangedEventArgs(nameof(Markdown));

        public string Markdown
        {
            get { return markdown; }
            set
            {
                markdown = value;
                PropertyChanged?.Invoke(this, markdownProperty);
                convert(markdown);
            }
        }


        private string html;
        private PropertyChangedEventArgs htmlProperty = new PropertyChangedEventArgs(nameof(Html));
        public string Html
        {
            get { return html; }
            private set
            {
                html = value;
                Helper.Html = html;
                PropertyChanged?.Invoke(this, htmlProperty);
            }
        }

        private Exception error;
        private PropertyChangedEventArgs errorProperty = new PropertyChangedEventArgs(nameof(Error));
        public Exception Error
        {
            get { return error; }
            internal set
            {
                error = value;
                PropertyChanged?.Invoke(this, errorProperty);
            }
        }

        private List<FileInfoViewModel> files;
        private PropertyChangedEventArgs filesProperty = new PropertyChangedEventArgs(nameof(Files));
        public List<FileInfoViewModel> Files
        {
            get { return files; }
            private set
            {
                files = value;
                PropertyChanged?.Invoke(this, filesProperty);
            }
        }

        private FileInfo editFile;
        private PropertyChangedEventArgs editFileProperty = new PropertyChangedEventArgs(nameof(EditFile));
        public FileInfo EditFile
        {
            get { return editFile; }
            set
            {
                editFile = value;
                PropertyChanged?.Invoke(this, editFileProperty);
                SaveFileCommand.RaiseCanExecuteChanged();
            }
        }

        public BrowsHelper Helper { get; }=new BrowsHelper();

        public MarkdownViewModel()
        {
            try
            {
                if (!Directory.Exists(Environment.CurrentDirectory + "\\plugin"))
                {
                    Directory.CreateDirectory(Environment.CurrentDirectory + "\\plugin");
                }
                convertor = new MarkdownConvertor();
                if (!Directory.Exists(Environment.CurrentDirectory + "\\markdown"))
                {
                    Directory.CreateDirectory(Environment.CurrentDirectory + "\\markdown");
                }
            }
            catch { }

            CreateFileListCommand = new ViewModelCommand(CreateFileList);
            OpenFileCommand = new ListenerCommand<FileInfo>(async (file) => await OpenFile(file));
            SaveFileCommand = new ViewModelCommand(async () => await SaveFile(), () => editFile != null);
            CreateFileCommand = new ListenerCommand<string>(CreateFile);
            DeleteFileCommand = new ListenerCommand<FileInfo>(DeleteFile);
            CreateFileList();
        }

        public ICommand CreateFileListCommand { get; }

        public void CreateFileList()
        {
            Files = Directory.GetFiles(Environment.CurrentDirectory + "\\markdown", "*.md", SearchOption.TopDirectoryOnly).Select(md => new FileInfoViewModel(md, this)).ToList();
        }

        public ICommand OpenFileCommand { get; }
        public async Task OpenFile(FileInfo file)
        {
            EditFile = file;
            using (var reader = file.OpenText())
            {
                Markdown = await reader.ReadToEndAsync();
            }
        }

        public ViewModelCommand SaveFileCommand { get; }
        public async Task SaveFile()
        {
            using (var file = editFile.CreateText())
            {
                await file.WriteAsync(markdown);
            }
        }


        public ICommand CreateFileCommand { get; }
        public void CreateFile(string filename)
        {
            editFile = new FileInfo($"{Environment.CurrentDirectory}\\markdown\\{filename}.md");
            editFile.Create().Close();
            CreateFileList();
        }

        public ICommand DeleteFileCommand { get; }
        public void DeleteFile(FileInfo file)
        {
            file.Delete();
        }

        private async Task convert(string text)
        {
            try
            {
                Html = await convertor.Transform(text);
            }
            catch (Exception ex)
            {
                Error = ex;
            }
        }
    }
}
