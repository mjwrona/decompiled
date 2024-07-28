// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.Commands.WhereCommand
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.ExternalIntegration.HostIdMapping;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Pipelines.Server.Providers;
using Microsoft.VisualStudio.Services.ExternalEvent;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Pipelines.Server.Commands
{
  internal class WhereCommand : ICommentCommand
  {
    public string CommandKeyword => CommandNames.Where;

    public string ShortDescription => "Report back the Azure DevOps orgs that are related to this repository and org";

    public string ExampleUsage => "\"where\"";

    public bool IsValid(
      IVssRequestContext requestContext,
      string providerId,
      JObject authentication,
      ExternalPullRequestCommentEvent commentEvent,
      MergeJobStatus mergeJobStatus,
      out string responseMessage)
    {
      if (!requestContext.GetService<IPipelineSourceProviderService>().GetProvider(providerId).PullRequestProvider.DoesUserHaveWritePermissions(requestContext, authentication, commentEvent.AuthorAssociation, commentEvent.Repo.Id, commentEvent.CommentedBy.Name))
      {
        responseMessage = CommentResponseBuilder.Build("Commenter does not have sufficient privileges for PR " + commentEvent.PullRequest.Number + " in repo " + commentEvent.PullRequest.Repo.Id);
        return false;
      }
      responseMessage = string.Empty;
      return true;
    }

    public bool TryExecute(
      IVssRequestContext requestContext,
      string providerId,
      JObject authentication,
      ExternalPullRequestCommentEvent commentEvent,
      MergeJobStatus mergeJobStatus,
      out string responseMessage,
      out List<Exception> exceptions)
    {
      exceptions = new List<Exception>();
      IPipelineSourceProvider provider = requestContext.GetService<IPipelineSourceProviderService>().GetProvider(providerId);
      string id = commentEvent.Repo.Id;
      string installationIdForRepository = provider.ExternalApp?.GetInstallationIdForRepository(requestContext, id);
      IHostIdMappingService service = requestContext.GetService<IHostIdMappingService>();
      HostIdMappingData mappingData1 = provider.Routers.First<IHostIdMappingRouter>().GetMappingData(requestContext, installationIdForRepository, id);
      IVssRequestContext deploymentRequestContext = requestContext;
      string providerId1 = providerId;
      HostIdMappingData mappingData2 = mappingData1;
      Guid? hostId = service.GetHostId(deploymentRequestContext, providerId1, mappingData2, true);
      responseMessage = "where indeed!";
      if (hostId.HasValue)
        responseMessage = this.GetResponse(requestContext, (IEnumerable<Guid>) new Guid[1]
        {
          hostId.Value
        });
      else
        responseMessage = CommentResponseBuilder.Build("No Azure DevOps organizations are setup to build repository " + id + " using the Azure Pipelines App.");
      return true;
    }

    private string GetResponse(IVssRequestContext requestContext, IEnumerable<Guid> hostIds)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      TeamFoundationHostManagementService service = vssRequestContext.GetService<TeamFoundationHostManagementService>();
      CommentResponseBuilder commentResponseBuilder = new CommentResponseBuilder();
      commentResponseBuilder.AppendLine("Azure DevOps orgs getting events for this repository:").StartList();
      foreach (Guid hostId in hostIds)
      {
        HostProperties hostProperties = (HostProperties) service.QueryServiceHostProperties(vssRequestContext, hostId);
        commentResponseBuilder.StartListItem().AppendLink("https://codedev.ms/" + hostProperties.Name, hostProperties.Name).EndListItem();
      }
      return commentResponseBuilder.EndList().ToString();
    }
  }
}
