// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinitionArtifactSourceMap
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.FormInput;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public class ReleaseDefinitionArtifactSourceMap
  {
    public int ReleaseDefinitionId { get; set; }

    public Guid ProjectId { get; set; }

    public int ArtifactSourceId { get; set; }

    public string SourceId { get; set; }

    public string Alias { get; set; }

    public string ArtifactTypeId { get; set; }

    public Dictionary<string, InputValue> SourceData { get; private set; }

    public bool IsPrimary { get; set; }

    public ReleaseDefinitionArtifactSourceMap() => this.SourceData = new Dictionary<string, InputValue>();
  }
}
