using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
using WpfAnket.Command;
using WpfAnket.Models;

namespace WpfAnket.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private User userData = new User();

       
        public class Json
        {
            public void WriteJson(object obj, string filename)
            {
                var serializer = new JsonSerializer();
                using (var sw = new StreamWriter(filename))
                {
                    using (var jw = new JsonTextWriter(sw))
                    {
                        jw.Formatting = Formatting.Indented;
                        serializer.Serialize(jw, obj);
                    }
                }
            }

            public T ReadJson<T>(string filename) where T : class
            {
                var serializer = new Newtonsoft.Json.JsonSerializer();
                using (var sr = new StreamReader(filename))
                {
                    using (var jr = new JsonTextReader(sr))
                    {
                        var result = serializer.Deserialize<T>(jr);
                        return result;
                    }
                }
            }
        }

        public class Xml
        {
            public void WriteXml(object obj, string filename)
            {
                var serializer = new XmlSerializer(obj.GetType());
                using (var sw = new StreamWriter(filename))
                {
                    serializer.Serialize(sw, obj);
                }
            }

            public T ReadXml<T>(string filename) where T : class
            {
                var serializer = new XmlSerializer(typeof(T));
                using (var sr = new StreamReader(filename))
                {
                    var result = serializer.Deserialize(sr) as T;
                    return result;
                }
            }
        }


        public interface IAdapter
        {
            void Write(User data, string filename);
            User Read(string filename);
        }

        public class JsonAdapter : IAdapter
        {
            private Json _json;

            public JsonAdapter()
            {
                _json = new Json();
            }

            public User Read(string filename)
            {
                return _json.ReadJson<User>(filename);
            }

            public void Write(User data, string filename)
            {
                _json.WriteJson(data, filename);
            }
        }

        public class XmlAdapter : IAdapter
        {
            private Xml _xml;   

            public XmlAdapter()
            {
                _xml = new Xml();
            }

            public User Read(string filename)
            {
                return _xml.ReadXml<User>(filename);
            }

            public void Write(User data, string filename)
            {
               _xml.WriteXml(data, filename);
            }
        }


        public class Converter : IAdapter
        {
            private readonly IAdapter _adapter;

            public Converter(IAdapter adapter)
            {
                _adapter = adapter;
            }
            public User Read(string filename)
            {
                return _adapter.Read(filename);
            }

            public void Write(User data, string filename)
            {
                _adapter.Write(data,filename);
            }
        }

        public RelayCommand SaveCommand { get; set; }
        public RelayCommand LoadCommand { get; set; }
        public RelayCommand ClearCommand { get; set; }

        private IAdapter _adapter;
        public MainViewModel()
        {
            SaveCommand = new RelayCommand((obj) =>
            {
                userData.Name = Name;
                userData.Surname = Surname;
                userData.Age = Age;
                userData.SeriaNo = SeriaNo;

                if (IsJsonChecked)
                {
                    _adapter = new JsonAdapter();
                    _adapter.Write(userData, "filename.json");
                    MessageBox.Show("Data saved to JSON.");
                }
                else if (IsXmlChecked)
                {
                    _adapter = new XmlAdapter();
                    _adapter.Write(userData, "filename.xml");
                    MessageBox.Show("Data saved to XML.");
                }
            });

            LoadCommand = new RelayCommand((obj) =>
            {
                if (IsJsonChecked)
                {
                    _adapter = new JsonAdapter();
                    userData = _adapter.Read("filename.json");
                    MessageBox.Show("Data loaded from JSON.");
                }
                else if (IsXmlChecked)
                {
                    _adapter = new XmlAdapter();
                    userData = _adapter.Read("filename.xml");
                    MessageBox.Show("Data loaded from XML.");
                }
                UpdateUIWithData();
            });

            ClearCommand = new RelayCommand((obj) =>
            {
                Name = string.Empty;
                Surname = string.Empty;
                Age = string.Empty;
                SeriaNo = string.Empty;

                userData = new User();

                MessageBox.Show("Data cleared");
            });
        }

        private void UpdateUIWithData()
        {
            Name = userData.Name;
            Surname = userData.Surname;
            Age = userData.Age;
            SeriaNo = userData.SeriaNo;
        }

        private bool _isJsonChecked;
        public bool IsJsonChecked
        {
            get { return _isJsonChecked; }
            set { _isJsonChecked = value; OnPropertyChanged(nameof(IsJsonChecked)); }
        }


        private bool _isXmlChecked;
        public bool IsXmlChecked
        {
            get { return _isXmlChecked; }
            set { _isXmlChecked = value; OnPropertyChanged(nameof(IsXmlChecked)); }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(nameof(Name)); }
        }

        private string _surname;
        public string Surname
        {
            get { return _surname; }
            set { _surname = value; OnPropertyChanged(nameof(Surname)); }
        }

        private string _age;
        public string Age
        {
            get { return _age; }
            set { _age = value; OnPropertyChanged(nameof(Age)); }
        }

        private string _seriaNo;
        public string SeriaNo
        {
            get { return _seriaNo; }
            set { _seriaNo = value; OnPropertyChanged(nameof(SeriaNo)); }
        }


    }
}
