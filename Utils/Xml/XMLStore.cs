using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Utils.Xml
{
    public class XMLStore<T> where T : class, new()
    {
        private string _filename;
        private object _lock;

        public XMLStore(string filename)
        {
            _filename = filename;
            _lock = new object();
        }

        public T Load()
        {
            lock (_lock)
            {
                if (File.Exists(_filename) == false)
                    throw new FileNotFoundException("File not found", _filename);

                try
                {
                    Utils.XMLSerializer x = new Utils.XMLSerializer();
                    return x.ReadObjectFromFile(_filename) as T;
                }
                catch (ThreadAbortException)
                {
                    throw;
                }
                catch (Exception)
                {
                    if (File.Exists(_filename + ".invalid"))
                        File.Delete(_filename + ".invalid");
                    File.Move(_filename, _filename + ".invalid");
                    return new T();
                }
            }
        }

        public void Save(T data)
        {
            lock (_lock)
            {
                if (data != null)
                {
                    FileInfo fi = new FileInfo(_filename);
                    if (fi.Directory.Exists == false)
                        fi.Directory.Create();

                    string tmpFile = _filename + ".tmp";

                    //var settings = new System.Xml.XmlWriterSettings();
                    //settings.Indent = true;
                    //using (var writer = System.Xml.XmlDictionaryWriter.Create(_filename, settings))
                    //{
                    //    var x = new Utils.XMLSerializer();
                    //    x.WriteObject(writer, data);
                    //}

                    Utils.XMLSerializer x = new Utils.XMLSerializer();
                    XmlDoc doc = x.WriteObjectToXmlDoc(data);
                    doc.Save(tmpFile);

                    File.Copy(tmpFile, _filename, true);
                    File.Delete(tmpFile);
                }
            }
        }

    }
}

