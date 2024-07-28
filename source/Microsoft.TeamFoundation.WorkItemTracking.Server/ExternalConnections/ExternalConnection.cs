// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections.ExternalConnection
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.VisualStudio.Services.ExternalEvent;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections
{
  public class ExternalConnection
  {
    public ExternalConnection() => this.ConnectionMetadata = new ExternalConnectionMetadata();

    public Guid Id { get; set; }

    public string Name { get; set; }

    public Guid ProjectId { get; set; }

    public string ProviderKey { get; set; }

    public ServiceEndpointViewModel ServiceEndpoint { get; set; }

    public IEnumerable<ExternalGitRepo> ExternalGitRepos { get; set; }

    public string StatusMessage { get; set; }

    public Guid CreatedBy { get; set; }

    public ExternalConnectionMetadata ConnectionMetadata { get; set; }

    public bool IsConnectionMetadataValid
    {
      get
      {
        if (this.ConnectionMetadata.ConnectionErrorType.HasValue)
        {
          ConnectionErrorType? connectionErrorType1 = this.ConnectionMetadata.ConnectionErrorType;
          ConnectionErrorType connectionErrorType2 = ConnectionErrorType.NoRepositories;
          if (!(connectionErrorType1.GetValueOrDefault() == connectionErrorType2 & connectionErrorType1.HasValue))
          {
            connectionErrorType1 = this.ConnectionMetadata.ConnectionErrorType;
            ConnectionErrorType connectionErrorType3 = ConnectionErrorType.RepositoriesMappedToDifferentOrganization;
            return connectionErrorType1.GetValueOrDefault() == connectionErrorType3 & connectionErrorType1.HasValue;
          }
        }
        return true;
      }
    }

    public bool IsConnectionValid => this.IsConnectionMetadataValid && this.ServiceEndpoint.IsEndpointValid;

    public string FriendlyStatusMessage
    {
      get
      {
        ConnectionErrorType? connectionErrorType = this.ConnectionMetadata.ConnectionErrorType;
        if (connectionErrorType.HasValue)
        {
          switch (connectionErrorType.GetValueOrDefault())
          {
            case ConnectionErrorType.BadCredentials:
              return ServerResources.ExternalConnectionInvalidCredentials();
            case ConnectionErrorType.AppUninstalled:
              return ServerResources.ExternalConnectionGitHubAppUninstalled();
            case ConnectionErrorType.OAuthConfigurationDeleted:
              return ServerResources.ExternalConnectionOAuthConfigurationDeleted();
            case ConnectionErrorType.UnreachableRepositories:
              return ServerResources.ExternalConnectionUnreachableRepositories((object) string.Join(",", this.ConnectionMetadata.UnreachableRepositories));
            case ConnectionErrorType.NoRepositories:
              return ServerResources.ExternalConnectionNoRepositories();
            case ConnectionErrorType.RepositoriesMappedToDifferentOrganization:
              return ServerResources.RepositoriesWithMappingToDifferentOrganization((object) string.Join(",", this.ExternalGitRepos.Where<ExternalGitRepo>((Func<ExternalGitRepo, bool>) (r => this.ConnectionMetadata.RepositoriesWithMappingToDifferentOrganization.Contains(r.NodeId()))).Select<ExternalGitRepo, string>((Func<ExternalGitRepo, string>) (r => r.RepoNameWithOwner()))));
          }
        }
        return (string) null;
      }
    }
  }
}
