// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.StorageContext
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.StorageEndpoint;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public class StorageContext : IDisposable
  {
    private readonly IEntityType m_entityType = (IEntityType) AllEntityType.GetInstance();
    public const int WorkingDirBranchNameHashLen = 2;
    private bool m_disposeStorageEndpointCollection;
    private bool m_disposedValue;

    protected string IndexWorkerDirectoryName { get; private set; }

    public IStorageEndpointCollection StorageEndpointCollection { get; private set; }

    public virtual bool BackingStoreShouldExist => false;

    protected internal StorageContext()
    {
    }

    protected StorageContext(
      string workingDirectory,
      Func<string, IStorageEndpointCollection> storageEndpointCollectionFactory)
    {
      if (storageEndpointCollectionFactory == null)
        throw new ArgumentNullException(nameof (storageEndpointCollectionFactory));
      this.IndexWorkerDirectoryName = workingDirectory;
      this.Settings = (StoreSettings) null;
      this.StorageEndpointCollection = storageEndpointCollectionFactory(workingDirectory);
      this.m_disposeStorageEndpointCollection = false;
    }

    protected internal StorageContext(
      IStorageEndpointCollection storageEndpointCollection)
    {
      this.StorageEndpointCollection = storageEndpointCollection;
      this.m_disposeStorageEndpointCollection = false;
    }

    protected void CopyFrom(StorageContext storageContext)
    {
      this.IndexWorkerDirectoryName = storageContext != null ? storageContext.IndexWorkerDirectoryName : throw new ArgumentNullException(nameof (storageContext));
      this.Settings = storageContext.Settings;
      this.StorageEndpointCollection = storageContext.StorageEndpointCollection;
      this.m_disposeStorageEndpointCollection = false;
    }

    public StorageContext(
      IVssRequestContext requestContext,
      Guid uniqueIdentifierOfEntity,
      IEntityType entityType)
    {
      this.m_entityType = entityType;
      this.IndexWorkerDirectoryName = this.m_entityType?.ToString() + "_" + uniqueIdentifierOfEntity.ToString().Substring(0, 8);
      this.Settings = new StoreSettings(requestContext);
      this.StorageEndpointCollection = requestContext.GetService<IStorageEndpointCollectionService>().CreateBackingStoreCollection(requestContext, entityType, this.IndexWorkerDirectoryName);
      this.m_disposeStorageEndpointCollection = true;
    }

    [SuppressMessage("Microsoft.Cryptographic.Standard", "CA5350:MD5CannotBeUsed", Target = "md5", Justification = "MD5 hashing is not used for security purposes here")]
    public StorageContext(
      IVssRequestContext requestContext,
      Guid uniqueIdentifierOfEntity,
      string branchName,
      IEntityType entityType,
      bool isShadowIndexingUnit)
    {
      this.m_entityType = entityType;
      string str1 = string.Empty;
      if (!string.IsNullOrEmpty(branchName))
      {
        using (MD5 md5 = MD5.Create())
          str1 = StorageContext.GetMD5Hash(md5, branchName, 2);
      }
      string str2 = isShadowIndexingUnit ? "s" : "";
      this.IndexWorkerDirectoryName = this.m_entityType?.ToString() + str2 + "_" + uniqueIdentifierOfEntity.ToString().Substring(0, 8) + str1;
      this.Settings = new StoreSettings(requestContext);
      this.StorageEndpointCollection = requestContext.GetService<IStorageEndpointCollectionService>().CreateBackingStoreCollection(requestContext, entityType, this.IndexWorkerDirectoryName);
      this.m_disposeStorageEndpointCollection = true;
    }

    public virtual StoreSettings Settings { get; private set; }

    private static string GetMD5Hash(MD5 md5, string input, int hashLen)
    {
      if (md5 == null)
        throw new ArgumentNullException(nameof (md5));
      if (input == null)
        throw new ArgumentNullException(nameof (input));
      byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
      StringBuilder stringBuilder = new StringBuilder();
      int num = input.Length < hashLen ? input.Length : hashLen;
      for (int index = 0; index < num; ++index)
        stringBuilder.Append(hash[index].ToString("x2", (IFormatProvider) CultureInfo.InvariantCulture));
      return stringBuilder.ToString();
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.m_disposedValue)
        return;
      if (disposing && this.m_disposeStorageEndpointCollection && this.StorageEndpointCollection != null)
      {
        this.StorageEndpointCollection.Dispose();
        this.StorageEndpointCollection = (IStorageEndpointCollection) null;
      }
      this.m_disposedValue = true;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }
  }
}
