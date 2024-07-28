// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.ReSearch.Core.Core.Stores.BackingStoreCollections.LocalStorageEndpointCollection
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.StorageEndpoint;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore;
using Microsoft.VisualStudio.Services.Search.ReSearch.Core.Stores.StorageEndpoint.Extensions;
using Microsoft.VisualStudio.Services.Search.ReSearch.Core.Stores.TreeStore;
using Microsoft.VisualStudio.Services.Search.Server.Storage.FileSystem.Definitions;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;

namespace Microsoft.VisualStudio.Services.Search.ReSearch.Core.Core.Stores.BackingStoreCollections
{
  [Export(typeof (IStorageEndpointCollection))]
  public class LocalStorageEndpointCollection : IStorageEndpointCollection, IDisposable
  {
    private string m_workingDirectoryName;
    private IDirectory m_workingDirectory;
    private bool m_storeShouldExist;
    public const string CrawlStoreRootDir = "CrawlStore";
    public const string MetaDataStoreRootDir = "TreeStore";
    public const string ParseStoreRootDir = "ParseStore";
    private bool m_disposedValue;

    protected IFileSystem FileSystem { get; set; }

    protected IStorageEndpoint m_crawlStore { get; set; }

    protected IStorageEndpoint m_parseStore { get; set; }

    protected IMetaDataStoreProvider m_metaDataStoreProvider { get; set; }

    public virtual IStorageEndpoint CrawlStore
    {
      get
      {
        if (this.m_crawlStore == null)
          this.m_crawlStore = (IStorageEndpoint) new LooseFileStorageEndpoint(Path.Combine(this.WorkingDirectory.FullPath, nameof (CrawlStore)), this.FileSystem);
        return this.m_crawlStore;
      }
    }

    public virtual IStorageEndpoint ParseStore
    {
      get
      {
        if (this.m_parseStore == null)
          this.m_parseStore = (IStorageEndpoint) new LooseFileStorageEndpoint(Path.Combine(this.WorkingDirectory.FullPath, nameof (ParseStore)), this.FileSystem);
        return this.m_parseStore;
      }
    }

    public IMetaDataStoreProvider MetaDataStoreProvider
    {
      get
      {
        if (this.m_metaDataStoreProvider == null)
          this.m_metaDataStoreProvider = (IMetaDataStoreProvider) new FileSystemMetaDataStoreProvider(Path.Combine(this.WorkingDirectory.FullPath, "TreeStore"), this.FileSystem);
        return this.m_metaDataStoreProvider;
      }
    }

    public IDirectory WorkingDirectory
    {
      get
      {
        if (this.m_workingDirectory == null)
          this.m_workingDirectory = this.GetWorkingDirectory();
        return this.m_workingDirectory;
      }
    }

    public virtual IList<IEntityType> SupportedEntityTypes => (IList<IEntityType>) new List<IEntityType>()
    {
      (IEntityType) CodeEntityType.GetInstance(),
      (IEntityType) FileEntityType.GetInstance(),
      (IEntityType) PackageEntityType.GetInstance(),
      (IEntityType) ProjectRepoEntityType.GetInstance(),
      (IEntityType) TenantCodeEntityType.GetInstance(),
      (IEntityType) TenantWikiEntityType.GetInstance(),
      (IEntityType) WikiEntityType.GetInstance(),
      (IEntityType) BoardEntityType.GetInstance(),
      (IEntityType) SettingEntityType.GetInstance()
    };

    public LocalStorageEndpointCollection()
    {
    }

    public LocalStorageEndpointCollection(string workingDirectoryName)
    {
      this.FileSystem = (IFileSystem) Microsoft.VisualStudio.Services.Search.Server.Storage.FileSystem.Local.FileSystem.Instance;
      this.m_workingDirectoryName = workingDirectoryName;
      this.m_storeShouldExist = false;
    }

    public LocalStorageEndpointCollection(string workingDirectoryName, bool storeShouldExist)
    {
      this.FileSystem = (IFileSystem) Microsoft.VisualStudio.Services.Search.Server.Storage.FileSystem.Local.FileSystem.Instance;
      this.m_workingDirectoryName = workingDirectoryName;
      this.m_storeShouldExist = storeShouldExist;
    }

    public LocalStorageEndpointCollection(
      string workingDirectoryName,
      bool storeShouldExist,
      IFileSystem fileSystem)
    {
      this.FileSystem = fileSystem;
      this.m_workingDirectoryName = workingDirectoryName;
      this.m_storeShouldExist = storeShouldExist;
    }

    protected virtual IDirectory GetWorkingDirectory() => !this.m_storeShouldExist ? this.GetWorkingDirectory(new Action<string>(this.DeleteDirectoryWithRetry)) : this.GetExistingWorkingDirectory();

    private IDirectory GetExistingWorkingDirectory()
    {
      IDirectory temporaryDirectory = this.FileSystem.GetTemporaryDirectory(this.m_workingDirectoryName, false);
      return temporaryDirectory != null ? temporaryDirectory : throw new DirectoryNotFoundException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The directory {0} does not exist.", (object) temporaryDirectory));
    }

    private IDirectory GetWorkingDirectory(Action<string> directoryDeleteActionPerformer)
    {
      IDirectory temporaryDirectory = this.FileSystem.GetTemporaryDirectory(this.m_workingDirectoryName, false);
      if (temporaryDirectory != null)
        directoryDeleteActionPerformer(temporaryDirectory.FullPath);
      if (temporaryDirectory != null && this.FileSystem.DirectoryExists(temporaryDirectory.FullPath))
      {
        string workingDirectoryName = this.m_workingDirectoryName;
        this.m_workingDirectoryName = this.m_workingDirectoryName + "_" + Guid.NewGuid().ToString().Substring(0, 8);
        Tracer.TraceWarning(1080262, "Indexing Pipeline", "Repository Indexer Job", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unable to delete existing RepositoryIndexerJob working directory '{0}' Creating a new dir '{1}'", (object) workingDirectoryName, (object) this.m_workingDirectoryName));
      }
      return this.FileSystem.GetTemporaryDirectory(this.m_workingDirectoryName, true);
    }

    public void DeleteDirectoryWithRetry(string dirPath)
    {
      try
      {
        this.FileSystem.DeleteDirectory(dirPath);
      }
      catch (Exception ex1)
      {
        DirectoryNotFoundException notFoundException = ex1 as DirectoryNotFoundException;
        if (ex1 is UnauthorizedAccessException)
        {
          try
          {
            this.FileSystem.CleanDirectoryAttributes(dirPath);
            this.FileSystem.DeleteDirectory(dirPath);
          }
          catch (Exception ex2)
          {
            Tracer.TraceWarning(1080262, "Indexing Pipeline", "Repository Indexer Job", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DeleteDirectoryWithRetry : Failed to delete directory even after retry. ex = {0}", (object) ex2.ToString()));
          }
        }
        else
          Tracer.TraceWarning(1080262, "Indexing Pipeline", "Repository Indexer Job", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DeleteDirectoryWithRetry : Failed to delete directory. ex = {0}", (object) ex1.ToString()));
      }
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.m_disposedValue)
        return;
      if (disposing && !this.m_storeShouldExist && this.m_workingDirectory?.FullPath != null)
        this.DeleteDirectoryWithRetry(this.m_workingDirectory.FullPath);
      this.m_disposedValue = true;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }
  }
}
