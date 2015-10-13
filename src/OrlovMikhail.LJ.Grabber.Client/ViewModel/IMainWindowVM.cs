using System;
using System.Windows.Input;

namespace OrlovMikhail.LJ.Grabber.Client
{
    public interface IMainWindowVM
    {
        string URI { get; set; }
        string Cookie { get; set; }
        string BookRootLocation { get; set; }
        string Subfolder { get; set; }

        bool IsEnabled { get; }

        string Log { get; }
        ICommand RunCommand { get; }

        void SaveSettings();
    }
}
