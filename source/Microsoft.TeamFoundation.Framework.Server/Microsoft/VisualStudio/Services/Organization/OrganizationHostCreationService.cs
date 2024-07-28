// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.OrganizationHostCreationService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.HostManagement;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Organization
{
  public class OrganizationHostCreationService : IVssFrameworkService, IHostCreator
  {
    private const string c_area = "Organization";
    private const string c_layer = "OrganizationHostCreationService";

    public void ServiceStart(IVssRequestContext requestContext) => requestContext.CheckDeploymentRequestContext();

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public virtual ServicingJobDetail CreateHost(
      IVssRequestContext requestContext,
      ServiceHostProperties hostProperties,
      IDictionary<string, string> servicingTokens = null,
      Microsoft.VisualStudio.Services.Identity.Identity requestingIdentity = null)
    {
      ServicingJobData servicingJobData = this.GetServicingJobData(requestContext, hostProperties, servicingTokens, requestingIdentity);
      ServicingJobDetail host = requestContext.GetService<TeamFoundationServicingService>().PerformServicingJob(requestContext, servicingJobData, hostProperties.HostId, DateTime.UtcNow, (ITFLogger) null);
      if (host.Result == ServicingJobResult.Failed)
        throw new ConfigurationErrorsException(FrameworkResources.CreateAccountFailed((object) host.JobId.ToString("D")));
      OrganizationHostModifiedMessage hostModifiedMessage = new OrganizationHostModifiedMessage();
      hostModifiedMessage.HostId = hostProperties.HostId;
      hostModifiedMessage.HostName = hostProperties.Name;
      hostModifiedMessage.ModificationType = HostModificationType.Created;
      OrganizationHostModifiedMessage notificationEvent = hostModifiedMessage;
      requestContext.GetService<TeamFoundationEventService>().PublishNotification(requestContext, (object) notificationEvent);
      return host;
    }

    public virtual void CreateVirtualHost(
      IVssRequestContext requestContext,
      ServiceHostProperties hostProperties)
    {
      requestContext.GetService<IVirtualHostInstanceMappingRegistrationService>().RegisterVirtualHostInstanceMapping(requestContext, hostProperties.HostId);
      TeamFoundationServiceHostProperties serviceHostProperties = new TeamFoundationServiceHostProperties();
      serviceHostProperties.Id = hostProperties.HostId;
      serviceHostProperties.ParentId = requestContext.ServiceHost.InstanceId;
      serviceHostProperties.Name = hostProperties.Name;
      serviceHostProperties.Description = hostProperties.Description;
      serviceHostProperties.Status = TeamFoundationServiceHostStatus.Started;
      serviceHostProperties.StatusReason = (string) null;
      serviceHostProperties.HostType = TeamFoundationHostType.Application;
      serviceHostProperties.DatabaseId = -2;
      serviceHostProperties.StorageAccountId = -2;
      serviceHostProperties.ServiceLevel = requestContext.ServiceHost.DeploymentServiceHost.DatabaseProperties.ServiceLevel;
      TeamFoundationServiceHostProperties hostProperties1 = serviceHostProperties;
      requestContext.GetService<ITeamFoundationHostManagementService>().CreateServiceHost(requestContext, hostProperties1, (ISqlConnectionInfo) null, CreateHostOptions.None);
    }

    private ServicingJobData GetServicingJobData(
      IVssRequestContext requestContext,
      ServiceHostProperties hostProperties,
      IDictionary<string, string> servicingTokens,
      Microsoft.VisualStudio.Services.Identity.Identity requestingIdentity)
    {
      ServicingJobData servicingJobData = new ServicingJobData(new string[1]
      {
        "CreateHostingOrganization"
      });
      servicingJobData.JobTitle = string.Format((IFormatProvider) CultureInfo.InvariantCulture, JobNameConstants.CreateHostingOrganizationFormat, (object) hostProperties.Name, (object) hostProperties.HostId);
      servicingJobData.OperationClass = "CreateHostingOrganization";
      servicingJobData.ServicingOptions = ServicingFlags.None;
      servicingJobData.ServicingHostId = hostProperties.HostId;
      servicingJobData.ServicingItems[ServicingItemConstants.HostProperties] = (object) hostProperties;
      servicingJobData.ServicingItems[ServicingItemConstants.AccountId] = (object) hostProperties.HostId;
      servicingJobData.ServicingItems[ServicingItemConstants.AccountHostId] = (object) hostProperties.HostId;
      servicingJobData.ServicingItems[ServicingItemConstants.AccountName] = (object) hostProperties.Name;
      servicingJobData.ServicingItems[ServicingItemConstants.ServicingLogLevel] = (object) 2;
      servicingJobData.ServicingTokens[ServicingTokenConstants.FinalHostState] = "Started";
      string str = requestContext.ServiceHost.DeploymentServiceHost.DatabaseProperties.ServiceLevel;
      if (str.EndsWith(";", StringComparison.Ordinal))
        str = str.Substring(0, str.Length - 1);
      servicingJobData.ServicingTokens[ServicingTokenConstants.ServiceLevel] = str;
      servicingJobData.ServicingTokens[ServicingTokenConstants.UseSharedService] = (requestContext.ServiceInstanceType() != ServiceInstanceTypes.SPS).ToString();
      if (requestingIdentity != null)
      {
        servicingJobData.ServicingItems[ServicingItemConstants.RequestingIdentity] = (object) requestingIdentity.Descriptor;
        if (requestContext.ExecutionEnvironment.IsHostedDeployment)
          servicingJobData.ServicingItems[ServicingItemConstants.RequestingIdentityObject] = (object) requestingIdentity;
      }
      if (requestContext.ServiceInstanceType() != ServiceInstanceTypes.SPS)
        servicingJobData.ServicingTokens.Add(ServicingItemConstants.SkipCollectionCreation, bool.TrueString);
      if (servicingTokens != null)
      {
        foreach (KeyValuePair<string, string> servicingToken in (IEnumerable<KeyValuePair<string, string>>) servicingTokens)
          servicingJobData.ServicingTokens[servicingToken.Key] = servicingToken.Value;
      }
      return servicingJobData;
    }
  }
}
