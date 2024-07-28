// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.FrameworkDirectoryDiscoveryService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class FrameworkDirectoryDiscoveryService : DirectoryDiscoveryService
  {
    private Guid m_serviceHostId;
    private IDisposableReadOnlyList<IDirectory> m_directories;

    public override void ServiceStart(IVssRequestContext context)
    {
      this.m_serviceHostId = context.ServiceHost.InstanceId;
      this.ValidateContext(context);
      this.m_directories = context.GetExtensions<IDirectory>();
      if (this.m_directories.IsNullOrEmpty<IDirectory>())
        throw new DirectoryDiscoveryServiceException("Could not find any directories.");
    }

    public override void ServiceEnd(IVssRequestContext context)
    {
      if (this.m_directories == null)
        return;
      this.m_directories.Dispose();
      this.m_directories = (IDisposableReadOnlyList<IDirectory>) null;
    }

    public override DirectoryConvertKeysResponse ConvertKeys(
      IVssRequestContext context,
      DirectoryConvertKeysRequest request)
    {
      return (DirectoryConvertKeysResponse) this.ProcessRequest(context, (DirectoryRequest) request, nameof (ConvertKeys));
    }

    public override DirectoryGetAvatarsResponse GetAvatars(
      IVssRequestContext context,
      DirectoryGetAvatarsRequest request)
    {
      return (DirectoryGetAvatarsResponse) this.ProcessRequest(context, (DirectoryRequest) request, nameof (GetAvatars));
    }

    public override DirectoryGetEntitiesResponse GetEntities(
      IVssRequestContext context,
      DirectoryGetEntitiesRequest request)
    {
      return (DirectoryGetEntitiesResponse) this.ProcessRequest(context, (DirectoryRequest) request, nameof (GetEntities));
    }

    public override DirectoryGetRelatedEntitiesResponse GetRelatedEntities(
      IVssRequestContext context,
      DirectoryGetRelatedEntitiesRequest request)
    {
      return (DirectoryGetRelatedEntitiesResponse) this.ProcessRequest(context, (DirectoryRequest) request, nameof (GetRelatedEntities));
    }

    public override DirectoryGetRelatedEntityIdsResponse GetRelatedEntityIds(
      IVssRequestContext context,
      DirectoryGetRelatedEntityIdsRequest request)
    {
      return (DirectoryGetRelatedEntityIdsResponse) this.ProcessRequest(context, (DirectoryRequest) request, nameof (GetRelatedEntityIds));
    }

    public override DirectorySearchResponse Search(
      IVssRequestContext context,
      DirectorySearchRequest request)
    {
      return (DirectorySearchResponse) this.ProcessRequest(context, (DirectoryRequest) request, nameof (Search));
    }

    internal override DirectoryGetEntitiesResponse GetEntitiesInternal(
      IVssRequestContext context,
      DirectoryGetEntitiesInternalRequest request)
    {
      return (DirectoryGetEntitiesResponse) this.ProcessRequest(context, (DirectoryRequest) request, nameof (GetEntitiesInternal));
    }

    private DirectoryResponse ProcessRequest(
      IVssRequestContext context,
      DirectoryRequest request,
      [CallerMemberName] string operation = "")
    {
      this.ValidateContext(context);
      try
      {
        context.TraceEnter(15001001, "VisualStudio.Services.DirectoryDiscovery", "Service", operation);
        FrameworkDirectoryDiscoveryService.ValidateRequest(request);
        try
        {
          return request.Execute(context, (IEnumerable<IDirectory>) this.m_directories);
        }
        catch (Exception ex)
        {
          context.TraceException(15001008, TraceLevel.Error, "VisualStudio.Services.DirectoryDiscovery", "Service", ex);
          throw;
        }
      }
      finally
      {
        context.TraceLeave(15001009, "VisualStudio.Services.DirectoryDiscovery", "Service", operation);
      }
    }

    private void ValidateContext(IVssRequestContext context)
    {
      if (context == null)
        throw new DirectoryDiscoveryServiceClientException("Request context must be non-null.");
      context.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);
      if (context.ServiceHost.Is(TeamFoundationHostType.Deployment) && context.ExecutionEnvironment.IsHostedDeployment)
        throw new DirectoryDiscoveryServiceClientException("This service is not available at deployment level in a hosted environment.");
    }

    private static void ValidateRequest(DirectoryRequest request)
    {
      if (request == null)
        throw new DirectoryDiscoveryServiceClientException("Request must be non-null.");
      request.Validate();
    }
  }
}
