// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Html.AccountTrialExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Models;
using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Html
{
  public static class AccountTrialExtensions
  {
    private const int WebAccessExceptionEaten = 599999;
    private static readonly string s_accountTrialInformationData = "accountTrialInformationData";
    private static readonly int s_accountTrialDuration = 91;

    public static MvcHtmlString AccountTrialInformation(this HtmlHelper htmlHelper)
    {
      htmlHelper.TraceEnter(0, "WebAccess", TfsTraceLayers.Content, nameof (AccountTrialInformation));
      try
      {
        TfsWebContext webContext = htmlHelper.ViewContext.TfsWebContext();
        return htmlHelper.JsonIsland((object) AccountTrialExtensions.GetAccountTrialInformation(webContext), (object) new
        {
          @class = AccountTrialExtensions.s_accountTrialInformationData
        });
      }
      finally
      {
        htmlHelper.TraceLeave(0, "WebAccess", TfsTraceLayers.Content, nameof (AccountTrialInformation));
      }
    }

    public static JsObject GetAccountTrialInformation(TfsWebContext webContext)
    {
      JsObject trialInformation = new JsObject();
      if (webContext.IsFeatureAvailable("WebAccess.TrialModeNotification") && webContext.IsHosted)
      {
        AccountTrialModeModel trialModeModel;
        AccountTrialExtensions.GetAccountTrialDates(webContext, out DateTime _, out trialModeModel);
        IVssRequestContext deploymentRequestContext = webContext.TfsRequestContext.To(TeamFoundationHostType.Deployment);
        trialInformation = AccountTrialExtensions.AccountTrialModeModelToJson(trialModeModel, deploymentRequestContext);
        trialInformation["TrialFeatureUrl"] = trialModeModel.IsAccountTrialModeExpired ? (object) WACommonResources.TrialFeatureExpiredUrl : (object) WACommonResources.TrialFeatureUrl;
        JsObject jsObject = trialInformation;
        DateTime dateTime = DateTime.UtcNow;
        dateTime = dateTime.AddDays((double) AccountTrialExtensions.s_accountTrialDuration);
        string str = dateTime.ToString("D");
        jsObject["EndDateIfTrialStarted"] = (object) str;
      }
      return trialInformation;
    }

    private static JsObject AccountTrialModeModelToJson(
      AccountTrialModeModel trialModel,
      IVssRequestContext deploymentRequestContext)
    {
      JsObject json = new JsObject();
      try
      {
        json["StartDate"] = (object) trialModel.StartDate.ToString("D");
        json["EndDate"] = (object) trialModel.EndDate.ToString("D");
        json["IsAccountInTrialMode"] = (object) trialModel.IsAccountInTrialMode;
        json["IsAccountEligibleForTrialMode"] = (object) trialModel.IsAccountEligibleForTrialMode;
        json["IsAccountTrialModeExpired"] = (object) trialModel.IsAccountTrialModeExpired;
        json["DaysLeftOnTrialMode"] = (object) trialModel.DaysLeftOnTrialMode;
      }
      catch (Exception ex)
      {
        deploymentRequestContext.TraceException(599999, "WebAccess", TfsTraceLayers.Content, ex);
      }
      return json;
    }

    internal static void GetAccountTrialDates(
      TfsWebContext webContext,
      out DateTime accountCreationDate,
      out AccountTrialModeModel trialModeModel)
    {
      webContext.TfsRequestContext.CheckProjectCollectionRequestContext();
      Collection collection = webContext.TfsRequestContext.GetService<ICollectionService>().GetCollection(webContext.TfsRequestContext.Elevate(), (IEnumerable<string>) new string[2]
      {
        "SystemProperty.TrialStartDate",
        "SystemProperty.TrialEndDate"
      });
      accountCreationDate = collection.DateCreated;
      object startDate;
      collection.Properties.TryGetValue("SystemProperty.TrialStartDate", out startDate);
      object endDate;
      collection.Properties.TryGetValue("SystemProperty.TrialEndDate", out endDate);
      trialModeModel = new AccountTrialModeModel(startDate as DateTime?, endDate as DateTime?);
    }
  }
}
