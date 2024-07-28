// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.CompositeAzureBlobProvider
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.Storage;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class CompositeAzureBlobProvider : IBlobProvider
  {
    private IBlobProvider m_primary;
    private ISecondaryBlobProviderCache m_secondary;
    private const string c_area = "AzureBlobProvider";
    private const string c_layer = "CompositeProvider";

    public CompositeAzureBlobProvider(IBlobProvider primary, ISecondaryBlobProviderCache secondary)
    {
      ArgumentUtility.CheckForNull<IBlobProvider>(primary, nameof (primary));
      ArgumentUtility.CheckForNull<ISecondaryBlobProviderCache>(secondary, nameof (secondary));
      this.m_primary = primary;
      this.m_secondary = secondary;
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public void ServiceStart(IVssRequestContext requestContext)
    {
    }

    public void ServiceStart(
      IVssRequestContext requestContext,
      IDictionary<string, string> settings)
    {
    }

    public RemoteStoreId RemoteStoreId => this.m_primary.RemoteStoreId;

    public bool BlobExists(
      IVssRequestContext requestContext,
      string containerId,
      string resourceId,
      TimeSpan? clientTimeout = null)
    {
      if (this.m_primary.BlobExists(requestContext, containerId, resourceId, clientTimeout))
        return true;
      IBlobProvider provider;
      return this.m_secondary.TryGetProvider(requestContext, containerId, out provider) && provider.BlobExists(requestContext, containerId, resourceId, clientTimeout);
    }

    public bool BlobExists(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      TimeSpan? clientTimeout = null)
    {
      return this.BlobExists(requestContext, containerId.ToString("N"), resourceId, clientTimeout);
    }

    public bool ContainerExists(
      IVssRequestContext requestContext,
      Guid containerId,
      TimeSpan? clientTimeout = null)
    {
      if (this.m_primary.ContainerExists(requestContext, containerId, clientTimeout))
        return true;
      IBlobProvider provider;
      return this.m_secondary.TryGetProvider(requestContext, containerId, out provider) && provider.ContainerExists(requestContext, containerId, clientTimeout);
    }

    public bool DeleteBlob(
      IVssRequestContext requestContext,
      string containerId,
      string resourceId,
      TimeSpan? clientTimeout = null)
    {
      bool flag = false;
      IBlobProvider provider;
      if (this.m_secondary.TryGetProvider(requestContext, containerId, out provider))
        flag = provider.DeleteBlob(requestContext, containerId, resourceId, clientTimeout);
      return flag | this.m_primary.DeleteBlob(requestContext, containerId, resourceId, clientTimeout);
    }

    public bool DeleteBlob(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      TimeSpan? clientTimeout = null)
    {
      return this.DeleteBlob(requestContext, containerId.ToString("N"), resourceId, clientTimeout);
    }

    public List<string> DeleteBlobs(
      IVssRequestContext requestContext,
      Guid containerId,
      List<string> resourceIds,
      TimeSpan? clientTimeout = null)
    {
      HashSet<string> source = new HashSet<string>();
      IBlobProvider provider;
      if (this.m_secondary.TryGetProvider(requestContext, containerId, out provider))
        source.UnionWith((IEnumerable<string>) provider.DeleteBlobs(requestContext, containerId, resourceIds, clientTimeout));
      source.UnionWith((IEnumerable<string>) this.m_primary.DeleteBlobs(requestContext, containerId, resourceIds, clientTimeout));
      return source.ToList<string>();
    }

    public List<Guid> DeleteBlobs(
      IVssRequestContext requestContext,
      Guid containerId,
      List<Guid> resourceIds,
      TimeSpan? clientTimeout = null)
    {
      HashSet<Guid> source = new HashSet<Guid>();
      IBlobProvider provider;
      if (this.m_secondary.TryGetProvider(requestContext, containerId, out provider))
        source.UnionWith((IEnumerable<Guid>) provider.DeleteBlobs(requestContext, containerId, resourceIds, clientTimeout));
      source.UnionWith((IEnumerable<Guid>) this.m_primary.DeleteBlobs(requestContext, containerId, resourceIds, clientTimeout));
      return source.ToList<Guid>();
    }

    public void DeleteContainer(
      IVssRequestContext requestContext,
      Guid containerId,
      TimeSpan? clientTimeout = null)
    {
      IBlobProvider provider;
      if (this.m_secondary.TryGetProvider(requestContext, containerId, out provider))
        provider.DeleteContainer(requestContext, containerId, clientTimeout);
      this.m_primary.DeleteContainer(requestContext, containerId, clientTimeout);
    }

    public void DownloadToStream(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      Stream targetStream,
      TimeSpan? clientTimeout = null)
    {
      long initialLength = targetStream.Length;
      this.HandleReadThrough<bool>(requestContext, (CompositeAzureBlobProvider.QueryDelegate<bool>) ((rc, provider) =>
      {
        provider.DownloadToStream(rc, containerId, resourceId, targetStream, clientTimeout);
        return targetStream.Length > initialLength;
      }), (CompositeAzureBlobProvider.ValidateDelegate<bool>) ((rc, result) => result), this.m_primary, this.m_secondary, containerId, (CompositeAzureBlobProvider.LogDelegate) ((rc, msg) => rc.TraceAlways(14610, TraceLevel.Info, "AzureBlobProvider", "CompositeProvider", "Blob {0}/{1}: {2}", (object) containerId, (object) resourceId, (object) msg)), nameof (DownloadToStream));
    }

    public void DownloadToStreamLargeBlocks(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      Stream targetStream,
      TimeSpan? clientTimeout = null)
    {
      long initialLength = targetStream.Length;
      this.HandleReadThrough<bool>(requestContext, (CompositeAzureBlobProvider.QueryDelegate<bool>) ((rc, provider) =>
      {
        provider.DownloadToStreamLargeBlocks(rc, containerId, resourceId, targetStream, clientTimeout);
        return targetStream.Length > initialLength;
      }), (CompositeAzureBlobProvider.ValidateDelegate<bool>) ((rc, result) => result), this.m_primary, this.m_secondary, containerId, (CompositeAzureBlobProvider.LogDelegate) ((rc, msg) => rc.TraceAlways(14611, TraceLevel.Info, "AzureBlobProvider", "CompositeProvider", "Blob {0}/{1}: {2}", (object) containerId, (object) resourceId, (object) msg)), nameof (DownloadToStreamLargeBlocks));
    }

    public IEnumerable<string> EnumerateBlobs(
      IVssRequestContext requestContext,
      Guid containerId,
      TimeSpan? clientTimeout = null)
    {
      IEnumerable<string> first = this.m_primary.EnumerateBlobs(requestContext, containerId, clientTimeout);
      IEnumerable<string> second = (IEnumerable<string>) null;
      IBlobProvider provider;
      if (this.m_secondary.TryGetProvider(requestContext, containerId, out provider))
      {
        try
        {
          second = provider.EnumerateBlobs(requestContext, containerId, clientTimeout);
        }
        catch (StorageException ex) when (BlobCopyUtil.HasHttpStatusCode((Exception) ex, HttpStatusCode.NotFound))
        {
          requestContext.TraceAlways(14616, TraceLevel.Info, "AzureBlobProvider", "CompositeProvider", "No secondary container exists.");
        }
      }
      return second == null ? first : first.MergeDistinct<string>(second, (IComparer<string>) StringComparer.Ordinal);
    }

    public Stream GetStream(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      TimeSpan? clientTimeout = null)
    {
      return this.HandleReadThrough<Stream>(requestContext, (CompositeAzureBlobProvider.QueryDelegate<Stream>) ((rc, provider) => provider.GetStream(rc, containerId, resourceId, clientTimeout)), (CompositeAzureBlobProvider.ValidateDelegate<Stream>) ((rc, result) => result.Length > 0L), this.m_primary, this.m_secondary, containerId, (CompositeAzureBlobProvider.LogDelegate) ((rc, msg) => rc.TraceAlways(14612, TraceLevel.Info, "AzureBlobProvider", "CompositeProvider", "Blob {0}/{1}: {2}", (object) containerId, (object) resourceId, (object) msg)), nameof (GetStream));
    }

    public void PutChunk(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      byte[] contentBlock,
      int contentBlockLength,
      long compressedLength,
      long offset,
      bool isLastChunk,
      IDictionary<string, string> blobMetadata = null,
      TimeSpan? clientTimeout = null)
    {
      this.m_primary.PutChunk(requestContext, containerId, resourceId, contentBlock, contentBlockLength, compressedLength, offset, isLastChunk, blobMetadata, clientTimeout);
    }

    public void PutStream(
      IVssRequestContext requestContext,
      string containerName,
      string resourceId,
      Stream stream,
      IDictionary<string, string> metadata,
      TimeSpan? clientTimeout = null)
    {
      this.m_primary.PutStream(requestContext, containerName, resourceId, stream, metadata, clientTimeout);
    }

    public void PutStream(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      Stream stream,
      IDictionary<string, string> metadata,
      TimeSpan? clientTimeout = null)
    {
      this.m_primary.PutStream(requestContext, containerId, resourceId, stream, metadata, clientTimeout);
    }

    public IDictionary<string, string> ReadBlobMetadata(
      IVssRequestContext requestContext,
      string containerId,
      string resourceId,
      TimeSpan? clientTimeout = null)
    {
      return this.HandleReadThrough<IDictionary<string, string>>(requestContext, (CompositeAzureBlobProvider.QueryDelegate<IDictionary<string, string>>) ((rc, provider) => provider.ReadBlobMetadata(rc, containerId, resourceId, clientTimeout)), (CompositeAzureBlobProvider.ValidateDelegate<IDictionary<string, string>>) null, this.m_primary, this.m_secondary, Guid.Parse(containerId), (CompositeAzureBlobProvider.LogDelegate) ((rc, msg) => rc.TraceAlways(14613, TraceLevel.Info, "AzureBlobProvider", "CompositeProvider", "Blob {0}/{1}: {2}", (object) containerId, (object) resourceId, (object) msg)), nameof (ReadBlobMetadata));
    }

    public IDictionary<string, string> ReadBlobMetadata(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      TimeSpan? clientTimeout = null)
    {
      return this.HandleReadThrough<IDictionary<string, string>>(requestContext, (CompositeAzureBlobProvider.QueryDelegate<IDictionary<string, string>>) ((rc, provider) => provider.ReadBlobMetadata(rc, containerId, resourceId, clientTimeout)), (CompositeAzureBlobProvider.ValidateDelegate<IDictionary<string, string>>) null, this.m_primary, this.m_secondary, containerId, (CompositeAzureBlobProvider.LogDelegate) ((rc, msg) => rc.TraceAlways(14614, TraceLevel.Info, "AzureBlobProvider", "CompositeProvider", "Blob {0}/{1}: {2}", (object) containerId, (object) resourceId, (object) msg)), nameof (ReadBlobMetadata));
    }

    public BlobProperties ReadBlobProperties(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      TimeSpan? clientTimeout = null)
    {
      return this.HandleReadThrough<BlobProperties>(requestContext, (CompositeAzureBlobProvider.QueryDelegate<BlobProperties>) ((rc, provider) => provider.ReadBlobProperties(rc, containerId, resourceId, clientTimeout)), (CompositeAzureBlobProvider.ValidateDelegate<BlobProperties>) null, this.m_primary, this.m_secondary, containerId, (CompositeAzureBlobProvider.LogDelegate) ((rc, msg) => rc.TraceAlways(14615, TraceLevel.Info, "AzureBlobProvider", "CompositeProvider", "Blob {0}/{1}: {2}", (object) containerId, (object) resourceId, (object) msg)), nameof (ReadBlobProperties));
    }

    public void RenameBlob(
      IVssRequestContext requestContext,
      string containerId,
      string sourceResourceId,
      string targetResourceId,
      TimeSpan? clientTimeout = null)
    {
      this.RenameBlob(requestContext, new Guid(containerId), sourceResourceId, targetResourceId, clientTimeout);
    }

    public void RenameBlob(
      IVssRequestContext requestContext,
      Guid containerId,
      string sourceResourceId,
      string targetResourceId,
      TimeSpan? clientTimeout = null)
    {
      IBlobProvider provider = (IBlobProvider) null;
      this.m_secondary.TryGetProvider(requestContext, containerId, out provider);
      bool? nullable = new bool?();
      try
      {
        if (this.m_primary.ReadBlobProperties(requestContext, containerId, sourceResourceId, clientTimeout).Length == 0L)
        {
          nullable = new bool?(provider != null && provider.BlobExists(requestContext, containerId, sourceResourceId, clientTimeout));
          if (nullable.GetValueOrDefault())
            throw new NotSupportedException("RenameBlob not supported in CompositeAzureBlobProvider.  Blob is pending copy.");
        }
      }
      catch (StorageException ex) when (BlobCopyUtil.HasHttpStatusCode((Exception) ex, HttpStatusCode.NotFound))
      {
        if (provider != null && provider.BlobExists(requestContext, containerId, sourceResourceId, clientTimeout))
          throw new NotSupportedException("RenameBlob not supported in CompositeAzureBlobProvider.  Blob exists only on secondary.");
        throw;
      }
      if (requestContext.GetService<ITeamFoundationFeatureAvailabilityService>().IsFeatureEnabled(requestContext, FrameworkServerConstants.EnableCompositeBlobProviderRenameOnSecondaryFF))
      {
        try
        {
          if (((int) nullable ?? (provider == null ? 0 : (provider.BlobExists(requestContext, containerId, sourceResourceId, clientTimeout) ? 1 : 0))) != 0)
            provider.DeleteBlob(requestContext, containerId, sourceResourceId, clientTimeout);
        }
        catch (StorageException ex) when (!BlobCopyUtil.HasHttpStatusCode((Exception) ex, HttpStatusCode.ServiceUnavailable))
        {
          requestContext.TraceAlways(14618, TraceLevel.Info, "AzureBlobProvider", "CompositeProvider", "Secondary Blob could not be deleted {0}/{1}: {2}", (object) containerId, (object) sourceResourceId, (object) ex.Message);
        }
      }
      this.m_primary.RenameBlob(requestContext, containerId, sourceResourceId, targetResourceId, clientTimeout);
    }

    public void SetBlobHeaders(
      IVssRequestContext requestContext,
      string containerId,
      string resourceId,
      string cacheControl,
      string contentType,
      string contentDisposition,
      string contentEncoding,
      string contentLanguage,
      TimeSpan? clientTimeout = null)
    {
      this.m_primary.SetBlobHeaders(requestContext, containerId, resourceId, cacheControl, contentType, contentDisposition, contentEncoding, contentLanguage, clientTimeout);
    }

    public void SetBlobHeaders(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      string cacheControl,
      string contentType,
      string contentDisposition,
      string contentEncoding,
      string contentLanguage,
      TimeSpan? clientTimeout = null)
    {
      this.m_primary.SetBlobHeaders(requestContext, containerId, resourceId, cacheControl, contentType, contentDisposition, contentEncoding, contentLanguage, clientTimeout);
    }

    public void WriteBlobMetadata(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      IDictionary<string, string> metadata,
      TimeSpan? clientTimeout = null)
    {
      this.m_primary.WriteBlobMetadata(requestContext, containerId, resourceId, metadata, clientTimeout);
    }

    public void WriteBlobTags(
      IVssRequestContext requestContext,
      string containerId,
      string resourceId,
      IDictionary<string, string> tags,
      TimeSpan? clientTimeout = null)
    {
      this.m_primary.WriteBlobTags(requestContext, containerId, resourceId, tags, clientTimeout);
    }

    public IDictionary<string, string> ReadBlobTags(
      IVssRequestContext requestContext,
      string containerId,
      string resourceId)
    {
      return this.HandleReadThrough<IDictionary<string, string>>(requestContext, (CompositeAzureBlobProvider.QueryDelegate<IDictionary<string, string>>) ((rc, provider) => provider.ReadBlobTags(rc, containerId, resourceId)), (CompositeAzureBlobProvider.ValidateDelegate<IDictionary<string, string>>) null, this.m_primary, this.m_secondary, Guid.Parse(containerId), (CompositeAzureBlobProvider.LogDelegate) ((rc, msg) => rc.TraceAlways(14617, TraceLevel.Info, "AzureBlobProvider", "CompositeProvider", "Blob {0}/{1}: {2}", (object) containerId, (object) resourceId, (object) msg)), nameof (ReadBlobTags));
    }

    private R HandleReadThrough<R>(
      IVssRequestContext requestContext,
      CompositeAzureBlobProvider.QueryDelegate<R> queryResult,
      CompositeAzureBlobProvider.ValidateDelegate<R> validateResult,
      IBlobProvider primaryProvider,
      ISecondaryBlobProviderCache secondaryProviders,
      Guid containerId,
      CompositeAzureBlobProvider.LogDelegate log,
      [CallerMemberName] string caller = null)
    {
      R r = default (R);
      R result;
      try
      {
        result = queryResult(requestContext, this.m_primary);
      }
      catch (StorageException ex) when (BlobCopyUtil.HasHttpStatusCode((Exception) ex, HttpStatusCode.NotFound))
      {
        R secondaryResult;
        if (this.HandleReadThrough_TryCallSecondary<R>(requestContext, queryResult, secondaryProviders, containerId, log, caller, "Blob not found in primary provider", out secondaryResult))
          return secondaryResult;
        throw;
      }
      try
      {
        if (validateResult != null)
        {
          if (!validateResult(requestContext, result))
          {
            R secondaryResult;
            if (this.HandleReadThrough_TryCallSecondary<R>(requestContext, queryResult, secondaryProviders, containerId, log, caller, "Blob appears to be pending copy", out secondaryResult))
            {
              if (result is IDisposable disposable)
                disposable.Dispose();
              return secondaryResult;
            }
          }
        }
      }
      catch (Exception ex)
      {
        if (result is IDisposable disposable)
          disposable.Dispose();
        throw;
      }
      return result;
    }

    private bool HandleReadThrough_TryCallSecondary<R>(
      IVssRequestContext requestContext,
      CompositeAzureBlobProvider.QueryDelegate<R> queryResult,
      ISecondaryBlobProviderCache secondaryProviders,
      Guid containerId,
      CompositeAzureBlobProvider.LogDelegate log,
      string caller,
      string reason,
      out R secondaryResult)
    {
      IBlobProvider provider;
      if (secondaryProviders.TryGetProvider(requestContext, containerId, out provider))
      {
        try
        {
          log(requestContext, "Blob did not exist on primary, attempting to call the secondary. Method: " + caller + ", Reason: " + reason);
          secondaryResult = queryResult(requestContext, provider);
          log(requestContext, "Blob did not exist on primary, but was found on the secondary. Method: " + caller);
          return true;
        }
        catch (StorageException ex) when (BlobCopyUtil.HasHttpStatusCode((Exception) ex, HttpStatusCode.NotFound))
        {
          log(requestContext, "Blob did not exist in the primary or secondary. Method: " + caller);
          secondaryResult = default (R);
          return false;
        }
      }
      else
      {
        secondaryResult = default (R);
        return false;
      }
    }

    private delegate bool ValidateDelegate<R>(IVssRequestContext requestContext, R result);

    private delegate R QueryDelegate<R>(IVssRequestContext requestContext, IBlobProvider provider);

    private delegate void LogDelegate(IVssRequestContext requestContext, string message);
  }
}
