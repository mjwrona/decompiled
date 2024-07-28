// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IContentValidationServiceExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.Threading;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class IContentValidationServiceExtensions
  {
    public static Task FireAndForgetSubmitEmbeddedContentAsync(
      this IContentValidationService cvs,
      IVssRequestContext rc,
      Guid projectId,
      Microsoft.VisualStudio.Services.Identity.Identity creatorIdentity,
      string creatorIpAddress,
      string sourceFileName,
      IEnumerable<DataUriEmbeddedContent> embeddedContents)
    {
      if (!rc.IsSystemContext)
        throw new ArgumentException("rc must be a system context.");
      return rc.Fork((Func<IVssRequestContext, Task>) (async forkedRc =>
      {
        try
        {
          foreach (DataUriEmbeddedContent embeddedContent in embeddedContents)
            await cvs.SubmitAsync(forkedRc, projectId, embeddedContent.ParsedBase64Source, ContentValidationUtil.GetScanTypeFromContentType(embeddedContent.MimeType), sourceFileName, creatorIdentity, creatorIpAddress);
        }
        catch (Exception ex)
        {
          forkedRc.TraceException(21701002, "IContentValidationService", nameof (FireAndForgetSubmitEmbeddedContentAsync), ex);
        }
      }), nameof (FireAndForgetSubmitEmbeddedContentAsync));
    }
  }
}
