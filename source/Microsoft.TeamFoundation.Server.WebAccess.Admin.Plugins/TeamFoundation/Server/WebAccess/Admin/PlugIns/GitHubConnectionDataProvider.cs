// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.GitHubConnectionDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.GitHubConnector;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  public class GitHubConnectionDataProvider : IExtensionDataProvider
  {
    private const string s_area = "OrgSettings-GitHubConnection";
    private const string s_layer = "GitHubConnectionDataProvider";

    public string Name => "Admin.GitHubConnection";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      requestContext.GetService<IClientLocationProviderService>().AddLocation(requestContext, providerContext.SharedData, ServiceInstanceTypes.SPS, new TeamFoundationHostType?(TeamFoundationHostType.ProjectCollection));
      GitHubConnectionHelper.SetUserContinuationToken(requestContext);
      Microsoft.VisualStudio.Services.Organization.Organization organization = GitHubConnectionHelper.GetOrganization(requestContext);
      bool canLink = GitHubConnectionHelper.DetermineCanLink(requestContext, organization);
      Guid instanceId = requestContext.ServiceHost.InstanceId;
      Collection collection = GitHubConnectionHelper.GetCollection(requestContext);
      bool inviteSet = GitHubConnectionHelper.DetermineInviteSet(collection);
      IGitHubConnectorService service = requestContext.GetService<IGitHubConnectorService>();
      Guid defaultConnectionId;
      bool flag = service.IsConnected(requestContext, out defaultConnectionId);
      if (flag)
        service.GetConnectionInfo(requestContext, defaultConnectionId);
      string gitHubConnectUrl = GitHubConnectionHelper.GetGitHubConnectUrl(requestContext);
      return (object) new GitHubConnectionDataProvider.GitHubConnectionData()
      {
        CanLink = canLink,
        InviteSet = inviteSet,
        CollectionId = instanceId,
        OrganizationName = collection.Name,
        IsAadConnected = !(organization.TenantId == Guid.Empty),
        GitHubConnectUrl = gitHubConnectUrl,
        IsGitHubConnected = flag
      };
    }

    [DataContract]
    public class GitHubConnectionData
    {
      [DataMember(EmitDefaultValue = false)]
      public bool CanLink { get; set; }

      [DataMember(EmitDefaultValue = false)]
      public bool InviteSet { get; set; }

      [DataMember(EmitDefaultValue = false)]
      public Guid CollectionId { get; set; }

      [DataMember(EmitDefaultValue = false)]
      public string OrganizationName { get; set; }

      [DataMember(EmitDefaultValue = false)]
      public bool IsAadConnected { get; set; }

      [DataMember(EmitDefaultValue = false)]
      public string GitHubConnectUrl { get; set; }

      [DataMember(EmitDefaultValue = false)]
      public bool IsGitHubConnected { get; set; }
    }
  }
}
