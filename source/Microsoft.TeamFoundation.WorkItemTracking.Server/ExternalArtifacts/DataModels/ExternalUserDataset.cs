// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.DataModels.ExternalUserDataset
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.DataModels
{
  internal class ExternalUserDataset
  {
    public string ProviderKey { get; set; }

    public string UserId { get; set; }

    public string UserName { get; set; }

    public string UserLogin { get; set; }

    public string UserAvatarUrl { get; set; }

    public bool UpdateOnly { get; set; }

    public string RelatedArtifactId { get; set; }

    public Guid RelatedInternalRepositoryId { get; set; }

    public ArtifactType RelatedArtifactType { get; set; }
  }
}
