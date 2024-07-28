// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore.BasicMetaDataStoreReader`2
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using Microsoft.VisualStudio.Services.Search.Server.Storage.FileSystem.Definitions;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;

namespace Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore
{
  public class BasicMetaDataStoreReader<T, TEncoder> : 
    IMetaDataStore<T>,
    IMetaDataStore,
    IEnumerable,
    IEnumerable<T>
    where T : IMetaDataStoreItem, new()
    where TEncoder : IMetaDataStoreEncoder<T>, new()
  {
    private IReadOnlyDictionary<string, string> m_propertyBag;
    private readonly string m_path;
    private readonly IFileSystem m_fileSystem;
    private IList<string> m_contentKeys;

    public BasicMetaDataStoreReader(string path, IFileSystem fileSystem)
    {
      this.m_path = path;
      this.m_fileSystem = fileSystem;
    }

    public string this[string key]
    {
      get
      {
        if (this.m_propertyBag == null)
        {
          using (IFileReader dataStream = this.GetDataStream())
            this.m_propertyBag = new BasicMetaDataStoreReader<T, TEncoder>.DataReader(dataStream).PropertyBag;
        }
        return this.m_propertyBag[key];
      }
      set => throw new NotSupportedException();
    }

    public IList<string> ContentKeys
    {
      get
      {
        if (this.m_contentKeys == null)
        {
          using (IFileReader dataStream = this.GetDataStream())
            this.m_contentKeys = (IList<string>) new BasicMetaDataStoreReader<T, TEncoder>.DataReader(dataStream).ContentKeys;
        }
        return this.m_contentKeys;
      }
      set => throw new NotSupportedException();
    }

    public void AddOrUpdate(T item) => throw new NotSupportedException();

    public void Add(string key, T item, bool throwOnDuplicates) => throw new NotSupportedException();

    public int CompleteAdding() => throw new NotSupportedException();

    public IEnumerator<T> GetEnumerator()
    {
      using (IFileReader dataStream = this.GetDataStream())
      {
        BasicMetaDataStoreReader<T, TEncoder>.DataReader reader = new BasicMetaDataStoreReader<T, TEncoder>.DataReader(dataStream);
        while (reader.Items.MoveNext())
          yield return reader.Items.Current;
        reader = (BasicMetaDataStoreReader<T, TEncoder>.DataReader) null;
      }
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    private IFileReader GetDataStream() => this.m_fileSystem.GetDirectory(Path.GetDirectoryName(this.m_path) ?? "\\", true).GetFileReader(Path.GetFileName(this.m_path));

    private class DataReader
    {
      private TEncoder m_encoder;
      private readonly Dictionary<string, string> m_propertyBag = new Dictionary<string, string>();
      private List<string> m_contentKeys = new List<string>();
      private IFileReader m_reader;

      public DataReader(IFileReader reader)
      {
        this.m_encoder = new TEncoder();
        this.m_encoder.Initialize();
        this.m_reader = reader;
        string str1 = reader.ReadString();
        if (typeof (T).Name != str1)
        {
          IOException e = new IOException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DataReader : The item type specified was {0}, but the store has {1}.", (object) typeof (T).Name, (object) str1));
          e.Data[(object) "Reason"] = (object) "ItemTypeMismatch";
          Tracer.TraceException(1080405, "Indexing Pipeline", "Framework", (Exception) e);
        }
        string str2 = reader.ReadString();
        if (typeof (TEncoder).Name != str2)
        {
          IOException e = new IOException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DataReader : The item encoder type specified was {0}, but the store has {1}.", (object) typeof (TEncoder).Name, (object) str2));
          e.Data[(object) "Reason"] = (object) "ItemEncoderTypeMismatch";
          Tracer.TraceException(1080405, "Indexing Pipeline", "Framework", (Exception) e);
        }
        int num1 = reader.ReadInt32();
        for (int index = 0; index < num1; ++index)
          this.m_propertyBag[reader.ReadString()] = reader.ReadString();
        int num2 = reader.ReadInt32();
        for (int index = 0; index < num2; ++index)
          this.m_contentKeys.Add(reader.ReadString());
        this.Items = this.GetEnumerator();
      }

      public IEnumerator<T> Items { get; private set; }

      public IReadOnlyDictionary<string, string> PropertyBag => (IReadOnlyDictionary<string, string>) new ReadOnlyDictionary<string, string>((IDictionary<string, string>) this.m_propertyBag);

      public List<string> ContentKeys => this.m_contentKeys;

      private IEnumerator<T> GetEnumerator()
      {
        while (this.m_reader.CanRead())
        {
          T obj = new T();
          this.m_encoder.Decode(this.m_reader, obj);
          yield return obj;
        }
      }
    }
  }
}
