// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.WellKnownArtifactInputs
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions
{
  public static class WellKnownArtifactInputs
  {
    public static readonly string VersionsInput = "Versions";
    public static readonly string LatestVersionInput = "LatestVersion";
    public static readonly string ArtifactDetails = nameof (ArtifactDetails);
    public static readonly string Artifacts = nameof (Artifacts);
    public static readonly string ArtifactItems = nameof (ArtifactItems);
    public static readonly string ArtifactSize = nameof (ArtifactSize);
    public static readonly string ArtifactSourceDefinitionUrl = nameof (ArtifactSourceDefinitionUrl);
    public static readonly string ArtifactSourceVersionUrl = nameof (ArtifactSourceVersionUrl);
  }
}
