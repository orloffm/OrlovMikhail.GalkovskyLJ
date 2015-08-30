using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace OrlovMikhail.LJ.Grabber.Client
{
    public class DesignMainWindowVM : IMainWindowVM
    {
        public string Cookie
        {
            get { return "Cookie text"; }
            set { }
        }

        public string Log
        {
            get { return "A\nB\nC\nD"; }
        }

        public ICommand RunCommand
        {
            get { return null; }
        }

        public void SaveSettings()
        {
            
        }

        public ICommand SaveResultCommand
        {
            get { return null; }
        }

        public string URI
        {
            get { return "http://galkovsky.livejournal.com/"; }
            set { }
        }


        public string BookRootLocation
        {
            get { return "Base location"; }
            set
            {

            }
        }

        public string Subfolder
        {
            get { return "Sub folder"; }
            set
            {
            }
        }

        public bool IsEnabled
        {
            get { return true; }
        }
    }
}
