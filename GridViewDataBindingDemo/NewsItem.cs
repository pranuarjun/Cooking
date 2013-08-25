using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GridViewDataBindingDemo
{
    class NewsItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _headline;
        private string _content;
        private string _image;
	    private string _fileContent;

        public string Headline
        {
            get { return _headline; }
            set
            {
                _headline = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Headline"));
            }
        }

        public string ContentUri
        {
            get { return _content; }
            set
            {
                _content = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ContentUri"));
            }
        }

        public string ImageUri
        {
            get { return _image; }
            set
            {
                _image = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ImageUri"));
            }
        }

	    public string FileContent
	    {
			get { return _fileContent; }
			set { _fileContent = value;
				if (PropertyChanged !=null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs("FileContent"));
				}
			}

	    }
    }
}
