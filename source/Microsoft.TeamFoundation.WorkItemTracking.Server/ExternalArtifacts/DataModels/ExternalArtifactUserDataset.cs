// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.DataModels.ExternalArtifactUserDataset
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.DataModels
{
  internal class ExternalArtifactUserDataset
  {
    public ExternalArtifactUserDataset(
      Guid repositoryId,
      string artifactId,
      string userId,
      ExternalArtifactRelationshipType relationshipType,
      bool updateOnly)
    {
      this.InternalRepositoryId = repositoryId;
      this.ArtifactId = artifactId;
      this.UserId = userId;
      this.RelationshipType = relationshipType;
      this.UpdateOnly = updateOnly;
    }

    public Guid InternalRepositoryId { get; set; }

    public string ArtifactId { get; set; }

    public string UserId { get; set; }

    public ExternalArtifactRelationshipType RelationshipType { get; set; }

    public bool UpdateOnly { get; set; }
  }
}
