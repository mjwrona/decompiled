// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TeamProjectCollectionPropertiesServiceBase
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core
{
  public abstract class TeamProjectCollectionPropertiesServiceBase : 
    ITeamProjectCollectionPropertiesService,
    IVssFrameworkService
  {
    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext) => this.CheckRequestContextType(systemRequestContext);

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public TeamProjectCollectionProperties GetCollectionProperties(
      IVssRequestContext requestContext,
      Guid collectionId,
      ServiceHostFilterFlags filterFlags,
      bool includeProcessCustomization = false)
    {
      this.ValidateArguments(requestContext, collectionId);
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      ITeamFoundationHostManagementService service = vssRequestContext.GetService<ITeamFoundationHostManagementService>();
      filterFlags &= ~ServiceHostFilterFlags.IncludeChildren;
      IVssRequestContext requestContext1 = vssRequestContext;
      Guid hostId = collectionId;
      int filterFlags1 = (int) filterFlags;
      TeamFoundationServiceHostProperties serviceHostProperties = service.QueryServiceHostProperties(requestContext1, hostId, (ServiceHostFilterFlags) filterFlags1);
      ISqlConnectionInfo defaultConnectionInfo = this.GetDefaultConnectionInfo(collectionId, vssRequestContext, (HostProperties) serviceHostProperties);
      if (!includeProcessCustomization)
        return new TeamProjectCollectionProperties(serviceHostProperties, defaultConnectionInfo, false);
      ProcessCustomizationType customizationType = this.IsInheritedProcessEnabledOnCollection(vssRequestContext, collectionId);
      return (TeamProjectCollectionProperties) new TfsTeamProjectCollectionProperties(serviceHostProperties, defaultConnectionInfo, false)
      {
        EnableInheritedProcessCustomization = customizationType
      };
    }

    public TeamProjectCollectionProperties GetCollectionPropertiesCached(
      IVssRequestContext requestContext,
      Guid collectionId,
      bool returnProcessCustomizationProperty = false)
    {
      this.ValidateArguments(requestContext, collectionId);
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      ITeamFoundationHostManagementService service = vssRequestContext.GetService<ITeamFoundationHostManagementService>();
      HostProperties hostProperties = (HostProperties) null;
      if (this.TryCheckGetCollectionPropertiesPermission(requestContext))
        hostProperties = service.QueryServiceHostPropertiesCached(vssRequestContext, collectionId);
      ISqlConnectionInfo defaultConnectionInfo = this.GetDefaultConnectionInfo(collectionId, vssRequestContext, hostProperties);
      if (!returnProcessCustomizationProperty)
        return new TeamProjectCollectionProperties(hostProperties, defaultConnectionInfo);
      ProcessCustomizationType customizationType = this.IsInheritedProcessEnabledOnCollection(vssRequestContext, collectionId);
      return (TeamProjectCollectionProperties) new TfsTeamProjectCollectionProperties(hostProperties, defaultConnectionInfo)
      {
        EnableInheritedProcessCustomization = customizationType
      };
    }

    public List<TeamProjectCollectionProperties> GetCollectionProperties(
      IVssRequestContext requestContext,
      ServiceHostFilterFlags filterFlags)
    {
      return this.GetCollectionProperties(requestContext, (IList<Guid>) null, filterFlags);
    }

    public List<TeamProjectCollectionProperties> GetCollectionPropertiesCached(
      IVssRequestContext requestContext)
    {
      return this.GetCollectionPropertiesCached(requestContext, (IList<Guid>) null);
    }

    public List<TeamProjectCollectionProperties> GetCollectionProperties(
      IVssRequestContext requestContext,
      IList<Guid> collectionIds,
      ServiceHostFilterFlags filterFlags)
    {
      return this.GetCollectionPropertiesInternal(requestContext, collectionIds, filterFlags, false);
    }

    public List<TeamProjectCollectionProperties> GetCollectionPropertiesCached(
      IVssRequestContext requestContext,
      IList<Guid> collectionIds)
    {
      return this.GetCollectionPropertiesInternal(requestContext, collectionIds, ServiceHostFilterFlags.None, true);
    }

    private List<TeamProjectCollectionProperties> GetCollectionPropertiesInternal(
      IVssRequestContext requestContext,
      IList<Guid> collectionIds,
      ServiceHostFilterFlags filterFlags,
      bool useCache)
    {
      this.CheckRequestContextType(requestContext);
      if (!this.TryCheckGetCollectionPropertiesPermission(requestContext))
        return new List<TeamProjectCollectionProperties>();
      IEnumerable<HostProperties> hostPropertieses = useCache ? this.GetCollectionHostPropertiesCached(requestContext) : (IEnumerable<HostProperties>) this.GetCollectionHostProperties(requestContext, filterFlags);
      HashSet<Guid> guidSet = collectionIds == null || collectionIds.Count == 0 ? (HashSet<Guid>) null : new HashSet<Guid>((IEnumerable<Guid>) collectionIds);
      Dictionary<Guid, TeamProjectCollectionProperties> dictionary = new Dictionary<Guid, TeamProjectCollectionProperties>();
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      ITeamFoundationDatabaseManagementService service = vssRequestContext.GetService<ITeamFoundationDatabaseManagementService>();
      foreach (HostProperties hostProperties in hostPropertieses)
      {
        if (hostProperties.HostType == TeamFoundationHostType.ProjectCollection && (collectionIds == null || collectionIds.Contains(hostProperties.Id)))
        {
          ISqlConnectionInfo connectionInfo = (ISqlConnectionInfo) null;
          if (hostProperties.DatabaseId != DatabaseManagementConstants.InvalidDatabaseId)
          {
            try
            {
              connectionInfo = service.GetSqlConnectionInfo(vssRequestContext, hostProperties.DatabaseId);
            }
            catch (DatabaseNotFoundException ex)
            {
            }
          }
          dictionary[hostProperties.Id] = TeamProjectCollectionPropertiesServiceBase.BuildCollectionProperties(hostProperties, connectionInfo);
        }
      }
      if (guidSet == null)
        return dictionary.Values.ToList<TeamProjectCollectionProperties>();
      List<TeamProjectCollectionProperties> propertiesInternal = new List<TeamProjectCollectionProperties>(collectionIds.Count);
      foreach (Guid collectionId in (IEnumerable<Guid>) collectionIds)
      {
        TeamProjectCollectionProperties collectionProperties;
        propertiesInternal.Add(dictionary.TryGetValue(collectionId, out collectionProperties) ? collectionProperties : (TeamProjectCollectionProperties) null);
      }
      return propertiesInternal;
    }

    private void ValidateArguments(IVssRequestContext requestContext, Guid collectionId)
    {
      this.CheckRequestContextType(requestContext);
      IVssServiceHost serviceHost = requestContext.RootContext.ServiceHost;
      if (serviceHost.Is(TeamFoundationHostType.ProjectCollection) && !object.Equals((object) serviceHost.InstanceId, (object) collectionId))
        throw new CollectionDoesNotExistException(collectionId);
    }

    private ISqlConnectionInfo GetDefaultConnectionInfo(
      Guid collectionId,
      IVssRequestContext deploymentRequestContext,
      HostProperties hostProperties)
    {
      if (hostProperties == null)
        throw new CollectionDoesNotExistException(collectionId);
      ITeamFoundationDatabaseManagementService service = deploymentRequestContext.GetService<ITeamFoundationDatabaseManagementService>();
      ISqlConnectionInfo defaultConnectionInfo = (ISqlConnectionInfo) null;
      if (hostProperties.DatabaseId != DatabaseManagementConstants.InvalidDatabaseId)
        defaultConnectionInfo = service.GetSqlConnectionInfo(deploymentRequestContext, hostProperties.DatabaseId);
      return defaultConnectionInfo;
    }

    private void CheckRequestContextType(IVssRequestContext requestContext)
    {
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
        requestContext.CheckOrganizationOnlyRequestContext();
      else
        requestContext.CheckOrganizationRequestContext();
    }

    private static TeamProjectCollectionProperties BuildCollectionProperties(
      HostProperties hostProperties,
      ISqlConnectionInfo connectionInfo)
    {
      return hostProperties is TeamFoundationServiceHostProperties serviceHostProperties ? new TeamProjectCollectionProperties(serviceHostProperties, connectionInfo, false) : new TeamProjectCollectionProperties(hostProperties, connectionInfo);
    }

    private ProcessCustomizationType IsInheritedProcessEnabledOnCollection(
      IVssRequestContext enterpriseRequestContext,
      Guid collectionId)
    {
      ProcessCustomizationType customizationType = ProcessCustomizationType.Unknown;
      IVssRequestContext vssRequestContext = enterpriseRequestContext.To(TeamFoundationHostType.Deployment);
      ITeamFoundationHostManagementService service = vssRequestContext.GetService<ITeamFoundationHostManagementService>();
      try
      {
        using (IVssRequestContext requestContext = service.BeginRequest(vssRequestContext, collectionId, RequestContextType.UserContext, throwIfShutdown: false))
          customizationType = requestContext.IsFeatureEnabled("WebAccess.Process.Hierarchy") ? ProcessCustomizationType.Inherited : ProcessCustomizationType.Xml;
      }
      catch (Exception ex)
      {
        enterpriseRequestContext.Trace(54007, TraceLevel.Error, nameof (IsInheritedProcessEnabledOnCollection), nameof (TeamProjectCollectionPropertiesServiceBase), string.Format("Error in finding process customisation on a collection with Id: {0}. error: {1}", (object) collectionId, (object) ex.Message));
      }
      return customizationType;
    }

    protected abstract IEnumerable<TeamFoundationServiceHostProperties> GetCollectionHostProperties(
      IVssRequestContext requestContext,
      ServiceHostFilterFlags filterFlags);

    protected abstract IEnumerable<HostProperties> GetCollectionHostPropertiesCached(
      IVssRequestContext requestContext);

    protected abstract bool TryCheckGetCollectionPropertiesPermission(
      IVssRequestContext requestContext);
  }
}
