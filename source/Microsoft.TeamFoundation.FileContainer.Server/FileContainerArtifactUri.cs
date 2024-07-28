// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileContainerArtifactUri
// Assembly: Microsoft.TeamFoundation.FileContainer.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D69E508F-D51F-4EF2-B979-7E96E61DA19E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.FileContainer.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class FileContainerArtifactUri
  {
    public FileContainerArtifactUri(string artifactUri) => this.ArtifactUri = artifactUri;

    public string ArtifactUri { get; set; }
  }
}
