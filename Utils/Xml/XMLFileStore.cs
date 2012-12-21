using System;
using System.Data;
using System.Configuration;
using System.IO;
using System.Web;
using System.Threading;

namespace Utils.Xml
{
    public class XMLFileStore<T> where T : class, new()
    {
        private T _data;
        private string _xmlFilename;
        private DateTime _lastModified;
        private DateTime _lastModifiedLastChecked;
        private object _lock;

        public XMLFileStore(string xmlFilename)
        {
            _data = null;
            _xmlFilename = xmlFilename;
            _lastModified = DateTime.MinValue;
            _lastModifiedLastChecked = DateTime.MinValue;
            _lock = new object();
        }

        public T Data
        {
            get
            {
                Load();
                return _data;
            }
        }

        public void SetData(T data)
        {
            _data = data;
        }

        public void Reload()
        {
            lock (_lock)
            {
                _lastModifiedLastChecked = DateTime.MinValue;
                _data = null;
                Load();
            }
        }

        private void Load()
        {
            lock (_lock)
            {
                if (DateTime.Now.Subtract(_lastModifiedLastChecked).TotalSeconds < 1.0)
                    return;
                _lastModifiedLastChecked = DateTime.Now;

                DateTime lastModified = DateTime.MinValue;
                bool fileExists = File.Exists(_xmlFilename);
                if (fileExists)
                {
                    lastModified = File.GetLastWriteTimeUtc(_xmlFilename);
                }

                if ((_data == null) || (lastModified > _lastModified))
                {
                    if (fileExists)
                    {
                        _lastModified = lastModified;
                        try
                        {
                            using (StreamReader sr = new StreamReader(_xmlFilename))
                            {
                                string content = sr.ReadToEnd();

                                Utils.XMLSerializer x = new Utils.XMLSerializer();
                                _data = x.ReadObject(content) as T;
                            }
                        }
                        catch (ThreadAbortException)
                        {
                        }
                        catch (Exception)
                        {
                            if (File.Exists(_xmlFilename + ".invalid"))
                                File.Delete(_xmlFilename + ".invalid");
                            File.Move(_xmlFilename, _xmlFilename + ".invalid");
                            _data = new T();
                        }
                    }
                    else
                    {
                        _data = new T();
                    }
                }
            }
        }

        public void Save()
        {
            lock (_lock)
            {
                if (_data != null)
                {
                    FileInfo fi = new FileInfo(_xmlFilename);
                    if (fi.Directory.Exists == false)
                        fi.Directory.Create();

                    var settings = new System.Xml.XmlWriterSettings();
                    settings.Indent = true;
                    using (var writer = System.Xml.XmlDictionaryWriter.Create(_xmlFilename, settings))
                    {
                        Utils.XMLSerializer x = new Utils.XMLSerializer();
                        x.WriteObject(writer, _data);
                    }
                    _lastModified = File.GetLastWriteTimeUtc(_xmlFilename);
                }
            }
        }

    }
}
