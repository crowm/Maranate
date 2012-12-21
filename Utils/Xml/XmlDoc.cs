using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Utils.Xml
{
    public interface XmlDocElementCollection
    {
        XmlDocElement this[string elementName] { get; }
        IEnumerable<XmlDocElement> GetChildElements();
        IEnumerable<XmlDocElement> GetChildElements(string elementName);
        void AddElement(XmlElement xmlElement);
        string XPath { get; }
    }


    public class XmlDoc : XmlDocElementCollection
    {
        private XmlDocument _xmlDocument;
        private string _filename;

        public string Filename
        {
            get { return _filename; }
            set { _filename = value; }
        }

        public XmlDoc()
        {
            _xmlDocument = new XmlDocument();
            _filename = null;
        }

        public XmlDoc(string filename)
        {
            _xmlDocument = new XmlDocument();
            _xmlDocument.Load(filename);
            _filename = filename;
        }

        public string XPath
        {
            get
            {
                return "DocRoot";
            }
        }

        public void Save()
        {
            if (_filename == null)
                throw new InvalidOperationException("Filename must be set before saving");
            _xmlDocument.Save(_filename);
        }

        public void Save(string filename)
        {
            Filename = filename;
            Save();
        }

        public override string ToString()
        {
            using (var sw = new System.IO.StringWriter())
            {
                using (var xtw = new XmlTextWriter(sw))
                {
                    xtw.Formatting = Formatting.Indented;
                    _xmlDocument.WriteTo(xtw);
                    return sw.ToString();
                }
            }
        }

        public static XmlDoc FromString(string xml)
        {
            XmlDoc doc = new XmlDoc();
            doc._xmlDocument.LoadXml(xml);
            return doc;
        }

        public XmlDocElement this[string elementName]
        {
            get
            {
                XmlElement node = _xmlDocument[elementName];
                return new XmlDocElement(elementName, _xmlDocument, node, this);
            }
        }

        public IEnumerable<XmlDocElement> GetChildElements()
        {
            List<XmlDocElement> result = new List<XmlDocElement>();
            foreach (XmlNode node in _xmlDocument.ChildNodes)
            {
                if (node.NodeType != XmlNodeType.Element)
                    continue;
                yield return new XmlDocElement(node.Name, _xmlDocument, node as XmlElement, this);
            }
        }
        public IEnumerable<XmlDocElement> GetChildElements(string elementName)
        {
            List<XmlDocElement> result = new List<XmlDocElement>();
            foreach (XmlNode node in _xmlDocument.ChildNodes)
            {
                if (node.NodeType != XmlNodeType.Element)
                    continue;
                if (elementName != node.Name)
                    continue;
                yield return new XmlDocElement(node.Name, _xmlDocument, node as XmlElement, this);
            }
        }

        void XmlDocElementCollection.AddElement(XmlElement xmlElement)
        {
            _xmlDocument.AppendChild(xmlElement);
        }

        public void AddXmlDeclaration(string version, string encoding, string standalone)
        {
            _xmlDocument.AppendChild(_xmlDocument.CreateXmlDeclaration(version, encoding, standalone));
        }

        public XmlDocElement GetElementFromPath(string path)
        {
            XmlDocElement element = null;
            XmlDocElementCollection parent = this;

            string[] elementNames = path.Trim('/').Split('/');
            foreach (string elementName in elementNames)
            {
                element = parent[elementName];
                parent = element;
            }

            return element;
        }

        public XmlDocElement[] GetElementsFromPath(string path)
        {
            List<XmlDocElement> childElements = new List<XmlDocElement>();
            List<XmlDocElementCollection> parentElements = new List<XmlDocElementCollection>();
            parentElements.Add(this);

            string[] elementNames = path.Trim('/').Split('/');
            foreach (string elementName in elementNames)
            {
                childElements.Clear();
                foreach (XmlDocElementCollection elementCollection in parentElements)
                {
                    childElements.AddRange(elementCollection.GetChildElements(elementName));
                }

                parentElements.Clear();
                foreach (XmlDocElement element in childElements)
                {
                    parentElements.Add(element);
                }
            }

            return childElements.ToArray();
        }

    }

    public class XmlDocElement : XmlDocElementCollection
    {
        private string _elementName;
        private XmlDocument _xmlDocument;
        private XmlElement _xmlElement;
        private XmlDocElementCollection _xmlElementCollection;

        public XmlDocElement(string elementName, XmlDocument xmlDocument, XmlElement xmlElement, XmlDocElementCollection xmlElementCollection)
        {
            _elementName = elementName;
            _xmlDocument = xmlDocument;
            _xmlElement = xmlElement;
            _xmlElementCollection = xmlElementCollection;
        }

        public string XPath
        {
            get
            {
                return _xmlElementCollection.XPath + @"\" + _elementName;
            }
        }

        public string Name
        {
            get
            {
                return _elementName;
            }
        }

        public string Value
        {
            get
            {
                if (_xmlElement == null)
                    return null;
                return _xmlElement.InnerText;
            }
            set
            {
                MakeSureElementExists();
                _xmlElement.InnerText = value;
            }
        }

        public bool Exists
        {
            get
            {
                return (_xmlElement != null);
            }
        }

        public XmlDocElement this[string elementName]
        {
            get
            {
                XmlElement node = null;
                if (_xmlElement != null)
                    node = _xmlElement[elementName];
                return new XmlDocElement(elementName, _xmlDocument, node, this);
            }
        }

        public IEnumerable<XmlDocElement> GetChildElements()
        {
            List<XmlDocElement> result = new List<XmlDocElement>();
            if (_xmlElement != null)
            {
                foreach (XmlNode node in _xmlElement.ChildNodes)
                {
                    if (node.NodeType != XmlNodeType.Element)
                        continue;
                    yield return new XmlDocElement(node.Name, _xmlDocument, node as XmlElement, this);
                }
            }
        }

        public IEnumerable<XmlDocElement> GetChildElements(string elementName)
        {
            List<XmlDocElement> result = new List<XmlDocElement>();
            if (_xmlElement != null)
            {
                foreach (XmlNode node in _xmlElement.ChildNodes)
                {
                    if (node.NodeType != XmlNodeType.Element)
                        continue;
                    if (elementName != node.Name)
                        continue;
                    yield return new XmlDocElement(node.Name, _xmlDocument, node as XmlElement, this);
                }
            }
        }

        public string GetValue(string defaultValue)
        {
            if (_xmlElement == null)
                return defaultValue;
            return _xmlElement.InnerText;
        }

        public string GetAttribute(string attributeName)
        {
            if (_xmlElement == null)
                return null;
            return _xmlElement.GetAttribute(attributeName);
        }
        public string GetAttribute(string attributeName, string defaultValue)
        {
            string result = GetAttribute(attributeName);
            if (result == null)
                return defaultValue;
            return result;
        }
        public void SetAttribute(string attributeName, string value)
        {
            MakeSureElementExists();
            _xmlElement.SetAttribute(attributeName, value);
        }

        void XmlDocElementCollection.AddElement(XmlElement xmlElement)
        {
            MakeSureElementExists();
            //_xmlDocument.AppendChild(xmlElement);
            _xmlElement.AppendChild(xmlElement);
        }

        public XmlDocElement AddElement(string elementName)
        {
            return new XmlDocElement(elementName, _xmlDocument, null, this);
        }

        public void MakeSureElementExists()
        {
            if (_xmlElement == null)
            {
                _xmlElement = _xmlDocument.CreateElement(_elementName);
                _xmlElement.IsEmpty = true;
                _xmlElementCollection.AddElement(_xmlElement);
            }
        }

    }
}
