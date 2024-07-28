// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.ConnectionsService
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Pipelines.Server.Providers;
using Microsoft.TeamFoundation.Pipelines.WebApi;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ConnectionsService : IConnectionsService, IVssFrameworkService
  {
    private const string c_layer = "ConnectionsService";
    private Guid m_serviceHostId;

    public void ServiceStart(IVssRequestContext context)
    {
      this.m_serviceHostId = context.ServiceHost.InstanceId;
      this.ValidateRequestContext(context);
    }

    public void ServiceEnd(IVssRequestContext context)
    {
    }

    public static ProjectInfo GetProjectInfoFromTeamProject(
      IVssRequestContext requestContext,
      TeamProject project)
    {
      IProjectService service = requestContext.GetService<IProjectService>();
      ProjectInfo project1;
      if (project.Id == Guid.Empty)
      {
        ArgumentUtility.CheckStringForNullOrEmpty(project.Name, "projectName");
        project1 = service.GetProject(requestContext, project.Name);
      }
      else
        project1 = service.GetProject(requestContext, project.Id);
      return project1;
    }

    public PipelineConnection CreateConnection(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      CreatePipelineConnectionInputs inputs)
    {
      return this.CreateConnectionInternal(requestContext, projectInfo, inputs);
    }

    private PipelineConnection CreateConnectionInternal(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      CreatePipelineConnectionInputs inputs,
      ServiceEndpoint endpoint = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (new Tracer<ConnectionsService>(requestContext, TracePoints.Connections.CreateConnectionEnter, TracePoints.Connections.CreateConnectionLeave, nameof (CreateConnectionInternal)))
      {
        ArgumentUtility.CheckForNull<ProjectInfo>(projectInfo, nameof (projectInfo));
        ArgumentUtility.CheckForNull<CreatePipelineConnectionInputs>(inputs, nameof (inputs));
        IPipelineSourceProvider provider = requestContext.GetService<IPipelineSourceProviderService>().GetProvider(inputs.ProviderId);
        CreatePipelineConnectionInputs connectionInputs = inputs;
        TeamProject teamProject = new TeamProject();
        teamProject.Id = projectInfo.Id;
        teamProject.Name = projectInfo.Name;
        connectionInputs.Project = teamProject;
        ConnectionsService.PublishCIData(requestContext, inputs);
        provider.ConnectionCreator.PreCreateConnection(requestContext, provider, inputs);
        string installationId = provider.ConnectionCreator.GetInstallationId(requestContext, (IDictionary<string, string>) inputs.ProviderData);
        string str = (string) null;
        if ((provider.ExternalApp == null ? 0 : (provider.ExternalApp.ManageConnections(requestContext) ? 1 : 0)) == 0 && endpoint == null)
          endpoint = ServiceEndpointHelper.GetProviderEndpointForUser(requestContext, projectInfo.Id, provider, installationId);
        if (endpoint == null)
        {
          endpoint = provider.ConnectionCreator.CreateServiceEndpoint(requestContext, inputs.Project.Id, inputs.Project.Name, (IDictionary<string, string>) inputs.ProviderData);
          if (endpoint != null)
          {
            if ((provider.ExternalApp == null ? 0 : (provider.ExternalApp.RequireValidation(requestContext, installationId) ? 1 : 0)) == 0)
            {
              endpoint = requestContext.GetService<IServiceEndpointService2>().CreateServiceEndpoint(requestContext, projectInfo.Id, endpoint);
            }
            else
            {
              string redirectUrl = provider.ConnectionCreator.GetRedirectUrl(requestContext, inputs, (ServiceEndpoint) null);
              str = provider.ExternalApp.GetValidationUrl(requestContext, projectInfo, endpoint, installationId, redirectUrl);
            }
          }
        }
        if (string.IsNullOrEmpty(str))
          str = provider.ConnectionCreator.GetRedirectUrl(requestContext, inputs, endpoint);
        PipelineConnection connection = new PipelineConnection()
        {
          AccountId = new Guid?(requestContext.ServiceHost.CollectionServiceHost.InstanceId),
          TeamProjectId = new Guid?(projectInfo.Id),
          RedirectUrl = str
        };
        CustomerIntelligenceData ciData = new CustomerIntelligenceData();
        ciData.AddConnectionData(connection, inputs);
        ciData.PublishCI(requestContext, "Connection", inputs.ProviderData[PipelineConstants.ProviderDataVstsUserKey]);
        return connection;
      }
    }

    private static void PublishCIData(
      IVssRequestContext requestContext,
      CreatePipelineConnectionInputs inputs)
    {
      Guid userId = requestContext.GetUserId();
      if (inputs.ProviderData == null)
        inputs.ProviderData = new Dictionary<string, string>();
      inputs.ProviderData[PipelineConstants.ProviderDataVstsUserKey] = userId.ToString();
      inputs.ProviderData[PipelineConstants.ProviderAccountIdKey] = requestContext.ServiceHost.CollectionServiceHost.InstanceId.ToString();
      CustomerIntelligenceData ciData = new CustomerIntelligenceData();
      ciData.AddProjectData(inputs);
      ciData.PublishCI(requestContext, "Project", userId);
    }

    private void ValidateRequestContext(IVssRequestContext requestContext)
    {
      requestContext.CheckHostedDeployment();
      requestContext.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);
    }
  }
}
