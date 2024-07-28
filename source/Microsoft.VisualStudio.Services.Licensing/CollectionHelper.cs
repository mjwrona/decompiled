// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.CollectionHelper
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Licensing
{
  internal static class CollectionHelper
  {
    private static readonly string Area = "Licensing";
    private static readonly string Layer = nameof (CollectionHelper);

    internal static void WithCollectionContext(
      IVssRequestContext requestContext,
      Guid hostId,
      Action<IVssRequestContext> action,
      [CallerMemberName] string method = null)
    {
      requestContext.TraceEnter(1059520, CollectionHelper.Area, CollectionHelper.Layer, nameof (WithCollectionContext));
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        HostProperties hostProperties = vssRequestContext.GetService<ITeamFoundationHostManagementService>().QueryServiceHostPropertiesCached(vssRequestContext, hostId);
        if (hostProperties == null)
          throw new HostDoesNotExistException(hostId);
        if (hostProperties.HostType.HasFlag((Enum) TeamFoundationHostType.Deployment))
          throw new UnexpectedHostTypeException(hostProperties.HostType);
        if (hostProperties.HostType == TeamFoundationHostType.Application)
        {
          requestContext.Trace(1059523, TraceLevel.Warning, CollectionHelper.Area, CollectionHelper.Layer, string.Format("{0} received Organization host ID {1}. Caller: {2}", (object) nameof (WithCollectionContext), (object) hostId, (object) method));
          Guid defaultCollectionId = CollectionHelper.GetDefaultCollectionId(requestContext, hostId);
          using (VssRequestContextLicensingExtensions.VssRequestContextHolder collection = requestContext.ToCollection(defaultCollectionId))
            action(collection.RequestContext);
        }
        else
        {
          using (VssRequestContextLicensingExtensions.VssRequestContextHolder collection = requestContext.ToCollection(hostId))
            action(collection.RequestContext);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1059521, CollectionHelper.Area, CollectionHelper.Layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1059522, CollectionHelper.Area, CollectionHelper.Layer, nameof (WithCollectionContext));
      }
    }

    internal static string GetCollectionName(IVssRequestContext requestContext)
    {
      CollectionHelper.ValidateCollectionContext(requestContext);
      return CollectionHelper.GetCollection(requestContext)?.Name;
    }

    internal static Guid GetCollectionOwner(IVssRequestContext requestContext)
    {
      CollectionHelper.ValidateCollectionContext(requestContext);
      Collection collection = CollectionHelper.GetCollection(requestContext);
      return collection == null ? Guid.Empty : collection.Owner;
    }

    internal static Guid GetDefaultCollectionId(IVssRequestContext requestContext, Guid hostId)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      ITeamFoundationHostManagementService service = vssRequestContext.GetService<ITeamFoundationHostManagementService>();
      IEnumerable<HostProperties> hostPropertieses = service.QueryChildrenServiceHostPropertiesCached(vssRequestContext, hostId);
      if (hostPropertieses.IsNullOrEmpty<HostProperties>())
      {
        if (service.QueryServiceHostPropertiesCached(vssRequestContext, hostId).HostType == TeamFoundationHostType.ProjectCollection)
          return hostId;
        string message = string.Format("Did not find any child collection hosts for parent host {0} and also it is not a collection host id.", (object) hostId);
        requestContext.Trace(1059420, TraceLevel.Error, CollectionHelper.Area, CollectionHelper.Layer, message);
        throw new ArgumentException(message, nameof (hostId));
      }
      if (hostPropertieses.Count<HostProperties>() != 1)
      {
        string message = string.Format("Expected to find only one child collection for parent host {0} but found {1} instead.", (object) requestContext.ServiceHost.InstanceId, (object) hostPropertieses.Count<HostProperties>());
        requestContext.Trace(1059420, TraceLevel.Error, CollectionHelper.Area, CollectionHelper.Layer, message);
      }
      return hostPropertieses.First<HostProperties>().Id;
    }

    internal static bool IsCollectionEnabled(IVssRequestContext requestContext)
    {
      CollectionHelper.ValidateCollectionContext(requestContext);
      Collection collection = CollectionHelper.GetCollection(requestContext);
      return collection != null && collection.Status == CollectionStatus.Enabled;
    }

    private static Collection GetCollection(IVssRequestContext collectionContext)
    {
      collectionContext.CheckProjectCollectionRequestContext();
      return collectionContext.GetService<ICollectionService>().GetCollection(collectionContext.Elevate(), (IEnumerable<string>) null);
    }

    private static void ValidateCollectionContext(IVssRequestContext requestContext)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        throw new InvalidRequestContextHostException("Expected a hosted deployment.");
      requestContext.CheckProjectCollectionRequestContext();
    }
  }
}
