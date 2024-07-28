// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask.ArtifactUriCreator
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask
{
  public class ArtifactUriCreator
  {
    public static readonly string ReleaseArtifactTypeName = "Release";
    public static readonly string ReleaseDefinitionTypeName = "Definition";
    public static readonly string EnvironmentArtifactTypeName = "Environment";
    private readonly Func<ArtifactId, string> artifactUriEncoder;

    public ArtifactUriCreator()
      : this(ArtifactUriCreator.\u003C\u003EO.\u003C0\u003E__EncodeUri ?? (ArtifactUriCreator.\u003C\u003EO.\u003C0\u003E__EncodeUri = new Func<ArtifactId, string>(LinkingUtilities.EncodeUri)))
    {
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
    }

    protected ArtifactUriCreator(Func<ArtifactId, string> artifactUriEncoder) => this.artifactUriEncoder = artifactUriEncoder;

    public Uri CreateReleaseUri(int releaseId) => this.CreateArtifactUri(ArtifactUriCreator.ReleaseArtifactTypeName, releaseId.ToString((IFormatProvider) CultureInfo.InvariantCulture));

    public Uri CreateReleaseEnvironmentUri(int environmentId) => this.CreateArtifactUri(ArtifactUriCreator.EnvironmentArtifactTypeName, environmentId.ToString((IFormatProvider) CultureInfo.InvariantCulture));

    public Uri CreateReleaseDefinitionUri(int definitionId) => this.CreateArtifactUri(ArtifactUriCreator.ReleaseDefinitionTypeName, definitionId.ToString((IFormatProvider) CultureInfo.InvariantCulture));

    public Uri CreateArtifactUri(string artifactTypeName, string toolSpecificId) => new Uri(this.artifactUriEncoder(new ArtifactId()
    {
      ToolSpecificId = toolSpecificId,
      ArtifactType = artifactTypeName,
      Tool = "ReleaseManagement"
    }), UriKind.Absolute);
  }
}
