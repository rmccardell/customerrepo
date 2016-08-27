using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace LibEntityPersistence.Models
{
    public abstract class PersistableEntity<T> : IEquatable<PersistableEntity<T>> where T : PersistableEntity<T>, IEquatable<T>
    {
        public Guid? Id { get; set; }

        private static EntityCollection<T> _entityCollection;

        // ReSharper disable once StaticMemberInGenericType
        private static readonly object Locker = new object();

        // ReSharper disable once StaticMemberInGenericType
        private static XmlSerializer _entitySerializer;

        private static XmlSerializer EntitySerializer
        {
            get
            {
                // only create a new instance if one doesn't already exist.
                if (_entitySerializer == null)
                {
                    // use this lock to ensure that only one thread is access
                    // this block of code at once.
                    lock (Locker)
                    {
                        if (_entitySerializer == null)
                        {
                            _entitySerializer =  new XmlSerializer(typeof(EntityCollection<T>));

                        }
                    }
                }
                
                return _entitySerializer;
            }
        }

        private static string SerializeEntity(EntityCollection<T> entity)
        {
            var xmlString = new StringBuilder();
            TextWriter xmlWriter = new EncodingStringWriter(xmlString, Encoding.UTF8);

            try
            {
                EntitySerializer.Serialize(xmlWriter, entity);
                return xmlString.ToString();
            }
            finally
            {
                xmlWriter.Close();
                xmlWriter.Dispose();
            }
        }

        // ReSharper disable once StaticMemberInGenericType
        private static string _entityName;

        private static string StorageName
        {
            get
            {

                _entityName = _entityName ?? typeof(T).Name;
                // ReSharper disable once UseStringInterpolation
                return string.Format("{0}.xml", _entityName);
            }
        }

        private static void HydrateEntityCollectionFromStorage(string path)
        {
            lock (Locker)
            {
                using (var fs = new FileStream(StorageName, FileMode.Open))
                {
                    _entityCollection = (EntityCollection<T>)EntitySerializer.Deserialize(fs);
                }
            }
        }

        private static int GetItemIndex(Guid? id)
        {
            return _entityCollection.Items.FindIndex(item => item.Id == id);
        }

        private static void Remove(Guid? id)
        {
            var storedItemIndx = GetItemIndex(id);

            if (storedItemIndx > -1)
            {
                _entityCollection.Items.RemoveAt(storedItemIndx);
                SerializeEntity(_entityCollection);
            }

        }

        static PersistableEntity()
        {
            //attempt to load entity collection from file storage
            if (File.Exists(StorageName))
            {
                HydrateEntityCollectionFromStorage(StorageName);
            }
            else
            {
                _entityCollection = new EntityCollection<T>();
            }
        }

        public abstract T Create(PersistableEntity<T>  entity);

        public void Save()
        {
            if (Id == null)
            {
                Id = Guid.NewGuid();
                T item = Create(this);
                _entityCollection.Items.Add(item);
            }
            else
            {
                int index = GetItemIndex(Id);

                if (index > -1)
                    _entityCollection.Items[index] = (T)this;
            }

            string collection = SerializeEntity(_entityCollection);

            lock (Locker)
            {
                File.WriteAllText(StorageName, collection);
            }
        }

        public virtual void Delete()
        {
            Remove(Id);
            Id = null;
        }

        public static T Find(Guid? id)
        {
            // ReSharper disable once UseNullPropagation
            if (_entityCollection == null)
                return null;

            var item = (T)_entityCollection.Items.Cast<PersistableEntity<T>>().FirstOrDefault(e => e.Id == id);

            return item;
        }

        public bool Equals(PersistableEntity<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PersistableEntity<T>) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    internal class EncodingStringWriter : StringWriter
    {
        private readonly Encoding _encoding;

        public EncodingStringWriter(StringBuilder builder, Encoding encoding) : base(builder)
        {
            _encoding = encoding;
        }

        // ReSharper disable once ConvertToAutoProperty
        public override Encoding Encoding
        {
            get { return _encoding; }
        }
    }

    public class EntityCollection<T> where T : PersistableEntity<T>, IEquatable<T>
    {
        public List<T> Items { get; set; }

        public EntityCollection()
        {
            Items = new List<T>();
        }
    }
}

