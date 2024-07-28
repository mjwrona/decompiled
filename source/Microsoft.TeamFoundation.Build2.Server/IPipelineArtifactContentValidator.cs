// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.IPipelineArtifactContentValidator
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using System;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal interface IPipelineArtifactContentValidator
  {
    Task ValidateContentAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId,
      BuildArtifact buildArtifact,
      Manifest manifest,
      Microsoft.VisualStudio.Services.Identity.Identity publisher,
      string publisherIpAddress);
  }
}
