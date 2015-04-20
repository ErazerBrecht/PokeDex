using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS_PokedexManager
{
    public class DescriptionProgressBar : INotifyPropertyChanged
    {
        private string _description;

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                NoticeMe("Description");
            }
        }

        public void NoticeMe(string p)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(p));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
