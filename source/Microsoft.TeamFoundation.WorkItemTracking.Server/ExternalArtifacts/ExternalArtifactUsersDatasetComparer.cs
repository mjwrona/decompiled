// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.ExternalArtifactUsersDatasetComparer
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.DataModels;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts
{
  internal class ExternalArtifactUsersDatasetComparer : 
    IEqualityComparer<ExternalArtifactUserDataset>
  {
    public bool Equals(ExternalArtifactUserDataset a, ExternalArtifactUserDataset b) => a.InternalRepositoryId == b.InternalRepositoryId && a.ArtifactId == b.ArtifactId && a.UserId == b.UserId && a.RelationshipType == b.RelationshipType;

    public int GetHashCode(ExternalArtifactUserDataset dataset)
    {
      int num = dataset.InternalRepositoryId.GetHashCode() ^ dataset.ArtifactId.GetHashCode();
      int? hashCode = dataset.UserId?.GetHashCode();
      return (hashCode.HasValue ? new int?(num ^ hashCode.GetValueOrDefault()) : new int?()) ?? 0 ^ dataset.RelationshipType.GetHashCode();
    }

    public static ExternalArtifactUsersDatasetComparer Instance { get; } = new ExternalArtifactUsersDatasetComparer();
  }
}
