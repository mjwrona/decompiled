// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.ReSearch.Core.Stores.TreeStore.FileSystemMetaDataStoreProvider
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore;
using Microsoft.VisualStudio.Services.Search.Server.Storage.FileSystem.Definitions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;

namespace Microsoft.VisualStudio.Services.Search.ReSearch.Core.Stores.TreeStore
{
  [Export(typeof (IMetaDataStoreProvider))]
  public class FileSystemMetaDataStoreProvider : IMetaDataStoreProvider
  {
    public const string MetaDataStoreFileName = "Crawl.tstore";
    private string m_path;
    private IFileSystem m_fileSystem;

    public FileSystemMetaDataStoreProvider() => this.m_fileSystem = (IFileSystem) Microsoft.VisualStudio.Services.Search.Server.Storage.FileSystem.Local.FileSystem.Instance;

    public FileSystemMetaDataStoreProvider(IFileSystem fileSystem) => this.m_fileSystem = fileSystem;

    public FileSystemMetaDataStoreProvider(string path, IFileSystem fileSystem)
    {
      this.m_path = path;
      this.m_fileSystem = fileSystem;
    }

    public IMetaDataStore GetMetaDataStore() => BasicMetaDataStoreReader.Create(Path.Combine(this.m_path, "Crawl.tstore"), this.m_fileSystem);

    public IEnumerable<IMetaDataStore> GetMetaDataStores()
    {
      IDirectory directory = this.m_fileSystem.GetDirectory(this.m_path, false);
      if (directory == null)
        throw new Exception(string.Format("Failed to get metaDataStoreDirectory from FileMetadataStore for fileSystem: {0} and path: {1}", (object) this.m_fileSystem, (object) this.m_path));
      foreach (string enumerateFile in directory.EnumerateFiles("*.tstore"))
        yield return BasicMetaDataStoreReader.Create(enumerateFile, this.m_fileSystem);
    }

    public IMetaDataStore<T> GetMetaDataStoreWriter<T, TEncoder>(StringComparer stringComparer)
      where T : IMetaDataStoreItem, new()
      where TEncoder : IMetaDataStoreEncoder<T>, new()
    {
      return (IMetaDataStore<T>) new BasicMetaDataStoreWriter<T, TEncoder>(Path.Combine(this.m_path, "Crawl.tstore"), this.m_fileSystem, stringComparer);
    }
  }
}
