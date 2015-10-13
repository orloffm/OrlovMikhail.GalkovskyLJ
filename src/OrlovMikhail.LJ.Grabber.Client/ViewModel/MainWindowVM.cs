using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using OrlovMikhail.LJ.Grabber;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using log4net;
using OrlovMikhail.LJ.Grabber.Client.Properties;

namespace OrlovMikhail.LJ.Grabber.Client
{
    public class MainWindowVM : ViewModelBase, IMainWindowVM
    {
        IWorker _w;

        static readonly ILog log = LogManager.GetLogger(typeof(MainWindowVM));

        public MainWindowVM(IWorker w)
        {
            _w = w;

            _runCommand = new RelayCommand(Run, CanRun);

            // Appender that allows us to log to a window.
            UIAppender app = new UIAppender();
            app.StringAdded += (sender, args) =>
            {
                string s = Log;
                string add = String.IsNullOrWhiteSpace(s) ? "" : Environment.NewLine;
                add += args.Value;

                Log += add;
            };

            // Set it.
            ((log4net.Repository.Hierarchy.Hierarchy)log4net.LogManager.GetLoggerRepository()).Root.AddAppender(app);
            LoadSettings();

            IsEnabled = true;
        }

        void LoadSettings()
        {
            this.URI = Settings.Default.URL;
            this.Cookie = Settings.Default.Cookie;
            this.BookRootLocation = Settings.Default.RootFolder;
            this.Subfolder = Settings.Default.SubFolder;
        }

        public void SaveSettings()
        {
            Settings.Default.URL = this.URI;
            Settings.Default.Cookie = this.Cookie;
            Settings.Default.RootFolder = this.BookRootLocation;
            Settings.Default.SubFolder = this.Subfolder;

            Settings.Default.Save();
        }

        RelayCommand _runCommand;
        public ICommand RunCommand { get { return _runCommand; } }

        private bool CanRun()
        {
            return !String.IsNullOrWhiteSpace(URI)
                && !String.IsNullOrWhiteSpace(Cookie)
                && !String.IsNullOrWhiteSpace(BookRootLocation)
                && !String.IsNullOrWhiteSpace(Subfolder)
                ;
        }

        private void Run()
        {
            if (!CanRun())
                return;

            Task task = Task.Factory.StartNew(() => RunInternal());
            task.ContinueWith(t => t.Exception.Handle(ex =>
                    {
                        log.Error("Error: " + ex.Message + Environment.NewLine + ex.StackTrace);
                        return true;
                    }), TaskContinuationOptions.OnlyOnFaulted);
        }

        private void RunInternal()
        {
            try
            {
                this.IsEnabled = false;

                Log = String.Empty;
                SaveSettings();

                _w.WorkInGivenTarget(URI, BookRootLocation, Subfolder, "dump.xml", Cookie);
            }
            finally
            {
                this.IsEnabled = true;
            }
        }

        #region properties
        private string _log;
        public string Log
        {
            get { return _log; }
            set
            {
                Set<string>(() => Log, ref _log, value);
                _runCommand.RaiseCanExecuteChanged();
            }
        }

        private string _uri;
        public string URI
        {
            get { return _uri; }
            set
            {
                Set<string>(() => URI, ref _uri, value);
                _runCommand.RaiseCanExecuteChanged();
            }
        }

        private string _cookie;
        public string Cookie
        {
            get { return _cookie; }
            set
            {
                Set<string>(() => Cookie, ref _cookie, value);
                _runCommand.RaiseCanExecuteChanged();
            }
        }

        private string _bookRootLocation;
        public string BookRootLocation
        {
            get { return _bookRootLocation; }
            set
            {
                Set<string>(() => BookRootLocation, ref _bookRootLocation, value);
                _runCommand.RaiseCanExecuteChanged();
            }
        }

        private string _subFolder;
        public string Subfolder
        {
            get { return _subFolder; }
            set
            {
                Set<string>(() => Subfolder, ref _subFolder, value);
                _runCommand.RaiseCanExecuteChanged();
            }
        }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { Set<bool>(() => IsEnabled, ref _isEnabled, value); }
        }
        #endregion
    }
}
