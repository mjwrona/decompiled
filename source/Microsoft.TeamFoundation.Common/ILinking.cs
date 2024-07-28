// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ILinking
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation
{
  public interface ILinking : ILinkingProvider, ILinkingConsumer
  {
    Artifact[] GetReferencingArtifacts(string[] uriList, LinkFilter[] filters);

    string GetArtifactUrl(string uri);

    string GetArtifactUrl(ArtifactId artId);

    string GetArtifactUrlExternal(string uri);

    string GetArtifactUrlExternal(ArtifactId artId);
  }
}
