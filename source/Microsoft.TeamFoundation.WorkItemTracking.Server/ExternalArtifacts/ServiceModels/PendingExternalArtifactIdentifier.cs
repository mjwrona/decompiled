// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.ServiceModels.PendingExternalArtifactIdentifier
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.ServiceModels
{
  public class PendingExternalArtifactIdentifier
  {
    public string ProviderKey { get; set; }

    public Guid InternalRepositoryId { get; set; }

    public string ExternalRepositoryId { get; set; }

    public string ArtifactId { get; set; }

    public GitHubLinkItemType ArtifactType { get; set; }

    public ExternalArtifactHydrationStatus HydrationStatus { get; set; }

    public ExternalArtifactHydrationStatusDetails HydrationStatusDetails { get; set; }
  }
}
