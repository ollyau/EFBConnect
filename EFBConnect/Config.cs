using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace EFBConnect
{
    class Config
    {
        private object _lock;
        private string _path;
        private Dictionary<string, object> _data;
        private DataContractSerializer _serializer;

        public Config(List<Type> knownTypes = null)
        {
            _lock = new object();
            _data = new Dictionary<string, object>();
            _serializer = knownTypes == null ? new DataContractSerializer(typeof(Dictionary<string, object>)) : new DataContractSerializer(typeof(Dictionary<string, object>), knownTypes);
        }

        public Config(string path, List<Type> knownTypes = null)
            : this(knownTypes)
        {
            _path = path;
            if (File.Exists(path))
            {
                Load(path);
            }
        }

        public bool Load()
        {
            if (!string.IsNullOrEmpty(_path) && File.Exists(_path))
            {
                Load(_path);
                return true;
            }
            return false;
        }

        public bool Save()
        {
            if (!string.IsNullOrEmpty(_path))
            {
                Save(_path);
                return true;
            }
            return false;
        }

        public void Load(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open))
            using (var reader = new XmlTextReader(fs))
            {
                _data = (Dictionary<string, object>)_serializer.ReadObject(reader);
            }
        }

        public void Save(string path)
        {
            using (var fs = new FileStream(path, FileMode.Create))
            {
                _serializer.WriteObject(fs, _data);
            }
        }

        public T Get<T>(string key)
        {
            lock (_lock)
            {
                object value;
                var result = _data.TryGetValue(key, out value);
                if (result)
                {
                    return (T)value;
                }
                else
                {
                    throw new KeyNotFoundException(string.Format("Key '{0}' not found in configuration.", key));
                }
            }
        }

        public T Get<T>(string key, T defaultValue, bool addDefaultValue = false)
        {
            lock (_lock)
            {
                object value;
                var result = _data.TryGetValue(key, out value);
                if (result)
                {
                    return (T)value;
                }
                else
                {
                    if (addDefaultValue)
                    {
                        _data.Add(key, defaultValue);
                    }
                    return defaultValue;
                }
            }
        }

        public void Set<T>(string key, T value)
        {
            lock (_lock)
            {
                var result = _data.ContainsKey(key);
                if (!result)
                {
                    _data.Add(key, value);
                }
                else
                {
                    _data[key] = value;
                }
            }
        }
    }
}