// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Html.StakeholderExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.WebAccess.Models;
using Microsoft.VisualStudio.Services.Account;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Licensing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Html
{
  public static class StakeholderExtensions
  {
    private const string s_area = "StakeholderExtensions";
    private const string s_layer = "StakeholderExtensions";

    public static void StakeholderMessage(this HtmlHelper htmlHelper)
    {
      htmlHelper.TraceEnter(0, "WebAccess", TfsTraceLayers.Content, nameof (StakeholderMessage));
      try
      {
        IVssRequestContext tfsRequestContext = htmlHelper.ViewContext.TfsWebContext().TfsRequestContext;
        if (tfsRequestContext.IntendedHostTypeIs(TeamFoundationHostType.Deployment) || !tfsRequestContext.IsStakeholder())
          return;
        if (tfsRequestContext.IsHosted())
          StakeholderExtensions.DisplayHostedStakeholderErrorMessage(tfsRequestContext, htmlHelper);
        else
          StakeholderExtensions.DisplayStakeholderNotification(htmlHelper);
      }
      finally
      {
        htmlHelper.TraceLeave(0, "WebAccess", TfsTraceLayers.Content, nameof (StakeholderMessage));
      }
    }

    public static bool IsStakeholderMsdnEligible(this HtmlHelper htmlHelper)
    {
      htmlHelper.TraceEnter(0, "WebAccess", TfsTraceLayers.Content, nameof (IsStakeholderMsdnEligible));
      try
      {
        IVssRequestContext tfsRequestContext = htmlHelper.ViewContext.TfsWebContext().TfsRequestContext;
        if (tfsRequestContext.IntendedHostTypeIs(TeamFoundationHostType.Deployment) || !tfsRequestContext.IsStakeholder() || !tfsRequestContext.IsHosted())
          return false;
        AccountEntitlement accountEntitlement = tfsRequestContext.GetAccountEntitlement();
        return accountEntitlement != (AccountEntitlement) null && accountEntitlement.License.Source == LicensingSource.Msdn && accountEntitlement.License == (License) MsdnLicense.Eligible || accountEntitlement?.Rights?.Level.GetValueOrDefault() == VisualStudioOnlineServiceLevel.None;
      }
      finally
      {
        htmlHelper.TraceLeave(0, "WebAccess", TfsTraceLayers.Content, nameof (IsStakeholderMsdnEligible));
      }
    }

    private static void DisplayHostedStakeholderErrorMessage(
      IVssRequestContext requestContext,
      HtmlHelper htmlHelper)
    {
      string headerMessage = string.Empty;
      MessageAreaType messageType = MessageAreaType.Info;
      string str = StakeholderExtensions.BuildLink(WACommonResources.StakeholderExpiredLicenseReminderLinkText, WACommonResources.StakeholderExpiredLicenseReminderLink);
      AccountEntitlement accountEntitlement = requestContext.GetAccountEntitlement();
      if (accountEntitlement != (AccountEntitlement) null)
      {
        if (accountEntitlement.License.Source == LicensingSource.Msdn)
        {
          if (accountEntitlement.License == (License) MsdnLicense.Eligible)
          {
            if (requestContext.IsTracing(9058100, TraceLevel.Verbose, nameof (StakeholderExtensions), nameof (StakeholderExtensions)))
              requestContext.Trace(9058100, TraceLevel.Verbose, nameof (StakeholderExtensions), nameof (StakeholderExtensions), "User = {0}, License = {1}, Status = {2}, StackTrace = {3}", (object) requestContext.UserContext, (object) accountEntitlement.License, (object) accountEntitlement.UserStatus, (object) Environment.StackTrace);
            headerMessage = string.Format(WACommonResources.StakeholderMsdnEligibleReminderMessage, (object) WACommonResources.HtmlContentMsdnEligible);
            messageType = MessageAreaType.Error;
          }
          else
          {
            if (requestContext.IsTracing(9058101, TraceLevel.Verbose, nameof (StakeholderExtensions), nameof (StakeholderExtensions)))
              requestContext.Trace(9058101, TraceLevel.Verbose, nameof (StakeholderExtensions), nameof (StakeholderExtensions), "User = {0}, License = {1}, Status = {2}, StackTrace = {3}", (object) requestContext.UserContext, (object) accountEntitlement.License, (object) accountEntitlement.UserStatus, (object) Environment.StackTrace);
            headerMessage = string.Format(WACommonResources.StakeholderMsdnExpiredReminderMessage, (object) str);
          }
        }
        else if (accountEntitlement.License.Source == LicensingSource.Account && accountEntitlement.UserStatus == AccountUserStatus.Disabled)
        {
          if (requestContext.IsTracing(9058102, TraceLevel.Verbose, nameof (StakeholderExtensions), nameof (StakeholderExtensions)))
            requestContext.Trace(9058102, TraceLevel.Verbose, nameof (StakeholderExtensions), nameof (StakeholderExtensions), "User = {0}, License = {1}, Status = {2}, StackTrace = {3}", (object) requestContext.UserContext, (object) accountEntitlement.License, (object) accountEntitlement.UserStatus, (object) Environment.StackTrace);
          AccountLicense license = (AccountLicense) accountEntitlement.License;
          headerMessage = string.Format(WACommonResources.StakeholderAccountExpiredReminderMessage, license.License == AccountLicenseType.Express ? (object) "Basic" : (object) license.License.ToString(), (object) str);
        }
      }
      if (requestContext.GetAccountEntitlement(requestContext.GetUserId())?.Rights?.Level.GetValueOrDefault() == VisualStudioOnlineServiceLevel.None)
      {
        if (requestContext.IsTracing(9058103, TraceLevel.Verbose, nameof (StakeholderExtensions), nameof (StakeholderExtensions)))
          requestContext.Trace(9058103, TraceLevel.Verbose, nameof (StakeholderExtensions), nameof (StakeholderExtensions), "User = {0}, License = None, StackTrace = {1}.", (object) requestContext.UserContext, (object) Environment.StackTrace);
        headerMessage = string.Format(WACommonResources.StakeholderMsdnEligibleReminderMessage, (object) WACommonResources.HtmlContentMsdnEligible);
        messageType = MessageAreaType.Error;
      }
      if (!string.IsNullOrEmpty(headerMessage))
        StakeholderExtensions.DisplayStakeholderExpiredLicenseNotification(headerMessage, htmlHelper, messageType);
      else
        StakeholderExtensions.DisplayStakeholderNotification(htmlHelper);
    }

    private static void DisplayStakeholderExpiredLicenseNotification(
      string headerMessage,
      HtmlHelper htmlHelper,
      MessageAreaType messageType)
    {
      htmlHelper.DismissableNotifications((IEnumerable<NotificationMessageModel>) new NotificationMessageModel[1]
      {
        new NotificationMessageModel("83E6FCAF-9B4A-414D-BF61-AEE13969D30B", messageType, headerMessage)
      }, "tfs-stakeholder-expired-notification", showIcon: true, clientDismissable: true);
    }

    private static void DisplayStakeholderNotification(HtmlHelper htmlHelper)
    {
      htmlHelper.ViewContext.TfsWebContext();
      string stakeholderReminderMessage = WACommonResources.StakeholderReminderMessage;
      string str = StakeholderExtensions.BuildLink(WACommonResources.StakeholderReminderLinkText, WACommonResources.StakeholderReminderLink);
      htmlHelper.DismissableNotifications((IEnumerable<NotificationMessageModel>) new NotificationMessageModel[1]
      {
        new NotificationMessageModel("181826E7-3E46-41E1-96BC-04F992BD33A8", MessageAreaType.Info, string.Format(stakeholderReminderMessage, (object) str))
      }, "tfs-stakeholder-notification", showIcon: true, clientDismissable: true);
    }

    private static string BuildLink(string text, string uri)
    {
      TagBuilder tagBuilder = new TagBuilder("a");
      tagBuilder.MergeAttribute("href", uri);
      tagBuilder.MergeAttribute("target", "_blank");
      tagBuilder.Text(text);
      return tagBuilder.ToString();
    }

    private static AccountEntitlement GetAccountEntitlement(this IVssRequestContext requestContext)
    {
      string key = "$vss:AccountEntitlement/" + requestContext.GetUserId().ToString();
      if (IdentityHelper.IsServiceIdentity(requestContext, (IReadOnlyVssIdentity) requestContext.GetUserIdentity()))
        return (AccountEntitlement) null;
      AccountEntitlement accountEntitlement;
      if (!requestContext.RootContext.TryGetItem<AccountEntitlement>(key, out accountEntitlement))
      {
        accountEntitlement = requestContext.GetService<ILicensingEntitlementService>().GetAccountEntitlement(requestContext, requestContext.GetUserId());
        requestContext.RootContext.Items.Add(key, (object) accountEntitlement);
      }
      return accountEntitlement;
    }
  }
}
