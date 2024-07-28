// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.AzureActiveDirectory
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Aad;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal class AzureActiveDirectory : IDirectory
  {
    internal static readonly VssPerformanceCounter s_elevatedRequestsPerfCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Directories.DiscoveryService.PerfCounters.DiscoveryService.AzureDirectory.Elevated.Requests");

    public string Name => "aad";

    public DirectoryInternalConvertKeysResponse ConvertKeys(
      IVssRequestContext context,
      DirectoryInternalConvertKeysRequest request)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return AzureActiveDirectory.Execute<DirectoryInternalConvertKeysRequest, DirectoryInternalConvertKeysResponse>(AzureActiveDirectory.\u003C\u003EO.\u003C0\u003E__ConvertKeys ?? (AzureActiveDirectory.\u003C\u003EO.\u003C0\u003E__ConvertKeys = new Func<IVssRequestContext, DirectoryInternalConvertKeysRequest, DirectoryInternalConvertKeysResponse>(AzureActiveDirectoryConvertKeysHelper.ConvertKeys)), context, request);
    }

    public DirectoryInternalGetAvatarsResponse GetAvatars(
      IVssRequestContext context,
      DirectoryInternalGetAvatarsRequest request)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return AzureActiveDirectory.Execute<DirectoryInternalGetAvatarsRequest, DirectoryInternalGetAvatarsResponse>(AzureActiveDirectory.\u003C\u003EO.\u003C1\u003E__GetAvatars ?? (AzureActiveDirectory.\u003C\u003EO.\u003C1\u003E__GetAvatars = new Func<IVssRequestContext, DirectoryInternalGetAvatarsRequest, DirectoryInternalGetAvatarsResponse>(AzureActiveDirectoryGetAvatarsHelper.GetAvatars)), context, request);
    }

    public DirectoryInternalGetEntitiesResponse GetEntities(
      IVssRequestContext context,
      DirectoryInternalGetEntitiesRequest request)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return AzureActiveDirectory.Execute<DirectoryInternalGetEntitiesRequest, DirectoryInternalGetEntitiesResponse>(AzureActiveDirectory.\u003C\u003EO.\u003C2\u003E__GetEntities ?? (AzureActiveDirectory.\u003C\u003EO.\u003C2\u003E__GetEntities = new Func<IVssRequestContext, DirectoryInternalGetEntitiesRequest, DirectoryInternalGetEntitiesResponse>(AzureActiveDirectoryGetEntitiesHelper.GetEntities)), context, request);
    }

    public DirectoryInternalGetRelatedEntitiesResponse GetRelatedEntities(
      IVssRequestContext context,
      DirectoryInternalGetRelatedEntitiesRequest request)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return AzureActiveDirectory.Execute<DirectoryInternalGetRelatedEntitiesRequest, DirectoryInternalGetRelatedEntitiesResponse>(AzureActiveDirectory.\u003C\u003EO.\u003C3\u003E__GetRelatedEntities ?? (AzureActiveDirectory.\u003C\u003EO.\u003C3\u003E__GetRelatedEntities = new Func<IVssRequestContext, DirectoryInternalGetRelatedEntitiesRequest, DirectoryInternalGetRelatedEntitiesResponse>(AzureActiveDirectoryGetRelatedEntitiesHelper.GetRelatedEntities)), context, request);
    }

    public DirectoryInternalGetRelatedEntityIdsResponse GetRelatedEntityIds(
      IVssRequestContext context,
      DirectoryInternalGetRelatedEntityIdsRequest request)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return AzureActiveDirectory.Execute<DirectoryInternalGetRelatedEntityIdsRequest, DirectoryInternalGetRelatedEntityIdsResponse>(AzureActiveDirectory.\u003C\u003EO.\u003C4\u003E__GetRelatedEntityIds ?? (AzureActiveDirectory.\u003C\u003EO.\u003C4\u003E__GetRelatedEntityIds = new Func<IVssRequestContext, DirectoryInternalGetRelatedEntityIdsRequest, DirectoryInternalGetRelatedEntityIdsResponse>(AzureActiveDirectoryGetRelatedEntityIdsHelper.GetRelatedEntityIds)), context, request);
    }

    public DirectoryInternalSearchResponse Search(
      IVssRequestContext context,
      DirectoryInternalSearchRequest request)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return AzureActiveDirectory.Execute<DirectoryInternalSearchRequest, DirectoryInternalSearchResponse>(AzureActiveDirectory.\u003C\u003EO.\u003C5\u003E__Search ?? (AzureActiveDirectory.\u003C\u003EO.\u003C5\u003E__Search = new Func<IVssRequestContext, DirectoryInternalSearchRequest, DirectoryInternalSearchResponse>(AzureActiveDirectorySearchHelper.Search)), context, request);
    }

    private static TResponse Execute<TRequest, TResponse>(
      Func<IVssRequestContext, TRequest, TResponse> function,
      IVssRequestContext context,
      TRequest request)
      where TRequest : DirectoryInternalRequest
      where TResponse : DirectoryInternalResponse
    {
      try
      {
        return AzureActiveDirectory.ExecuteInternal<TRequest, TResponse>(function, context, request);
      }
      catch (AadAccessException ex)
      {
        throw new DirectoryDiscoveryServiceAccessException("AAD threw Access exception.", (Exception) ex);
      }
      catch (AadCredentialsNotFoundException ex)
      {
        throw new DirectoryDiscoveryServiceAccessException("AAD threw CredentialsNotFound exception.", (Exception) ex);
      }
      catch (AadGraphException ex)
      {
        if (ex.InnerException is AuthorizationException)
        {
          if (DirectoryUtils.IsRequestByAadGuestUser(context))
            throw new DirectoryDiscoveryServiceGuestAccessException("Guest users do not have access to browse AAD users.", (Exception) ex);
          throw new DirectoryDiscoveryServiceAccessException("AAD threw GraphClient Authorization exception.", (Exception) ex);
        }
        throw;
      }
      catch (AadServiceNotAvailableException ex)
      {
        throw new DirectoryDiscoveryServiceNotAvailable("AAD Service is not available.", (Exception) ex);
      }
      catch (AadException ex)
      {
        throw new DirectoryDiscoveryServiceException("AAD threw a generic exception.", (Exception) ex);
      }
    }

    private static TResponse ExecuteInternal<TRequest, TResponse>(
      Func<IVssRequestContext, TRequest, TResponse> function,
      IVssRequestContext context,
      TRequest request)
      where TRequest : DirectoryInternalRequest
      where TResponse : DirectoryInternalResponse
    {
      TResponse response = default (TResponse);
      try
      {
        return function(context, request);
      }
      catch (AadException ex)
      {
        switch (ex)
        {
          case AadAccessException _:
          case AadCredentialsNotFoundException _:
            if (DirectoryUtils.IsRequestByAadGuestUser(context))
            {
              context.Trace(15001008, TraceLevel.Info, "VisualStudio.Services.DirectoryDiscovery", "Service", ex.Message);
              throw new DirectoryDiscoveryServiceGuestAccessException("Guest users do not have access to browse AAD users.", (Exception) ex);
            }
            if (AzureActiveDirectory.TryElevatedExecute<TRequest, TResponse>(function, context, request, out response))
            {
              context.Trace(15004007, TraceLevel.Info, "VisualStudio.Services.DirectoryDiscovery", "Service", ex.Message);
              return response;
            }
            throw;
          default:
            throw;
        }
      }
    }

    private static bool TryElevatedExecute<TRequest, TResponse>(
      Func<IVssRequestContext, TRequest, TResponse> function,
      IVssRequestContext context,
      TRequest request,
      out TResponse response)
      where TRequest : DirectoryInternalRequest
      where TResponse : DirectoryInternalResponse
    {
      response = default (TResponse);
      if (context.IsFeatureEnabled("VisualStudio.Services.IdentityPicker.DisableUidElevation"))
        return false;
      AzureActiveDirectory.s_elevatedRequestsPerfCounter.Increment();
      IVssRequestContext vssRequestContext = context.Elevate();
      response = function(vssRequestContext, request);
      return true;
    }
  }
}
