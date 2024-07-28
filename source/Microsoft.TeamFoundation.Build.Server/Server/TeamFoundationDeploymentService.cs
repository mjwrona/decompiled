// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.TeamFoundationDeploymentService
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.Azure.Boards.Linking;
using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.Server.DataAccess;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Integration.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.TeamFoundation.VersionControl.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Xaml;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.Build.Server
{
  public sealed class TeamFoundationDeploymentService : IVssFrameworkService
  {
    private const string CertXmlFormat = "<?xml version='1.0' encoding='utf-8'?>\r\n            <PublishData>\r\n              <PublishProfile\r\n                SchemaVersion='2.0'\r\n                PublishMethod='AzureServiceManagementAPI'>\r\n                <Subscription\r\n                  ServiceManagementUrl='{0}'\r\n                  Id='{1}'\r\n                  Name='{2}'\r\n                  ManagementCertificate='{3}' />\r\n              </PublishProfile>\r\n            </PublishData>";
    private const string CredentialsXmlFormat = "<?xml version = '1.0' encoding='utf-8'?>\r\n            <Subscription Id = '{0}' Name='{1}'>\r\n              <Credentials>\r\n                <Username>{2}</Username>\r\n                <Password>{3}</Password>\r\n              </Credentials>\r\n            </Subscription>";
    private const string serviceManagementUrl = "https://management.core.windows.net";
    private IBuild2Converter m_build2Converter;

    internal TeamFoundationDeploymentService()
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(0, "Deployment", "Service", "ServiceStart");
      this.m_build2Converter = systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) ? systemRequestContext.GetExtension<IBuild2Converter>() : throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
      systemRequestContext.TraceLeave(0, "Deployment", "Service", "ServiceStart");
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(0, "Deployment", "Service", "ServiceEnd");
      if (this.m_build2Converter != null)
        this.m_build2Converter = (IBuild2Converter) null;
      systemRequestContext.TraceLeave(0, "Deployment", "Service", "ServiceEnd");
    }

    public DeploymentEnvironmentMetadata CreateDeploymentEnvironment(
      IVssRequestContext requestContext,
      DeploymentEnvironmentCreationData deploymentEnvironmentCreationData)
    {
      requestContext.TraceEnter(0, "Deployment", "Service", nameof (CreateDeploymentEnvironment));
      Guid drawerId = Guid.Empty;
      TeamFoundationStrongBoxService service1 = requestContext.GetService<TeamFoundationStrongBoxService>();
      IVssRequestContext requestContext1 = requestContext;
      try
      {
        Validation.CheckValidatable(requestContext, nameof (deploymentEnvironmentCreationData), (IValidatable) deploymentEnvironmentCreationData, false, ValidationContext.Add);
        string projectUri;
        Guid projectIdFromName = this.GetProjectIdFromName(requestContext, deploymentEnvironmentCreationData.EnvironmentMetadata.TeamProject, out projectUri);
        requestContext1 = TeamFoundationDeploymentService.GetStrongBoxRequestContext(requestContext, projectUri);
        string name = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}.EnvironmentData", (object) projectIdFromName, (object) deploymentEnvironmentCreationData.EnvironmentMetadata.Name);
        drawerId = service1.CreateDrawer(requestContext1, name);
        if (string.IsNullOrWhiteSpace(deploymentEnvironmentCreationData.EnvironmentMetadata.ConnectedServiceName))
          throw new ArgumentNullException("ConnectedServiceName", BuildTypeResource.InvalidInputParameterNull((object) "ConnectedServiceName"));
        TeamFoundationConnectedServicesService service2 = requestContext.GetService<TeamFoundationConnectedServicesService>();
        string connectedServiceName = deploymentEnvironmentCreationData.EnvironmentMetadata.ConnectedServiceName;
        string resource = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DeploymentEnvironmentLock.{0}.{1}", (object) deploymentEnvironmentCreationData.EnvironmentMetadata.TeamProject, (object) connectedServiceName);
        using (requestContext.GetService<ITeamFoundationLockingService>().AcquireLock(requestContext, TeamFoundationLockMode.Shared, resource))
        {
          if (!service2.DoesConnectedServiceExist(requestContext, connectedServiceName, deploymentEnvironmentCreationData.EnvironmentMetadata.TeamProject))
            throw new ArgumentException(BuildTypeResource.InvalidInputParameterNull((object) "ConnectedService"));
          if (deploymentEnvironmentCreationData.EnvironmentPropertiesValue != null)
          {
            foreach (InformationField informationField in deploymentEnvironmentCreationData.EnvironmentPropertiesValue)
              service1.AddString(requestContext1, drawerId, informationField.Name, informationField.Value);
          }
          using (DeploymentEnvironmentComponent component = requestContext.CreateComponent<DeploymentEnvironmentComponent>("Build"))
            component.AddDeploymentEnvironment(deploymentEnvironmentCreationData, projectIdFromName);
          service2.AddConnectedServiceProjectAssociation(requestContext, deploymentEnvironmentCreationData.EnvironmentMetadata.ConnectedServiceName, deploymentEnvironmentCreationData.EnvironmentMetadata.TeamProject);
          return deploymentEnvironmentCreationData.EnvironmentMetadata;
        }
      }
      catch (Exception ex)
      {
        if (drawerId != Guid.Empty)
          service1.DeleteDrawer(requestContext1, drawerId);
        requestContext.TraceException(0, "Deployment", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(0, "Deployment", "Service", nameof (CreateDeploymentEnvironment));
      }
    }

    public ConnectedServiceMetadata UpdateSubscription(
      IVssRequestContext requestContext,
      ConnectedServiceMetadata data)
    {
      requestContext.TraceEnter(0, "Deployment", "Service", "RenameSubscription");
      try
      {
        TeamFoundationConnectedServicesService service = requestContext.GetService<TeamFoundationConnectedServicesService>();
        ConnectedServiceCreationData connectedServiceCreationData = new ConnectedServiceCreationData(data.Name, data.TeamProject, data.Kind, data.FriendlyName, data.Description, data.ServiceUri, "https://management.core.windows.net", (service.GetConnectedService(requestContext, data.Name, data.TeamProject) ?? throw new ConnectedServiceNotFoundException(data.Name)).GetCredentialsXml(requestContext));
        return service.CreateConnectedService(requestContext, connectedServiceCreationData);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "Deployment", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(0, "Deployment", "Service", "RenameSubscription");
      }
    }

    public DeploymentEnvironmentMetadata CreateDeploymentEnvironmentForAzure(
      IVssRequestContext requestContext,
      DeploymentEnvironmentApiData data)
    {
      requestContext.TraceEnter(0, "Deployment", "Service", nameof (CreateDeploymentEnvironmentForAzure));
      try
      {
        TeamFoundationConnectedServicesService service = requestContext.GetService<TeamFoundationConnectedServicesService>();
        if (data.DisconnectSubscription)
        {
          try
          {
            if (this.GetDeploymentEnvironment(requestContext, data.SubscriptionName, data.ProjectName) != null)
              this.DeleteDeploymentEnvironment(requestContext, data.SubscriptionName, data.ProjectName);
          }
          catch (DeploymentEnvironmentNotFoundException ex)
          {
            requestContext.Trace(0, TraceLevel.Info, "Deployment", "Service", string.Format("The azure subscription {0}'s deployment environment was probably added through VS with a different name.", (object) data.SubscriptionName));
          }
        }
        string credentialsXml;
        if (data.Cert.IsNullOrEmpty<char>())
          credentialsXml = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<?xml version = '1.0' encoding='utf-8'?>\r\n            <Subscription Id = '{0}' Name='{1}'>\r\n              <Credentials>\r\n                <Username>{2}</Username>\r\n                <Password>{3}</Password>\r\n              </Credentials>\r\n            </Subscription>", (object) data.SubscriptionId, (object) HttpUtility.HtmlEncode(data.SubscriptionName), (object) HttpUtility.HtmlEncode(data.UserName), (object) HttpUtility.HtmlEncode(data.Password));
        else
          credentialsXml = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<?xml version='1.0' encoding='utf-8'?>\r\n            <PublishData>\r\n              <PublishProfile\r\n                SchemaVersion='2.0'\r\n                PublishMethod='AzureServiceManagementAPI'>\r\n                <Subscription\r\n                  ServiceManagementUrl='{0}'\r\n                  Id='{1}'\r\n                  Name='{2}'\r\n                  ManagementCertificate='{3}' />\r\n              </PublishProfile>\r\n            </PublishData>", (object) "https://management.core.windows.net", (object) data.SubscriptionId, (object) HttpUtility.HtmlEncode(data.SubscriptionName), (object) data.Cert);
        ConnectedServiceCreationData connectedServiceCreationData = new ConnectedServiceCreationData(data.SubscriptionId, data.ProjectName, Microsoft.TeamFoundation.Server.Core.ConnectedServiceKind.AzureSubscription, data.SubscriptionName, (string) null, (string) null, "https://management.core.windows.net", credentialsXml);
        ConnectedServiceMetadata connectedService = service.CreateConnectedService(requestContext, connectedServiceCreationData);
        requestContext.GetService<TeamFoundationDeploymentService>();
        DeploymentEnvironmentCreationData deploymentEnvironmentCreationData = new DeploymentEnvironmentCreationData(data.DeploymentName, data.ProjectName, connectedService.Name, DeploymentEnvironmentKind.AzureWebsite, connectedService.FriendlyName, connectedService.Description, new Dictionary<string, string>()
        {
          {
            "subscription",
            connectedService.Name
          },
          {
            "webspace",
            "__notused__webspace"
          },
          {
            "website",
            "__notused__websitename"
          }
        });
        return this.CreateDeploymentEnvironment(requestContext, deploymentEnvironmentCreationData);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "Deployment", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(0, "Deployment", "Service", nameof (CreateDeploymentEnvironmentForAzure));
      }
    }

    public List<DeploymentEnvironmentMetadata> QueryDeploymentEnvironments(
      IVssRequestContext requestContext,
      string teamProject,
      string serviceName = "")
    {
      requestContext.TraceEnter(0, "Deployment", "Service", "GetDeploymentEnvironments");
      if (!requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.FrameworkNamespaceId).HasPermission(requestContext, FrameworkSecurity.FrameworkNamespaceToken, 1, false))
        return new List<DeploymentEnvironmentMetadata>();
      try
      {
        Guid projectIdFromName = this.GetProjectIdFromName(requestContext, teamProject);
        using (DeploymentEnvironmentComponent component = requestContext.CreateComponent<DeploymentEnvironmentComponent>("Build"))
          return component.GetDeploymentEnvironments(projectIdFromName, teamProject, serviceName);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "Deployment", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(0, "Deployment", "Service", "GetDeploymentEnvironments");
      }
    }

    public DeploymentEnvironment GetDeploymentEnvironment(
      IVssRequestContext requestContext,
      string name,
      string teamProject)
    {
      requestContext.TraceEnter(0, "Deployment", "Service", nameof (GetDeploymentEnvironment));
      try
      {
        ArgumentUtility.CheckStringForNullOrWhiteSpace(name, nameof (name));
        TeamFoundationStrongBoxService service1 = requestContext.GetService<TeamFoundationStrongBoxService>();
        ITeamFoundationLockingService service2 = requestContext.GetService<ITeamFoundationLockingService>();
        string projectUri;
        Guid projectIdFromName = this.GetProjectIdFromName(requestContext, teamProject, out projectUri);
        IVssRequestContext requestContext1 = !requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext.Elevate(), FrameworkSecurity.StrongBoxNamespaceId).HasPermission(requestContext, FrameworkSecurity.StrongBoxSecurityNamespaceRootToken, 32, false) ? TeamFoundationDeploymentService.GetStrongBoxRequestContext(requestContext, projectUri) : requestContext;
        try
        {
          string name1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}.EnvironmentData", (object) projectIdFromName, (object) name);
          Guid drawerId = service1.UnlockDrawer(requestContext1, name1, true);
          using (service2.AcquireLock(requestContext, TeamFoundationLockMode.Shared, drawerId.ToString()))
          {
            DeploymentEnvironmentMetadata deploymentEnvironment;
            using (DeploymentEnvironmentComponent component = requestContext.CreateComponent<DeploymentEnvironmentComponent>("Build"))
              deploymentEnvironment = component.GetDeploymentEnvironment(name, projectIdFromName, teamProject);
            List<StrongBoxItemInfo> drawerContents = service1.GetDrawerContents(requestContext1, drawerId);
            return new DeploymentEnvironment(deploymentEnvironment, drawerContents);
          }
        }
        catch (StrongBoxDrawerNotFoundException ex)
        {
          throw new DeploymentEnvironmentNotFoundException(name, (Exception) ex);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "Deployment", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(0, "Deployment", "Service", nameof (GetDeploymentEnvironment));
      }
    }

    public void DeleteDeploymentEnvironment(
      IVssRequestContext requestContext,
      string name,
      string teamProject,
      bool deleteService = true)
    {
      requestContext.TraceEnter(0, "Deployment", "Service", nameof (DeleteDeploymentEnvironment));
      try
      {
        string projectUri;
        Guid projectIdFromName = this.GetProjectIdFromName(requestContext, teamProject, out projectUri);
        IVssRequestContext boxRequestContext = TeamFoundationDeploymentService.GetStrongBoxRequestContext(requestContext, projectUri);
        DeploymentEnvironment deploymentEnvironment;
        try
        {
          deploymentEnvironment = this.GetDeploymentEnvironment(requestContext, name, teamProject);
        }
        catch (DeploymentEnvironmentNotFoundException ex)
        {
          return;
        }
        TeamFoundationStrongBoxService service1 = requestContext.GetService<TeamFoundationStrongBoxService>();
        ITeamFoundationLockingService service2 = requestContext.GetService<ITeamFoundationLockingService>();
        string name1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}.EnvironmentData", (object) projectIdFromName, (object) name);
        Guid drawerId = service1.UnlockDrawer(boxRequestContext, name1, false);
        using (service2.AcquireLock(requestContext, TeamFoundationLockMode.Exclusive, drawerId.ToString()))
        {
          if (drawerId != Guid.Empty)
            service1.DeleteDrawer(boxRequestContext, drawerId);
          using (DeploymentEnvironmentComponent component = requestContext.CreateComponent<DeploymentEnvironmentComponent>("Build"))
            component.DeleteDeploymentEnvironment(name, projectIdFromName);
        }
        if (!deleteService)
          return;
        string connectedServiceName = deploymentEnvironment.EnvironmentMetadata.ConnectedServiceName;
        string resource = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DeploymentEnvironmentLock.{0}.{1}", (object) teamProject, (object) connectedServiceName);
        using (service2.AcquireLock(requestContext, TeamFoundationLockMode.Exclusive, resource))
        {
          if (this.QueryDeploymentEnvironments(requestContext, teamProject).Exists((Predicate<DeploymentEnvironmentMetadata>) (x => x.ConnectedServiceName.Equals(connectedServiceName, StringComparison.OrdinalIgnoreCase))))
            return;
          requestContext.GetService<TeamFoundationConnectedServicesService>().DeleteConnectedService(requestContext, connectedServiceName, teamProject);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "Deployment", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(0, "Deployment", "Service", nameof (DeleteDeploymentEnvironment));
      }
    }

    public void DeleteTeamProject(IVssRequestContext requestContext, string projectUri)
    {
      requestContext.TraceEnter(0, "Deployment", "Service", nameof (DeleteTeamProject));
      TeamProject teamProject = (TeamProject) null;
      try
      {
        IProjectService service1 = requestContext.GetService<IProjectService>();
        TeamFoundationStrongBoxService service2 = requestContext.GetService<TeamFoundationStrongBoxService>();
        ITeamFoundationLockingService service3 = requestContext.GetService<ITeamFoundationLockingService>();
        IVssRequestContext requestContext1 = requestContext;
        string uri = projectUri;
        teamProject = service1.GetTeamProjectFromUri(requestContext1, uri);
        List<string> source = new List<string>();
        List<DeploymentEnvironmentMetadata> deploymentEnvironments;
        using (DeploymentEnvironmentComponent component = requestContext.CreateComponent<DeploymentEnvironmentComponent>("Build"))
          deploymentEnvironments = component.GetDeploymentEnvironments(teamProject.Id, string.Empty);
        foreach (DeploymentEnvironmentMetadata environmentMetadata in deploymentEnvironments)
        {
          source.Add(environmentMetadata.ConnectedServiceName);
          string name = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}.EnvironmentData", (object) teamProject.Id, (object) environmentMetadata.Name);
          Guid drawerId = service2.UnlockDrawer(requestContext, name, false);
          using (service3.AcquireLock(requestContext, TeamFoundationLockMode.Exclusive, drawerId.ToString()))
          {
            if (drawerId != Guid.Empty)
              service2.DeleteDrawer(requestContext, drawerId);
            using (DeploymentEnvironmentComponent component = requestContext.CreateComponent<DeploymentEnvironmentComponent>("Build"))
              component.DeleteDeploymentEnvironment(environmentMetadata.Name, teamProject.Id);
          }
        }
        foreach (string name in source.Distinct<string>())
          requestContext.GetService<TeamFoundationConnectedServicesService>().DeleteConnectedService(requestContext, name, teamProject.Name);
      }
      catch (DataspaceNotFoundException ex)
      {
        ProjectInfo projectInfo = (ProjectInfo) null;
        if (teamProject != null)
          projectInfo = requestContext.GetService<IProjectService>().GetProject(requestContext, teamProject.Id);
        if (projectInfo == null || projectInfo.State != ProjectState.WellFormed)
          return;
        requestContext.TraceException(0, "Deployment", "Service", (Exception) ex);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "Deployment", "Service", ex);
      }
      finally
      {
        requestContext.TraceLeave(0, "Deployment", "Service", nameof (DeleteTeamProject));
      }
    }

    public BuildDefinition ConnectAzureWebsite(
      IVssRequestContext requestContext,
      string teamProject,
      string subscriptionId,
      string webspace,
      string website)
    {
      return this.ConnectAzureWebsite(requestContext, teamProject, subscriptionId, webspace, website, string.Empty, string.Empty);
    }

    public ShallowReference ConnectAzureWebsiteWithNewBuildDefinition(
      IVssRequestContext requestContext,
      string teamProject,
      string subscriptionId,
      string webspace,
      string website)
    {
      return this.ConnectAzureWebsiteWithNewBuildDefinition(requestContext, teamProject, subscriptionId, webspace, website, string.Empty, string.Empty);
    }

    internal void DisconnectServiceEndpoint(
      IVssRequestContext requestContext,
      string subscriptionId,
      Guid projectId)
    {
      try
      {
        IServiceEndpointService2 service = requestContext.GetService<IServiceEndpointService2>();
        Guid endpointId = Guid.Parse(subscriptionId);
        if (service.GetServiceEndpoint(requestContext, projectId, endpointId) == null)
          return;
        try
        {
          service.DeleteServiceEndpoint(requestContext, projectId, endpointId);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(0, "Deployment", "Service", ex);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(30254, "Deployment", "Service", ex);
      }
    }

    internal void CreateServiceEndpoint(
      IVssRequestContext requestContext,
      ConnectedServiceCreationData serviceData,
      Guid projectId)
    {
      try
      {
        IServiceEndpointService2 service1 = requestContext.GetService<IServiceEndpointService2>();
        Guid endpointId = Guid.Parse(serviceData.ServiceMetadata.Name);
        if (service1.GetServiceEndpoint(requestContext, projectId, endpointId) != null)
          return;
        if (!requestContext.IsServicingContext || serviceData.ServiceMetadata.AuthenticatedBy == Guid.Empty)
        {
          TeamFoundationIdentityService service2 = requestContext.GetService<TeamFoundationIdentityService>();
          serviceData.ServiceMetadata.AuthenticatedBy = service2.ReadRequestIdentity(requestContext).TeamFoundationId;
        }
        Uri result;
        Uri.TryCreate(serviceData.Endpoint, UriKind.Absolute, out result);
        ServiceEndpoint endpoint = new ServiceEndpoint()
        {
          Id = endpointId,
          Name = serviceData.ServiceMetadata.FriendlyName,
          Authorization = ConnectedService.GetAuthorization(requestContext, serviceData.CredentialsXml),
          CreatedBy = new IdentityRef()
          {
            Id = serviceData.ServiceMetadata.AuthenticatedBy.ToString("D")
          },
          Description = serviceData.ServiceMetadata.Description,
          Type = "Azure",
          Url = result
        };
        endpoint.Data.Add("SubscriptionId", serviceData.ServiceMetadata.Name);
        endpoint.Data.Add("SubscriptionName", serviceData.ServiceMetadata.FriendlyName);
        endpoint.Data.Add("ServiceUri", serviceData.ServiceMetadata.ServiceUri);
        string empty = string.Empty;
        if (ServiceEndpoint.ValidateServiceEndpoint(endpoint, ref empty))
        {
          service1.CreateServiceEndpoint(requestContext, projectId, endpoint);
        }
        else
        {
          string message = string.Format("Cannot convert to end point due to invalid connected service data.This means the CI new build definition created won't have correct subscription setup. User should go and manually add the service. ConnectedService id: {0}, Project id: {1}. Error:: {2}", (object) serviceData.ServiceMetadata.Name, (object) projectId, (object) empty);
          requestContext.Trace(30254, TraceLevel.Error, "Deployment", "Service", message);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(30254, "Deployment", "Service", ex);
      }
    }

    internal BuildDefinition ConnectAzureWebsite(
      IVssRequestContext requestContext,
      string teamProject,
      string subscriptionId,
      string webspace,
      string website,
      string repositoryId,
      string gitBranch)
    {
      requestContext.TraceEnter(0, "Deployment", "Service", "ConnectAzureWebSite");
      ArgumentUtility.CheckStringForNullOrEmpty(teamProject, nameof (teamProject));
      ArgumentUtility.CheckStringForNullOrEmpty(subscriptionId, nameof (subscriptionId));
      ArgumentUtility.CheckStringForNullOrEmpty(webspace, nameof (webspace));
      ArgumentUtility.CheckStringForNullOrEmpty(website, nameof (website));
      try
      {
        string str = website;
        DeploymentEnvironment deploymentEnvironment = (DeploymentEnvironment) null;
        try
        {
          deploymentEnvironment = this.GetDeploymentEnvironment(requestContext, str, teamProject);
        }
        catch (DeploymentEnvironmentNotFoundException ex)
        {
        }
        if (deploymentEnvironment == null)
        {
          DeploymentEnvironmentCreationData deploymentEnvironmentCreationData = new DeploymentEnvironmentCreationData(str, teamProject, subscriptionId, DeploymentEnvironmentKind.AzureWebsite, "", "", new Dictionary<string, string>()
          {
            {
              "subscription",
              subscriptionId
            },
            {
              nameof (webspace),
              webspace
            },
            {
              nameof (website),
              website
            }
          });
          this.CreateDeploymentEnvironment(requestContext, deploymentEnvironmentCreationData);
        }
        return this.CreateAzureDeploymentBuildDefinition(requestContext, str, teamProject, repositoryId, gitBranch);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "Deployment", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(0, "Deployment", "Service", "ConnectAzureWebSite");
      }
    }

    internal ShallowReference ConnectAzureWebsiteWithNewBuildDefinition(
      IVssRequestContext requestContext,
      string teamProject,
      string subscriptionId,
      string webspace,
      string website,
      string repositoryId,
      string gitBranch)
    {
      requestContext.TraceEnter(0, "Deployment", "Service", nameof (ConnectAzureWebsiteWithNewBuildDefinition));
      try
      {
        return JsonConvert.DeserializeObject<ShallowReference>(this.m_build2Converter.CreateAzureWebsiteBuildDefinition(requestContext, this.GetBuildDefinitionNameFromEnvironmentName(website), teamProject, website, subscriptionId, repositoryId, gitBranch));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "Deployment", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(0, "Deployment", "Service", "ConnectAzureWebsiteBuildDefinition");
      }
    }

    public void DisconnectAzureWebsite(
      IVssRequestContext requestContext,
      string teamProject,
      string website)
    {
      requestContext.TraceEnter(0, "Deployment", "Service", nameof (DisconnectAzureWebsite));
      this.DisconnectDeploymentBuildDefinition(requestContext, teamProject, website);
      requestContext.TraceLeave(0, "Deployment", "Service", nameof (DisconnectAzureWebsite));
    }

    public void DisconnectAzureWebsiteNewBuildDefinition(
      IVssRequestContext requestContext,
      string teamProject,
      string website)
    {
      requestContext.TraceEnter(0, "Deployment", "Service", nameof (DisconnectAzureWebsiteNewBuildDefinition));
      this.DisconnectDeploymentNewBuildDefinition(requestContext, teamProject, website);
      requestContext.TraceLeave(0, "Deployment", "Service", nameof (DisconnectAzureWebsiteNewBuildDefinition));
    }

    public BuildDefinition ConnectAzureCloudApp(
      IVssRequestContext requestContext,
      string teamProject,
      string subscriptionId,
      string hostedServiceName,
      string storageAccountName,
      string deploymentLabel)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(teamProject, nameof (teamProject));
      ArgumentUtility.CheckStringForNullOrEmpty(hostedServiceName, nameof (hostedServiceName));
      ArgumentUtility.CheckStringForNullOrEmpty(deploymentLabel, nameof (deploymentLabel));
      requestContext.TraceEnter(0, "Deployment", "Service", nameof (ConnectAzureCloudApp));
      try
      {
        AzureCloudAppProfile defaultProfile = AzureCloudAppProfile.CreateDefaultProfile(hostedServiceName, storageAccountName, deploymentLabel);
        return this.ConnectAzureCloudAppInternal(requestContext, teamProject, subscriptionId, defaultProfile);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "Deployment", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(0, "Deployment", "Service", nameof (ConnectAzureCloudApp));
      }
    }

    public ShallowReference ConnectAzureCloudAppWithNewBuildDefinition(
      IVssRequestContext requestContext,
      string teamProject,
      string subscriptionId,
      string hostedServiceName,
      string storageAccountName,
      string deploymentLabel)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(teamProject, nameof (teamProject));
      ArgumentUtility.CheckStringForNullOrEmpty(hostedServiceName, nameof (hostedServiceName));
      ArgumentUtility.CheckStringForNullOrEmpty(deploymentLabel, nameof (deploymentLabel));
      requestContext.TraceEnter(0, "Deployment", "Service", nameof (ConnectAzureCloudAppWithNewBuildDefinition));
      try
      {
        AzureCloudAppProfile defaultProfile = AzureCloudAppProfile.CreateDefaultProfile(hostedServiceName, storageAccountName, deploymentLabel);
        return this.ConnectAzureCloudAppInternalWithNewBuildDefinition(requestContext, teamProject, storageAccountName, hostedServiceName, subscriptionId, defaultProfile);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "Deployment", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(0, "Deployment", "Service", nameof (ConnectAzureCloudAppWithNewBuildDefinition));
      }
    }

    public BuildDefinition ConnectAzureCloudApp(
      IVssRequestContext requestContext,
      string teamProject,
      string subscriptionId,
      string cloudAppPublishProfile)
    {
      requestContext.TraceEnter(0, "Deployment", "Service", nameof (ConnectAzureCloudApp));
      ArgumentUtility.CheckStringForNullOrEmpty(teamProject, nameof (teamProject));
      ArgumentUtility.CheckStringForNullOrEmpty(cloudAppPublishProfile, nameof (cloudAppPublishProfile));
      try
      {
        AzureCloudAppProfile profile = AzureCloudAppProfile.Parse(cloudAppPublishProfile);
        return this.ConnectAzureCloudAppInternal(requestContext, teamProject, subscriptionId, profile);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "Deployment", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(0, "Deployment", "Service", nameof (ConnectAzureCloudApp));
      }
    }

    public ShallowReference ConnectAzureCloudAppWithNewBuildDefinition(
      IVssRequestContext requestContext,
      string teamProject,
      string subscriptionId,
      string cloudAppPublishProfile)
    {
      requestContext.TraceEnter(0, "Deployment", "Service", nameof (ConnectAzureCloudAppWithNewBuildDefinition));
      ArgumentUtility.CheckStringForNullOrEmpty(teamProject, nameof (teamProject));
      ArgumentUtility.CheckStringForNullOrEmpty(cloudAppPublishProfile, nameof (cloudAppPublishProfile));
      try
      {
        AzureCloudAppProfile profile = AzureCloudAppProfile.Parse(cloudAppPublishProfile);
        return this.ConnectAzureCloudAppInternalWithNewBuildDefinition(requestContext, teamProject, profile.AzureStorageAccountName, profile.AzureHostedServiceName, subscriptionId, profile);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "Deployment", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(0, "Deployment", "Service", nameof (ConnectAzureCloudAppWithNewBuildDefinition));
      }
    }

    public void DisconnectAzureCloudApp(
      IVssRequestContext requestContext,
      string teamProject,
      string hostedServiceName)
    {
      requestContext.TraceEnter(0, "Deployment", "Service", nameof (DisconnectAzureCloudApp));
      this.DisconnectDeploymentBuildDefinition(requestContext, teamProject, hostedServiceName);
      requestContext.TraceLeave(0, "Deployment", "Service", nameof (DisconnectAzureCloudApp));
    }

    public void DisconnectAzureCloudAppNewBuildDefinition(
      IVssRequestContext requestContext,
      string teamProject,
      string hostedServiceName)
    {
      requestContext.TraceEnter(0, "Deployment", "Service", nameof (DisconnectAzureCloudAppNewBuildDefinition));
      this.DisconnectDeploymentNewBuildDefinition(requestContext, teamProject, hostedServiceName);
      requestContext.TraceLeave(0, "Deployment", "Service", nameof (DisconnectAzureCloudAppNewBuildDefinition));
    }

    public BuildDeployment CreateBuildDeployment(
      IVssRequestContext requestContext,
      string deploymentUri,
      string sourceUri,
      string environmentName)
    {
      requestContext.TraceEnter(0, "Deployment", "Service", nameof (CreateBuildDeployment));
      ArgumentValidation.CheckUri("deploymnetUri", deploymentUri, false, ResourceStrings.MissingUri());
      ArgumentValidation.CheckUri(nameof (sourceUri), sourceUri, false, ResourceStrings.MissingUri());
      ArgumentValidation.Check(nameof (environmentName), (object) environmentName, false);
      BuildDetail buildDetail = (BuildDetail) null;
      TeamFoundationBuildHost service = requestContext.GetService<TeamFoundationBuildHost>();
      using (TeamFoundationDataReader foundationDataReader = requestContext.GetService<TeamFoundationBuildService>().QueryBuildsByUri(requestContext, (IList<string>) new string[1]
      {
        deploymentUri
      }, (IList<string>) null, QueryOptions.Definitions, QueryDeletedOption.ExcludeDeleted, new Guid(), false))
      {
        using (IEnumerator<BuildDetail> enumerator = foundationDataReader.Current<BuildQueryResult>().Builds.GetEnumerator())
        {
          if (enumerator.MoveNext())
            buildDetail = enumerator.Current;
        }
      }
      if (buildDetail == null)
        throw new InvalidBuildUriException(deploymentUri);
      service.SecurityManager.CheckBuildPermission(requestContext, buildDetail.Definition, BuildPermissions.UpdateBuildInformation);
      try
      {
        List<BuildDeployment> items1;
        List<Tuple<string, Guid>> items2;
        List<Tuple<string, ChangesetDisplayInformation>> items3;
        using (BuildDeploymentComponent component = requestContext.CreateComponent<BuildDeploymentComponent>("Build"))
        {
          using (ResultCollection resultCollection = component.AddBuildDeployment(buildDetail.Definition.TeamProject.Id, deploymentUri, sourceUri, environmentName))
          {
            items1 = resultCollection.GetCurrent<BuildDeployment>().Items;
            resultCollection.NextResult();
            items2 = resultCollection.GetCurrent<Tuple<string, Guid>>().Items;
            resultCollection.NextResult();
            items3 = resultCollection.GetCurrent<Tuple<string, ChangesetDisplayInformation>>().Items;
          }
        }
        return this.ConvertResultsToBuildDeployments(requestContext, items1, items2, items3)[0];
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "Deployment", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(0, "Deployment", "Service", nameof (CreateBuildDeployment));
      }
    }

    public void AddBuildDeploymentProperty(
      IVssRequestContext requestContext,
      string deploymentUri,
      string key,
      string value)
    {
      requestContext.TraceEnter(0, "Deployment", "Service", nameof (AddBuildDeploymentProperty));
      try
      {
        ArgumentValidation.CheckUri(nameof (deploymentUri), deploymentUri, "Build", false, ResourceStrings.MissingUri());
        ArgumentValidation.Check(nameof (key), (object) key, false);
        ArgumentValidation.Check(nameof (value), (object) value, false);
        BuildDetail build = (BuildDetail) null;
        TeamFoundationBuildHost service1 = requestContext.GetService<TeamFoundationBuildHost>();
        using (TeamFoundationDataReader foundationDataReader = requestContext.GetService<TeamFoundationBuildService>().QueryBuildsByUri(requestContext, (IList<string>) new string[1]
        {
          deploymentUri
        }, (IList<string>) null, QueryOptions.Definitions, QueryDeletedOption.ExcludeDeleted, new Guid(), false))
          build = foundationDataReader.Current<BuildQueryResult>().Builds.FirstOrDefault<BuildDetail>();
        if (build == null)
          throw new InvalidBuildUriException(deploymentUri);
        service1.SecurityManager.CheckBuildPermission(requestContext, build.Definition, BuildPermissions.UpdateBuildInformation);
        int num;
        using (BuildDeploymentComponent component = requestContext.CreateComponent<BuildDeploymentComponent>("Build"))
          num = component.IsADeployment(build) ? component.Version : throw new ArgumentException(ResourceStrings.CannotAddPropertyToNondeploymentBuild());
        TeamFoundationPropertyService service2 = requestContext.Elevate().GetService<TeamFoundationPropertyService>();
        List<Microsoft.TeamFoundation.Framework.Server.PropertyValue> propertyValueList1 = new List<Microsoft.TeamFoundation.Framework.Server.PropertyValue>();
        propertyValueList1.Add(new Microsoft.TeamFoundation.Framework.Server.PropertyValue(key, (object) value));
        ArtifactSpec artifactSpec1 = ArtifactHelper.CreateArtifactSpec(deploymentUri, BuildPropertyKinds.BuildDeployment, num >= 5 ? build.Definition.TeamProject.Id : Guid.Empty);
        if (artifactSpec1 == null)
          throw new ArgumentException(ResourceStrings.FailedToExtractArtifactSpecFromUri());
        IVssRequestContext requestContext1 = requestContext;
        ArtifactSpec artifactSpec2 = artifactSpec1;
        List<Microsoft.TeamFoundation.Framework.Server.PropertyValue> propertyValueList2 = propertyValueList1;
        service2.SetProperties(requestContext1, artifactSpec2, (IEnumerable<Microsoft.TeamFoundation.Framework.Server.PropertyValue>) propertyValueList2);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "Deployment", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(0, "Deployment", "Service", nameof (AddBuildDeploymentProperty));
      }
    }

    public List<BuildDeployment> QueryDeployments(
      IVssRequestContext requestContext,
      BuildDeploymentSpec spec)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (QueryDeployments));
      if (!requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.FrameworkNamespaceId).HasPermission(requestContext, FrameworkSecurity.FrameworkNamespaceToken, 1, false))
        return new List<BuildDeployment>();
      try
      {
        Validation.CheckValidatable(requestContext, "specs", (IValidatable) spec, false, ValidationContext.Query);
        List<BuildDeployment> items1;
        List<Tuple<string, Guid>> items2;
        List<Tuple<string, ChangesetDisplayInformation>> items3;
        using (BuildDeploymentComponent component = requestContext.CreateComponent<BuildDeploymentComponent>("Build"))
        {
          using (ResultCollection resultCollection = component.QueryBuildDeployments(requestContext, spec))
          {
            items1 = resultCollection.GetCurrent<BuildDeployment>().Items;
            resultCollection.NextResult();
            items2 = resultCollection.GetCurrent<Tuple<string, Guid>>().Items;
            resultCollection.NextResult();
            items3 = resultCollection.GetCurrent<Tuple<string, ChangesetDisplayInformation>>().Items;
          }
        }
        return this.ConvertResultsToBuildDeployments(requestContext, items1, items2, items3);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "Deployment", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(0, "Deployment", "Service", nameof (QueryDeployments));
      }
    }

    public List<BuildDeployment> QueryDeploymentsByUri(
      IVssRequestContext requestContext,
      IList<string> deploymentUris)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (QueryDeploymentsByUri));
      if (deploymentUris == null || deploymentUris.Count == 0)
        return new List<BuildDeployment>();
      if (!requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.FrameworkNamespaceId).HasPermission(requestContext, FrameworkSecurity.FrameworkNamespaceToken, 1, false))
      {
        List<BuildDeployment> buildDeploymentList = new List<BuildDeployment>();
        for (int index = 0; index < deploymentUris.Count; ++index)
          buildDeploymentList.Add((BuildDeployment) null);
        return buildDeploymentList;
      }
      try
      {
        ArgumentValidation.CheckArray<string>(nameof (deploymentUris), deploymentUris, (Validate<string>) ((argumentName, value, allowEmpty, errorMessage) => ArgumentValidation.CheckUri(argumentName, value, "Build", allowEmpty, errorMessage)), false, (string) null);
        List<BuildDeployment> items1;
        List<Tuple<string, Guid>> items2;
        List<Tuple<string, ChangesetDisplayInformation>> items3;
        using (BuildDeploymentComponent component = requestContext.CreateComponent<BuildDeploymentComponent>("Build"))
        {
          using (ResultCollection resultCollection = component.QueryBuildDeploymentsByUri(requestContext, deploymentUris))
          {
            items1 = resultCollection.GetCurrent<BuildDeployment>().Items;
            resultCollection.NextResult();
            items2 = resultCollection.GetCurrent<Tuple<string, Guid>>().Items;
            resultCollection.NextResult();
            items3 = resultCollection.GetCurrent<Tuple<string, ChangesetDisplayInformation>>().Items;
          }
        }
        List<BuildDeployment> buildDeployments = this.ConvertResultsToBuildDeployments(requestContext, items1, items2, items3);
        for (int index = 0; index < deploymentUris.Count; ++index)
        {
          if (buildDeployments.Count <= index || !string.Equals(buildDeployments[index].Deployment.Uri, deploymentUris[index], StringComparison.OrdinalIgnoreCase))
            buildDeployments.Insert(index, (BuildDeployment) null);
        }
        return buildDeployments;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "Deployment", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(0, "Deployment", "Service", nameof (QueryDeploymentsByUri));
      }
    }

    public BuildQueueQueryResult Redeploy(IVssRequestContext requestContext, string deploymentUri) => this.Redeploy(requestContext, deploymentUri, AzureDeploymentSlot.Staging, false);

    public BuildQueueQueryResult Redeploy(
      IVssRequestContext requestContext,
      string deploymentUri,
      AzureDeploymentSlot deploymentSlot,
      bool updateSlot)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (Redeploy));
      TeamFoundationBuildService service = requestContext.GetService<TeamFoundationBuildService>();
      BuildDetail build = (BuildDetail) null;
      try
      {
        using (TeamFoundationDataReader foundationDataReader = service.QueryBuildsByUri(requestContext, (IList<string>) new string[1]
        {
          deploymentUri
        }, (IList<string>) null, QueryOptions.Definitions | QueryOptions.BatchedRequests, QueryDeletedOption.ExcludeDeleted, new Guid(), false))
          build = foundationDataReader.Current<BuildQueryResult>().Builds.FirstOrDefault<BuildDetail>();
        if (build == null)
          throw new CannotRedeployException(ResourceStrings.CannotRedeployBuildNotFound());
        requestContext.GetService<TeamFoundationBuildHost>().SecurityManager.CheckBuildPermission(requestContext, build.Definition, BuildPermissions.QueueBuilds);
        if (build.Reason == BuildReason.CheckInShelveset || build.Reason == BuildReason.ValidateShelveset || build.QueueIds.Count != 1)
          throw new CannotRedeployException(ResourceStrings.CannotRedeployBuildReason());
        using (BuildDeploymentComponent component = requestContext.CreateComponent<BuildDeploymentComponent>("Build"))
        {
          if (!component.IsADeployment(build))
            throw new CannotRedeployException(ResourceStrings.CannotRedeployNondeploymentBuild());
        }
        BuildDefinition definition = build.Definition;
        string sourceGetVersion = build.SourceGetVersion;
        switch (VersionSpec.ParseSingleSpec(sourceGetVersion, "."))
        {
          case ChangesetVersionSpec _:
          case DateVersionSpec _:
            string processParameters1 = build.ProcessParameters;
            string processParameters2;
            if (!updateSlot)
              processParameters2 = processParameters1;
            else if (string.IsNullOrWhiteSpace(processParameters1))
            {
              processParameters2 = XamlHelper.Save((IDictionary<string, string>) new Dictionary<string, string>()
              {
                {
                  "AlternateDeploymentSlot",
                  deploymentSlot.ToString()
                }
              });
            }
            else
            {
              XElement xelement1 = XElement.Parse(processParameters1);
              XName name1 = XName.Get("String", "http://schemas.microsoft.com/winfx/2006/xaml");
              XName name2 = XName.Get("Key", "http://schemas.microsoft.com/winfx/2006/xaml");
              IEnumerable<XElement> xelements = xelement1.Elements().Select<XElement, XElement>((Func<XElement, XElement>) (element => element));
              bool flag = false;
              foreach (XElement xelement2 in xelements)
              {
                if (xelement2.Attribute(name2).Value.Equals("AlternateDeploymentSlot", StringComparison.OrdinalIgnoreCase))
                {
                  xelement2.SetValue((object) deploymentSlot.ToString());
                  flag = true;
                  break;
                }
              }
              if (!flag)
              {
                XElement content = new XElement(name1);
                content.SetAttributeValue(name2, (object) "AlternateDeploymentSlot");
                content.SetValue((object) deploymentSlot.ToString());
                xelement1.Add((object) content);
              }
              processParameters2 = xelement1.ToString();
            }
            return service.QueueBuilds(requestContext, (IList<BuildRequest>) new BuildRequest[1]
            {
              new BuildRequest(build.BuildControllerUri, build.BuildDefinitionUri, definition.DefaultDropLocation, QueuePriority.Normal, processParameters2, BuildReason.Manual)
              {
                GetOption = GetOption.Custom,
                CustomGetVersion = sourceGetVersion
              }
            }, QueueOptions.None, new Guid());
          default:
            throw new CannotRedeployException(ResourceStrings.CannotRedeployBuildReason());
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "Deployment", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(0, "Deployment", "Service", nameof (Redeploy));
      }
    }

    private Guid GetProjectIdFromName(IVssRequestContext requestContext, string teamProject) => this.GetProjectIdFromName(requestContext, teamProject, out string _);

    private Guid GetProjectIdFromName(
      IVssRequestContext requestContext,
      string teamProjectName,
      out string projectUri)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(teamProjectName, nameof (teamProjectName));
      TeamProject projectFromGuidOrName = requestContext.GetService<IProjectService>().GetTeamProjectFromGuidOrName(requestContext, teamProjectName);
      projectUri = projectFromGuidOrName.Uri;
      return projectFromGuidOrName.Id;
    }

    private static IVssRequestContext GetStrongBoxRequestContext(
      IVssRequestContext requestContext,
      string projectUri)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.TeamProjectNamespaceId);
      securityNamespace.CheckPermission(requestContext, securityNamespace.NamespaceExtension.HandleIncomingToken(requestContext, securityNamespace, projectUri), TeamProjectPermissions.GenericWrite, false);
      return requestContext.Elevate();
    }

    private List<BuildDeployment> ConvertResultsToBuildDeployments(
      IVssRequestContext requestContext,
      List<BuildDeployment> buildDeployments,
      List<Tuple<string, Guid>> buildDeploymentRequestForRaw,
      List<Tuple<string, ChangesetDisplayInformation>> buildDeploymentChangeset)
    {
      Guid[] array = buildDeploymentRequestForRaw.Select<Tuple<string, Guid>, Guid>((Func<Tuple<string, Guid>, Guid>) (x => x.Item2)).Distinct<Guid>().ToArray<Guid>();
      Dictionary<Guid, TeamFoundationIdentity> dictionary1 = ((IEnumerable<TeamFoundationIdentity>) requestContext.GetService<TeamFoundationIdentityService>().ReadIdentities(requestContext, array)).ToDictionary<TeamFoundationIdentity, Guid>((Func<TeamFoundationIdentity, Guid>) (i => i.TeamFoundationId));
      List<Tuple<string, RequestedForDisplayInformation>> source = new List<Tuple<string, RequestedForDisplayInformation>>();
      foreach (Tuple<string, Guid> tuple in buildDeploymentRequestForRaw)
      {
        RequestedForDisplayInformation displayInformation = new RequestedForDisplayInformation(tuple.Item2, dictionary1[tuple.Item2].DisplayName);
        source.Add(new Tuple<string, RequestedForDisplayInformation>(tuple.Item1, displayInformation));
      }
      Dictionary<string, IGrouping<string, RequestedForDisplayInformation>> dictionary2 = source.GroupBy<Tuple<string, RequestedForDisplayInformation>, string, RequestedForDisplayInformation>((Func<Tuple<string, RequestedForDisplayInformation>, string>) (x => x.Item1), (Func<Tuple<string, RequestedForDisplayInformation>, RequestedForDisplayInformation>) (x => x.Item2)).ToDictionary<IGrouping<string, RequestedForDisplayInformation>, string, IGrouping<string, RequestedForDisplayInformation>>((Func<IGrouping<string, RequestedForDisplayInformation>, string>) (x => x.Key), (Func<IGrouping<string, RequestedForDisplayInformation>, IGrouping<string, RequestedForDisplayInformation>>) (x => x));
      Dictionary<string, IGrouping<string, ChangesetDisplayInformation>> dictionary3 = buildDeploymentChangeset.GroupBy<Tuple<string, ChangesetDisplayInformation>, string, ChangesetDisplayInformation>((Func<Tuple<string, ChangesetDisplayInformation>, string>) (x => x.Item1), (Func<Tuple<string, ChangesetDisplayInformation>, ChangesetDisplayInformation>) (x => x.Item2)).ToDictionary<IGrouping<string, ChangesetDisplayInformation>, string, IGrouping<string, ChangesetDisplayInformation>>((Func<IGrouping<string, ChangesetDisplayInformation>, string>) (x => x.Key), (Func<IGrouping<string, ChangesetDisplayInformation>, IGrouping<string, ChangesetDisplayInformation>>) (x => x));
      TswaServerHyperlinkService service1 = requestContext.GetService<TswaServerHyperlinkService>();
      TeamFoundationPropertyService service2 = requestContext.Elevate().GetService<TeamFoundationPropertyService>();
      foreach (BuildDeployment buildDeployment in buildDeployments)
      {
        string str = service1.GetViewBuildDetailsUrl(new Uri(buildDeployment.Deployment.Uri)).ToString();
        buildDeployment.WebsiteUrl = str;
        using (TeamFoundationDataReader properties = service2.GetProperties(requestContext, ArtifactHelper.CreateArtifactSpec(buildDeployment.Deployment.Uri, BuildPropertyKinds.BuildDeployment, buildDeployment.ProjectId), (IEnumerable<string>) null))
        {
          foreach (ArtifactPropertyValue artifactPropertyValue in properties)
          {
            foreach (Microsoft.TeamFoundation.Framework.Server.PropertyValue propertyValue in artifactPropertyValue.PropertyValues)
              buildDeployment.Properties.Add(propertyValue);
          }
        }
        IGrouping<string, RequestedForDisplayInformation> collection1;
        if (dictionary2.TryGetValue(buildDeployment.Deployment.Uri, out collection1))
          buildDeployment.Deployment.RequestedFor.AddRange((IEnumerable<RequestedForDisplayInformation>) collection1);
        if (dictionary2.TryGetValue(buildDeployment.Source.Uri, out collection1))
          buildDeployment.Source.RequestedFor.AddRange((IEnumerable<RequestedForDisplayInformation>) collection1);
        IGrouping<string, ChangesetDisplayInformation> collection2;
        if (dictionary3.TryGetValue(buildDeployment.Deployment.Uri, out collection2))
          buildDeployment.Deployment.ChangeSet.AddRange((IEnumerable<ChangesetDisplayInformation>) collection2);
        if (dictionary3.TryGetValue(buildDeployment.Source.Uri, out collection2))
          buildDeployment.Source.ChangeSet.AddRange((IEnumerable<ChangesetDisplayInformation>) collection2);
        if (!string.IsNullOrWhiteSpace(buildDeployment.SourceGetVersion))
        {
          try
          {
            int changesetNumber = VersionSpec.ParseSingleSpec(buildDeployment.SourceGetVersion, ".").ToChangeset(requestContext);
            if (changesetNumber != VersionSpec.UnknownChangeset)
            {
              if (!buildDeployment.Source.ChangeSet.Exists((Predicate<ChangesetDisplayInformation>) (x => x.ChangesetId == changesetNumber)))
                buildDeployment.Source.ChangeSet.Add(this.GetChangesetDisplayInformation(requestContext, changesetNumber));
            }
          }
          catch (InvalidVersionSpecException ex)
          {
          }
          catch (ChangesetNotFoundException ex)
          {
          }
          catch (DateVersionSpecBeforeBeginningOfRepositoryException ex)
          {
          }
          catch (ResourceAccessException ex)
          {
          }
        }
      }
      return buildDeployments;
    }

    private ChangesetDisplayInformation GetChangesetDisplayInformation(
      IVssRequestContext requestContext,
      int changesetNumber)
    {
      using (TeamFoundationDataReader foundationDataReader = requestContext.GetService<TeamFoundationVersionControlService>().QueryChangeset(requestContext, changesetNumber, false, false, false))
      {
        Changeset changeset = foundationDataReader.Current<Changeset>();
        return new ChangesetDisplayInformation(changesetNumber, changeset.OwnerDisplayName);
      }
    }

    private ProcessTemplate FindAzureContinuousDeploymentTemplate(
      IVssRequestContext requestContext,
      TeamFoundationBuildService build,
      string teamProject,
      bool gitTemplate)
    {
      string str = !gitTemplate ? "TfvcContinuousDeploymentTemplate.12" : "GitContinuousDeploymentTemplate.12";
      List<ProcessTemplate> processTemplateList = build.QueryProcessTemplates(requestContext, teamProject, (IList<ProcessTemplateType>) new List<ProcessTemplateType>()
      {
        ProcessTemplateType.Custom
      });
      ProcessTemplate deploymentTemplate = (ProcessTemplate) null;
      foreach (ProcessTemplate processTemplate in processTemplateList)
      {
        if (processTemplate.ServerPath.Contains(str))
        {
          deploymentTemplate = processTemplate;
          break;
        }
      }
      return deploymentTemplate;
    }

    private BuildController FindHostedBuildController(IVssRequestContext requestContext)
    {
      TeamFoundationBuildResourceService service = requestContext.GetService<TeamFoundationBuildResourceService>();
      BuildControllerSpec controllerSpec = new BuildControllerSpec(BuildConstants.Star, BuildConstants.Star, false);
      BuildController hostedBuildController = (BuildController) null;
      foreach (BuildController controller in service.QueryBuildControllers(requestContext, controllerSpec).Controllers)
      {
        TeamFoundationBuildResourceService buildResourceService = service;
        IVssRequestContext requestContext1 = requestContext;
        foreach (BuildServiceHost serviceHost in buildResourceService.QueryBuildServiceHostsByUri(requestContext1, (IList<string>) new List<string>()
        {
          controller.ServiceHostUri
        }).ServiceHosts)
        {
          if (serviceHost.IsVirtual)
          {
            hostedBuildController = controller;
            break;
          }
        }
        if (hostedBuildController != null)
          break;
      }
      return hostedBuildController;
    }

    private string FindSolutionToBuild(IVssRequestContext requestContext, string teamProjectPath)
    {
      TeamFoundationVersionControlService service = requestContext.GetService<TeamFoundationVersionControlService>();
      ItemSpec itemSpec = new ItemSpec(VersionControlPath.Combine(teamProjectPath, "*.sln"), RecursionType.Full);
      IVssRequestContext requestContext1 = requestContext;
      ItemSpec[] items = new ItemSpec[1]{ itemSpec };
      LatestVersionSpec version = new LatestVersionSpec();
      using (TeamFoundationDataReader foundationDataReader = service.QueryItems(requestContext1, (string) null, (string) null, items, (VersionSpec) version, DeletedState.NonDeleted, ItemType.File, false, 0))
      {
        foreach (ItemSet itemSet in foundationDataReader)
        {
          if (itemSet.Items.Count<Microsoft.TeamFoundation.VersionControl.Server.Item>() == 1)
          {
            itemSet.Items.MoveNext();
            return itemSet.Items.Current.ServerItem;
          }
        }
      }
      return (string) null;
    }

    private void FindGitSolutionToBuild(
      TfsGitTree gitTree,
      string parentPath,
      HashSet<string> solutions)
    {
      if (gitTree == null)
        return;
      if (!string.IsNullOrEmpty(parentPath) && !parentPath.EndsWith("/"))
        parentPath += "/";
      foreach (TfsGitTreeEntry treeEntry in gitTree.GetTreeEntries())
      {
        if (treeEntry.ObjectType == GitObjectType.Tree)
          this.FindGitSolutionToBuild((TfsGitTree) treeEntry.Object, treeEntry.Name, solutions);
        else if (treeEntry.Name.EndsWith(".sln", StringComparison.OrdinalIgnoreCase))
          solutions.Add(parentPath + treeEntry.Name);
      }
    }

    private BuildDefinition ConnectAzureCloudAppInternal(
      IVssRequestContext requestContext,
      string teamProject,
      string subscriptionId,
      AzureCloudAppProfile profile)
    {
      string hostedServiceName = profile.AzureHostedServiceName;
      DeploymentEnvironment deploymentEnvironment = (DeploymentEnvironment) null;
      try
      {
        deploymentEnvironment = this.GetDeploymentEnvironment(requestContext, hostedServiceName, teamProject);
      }
      catch (DeploymentEnvironmentNotFoundException ex)
      {
      }
      if (deploymentEnvironment == null)
      {
        DeploymentEnvironmentCreationData deploymentEnvironmentCreationData = new DeploymentEnvironmentCreationData(hostedServiceName, teamProject, subscriptionId, DeploymentEnvironmentKind.AzureCloudApp, "", "", new Dictionary<string, string>()
        {
          {
            "publishProfile",
            profile.ToXml()
          }
        });
        this.CreateDeploymentEnvironment(requestContext, deploymentEnvironmentCreationData);
      }
      return this.CreateAzureDeploymentBuildDefinition(requestContext, hostedServiceName, teamProject);
    }

    private ShallowReference ConnectAzureCloudAppInternalWithNewBuildDefinition(
      IVssRequestContext requestContext,
      string teamProject,
      string storageAccountName,
      string hostedServiceName,
      string subscriptionId,
      AzureCloudAppProfile profile)
    {
      return JsonConvert.DeserializeObject<ShallowReference>(this.m_build2Converter.CreateAzureCloudBuildDefinition(requestContext, this.GetBuildDefinitionNameFromEnvironmentName(profile.AzureHostedServiceName), teamProject, hostedServiceName, subscriptionId, storageAccountName));
    }

    private BuildDefinition CreateAzureDeploymentBuildDefinition(
      IVssRequestContext requestContext,
      string deploymentEnvironmentName,
      string teamProject,
      string repositoryId = null,
      string gitBranch = null)
    {
      CommonStructureProjectInfo project = requestContext.Elevate().GetService<CommonStructureService>().GetProjectFromName(requestContext.Elevate(), teamProject);
      bool gitTemplate = TeamFoundationBuildService.IsGitTeamProject(requestContext, project.Uri);
      TeamFoundationBuildService service1 = requestContext.GetService<TeamFoundationBuildService>();
      string fromEnvironmentName = this.GetBuildDefinitionNameFromEnvironmentName(deploymentEnvironmentName);
      string fullPath = BuildPath.Root(teamProject, fromEnvironmentName);
      BuildDefinition deploymentBuildDefinition = service1.QueryBuildDefinitions(requestContext, new BuildDefinitionSpec(fullPath)).Definitions.FirstOrDefault<BuildDefinition>();
      if (deploymentBuildDefinition != null)
      {
        if (deploymentBuildDefinition.QueueStatus == DefinitionQueueStatus.Disabled && (deploymentBuildDefinition.ProcessParameters.Contains("SolutionToBuild") && !gitTemplate || deploymentBuildDefinition.ProcessParameters.Contains("ProjectsToBuild")))
        {
          deploymentBuildDefinition.QueueStatus = DefinitionQueueStatus.Enabled;
          deploymentBuildDefinition = service1.UpdateBuildDefinitions(requestContext, (IList<BuildDefinition>) new List<BuildDefinition>()
          {
            deploymentBuildDefinition
          }).FirstOrDefault<BuildDefinition>();
        }
        return deploymentBuildDefinition;
      }
      BuildDefinition buildDefinition = new BuildDefinition();
      ProcessTemplate deploymentTemplate = this.FindAzureContinuousDeploymentTemplate(requestContext, service1, teamProject, gitTemplate);
      if (deploymentTemplate == null)
        throw new ArgumentException(ResourceStrings.AzureContinuousDeploymentTemplateNotFound((object) teamProject));
      BuildController hostedBuildController = this.FindHostedBuildController(requestContext);
      if (hostedBuildController == null)
        throw new BuildServerException(ResourceStrings.CannotCreateCDBuildDefWithoutHostedController());
      buildDefinition.Process = deploymentTemplate;
      buildDefinition.FullPath = fullPath;
      buildDefinition.TriggerType = DefinitionTriggerType.ContinuousIntegration;
      buildDefinition.DefaultDropLocation = BuildContainerPath.RootFolder;
      buildDefinition.BuildControllerUri = hostedBuildController.Uri;
      DeleteOptions deleteOptions = DeleteOptions.DropLocation | DeleteOptions.Label | DeleteOptions.Details | DeleteOptions.Symbols;
      buildDefinition.RetentionPolicies.Add(new RetentionPolicy(BuildReason.Triggered, BuildStatus.Failed, 10)
      {
        DeleteOptions = deleteOptions
      });
      buildDefinition.RetentionPolicies.Add(new RetentionPolicy(BuildReason.Triggered, BuildStatus.PartiallySucceeded, 10)
      {
        DeleteOptions = deleteOptions
      });
      buildDefinition.RetentionPolicies.Add(new RetentionPolicy(BuildReason.Triggered, BuildStatus.Stopped, 1)
      {
        DeleteOptions = deleteOptions
      });
      buildDefinition.RetentionPolicies.Add(new RetentionPolicy(BuildReason.Triggered, BuildStatus.Succeeded, 10)
      {
        DeleteOptions = deleteOptions
      });
      buildDefinition.RetentionPolicies.Add(new RetentionPolicy(BuildReason.ValidateShelveset, BuildStatus.Failed, 10)
      {
        DeleteOptions = deleteOptions
      });
      buildDefinition.RetentionPolicies.Add(new RetentionPolicy(BuildReason.ValidateShelveset, BuildStatus.PartiallySucceeded, 10)
      {
        DeleteOptions = deleteOptions
      });
      buildDefinition.RetentionPolicies.Add(new RetentionPolicy(BuildReason.ValidateShelveset, BuildStatus.Stopped, 1)
      {
        DeleteOptions = deleteOptions
      });
      buildDefinition.RetentionPolicies.Add(new RetentionPolicy(BuildReason.ValidateShelveset, BuildStatus.Succeeded, 10)
      {
        DeleteOptions = deleteOptions
      });
      if (gitTemplate)
      {
        ITeamFoundationGitRepositoryService service2 = requestContext.GetService<ITeamFoundationGitRepositoryService>();
        IList<TfsGitRepositoryInfo> source = service2.QueryRepositories(requestContext, project.Uri, true);
        if (source.Count == 0)
          throw new BuildServerException(ResourceStrings.NoValidRepoFound((object) project.Name));
        TfsGitRepositoryInfo gitRepositoryInfo;
        Guid repoId;
        if (!string.IsNullOrEmpty(repositoryId) && Guid.TryParse(repositoryId, out repoId))
        {
          gitRepositoryInfo = source.Where<TfsGitRepositoryInfo>((Func<TfsGitRepositoryInfo, bool>) (x => x.Key.RepoId == repoId)).FirstOrDefault<TfsGitRepositoryInfo>();
          if (gitRepositoryInfo == null)
            throw new BuildServerException(ResourceStrings.NoValidRepoFound((object) project.Name));
        }
        else
          gitRepositoryInfo = source.Where<TfsGitRepositoryInfo>((Func<TfsGitRepositoryInfo, bool>) (x => x.Name.Equals(project.Name, StringComparison.OrdinalIgnoreCase))).FirstOrDefault<TfsGitRepositoryInfo>();
        if (gitRepositoryInfo == null)
          gitRepositoryInfo = source.First<TfsGitRepositoryInfo>();
        using (ITfsGitRepository repositoryByNameAndUri = service2.FindRepositoryByNameAndUri(requestContext, project.Uri, gitRepositoryInfo.Name))
        {
          string repositoryCloneUri = repositoryByNameAndUri.GetRepositoryCloneUri();
          if (string.IsNullOrEmpty(gitBranch))
            gitBranch = gitRepositoryInfo.DefaultBranch;
          buildDefinition.SourceProviders.Add(new BuildDefinitionSourceProvider()
          {
            Name = BuildSourceProviders.TfGit,
            Fields = {
              new NameValueField(BuildSourceProviders.GitProperties.RepositoryName, BuildSourceProviders.GitProperties.CreateUniqueRepoName(project.Name, gitRepositoryInfo.Name)),
              new NameValueField(BuildSourceProviders.GitProperties.DefaultBranch, gitBranch),
              new NameValueField(BuildSourceProviders.GitProperties.CIBranches, gitBranch),
              new NameValueField(BuildSourceProviders.GitProperties.RepositoryUrl, repositoryCloneUri)
            }
          });
          TfsGitRef tfsGitRef = string.IsNullOrEmpty(gitBranch) ? repositoryByNameAndUri.Refs.GetDefault() : repositoryByNameAndUri.Refs.MatchingName(gitBranch);
          TfsGitTree gitTree = (TfsGitTree) null;
          if (tfsGitRef != null)
          {
            TfsGitObject tfsGitObject = repositoryByNameAndUri.LookupObject(tfsGitRef.ObjectId);
            if (tfsGitObject.ObjectType == GitObjectType.Commit)
              gitTree = (tfsGitObject as TfsGitCommit).GetTree();
          }
          HashSet<string> stringSet = new HashSet<string>();
          string solution = string.Empty;
          this.FindGitSolutionToBuild(gitTree, string.Empty, stringSet);
          if (stringSet.Count != 1)
            buildDefinition.QueueStatus = DefinitionQueueStatus.Disabled;
          else
            solution = stringSet.First<string>();
          buildDefinition.ProcessParameters = TeamFoundationDeploymentService.SetProcessParamsV12(deploymentEnvironmentName, solution);
          buildDefinition.WorkspaceTemplate = this.GetWorkspaceTemplate(requestContext, VersionControlPath.Combine("$/", teamProject));
        }
      }
      else
      {
        string teamProjectPath = VersionControlPath.Combine("$/", teamProject);
        string solutionToBuild = this.FindSolutionToBuild(requestContext, teamProjectPath);
        string directory;
        if (string.IsNullOrEmpty(solutionToBuild))
        {
          directory = teamProjectPath;
          buildDefinition.QueueStatus = DefinitionQueueStatus.Disabled;
        }
        else
          directory = VersionControlPath.GetFolderName(solutionToBuild);
        buildDefinition.WorkspaceTemplate = this.GetWorkspaceTemplate(requestContext, directory);
        buildDefinition.ProcessParameters = TeamFoundationDeploymentService.SetProcessParamsV12(deploymentEnvironmentName, solutionToBuild);
      }
      return service1.AddBuildDefinitions(requestContext, (IList<BuildDefinition>) new List<BuildDefinition>()
      {
        buildDefinition
      }).FirstOrDefault<BuildDefinition>();
    }

    private void DisconnectDeployment(
      IVssRequestContext requestContext,
      string teamProject,
      string deploymentEnvironmentName)
    {
      requestContext.TraceEnter(0, "Deployment", "Service", nameof (DisconnectDeployment));
      ArgumentUtility.CheckStringForNullOrEmpty(teamProject, nameof (teamProject));
      ArgumentUtility.CheckStringForNullOrEmpty(deploymentEnvironmentName, nameof (deploymentEnvironmentName));
      try
      {
        DeploymentEnvironment deploymentEnvironment = (DeploymentEnvironment) null;
        try
        {
          deploymentEnvironment = this.GetDeploymentEnvironment(requestContext, deploymentEnvironmentName, teamProject);
        }
        catch (DeploymentEnvironmentNotFoundException ex)
        {
        }
        if (deploymentEnvironment == null)
          return;
        this.DeleteDeploymentEnvironment(requestContext, deploymentEnvironmentName, teamProject);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "Deployment", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(0, "Deployment", "Service", nameof (DisconnectDeployment));
      }
    }

    private void DisconnectDeploymentBuildDefinition(
      IVssRequestContext requestContext,
      string teamProject,
      string deploymentEnvironmentName)
    {
      requestContext.TraceEnter(0, "Deployment", "Service", nameof (DisconnectDeploymentBuildDefinition));
      this.DisconnectDeployment(requestContext, teamProject, deploymentEnvironmentName);
      try
      {
        this.DisableBuildDefinition(requestContext, teamProject, deploymentEnvironmentName);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "Deployment", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(0, "Deployment", "Service", nameof (DisconnectDeploymentBuildDefinition));
      }
    }

    private void DisconnectDeploymentNewBuildDefinition(
      IVssRequestContext requestContext,
      string teamProject,
      string deploymentEnvironmentName)
    {
      requestContext.TraceEnter(0, "Deployment", "Service", nameof (DisconnectDeploymentNewBuildDefinition));
      this.DisconnectDeployment(requestContext, teamProject, deploymentEnvironmentName);
      string fromEnvironmentName = this.GetBuildDefinitionNameFromEnvironmentName(deploymentEnvironmentName);
      try
      {
        if (this.m_build2Converter.DisconnectBuildDefinition(requestContext, fromEnvironmentName, teamProject))
          return;
        this.DisableBuildDefinition(requestContext, teamProject, deploymentEnvironmentName);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "Deployment", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(0, "Deployment", "Service", nameof (DisconnectDeploymentNewBuildDefinition));
      }
    }

    private void DisableBuildDefinition(
      IVssRequestContext requestContext,
      string teamProject,
      string deploymentEnvironmentName)
    {
      string fromEnvironmentName = this.GetBuildDefinitionNameFromEnvironmentName(deploymentEnvironmentName);
      string fullPath = BuildPath.Root(teamProject, fromEnvironmentName);
      TeamFoundationBuildService service = requestContext.GetService<TeamFoundationBuildService>();
      BuildDefinition buildDefinition = service.QueryBuildDefinitions(requestContext, new BuildDefinitionSpec(fullPath)).Definitions.FirstOrDefault<BuildDefinition>();
      if (buildDefinition == null || buildDefinition.QueueStatus != DefinitionQueueStatus.Enabled)
        return;
      buildDefinition.QueueStatus = DefinitionQueueStatus.Disabled;
      service.UpdateBuildDefinitions(requestContext, (IList<BuildDefinition>) new BuildDefinition[1]
      {
        buildDefinition
      });
    }

    internal string GetBuildDefinitionNameFromEnvironmentName(string deploymentEnvironmentName) => deploymentEnvironmentName.Length <= 48 ? deploymentEnvironmentName + "_CD" : deploymentEnvironmentName.Substring(0, 48) + "_CD";

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string SetProcessParams(string depEnvName, string solution)
    {
      IDictionary<string, object> instance = (IDictionary<string, object>) new Dictionary<string, object>();
      if (!string.IsNullOrEmpty(depEnvName))
        instance["DeploymentEnvironmentName"] = (object) depEnvName;
      if (!string.IsNullOrEmpty(solution))
        instance["SolutionToBuild"] = (object) solution;
      return XamlServices.Save((object) instance);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string SetProcessParamsV12(string depEnvName, string solution)
    {
      IDictionary<string, object> instance = (IDictionary<string, object>) new Dictionary<string, object>();
      if (!string.IsNullOrEmpty(solution))
        instance["ProjectsToBuild"] = (object) new string[1]
        {
          solution
        };
      if (!string.IsNullOrEmpty(depEnvName))
      {
        BuildParameter buildParameter = new BuildParameter(string.Format("{{\"SharePointDeploymentEnvironmentName\":\"\",\"ProviderHostedDeploymentEnvironmentName\":\"{0}\",\"PublishProfile\":\"\",\"AllowUntrustedCertificates\":true,\"AllowUpgrade\":true}}", (object) depEnvName));
        instance["DeploymentSettings"] = (object) buildParameter;
      }
      return XamlServices.Save((object) instance);
    }

    private WorkspaceTemplate GetWorkspaceTemplate(
      IVssRequestContext requestContext,
      string directory)
    {
      List<WorkspaceMapping> mappings = new List<WorkspaceMapping>();
      List<string> stringList = new List<string>()
      {
        "BuildProcessTemplates",
        "Drops"
      };
      mappings.Add(new WorkspaceMapping()
      {
        ServerItem = directory,
        LocalItem = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "$(SourceDir)\\{0}", (object) VersionControlPath.GetFileName(directory)),
        MappingType = WorkspaceMappingType.Map
      });
      if (string.Compare(directory, VersionControlPath.GetTeamProject(directory), StringComparison.OrdinalIgnoreCase) == 0)
      {
        foreach (string relative in stringList)
          mappings.Add(new WorkspaceMapping()
          {
            ServerItem = VersionControlPath.Combine(directory, relative),
            LocalItem = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "$(SourceDir)\\{0}", (object) relative),
            MappingType = WorkspaceMappingType.Cloak
          });
      }
      return new WorkspaceTemplate(mappings);
    }

    internal static class AzureDeploymentEnvironmentKeys
    {
      internal const string Subscription = "subscription";
      internal const string Webspace = "webspace";
      internal const string Website = "website";
      internal const string PublishProfile = "publishProfile";
    }
  }
}
