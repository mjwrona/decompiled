// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.WebServices.LocationWebService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core.Location;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Services;

namespace Microsoft.TeamFoundation.Server.Core.WebServices
{
  [WebService(Namespace = "http://microsoft.com/webservices/")]
  [ClientService(ComponentName = "Framework", RegistrationName = "Framework", ServerConfiguration = ServerConfiguration.TfsConnection, ServiceName = "LocationService", CollectionServiceIdentifier = "bf9cf1d0-24ac-4d35-aeca-6cd18c69c1fe", ConfigurationServiceIdentifier = "bf9cf1d0-24ac-4d35-aeca-6cd18c69c1fe")]
  public class LocationWebService : FrameworkWebService
  {
    private SecuredLocationManager m_locationManager;
    private ILocationService m_locationService;
    private TeamFoundationConnectionService m_connectionService;

    public LocationWebService()
    {
      this.m_locationManager = this.RequestContext.GetService<SecuredLocationManager>();
      this.m_locationService = this.RequestContext.GetService<ILocationService>();
      this.m_connectionService = this.RequestContext.GetService<TeamFoundationConnectionService>();
    }

    [WebMethod]
    public Microsoft.TeamFoundation.Server.Core.Location.LocationServiceData SaveServiceDefinitions(
      List<Microsoft.TeamFoundation.Server.Core.Location.ServiceDefinition> serviceDefinitions,
      int lastChangeId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (SaveServiceDefinitions), MethodType.Admin, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<Microsoft.TeamFoundation.Server.Core.Location.ServiceDefinition>(nameof (serviceDefinitions), (IList<Microsoft.TeamFoundation.Server.Core.Location.ServiceDefinition>) serviceDefinitions);
        methodInformation.AddParameter(nameof (lastChangeId), (object) lastChangeId);
        this.EnterMethod(methodInformation);
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) serviceDefinitions, nameof (serviceDefinitions));
        this.RequestContext.CheckOnPremisesDeployment(true);
        bool clientCacheFresh = (long) lastChangeId == this.m_locationManager.GetLastChangeId(this.RequestContext);
        IEnumerable<Microsoft.VisualStudio.Services.Location.ServiceDefinition> serviceDefinitions1 = LocationCompatUtil.Convert((IEnumerable<Microsoft.TeamFoundation.Server.Core.Location.ServiceDefinition>) serviceDefinitions);
        this.m_locationManager.SaveServiceDefinitions(this.RequestContext, serviceDefinitions1);
        return new Microsoft.TeamFoundation.Server.Core.Location.LocationServiceData(this.m_locationManager.GetLastChangeId(this.RequestContext), LocationCompatUtil.Convert(this.RequestContext, serviceDefinitions1), LocationCompatUtil.Convert(this.RequestContext, this.m_locationManager.GetAccessMappings(this.RequestContext)), this.m_locationService.GetDefaultAccessMapping(this.RequestContext).Moniker, clientCacheFresh, LocationServiceHelper.GetClientCacheTimeToLive(this.RequestContext), this.RequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection));
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public Microsoft.TeamFoundation.Server.Core.Location.LocationServiceData RemoveServiceDefinitions(
      List<Microsoft.TeamFoundation.Server.Core.Location.ServiceDefinition> serviceDefinitions,
      int lastChangeId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (RemoveServiceDefinitions), MethodType.Admin, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<Microsoft.TeamFoundation.Server.Core.Location.ServiceDefinition>(nameof (serviceDefinitions), (IList<Microsoft.TeamFoundation.Server.Core.Location.ServiceDefinition>) serviceDefinitions);
        methodInformation.AddParameter(nameof (lastChangeId), (object) lastChangeId);
        this.EnterMethod(methodInformation);
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) serviceDefinitions, nameof (serviceDefinitions));
        if (!this.RequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
          this.RequestContext.CheckOnPremisesDeployment(true);
        bool clientCacheFresh = (long) lastChangeId == this.m_locationManager.GetLastChangeId(this.RequestContext);
        this.m_locationManager.RemoveServiceDefinitions(this.RequestContext, LocationCompatUtil.Convert((IEnumerable<Microsoft.TeamFoundation.Server.Core.Location.ServiceDefinition>) serviceDefinitions));
        return new Microsoft.TeamFoundation.Server.Core.Location.LocationServiceData(this.m_locationManager.GetLastChangeId(this.RequestContext), this.m_locationService.GetDefaultAccessMapping(this.RequestContext).Moniker, clientCacheFresh, LocationServiceHelper.GetClientCacheTimeToLive(this.RequestContext));
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public Microsoft.TeamFoundation.Server.Core.Location.LocationServiceData ConfigureAccessMapping(
      Microsoft.TeamFoundation.Server.Core.Location.AccessMapping accessMapping,
      int lastChangeId,
      bool makeDefault)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (ConfigureAccessMapping), MethodType.Admin, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (accessMapping), (object) accessMapping);
        methodInformation.AddParameter(nameof (lastChangeId), (object) lastChangeId);
        this.EnterMethod(methodInformation);
        ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.Server.Core.Location.AccessMapping>(accessMapping, nameof (accessMapping));
        ArgumentUtility.CheckStringForNullOrEmpty(accessMapping.Moniker, "accessMapping.Moniker");
        if (!this.RequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
          this.RequestContext.CheckOnPremisesDeployment(true);
        bool clientCacheFresh = (long) lastChangeId == this.m_locationManager.GetLastChangeId(this.RequestContext);
        this.m_locationManager.ConfigureAccessMapping(this.RequestContext, LocationCompatUtil.Convert(accessMapping), makeDefault);
        return new Microsoft.TeamFoundation.Server.Core.Location.LocationServiceData(this.m_locationManager.GetLastChangeId(this.RequestContext), LocationCompatUtil.Convert(this.RequestContext, this.m_locationManager.QueryServices(this.RequestContext)), LocationCompatUtil.Convert(this.RequestContext, this.m_locationManager.GetAccessMappings(this.RequestContext)), this.m_locationService.GetDefaultAccessMapping(this.RequestContext).Moniker, clientCacheFresh, LocationServiceHelper.GetClientCacheTimeToLive(this.RequestContext), this.RequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection));
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public Microsoft.TeamFoundation.Server.Core.Location.LocationServiceData SetDefaultAccessMapping(
      Microsoft.TeamFoundation.Server.Core.Location.AccessMapping accessMapping,
      int lastChangeId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (SetDefaultAccessMapping), MethodType.Admin, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (accessMapping), (object) accessMapping);
        methodInformation.AddParameter(nameof (lastChangeId), (object) lastChangeId);
        this.EnterMethod(methodInformation);
        ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.Server.Core.Location.AccessMapping>(accessMapping, nameof (accessMapping));
        ArgumentUtility.CheckStringForNullOrEmpty(accessMapping.Moniker, "accessMapping.Moniker");
        if (!this.RequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
          this.RequestContext.CheckOnPremisesDeployment(true);
        bool clientCacheFresh = (long) lastChangeId == this.m_locationManager.GetLastChangeId(this.RequestContext);
        this.m_locationManager.SetDefaultAccessMapping(this.RequestContext, LocationCompatUtil.Convert(accessMapping));
        return new Microsoft.TeamFoundation.Server.Core.Location.LocationServiceData(this.m_locationManager.GetLastChangeId(this.RequestContext), LocationCompatUtil.Convert(this.RequestContext, this.m_locationManager.QueryServices(this.RequestContext)), LocationCompatUtil.Convert(this.RequestContext, this.m_locationManager.GetAccessMappings(this.RequestContext)), this.m_locationService.GetDefaultAccessMapping(this.RequestContext).Moniker, clientCacheFresh, LocationServiceHelper.GetClientCacheTimeToLive(this.RequestContext), this.RequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection));
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public Microsoft.TeamFoundation.Server.Core.Location.LocationServiceData RemoveAccessMapping(
      Microsoft.TeamFoundation.Server.Core.Location.AccessMapping accessMapping,
      int lastChangeId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation("UpdateAccessMappings", MethodType.Admin, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (accessMapping), (object) accessMapping);
        methodInformation.AddParameter(nameof (lastChangeId), (object) lastChangeId);
        this.EnterMethod(methodInformation);
        ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.Server.Core.Location.AccessMapping>(accessMapping, "accessMappings");
        if (!this.RequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
          this.RequestContext.CheckOnPremisesDeployment(true);
        bool clientCacheFresh = (long) lastChangeId == this.m_locationManager.GetLastChangeId(this.RequestContext);
        this.m_locationManager.RemoveAccessMapping(this.RequestContext, LocationCompatUtil.Convert(accessMapping));
        return new Microsoft.TeamFoundation.Server.Core.Location.LocationServiceData(this.m_locationManager.GetLastChangeId(this.RequestContext), this.m_locationService.GetDefaultAccessMapping(this.RequestContext).Moniker, clientCacheFresh, LocationServiceHelper.GetClientCacheTimeToLive(this.RequestContext));
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public Microsoft.TeamFoundation.Server.Core.Location.LocationServiceData QueryServices(
      List<ServiceTypeFilter> serviceTypeFilters,
      int lastChangeId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryServices), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (serviceTypeFilters), (object) serviceTypeFilters);
        methodInformation.AddParameter(nameof (lastChangeId), (object) lastChangeId);
        this.EnterMethod(methodInformation);
        return this.m_connectionService.GetLocationServiceData(this.RequestContext, lastChangeId);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public Microsoft.TeamFoundation.Server.Core.Location.ConnectionData Connect(
      int connectOptions,
      int lastChangeId,
      int features)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (Connect), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (connectOptions), (object) connectOptions);
        methodInformation.AddParameter(nameof (lastChangeId), (object) lastChangeId);
        TeamFoundationSupportedFeatures supportedFeatures = (TeamFoundationSupportedFeatures) features;
        methodInformation.AddParameter("supportedFeatures", (object) supportedFeatures);
        this.EnterMethod(methodInformation);
        return this.m_connectionService.Connect(this.RequestContext, connectOptions, lastChangeId, supportedFeatures);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }
  }
}
