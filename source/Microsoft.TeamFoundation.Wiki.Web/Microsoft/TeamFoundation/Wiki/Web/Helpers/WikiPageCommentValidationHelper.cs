// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Web.Helpers.WikiPageCommentValidationHelper
// Assembly: Microsoft.TeamFoundation.Wiki.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D56D2437-BFF5-4193-B3F8-AC19F31B2530
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Wiki.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebServer;
using Microsoft.TeamFoundation.Wiki.Server;
using Microsoft.TeamFoundation.Wiki.Server.Providers;
using Microsoft.TeamFoundation.Wiki.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Wiki.Web.Helpers
{
  internal class WikiPageCommentValidationHelper
  {
    internal static WikiV2 ValidateWikiAndPageId(
      IVssRequestContext requestContext,
      Guid projectId,
      string wikiIdentifier,
      int pageId)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(wikiIdentifier, nameof (wikiIdentifier));
      WikiV2 wikiByIdentifier = WikiV2Helper.GetWikiByIdentifier(requestContext, projectId, wikiIdentifier);
      if (wikiByIdentifier == null)
        throw new WikiNotFoundException(Microsoft.TeamFoundation.Wiki.Web.Resources.WikiNotFound);
      if (pageId <= 0)
        throw new InvalidArgumentValueException(nameof (pageId), Microsoft.TeamFoundation.Wiki.Web.Resources.ErrorMessageInvalidPageId);
      new WikiPageIdDetailsProvider().GetPageIdDetails(requestContext, projectId, wikiByIdentifier, pageId);
      return wikiByIdentifier;
    }
  }
}
