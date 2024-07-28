// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.File.CloudFileShare
// Assembly: Microsoft.Azure.Storage.File, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C68E95B0-8DFB-410C-8E70-706406D1A279
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.File.dll

using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Core;
using Microsoft.Azure.Storage.Core.Auth;
using Microsoft.Azure.Storage.Core.Executor;
using Microsoft.Azure.Storage.Core.Util;
using Microsoft.Azure.Storage.File.Protocol;
using Microsoft.Azure.Storage.Shared.Protocol;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.File
{
  public class CloudFileShare
  {
    [DoesServiceRequest]
    public virtual void Create(FileRequestOptions requestOptions = null, OperationContext operationContext = null)
    {
      this.AssertNoSnapshot();
      if (this.Properties.Quota.HasValue)
        CommonUtility.AssertInBounds<int>("Quota", this.Properties.Quota.Value, 1);
      FileRequestOptions options = FileRequestOptions.ApplyDefaults(requestOptions, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.CreateShareImpl(options), options.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginCreate(AsyncCallback callback, object state) => this.BeginCreate((FileRequestOptions) null, (OperationContext) null, callback, state);

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginCreate(
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.CreateAsync(options, operationContext, token)), callback, state);
    }

    public virtual void EndCreate(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task CreateAsync() => this.CreateAsync(CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task CreateAsync(CancellationToken cancellationToken) => this.CreateAsync((FileRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task CreateAsync(FileRequestOptions options, OperationContext operationContext) => this.CreateAsync(options, operationContext, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task CreateAsync(
      FileRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      this.AssertNoSnapshot();
      if (this.Properties.Quota.HasValue)
        CommonUtility.AssertInBounds<int>("Quota", this.Properties.Quota.Value, 1);
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.CreateShareImpl(options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual bool CreateIfNotExists(
      FileRequestOptions requestOptions = null,
      OperationContext operationContext = null)
    {
      try
      {
        this.Create(requestOptions, operationContext);
        return true;
      }
      catch (StorageException ex)
      {
        if (ex.RequestInformation.HttpStatusCode == 409 && (ex.RequestInformation.ExtendedErrorInformation == null || ex.RequestInformation.ExtendedErrorInformation.ErrorCode == FileErrorCodeStrings.ShareAlreadyExists))
          return false;
        throw;
      }
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginCreateIfNotExists(
      AsyncCallback callback,
      object state)
    {
      return this.BeginCreateIfNotExists((FileRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginCreateIfNotExists(
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<bool>((Func<CancellationToken, Task<bool>>) (token => this.CreateIfNotExistsAsync(options, operationContext, token)), callback, state);
    }

    public virtual bool EndCreateIfNotExists(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<bool>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<bool> CreateIfNotExistsAsync() => this.CreateIfNotExistsAsync(CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<bool> CreateIfNotExistsAsync(CancellationToken cancellationToken) => this.CreateIfNotExistsAsync((FileRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task<bool> CreateIfNotExistsAsync(
      FileRequestOptions options,
      OperationContext operationContext)
    {
      return this.CreateIfNotExistsAsync(options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual async Task<bool> CreateIfNotExistsAsync(
      FileRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      try
      {
        await this.CreateAsync(options, operationContext, cancellationToken).ConfigureAwait(false);
        return true;
      }
      catch (StorageException ex)
      {
        if (ex.RequestInformation.HttpStatusCode == 409 && (ex.RequestInformation.ExtendedErrorInformation == null || ex.RequestInformation.ExtendedErrorInformation.ErrorCode == FileErrorCodeStrings.ShareAlreadyExists))
          return false;
        throw;
      }
    }

    [DoesServiceRequest]
    public CloudFileShare Snapshot(
      IDictionary<string, string> metadata = null,
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.AssertNoSnapshot();
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<CloudFileShare>(this.SnapshotImpl(metadata, accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public ICancellableAsyncResult BeginSnapshot(AsyncCallback callback, object state) => this.BeginSnapshot((IDictionary<string, string>) null, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, callback, state);

    [DoesServiceRequest]
    public ICancellableAsyncResult BeginSnapshot(
      IDictionary<string, string> metadata,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<CloudFileShare>((Func<CancellationToken, Task<CloudFileShare>>) (token => this.SnapshotAsync(metadata, accessCondition, options, operationContext, token)), callback, state);
    }

    public CloudFileShare EndSnapshot(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<CloudFileShare>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public Task<CloudFileShare> SnapshotAsync() => this.SnapshotAsync(CancellationToken.None);

    [DoesServiceRequest]
    public Task<CloudFileShare> SnapshotAsync(CancellationToken cancellationToken) => this.SnapshotAsync((IDictionary<string, string>) null, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public Task<CloudFileShare> SnapshotAsync(
      IDictionary<string, string> metadata,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      return this.SnapshotAsync(metadata, accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public Task<CloudFileShare> SnapshotAsync(
      IDictionary<string, string> metadata,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      this.AssertNoSnapshot();
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<CloudFileShare>(this.SnapshotImpl(metadata, accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual void Delete(
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.Delete(DeleteShareSnapshotsOption.None, accessCondition, options, operationContext);
    }

    [DoesServiceRequest]
    public virtual void Delete(
      DeleteShareSnapshotsOption deleteSnapshotsOption,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.DeleteShareImpl(deleteSnapshotsOption, accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDelete(AsyncCallback callback, object state) => this.BeginDelete(DeleteShareSnapshotsOption.None, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, callback, state);

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDelete(
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return this.BeginDelete(DeleteShareSnapshotsOption.None, accessCondition, options, operationContext, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDelete(
      DeleteShareSnapshotsOption deleteSnapshotsOption,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.DeleteAsync(deleteSnapshotsOption, accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual void EndDelete(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task DeleteAsync() => this.DeleteAsync(CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task DeleteAsync(CancellationToken cancellationToken) => this.DeleteAsync(DeleteShareSnapshotsOption.None, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task DeleteAsync(
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      return this.DeleteAsync(DeleteShareSnapshotsOption.None, accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task DeleteAsync(
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.DeleteAsync(DeleteShareSnapshotsOption.None, accessCondition, options, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task DeleteAsync(
      DeleteShareSnapshotsOption deleteSnapshotsOption,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.DeleteShareImpl(deleteSnapshotsOption, accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual bool DeleteIfExists(
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
      OperationContext operationContext = null)
    {
      return this.DeleteIfExists(DeleteShareSnapshotsOption.None, accessCondition, options, operationContext);
    }

    [DoesServiceRequest]
    public virtual bool DeleteIfExists(
      DeleteShareSnapshotsOption deleteSnapshotsOption,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      FileRequestOptions fileRequestOptions = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      try
      {
        if (!this.Exists(fileRequestOptions, operationContext))
          return false;
      }
      catch (StorageException ex)
      {
        if (ex.RequestInformation.HttpStatusCode != 403)
          throw;
      }
      try
      {
        this.Delete(deleteSnapshotsOption, accessCondition, fileRequestOptions, operationContext);
        return true;
      }
      catch (StorageException ex)
      {
        if (ex.RequestInformation.HttpStatusCode == 404)
        {
          if (ex.RequestInformation.ExtendedErrorInformation == null || ex.RequestInformation.ExtendedErrorInformation.ErrorCode == FileErrorCodeStrings.ShareNotFound)
            return false;
          throw;
        }
        else
          throw;
      }
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDeleteIfExists(AsyncCallback callback, object state) => this.BeginDeleteIfExists(DeleteShareSnapshotsOption.None, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, callback, state);

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDeleteIfExists(
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return this.BeginDeleteIfExists(DeleteShareSnapshotsOption.None, accessCondition, options, operationContext, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDeleteIfExists(
      DeleteShareSnapshotsOption deleteSnapshotsOption,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<bool>((Func<CancellationToken, Task<bool>>) (token => this.DeleteIfExistsAsync(deleteSnapshotsOption, accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual bool EndDeleteIfExists(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<bool>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<bool> DeleteIfExistsAsync() => this.DeleteIfExistsAsync(CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<bool> DeleteIfExistsAsync(CancellationToken cancellationToken) => this.DeleteIfExistsAsync(DeleteShareSnapshotsOption.None, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task<bool> DeleteIfExistsAsync(
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      return this.DeleteIfExistsAsync(DeleteShareSnapshotsOption.None, accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<bool> DeleteIfExistsAsync(
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.DeleteIfExistsAsync(DeleteShareSnapshotsOption.None, accessCondition, options, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual async Task<bool> DeleteIfExistsAsync(
      DeleteShareSnapshotsOption deleteSnapshotsOption,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      FileRequestOptions modifiedOptions = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      try
      {
        if (!await this.ExistsAsync(modifiedOptions, operationContext, cancellationToken).ConfigureAwait(false))
          return false;
      }
      catch (StorageException ex)
      {
        if (ex.RequestInformation.HttpStatusCode != 403)
          throw;
      }
      try
      {
        await this.DeleteAsync(deleteSnapshotsOption, accessCondition, modifiedOptions, operationContext, cancellationToken).ConfigureAwait(false);
        return true;
      }
      catch (StorageException ex)
      {
        if (ex.RequestInformation.HttpStatusCode == 404)
        {
          if (ex.RequestInformation.ExtendedErrorInformation == null || ex.RequestInformation.ExtendedErrorInformation.ErrorCode == FileErrorCodeStrings.ShareNotFound)
            return false;
          throw;
        }
        else
          throw;
      }
    }

    [DoesServiceRequest]
    public virtual bool Exists(FileRequestOptions requestOptions = null, OperationContext operationContext = null)
    {
      FileRequestOptions options = FileRequestOptions.ApplyDefaults(requestOptions, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<bool>(this.ExistsImpl(options), options.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginExists(AsyncCallback callback, object state) => this.BeginExists((FileRequestOptions) null, (OperationContext) null, callback, state);

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginExists(
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<bool>((Func<CancellationToken, Task<bool>>) (token => this.ExistsAsync(options, operationContext, token)), callback, state);
    }

    public virtual bool EndExists(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<bool>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<bool> ExistsAsync() => this.ExistsAsync(CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<bool> ExistsAsync(CancellationToken cancellationToken) => this.ExistsAsync((FileRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task<bool> ExistsAsync(
      FileRequestOptions options,
      OperationContext operationContext)
    {
      return this.ExistsAsync(options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<bool> ExistsAsync(
      FileRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<bool>(this.ExistsImpl(options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual void FetchAttributes(
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
      OperationContext operationContext = null)
    {
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.FetchAttributesImpl(accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginFetchAttributes(
      AsyncCallback callback,
      object state)
    {
      return this.BeginFetchAttributes((AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginFetchAttributes(
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.FetchAttributesAsync(accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual void EndFetchAttributes(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task FetchAttributesAsync() => this.FetchAttributesAsync(CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task FetchAttributesAsync(CancellationToken cancellationToken) => this.FetchAttributesAsync((AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task FetchAttributesAsync(
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      return this.FetchAttributesAsync(accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task FetchAttributesAsync(
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.FetchAttributesImpl(accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual FileSharePermissions GetPermissions(
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.AssertNoSnapshot();
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<FileSharePermissions>(this.GetPermissionsImpl(accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginGetPermissions(AsyncCallback callback, object state) => this.BeginGetPermissions((AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, callback, state);

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginGetPermissions(
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<FileSharePermissions>((Func<CancellationToken, Task<FileSharePermissions>>) (token => this.GetPermissionsAsync(accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual FileSharePermissions EndGetPermissions(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<FileSharePermissions>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<FileSharePermissions> GetPermissionsAsync() => this.GetPermissionsAsync(CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<FileSharePermissions> GetPermissionsAsync(
      CancellationToken cancellationToken)
    {
      return this.GetPermissionsAsync((AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<FileSharePermissions> GetPermissionsAsync(
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      return this.GetPermissionsAsync(accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<FileSharePermissions> GetPermissionsAsync(
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      this.AssertNoSnapshot();
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<FileSharePermissions>(this.GetPermissionsImpl(accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual ShareStats GetStats(
      FileRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.AssertNoSnapshot();
      options = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<ShareStats>(this.GetStatsImpl(options), options.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginGetStats(AsyncCallback callback, object state) => this.BeginGetStats((FileRequestOptions) null, (OperationContext) null, callback, state);

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginGetStats(
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<ShareStats>((Func<CancellationToken, Task<ShareStats>>) (token => this.GetStatsAsync(options, operationContext, token)), callback, state);
    }

    public virtual ShareStats EndGetStats(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<ShareStats>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<ShareStats> GetStatsAsync() => this.GetStatsAsync(CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<ShareStats> GetStatsAsync(CancellationToken cancellationToken) => this.GetStatsAsync((FileRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task<ShareStats> GetStatsAsync(
      FileRequestOptions options,
      OperationContext operationContext)
    {
      return this.GetStatsAsync(options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<ShareStats> GetStatsAsync(
      FileRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      this.AssertNoSnapshot();
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<ShareStats>(this.GetStatsImpl(options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual void SetMetadata(
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.AssertNoSnapshot();
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.SetMetadataImpl(accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginSetMetadata(AsyncCallback callback, object state) => this.BeginSetMetadata((AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, callback, state);

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginSetMetadata(
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.SetMetadataAsync(accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual void EndSetMetadata(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task SetMetadataAsync() => this.SetMetadataAsync(CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task SetMetadataAsync(CancellationToken cancellationToken) => this.SetMetadataAsync((AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task SetMetadataAsync(
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      return this.SetMetadataAsync(accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task SetMetadataAsync(
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      this.AssertNoSnapshot();
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.SetMetadataImpl(accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual void SetPermissions(
      FileSharePermissions permissions,
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.AssertNoSnapshot();
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.SetPermissionsImpl(permissions, accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginSetPermissions(
      FileSharePermissions permissions,
      AsyncCallback callback,
      object state)
    {
      return this.BeginSetPermissions(permissions, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginSetPermissions(
      FileSharePermissions permissions,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.SetPermissionsAsync(permissions, accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual void EndSetPermissions(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task SetPermissionsAsync(FileSharePermissions permissions) => this.SetPermissionsAsync(permissions, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task SetPermissionsAsync(
      FileSharePermissions permissions,
      CancellationToken cancellationToken)
    {
      return this.SetPermissionsAsync(permissions, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task SetPermissionsAsync(
      FileSharePermissions permissions,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      return this.SetPermissionsAsync(permissions, accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task SetPermissionsAsync(
      FileSharePermissions permissions,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      this.AssertNoSnapshot();
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.SetPermissionsImpl(permissions, accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual void SetProperties(
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.AssertNoSnapshot();
      if (this.Properties.Quota.HasValue)
        CommonUtility.AssertInBounds<int>("Quota", this.Properties.Quota.Value, 1);
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.SetPropertiesImpl(accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginSetProperties(AsyncCallback callback, object state) => this.BeginSetProperties((AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, callback, state);

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginSetProperties(
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.SetPropertiesAsync(accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual void EndSetProperties(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task SetPropertiesAsync() => this.SetPropertiesAsync(CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task SetPropertiesAsync(CancellationToken cancellationToken) => this.SetPropertiesAsync((AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task SetPropertiesAsync(
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      return this.SetPropertiesAsync(accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task SetPropertiesAsync(
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      this.AssertNoSnapshot();
      if (this.Properties.Quota.HasValue)
        CommonUtility.AssertInBounds<int>("Quota", this.Properties.Quota.Value, 1);
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.SetPropertiesImpl(accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual string CreateFilePermission(
      string permission,
      FileRequestOptions options = null,
      OperationContext operationContext = null)
    {
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<string>(this.CreateFilePermissionImp(permission, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual Task<string> CreateFilePermissionAsync(
      string permission,
      FileRequestOptions options = null,
      OperationContext operationContext = null,
      CancellationToken? cancellationToken = null)
    {
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<string>(this.CreateFilePermissionImp(permission, options1), options1.RetryPolicy, operationContext, cancellationToken ?? CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual string GetFilePermission(
      string filePermissionKey,
      FileRequestOptions options = null,
      OperationContext operationContext = null)
    {
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<string>(this.GetFilePermissionImp(filePermissionKey, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual Task<string> GetFilePermissionAsync(
      string filePermissionKey,
      FileRequestOptions options = null,
      OperationContext operationContext = null,
      CancellationToken? cancellationToken = null)
    {
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<string>(this.GetFilePermissionImp(filePermissionKey, options1), options1.RetryPolicy, operationContext, cancellationToken ?? CancellationToken.None);
    }

    private RESTCommand<NullType> CreateShareImpl(FileRequestOptions options)
    {
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) =>
      {
        StorageRequestMessage request = ShareHttpRequestMessageFactory.Create(uri, this.Properties, serverTimeout, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials);
        ShareHttpRequestMessageFactory.AddMetadata(request, this.Metadata);
        return request;
      });
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.Created, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex);
        this.Properties = ShareHttpResponseParsers.GetProperties(resp);
        this.Metadata = ShareHttpResponseParsers.GetMetadata(resp);
        return NullType.Value;
      });
      return cmd1;
    }

    internal RESTCommand<CloudFileShare> SnapshotImpl(
      IDictionary<string, string> metadata,
      AccessCondition accessCondition,
      FileRequestOptions options)
    {
      RESTCommand<CloudFileShare> cmd1 = new RESTCommand<CloudFileShare>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<CloudFileShare>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<CloudFileShare>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) =>
      {
        StorageRequestMessage request = ShareHttpRequestMessageFactory.Snapshot(uri, serverTimeout, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials);
        if (metadata != null)
          FileHttpRequestMessageFactory.AddMetadata(request, metadata);
        return request;
      });
      cmd1.PreProcessResponse = (Func<RESTCommand<CloudFileShare>, HttpResponseMessage, Exception, OperationContext, CloudFileShare>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<CloudFileShare>(HttpStatusCode.Created, resp, (CloudFileShare) null, (StorageCommandBase<CloudFileShare>) cmd, ex);
        DateTimeOffset snapshotTime = NavigationHelper.ParseSnapshotTime(ShareHttpResponseParsers.GetSnapshotTime(resp));
        CloudFileShare cloudFileShare = new CloudFileShare(this.Properties, (IDictionary<string, string>) new Dictionary<string, string>(metadata ?? this.Metadata, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase), this.Name, new DateTimeOffset?(snapshotTime), this.ServiceClient);
        this.UpdateETagAndLastModified(resp);
        return cloudFileShare;
      });
      return cmd1;
    }

    private RESTCommand<NullType> DeleteShareImpl(
      DeleteShareSnapshotsOption deleteSnapshotsOption,
      AccessCondition accessCondition,
      FileRequestOptions options)
    {
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => ShareHttpRequestMessageFactory.Delete(uri, serverTimeout, this.SnapshotTime, deleteSnapshotsOption, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.Accepted, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex));
      return cmd1;
    }

    private RESTCommand<NullType> FetchAttributesImpl(
      AccessCondition accessCondition,
      FileRequestOptions options)
    {
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.CommandLocationMode = CommandLocationMode.PrimaryOrSecondary;
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => ShareHttpRequestMessageFactory.GetProperties(uri, serverTimeout, this.SnapshotTime, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.OK, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex);
        this.Properties = ShareHttpResponseParsers.GetProperties(resp);
        this.Metadata = ShareHttpResponseParsers.GetMetadata(resp);
        return NullType.Value;
      });
      return cmd1;
    }

    private RESTCommand<bool> ExistsImpl(FileRequestOptions options)
    {
      RESTCommand<bool> cmd1 = new RESTCommand<bool>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<bool>(cmd1);
      cmd1.CommandLocationMode = CommandLocationMode.PrimaryOrSecondary;
      cmd1.BuildRequest = (Func<RESTCommand<bool>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => ShareHttpRequestMessageFactory.GetProperties(uri, serverTimeout, this.SnapshotTime, (AccessCondition) null, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<bool>, HttpResponseMessage, Exception, OperationContext, bool>) ((cmd, resp, ex, ctx) =>
      {
        if (resp.StatusCode == HttpStatusCode.NotFound)
          return false;
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<bool>(HttpStatusCode.OK, resp, true, (StorageCommandBase<bool>) cmd, ex);
        this.Properties = ShareHttpResponseParsers.GetProperties(resp);
        this.Metadata = ShareHttpResponseParsers.GetMetadata(resp);
        return true;
      });
      return cmd1;
    }

    private RESTCommand<FileSharePermissions> GetPermissionsImpl(
      AccessCondition accessCondition,
      FileRequestOptions options)
    {
      FileSharePermissions shareAcl = (FileSharePermissions) null;
      RESTCommand<FileSharePermissions> cmd1 = new RESTCommand<FileSharePermissions>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<FileSharePermissions>(cmd1);
      cmd1.CommandLocationMode = CommandLocationMode.PrimaryOrSecondary;
      cmd1.RetrieveResponseStream = true;
      cmd1.BuildRequest = (Func<RESTCommand<FileSharePermissions>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => ShareHttpRequestMessageFactory.GetAcl(uri, serverTimeout, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<FileSharePermissions>, HttpResponseMessage, Exception, OperationContext, FileSharePermissions>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<FileSharePermissions>(HttpStatusCode.OK, resp, (FileSharePermissions) null, (StorageCommandBase<FileSharePermissions>) cmd, ex);
        shareAcl = new FileSharePermissions();
        return shareAcl;
      });
      cmd1.PostProcessResponseAsync = (Func<RESTCommand<FileSharePermissions>, HttpResponseMessage, OperationContext, CancellationToken, Task<FileSharePermissions>>) (async (cmd, resp, ctx, ct) =>
      {
        this.UpdateETagAndLastModified(resp);
        await ShareHttpResponseParsers.ReadSharedAccessIdentifiersAsync(cmd.ResponseStream, shareAcl, ct).ConfigureAwait(false);
        return shareAcl;
      });
      return cmd1;
    }

    private RESTCommand<ShareStats> GetStatsImpl(FileRequestOptions options)
    {
      RESTCommand<ShareStats> cmd1 = new RESTCommand<ShareStats>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<ShareStats>(cmd1);
      cmd1.CommandLocationMode = CommandLocationMode.PrimaryOrSecondary;
      cmd1.BuildRequest = (Func<RESTCommand<ShareStats>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => ShareHttpRequestMessageFactory.GetStats(uri, serverTimeout, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.RetrieveResponseStream = true;
      cmd1.PreProcessResponse = (Func<RESTCommand<ShareStats>, HttpResponseMessage, Exception, OperationContext, ShareStats>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<ShareStats>(HttpStatusCode.OK, resp, (ShareStats) null, (StorageCommandBase<ShareStats>) cmd, ex));
      cmd1.PostProcessResponseAsync = (Func<RESTCommand<ShareStats>, HttpResponseMessage, OperationContext, CancellationToken, Task<ShareStats>>) ((cmd, resp, ctx, ct) => ShareHttpResponseParsers.ReadShareStatsAsync(cmd.ResponseStream, ct));
      return cmd1;
    }

    private RESTCommand<NullType> SetMetadataImpl(
      AccessCondition accessCondition,
      FileRequestOptions options)
    {
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) =>
      {
        StorageRequestMessage request = ShareHttpRequestMessageFactory.SetMetadata(uri, serverTimeout, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials);
        ShareHttpRequestMessageFactory.AddMetadata(request, this.Metadata);
        return request;
      });
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.OK, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex);
        this.UpdateETagAndLastModified(resp);
        return NullType.Value;
      });
      return cmd1;
    }

    private RESTCommand<NullType> SetPermissionsImpl(
      FileSharePermissions acl,
      AccessCondition accessCondition,
      FileRequestOptions options)
    {
      MultiBufferMemoryStream memoryStream = new MultiBufferMemoryStream(this.ServiceClient.BufferManager, 1024);
      FileRequest.WriteSharedAccessIdentifiers(acl.SharedAccessPolicies, (Stream) memoryStream);
      memoryStream.Seek(0L, SeekOrigin.Begin);
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => ShareHttpRequestMessageFactory.SetAcl(uri, serverTimeout, FileSharePublicAccessType.Off, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.BuildContent = (Func<RESTCommand<NullType>, OperationContext, HttpContent>) ((cmd, ctx) => HttpContentFactory.BuildContentFromStream<NullType>((Stream) memoryStream, 0L, new long?(memoryStream.Length), Checksum.None, cmd, ctx));
      cmd1.StreamToDispose = (Stream) memoryStream;
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.OK, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex);
        this.UpdateETagAndLastModified(resp);
        return NullType.Value;
      });
      return cmd1;
    }

    private RESTCommand<NullType> SetPropertiesImpl(
      AccessCondition accessCondition,
      FileRequestOptions options)
    {
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) =>
      {
        StorageRequestMessage request = ShareHttpRequestMessageFactory.SetProperties(uri, serverTimeout, this.Properties, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials);
        ShareHttpRequestMessageFactory.AddMetadata(request, this.Metadata);
        return request;
      });
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.OK, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex);
        this.UpdateETagAndLastModified(resp);
        return NullType.Value;
      });
      return cmd1;
    }

    private RESTCommand<string> CreateFilePermissionImp(
      string permission,
      FileRequestOptions options)
    {
      RESTCommand<string> cmd1 = new RESTCommand<string>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<string>(cmd1);
      cmd1.BuildContent = (Func<RESTCommand<string>, OperationContext, HttpContent>) ((cmd, ctx) => (HttpContent) new StringContent(JsonConvert.SerializeObject((object) new Dictionary<string, string>()
      {
        {
          "Permission",
          permission
        }
      }, Formatting.Indented)));
      cmd1.BuildRequest = (Func<RESTCommand<string>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => ShareHttpRequestMessageFactory.CreateFilePermission(uri, serverTimeout, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<string>, HttpResponseMessage, Exception, OperationContext, string>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<string>(HttpStatusCode.Created, resp, (string) null, (StorageCommandBase<string>) cmd, ex);
        return HttpResponseParsers.GetHeader(resp, "x-ms-file-permission-key");
      });
      return cmd1;
    }

    private RESTCommand<string> GetFilePermissionImp(
      string filePermissionKey,
      FileRequestOptions options)
    {
      RESTCommand<string> cmd1 = new RESTCommand<string>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<string>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<string>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => ShareHttpRequestMessageFactory.GetFilePermission(uri, serverTimeout, filePermissionKey, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.RetrieveResponseStream = true;
      cmd1.PreProcessResponse = (Func<RESTCommand<string>, HttpResponseMessage, Exception, OperationContext, string>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<string>(HttpStatusCode.OK, resp, (string) null, (StorageCommandBase<string>) cmd, ex));
      cmd1.PostProcessResponseAsync = (Func<RESTCommand<string>, HttpResponseMessage, OperationContext, CancellationToken, Task<string>>) (async (cmd, resp, ex, ctx) => JsonConvert.DeserializeObject<Dictionary<string, string>>(await new StreamReader(cmd.ResponseStream).ReadToEndAsync().ConfigureAwait(false))["Permission".ToLowerInvariant()]);
      return cmd1;
    }

    private void UpdateETagAndLastModified(HttpResponseMessage response)
    {
      FileShareProperties properties = ShareHttpResponseParsers.GetProperties(response);
      this.Properties.ETag = properties.ETag;
      this.Properties.LastModified = properties.LastModified;
    }

    public CloudFileShare(Uri shareAddress)
      : this(shareAddress, (StorageCredentials) null)
    {
    }

    public CloudFileShare(Uri shareAddress, StorageCredentials credentials)
      : this(new StorageUri(shareAddress), credentials)
    {
    }

    public CloudFileShare(
      Uri shareAddress,
      DateTimeOffset? snapshotTime,
      StorageCredentials credentials)
      : this(new StorageUri(shareAddress), snapshotTime, credentials)
    {
    }

    public CloudFileShare(StorageUri shareAddress, StorageCredentials credentials)
      : this(shareAddress, new DateTimeOffset?(), credentials)
    {
    }

    public CloudFileShare(
      StorageUri shareAddress,
      DateTimeOffset? snapshotTime,
      StorageCredentials credentials)
    {
      CommonUtility.AssertNotNull(nameof (shareAddress), (object) shareAddress);
      CommonUtility.AssertNotNull(nameof (shareAddress), (object) shareAddress.PrimaryUri);
      this.SnapshotTime = snapshotTime;
      this.ParseQueryAndVerify(shareAddress, credentials);
      this.Metadata = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.Properties = new FileShareProperties();
    }

    internal CloudFileShare(
      string shareName,
      DateTimeOffset? snapshotTime,
      CloudFileClient serviceClient)
      : this(new FileShareProperties(), (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase), shareName, snapshotTime, serviceClient)
    {
    }

    internal CloudFileShare(
      FileShareProperties properties,
      IDictionary<string, string> metadata,
      string shareName,
      DateTimeOffset? snapshotTime,
      CloudFileClient serviceClient)
    {
      this.StorageUri = NavigationHelper.AppendPathToUri(serviceClient.StorageUri, shareName);
      this.ServiceClient = serviceClient;
      this.Name = shareName;
      this.Metadata = metadata;
      this.Properties = properties;
      this.SnapshotTime = snapshotTime;
    }

    public CloudFileClient ServiceClient { get; private set; }

    public Uri Uri => this.StorageUri.PrimaryUri;

    public StorageUri StorageUri { get; private set; }

    public DateTimeOffset? SnapshotTime { get; internal set; }

    public bool IsSnapshot => this.SnapshotTime.HasValue;

    public Uri SnapshotQualifiedUri
    {
      get
      {
        if (!this.SnapshotTime.HasValue)
          return this.Uri;
        UriQueryBuilder uriQueryBuilder = new UriQueryBuilder();
        uriQueryBuilder.Add("sharesnapshot", Request.ConvertDateTimeToSnapshotString(this.SnapshotTime.Value));
        return uriQueryBuilder.AddToUri(this.Uri);
      }
    }

    public StorageUri SnapshotQualifiedStorageUri
    {
      get
      {
        if (!this.SnapshotTime.HasValue)
          return this.StorageUri;
        UriQueryBuilder uriQueryBuilder = new UriQueryBuilder();
        uriQueryBuilder.Add("sharesnapshot", Request.ConvertDateTimeToSnapshotString(this.SnapshotTime.Value));
        return uriQueryBuilder.AddToUri(this.StorageUri);
      }
    }

    internal void AssertNoSnapshot()
    {
      if (this.SnapshotTime.HasValue)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot perform this operation on a share representing a snapshot."));
    }

    public string Name { get; private set; }

    public IDictionary<string, string> Metadata { get; private set; }

    public FileShareProperties Properties { get; private set; }

    private string GetSharedAccessCanonicalName() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/{0}/{1}/{2}", (object) "file", (object) this.ServiceClient.Credentials.AccountName, (object) this.Name);

    public string GetSharedAccessSignature(SharedAccessFilePolicy policy) => this.GetSharedAccessSignature(policy, (string) null);

    public string GetSharedAccessSignature(
      SharedAccessFilePolicy policy,
      string groupPolicyIdentifier)
    {
      return this.GetSharedAccessSignature(policy, groupPolicyIdentifier, new SharedAccessProtocol?(), (IPAddressOrRange) null);
    }

    public string GetSharedAccessSignature(
      SharedAccessFilePolicy policy,
      string groupPolicyIdentifier,
      SharedAccessProtocol? protocols,
      IPAddressOrRange ipAddressOrRange)
    {
      if (!this.ServiceClient.Credentials.IsSharedKey)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot create Shared Access Signature unless Account Key credentials are used."));
      string accessCanonicalName = this.GetSharedAccessCanonicalName();
      StorageAccountKey key = this.ServiceClient.Credentials.Key;
      string hash = FileSharedAccessSignatureHelper.GetHash(policy, (SharedAccessFileHeaders) null, groupPolicyIdentifier, accessCanonicalName, "2019-07-07", protocols, ipAddressOrRange, key.KeyValue);
      string keyName = key.KeyName;
      return FileSharedAccessSignatureHelper.GetSignature(policy, (SharedAccessFileHeaders) null, groupPolicyIdentifier, "s", hash, keyName, "2019-07-07", protocols, ipAddressOrRange).ToString();
    }

    private void ParseQueryAndVerify(StorageUri address, StorageCredentials credentials)
    {
      StorageCredentials parsedCredentials;
      DateTimeOffset? parsedShareSnapshot;
      this.StorageUri = NavigationHelper.ParseFileQueryAndVerify(address, out parsedCredentials, out parsedShareSnapshot);
      if (parsedCredentials != null && credentials != null && !credentials.Equals(new StorageCredentials()))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot provide credentials as part of the address and as constructor parameter. Either pass in the address or use a different constructor."));
      if (parsedShareSnapshot.HasValue && this.SnapshotTime.HasValue && !parsedShareSnapshot.Value.Equals(this.SnapshotTime.Value))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Multiple different snapshot times provided as part of query '{0}' and as constructor parameter '{1}'.", (object) parsedShareSnapshot, (object) this.SnapshotTime));
      if (parsedShareSnapshot.HasValue)
        this.SnapshotTime = parsedShareSnapshot;
      if (this.ServiceClient == null)
        this.ServiceClient = new CloudFileClient(NavigationHelper.GetServiceClientBaseAddress(this.StorageUri, new bool?()), credentials ?? parsedCredentials);
      this.Name = NavigationHelper.GetShareNameFromShareAddress(this.Uri, new bool?(this.ServiceClient.UsePathStyleUris));
    }

    public virtual CloudFileDirectory GetRootDirectoryReference() => new CloudFileDirectory(this.StorageUri, string.Empty, this);
  }
}
