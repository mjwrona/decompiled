// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.ICodeReviewSecurityExtension
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  [InheritedExport]
  public interface ICodeReviewSecurityExtension
  {
    bool CanResolve(string artifactUri);

    bool HasReadReviewAccess(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      string artifactUri);

    void CheckReadPermissionBatch(
      IVssRequestContext requestContext,
      List<Review> reviewsBySourceArtifactId,
      Dictionary<int, bool> reviewsChecked);

    void CheckWriteReviewAccess(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      string artifactUri);
  }
}
