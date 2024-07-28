// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CollectionHelper
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Commerce.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.Security;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal static class CollectionHelper
  {
    private const string Area = "Commerce";
    private const string Layer = "CollectionHelper";

    internal static void WithCollectionContext(
      IVssRequestContext requestContext,
      Guid hostId,
      Action<IVssRequestContext> action,
      RequestContextType? requestContextType = null,
      [CallerMemberName] string method = null)
    {
      requestContext.TraceEnter(5108822, "Commerce", nameof (CollectionHelper), nameof (WithCollectionContext));
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        HostProperties hostProperties = vssRequestContext.GetService<CommerceHostManagementService>().QueryServiceHostPropertiesCached(vssRequestContext, hostId);
        if (hostProperties == null)
          throw new HostDoesNotExistException(hostId);
        if (hostProperties.HostType.HasFlag((Enum) TeamFoundationHostType.Deployment))
          throw new UnexpectedHostTypeException(hostProperties.HostType);
        if (hostProperties.HostType == TeamFoundationHostType.Application)
        {
          requestContext.Trace(5108878, TraceLevel.Warning, "Commerce", nameof (CollectionHelper), string.Format("{0} received Organization host ID {1}. Caller: {2}", (object) nameof (WithCollectionContext), (object) hostId, (object) method));
          Guid defaultCollectionId = CollectionHelper.GetDefaultCollectionId(requestContext, hostId);
          using (CommerceVssRequestContextExtensions.VssRequestContextHolder collection = requestContext.ToCollection(defaultCollectionId, requestContextType))
          {
            CollectionHelper.CopyRequestContextItems(requestContext, collection.RequestContext);
            action(collection.RequestContext);
          }
        }
        else
        {
          using (CommerceVssRequestContextExtensions.VssRequestContextHolder collection = requestContext.ToCollection(hostId, (IdentityDescriptor) null, requestContextType))
          {
            CollectionHelper.CopyRequestContextItems(requestContext, collection.RequestContext);
            action(collection.RequestContext);
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5108823, "Commerce", nameof (CollectionHelper), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5108824, "Commerce", nameof (CollectionHelper), nameof (WithCollectionContext));
      }
    }

    internal static void WithCollectionContext(
      IVssRequestContext requestContext,
      Guid hostId,
      Action<IVssRequestContext, Microsoft.VisualStudio.Services.Identity.Identity> action,
      bool checkHostType,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      [CallerMemberName] string method = null)
    {
      requestContext.TraceEnter(5108825, "Commerce", nameof (CollectionHelper), nameof (WithCollectionContext));
      Guid hostId1 = hostId;
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        CommerceHostManagementService service = vssRequestContext.GetService<CommerceHostManagementService>();
        service.EnsureHostUpdated(requestContext, hostId);
        if (checkHostType)
        {
          HostProperties hostProperties = service.QueryServiceHostPropertiesCached(vssRequestContext, hostId);
          if (hostProperties == null)
            throw new HostDoesNotExistException(hostId);
          if (hostProperties.HostType.HasFlag((Enum) TeamFoundationHostType.Deployment))
            throw new UnexpectedHostTypeException(hostProperties.HostType);
          if (hostProperties.HostType == TeamFoundationHostType.Application)
          {
            requestContext.Trace(5108878, TraceLevel.Warning, "Commerce", nameof (CollectionHelper), string.Format("{0} received Organization host ID {1}. Caller: {2}", (object) nameof (WithCollectionContext), (object) hostId, (object) method));
            hostId1 = CollectionHelper.GetDefaultCollectionId(requestContext, hostId);
          }
        }
        bool flag = ServicePrincipals.IsServicePrincipal(requestContext, identity.Descriptor);
        try
        {
          IdentityDescriptor identityDescriptor = (IdentityDescriptor) null;
          if (!flag)
            identityDescriptor = identity.Descriptor;
          using (CommerceVssRequestContextExtensions.VssRequestContextHolder collection = requestContext.ToCollection(hostId1, identityDescriptor))
          {
            CollectionHelper.CopyRequestContextItems(requestContext, collection.RequestContext);
            action(collection.RequestContext, identity);
          }
        }
        catch (Exception ex) when (
        {
          // ISSUE: unable to correctly present filter
          int num;
          switch (ex)
          {
            case IdentityNotFoundException _:
            case SecurityException _:
              num = 1;
              break;
            case VssServiceResponseException _:
              num = ex.Message.Contains("identifier '" + identity.Descriptor.Identifier + "' could not be found") ? 1 : 0;
              break;
            default:
              num = 0;
              break;
          }
          if ((uint) num > 0U)
          {
            SuccessfulFiltering;
          }
          else
            throw;
        }
        )
        {
          if (flag)
          {
            throw;
          }
          else
          {
            Microsoft.VisualStudio.Services.Identity.Identity primaryMsaIdentity = requestContext.GetService<IdentityService>().GetPrimaryMsaIdentity(requestContext, (IReadOnlyVssIdentity) identity);
            if (primaryMsaIdentity != null)
            {
              Guid instanceId = requestContext.To(TeamFoundationHostType.Deployment).ServiceHost.InstanceId;
              using (CommerceVssRequestContextExtensions.VssRequestContextHolder collection1 = requestContext.ToCollection(instanceId, (IdentityDescriptor) null, new RequestContextType?(RequestContextType.SystemContext)))
              {
                using (CommerceVssRequestContextExtensions.VssRequestContextHolder collection2 = collection1.RequestContext.ToCollection(hostId1, primaryMsaIdentity.Descriptor))
                {
                  CollectionHelper.CopyRequestContextItems(requestContext, collection2.RequestContext);
                  action(collection2.RequestContext, primaryMsaIdentity);
                }
              }
            }
            else
              throw;
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5108826, "Commerce", nameof (CollectionHelper), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5108830, "Commerce", nameof (CollectionHelper), nameof (WithCollectionContext));
      }
    }

    internal static Collection GetCollection(
      IVssRequestContext requestContext,
      Guid collectionId,
      IEnumerable<string> propertyNames = null)
    {
      Collection collection = (Collection) null;
      if (requestContext.ServiceHost.HostType == TeamFoundationHostType.ProjectCollection)
        collection = requestContext.GetService<ICollectionService>().GetCollection(requestContext, propertyNames);
      else if (requestContext.RootContext.ServiceHost.HostType == TeamFoundationHostType.ProjectCollection)
        collection = requestContext.RootContext.GetService<ICollectionService>().GetCollection(requestContext.RootContext, propertyNames);
      else
        CollectionHelper.WithCollectionContext(requestContext, collectionId, (Action<IVssRequestContext>) (collectionContext => collection = collectionContext.GetService<ICollectionService>().GetCollection(collectionContext, propertyNames)), method: nameof (GetCollection));
      return collection;
    }

    internal static string GetCollectionName(IVssRequestContext collectionContext) => CollectionHelper.GetCollectionDetails(collectionContext).Name;

    internal static CollectionHelper.CollectionDetails GetCollectionDetails(
      IVssRequestContext collectionContext)
    {
      CollectionHelper.CollectionDetails collectionDetails = new CollectionHelper.CollectionDetails();
      CollectionHelper.ValidateCollectionContext(collectionContext);
      Collection collection = collectionContext.GetService<ICollectionService>().GetCollection(collectionContext, (IEnumerable<string>) null);
      if (ServiceHostTags.FromString(collectionContext.ServiceHost.ParentServiceHost.Description).HasTag(CommerceWellKnownServiceHostTags.AssociatedWithOnPremisesCollection))
        collectionDetails.Name = CollectionHelper.GetConnectedOnPremHostName(collectionContext);
      collectionDetails.PreferredRegion = collection?.PreferredRegion ?? string.Empty;
      if (string.IsNullOrEmpty(collectionDetails.Name))
        collectionDetails.Name = collectionContext.ServiceHost.Name;
      collectionContext.Trace(5108828, TraceLevel.Info, "Commerce", nameof (CollectionHelper), "Collection name " + collectionDetails.Name + ", preferred region " + collectionDetails.PreferredRegion);
      return collectionDetails;
    }

    internal static Guid GetParentOrganizationId(IVssRequestContext requestContext, Guid hostId)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      HostProperties hostProperties = vssRequestContext.GetService<CommerceHostManagementService>().QueryServiceHostPropertiesCached(vssRequestContext, hostId);
      if (hostProperties == null)
        throw new HostDoesNotExistException(hostId);
      return hostProperties.HostType == TeamFoundationHostType.ProjectCollection ? hostProperties.ParentId : hostId;
    }

    internal static Guid GetDefaultCollectionId(IVssRequestContext requestContext)
    {
      CollectionHelper.ValidateNonDeploymentRequestContext(requestContext);
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        return requestContext.ServiceHost.InstanceId;
      if (requestContext.RootContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        return requestContext.RootContext.ServiceHost.InstanceId;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IEnumerable<HostProperties> hostPropertieses = vssRequestContext.GetService<CommerceHostManagementService>().QueryChildrenServiceHostPropertiesCached(vssRequestContext, requestContext.ServiceHost.InstanceId);
      if (hostPropertieses.IsNullOrEmpty<HostProperties>())
      {
        string message = string.Format("Did not find any child collection hosts for parent host {0}.", (object) requestContext.ServiceHost.InstanceId);
        requestContext.Trace(5108827, TraceLevel.Error, "Commerce", nameof (CollectionHelper), message);
        throw new InvalidOperationException(message);
      }
      if (hostPropertieses.Count<HostProperties>() != 1)
      {
        string message = string.Format("Expected to find only one child collection for parent host {0} but found {1} instead.", (object) requestContext.ServiceHost.InstanceId, (object) hostPropertieses.Count<HostProperties>());
        requestContext.Trace(5108827, TraceLevel.Error, "Commerce", nameof (CollectionHelper), message);
      }
      return hostPropertieses.First<HostProperties>().Id;
    }

    internal static Guid GetDefaultCollectionId(IVssRequestContext requestContext, Guid hostId)
    {
      ArgumentUtility.CheckForEmptyGuid(hostId, nameof (hostId));
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      CommerceHostManagementService service = vssRequestContext.GetService<CommerceHostManagementService>();
      IEnumerable<HostProperties> hostPropertieses = service.QueryChildrenServiceHostPropertiesCached(vssRequestContext, hostId);
      if (hostPropertieses.IsNullOrEmpty<HostProperties>())
      {
        HostProperties hostProperties = service.QueryServiceHostPropertiesCached(vssRequestContext, hostId);
        if (hostProperties == null)
          throw new HostDoesNotExistException(hostId);
        if (hostProperties.HostType == TeamFoundationHostType.ProjectCollection)
          return hostId;
        if (hostProperties.HostType == TeamFoundationHostType.Application)
        {
          hostPropertieses = (IEnumerable<HostProperties>) service.QueryServiceHostProperties(vssRequestContext, hostId, ServiceHostFilterFlags.IncludeChildren).Children;
          if (hostPropertieses.IsNullOrEmpty<HostProperties>())
          {
            string message = string.Format("Did not find any child collection hosts for parent host {0} and also it is not a collection host id. Host type is {1}.", (object) hostId, (object) hostProperties.HostType);
            requestContext.Trace(533617, TraceLevel.Error, "Commerce", nameof (CollectionHelper), message);
            throw new ArgumentException(message, nameof (hostId));
          }
        }
        else
        {
          string message = string.Format("Invalid host type found. Host id: {0} Host type : {1}.", (object) hostId, (object) hostProperties.HostType);
          requestContext.Trace(533619, TraceLevel.Error, "Commerce", nameof (CollectionHelper), message);
          throw new ArgumentException(message, nameof (hostId));
        }
      }
      if (hostPropertieses.Count<HostProperties>() != 1)
      {
        string message = string.Format("Expected to find only one child collection for parent host {0} but found {1} instead.", (object) hostId, (object) hostPropertieses.Count<HostProperties>());
        requestContext.Trace(5108827, TraceLevel.Error, "Commerce", nameof (CollectionHelper), message);
      }
      return hostPropertieses.First<HostProperties>().Id;
    }

    internal static bool TryGetHostProperties(
      IVssRequestContext requestContext,
      Guid hostId,
      out TeamFoundationHostType hostType,
      out Guid parentHostId)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      HostProperties hostProperties = vssRequestContext.GetService<CommerceHostManagementService>().QueryServiceHostPropertiesCached(vssRequestContext, hostId);
      if (hostProperties == null)
      {
        string message = string.Format("Invalid host. Did not find any properties for {0}", (object) hostId);
        requestContext.Trace(5108833, TraceLevel.Error, "Commerce", nameof (CollectionHelper), message);
        hostType = TeamFoundationHostType.Unknown;
        parentHostId = Guid.Empty;
        return false;
      }
      hostType = hostProperties.HostType;
      parentHostId = hostProperties.ParentId;
      return true;
    }

    internal static void FaultInCollection(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      vssRequestContext.GetService<ILocationService>().FindServiceDefinitionWithFaultIn(vssRequestContext, "LocationService2", new Guid("5D4A2F52-5A08-41FB-8CCA-768ADD070E18"), false);
    }

    private static string GetConnectedOnPremHostName(IVssRequestContext requestContext)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        return (string) null;
      CollectionHelper.ValidateNonDeploymentRequestContext(requestContext);
      if (requestContext.IsCommerceService())
      {
        IVssRequestContext context = requestContext.To(TeamFoundationHostType.Application).Elevate();
        CommerceHostHelperHttpClient client = context.GetClient<CommerceHostHelperHttpClient>();
        Guid serverPropertyKind = CommerceConstants.ConnectedServerPropertyKind;
        List<string> properties = new List<string>();
        properties.Add(ConnectedServerConstants.ConnectedServerNameProperty);
        properties.Add(ConnectedServerConstants.ConnectedServerHostNameProperty);
        IVssRequestContext userState = context;
        CancellationToken cancellationToken = new CancellationToken();
        List<string> stringList = client.GetInfrastructureOrganizationProperties(serverPropertyKind, (IEnumerable<string>) properties, (object) userState, cancellationToken).SyncResult<List<string>>();
        if (stringList != null)
          return "TFS:" + stringList[0] + "/" + stringList[1];
        requestContext.Trace(5108833, TraceLevel.Error, "Commerce", nameof (CollectionHelper), string.Format("Can't retrieve properties for linked onprem host {0}", (object) requestContext.ServiceHost.InstanceId));
        return (string) null;
      }
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      HostProperties hostProperties = vssRequestContext.GetService<CommerceHostManagementService>().QueryServiceHostPropertiesCached(vssRequestContext, requestContext.ServiceHost.InstanceId);
      if (hostProperties == null)
        throw new HostDoesNotExistException(requestContext.ServiceHost.InstanceId);
      Guid organizationHostId = requestContext.ServiceHost.HostType == TeamFoundationHostType.ProjectCollection ? hostProperties.ParentId : hostProperties.Id;
      using (CommerceVssRequestContextExtensions.VssRequestContextHolder organization = requestContext.ToOrganization(organizationHostId))
      {
        IVssRequestContext requestContext1 = organization.RequestContext;
        CommercePropertyStore commercePropertyStore = new CommercePropertyStore();
        if (!commercePropertyStore.HasPropertyKind(requestContext1, CommerceConstants.ConnectedServerPropertyKind))
          return (string) null;
        PropertiesCollection properties = commercePropertyStore.GetProperties(requestContext1, CommerceConstants.ConnectedServerPropertyKind);
        string str1;
        properties.TryGetValue<string>(ConnectedServerConstants.ConnectedServerNameProperty, out str1);
        string str2;
        properties.TryGetValue<string>(ConnectedServerConstants.ConnectedServerHostNameProperty, out str2);
        return str1 != null && str2 != null ? "TFS:" + str1 + "/" + str2 : (string) null;
      }
    }

    private static void ValidateNonDeploymentRequestContext(IVssRequestContext requestContext)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        throw new InvalidRequestContextHostException("Expected a hosted deployment.");
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new InvalidRequestContextHostException("Expected a non-deployment request context");
    }

    private static void ValidateCollectionContext(IVssRequestContext requestContext)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        throw new InvalidRequestContextHostException("Expected a hosted deployment.");
      requestContext.CheckProjectCollectionRequestContext();
    }

    private static void CopyRequestContextItems(
      IVssRequestContext originalRequestContext,
      IVssRequestContext newRequestContext)
    {
      string str;
      if (originalRequestContext.TryGetItem<string>("Commerce.RequestSource", out str) && !newRequestContext.Items.ContainsKey("Commerce.RequestSource"))
        newRequestContext.Items.Add("Commerce.RequestSource", (object) str);
      bool flag;
      if (!originalRequestContext.TryGetItem<bool>("Commerce.DualWrite", out flag) || newRequestContext.Items.ContainsKey("Commerce.DualWrite"))
        return;
      newRequestContext.Items.Add("Commerce.DualWrite", (object) flag);
    }

    internal class CollectionDetails
    {
      public string Name { get; set; }

      public string PreferredRegion { get; set; }
    }
  }
}
