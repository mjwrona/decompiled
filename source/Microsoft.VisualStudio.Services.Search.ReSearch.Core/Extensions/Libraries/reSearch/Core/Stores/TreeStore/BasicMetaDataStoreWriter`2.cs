// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore.BasicMetaDataStoreWriter`2
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Server.Storage.FileSystem.Definitions;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore
{
  public class BasicMetaDataStoreWriter<T, TEncoder> : 
    IMetaDataStore<T>,
    IMetaDataStore,
    IEnumerable,
    IEnumerable<T>
    where T : IMetaDataStoreItem, new()
    where TEncoder : IMetaDataStoreEncoder<T>, new()
  {
    [StaticSafe]
    private static readonly TraceMetaData s_traceMetadata = new TraceMetaData(1080405, "Indexing Pipeline", "Crawl");
    private bool m_completeAdding;
    private Dictionary<string, string> m_propertyBag = new Dictionary<string, string>();
    private readonly string m_path;
    private IDictionary<string, T> m_fileDictionary;
    private IList<string> m_contentKeys;
    private IFileSystem m_fileStore;

    public BasicMetaDataStoreWriter(StringComparer stringComparer) => this.InitializeFileDictionary(stringComparer);

    public string this[string key]
    {
      get => this.m_propertyBag[key];
      set
      {
        Dictionary<string, string> propertyBag = this.m_propertyBag;
        string key1 = key;
        propertyBag[key1] = value ?? throw new ArgumentNullException(nameof (value), FormattableString.Invariant(FormattableStringFactory.Create("Cannot assign null to key [{0}].", (object) key)));
      }
    }

    public IList<string> ContentKeys
    {
      get
      {
        if (this.m_contentKeys == null)
          this.m_contentKeys = (IList<string>) new List<string>();
        return this.m_contentKeys;
      }
      set => this.m_contentKeys = value;
    }

    public BasicMetaDataStoreWriter(
      string path,
      IFileSystem fileSystem,
      StringComparer stringComparer)
    {
      if (path == null)
        throw new ArgumentNullException(nameof (path));
      if (fileSystem == null)
        throw new ArgumentNullException(nameof (fileSystem));
      this.m_path = path;
      this.m_fileStore = fileSystem;
      this.InitializeFileDictionary(stringComparer);
    }

    private void InitializeFileDictionary(StringComparer stringComparer) => this.m_fileDictionary = (IDictionary<string, T>) new SortedDictionary<string, T>((IComparer<string>) stringComparer);

    public void AddOrUpdate(T item) => this.m_fileDictionary[item.Path] = (object) item != null ? item : throw new ArgumentNullException(nameof (item));

    public void Add(string key, T item, bool throwOnDuplicates)
    {
      if (key == null)
        throw new ArgumentNullException(nameof (key));
      if ((object) item == null)
        throw new ArgumentNullException(nameof (item));
      T obj;
      if (this.m_fileDictionary.TryGetValue(key, out obj))
      {
        string message;
        if ((object) obj != null)
        {
          if (obj.BranchesInfo != null)
          {
            string str = string.Join(", ", (obj.BranchesInfo.Count > 5 ? (IEnumerable<BranchInfo>) obj.BranchesInfo.Take<BranchInfo>(5).ToList<BranchInfo>() : (IEnumerable<BranchInfo>) obj.BranchesInfo).Select<BranchInfo, string>((Func<BranchInfo, string>) (x => x.BranchName)));
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(BasicMetaDataStoreWriter<T, TEncoder>.s_traceMetadata, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Branches in existing item {0}", (object) str));
          }
          if (item.BranchesInfo != null)
          {
            string str = string.Join(", ", (item.BranchesInfo.Count > 5 ? (IEnumerable<BranchInfo>) item.BranchesInfo.Take<BranchInfo>(5).ToList<BranchInfo>() : (IEnumerable<BranchInfo>) item.BranchesInfo).Select<BranchInfo, string>((Func<BranchInfo, string>) (x => x.BranchName)));
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(BasicMetaDataStoreWriter<T, TEncoder>.s_traceMetadata, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Branches in New item {0}", (object) str));
          }
          message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Duplicate Entry being added to MetadataStore: {0}, Existing Item Values: DocumentId: {1}, Path: {2}, UpdateType: {3}, ContentId: {4} New Item Values: DocumentId: {5}, Path: {6}, UpdateType: {7}, ContentId: {8}", (object) key, (object) obj.DocumentId, (object) obj.Path, (object) obj.UpdateType, (object) obj.ContentId, (object) item.DocumentId, (object) item.Path, (object) item.UpdateType, (object) item.ContentId);
        }
        else
          message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Existing Item is null, New Item Value: DocumentId: {0}, Path: {1}, UpdateType: {2}, ContentId: {3}", (object) item.DocumentId, (object) item.Path, (object) item.UpdateType, (object) item.ContentId);
        if (throwOnDuplicates)
          throw new ArgumentException(message);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(BasicMetaDataStoreWriter<T, TEncoder>.s_traceMetadata, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", (object) message));
      }
      this.m_fileDictionary[key] = item;
    }

    public int CompleteAdding()
    {
      if (!this.m_completeAdding)
      {
        this.m_completeAdding = true;
        if (this.m_path != null)
          return this.SaveTo(this.m_path, (IMetaDataStoreEncoder<T>) new TEncoder());
      }
      return 0;
    }

    internal int SaveTo(string path, IMetaDataStoreEncoder<T> encoder) => this.SaveTo(this.m_fileStore.GetDirectory(Path.GetDirectoryName(path) ?? "\\", true).GetFileWriter(Path.GetFileName(path)), encoder);

    internal int SaveTo(IFileWriter writer, IMetaDataStoreEncoder<T> encoder)
    {
      using (writer)
      {
        writer.WriteString(typeof (T).Name);
        writer.WriteString(typeof (TEncoder).Name);
        this.WritePropertyBag(writer);
        this.WriteContentKeys(writer);
        return this.WriteData(writer, encoder);
      }
    }

    private int WriteData(IFileWriter writer, IMetaDataStoreEncoder<T> encoder)
    {
      encoder.Initialize();
      foreach (T obj in (IEnumerable<T>) this.m_fileDictionary.Values)
        encoder.Encode(writer, obj);
      return this.m_fileDictionary.Values.Count;
    }

    private void WritePropertyBag(IFileWriter writer)
    {
      writer.Write(this.m_propertyBag.Keys.Count);
      foreach (string key in this.m_propertyBag.Keys)
      {
        writer.WriteString(key);
        writer.WriteString(this.m_propertyBag[key]);
      }
    }

    internal void WriteContentKeys(IFileWriter writer)
    {
      writer.Write(this.m_contentKeys == null ? 0 : this.m_contentKeys.Count);
      if (this.m_contentKeys == null)
        return;
      foreach (string contentKey in (IEnumerable<string>) this.m_contentKeys)
        writer.WriteString(contentKey);
    }

    public IEnumerator<T> GetEnumerator() => throw new NotSupportedException();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
  }
}
