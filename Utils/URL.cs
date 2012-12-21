using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Web;

namespace Utils
{
    public class Url
    {
        private string _path;
        private string _fullPath;

        private QueryStringCollection _queryString;
        /// <summary>
        /// Returns the Path portion of the url. No hostname or query string.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string Path
        {
            get { return _path; }
            set
            {
                if ((!value.StartsWith("/")))
                {
                    value = "/" + value;
                }
                _fullPath = _fullPath.Substring(0, _fullPath.Length - _path.Length) + value;
                _path = value;
            }
        }

        /// <summary>
        /// Returns the Path including the hostname if it exists. No query string.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string FullPath
        {
            get { return _fullPath; }
            set
            {
                _fullPath = value;
                _path = _fullPath;
                int index = _path.IndexOf("//");
                if ((index >= 0))
                {
                    index = _path.IndexOf("/", index + 2);
                    if ((index >= 0))
                    {
                        _path = _path.Substring(index);
                    }
                }
            }
        }

        /// <summary>
        /// Returns the query string as a collection
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public QueryStringCollection QueryString
        {
            get { return _queryString; }
        }

        /// <summary>
        /// Returns the query string as a string.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string Query
        {
            get
            {
                StringBuilder result = new StringBuilder();

                for (int pos = 0; pos <= _queryString.Count - 1; pos++)
                {
                    if ((pos > 0))
                    {
                        result.Append("&");
                    }

                    string key = _queryString.GetKey(pos);
                    key = HttpUtility.UrlEncode(key);
                    if ((key == null))
                        key = string.Empty;

                    string value = _queryString[pos];
                    value = HttpUtility.UrlEncode(value);
                    if ((value == null))
                        value = string.Empty;

                    if ((key.Length > 0))
                    {
                        result.Append(key);
                    }
                    if (((key.Length > 0) & (value.Length > 0)))
                    {
                        result.Append("=");
                    }
                    if ((value.Length > 0))
                    {
                        result.Append(value);
                    }
                }

                return result.ToString();
            }
            set
            {
                _queryString.Clear();
                ParseQueryString(value);
            }
        }

        /// <summary>
        /// Returns the Url including hostname and query string if they exist.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string FullUrl
        {
            get
            {
                string query = this.Query;
                if ((query.Length > 0))
                {
                    return this.FullPath + "?" + query;
                }
                else
                {
                    return this.FullPath;
                }
            }
            set
            {
                int index = FullUrl.IndexOf("?");
                if ((index >= 0))
                {
                    FullPath = FullUrl.Substring(0, index);
                    Query = FullUrl.Substring(index + 1);
                }
                else
                {
                    FullPath = FullUrl;
                    Query = "";
                }
            }
        }

        /// <summary>
        /// Returns the Url including Path and Query if it exists. No hostname.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string PathAndQuery
        {
            get
            {
                string query = this.Query;
                if ((query.Length > 0))
                {
                    return this.Path + "?" + query;
                }
                else
                {
                    return this.Path;
                }
            }
        }


        public string PageName
        {
            get { return Path.Substring(Path.LastIndexOf("/") + 1); }
        }

        ///'''''''''''''''''''''''

        public Url(string url, bool fullUrl)
        {
            _queryString = new QueryStringCollection();

            string path = url;
            int index = url.IndexOf("?");
            if ((index >= 0))
            {
                path = url.Substring(0, index);
                Query = url.Substring(index + 1);
            }

            if ((fullUrl))
            {
                FullPath = path;
            }
            else
            {
                _fullPath = path;
                _path = path;
            }

        }

        public Url(System.Web.HttpRequest request)
            : this(request.Url.AbsoluteUri, true)
        {
            _queryString = new QueryStringCollection(request.QueryString);
        }

        public override string ToString()
        {
            return FullUrl;
        }

        public void AddQueryString(string query)
        {
            ParseQueryString(query);
        }

        private void ParseQueryString(string query)
        {
            int pos = 0;

            while ((pos < query.Length))
            {
                int startPos = pos;
                int @equals = -1;
                string name = "";
                string value = "";

                while ((pos < query.Length))
                {
                    char character = query[pos];
                    if (((@equals == -1) && (character == '=')))
                    {
                        @equals = pos;
                    }
                    else if ((character == '&'))
                    {
                        break; // TODO: might not be correct. Was : Exit While
                    }
                    pos += 1;
                }

                if ((@equals >= 0))
                {
                    name = query.Substring(startPos, @equals - startPos);
                    value = query.Substring(@equals + 1, (pos - @equals) - 1);
                }
                else
                {
                    value = query.Substring(startPos, pos - startPos);
                }

                name = HttpUtility.UrlDecode(name);
                value = HttpUtility.UrlDecode(value);
                _queryString.Add(name, value);

                pos += 1;
            }
        }

        public class QueryStringCollection
        {
            private List<string> _keys = new List<string>();
            private List<string> _values = new List<string>();
            private bool _ignoreDuplicateNames = false;
            public bool IgnoreDuplicateNames
            {
                get
                {
                    return this._ignoreDuplicateNames;
                }
                set
                {
                    this._ignoreDuplicateNames = value;
                }
            }
            public string[] Values
            {
                get
                {
                    return this._values.ToArray();
                }
            }
            public string[] Names
            {
                get
                {
                    return this._keys.ToArray();
                }
            }
            public int Count
            {
                get
                {
                    return this._keys.Count;
                }
            }
            public string this[int index]
            {
                get
                {
                    return this._values[index];
                }
                set
                {
                    this._values[index] = value;
                }
            }
            public string this[string name]
            {
                get
                {
                    for (int i = 0; i < _keys.Count; i++)
                    {
                        var key = _keys[i];
                        if (key.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                            return _values[i];
                    }
                    return null;
                }
                set
                {
                    for (int i = 0; i < _keys.Count; i++)
                    {
                        var key = _keys[i];
                        if (key.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                            _values[i] = value;
                    }
                    this.Add(name, value);
                }
            }
            public QueryStringCollection()
            {
            }
            public QueryStringCollection(NameValueCollection col)
            {
                for (int i = 0; i < col.Count; i++)
                {
                    var name = col.GetKey(i);
                    var value = col[i];

                    if (name == null)
                        name = "";
                    if (value == null)
                        value = "";

                    Add(name, value);
                }
            }
            public bool ContainsName(string name)
            {
                return (this._keys.Find(obj => obj.Equals(name, StringComparison.CurrentCultureIgnoreCase)) != null);
            }
            public bool ContainsValue(string value)
            {
                return this._values.Contains(value);
            }
            public void Add(string name, string value)
            {
                this._keys.Add(name);
                this._values.Add(value);
            }
            public void Remove(string name)
            {
                for (int i = 0; i < _keys.Count; i++)
                {
                    if (_keys[i].Equals(name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        _keys.RemoveAt(i);
                        _values.RemoveAt(i);
                    }
                }
            }
            public string GetKey(int index)
            {
                return this._keys[index];
            }
            public void Clear()
            {
                this._keys.Clear();
                this._values.Clear();
            }
        }
    }

}
