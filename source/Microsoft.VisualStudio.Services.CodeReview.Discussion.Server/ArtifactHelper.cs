// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.ArtifactHelper
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Discussion.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FFC5EC6C-1B94-4299-8BA9-787264C21330
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.CodeReview.Discussion.Server
{
  internal static class ArtifactHelper
  {
    internal static void ParseVersionUri(
      IVssRequestContext requestContext,
      string versionUri,
      IDiscussionArtifactPlugin pluginExtension,
      out int changesetId,
      out string shelvesetName,
      out string shelvesetOwner)
    {
      requestContext.TraceEnter(600230, TraceArea.Discussion, TraceLayer.Service, nameof (ParseVersionUri));
      changesetId = 0;
      shelvesetName = (string) null;
      shelvesetOwner = (string) null;
      if (!string.IsNullOrWhiteSpace(versionUri) && pluginExtension != null)
      {
        ArtifactId artifact = LinkingUtilities.DecodeUri(versionUri);
        object[] objArray = pluginExtension.DecodeArtifactId(artifact);
        if (VssStringComparer.ArtifactType.Equals(artifact.ArtifactType, "Changeset"))
        {
          changesetId = (int) objArray[0];
          requestContext.Trace(600231, TraceLevel.Info, TraceArea.Discussion, TraceLayer.Service, "Parsed changeset uri to id '{0}'", (object) changesetId);
        }
        else if (VssStringComparer.ArtifactType.Equals(artifact.ArtifactType, "Shelveset"))
        {
          shelvesetName = (string) objArray[0];
          shelvesetOwner = (string) objArray[1];
          requestContext.Trace(600232, TraceLevel.Info, TraceArea.Discussion, TraceLayer.Service, "Parsed shelveset uri to shelveset '{0}\\{1}'", (object) shelvesetName, (object) shelvesetOwner);
        }
      }
      if (changesetId <= 0 && (string.IsNullOrWhiteSpace(shelvesetName) || string.IsNullOrWhiteSpace(shelvesetOwner)))
        requestContext.Trace(600233, TraceLevel.Info, TraceArea.Discussion, TraceLayer.Service, "Using artifactUri as versionUri: {0}", (object) versionUri);
      requestContext.TraceLeave(600239, TraceArea.Discussion, TraceLayer.Service, nameof (ParseVersionUri));
    }

    internal static void ParseVersionId(
      IVssRequestContext requestContext,
      string versionId,
      out int changesetId,
      out string shelvesetName,
      out Guid shelvesetOwner)
    {
      requestContext.TraceEnter(600240, TraceArea.Discussion, TraceLayer.Service, nameof (ParseVersionId));
      changesetId = 0;
      shelvesetName = (string) null;
      shelvesetOwner = Guid.Empty;
      if (!string.IsNullOrWhiteSpace(versionId))
      {
        if (versionId[0] == 'C')
        {
          if (!int.TryParse(versionId.Substring(1), out changesetId))
            requestContext.Trace(600241, TraceLevel.Warning, TraceArea.Discussion, TraceLayer.Service, "Incorrect changeset version id '{0}'", (object) versionId);
        }
        else if (versionId[0] == 'S')
        {
          string[] strArray = versionId.Substring(1).Split(';');
          if (strArray.Length == 2)
          {
            shelvesetName = strArray[0];
            shelvesetOwner = new Guid(strArray[1]);
          }
          else
            requestContext.Trace(600242, TraceLevel.Warning, TraceArea.Discussion, TraceLayer.Service, "Incorrect shelveset version id '{0}'", (object) versionId);
        }
        else
          requestContext.Trace(600243, TraceLevel.Warning, TraceArea.Discussion, TraceLayer.Service, "Incorrect version id '{0}'", (object) versionId);
      }
      requestContext.TraceLeave(600249, TraceArea.Discussion, TraceLayer.Service, nameof (ParseVersionId));
    }

    internal static string CreateVersionId(int changesetId) => 'C'.ToString() + (object) changesetId;

    internal static string CreateVersionId(string shelvesetName, Guid shelvesetOwner) => 'S'.ToString() + shelvesetName + (object) ';' + shelvesetOwner.ToString().ToUpperInvariant();
  }
}
