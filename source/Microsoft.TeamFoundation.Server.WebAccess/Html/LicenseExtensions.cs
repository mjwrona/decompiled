// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Html.LicenseExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Html
{
  public static class LicenseExtensions
  {
    private static Guid[] s_featureLicenseIds = new Guid[19]
    {
      LicenseFeatures.AdminId,
      LicenseFeatures.AdvancedBacklogManagementId,
      LicenseFeatures.AdvancedHomePageId,
      LicenseFeatures.AdvancedPortfolioBacklogManagementId,
      LicenseFeatures.AgileBoardsId,
      LicenseFeatures.BacklogManagementId,
      LicenseFeatures.BuildId,
      LicenseFeatures.ChartAuthoringId,
      LicenseFeatures.ChartViewingId,
      LicenseFeatures.CodeId,
      LicenseFeatures.FeedbackId,
      LicenseFeatures.PortfolioBacklogManagementId,
      LicenseFeatures.ReleaseManagementStakeholderId,
      LicenseFeatures.ReleaseManagementId,
      LicenseFeatures.StandardFeaturesId,
      LicenseFeatures.TestManagementForBasicUsersId,
      LicenseFeatures.TestManagementId,
      LicenseFeatures.ViewMyWorkItemsId,
      LicenseFeatures.TestManagementForStakeholderId
    };

    public static Dictionary<string, object> GetUserFeatureLicenses(this TfsWebContext tfsWebContext) => (Dictionary<string, object>) LicenseExtensions.GetFeaturePayload(tfsWebContext);

    public static MvcHtmlString FeatureLicenseInfo(this HtmlHelper htmlHelper) => htmlHelper.JsonIsland((object) LicenseExtensions.GetFeaturePayload(htmlHelper.ViewContext.TfsWebContext()), (object) new
    {
      @class = "feature-licenses"
    });

    public static JsObject GetFeaturePayload(TfsWebContext tfsWebContext)
    {
      FeatureContext featureContext = tfsWebContext.FeatureContext;
      JsObject featurePayload = new JsObject();
      foreach (Guid featureLicenseId in LicenseExtensions.s_featureLicenseIds)
        featurePayload.Add(featureLicenseId.ToString(), (object) (int) featureContext.GetFeatureMode(featureLicenseId));
      return featurePayload;
    }
  }
}
