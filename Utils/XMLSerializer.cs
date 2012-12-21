using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using System.ServiceModel.Description;
using System.Runtime.Serialization;
using System.Xml;
using System.IO;
using System.ComponentModel;
using System.Globalization;

namespace System.Xml.Serialization
{
    /// <summary>Instructs the XMLSerializer not to serialize the public field or public read/write property value if it is either null, or an empty string.</summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class XmlIgnoreIfNullOrEmptyAttribute : Attribute
    {
    }
}

namespace Utils
{
    public class XMLSerializerOperationBehavior : DataContractSerializerOperationBehavior
    {
        private static XMLSerializer serializer = new XMLSerializer();

        public XMLSerializerOperationBehavior(OperationDescription operationDescription) : base(operationDescription) { }

        public override XmlObjectSerializer CreateSerializer(Type type, string name, string ns, IList<Type> knownTypes)
        {
            return XMLSerializerOperationBehavior.serializer;
        }

        public override XmlObjectSerializer CreateSerializer(Type type, XmlDictionaryString name, XmlDictionaryString ns, IList<Type> knownTypes)
        {
            return XMLSerializerOperationBehavior.serializer;
        }
    }

    public class XMLSerializer : XmlObjectSerializer
    {
        private DefaultTypeSerializer _defaultTypeSerializer;

        public XMLSerializer()
        {
            _defaultTypeSerializer = new DefaultTypeSerializer();
            _defaultTypeSerializer.AddTypeSerializer(new IListSerializer());
            _defaultTypeSerializer.AddTypeSerializer(new IDictionarySerializer());
        }

        public override bool IsStartObject(XmlDictionaryReader reader)
        {
            return ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "XMLSerializer"));
        }

        public override object ReadObject(XmlDictionaryReader reader, bool verifyObjectName)
        {
            reader.ReadStartElement();
            string xml = reader.Value;
            reader.Skip();
            reader.ReadEndElement();
            return ReadObject(xml);
        }

        public override void WriteEndObject(XmlDictionaryWriter writer)
        {
        }
        public override void WriteObjectContent(XmlDictionaryWriter writer, object graph)
        {
            writer.WriteStartElement("XMLSerializer");

            string xml = WriteObject(graph);
            using (var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xml)))
            {
                var settings = new System.Xml.XmlReaderSettings();
                settings.IgnoreWhitespace = true;
                var w = XmlDictionaryReader.Create(ms, settings);
                writer.WriteNode(w, true);
            }

            writer.WriteEndElement();
        }
        public override void WriteStartObject(XmlDictionaryWriter writer, object graph)
        {
        }
        
        public string WriteObject(object o)
        {
            Xml.XmlDoc doc = new Utils.Xml.XmlDoc();
            Xml.XmlDocElement dataElement = doc["Data"];
            _defaultTypeSerializer.WriteObject(o, dataElement, null);
            return doc.ToString();
        }

        public Xml.XmlDoc WriteObjectToXmlDoc(object o)
        {
            Xml.XmlDoc doc = new Utils.Xml.XmlDoc();
            doc.AddXmlDeclaration("1.0", "utf-8", null);
            Xml.XmlDocElement rootElement = doc["XMLSerializer"];
            Xml.XmlDocElement dataElement = rootElement["Data"];
            _defaultTypeSerializer.WriteObject(o, dataElement, null);
            return doc;
        }

        public object ReadObject(string xml)
        {
            Xml.XmlDoc doc = Utils.Xml.XmlDoc.FromString(xml);
            Xml.XmlDocElement rootElement = doc["XMLSerializer"];
            Xml.XmlDocElement dataElement = rootElement["Data"];
            object result = null;
            _defaultTypeSerializer.ReadObject(ref result, dataElement, null);
            return result;
        }

        public void WriteObjectToFile(object o, string filename)
        {
            string directory = System.IO.Path.GetDirectoryName(filename);
            if (Directory.Exists(directory) == false)
                Directory.CreateDirectory(directory);

            string tmpFile = filename + ".tmp";

            XMLSerializer x = new XMLSerializer();
            var doc = x.WriteObjectToXmlDoc(o);
            doc.Save(tmpFile);

            File.Copy(tmpFile, filename, true);
            File.Delete(tmpFile);
        }

        public object ReadObjectFromFile(string filename)
        {
            if (!File.Exists(filename))
                return null;
            Xml.XmlDoc doc = new Utils.Xml.XmlDoc(filename);
            Xml.XmlDocElement rootElement = doc["XMLSerializer"];
            Xml.XmlDocElement dataElement = rootElement["Data"];
            object result = null;
            _defaultTypeSerializer.ReadObject(ref result, dataElement, null);
            return result;
        }

        private static string GetTypeFriendlyName(Type type)
        {
            if (type == null)
                throw new System.ArgumentNullException("type");

            string name = type.Name;
            if (type.IsGenericType)
            {
                int backqIndex = name.IndexOf('`');
                if (backqIndex == 0)
                {
                    throw new InvalidOperationException("Bad type name: " + name);
                }
                else if (backqIndex > 0)
                {
                    name = name.Substring(0, backqIndex);
                }

                name += "Of";

                foreach (Type genType in type.GetGenericArguments())
                {
                    name += GetTypeFriendlyName(genType);
                }
            }
            else if (type.IsArray)
            {
                Type t = type.GetElementType();
                name = String.Format("Array{0}Of{1}", type.GetArrayRank(), GetTypeFriendlyName(t));
            }

            return name;
        }



        public abstract class BaseTypeSerializer
        {
            private BaseTypeSerializer _defaultSerializer;

            public BaseTypeSerializer DefaultSerializer
            {
                get { return _defaultSerializer; }
                set { _defaultSerializer = value; }
            }

            public abstract bool CanSerializeType(Type type);
            public abstract void WriteObject(object o, Xml.XmlDocElement xmlElement, Type expectedType);
            public abstract void ReadObject(ref object o, Xml.XmlDocElement xmlElement, Type expectedType);


            private Dictionary<Type, List<Type>> _genericArgumentLookup = new Dictionary<Type, List<Type>>();
            protected Type GenericArgumentLookup(Type type, int index)
            {
                List<Type> result;
                if (!_genericArgumentLookup.TryGetValue(type, out result))
                {
                    result = new List<Type>(type.GetGenericArguments());
                    _genericArgumentLookup.Add(type, result);
                }
                return result[index];
            }


        }

        public class DefaultTypeSerializer : BaseTypeSerializer
        {
            protected List<BaseTypeSerializer> _typeSerializers = new List<BaseTypeSerializer>();

            private Dictionary<Type, string> _canRecreateLookup = new Dictionary<Type, string>();
            private Dictionary<string, object[]> _customAttributesLookup = new Dictionary<string, object[]>();

            class MemberData
            {
                public FieldInfo field;
                public PropertyDescriptor property;
            }
            private Dictionary<Type, Dictionary<string, MemberData>> _memberLookup = new Dictionary<Type,Dictionary<string,MemberData>>();

            public DefaultTypeSerializer()
            {
                DefaultSerializer = this;
            }

            public void AddTypeSerializer(BaseTypeSerializer typeSerializer)
            {
                RemoveTypeSerializer(typeSerializer);
                _typeSerializers.Insert(0, typeSerializer);
            }

            public void RemoveTypeSerializer(BaseTypeSerializer typeSerializer)
            {
                if (_typeSerializers.Contains(typeSerializer))
                    _typeSerializers.Remove(typeSerializer);
            }

            public override bool CanSerializeType(Type type)
            {
                return true;
            }

            public override void WriteObject(object o, Utils.Xml.XmlDocElement xmlElement, Type expectedType)
            {
                if (o == null)
                {
                    xmlElement.SetAttribute("null", "true");
                    return;
                }

                Type type = o.GetType();
                string typeName = null;

                string canRecreateTypeName;
                if (!_canRecreateLookup.TryGetValue(type, out canRecreateTypeName))
                {
                    canRecreateTypeName = typeName;
                    if (canRecreateTypeName == null)
                        canRecreateTypeName = type.AssemblyQualifiedNameWithoutVersion();
                    bool canRecreate = (Type.GetType(canRecreateTypeName) != null);
                    if (!canRecreate)
                    {
                        canRecreateTypeName = type.AssemblyQualifiedName;
                        canRecreate = (Type.GetType(canRecreateTypeName) != null);
                        if (!canRecreate)
                            canRecreateTypeName = "";
                    }
                    _canRecreateLookup.Add(type, canRecreateTypeName);
                }
                if (canRecreateTypeName == "")
                    throw new Exception("Could not recreate type from Assembly Qualified Name");

                if (expectedType == null)
                {
                    xmlElement.SetAttribute("type", canRecreateTypeName);
                }

                if (o is DateTime)
                {
                    xmlElement.Value = ((DateTime)o).ToString("yyyy-MM-dd HH:mm:ss");
                    return;
                }

                if (o is IConvertible)
                {
                    xmlElement.Value = Convert.ChangeType(o, typeof(string)) as string;
                    return;
                }

                if (type.IsPrimitive)
                {
                    throw new Exception("Primitive that's not convertible");
                }

                if (type.IsArray)
                {
                    Type itemType = type.GetElementType();

                    Array array = o as Array;
                    foreach (object childValue in array)
                    {
                        Xml.XmlDocElement childElement = xmlElement.AddElement("Item");
                        WriteObject(childValue, childElement, itemType);
                    }
                    return;
                }

                for (; type != typeof(object); type = type.BaseType)
                {
                    foreach (BaseTypeSerializer typeSerializer in _typeSerializers)
                    {
                        if (typeSerializer.CanSerializeType(type))
                        {
                            typeSerializer.DefaultSerializer = this;
                            typeSerializer.WriteObject(o, xmlElement, type);
                            return;
                        }
                    }

                    FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                    foreach (FieldInfo field in fields)
                    {
                        object[] xmlIgnoreAttributes;
                        string attributesKey = "ig:Field " + field.ToString();
                        if (!_customAttributesLookup.TryGetValue(attributesKey, out xmlIgnoreAttributes))
                        {
                            xmlIgnoreAttributes = field.GetCustomAttributes(typeof(System.Xml.Serialization.XmlIgnoreAttribute), true);
                            _customAttributesLookup.Add(attributesKey, xmlIgnoreAttributes);
                        }
                        if (xmlIgnoreAttributes.Length > 0)
                            continue;

                        attributesKey = "igEmpty:Field " + field.ToString();
                        if (!_customAttributesLookup.TryGetValue(attributesKey, out xmlIgnoreAttributes))
                        {
                            xmlIgnoreAttributes = field.GetCustomAttributes(typeof(System.Xml.Serialization.XmlIgnoreIfNullOrEmptyAttribute), true);
                            _customAttributesLookup.Add(attributesKey, xmlIgnoreAttributes);
                        }
                        bool ignoreIfNullOrEmpty = (xmlIgnoreAttributes.Length > 0);

                        object childValue = field.GetValue(o);
                        if (ignoreIfNullOrEmpty)
                        {
                            if (childValue == null)
                                continue;
                            if ((childValue is string) && string.IsNullOrEmpty(childValue as string))
                                continue;
                        }

                        Xml.XmlDocElement childElement = xmlElement[field.Name];

                        if (childValue == o)
                        {
                            childElement.SetAttribute("selfReference", "true");
                        }
                        else
                        {
                            WriteObject(childValue, childElement, field.FieldType);
                        }
                    }

                    PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                    foreach (PropertyInfo property in properties)
                    {
                        if (!property.CanRead || !property.CanWrite)
                            continue;

                        object[] xmlIgnoreAttributes;
                        string attributesKey = "ig:Property " + property.ToString();
                        if (!_customAttributesLookup.TryGetValue(attributesKey, out xmlIgnoreAttributes))
                        {
                            xmlIgnoreAttributes = property.GetCustomAttributes(typeof(System.Xml.Serialization.XmlIgnoreAttribute), true);
                            _customAttributesLookup.Add(attributesKey, xmlIgnoreAttributes);
                        }
                        if (xmlIgnoreAttributes.Length > 0)
                            continue;

                        attributesKey = "igEmpty:Property " + property.ToString();
                        if (!_customAttributesLookup.TryGetValue(attributesKey, out xmlIgnoreAttributes))
                        {
                            xmlIgnoreAttributes = property.GetCustomAttributes(typeof(System.Xml.Serialization.XmlIgnoreIfNullOrEmptyAttribute), true);
                            _customAttributesLookup.Add(attributesKey, xmlIgnoreAttributes);
                        }
                        bool ignoreIfNullOrEmpty = (xmlIgnoreAttributes.Length > 0);

                        object childValue = property.GetValue(o, null);
                        if (ignoreIfNullOrEmpty)
                        {
                            if (childValue == null)
                                continue;
                            if ((childValue is string) && string.IsNullOrEmpty(childValue as string))
                                continue;
                        }

                        Xml.XmlDocElement childElement = xmlElement[property.Name];

                        if (childValue == o)
                        {
                            childElement.SetAttribute("selfReference", "true");
                        }
                        else
                        {
                            WriteObject(childValue, childElement, property.PropertyType);
                        }
                    }
                }
            }

            private Dictionary<Type, Dictionary<string, object>> _enumParseLookup = new Dictionary<Type, Dictionary<string, object>>();
            private object EnumParse(Type type, string value)
            {
                Dictionary<string, object> enumLookup;
                if (!_enumParseLookup.TryGetValue(type, out enumLookup))
                {
                    enumLookup = new Dictionary<string, object>();
                    _enumParseLookup.Add(type, enumLookup);
                }

                object result;
                if (!enumLookup.TryGetValue(value, out result))
                {
                    result = Enum.Parse(type, value);
                    enumLookup.Add(value, result);
                }

                return result;
            }

            public override void ReadObject(ref object result, Utils.Xml.XmlDocElement xmlElement, Type expectedType)
            {
                result = null;

                string nullReference = xmlElement.GetAttribute("null");
                if (nullReference == "true")
                {
                    result = null;
                    return;
                }

                string typeString = xmlElement.GetAttribute("type");
                Type type = null;
                if ((typeString == null) || (typeString == ""))
                {
                    type = expectedType;
                }
                else
                {
                    type = Type.GetType(typeString);
                }

                if (type == null)
                    throw new Exception("Cannot load an unknown type");

                if (type.IsEnum)
                {
                    result = EnumParse(type, xmlElement.Value);
                    return;
                }

                if (typeof(string) == type)
                {
                    result = xmlElement.Value;
                    return;
                }
                if (typeof(double) == type)
                {
                    //result = double.Parse(xmlElement.Value);
                    result = XmlConvert.ToDouble(xmlElement.Value);
                    return;
                }
                if (typeof(int) == type)
                {
                    //result = int.Parse(xmlElement.Value);
                    result = XmlConvert.ToInt32(xmlElement.Value);
                    return;
                }
                if (typeof(DateTime) == type)
                {
                    DateTime dateTimeResult;
                    if (!DateTime.TryParseExact(xmlElement.Value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeResult))
                    {
                        dateTimeResult = DateTime.Parse(xmlElement.Value);
                    }
                    result = dateTimeResult;
                    return;
                }
                if (typeof(IConvertible).IsAssignableFrom(type))
                {
                    result = Convert.ChangeType(xmlElement.Value, type);
                    return;
                }

                if (type.IsPrimitive)
                {
                    throw new Exception("Primitive that's not convertible");
                }

                if (type.IsArray)
                {
                    Type itemType = type.GetElementType();

                    ArrayList arrayList = new ArrayList();
                    foreach (var childElement in xmlElement.GetChildElements("Item"))
                    {
                        object child = null;
                        ReadObject(ref child, childElement, itemType);
                        arrayList.Add(child);
                    }

                    result = arrayList.ToArray(itemType);
                    return;
                }

                try
                {
                    result = Activator.CreateInstance(type);
                }
                catch (MissingMethodException e)
                {
                    throw new MissingMethodException("No parameterless constructor defined for type " + type.FullName, e);
                }

                foreach (BaseTypeSerializer typeSerializer in _typeSerializers)
                {
                    if (typeSerializer.CanSerializeType(type))
                    {
                        typeSerializer.DefaultSerializer = this;
                        typeSerializer.ReadObject(ref result, xmlElement, type);
                        return;
                    }
                }

                var memberLookup = GetMemberLookup(type);

                foreach (var childElement in xmlElement.GetChildElements())
                {
                    MemberData member;
                    memberLookup.TryGetValue(childElement.Name, out member);
                    if (member == null)
                    {
                        // If there's an entry in the xml that doesn't exist in the class then we'll just ignore it.
                    }
                    else
                    {
                        string selfReference = childElement.GetAttribute("selfReference");
                        if (member.field != null)
                        {
                            var field = member.field;
                            if (selfReference == "true")
                            {
                                field.SetValue(result, result);
                            }
                            else
                            {
                                object childValue = null;
                                ReadObject(ref childValue, childElement, field.FieldType);
                                field.SetValue(result, childValue);
                            }
                        }
                        else if (member.property != null)
                        {
                            var property = member.property;
                            if (selfReference == "true")
                            {
                                property.SetValue(result, result);
                            }
                            else
                            {
                                object childValue = null;
                                ReadObject(ref childValue, childElement, property.PropertyType);
                                property.SetValue(result, childValue);
                            }
                        }
                    }
                }

                for (type = type.BaseType; type != typeof(object); type = type.BaseType)
                {
                    foreach (BaseTypeSerializer typeSerializer in _typeSerializers)
                    {
                        if (typeSerializer.CanSerializeType(type))
                        {
                            typeSerializer.DefaultSerializer = this;
                            typeSerializer.ReadObject(ref result, xmlElement, type);
                            return;
                        }
                    }
                }


            }

            private Dictionary<string, MemberData> GetMemberLookup(Type type)
            {
                Dictionary<string, MemberData> memberLookup;
                _memberLookup.TryGetValue(type, out memberLookup);
                if (memberLookup == null)
                {
                    Hyper.ComponentModel.HyperTypeDescriptionProvider.Add(type);

                    memberLookup = new Dictionary<string, MemberData>();
                    FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var field in fields)
                    {
                        memberLookup.Add(field.Name, new MemberData()
                        {
                            field = field
                        });
                    }

                    var properties = TypeDescriptor.GetProperties(type);
                    foreach (PropertyDescriptor propertyDescriptor in properties)
                    {
                        memberLookup.Add(propertyDescriptor.Name, new MemberData()
                        {
                            property = propertyDescriptor
                        });
                    }

                    _memberLookup.Add(type, memberLookup);
                }
                return memberLookup;
            }

        }

        public class IListSerializer : BaseTypeSerializer
        {
            private Dictionary<Type, bool> _canSerializeTypeLookup = new Dictionary<Type, bool>();
            public override bool CanSerializeType(Type type)
            {
                bool result = false;
                if (!_canSerializeTypeLookup.TryGetValue(type, out result))
                {
                    if (type == typeof(ArrayList))
                        result = true;

                    var typeGenericDefinition = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
                    if (typeGenericDefinition == typeof(List<>))
                        result = true;
                    if (typeGenericDefinition == typeof(HashSet<>))
                        result = true;

                    _canSerializeTypeLookup.Add(type, result);
                }

                return result;
            }

            public override void WriteObject(object o, Utils.Xml.XmlDocElement xmlElement, Type expectedType)
            {
                Type itemType = null;
                if ((expectedType != null) && (expectedType.IsGenericType))
                {
                    itemType = GenericArgumentLookup(expectedType, 0);
                }

                // Make sure the element gets created when there are zero elements in the list
                xmlElement.MakeSureElementExists();
                foreach (object childValue in (o as System.Collections.IEnumerable))
                {
                    Xml.XmlDocElement childElement = xmlElement.AddElement("Item");
                    DefaultSerializer.WriteObject(childValue, childElement, itemType);
                }
            }

            public override void ReadObject(ref object result, Utils.Xml.XmlDocElement xmlElement, Type expectedType)
            {
                if (result == null)
                    throw new Exception("Cannot load an unknown type of list");

                Type itemType = null;
                if ((expectedType != null) && (expectedType.IsGenericType))
                {
                    itemType = GenericArgumentLookup(expectedType, 0);
                }

                var list = result as IList;
                foreach (Xml.XmlDocElement itemElement in xmlElement.GetChildElements("Item"))
                {
                    object child = null;
                    DefaultSerializer.ReadObject(ref child, itemElement, itemType);
                    if (list != null)
                        list.Add(child);
                    else
                        expectedType.InvokeMember("Add", BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod, null, result, new object[] { child });
                }
            }
        }

        public class IDictionarySerializer : BaseTypeSerializer
        {
            private Dictionary<Type, bool> _canSerializeTypeLookup = new Dictionary<Type, bool>();
            public override bool CanSerializeType(Type type)
            {
                bool result = false;
                if (!_canSerializeTypeLookup.TryGetValue(type, out result))
                {
                    if (type == typeof(Hashtable))
                        result = true;

                    var typeGenericDefinition = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
                    if (typeGenericDefinition == typeof(System.Collections.Generic.Dictionary<,>))
                        result = true;

                    _canSerializeTypeLookup.Add(type, result);
                }

                return result;
            }

            public override void WriteObject(object o, Utils.Xml.XmlDocElement xmlElement, Type expectedType)
            {
                Type keyType = null;
                Type valueType = null;
                if ((expectedType != null) && (expectedType.IsGenericType))
                {
                    keyType = GenericArgumentLookup(expectedType, 0);
                    valueType = GenericArgumentLookup(expectedType, 1);
                    xmlElement.SetAttribute("keyType", keyType.AssemblyQualifiedNameWithoutVersion());
                    xmlElement.SetAttribute("valueType", valueType.AssemblyQualifiedNameWithoutVersion());
                }

                // Make sure the element gets created when there are zero elements in the list
                xmlElement.MakeSureElementExists();
                IDictionary dict = o as System.Collections.IDictionary;
                foreach (DictionaryEntry entry in dict)
                {
                    Xml.XmlDocElement itemElement = xmlElement.AddElement("Item");
                    Xml.XmlDocElement keyElement = itemElement.AddElement("Key");
                    DefaultSerializer.WriteObject(entry.Key, keyElement, keyType);
                    Xml.XmlDocElement valueElement = itemElement.AddElement("Value");
                    DefaultSerializer.WriteObject(entry.Value, valueElement, valueType);
                }
            }

            public override void ReadObject(ref object result, Utils.Xml.XmlDocElement xmlElement, Type expectedType)
            {
                if (result == null)
                    throw new Exception("Cannot load an unknown type of dictionary");

                Type keyType = null;
                Type valueType = null;
                if ((expectedType != null) && (expectedType.IsGenericType))
                {
                    keyType = GenericArgumentLookup(expectedType, 0);
                    valueType = GenericArgumentLookup(expectedType, 1);
                }

                IDictionary dictionary = result as IDictionary;
                foreach (Xml.XmlDocElement itemElement in xmlElement.GetChildElements("Item"))
                {
                    Xml.XmlDocElement keyElement = itemElement["Key"];
                    Xml.XmlDocElement valueElement = itemElement["Value"];
                    if (keyElement.Exists && valueElement.Exists)
                    {
                        object key = null;
                        DefaultSerializer.ReadObject(ref key, keyElement, keyType);
                        object value = null;
                        DefaultSerializer.ReadObject(ref value, valueElement, valueType);

                        dictionary.Add(key, value);
                    }
                }
            }
        }

    }

}
