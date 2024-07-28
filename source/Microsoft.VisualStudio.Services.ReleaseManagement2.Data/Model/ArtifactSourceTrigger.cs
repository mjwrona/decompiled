// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactSourceTrigger
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public class ArtifactSourceTrigger : ReleaseTriggerBase
  {
    public ArtifactSourceTrigger() => this.TriggerType = ReleaseTriggerType.ArtifactSource;

    public int ArtifactSourceId { get; set; }

    public string Alias { get; set; }

    public int TargetEnvironmentId { get; set; }

    public string TargetEnvironmentName { get; set; }

    public IList<ArtifactFilter> TriggerConditions { get; set; }
  }
}
