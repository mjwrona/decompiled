// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.IReviewContentProvider
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  [InheritedExport]
  public interface IReviewContentProvider
  {
    bool CanResolveArtifactId(ArtifactId artifact);

    Stream GetContentStream(
      IVssRequestContext requestContext,
      ArtifactId artifact,
      ContentInfo contentInfo,
      out CompressionType compressionType);

    Dictionary<string, Stream> GetContentStreams(
      IVssRequestContext requestContext,
      ArtifactId artifact,
      IEnumerable<ContentInfo> contentInfo,
      out CompressionType compressionTypes);

    PushStreamContent GetZipPushStreamContent(
      IVssRequestContext requestContext,
      ArtifactId artifact,
      IEnumerable<ContentInfo> contentInfo);
  }
}
