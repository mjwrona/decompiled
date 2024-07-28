// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Artifact.WebApi.PipelineArtifactStatus
// Assembly: Microsoft.VisualStudio.Services.Artifact.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D39C0B4C-25E7-402A-9BC9-E3DFE7654881
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Artifact.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.Artifact.WebApi
{
  [Serializable]
  public enum PipelineArtifactStatus
  {
    Pending,
    Finalized,
  }
}
