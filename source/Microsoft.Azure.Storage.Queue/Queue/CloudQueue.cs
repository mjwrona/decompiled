// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Queue.CloudQueue
// Assembly: Microsoft.Azure.Storage.Queue, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3D35BFA0-638A-4C3C-8E74-B592D3B60EFD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Queue.dll

using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Core;
using Microsoft.Azure.Storage.Core.Auth;
using Microsoft.Azure.Storage.Core.Executor;
using Microsoft.Azure.Storage.Core.Util;
using Microsoft.Azure.Storage.Queue.Protocol;
using Microsoft.Azure.Storage.Shared.Protocol;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.Queue
{
  public class CloudQueue
  {
    private StorageUri messageRequestAddress;

    [DoesServiceRequest]
    public virtual void Create(QueueRequestOptions options = null, OperationContext operationContext = null)
    {
      QueueRequestOptions options1 = QueueRequestOptions.ApplyDefaults(options, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.CreateQueueImpl(options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginCreate(AsyncCallback callback, object state) => this.BeginCreate((QueueRequestOptions) null, (OperationContext) null, callback, state);

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginCreate(
      QueueRequestOptions options,
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
    public virtual Task CreateAsync(CancellationToken cancellationToken) => this.CreateAsync((QueueRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task CreateAsync(QueueRequestOptions options, OperationContext operationContext) => this.CreateAsync(options, operationContext, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task CreateAsync(
      QueueRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      QueueRequestOptions options1 = QueueRequestOptions.ApplyDefaults(options, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.CreateQueueImpl(options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual bool CreateIfNotExists(
      QueueRequestOptions options = null,
      OperationContext operationContext = null)
    {
      operationContext = operationContext ?? new OperationContext();
      try
      {
        this.Create(options, operationContext);
      }
      catch (StorageException ex)
      {
        if (ex.RequestInformation.HttpStatusCode == 409 && (ex.RequestInformation.ExtendedErrorInformation == null || ex.RequestInformation.ExtendedErrorInformation.ErrorCode == QueueErrorCodeStrings.QueueAlreadyExists))
          return false;
        throw;
      }
      return operationContext.LastResult != null && operationContext.LastResult.HttpStatusCode == 201;
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginCreateIfNotExists(
      AsyncCallback callback,
      object state)
    {
      return this.BeginCreateIfNotExists((QueueRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginCreateIfNotExists(
      QueueRequestOptions options,
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
    public virtual Task<bool> CreateIfNotExistsAsync(CancellationToken cancellationToken) => this.CreateIfNotExistsAsync((QueueRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task<bool> CreateIfNotExistsAsync(
      QueueRequestOptions options,
      OperationContext operationContext)
    {
      return this.CreateIfNotExistsAsync(options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual async Task<bool> CreateIfNotExistsAsync(
      QueueRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      operationContext = operationContext ?? new OperationContext();
      try
      {
        await this.CreateAsync(options, operationContext).ConfigureAwait(false);
      }
      catch (StorageException ex)
      {
        if (ex.RequestInformation.HttpStatusCode == 409 && (ex.RequestInformation.ExtendedErrorInformation == null || ex.RequestInformation.ExtendedErrorInformation.ErrorCode == QueueErrorCodeStrings.QueueAlreadyExists))
          return false;
        throw;
      }
      return operationContext.LastResult != null && operationContext.LastResult.HttpStatusCode == 201;
    }

    [DoesServiceRequest]
    public virtual bool DeleteIfExists(
      QueueRequestOptions options = null,
      OperationContext operationContext = null)
    {
      QueueRequestOptions options1 = QueueRequestOptions.ApplyDefaults(options, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      try
      {
        if (!this.Exists(true, options1, operationContext))
          return false;
      }
      catch (StorageException ex)
      {
        if (ex.RequestInformation.HttpStatusCode != 403)
          throw;
      }
      try
      {
        this.Delete(options1, operationContext);
        return true;
      }
      catch (StorageException ex)
      {
        if (ex.RequestInformation.HttpStatusCode == 404)
        {
          if (ex.RequestInformation.ExtendedErrorInformation == null || ex.RequestInformation.ExtendedErrorInformation.ErrorCode == QueueErrorCodeStrings.QueueNotFound)
            return false;
          throw;
        }
        else
          throw;
      }
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDeleteIfExists(AsyncCallback callback, object state) => this.BeginDeleteIfExists((QueueRequestOptions) null, (OperationContext) null, callback, state);

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDeleteIfExists(
      QueueRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<bool>((Func<CancellationToken, Task<bool>>) (token => this.DeleteIfExistsAsync(options, operationContext, token)), callback, state);
    }

    public virtual bool EndDeleteIfExists(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<bool>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<bool> DeleteIfExistsAsync() => this.DeleteIfExistsAsync(CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<bool> DeleteIfExistsAsync(CancellationToken cancellationToken) => this.DeleteIfExistsAsync((QueueRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task<bool> DeleteIfExistsAsync(
      QueueRequestOptions options,
      OperationContext operationContext)
    {
      return this.DeleteIfExistsAsync(options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual async Task<bool> DeleteIfExistsAsync(
      QueueRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      QueueRequestOptions modifiedOptions = QueueRequestOptions.ApplyDefaults(options, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      try
      {
        if (!await this.ExistsAsync(true, modifiedOptions, operationContext, cancellationToken).ConfigureAwait(false))
          return false;
      }
      catch (StorageException ex)
      {
        if (ex.RequestInformation.HttpStatusCode != 403)
          throw;
      }
      try
      {
        await this.DeleteAsync(modifiedOptions, operationContext, cancellationToken).ConfigureAwait(false);
        return true;
      }
      catch (StorageException ex)
      {
        if (ex.RequestInformation.HttpStatusCode == 404)
        {
          if (ex.RequestInformation.ExtendedErrorInformation == null || ex.RequestInformation.ExtendedErrorInformation.ErrorCode == QueueErrorCodeStrings.QueueNotFound)
            return false;
          throw;
        }
        else
          throw;
      }
    }

    [DoesServiceRequest]
    public virtual void Delete(QueueRequestOptions options = null, OperationContext operationContext = null)
    {
      QueueRequestOptions options1 = QueueRequestOptions.ApplyDefaults(options, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.DeleteQueueImpl(options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDelete(AsyncCallback callback, object state) => this.BeginDelete((QueueRequestOptions) null, (OperationContext) null, callback, state);

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDelete(
      QueueRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.DeleteAsync(options, operationContext, token)), callback, state);
    }

    public virtual void EndDelete(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task DeleteAsync() => this.DeleteAsync(CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task DeleteAsync(CancellationToken cancellationToken) => this.DeleteAsync((QueueRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task DeleteAsync(QueueRequestOptions options, OperationContext operationContext) => this.DeleteAsync(options, operationContext, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task DeleteAsync(
      QueueRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      QueueRequestOptions options1 = QueueRequestOptions.ApplyDefaults(options, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.DeleteQueueImpl(options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual void SetPermissions(
      QueuePermissions permissions,
      QueueRequestOptions options = null,
      OperationContext operationContext = null)
    {
      QueueRequestOptions options1 = QueueRequestOptions.ApplyDefaults(options, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.SetPermissionsImpl(permissions, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginSetPermissions(
      QueuePermissions permissions,
      AsyncCallback callback,
      object state)
    {
      return this.BeginSetPermissions(permissions, (QueueRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginSetPermissions(
      QueuePermissions permissions,
      QueueRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.SetPermissionsAsync(permissions, options, operationContext, token)), callback, state);
    }

    public virtual void EndSetPermissions(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task SetPermissionsAsync(QueuePermissions permissions) => this.SetPermissionsAsync(permissions, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task SetPermissionsAsync(
      QueuePermissions permissions,
      CancellationToken cancellationToken)
    {
      return this.SetPermissionsAsync(permissions, (QueueRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task SetPermissionsAsync(
      QueuePermissions permissions,
      QueueRequestOptions options,
      OperationContext operationContext)
    {
      return this.SetPermissionsAsync(permissions, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task SetPermissionsAsync(
      QueuePermissions permissions,
      QueueRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      QueueRequestOptions options1 = QueueRequestOptions.ApplyDefaults(options, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.SetPermissionsImpl(permissions, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual QueuePermissions GetPermissions(
      QueueRequestOptions options = null,
      OperationContext operationContext = null)
    {
      QueueRequestOptions options1 = QueueRequestOptions.ApplyDefaults(options, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<QueuePermissions>(this.GetPermissionsImpl(options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginGetPermissions(AsyncCallback callback, object state) => this.BeginGetPermissions((QueueRequestOptions) null, (OperationContext) null, callback, state);

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginGetPermissions(
      QueueRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<QueuePermissions>((Func<CancellationToken, Task<QueuePermissions>>) (token => this.GetPermissionsAsync(options, operationContext, token)), callback, state);
    }

    public virtual QueuePermissions EndGetPermissions(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<QueuePermissions>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<QueuePermissions> GetPermissionsAsync() => this.GetPermissionsAsync(CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<QueuePermissions> GetPermissionsAsync(CancellationToken cancellationToken) => this.GetPermissionsAsync((QueueRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task<QueuePermissions> GetPermissionsAsync(
      QueueRequestOptions options,
      OperationContext operationContext)
    {
      return this.GetPermissionsAsync(options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<QueuePermissions> GetPermissionsAsync(
      QueueRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      QueueRequestOptions options1 = QueueRequestOptions.ApplyDefaults(options, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<QueuePermissions>(this.GetPermissionsImpl(options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual bool Exists(QueueRequestOptions options = null, OperationContext operationContext = null) => this.Exists(false, options, operationContext);

    private bool Exists(
      bool primaryOnly,
      QueueRequestOptions options,
      OperationContext operationContext)
    {
      QueueRequestOptions options1 = QueueRequestOptions.ApplyDefaults(options, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<bool>(this.ExistsImpl(options1, primaryOnly), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginExists(AsyncCallback callback, object state) => this.BeginExists((QueueRequestOptions) null, (OperationContext) null, callback, state);

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginExists(
      QueueRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return this.BeginExists(false, options, operationContext, callback, state);
    }

    private ICancellableAsyncResult BeginExists(
      bool primaryOnly,
      QueueRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<bool>((Func<CancellationToken, Task<bool>>) (token => this.ExistsAsync(primaryOnly, options, operationContext, token)), callback, state);
    }

    public virtual bool EndExists(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<bool>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<bool> ExistsAsync() => this.ExistsAsync(CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<bool> ExistsAsync(CancellationToken cancellationToken) => this.ExistsAsync((QueueRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task<bool> ExistsAsync(
      QueueRequestOptions options,
      OperationContext operationContext)
    {
      return this.ExistsAsync(options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<bool> ExistsAsync(
      QueueRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.ExistsAsync(false, options, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    private Task<bool> ExistsAsync(
      bool primaryOnly,
      QueueRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      QueueRequestOptions options1 = QueueRequestOptions.ApplyDefaults(options, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<bool>(this.ExistsImpl(options1, primaryOnly), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual void SetMetadata(QueueRequestOptions options = null, OperationContext operationContext = null)
    {
      QueueRequestOptions options1 = QueueRequestOptions.ApplyDefaults(options, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.SetMetadataImpl(options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginSetMetadata(AsyncCallback callback, object state) => this.BeginSetMetadata((QueueRequestOptions) null, (OperationContext) null, callback, state);

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginSetMetadata(
      QueueRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.SetMetadataAsync(options, operationContext, token)), callback, state);
    }

    public virtual void EndSetMetadata(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task SetMetadataAsync() => this.SetMetadataAsync(CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task SetMetadataAsync(CancellationToken cancellationToken) => this.SetMetadataAsync((QueueRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task SetMetadataAsync(
      QueueRequestOptions options,
      OperationContext operationContext)
    {
      return this.SetMetadataAsync(options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task SetMetadataAsync(
      QueueRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      QueueRequestOptions options1 = QueueRequestOptions.ApplyDefaults(options, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.SetMetadataImpl(options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual void FetchAttributes(
      QueueRequestOptions options = null,
      OperationContext operationContext = null)
    {
      QueueRequestOptions options1 = QueueRequestOptions.ApplyDefaults(options, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.FetchAttributesImpl(options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginFetchAttributes(
      AsyncCallback callback,
      object state)
    {
      return this.BeginFetchAttributes((QueueRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginFetchAttributes(
      QueueRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.FetchAttributesAsync(options, operationContext, token)), callback, state);
    }

    public virtual void EndFetchAttributes(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task FetchAttributesAsync() => this.FetchAttributesAsync(CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task FetchAttributesAsync(CancellationToken cancellationToken) => this.FetchAttributesAsync((QueueRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task FetchAttributesAsync(
      QueueRequestOptions options,
      OperationContext operationContext)
    {
      return this.FetchAttributesAsync(options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task FetchAttributesAsync(
      QueueRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      QueueRequestOptions options1 = QueueRequestOptions.ApplyDefaults(options, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.FetchAttributesImpl(options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual void AddMessage(
      CloudQueueMessage message,
      TimeSpan? timeToLive = null,
      TimeSpan? initialVisibilityDelay = null,
      QueueRequestOptions options = null,
      OperationContext operationContext = null)
    {
      CommonUtility.AssertNotNull(nameof (message), (object) message);
      QueueRequestOptions options1 = QueueRequestOptions.ApplyDefaults(options, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.AddMessageImpl(message, timeToLive, initialVisibilityDelay, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginAddMessage(
      CloudQueueMessage message,
      AsyncCallback callback,
      object state)
    {
      return this.BeginAddMessage(message, new TimeSpan?(), new TimeSpan?(), (QueueRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginAddMessage(
      CloudQueueMessage message,
      TimeSpan? timeToLive,
      TimeSpan? initialVisibilityDelay,
      QueueRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.AddMessageAsync(message, timeToLive, initialVisibilityDelay, options, operationContext, token)), callback, state);
    }

    public virtual void EndAddMessage(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task AddMessageAsync(CloudQueueMessage message) => this.AddMessageAsync(message, new TimeSpan?(), new TimeSpan?(), (QueueRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task AddMessageAsync(
      CloudQueueMessage message,
      CancellationToken cancellationToken)
    {
      return this.AddMessageAsync(message, new TimeSpan?(), new TimeSpan?(), (QueueRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task AddMessageAsync(
      CloudQueueMessage message,
      TimeSpan? timeToLive,
      TimeSpan? initialVisibilityDelay,
      QueueRequestOptions options,
      OperationContext operationContext)
    {
      return this.AddMessageAsync(message, timeToLive, initialVisibilityDelay, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task AddMessageAsync(
      CloudQueueMessage message,
      TimeSpan? timeToLive,
      TimeSpan? initialVisibilityDelay,
      QueueRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      CommonUtility.AssertNotNull(nameof (message), (object) message);
      QueueRequestOptions options1 = QueueRequestOptions.ApplyDefaults(options, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.AddMessageImpl(message, timeToLive, initialVisibilityDelay, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual void UpdateMessage(
      CloudQueueMessage message,
      TimeSpan visibilityTimeout,
      MessageUpdateFields updateFields,
      QueueRequestOptions options = null,
      OperationContext operationContext = null)
    {
      QueueRequestOptions options1 = QueueRequestOptions.ApplyDefaults(options, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.UpdateMessageImpl(message, visibilityTimeout, updateFields, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginUpdateMessage(
      CloudQueueMessage message,
      TimeSpan visibilityTimeout,
      MessageUpdateFields updateFields,
      AsyncCallback callback,
      object state)
    {
      return this.BeginUpdateMessage(message, visibilityTimeout, updateFields, (QueueRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginUpdateMessage(
      CloudQueueMessage message,
      TimeSpan visibilityTimeout,
      MessageUpdateFields updateFields,
      QueueRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.UpdateMessageAsync(message, visibilityTimeout, updateFields, options, operationContext, token)), callback, state);
    }

    public virtual void EndUpdateMessage(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task UpdateMessageAsync(
      CloudQueueMessage message,
      TimeSpan visibilityTimeout,
      MessageUpdateFields updateFields)
    {
      return this.UpdateMessageAsync(message, visibilityTimeout, updateFields, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task UpdateMessageAsync(
      CloudQueueMessage message,
      TimeSpan visibilityTimeout,
      MessageUpdateFields updateFields,
      CancellationToken cancellationToken)
    {
      return this.UpdateMessageAsync(message, visibilityTimeout, updateFields, (QueueRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task UpdateMessageAsync(
      CloudQueueMessage message,
      TimeSpan visibilityTimeout,
      MessageUpdateFields updateFields,
      QueueRequestOptions options,
      OperationContext operationContext)
    {
      return this.UpdateMessageAsync(message, visibilityTimeout, updateFields, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task UpdateMessageAsync(
      CloudQueueMessage message,
      TimeSpan visibilityTimeout,
      MessageUpdateFields updateFields,
      QueueRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      QueueRequestOptions options1 = QueueRequestOptions.ApplyDefaults(options, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.UpdateMessageImpl(message, visibilityTimeout, updateFields, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual void DeleteMessage(
      CloudQueueMessage message,
      QueueRequestOptions options = null,
      OperationContext operationContext = null)
    {
      CommonUtility.AssertNotNull(nameof (message), (object) message);
      this.DeleteMessage(message.Id, message.PopReceipt, options, operationContext);
    }

    [DoesServiceRequest]
    public virtual void DeleteMessage(
      string messageId,
      string popReceipt,
      QueueRequestOptions options = null,
      OperationContext operationContext = null)
    {
      CommonUtility.AssertNotNull(nameof (messageId), (object) messageId);
      CommonUtility.AssertNotNull(nameof (popReceipt), (object) popReceipt);
      QueueRequestOptions options1 = QueueRequestOptions.ApplyDefaults(options, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.DeleteMessageImpl(messageId, popReceipt, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDeleteMessage(
      CloudQueueMessage message,
      AsyncCallback callback,
      object state)
    {
      return this.BeginDeleteMessage(message, (QueueRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDeleteMessage(
      CloudQueueMessage message,
      QueueRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      CommonUtility.AssertNotNull(nameof (message), (object) message);
      return this.BeginDeleteMessage(message.Id, message.PopReceipt, options, operationContext, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDeleteMessage(
      string messageId,
      string popReceipt,
      AsyncCallback callback,
      object state)
    {
      return this.BeginDeleteMessage(messageId, popReceipt, (QueueRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDeleteMessage(
      string messageId,
      string popReceipt,
      QueueRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.DeleteMessageAsync(messageId, popReceipt, options, operationContext, token)), callback, state);
    }

    public virtual void EndDeleteMessage(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task DeleteMessageAsync(CloudQueueMessage message) => this.DeleteMessageAsync(message, (QueueRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task DeleteMessageAsync(
      CloudQueueMessage message,
      CancellationToken cancellationToken)
    {
      return this.DeleteMessageAsync(message, (QueueRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task DeleteMessageAsync(
      CloudQueueMessage message,
      QueueRequestOptions options,
      OperationContext operationContext)
    {
      return this.DeleteMessageAsync(message, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task DeleteMessageAsync(
      CloudQueueMessage message,
      QueueRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.DeleteMessageAsync(message.Id, message.PopReceipt, options, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task DeleteMessageAsync(string messageId, string popReceipt) => this.DeleteMessageAsync(messageId, popReceipt, (QueueRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task DeleteMessageAsync(
      string messageId,
      string popReceipt,
      CancellationToken cancellationToken)
    {
      return this.DeleteMessageAsync(messageId, popReceipt, (QueueRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task DeleteMessageAsync(
      string messageId,
      string popReceipt,
      QueueRequestOptions options,
      OperationContext operationContext)
    {
      return this.DeleteMessageAsync(messageId, popReceipt, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task DeleteMessageAsync(
      string messageId,
      string popReceipt,
      QueueRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      CommonUtility.AssertNotNull(nameof (messageId), (object) messageId);
      CommonUtility.AssertNotNull(nameof (popReceipt), (object) popReceipt);
      QueueRequestOptions options1 = QueueRequestOptions.ApplyDefaults(options, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.DeleteMessageImpl(messageId, popReceipt, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual IEnumerable<CloudQueueMessage> GetMessages(
      int messageCount,
      TimeSpan? visibilityTimeout = null,
      QueueRequestOptions options = null,
      OperationContext operationContext = null)
    {
      QueueRequestOptions options1 = QueueRequestOptions.ApplyDefaults(options, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<IEnumerable<CloudQueueMessage>>(this.GetMessagesImpl(messageCount, visibilityTimeout, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginGetMessages(
      int messageCount,
      AsyncCallback callback,
      object state)
    {
      return this.BeginGetMessages(messageCount, new TimeSpan?(), (QueueRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginGetMessages(
      int messageCount,
      TimeSpan? visibilityTimeout,
      QueueRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<IEnumerable<CloudQueueMessage>>((Func<CancellationToken, Task<IEnumerable<CloudQueueMessage>>>) (token => this.GetMessagesAsync(messageCount, visibilityTimeout, options, operationContext, token)), callback, state);
    }

    public virtual IEnumerable<CloudQueueMessage> EndGetMessages(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<IEnumerable<CloudQueueMessage>>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<IEnumerable<CloudQueueMessage>> GetMessagesAsync(int messageCount) => this.GetMessagesAsync(messageCount, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<IEnumerable<CloudQueueMessage>> GetMessagesAsync(
      int messageCount,
      CancellationToken cancellationToken)
    {
      return this.GetMessagesAsync(messageCount, new TimeSpan?(), (QueueRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<IEnumerable<CloudQueueMessage>> GetMessagesAsync(
      int messageCount,
      TimeSpan? visibilityTimeout,
      QueueRequestOptions options,
      OperationContext operationContext)
    {
      return this.GetMessagesAsync(messageCount, visibilityTimeout, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<IEnumerable<CloudQueueMessage>> GetMessagesAsync(
      int messageCount,
      TimeSpan? visibilityTimeout,
      QueueRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      QueueRequestOptions options1 = QueueRequestOptions.ApplyDefaults(options, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<IEnumerable<CloudQueueMessage>>(this.GetMessagesImpl(messageCount, visibilityTimeout, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual CloudQueueMessage GetMessage(
      TimeSpan? visibilityTimeout = null,
      QueueRequestOptions options = null,
      OperationContext operationContext = null)
    {
      return this.GetMessages(1, visibilityTimeout, options, operationContext).FirstOrDefault<CloudQueueMessage>();
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginGetMessage(AsyncCallback callback, object state) => this.BeginGetMessage(new TimeSpan?(), (QueueRequestOptions) null, (OperationContext) null, callback, state);

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginGetMessage(
      TimeSpan? visibilityTimeout,
      QueueRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<CloudQueueMessage>((Func<CancellationToken, Task<CloudQueueMessage>>) (token => this.GetMessageAsync(visibilityTimeout, options, operationContext, token)), callback, state);
    }

    public virtual CloudQueueMessage EndGetMessage(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<CloudQueueMessage>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<CloudQueueMessage> GetMessageAsync() => this.GetMessageAsync(CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<CloudQueueMessage> GetMessageAsync(CancellationToken cancellationToken) => this.GetMessageAsync(new TimeSpan?(), (QueueRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task<CloudQueueMessage> GetMessageAsync(
      TimeSpan? visibilityTimeout,
      QueueRequestOptions options,
      OperationContext operationContext)
    {
      return this.GetMessageAsync(visibilityTimeout, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<CloudQueueMessage> GetMessageAsync(
      TimeSpan? visibilityTimeout,
      QueueRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      QueueRequestOptions options1 = QueueRequestOptions.ApplyDefaults(options, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<CloudQueueMessage>(this.GetMessageImpl(visibilityTimeout, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual IEnumerable<CloudQueueMessage> PeekMessages(
      int messageCount,
      QueueRequestOptions options = null,
      OperationContext operationContext = null)
    {
      QueueRequestOptions options1 = QueueRequestOptions.ApplyDefaults(options, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<IEnumerable<CloudQueueMessage>>(this.PeekMessagesImpl(messageCount, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginPeekMessages(
      int messageCount,
      AsyncCallback callback,
      object state)
    {
      return this.BeginPeekMessages(messageCount, (QueueRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginPeekMessages(
      int messageCount,
      QueueRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<IEnumerable<CloudQueueMessage>>((Func<CancellationToken, Task<IEnumerable<CloudQueueMessage>>>) (token => this.PeekMessagesAsync(messageCount, options, operationContext, token)), callback, state);
    }

    public virtual IEnumerable<CloudQueueMessage> EndPeekMessages(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<IEnumerable<CloudQueueMessage>>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<IEnumerable<CloudQueueMessage>> PeekMessagesAsync(int messageCount) => this.PeekMessagesAsync(messageCount, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<IEnumerable<CloudQueueMessage>> PeekMessagesAsync(
      int messageCount,
      CancellationToken cancellationToken)
    {
      return this.PeekMessagesAsync(messageCount, (QueueRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<IEnumerable<CloudQueueMessage>> PeekMessagesAsync(
      int messageCount,
      QueueRequestOptions options,
      OperationContext operationContext)
    {
      return this.PeekMessagesAsync(messageCount, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<IEnumerable<CloudQueueMessage>> PeekMessagesAsync(
      int messageCount,
      QueueRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      QueueRequestOptions options1 = QueueRequestOptions.ApplyDefaults(options, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<IEnumerable<CloudQueueMessage>>(this.PeekMessagesImpl(messageCount, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual CloudQueueMessage PeekMessage(
      QueueRequestOptions options = null,
      OperationContext operationContext = null)
    {
      return this.PeekMessages(1, options, operationContext).FirstOrDefault<CloudQueueMessage>();
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginPeekMessage(AsyncCallback callback, object state) => this.BeginPeekMessage((QueueRequestOptions) null, (OperationContext) null, callback, state);

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginPeekMessage(
      QueueRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<CloudQueueMessage>((Func<CancellationToken, Task<CloudQueueMessage>>) (token => this.PeekMessageAsync(options, operationContext, token)), callback, state);
    }

    public virtual CloudQueueMessage EndPeekMessage(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<CloudQueueMessage>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<CloudQueueMessage> PeekMessageAsync() => this.PeekMessageAsync(CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<CloudQueueMessage> PeekMessageAsync(CancellationToken cancellationToken) => this.PeekMessageAsync((QueueRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task<CloudQueueMessage> PeekMessageAsync(
      QueueRequestOptions options,
      OperationContext operationContext)
    {
      return this.PeekMessageAsync(options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<CloudQueueMessage> PeekMessageAsync(
      QueueRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      QueueRequestOptions options1 = QueueRequestOptions.ApplyDefaults(options, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<CloudQueueMessage>(this.PeekMessageImpl(options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual void Clear(QueueRequestOptions options = null, OperationContext operationContext = null)
    {
      QueueRequestOptions options1 = QueueRequestOptions.ApplyDefaults(options, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.ClearMessagesImpl(options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginClear(AsyncCallback callback, object state) => this.BeginClear((QueueRequestOptions) null, (OperationContext) null, callback, state);

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginClear(
      QueueRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.ClearAsync(options, operationContext, token)), callback, state);
    }

    public virtual void EndClear(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task ClearAsync() => this.ClearAsync(CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task ClearAsync(CancellationToken cancellationToken) => this.ClearAsync((QueueRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task ClearAsync(QueueRequestOptions options, OperationContext operationContext) => this.ClearAsync(options, operationContext, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task ClearAsync(
      QueueRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      QueueRequestOptions options1 = QueueRequestOptions.ApplyDefaults(options, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.ClearMessagesImpl(options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    private RESTCommand<NullType> ClearMessagesImpl(QueueRequestOptions options)
    {
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.GetMessageRequestAddress(), this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => QueueHttpRequestMessageFactory.ClearMessages(uri, serverTimeout, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.NoContent, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex));
      return cmd1;
    }

    private RESTCommand<NullType> CreateQueueImpl(QueueRequestOptions options)
    {
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) =>
      {
        StorageRequestMessage request = QueueHttpRequestMessageFactory.Create(uri, serverTimeout, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials);
        QueueHttpRequestMessageFactory.AddMetadata(request, this.Metadata);
        return request;
      });
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(new HttpStatusCode[2]
        {
          HttpStatusCode.Created,
          HttpStatusCode.NoContent
        }, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex);
        return NullType.Value;
      });
      return cmd1;
    }

    private RESTCommand<NullType> DeleteQueueImpl(QueueRequestOptions options)
    {
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => QueueHttpRequestMessageFactory.Delete(uri, serverTimeout, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.NoContent, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex));
      return cmd1;
    }

    private RESTCommand<NullType> FetchAttributesImpl(QueueRequestOptions options)
    {
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.CommandLocationMode = CommandLocationMode.PrimaryOrSecondary;
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => QueueHttpRequestMessageFactory.GetMetadata(uri, serverTimeout, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.OK, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex);
        this.GetMessageCountAndMetadataFromResponse(resp);
        return NullType.Value;
      });
      return cmd1;
    }

    private RESTCommand<bool> ExistsImpl(QueueRequestOptions options, bool primaryOnly)
    {
      RESTCommand<bool> cmd1 = new RESTCommand<bool>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<bool>(cmd1);
      cmd1.CommandLocationMode = primaryOnly ? CommandLocationMode.PrimaryOnly : CommandLocationMode.PrimaryOrSecondary;
      cmd1.BuildRequest = (Func<RESTCommand<bool>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => QueueHttpRequestMessageFactory.GetMetadata(uri, serverTimeout, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<bool>, HttpResponseMessage, Exception, OperationContext, bool>) ((cmd, resp, ex, ctx) =>
      {
        if (resp.StatusCode == HttpStatusCode.NotFound)
          return false;
        return resp.StatusCode == HttpStatusCode.PreconditionFailed || HttpResponseParsers.ProcessExpectedStatusCodeNoException<bool>(HttpStatusCode.OK, resp, true, (StorageCommandBase<bool>) cmd, ex);
      });
      return cmd1;
    }

    private RESTCommand<NullType> SetMetadataImpl(QueueRequestOptions options)
    {
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) =>
      {
        StorageRequestMessage request = QueueHttpRequestMessageFactory.SetMetadata(uri, serverTimeout, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials);
        QueueHttpRequestMessageFactory.AddMetadata(request, this.Metadata);
        return request;
      });
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.NoContent, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex);
        return NullType.Value;
      });
      return cmd1;
    }

    private RESTCommand<NullType> SetPermissionsImpl(
      QueuePermissions acl,
      QueueRequestOptions options)
    {
      MultiBufferMemoryStream memoryStream = new MultiBufferMemoryStream((IBufferManager) null, 1024);
      QueueRequest.WriteSharedAccessIdentifiers(acl.SharedAccessPolicies, (Stream) memoryStream);
      memoryStream.Seek(0L, SeekOrigin.Begin);
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => QueueHttpRequestMessageFactory.SetAcl(uri, serverTimeout, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.BuildContent = (Func<RESTCommand<NullType>, OperationContext, HttpContent>) ((cmd, ctx) => HttpContentFactory.BuildContentFromStream<NullType>((Stream) memoryStream, 0L, new long?(memoryStream.Length), Checksum.None, cmd, ctx));
      cmd1.StreamToDispose = (Stream) memoryStream;
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.NoContent, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex);
        return NullType.Value;
      });
      return cmd1;
    }

    private RESTCommand<QueuePermissions> GetPermissionsImpl(QueueRequestOptions options)
    {
      RESTCommand<QueuePermissions> cmd1 = new RESTCommand<QueuePermissions>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<QueuePermissions>(cmd1);
      cmd1.CommandLocationMode = CommandLocationMode.PrimaryOrSecondary;
      cmd1.RetrieveResponseStream = true;
      cmd1.BuildRequest = (Func<RESTCommand<QueuePermissions>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => QueueHttpRequestMessageFactory.GetAcl(uri, serverTimeout, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<QueuePermissions>, HttpResponseMessage, Exception, OperationContext, QueuePermissions>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<QueuePermissions>(HttpStatusCode.OK, resp, (QueuePermissions) null, (StorageCommandBase<QueuePermissions>) cmd, ex));
      cmd1.PostProcessResponseAsync = (Func<RESTCommand<QueuePermissions>, HttpResponseMessage, OperationContext, CancellationToken, Task<QueuePermissions>>) (async (cmd, resp, ctx, ct) =>
      {
        QueuePermissions queueAcl = new QueuePermissions();
        await QueueHttpResponseParsers.ReadSharedAccessIdentifiersAsync(cmd.ResponseStream, queueAcl, ct).ConfigureAwait(false);
        QueuePermissions permissionsImpl = queueAcl;
        queueAcl = (QueuePermissions) null;
        return permissionsImpl;
      });
      return cmd1;
    }

    private RESTCommand<NullType> AddMessageImpl(
      CloudQueueMessage message,
      TimeSpan? timeToLive,
      TimeSpan? initialVisibilityDelay,
      QueueRequestOptions options)
    {
      long? timeToLiveInSeconds = new long?();
      int? initialVisibilityDelayInSeconds = new int?();
      TimeSpan timeSpan1;
      if (timeToLive.HasValue)
      {
        if (!timeToLive.Value.Equals(TimeSpan.FromSeconds(-1.0)) && !(timeToLive.Value > TimeSpan.Zero))
          throw new ArgumentOutOfRangeException(nameof (timeToLive), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The argument is out of range. Value passed: {0}", (object) timeToLive));
        timeSpan1 = timeToLive.Value;
        timeToLiveInSeconds = new long?((long) timeSpan1.TotalSeconds);
      }
      if (initialVisibilityDelay.HasValue)
      {
        TimeSpan? nullable = new TimeSpan?(timeToLive ?? CloudQueueMessage.MaxVisibilityTimeout);
        TimeSpan timeSpan2;
        if (!(nullable.Value < TimeSpan.Zero))
        {
          timeSpan1 = nullable.Value;
          if (!(timeSpan1.Add(TimeSpan.FromSeconds(-1.0)) > CloudQueueMessage.MaxVisibilityTimeout))
          {
            timeSpan1 = nullable.Value;
            timeSpan2 = timeSpan1.Add(TimeSpan.FromSeconds(-1.0));
            goto label_9;
          }
        }
        timeSpan2 = CloudQueueMessage.MaxVisibilityTimeout;
label_9:
        TimeSpan max = timeSpan2;
        CommonUtility.AssertInBounds<TimeSpan>(nameof (initialVisibilityDelay), initialVisibilityDelay.Value, TimeSpan.Zero, max);
        timeSpan1 = initialVisibilityDelay.Value;
        initialVisibilityDelayInSeconds = new int?((int) timeSpan1.TotalSeconds);
      }
      CommonUtility.AssertNotNull(nameof (message), (object) message);
      MultiBufferMemoryStream memoryStream = new MultiBufferMemoryStream((IBufferManager) null, 1024);
      QueueRequest.WriteMessageContent(message.GetMessageContentForTransfer(this.EncodeMessage, options), (Stream) memoryStream);
      memoryStream.Seek(0L, SeekOrigin.Begin);
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.GetMessageRequestAddress(), this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.RetrieveResponseStream = true;
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => QueueHttpRequestMessageFactory.AddMessage(uri, serverTimeout, timeToLiveInSeconds, initialVisibilityDelayInSeconds, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.BuildContent = (Func<RESTCommand<NullType>, OperationContext, HttpContent>) ((cmd, ctx) => HttpContentFactory.BuildContentFromStream<NullType>((Stream) memoryStream, 0L, new long?(memoryStream.Length), Checksum.None, cmd, ctx));
      cmd1.StreamToDispose = (Stream) memoryStream;
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.Created, resp, (NullType) null, (StorageCommandBase<NullType>) cmd, ex));
      cmd1.PostProcessResponseAsync = (Func<RESTCommand<NullType>, HttpResponseMessage, OperationContext, CancellationToken, Task<NullType>>) (async (cmd, resp, ctx, ct) =>
      {
        IEnumerable<QueueMessage> source = await GetMessagesResponse.ParseAsync(cmd.ResponseStream, ct).ConfigureAwait(false);
        CloudQueue.CopyMessage(message, source.First<QueueMessage>());
        return NullType.Value;
      });
      return cmd1;
    }

    private RESTCommand<NullType> UpdateMessageImpl(
      CloudQueueMessage message,
      TimeSpan visibilityTimeout,
      MessageUpdateFields updateFields,
      QueueRequestOptions options)
    {
      CommonUtility.AssertNotNull(nameof (message), (object) message);
      CommonUtility.AssertNotNullOrEmpty("messageId", message.Id);
      CommonUtility.AssertNotNullOrEmpty("popReceipt", message.PopReceipt);
      CommonUtility.AssertInBounds<TimeSpan>(nameof (visibilityTimeout), visibilityTimeout, TimeSpan.Zero, CloudQueueMessage.MaxVisibilityTimeout);
      if ((updateFields & MessageUpdateFields.Visibility) == (MessageUpdateFields) 0)
        throw new ArgumentException("Calls to UpdateMessage must include the Visibility flag.", nameof (updateFields));
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.GetIndividualMessageAddress(message.Id), this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => QueueHttpRequestMessageFactory.UpdateMessage(uri, serverTimeout, message.PopReceipt, new TimeSpan?(visibilityTimeout), cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      if ((updateFields & MessageUpdateFields.Content) != (MessageUpdateFields) 0)
      {
        MultiBufferMemoryStream memoryStream = new MultiBufferMemoryStream(this.ServiceClient.BufferManager);
        QueueRequest.WriteMessageContent(message.GetMessageContentForTransfer(this.EncodeMessage, options), (Stream) memoryStream);
        memoryStream.Seek(0L, SeekOrigin.Begin);
        cmd1.BuildContent = (Func<RESTCommand<NullType>, OperationContext, HttpContent>) ((cmd, ctx) => HttpContentFactory.BuildContentFromStream<NullType>((Stream) memoryStream, 0L, new long?(memoryStream.Length), Checksum.None, cmd, ctx));
        cmd1.StreamToDispose = (Stream) memoryStream;
      }
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.NoContent, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex);
        CloudQueue.GetPopReceiptAndNextVisibleTimeFromResponse(message, resp);
        return NullType.Value;
      });
      return cmd1;
    }

    private RESTCommand<CloudQueueMessage> GetMessageImpl(
      TimeSpan? visibilityTimeout,
      QueueRequestOptions options)
    {
      options.AssertPolicyIfRequired();
      RESTCommand<CloudQueueMessage> cmd1 = new RESTCommand<CloudQueueMessage>(this.ServiceClient.Credentials, this.GetMessageRequestAddress(), this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<CloudQueueMessage>(cmd1);
      cmd1.RetrieveResponseStream = true;
      cmd1.BuildRequest = (Func<RESTCommand<CloudQueueMessage>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => QueueHttpRequestMessageFactory.GetMessages(uri, serverTimeout, 1, visibilityTimeout, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<CloudQueueMessage>, HttpResponseMessage, Exception, OperationContext, CloudQueueMessage>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<CloudQueueMessage>(HttpStatusCode.OK, resp, (CloudQueueMessage) null, (StorageCommandBase<CloudQueueMessage>) cmd, ex));
      Func<QueueMessage, CloudQueueMessage> func;
      cmd1.PostProcessResponseAsync = (Func<RESTCommand<CloudQueueMessage>, HttpResponseMessage, OperationContext, CancellationToken, Task<CloudQueueMessage>>) (async (cmd, resp, ctx, ct) => (await GetMessagesResponse.ParseAsync(cmd.ResponseStream, ct).ConfigureAwait(false)).Select<QueueMessage, CloudQueueMessage>(func ?? (func = (Func<QueueMessage, CloudQueueMessage>) (m => this.SelectGetMessageResponse(m, options)))).FirstOrDefault<CloudQueueMessage>());
      return cmd1;
    }

    private RESTCommand<CloudQueueMessage> PeekMessageImpl(QueueRequestOptions options)
    {
      options.AssertPolicyIfRequired();
      RESTCommand<CloudQueueMessage> cmd1 = new RESTCommand<CloudQueueMessage>(this.ServiceClient.Credentials, this.GetMessageRequestAddress(), this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<CloudQueueMessage>(cmd1);
      cmd1.RetrieveResponseStream = true;
      cmd1.BuildRequest = (Func<RESTCommand<CloudQueueMessage>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => QueueHttpRequestMessageFactory.PeekMessages(uri, serverTimeout, 1, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<CloudQueueMessage>, HttpResponseMessage, Exception, OperationContext, CloudQueueMessage>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<CloudQueueMessage>(HttpStatusCode.OK, resp, (CloudQueueMessage) null, (StorageCommandBase<CloudQueueMessage>) cmd, ex));
      Func<QueueMessage, CloudQueueMessage> func;
      cmd1.PostProcessResponseAsync = (Func<RESTCommand<CloudQueueMessage>, HttpResponseMessage, OperationContext, CancellationToken, Task<CloudQueueMessage>>) (async (cmd, resp, ctx, ct) => (await GetMessagesResponse.ParseAsync(cmd.ResponseStream, ct).ConfigureAwait(false)).Select<QueueMessage, CloudQueueMessage>(func ?? (func = (Func<QueueMessage, CloudQueueMessage>) (m => this.SelectPeekMessageResponse(m, options)))).FirstOrDefault<CloudQueueMessage>());
      return cmd1;
    }

    private RESTCommand<NullType> DeleteMessageImpl(
      string messageId,
      string popReceipt,
      QueueRequestOptions options)
    {
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.GetIndividualMessageAddress(messageId), this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => QueueHttpRequestMessageFactory.DeleteMessage(uri, serverTimeout, popReceipt, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.NoContent, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex));
      return cmd1;
    }

    private RESTCommand<IEnumerable<CloudQueueMessage>> GetMessagesImpl(
      int messageCount,
      TimeSpan? visibilityTimeout,
      QueueRequestOptions options)
    {
      options.AssertPolicyIfRequired();
      RESTCommand<IEnumerable<CloudQueueMessage>> cmd1 = new RESTCommand<IEnumerable<CloudQueueMessage>>(this.ServiceClient.Credentials, this.GetMessageRequestAddress(), this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<IEnumerable<CloudQueueMessage>>(cmd1);
      cmd1.RetrieveResponseStream = true;
      cmd1.BuildRequest = (Func<RESTCommand<IEnumerable<CloudQueueMessage>>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => QueueHttpRequestMessageFactory.GetMessages(uri, serverTimeout, messageCount, visibilityTimeout, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<IEnumerable<CloudQueueMessage>>, HttpResponseMessage, Exception, OperationContext, IEnumerable<CloudQueueMessage>>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<IEnumerable<CloudQueueMessage>>(HttpStatusCode.OK, resp, (IEnumerable<CloudQueueMessage>) null, (StorageCommandBase<IEnumerable<CloudQueueMessage>>) cmd, ex));
      Func<QueueMessage, CloudQueueMessage> func;
      cmd1.PostProcessResponseAsync = (Func<RESTCommand<IEnumerable<CloudQueueMessage>>, HttpResponseMessage, OperationContext, CancellationToken, Task<IEnumerable<CloudQueueMessage>>>) (async (cmd, resp, ctx, ct) => (IEnumerable<CloudQueueMessage>) (await GetMessagesResponse.ParseAsync(cmd.ResponseStream, ct).ConfigureAwait(false)).Select<QueueMessage, CloudQueueMessage>(func ?? (func = (Func<QueueMessage, CloudQueueMessage>) (m => this.SelectGetMessageResponse(m, options)))).ToList<CloudQueueMessage>());
      return cmd1;
    }

    private RESTCommand<IEnumerable<CloudQueueMessage>> PeekMessagesImpl(
      int messageCount,
      QueueRequestOptions options)
    {
      RESTCommand<IEnumerable<CloudQueueMessage>> cmd1 = new RESTCommand<IEnumerable<CloudQueueMessage>>(this.ServiceClient.Credentials, this.GetMessageRequestAddress(), this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<IEnumerable<CloudQueueMessage>>(cmd1);
      cmd1.CommandLocationMode = CommandLocationMode.PrimaryOrSecondary;
      cmd1.RetrieveResponseStream = true;
      cmd1.BuildRequest = (Func<RESTCommand<IEnumerable<CloudQueueMessage>>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => QueueHttpRequestMessageFactory.PeekMessages(uri, serverTimeout, messageCount, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<IEnumerable<CloudQueueMessage>>, HttpResponseMessage, Exception, OperationContext, IEnumerable<CloudQueueMessage>>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<IEnumerable<CloudQueueMessage>>(HttpStatusCode.OK, resp, (IEnumerable<CloudQueueMessage>) null, (StorageCommandBase<IEnumerable<CloudQueueMessage>>) cmd, ex));
      Func<QueueMessage, CloudQueueMessage> func;
      cmd1.PostProcessResponseAsync = (Func<RESTCommand<IEnumerable<CloudQueueMessage>>, HttpResponseMessage, OperationContext, CancellationToken, Task<IEnumerable<CloudQueueMessage>>>) (async (cmd, resp, ctx, ct) => (IEnumerable<CloudQueueMessage>) (await GetMessagesResponse.ParseAsync(cmd.ResponseStream, ct).ConfigureAwait(false)).Select<QueueMessage, CloudQueueMessage>(func ?? (func = (Func<QueueMessage, CloudQueueMessage>) (m => this.SelectPeekMessageResponse(m, options)))).ToList<CloudQueueMessage>());
      return cmd1;
    }

    private void GetMessageCountAndMetadataFromResponse(HttpResponseMessage webResponse)
    {
      this.Metadata = QueueHttpResponseParsers.GetMetadata(webResponse);
      string approximateMessageCount = QueueHttpResponseParsers.GetApproximateMessageCount(webResponse);
      this.ApproximateMessageCount = string.IsNullOrEmpty(approximateMessageCount) ? new int?() : new int?(int.Parse(approximateMessageCount, (IFormatProvider) CultureInfo.InvariantCulture));
    }

    private static void GetPopReceiptAndNextVisibleTimeFromResponse(
      CloudQueueMessage message,
      HttpResponseMessage webResponse)
    {
      message.PopReceipt = webResponse.Headers.GetHeaderSingleValueOrDefault("x-ms-popreceipt");
      message.NextVisibleTime = new DateTimeOffset?((DateTimeOffset) DateTime.Parse(webResponse.Headers.GetHeaderSingleValueOrDefault("x-ms-time-next-visible"), (IFormatProvider) DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal));
    }

    public CloudQueue(Uri queueAddress)
      : this(queueAddress, (StorageCredentials) null)
    {
    }

    public CloudQueue(Uri queueAddress, StorageCredentials credentials)
      : this(new StorageUri(queueAddress), credentials)
    {
    }

    public CloudQueue(StorageUri queueAddress, StorageCredentials credentials)
    {
      this.ParseQueryAndVerify(queueAddress, credentials);
      this.Metadata = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.EncodeMessage = true;
    }

    internal CloudQueue(string queueName, CloudQueueClient serviceClient)
      : this((IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase), queueName, serviceClient)
    {
    }

    internal CloudQueue(
      IDictionary<string, string> metadata,
      string queueName,
      CloudQueueClient serviceClient)
    {
      this.StorageUri = NavigationHelper.AppendPathToUri(serviceClient.StorageUri, queueName);
      this.ServiceClient = serviceClient;
      this.Name = queueName;
      this.Metadata = metadata;
      this.EncodeMessage = true;
    }

    public CloudQueueClient ServiceClient { get; private set; }

    public Uri Uri => this.StorageUri.PrimaryUri;

    public StorageUri StorageUri { get; private set; }

    public string Name { get; private set; }

    public int? ApproximateMessageCount { get; private set; }

    public bool EncodeMessage { get; set; }

    public IDictionary<string, string> Metadata { get; private set; }

    internal StorageUri GetMessageRequestAddress()
    {
      if (this.messageRequestAddress == (StorageUri) null)
        this.messageRequestAddress = NavigationHelper.AppendPathToUri(this.StorageUri, "messages");
      return this.messageRequestAddress;
    }

    internal StorageUri GetIndividualMessageAddress(string messageId) => NavigationHelper.AppendPathToUri(this.GetMessageRequestAddress(), messageId);

    private void ParseQueryAndVerify(StorageUri address, StorageCredentials credentials)
    {
      StorageCredentials parsedCredentials;
      this.StorageUri = NavigationHelper.ParseQueueTableQueryAndVerify(address, out parsedCredentials);
      if (parsedCredentials != null && credentials != null && !credentials.Equals(new StorageCredentials()))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot provide credentials as part of the address and as constructor parameter. Either pass in the address or use a different constructor."));
      this.ServiceClient = new CloudQueueClient(NavigationHelper.GetServiceClientBaseAddress(this.StorageUri, new bool?()), credentials ?? parsedCredentials);
      this.Name = NavigationHelper.GetQueueNameFromUri(this.Uri, new bool?(this.ServiceClient.UsePathStyleUris));
    }

    private string GetCanonicalName() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/{0}/{1}/{2}", (object) "queue", (object) this.ServiceClient.Credentials.AccountName, (object) this.Name);

    private static void CopyMessage(CloudQueueMessage message, QueueMessage protocolMessage)
    {
      message.InsertionTime = protocolMessage.InsertionTime;
      message.ExpirationTime = protocolMessage.ExpirationTime;
      message.NextVisibleTime = new DateTimeOffset?(protocolMessage.NextVisibleTime.Value);
      message.PopReceipt = protocolMessage.PopReceipt;
      message.Id = protocolMessage.Id;
    }

    private CloudQueueMessage SelectGetMessageResponse(
      QueueMessage protocolMessage,
      QueueRequestOptions options = null)
    {
      CloudQueueMessage messageResponse = this.SelectPeekMessageResponse(protocolMessage, options);
      messageResponse.PopReceipt = protocolMessage.PopReceipt;
      if (protocolMessage.NextVisibleTime.HasValue)
        messageResponse.NextVisibleTime = new DateTimeOffset?(protocolMessage.NextVisibleTime.Value);
      return messageResponse;
    }

    private CloudQueueMessage SelectPeekMessageResponse(
      QueueMessage protocolMessage,
      QueueRequestOptions options)
    {
      byte[] numArray = (byte[]) null;
      if (options != null && options.EncryptionPolicy != null)
        numArray = options.EncryptionPolicy.DecryptMessage(protocolMessage.Text, options.RequireEncryption);
      CloudQueueMessage cloudQueueMessage;
      if (this.EncodeMessage)
      {
        if (numArray != null)
          protocolMessage.Text = Convert.ToBase64String(numArray, 0, numArray.Length);
        cloudQueueMessage = new CloudQueueMessage(protocolMessage.Text, true);
      }
      else
        cloudQueueMessage = numArray == null ? new CloudQueueMessage(protocolMessage.Text) : new CloudQueueMessage(numArray);
      cloudQueueMessage.Id = protocolMessage.Id;
      cloudQueueMessage.InsertionTime = protocolMessage.InsertionTime;
      cloudQueueMessage.ExpirationTime = protocolMessage.ExpirationTime;
      cloudQueueMessage.DequeueCount = protocolMessage.DequeueCount;
      return cloudQueueMessage;
    }

    public string GetSharedAccessSignature(SharedAccessQueuePolicy policy) => this.GetSharedAccessSignature(policy, (string) null);

    public string GetSharedAccessSignature(
      SharedAccessQueuePolicy policy,
      string accessPolicyIdentifier)
    {
      return this.GetSharedAccessSignature(policy, accessPolicyIdentifier, new SharedAccessProtocol?(), (IPAddressOrRange) null);
    }

    public string GetSharedAccessSignature(
      SharedAccessQueuePolicy policy,
      string accessPolicyIdentifier,
      SharedAccessProtocol? protocols,
      IPAddressOrRange ipAddressOrRange)
    {
      if (!this.ServiceClient.Credentials.IsSharedKey)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot create Shared Access Signature unless Account Key credentials are used."));
      string canonicalName = this.GetCanonicalName();
      StorageAccountKey key = this.ServiceClient.Credentials.Key;
      string hash = QueueSharedAccessSignatureHelper.GetHash(policy, accessPolicyIdentifier, canonicalName, "2019-07-07", protocols, ipAddressOrRange, key.KeyValue);
      string keyName = key.KeyName;
      return QueueSharedAccessSignatureHelper.GetSignature(policy, accessPolicyIdentifier, hash, keyName, "2019-07-07", protocols, ipAddressOrRange).ToString();
    }
  }
}
