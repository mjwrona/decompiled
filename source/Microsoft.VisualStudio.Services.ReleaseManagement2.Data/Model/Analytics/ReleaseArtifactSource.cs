// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Analytics.ReleaseArtifactSource
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Analytics
{
  public class ReleaseArtifactSource
  {
    public Guid ProjectGuid { get; set; }

    public int ReleaseId { get; set; }

    public int ReleaseArtifactSourceId { get; set; }

    public int ReleaseDefinitionId { get; set; }

    public string SourceId { get; set; }

    public string Alias { get; set; }

    public string ArtifactType { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ArtifactVersionId { get; set; }

    public string ArtifactVersionName { get; set; }

    public string SourceBranch { get; set; }

    public bool IsPrimary { get; set; }

    public DateTime CreatedOn { get; set; }
  }
}
