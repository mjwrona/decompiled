// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.File.CloudFileDirectory
// Assembly: Microsoft.Azure.Storage.File, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C68E95B0-8DFB-410C-8E70-706406D1A279
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.File.dll

using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Core;
using Microsoft.Azure.Storage.Core.Executor;
using Microsoft.Azure.Storage.Core.Util;
using Microsoft.Azure.Storage.File.Protocol;
using Microsoft.Azure.Storage.Shared.Protocol;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.File
{
  public class CloudFileDirectory : IListFileItem
  {
    private CloudFileShare share;
    private CloudFileDirectory parent;
    private string filePermission;

    [DoesServiceRequest]
    public virtual void Create(FileRequestOptions requestOptions = null, OperationContext operationContext = null)
    {
      this.AssertNoSnapshot();
      FileRequestOptions options = FileRequestOptions.ApplyDefaults(requestOptions, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.CreateDirectoryImpl(options), options.RetryPolicy, operationContext);
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
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.CreateAsync(options, operationContext)), callback, state);
    }

    public virtual void EndCreate(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task CreateAsync() => this.CreateAsync((FileRequestOptions) null, (OperationContext) null, CancellationToken.None);

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
      this.Share.AssertNoSnapshot();
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.CreateDirectoryImpl(options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual bool CreateIfNotExists(
      FileRequestOptions requestOptions = null,
      OperationContext operationContext = null)
    {
      if (string.IsNullOrEmpty(this.Name))
      {
        this.ServiceClient.GetShareReference(this.Share.Name, this.Share.SnapshotTime).FetchAttributes(options: requestOptions, operationContext: operationContext);
        return false;
      }
      try
      {
        this.Create(requestOptions, operationContext);
        return true;
      }
      catch (StorageException ex)
      {
        if (ex.RequestInformation.HttpStatusCode == 409 && (ex.RequestInformation.ExtendedErrorInformation == null || ex.RequestInformation.ExtendedErrorInformation.ErrorCode == FileErrorCodeStrings.ResourceAlreadyExists))
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
      if (string.IsNullOrEmpty(this.Name))
      {
        await this.ServiceClient.GetShareReference(this.Share.Name, this.Share.SnapshotTime).FetchAttributesAsync((AccessCondition) null, options, operationContext, cancellationToken).ConfigureAwait(false);
        return false;
      }
      try
      {
        await this.CreateAsync(options, operationContext, cancellationToken).ConfigureAwait(false);
        return true;
      }
      catch (StorageException ex)
      {
        if (ex.RequestInformation.HttpStatusCode == 409 && (ex.RequestInformation.ExtendedErrorInformation == null || ex.RequestInformation.ExtendedErrorInformation.ErrorCode == FileErrorCodeStrings.ResourceAlreadyExists))
          return false;
        throw;
      }
    }

    [DoesServiceRequest]
    public virtual void Delete(
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.AssertNoSnapshot();
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.DeleteDirectoryImpl(accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDelete(AsyncCallback callback, object state) => this.BeginDelete((AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, callback, state);

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDelete(
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.DeleteAsync(accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual void EndDelete(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task DeleteAsync() => this.DeleteAsync(CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task DeleteAsync(CancellationToken cancellationToken) => this.DeleteAsync((AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task DeleteAsync(
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      return this.DeleteAsync(accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task DeleteAsync(
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      this.Share.AssertNoSnapshot();
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.DeleteDirectoryImpl(accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual bool DeleteIfExists(
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
      OperationContext operationContext = null)
    {
      try
      {
        if (!this.Exists(options, operationContext))
          return false;
      }
      catch (StorageException ex)
      {
        if (ex.RequestInformation.HttpStatusCode != 403)
          throw;
      }
      try
      {
        this.Delete(accessCondition, options, operationContext);
        return true;
      }
      catch (StorageException ex)
      {
        if (ex.RequestInformation.HttpStatusCode == 404)
          return false;
        throw;
      }
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDeleteIfExists(AsyncCallback callback, object state) => this.BeginDeleteIfExists((AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, callback, state);

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDeleteIfExists(
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<bool>((Func<CancellationToken, Task<bool>>) (token => this.DeleteIfExistsAsync(accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual bool EndDeleteIfExists(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<bool>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<bool> DeleteIfExistsAsync() => this.DeleteIfExistsAsync(CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<bool> DeleteIfExistsAsync(CancellationToken cancellationToken) => this.DeleteIfExistsAsync((AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task<bool> DeleteIfExistsAsync(
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      return this.DeleteIfExistsAsync(accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual async Task<bool> DeleteIfExistsAsync(
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      try
      {
        if (!await this.ExistsAsync(options, operationContext, cancellationToken).ConfigureAwait(false))
          return false;
      }
      catch (StorageException ex)
      {
        if (ex.RequestInformation.HttpStatusCode != 403)
          throw;
      }
      try
      {
        await this.DeleteAsync(accessCondition, options, operationContext, cancellationToken).ConfigureAwait(false);
        return true;
      }
      catch (StorageException ex)
      {
        if (ex.RequestInformation.HttpStatusCode == 404)
          return false;
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
    public virtual IEnumerable<IListFileItem> ListFilesAndDirectories(
      FileRequestOptions options = null,
      OperationContext operationContext = null)
    {
      return this.ListFilesAndDirectories((string) null, options, operationContext);
    }

    [DoesServiceRequest]
    public IEnumerable<IListFileItem> ListFilesAndDirectories(
      string prefix,
      FileRequestOptions options = null,
      OperationContext operationContext = null)
    {
      FileRequestOptions modifiedOptions = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      return CommonUtility.LazyEnumerable<IListFileItem>((Func<IContinuationToken, ResultSegment<IListFileItem>>) (token => this.ListFilesAndDirectoriesSegmentedCore(prefix, new int?(), (FileContinuationToken) token, modifiedOptions, operationContext)), long.MaxValue);
    }

    [DoesServiceRequest]
    public virtual FileResultSegment ListFilesAndDirectoriesSegmented(
      FileContinuationToken currentToken)
    {
      return this.ListFilesAndDirectoriesSegmented((string) null, new int?(), currentToken, (FileRequestOptions) null, (OperationContext) null);
    }

    [DoesServiceRequest]
    public virtual FileResultSegment ListFilesAndDirectoriesSegmented(
      int? maxResults,
      FileContinuationToken currentToken,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      return this.ListFilesAndDirectoriesSegmented((string) null, maxResults, currentToken, options, operationContext);
    }

    [DoesServiceRequest]
    public FileResultSegment ListFilesAndDirectoriesSegmented(
      string prefix,
      int? maxResults,
      FileContinuationToken currentToken,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      ResultSegment<IListFileItem> resultSegment = this.ListFilesAndDirectoriesSegmentedCore(prefix, maxResults, currentToken, options1, operationContext);
      return new FileResultSegment((IEnumerable<IListFileItem>) resultSegment.Results, (FileContinuationToken) resultSegment.ContinuationToken);
    }

    private ResultSegment<IListFileItem> ListFilesAndDirectoriesSegmentedCore(
      string prefix,
      int? maxResults,
      FileContinuationToken currentToken,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<ResultSegment<IListFileItem>>(this.ListFilesAndDirectoriesImpl(maxResults, options, currentToken, prefix), options.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginListFilesAndDirectoriesSegmented(
      FileContinuationToken currentToken,
      AsyncCallback callback,
      object state)
    {
      return this.BeginListFilesAndDirectoriesSegmented((string) null, new int?(), currentToken, (FileRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginListFilesAndDirectoriesSegmented(
      int? maxResults,
      FileContinuationToken currentToken,
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return this.BeginListFilesAndDirectoriesSegmented((string) null, maxResults, currentToken, options, operationContext, callback, state);
    }

    [DoesServiceRequest]
    public ICancellableAsyncResult BeginListFilesAndDirectoriesSegmented(
      string prefix,
      int? maxResults,
      FileContinuationToken currentToken,
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<FileResultSegment>((Func<CancellationToken, Task<FileResultSegment>>) (token => this.ListFilesAndDirectoriesSegmentedAsync(prefix, maxResults, currentToken, options, operationContext, token)), callback, state);
    }

    public virtual FileResultSegment EndListFilesAndDirectoriesSegmented(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<FileResultSegment>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<FileResultSegment> ListFilesAndDirectoriesSegmentedAsync(
      FileContinuationToken currentToken)
    {
      return this.ListFilesAndDirectoriesSegmentedAsync((string) null, currentToken, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<FileResultSegment> ListFilesAndDirectoriesSegmentedAsync(
      FileContinuationToken currentToken,
      CancellationToken cancellationToken)
    {
      return this.ListFilesAndDirectoriesSegmentedAsync((string) null, currentToken, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<FileResultSegment> ListFilesAndDirectoriesSegmentedAsync(
      string prefix,
      FileContinuationToken currentToken,
      CancellationToken cancellationToken)
    {
      return this.ListFilesAndDirectoriesSegmentedAsync(prefix, new int?(), currentToken, (FileRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<FileResultSegment> ListFilesAndDirectoriesSegmentedAsync(
      int? maxResults,
      FileContinuationToken currentToken,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      return this.ListFilesAndDirectoriesSegmentedAsync((string) null, maxResults, currentToken, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<FileResultSegment> ListFilesAndDirectoriesSegmentedAsync(
      int? maxResults,
      FileContinuationToken currentToken,
      FileRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.ListFilesAndDirectoriesSegmentedAsync((string) null, maxResults, currentToken, options, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual async Task<FileResultSegment> ListFilesAndDirectoriesSegmentedAsync(
      string prefix,
      int? maxResults,
      FileContinuationToken currentToken,
      FileRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      ResultSegment<IListFileItem> resultSegment = await Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<ResultSegment<IListFileItem>>(this.ListFilesAndDirectoriesImpl(maxResults, options1, currentToken, prefix), options1.RetryPolicy, operationContext, cancellationToken).ConfigureAwait(false);
      return new FileResultSegment((IEnumerable<IListFileItem>) resultSegment.Results, (FileContinuationToken) resultSegment.ContinuationToken);
    }

    [DoesServiceRequest]
    public virtual FileHandleResultSegment ListHandlesSegmented(
      FileContinuationToken token = null,
      int? maxResults = null,
      bool? recursive = null,
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
      OperationContext operationContext = null)
    {
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<FileHandleResultSegment>(this.ListHandlesImpl(token, maxResults, recursive, accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginListHandlesSegmented(
      FileContinuationToken token,
      int? maxResults,
      bool? recursive,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<FileHandleResultSegment>((Func<CancellationToken, Task<FileHandleResultSegment>>) (cancellationToken => this.ListHandlesSegmentedAsync(token, maxResults, recursive, accessCondition, options, operationContext, new CancellationToken?(cancellationToken))), callback, state);
    }

    public virtual FileHandleResultSegment EndListHandlesSegmented(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<FileHandleResultSegment>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<FileHandleResultSegment> ListHandlesSegmentedAsync(
      FileContinuationToken token = null,
      int? maxResults = null,
      bool? recursive = null,
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
      OperationContext operationContext = null,
      CancellationToken? cancellationToken = null)
    {
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<FileHandleResultSegment>(this.ListHandlesImpl(token, maxResults, recursive, accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken ?? CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual CloseFileHandleResultSegment CloseAllHandlesSegmented(
      FileContinuationToken token = null,
      bool? recursive = null,
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
      OperationContext operationContext = null)
    {
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<CloseFileHandleResultSegment>(this.CloseHandleImpl(token, "*", recursive, accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginCloseAllHandlesSegmented(
      FileContinuationToken token,
      bool? recursive,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<CloseFileHandleResultSegment>((Func<CancellationToken, Task<CloseFileHandleResultSegment>>) (cancellationToken => this.CloseAllHandlesSegmentedAsync(token, recursive, accessCondition, options, operationContext, new CancellationToken?(cancellationToken))), callback, state);
    }

    public virtual CloseFileHandleResultSegment EndCloseAllHandlesSegmented(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<CloseFileHandleResultSegment>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<CloseFileHandleResultSegment> CloseAllHandlesSegmentedAsync(
      FileContinuationToken token = null,
      bool? recursive = null,
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
      OperationContext operationContext = null,
      CancellationToken? cancellationToken = null)
    {
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<CloseFileHandleResultSegment>(this.CloseHandleImpl(token, "*", recursive, accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken ?? CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual CloseFileHandleResultSegment CloseHandleSegmented(
      string handleId,
      FileContinuationToken token = null,
      bool? recursive = null,
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
      OperationContext operationContext = null)
    {
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<CloseFileHandleResultSegment>(this.CloseHandleImpl(token, handleId, recursive, accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual CloseFileHandleResultSegment CloseHandleSegmented(
      ulong handleId,
      FileContinuationToken token = null,
      bool? recursive = null,
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
      OperationContext operationContext = null)
    {
      return this.CloseHandleSegmented(handleId.ToString(), token, recursive, accessCondition, options, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginCloseHandleSegmented(
      string handleId,
      FileContinuationToken token,
      bool? recursive,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<CloseFileHandleResultSegment>((Func<CancellationToken, Task<CloseFileHandleResultSegment>>) (cancellationToken => this.CloseHandleSegmentedAsync(handleId, token, recursive, accessCondition, options, operationContext, new CancellationToken?(cancellationToken))), callback, state);
    }

    public virtual CloseFileHandleResultSegment EndCloseHandleSegmented(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<CloseFileHandleResultSegment>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<CloseFileHandleResultSegment> CloseHandleSegmentedAsync(
      string handleId,
      FileContinuationToken token = null,
      bool? recursive = null,
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
      OperationContext operationContext = null,
      CancellationToken? cancellationToken = null)
    {
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<CloseFileHandleResultSegment>(this.CloseHandleImpl(token, handleId, recursive, accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken ?? CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<CloseFileHandleResultSegment> CloseHandleSegmentedAsync(
      ulong handleId,
      FileContinuationToken token = null,
      bool? recursive = null,
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
      OperationContext operationContext = null,
      CancellationToken? cancellationToken = null)
    {
      return this.CloseHandleSegmentedAsync(handleId.ToString(), token, recursive, accessCondition, options, operationContext, cancellationToken);
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
      this.Share.AssertNoSnapshot();
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.SetMetadataImpl(accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual void SetProperties(FileRequestOptions options = null, OperationContext operationContext = null)
    {
      this.AssertNoSnapshot();
      this.AssertValidFilePermissionOrKey();
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.SetPropertiesImpl(options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual Task SetPropertiesAsync(
      FileRequestOptions options = null,
      OperationContext operationContext = null,
      CancellationToken? cancellationToken = null)
    {
      this.AssertNoSnapshot();
      this.AssertValidFilePermissionOrKey();
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.SetPropertiesImpl(options1), options1.RetryPolicy, operationContext, cancellationToken ?? CancellationToken.None);
    }

    private RESTCommand<NullType> CreateDirectoryImpl(FileRequestOptions options)
    {
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) =>
      {
        StorageRequestMessage request = DirectoryHttpRequestMessageFactory.Create(uri, serverTimeout, this.Properties, this.FilePermission, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials);
        DirectoryHttpRequestMessageFactory.AddMetadata(request, this.Metadata);
        return request;
      });
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.Created, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex);
        DirectoryHttpResponseParsers.UpdateSmbProperties(resp, this.Properties);
        this.UpdateETagAndLastModified(resp);
        this.filePermission = (string) null;
        cmd.CurrentResult.IsRequestServerEncrypted = HttpResponseParsers.ParseServerRequestEncrypted(resp);
        return NullType.Value;
      });
      return cmd1;
    }

    private RESTCommand<NullType> DeleteDirectoryImpl(
      AccessCondition accessCondition,
      FileRequestOptions options)
    {
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => DirectoryHttpRequestMessageFactory.Delete(uri, serverTimeout, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.Accepted, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex));
      return cmd1;
    }

    private RESTCommand<bool> ExistsImpl(FileRequestOptions options)
    {
      RESTCommand<bool> cmd1 = new RESTCommand<bool>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<bool>(cmd1);
      cmd1.CommandLocationMode = CommandLocationMode.PrimaryOrSecondary;
      cmd1.BuildRequest = (Func<RESTCommand<bool>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => DirectoryHttpRequestMessageFactory.GetProperties(uri, serverTimeout, this.Share.SnapshotTime, (AccessCondition) null, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<bool>, HttpResponseMessage, Exception, OperationContext, bool>) ((cmd, resp, ex, ctx) =>
      {
        if (resp.StatusCode == HttpStatusCode.NotFound)
          return false;
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<bool>(HttpStatusCode.OK, resp, true, (StorageCommandBase<bool>) cmd, ex);
        this.Properties = DirectoryHttpResponseParsers.GetProperties(resp);
        this.Metadata = DirectoryHttpResponseParsers.GetMetadata(resp);
        return true;
      });
      return cmd1;
    }

    private RESTCommand<FileHandleResultSegment> ListHandlesImpl(
      FileContinuationToken token,
      int? maxResults,
      bool? recursive,
      AccessCondition accessCondition,
      FileRequestOptions options)
    {
      RESTCommand<FileHandleResultSegment> cmd1 = new RESTCommand<FileHandleResultSegment>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<FileHandleResultSegment>(cmd1);
      cmd1.CommandLocationMode = CommandLocationMode.PrimaryOrSecondary;
      cmd1.RetrieveResponseStream = true;
      cmd1.BuildRequest = (Func<RESTCommand<FileHandleResultSegment>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) =>
      {
        StorageRequestMessage request = FileHttpRequestMessageFactory.ListHandles(uri, serverTimeout, this.Share.SnapshotTime, maxResults, recursive, token, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials);
        FileHttpRequestMessageFactory.AddMetadata(request, this.Metadata);
        return request;
      });
      cmd1.PreProcessResponse = (Func<RESTCommand<FileHandleResultSegment>, HttpResponseMessage, Exception, OperationContext, FileHandleResultSegment>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<FileHandleResultSegment>(HttpStatusCode.OK, resp, (FileHandleResultSegment) null, (StorageCommandBase<FileHandleResultSegment>) cmd, ex));
      cmd1.PostProcessResponseAsync = (Func<RESTCommand<FileHandleResultSegment>, HttpResponseMessage, OperationContext, CancellationToken, Task<FileHandleResultSegment>>) ((cmd, resp, ctx, ct) =>
      {
        ListHandlesResponse listHandlesResponse = new ListHandlesResponse(cmd.ResponseStream);
        return Task.FromResult<FileHandleResultSegment>(new FileHandleResultSegment()
        {
          Results = listHandlesResponse.Handles,
          ContinuationToken = new FileContinuationToken()
          {
            NextMarker = listHandlesResponse.NextMarker
          }
        });
      });
      return cmd1;
    }

    private RESTCommand<CloseFileHandleResultSegment> CloseHandleImpl(
      FileContinuationToken token,
      string handleId,
      bool? recursive,
      AccessCondition accessCondition,
      FileRequestOptions options)
    {
      RESTCommand<CloseFileHandleResultSegment> cmd1 = new RESTCommand<CloseFileHandleResultSegment>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<CloseFileHandleResultSegment>(cmd1);
      cmd1.CommandLocationMode = CommandLocationMode.PrimaryOrSecondary;
      cmd1.RetrieveResponseStream = true;
      cmd1.BuildRequest = (Func<RESTCommand<CloseFileHandleResultSegment>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) =>
      {
        StorageRequestMessage request = FileHttpRequestMessageFactory.CloseHandle(uri, serverTimeout, this.Share.SnapshotTime, handleId, recursive, token, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials);
        FileHttpRequestMessageFactory.AddMetadata(request, this.Metadata);
        return request;
      });
      cmd1.PreProcessResponse = (Func<RESTCommand<CloseFileHandleResultSegment>, HttpResponseMessage, Exception, OperationContext, CloseFileHandleResultSegment>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<CloseFileHandleResultSegment>(HttpStatusCode.OK, resp, (CloseFileHandleResultSegment) null, (StorageCommandBase<CloseFileHandleResultSegment>) cmd, ex);
        int result;
        if (!int.TryParse(resp.Headers.GetHeaderSingleValueOrDefault("x-ms-number-of-handles-closed"), out result))
          result = -1;
        FileContinuationToken continuationToken = (FileContinuationToken) null;
        string singleValueOrDefault;
        if ((singleValueOrDefault = resp.Headers.GetHeaderSingleValueOrDefault("x-ms-marker")) != "")
          continuationToken = new FileContinuationToken()
          {
            NextMarker = singleValueOrDefault
          };
        return new CloseFileHandleResultSegment()
        {
          NumHandlesClosed = result,
          ContinuationToken = continuationToken
        };
      });
      return cmd1;
    }

    private RESTCommand<NullType> FetchAttributesImpl(
      AccessCondition accessCondition,
      FileRequestOptions options)
    {
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => DirectoryHttpRequestMessageFactory.GetProperties(uri, serverTimeout, this.Share.SnapshotTime, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.OK, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex);
        this.Properties = DirectoryHttpResponseParsers.GetProperties(resp);
        DirectoryHttpResponseParsers.UpdateSmbProperties(resp, this.Properties);
        this.Metadata = DirectoryHttpResponseParsers.GetMetadata(resp);
        return NullType.Value;
      });
      return cmd1;
    }

    private RESTCommand<ResultSegment<IListFileItem>> ListFilesAndDirectoriesImpl(
      int? maxResults,
      FileRequestOptions options,
      FileContinuationToken currentToken,
      string prefix)
    {
      FileListingContext fileListingContext = new FileListingContext(maxResults);
      fileListingContext.Marker = currentToken?.NextMarker;
      fileListingContext.Prefix = string.IsNullOrEmpty(prefix) ? (string) null : prefix;
      FileListingContext listingContext = fileListingContext;
      RESTCommand<ResultSegment<IListFileItem>> cmd1 = new RESTCommand<ResultSegment<IListFileItem>>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<ResultSegment<IListFileItem>>(cmd1);
      cmd1.CommandLocationMode = CommonUtility.GetListingLocationMode((IContinuationToken) currentToken);
      cmd1.RetrieveResponseStream = true;
      cmd1.BuildRequest = (Func<RESTCommand<ResultSegment<IListFileItem>>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => DirectoryHttpRequestMessageFactory.List(uri, serverTimeout, this.Share.SnapshotTime, listingContext, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<ResultSegment<IListFileItem>>, HttpResponseMessage, Exception, OperationContext, ResultSegment<IListFileItem>>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<ResultSegment<IListFileItem>>(HttpStatusCode.OK, resp, (ResultSegment<IListFileItem>) null, (StorageCommandBase<ResultSegment<IListFileItem>>) cmd, ex));
      Func<IListFileEntry, IListFileItem> func;
      cmd1.PostProcessResponseAsync = (Func<RESTCommand<ResultSegment<IListFileItem>>, HttpResponseMessage, OperationContext, CancellationToken, Task<ResultSegment<IListFileItem>>>) (async (cmd, resp, ctx, ct) =>
      {
        ListFilesAndDirectoriesResponse directoriesResponse = await ListFilesAndDirectoriesResponse.ParseAsync(cmd.ResponseStream, ct).ConfigureAwait(false);
        List<IListFileItem> list = directoriesResponse.Files.Select<IListFileEntry, IListFileItem>(func ?? (func = (Func<IListFileEntry, IListFileItem>) (item => this.SelectListFileItem(item)))).ToList<IListFileItem>();
        FileContinuationToken continuationToken = (FileContinuationToken) null;
        if (directoriesResponse.NextMarker != null)
          continuationToken = new FileContinuationToken()
          {
            NextMarker = directoriesResponse.NextMarker,
            TargetLocation = new StorageLocation?(cmd.CurrentResult.TargetLocation)
          };
        return new ResultSegment<IListFileItem>(list)
        {
          ContinuationToken = (IContinuationToken) continuationToken
        };
      });
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
        StorageRequestMessage request = DirectoryHttpRequestMessageFactory.SetMetadata(uri, serverTimeout, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials);
        DirectoryHttpRequestMessageFactory.AddMetadata(request, this.Metadata);
        return request;
      });
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.OK, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex);
        this.UpdateETagAndLastModified(resp);
        cmd.CurrentResult.IsRequestServerEncrypted = HttpResponseParsers.ParseServerRequestEncrypted(resp);
        return NullType.Value;
      });
      return cmd1;
    }

    private RESTCommand<NullType> SetPropertiesImpl(FileRequestOptions options)
    {
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) =>
      {
        StorageRequestMessage request = DirectoryHttpRequestMessageFactory.SetProperties(uri, serverTimeout, this.Properties, this.FilePermission, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials);
        DirectoryHttpRequestMessageFactory.AddMetadata(request, this.Metadata);
        return request;
      });
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.OK, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex);
        DirectoryHttpResponseParsers.UpdateSmbProperties(resp, this.Properties);
        this.UpdateETagAndLastModified(resp);
        this.filePermission = (string) null;
        cmd.CurrentResult.IsRequestServerEncrypted = HttpResponseParsers.ParseServerRequestEncrypted(resp);
        return NullType.Value;
      });
      return cmd1;
    }

    private void UpdateETagAndLastModified(HttpResponseMessage response)
    {
      FileDirectoryProperties properties = DirectoryHttpResponseParsers.GetProperties(response);
      this.Properties.ETag = properties.ETag;
      this.Properties.LastModified = properties.LastModified;
    }

    public CloudFileDirectory(Uri directoryAbsoluteUri)
      : this(new StorageUri(directoryAbsoluteUri), (StorageCredentials) null)
    {
    }

    public CloudFileDirectory(Uri directoryAbsoluteUri, StorageCredentials credentials)
      : this(new StorageUri(directoryAbsoluteUri), credentials)
    {
    }

    public CloudFileDirectory(StorageUri directoryAbsoluteUri, StorageCredentials credentials)
    {
      this.Metadata = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.Properties = new FileDirectoryProperties();
      this.ParseQueryAndVerify(directoryAbsoluteUri, credentials);
    }

    internal CloudFileDirectory(StorageUri uri, string directoryName, CloudFileShare share)
    {
      CommonUtility.AssertNotNull(nameof (uri), (object) uri);
      CommonUtility.AssertNotNull(nameof (directoryName), (object) directoryName);
      CommonUtility.AssertNotNull(nameof (share), (object) share);
      this.Metadata = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.Properties = new FileDirectoryProperties();
      this.StorageUri = uri;
      this.ServiceClient = share.ServiceClient;
      this.share = share;
      this.Name = directoryName;
    }

    public CloudFileClient ServiceClient { get; private set; }

    public Uri Uri => this.StorageUri.PrimaryUri;

    public StorageUri StorageUri { get; private set; }

    public Uri SnapshotQualifiedUri
    {
      get
      {
        if (!this.Share.SnapshotTime.HasValue)
          return this.Uri;
        UriQueryBuilder uriQueryBuilder = new UriQueryBuilder();
        uriQueryBuilder.Add("sharesnapshot", Request.ConvertDateTimeToSnapshotString(this.Share.SnapshotTime.Value));
        return uriQueryBuilder.AddToUri(this.Uri);
      }
    }

    public StorageUri SnapshotQualifiedStorageUri
    {
      get
      {
        if (!this.Share.SnapshotTime.HasValue)
          return this.StorageUri;
        UriQueryBuilder uriQueryBuilder = new UriQueryBuilder();
        uriQueryBuilder.Add("sharesnapshot", Request.ConvertDateTimeToSnapshotString(this.Share.SnapshotTime.Value));
        return uriQueryBuilder.AddToUri(this.StorageUri);
      }
    }

    internal void AssertNoSnapshot()
    {
      if (this.Share.IsSnapshot)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot perform this operation on a share representing a snapshot."));
    }

    internal void AssertValidFilePermissionOrKey()
    {
      if (this.filePermission != null && this.Properties?.filePermissionKeyToSet != null)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "File permission and file permission key cannot both be set"));
    }

    public FileDirectoryProperties Properties { get; internal set; }

    public IDictionary<string, string> Metadata { get; internal set; }

    public string FilePermission
    {
      get => this.filePermission;
      set
      {
        CommonUtility.AssertInBounds<long>(nameof (FilePermission), (long) Encoding.UTF8.GetBytes(value.ToCharArray()).Length, 0L, 8192L);
        this.filePermission = value;
      }
    }

    public CloudFileShare Share
    {
      get
      {
        if (this.share == null)
          this.share = this.ServiceClient.GetShareReference(NavigationHelper.GetShareName(this.Uri, new bool?(this.ServiceClient.UsePathStyleUris)));
        return this.share;
      }
    }

    public CloudFileDirectory Parent
    {
      get
      {
        string parentName;
        StorageUri parentAddress;
        if (this.parent == null && NavigationHelper.GetFileParentNameAndAddress(this.StorageUri, new bool?(this.ServiceClient.UsePathStyleUris), out parentName, out parentAddress))
          this.parent = new CloudFileDirectory(parentAddress, parentName, this.Share);
        return this.parent;
      }
    }

    public string Name { get; private set; }

    private IListFileItem SelectListFileItem(IListFileEntry protocolItem)
    {
      switch (protocolItem)
      {
        case ListFileEntry listFileEntry:
          CloudFile fileReference = this.GetFileReference(listFileEntry.Name);
          fileReference.Properties = listFileEntry.Properties;
          fileReference.attributes = listFileEntry.Attributes;
          return (IListFileItem) fileReference;
        case ListFileDirectoryEntry fileDirectoryEntry:
          CloudFileDirectory directoryReference = this.GetDirectoryReference(fileDirectoryEntry.Name);
          directoryReference.Properties = fileDirectoryEntry.Properties;
          return (IListFileItem) directoryReference;
        default:
          throw new InvalidOperationException("Invalid file list item returned");
      }
    }

    public virtual CloudFile GetFileReference(string fileName)
    {
      CommonUtility.AssertNotNullOrEmpty(nameof (fileName), fileName);
      return new CloudFile(NavigationHelper.AppendPathToUri(this.StorageUri, fileName), fileName, this.Share);
    }

    public virtual CloudFileDirectory GetDirectoryReference(string itemName)
    {
      CommonUtility.AssertNotNullOrEmpty(nameof (itemName), itemName);
      return new CloudFileDirectory(NavigationHelper.AppendPathToUri(this.StorageUri, itemName), itemName, this.Share);
    }

    private void ParseQueryAndVerify(StorageUri address, StorageCredentials credentials)
    {
      StorageCredentials parsedCredentials;
      DateTimeOffset? parsedShareSnapshot;
      this.StorageUri = NavigationHelper.ParseFileQueryAndVerify(address, out parsedCredentials, out parsedShareSnapshot);
      if (parsedCredentials != null && credentials != null && !credentials.Equals(new StorageCredentials()))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot provide credentials as part of the address and as constructor parameter. Either pass in the address or use a different constructor."));
      if (this.ServiceClient == null)
        this.ServiceClient = new CloudFileClient(NavigationHelper.GetServiceClientBaseAddress(this.StorageUri, new bool?()), credentials ?? parsedCredentials);
      if (parsedShareSnapshot.HasValue)
        this.Share.SnapshotTime = parsedShareSnapshot;
      this.Name = NavigationHelper.GetFileName(this.Uri, new bool?(this.ServiceClient.UsePathStyleUris));
    }
  }
}
