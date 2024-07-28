// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.DataModels.ExternalArtifactDataSet
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.DataModels
{
  internal class ExternalArtifactDataSet
  {
    public Guid InternalRepositoryId { get; set; }

    public string ExternalRepositoryId { get; set; }

    public string RepositoryNameWithOwner { get; set; }

    public string RepositoryUrl { get; set; }

    public string ArtifactId { get; set; }

    public string SecondaryId { get; set; }

    public string Url { get; set; }

    public byte HydrationStatus { get; set; }

    public string HydrationStatusDetails { get; set; }

    public bool UpdateOnly { get; set; }
  }
}
