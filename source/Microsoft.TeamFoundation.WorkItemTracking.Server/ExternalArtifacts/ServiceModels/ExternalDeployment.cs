// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.ServiceModels.ExternalDeployment
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.ServiceModels
{
  public class ExternalDeployment
  {
    public Guid ArtifactId { get; set; }

    public ExternalPipeline Pipeline { get; set; }

    public ExternalEnvironment Environment { get; set; }

    public int RunId { get; set; }

    public int SequenceNumber { get; set; }

    public string DisplayName { get; set; }

    public string Description { get; set; }

    public string Status { get; set; }

    public string Group { get; set; }

    public Uri Url { get; set; }

    public DateTime StatusDate { get; set; }

    public Guid CreatedBy { get; set; }
  }
}
