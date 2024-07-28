// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.UploadFileUtils
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Azure;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Microsoft.VisualStudio.Services.BlobStore
{
  public static class UploadFileUtils
  {
    private static readonly ReadOnlySet<string> whiteListedContainers = new ReadOnlySet<string>((ISet<string>) new HashSet<string>()
    {
      "ArtifactTool".ToLower(),
      "ArtifactTool-test".ToLower(),
      "Drop".ToLower(),
      "Drop-test".ToLower(),
      "Symbol".ToLower(),
      "Symbol-test".ToLower()
    });

    public static CloudBlobContainer GetCloudStorageContainer(
      IVssRequestContext requestContext,
      string containerName,
      int storageAccountIndex,
      bool bypassWhitelist)
    {
      if (containerName.StartsWith(BlobStoreProviderConstants.BlobContainerPrefix) || containerName.StartsWith(BlobStoreProviderConstants.DedupContainerPrefix))
        throw new ArgumentException("Container name starts with the blob/dedup container prefix. This is disallowed to avoid placing data into the service's shard containers.");
      if (!bypassWhitelist && !UploadFileUtils.whiteListedContainers.Contains(containerName))
        throw new ArgumentException("Cannot use container " + containerName + " as it is not whitelisted. You may be able to bypass the whitelist, but this is not recommended.");
      string str = Guid.Empty.ToString("N");
      if (containerName.Length >= str.Length && Guid.TryParse(containerName.Substring(containerName.Length - str.Length), out Guid _))
        throw new ArgumentException("Container name ends with a GUID format. This is disallowed to avoid placing data into the service's shard containers.");
      List<StrongBoxConnectionString> list = StorageAccountConfigurationFacade.ReadAllStorageAccounts(requestContext).ToList<StrongBoxConnectionString>();
      if (storageAccountIndex >= list.Count)
        throw new ArgumentException("Storage account index exceeds the total number of storage accounts for this deployment.");
      StrongBoxConnectionString connectionString = list[storageAccountIndex];
      if (connectionString == (StrongBoxConnectionString) null)
        throw new InvalidOperationException("Account is missing storage accounts.");
      CloudBlobContainer containerReference = CloudStorageAccount.Parse(connectionString.ConnectionString).CreateCloudBlobClient().GetContainerReference(containerName);
      try
      {
        containerReference.CreateIfNotExists();
      }
      catch (StorageException ex) when (ex.HasHttpStatus(HttpStatusCode.BadRequest))
      {
        throw new ArgumentException("Invalid container name. Check for blob container naming guidelines.");
      }
      return containerReference;
    }
  }
}
