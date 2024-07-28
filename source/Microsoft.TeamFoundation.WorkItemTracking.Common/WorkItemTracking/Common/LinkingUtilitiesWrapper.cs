// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.LinkingUtilitiesWrapper
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common
{
  public class LinkingUtilitiesWrapper
  {
    public virtual bool IsToolTypeWellFormed(string tool) => LinkingUtilities.IsToolTypeWellFormed(tool);

    public virtual bool IsUriWellFormed(string artifactUri) => LinkingUtilities.IsUriWellFormed(artifactUri);

    public virtual bool IsArtifactTypeWellFormed(string artifactType) => LinkingUtilities.IsArtifactTypeWellFormed(artifactType);

    public virtual bool IsArtifactToolSpecificIdWellFormed(string toolSpecificId) => LinkingUtilities.IsArtifactToolSpecificIdWellFormed(toolSpecificId);

    public virtual bool IsArtifactIdWellFormed(ArtifactId artifactId) => LinkingUtilities.IsArtifactIdWellFormed(artifactId);

    public virtual string EncodeUri(ArtifactId artifactId) => LinkingUtilities.EncodeUri(artifactId);

    public virtual ArtifactId DecodeUri(string uri) => LinkingUtilities.DecodeUri(uri);

    public virtual ArrayList RemoveDuplicateArtifacts(ArrayList artifactList) => LinkingUtilities.RemoveDuplicateArtifacts(artifactList);

    public virtual string GetArtifactUri(string artifactUrl) => LinkingUtilities.GetArtifactUri(artifactUrl);

    public virtual string GetArtifactUrl(
      string artifactDisplayUrl,
      ArtifactId artId,
      string serverUrl)
    {
      return LinkingUtilities.GetArtifactUrl(artifactDisplayUrl, artId, serverUrl);
    }

    public virtual string GetServerUrl(Uri serverUri) => LinkingUtilities.GetServerUrl(serverUri);
  }
}
