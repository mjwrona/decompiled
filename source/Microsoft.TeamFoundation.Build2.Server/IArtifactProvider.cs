// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.IArtifactProvider
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Net.Http;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [InheritedExport]
  public interface IArtifactProvider
  {
    IEnumerable<string> SupportedArtifactTypes { get; }

    IList<BuildDefinitionStep> CleanupArtifact(
      IVssRequestContext requestContext,
      string definitionName,
      BuildData build,
      BuildArtifact artifact);

    void AddArtifactReference(
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId,
      BuildArtifact artifact);

    void UpdateReferences(
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId,
      Microsoft.TeamFoundation.Build.WebApi.BuildArtifact artifact,
      bool expandSignedContent);

    bool TryGetWebUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      string resourceType,
      string resourceData,
      out string url);

    HttpResponseMessage GetDownloadResponse(
      IVssRequestContext requestContext,
      HttpRequestMessage requestMessage,
      Guid projectId,
      BuildArtifact artifact,
      ISecuredObject securedObject);

    HttpResponseMessage GetFileDownloadResponse(
      IVssRequestContext requestContext,
      string fileId,
      string fileName,
      BuildArtifact artifact,
      UrlValidationResult urlValidationResult,
      ISecuredObject securedObject);

    bool ValidateFileIdentifier(
      IVssRequestContext requestContext,
      string fileId,
      BuildArtifact artifact);
  }
}
