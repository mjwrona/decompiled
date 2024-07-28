// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.FrameworkGraphIdentifierConversionService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.ConfigFramework;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.Partitioning.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Graph
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class FrameworkGraphIdentifierConversionService : GraphIdentifierConversionServiceBase
  {
    public FrameworkGraphIdentifierConversionService()
    {
    }

    public FrameworkGraphIdentifierConversionService(IConfigQueryable<bool> respectSubjectType)
      : base(respectSubjectType)
    {
    }

    private Guid GetStorageKeyByDescriptorFromRemote(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor)
    {
      requestContext.TraceEnter(10007215, "Graph", "GraphIdentifierConversionServiceBase", nameof (GetStorageKeyByDescriptorFromRemote));
      try
      {
        GraphStorageKeyResult storageKeyResult = this.GetSpsHttpClient<GraphHttpClient>(requestContext.Elevate()).GetStorageKeyAsync((string) descriptor).SyncResult<GraphStorageKeyResult>();
        Guid result = storageKeyResult != null ? storageKeyResult.Value : Guid.Empty;
        requestContext.TraceDataConditionally(10007216, TraceLevel.Info, "Graph", "GraphIdentifierConversionServiceBase", "Result from GetStorageKeyByDescriptorFromRemote http client call", (Func<object>) (() => (object) result), nameof (GetStorageKeyByDescriptorFromRemote));
        return result;
      }
      catch (StorageKeyNotFoundException ex)
      {
        requestContext.Trace(10007217, TraceLevel.Info, "Graph", "GraphIdentifierConversionServiceBase", "StorageKeyNotFoundException from GetStorageKeyByDescriptorFromRemote; returning default value");
        return new Guid();
      }
      finally
      {
        requestContext.TraceLeave(10007215, "Graph", "GraphIdentifierConversionServiceBase", nameof (GetStorageKeyByDescriptorFromRemote));
      }
    }

    protected override Guid GetStorageKeyByGroupScopeDescriptor(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor)
    {
      return this.GetStorageKeyByDescriptorFromRemote(requestContext, descriptor);
    }

    protected override IdentityKeyMap GetKeyMapByCuidBasedDescriptor(
      IVssRequestContext organizationContext,
      SubjectDescriptor descriptor)
    {
      try
      {
        organizationContext.TraceEnter(10007200, "Graph", "GraphIdentifierConversionServiceBase", nameof (GetKeyMapByCuidBasedDescriptor));
        organizationContext.CheckOrganizationOnlyRequestContext();
        Guid cuid = descriptor.GetCuid();
        if (!this.IsExchangableId(cuid))
          return (IdentityKeyMap) null;
        IGraphIdentifierConversionCacheService service = organizationContext.GetService<IGraphIdentifierConversionCacheService>();
        IdentityKeyMap identityKeyMap;
        if (service.TryGetIdentityKeyMapByCuid(organizationContext, cuid, out identityKeyMap))
        {
          organizationContext.TraceDataConditionally(10007202, TraceLevel.Info, "Graph", "GraphIdentifierConversionServiceBase", "Found IdentityKeyMap in Framework cache", (Func<object>) (() => (object) identityKeyMap), nameof (GetKeyMapByCuidBasedDescriptor));
          if (IdentityIdChecker.IsCuid(identityKeyMap.StorageKey))
            organizationContext.TraceDataConditionally(10007204, TraceLevel.Error, "Graph", "GraphIdentifierConversionServiceBase", "Retrieved an IdentityKeyMap from Framework Cache with a StorageKey which is actually a CUID;", (Func<object>) (() => (object) identityKeyMap), nameof (GetKeyMapByCuidBasedDescriptor));
          return identityKeyMap;
        }
        Guid storageKey = this.GetStorageKeyByDescriptorFromRemote(organizationContext, descriptor);
        if (storageKey == Guid.Empty)
        {
          organizationContext.TraceDataConditionally(10007206, TraceLevel.Info, "Graph", "GraphIdentifierConversionServiceBase", "StorageKey not found in remote", (Func<object>) (() => (object) new
          {
            storageKey = storageKey,
            cuid = cuid,
            SubjectType = descriptor.SubjectType,
            descriptor = descriptor
          }), nameof (GetKeyMapByCuidBasedDescriptor));
          return (IdentityKeyMap) null;
        }
        identityKeyMap = new IdentityKeyMap()
        {
          StorageKey = storageKey,
          Cuid = descriptor.GetCuid(),
          SubjectType = descriptor.SubjectType
        };
        service.AddIdentityKeyMap(organizationContext, identityKeyMap);
        organizationContext.TraceDataConditionally(10007203, TraceLevel.Info, "Graph", "GraphIdentifierConversionServiceBase", "Retrieved StorageKey from remote and built new IdentityKeyMap", (Func<object>) (() => (object) identityKeyMap), nameof (GetKeyMapByCuidBasedDescriptor));
        if (IdentityIdChecker.IsCuid(identityKeyMap.StorageKey))
          organizationContext.TraceDataConditionally(10007205, TraceLevel.Error, "Graph", "GraphIdentifierConversionServiceBase", "Retrieved an IdentityKeyMap from SPS with a StorageKey which is actually a CUID", (Func<object>) (() => (object) identityKeyMap), nameof (GetKeyMapByCuidBasedDescriptor));
        return identityKeyMap;
      }
      finally
      {
        organizationContext.TraceLeave(10007201, "Graph", "GraphIdentifierConversionServiceBase", nameof (GetKeyMapByCuidBasedDescriptor));
      }
    }

    protected override SubjectDescriptor GetDescriptorByStorageKeyFromRemote(
      IVssRequestContext collectionOrOrganizationContext,
      Guid storageKey)
    {
      collectionOrOrganizationContext.TraceEnter(10007212, "Graph", "GraphIdentifierConversionServiceBase", nameof (GetDescriptorByStorageKeyFromRemote));
      collectionOrOrganizationContext.CheckProjectCollectionOrOrganizationRequestContext();
      try
      {
        GraphDescriptorResult descriptorResult = this.GetSpsHttpClient<GraphHttpClient>(collectionOrOrganizationContext.Elevate()).GetDescriptorAsync(storageKey).SyncResult<GraphDescriptorResult>();
        SubjectDescriptor result = descriptorResult != null ? descriptorResult.Value : new SubjectDescriptor();
        collectionOrOrganizationContext.TraceDataConditionally(10007213, TraceLevel.Info, "Graph", "GraphIdentifierConversionServiceBase", "Result from GetDescriptorByStorageKeyFromRemote http client call", (Func<object>) (() => (object) result), nameof (GetDescriptorByStorageKeyFromRemote));
        return result;
      }
      catch (SubjectDescriptorNotFoundException ex)
      {
        collectionOrOrganizationContext.Trace(10007214, TraceLevel.Info, "Graph", "GraphIdentifierConversionServiceBase", "SubjectDescriptorNotFoundException from GetDescriptorByStorageKeyFromRemote; returning default value");
        return new SubjectDescriptor();
      }
      finally
      {
        collectionOrOrganizationContext.TraceLeave(10007212, "Graph", "GraphIdentifierConversionServiceBase", nameof (GetDescriptorByStorageKeyFromRemote));
      }
    }

    internal virtual TClient GetSpsHttpClient<TClient>(IVssRequestContext requestContext) where TClient : class, IVssHttpClient => !requestContext.RootContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) ? PartitionedClientHelper.GetSpsClientForHostId<TClient>(requestContext, requestContext.RootContext.ServiceHost.InstanceId) : requestContext.GetClient<TClient>();
  }
}
