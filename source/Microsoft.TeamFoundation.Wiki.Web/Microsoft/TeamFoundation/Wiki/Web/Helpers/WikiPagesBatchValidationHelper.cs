// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Web.Helpers.WikiPagesBatchValidationHelper
// Assembly: Microsoft.TeamFoundation.Wiki.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D56D2437-BFF5-4193-B3F8-AC19F31B2530
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Wiki.Web.dll

using Microsoft.TeamFoundation.Wiki.Server;
using Microsoft.TeamFoundation.Wiki.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Wiki.Web.Helpers
{
  public class WikiPagesBatchValidationHelper
  {
    public static void Validate(WikiPagesBatchRequest pagesBatchRequest, out int? afterPageId)
    {
      if (pagesBatchRequest == null)
        throw new ArgumentNullException(nameof (pagesBatchRequest), Microsoft.TeamFoundation.Wiki.Web.Resources.InvalidParametersOrNull);
      if (pagesBatchRequest.Top.HasValue)
        ArgumentUtility.CheckBoundsInclusive(pagesBatchRequest.Top.Value, WikiPagesBatchConstants.MinTopCount, WikiPagesBatchConstants.MaxTopCount, "Top");
      afterPageId = new int?();
      string continuationToken = pagesBatchRequest.ContinuationToken;
      if (continuationToken != null)
      {
        int result;
        if (!int.TryParse(pagesBatchRequest.ContinuationToken, out result) || result <= 0)
          throw new ArgumentException(string.Format(Microsoft.TeamFoundation.Wiki.Web.Resources.WikiPagesBatchInvalidContinuationToken, (object) continuationToken));
        afterPageId = new int?(result);
      }
      if (!pagesBatchRequest.PageViewsForDays.HasValue || pagesBatchRequest.PageViewsForDays.Value == 0)
        return;
      ArgumentUtility.CheckBoundsInclusive(pagesBatchRequest.PageViewsForDays.Value, WikiPageViewsConstants.MinPageViewsDays, WikiPageViewsConstants.MaxPageViewsDays, "PageViewsForDays");
    }
  }
}
