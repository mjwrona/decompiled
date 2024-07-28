// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Html.AccountAadInformationExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Aad;
using System;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Html
{
  public static class AccountAadInformationExtensions
  {
    private const int WebAccessExceptionEaten = 599999;
    private static readonly string s_accountAadInformationData = "AccountAadInformationData";

    public static MvcHtmlString AccountAadInformation(this HtmlHelper htmlHelper)
    {
      htmlHelper.TraceEnter(0, "WebAccess", TfsTraceLayers.Content, nameof (AccountAadInformation));
      try
      {
        TfsWebContext webContext = htmlHelper.ViewContext.TfsWebContext();
        return htmlHelper.JsonIsland((object) AccountAadInformationExtensions.GetAccountAadInformation(webContext), (object) new
        {
          @class = AccountAadInformationExtensions.s_accountAadInformationData
        });
      }
      finally
      {
        htmlHelper.TraceLeave(0, "WebAccess", TfsTraceLayers.Content, nameof (AccountAadInformation));
      }
    }

    public static JsObject GetAccountAadInformation(TfsWebContext webContext)
    {
      JsObject accountAadInformation = new JsObject();
      if (webContext.IsFeatureAvailable("WebAccess.AccountAadInformationNotification") && webContext.IsHosted)
      {
        IVssRequestContext vssRequestContext = webContext.TfsRequestContext.To(TeamFoundationHostType.Application);
        AccountAadInformationModel model = new AccountAadInformationModel();
        if (AccountAadInformationExtensions.IsAADAccount(vssRequestContext, webContext.CurrentUserIdentity))
        {
          model.IsAadAccount = true;
          AadTenant tenant = AccountAadInformationExtensions.GetTenant(vssRequestContext, webContext.CurrentUserIdentity);
          if (tenant != null)
            model.AccountAadTenantName = tenant.DisplayName;
        }
        accountAadInformation = AccountAadInformationExtensions.AccountAadInformationModelToJson(model);
      }
      return accountAadInformation;
    }

    private static JsObject AccountAadInformationModelToJson(AccountAadInformationModel model)
    {
      JsObject json = new JsObject();
      json["IsAadAccount"] = (object) model.IsAadAccount;
      json["AccountAadTenantName"] = (object) model.AccountAadTenantName;
      return json;
    }

    private static AadTenant GetTenant(
      IVssRequestContext requestContext,
      TeamFoundationIdentity identity)
    {
      Guid empty = Guid.Empty;
      AadTenant tenant = (AadTenant) null;
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        if (identity.IsExternalUser)
        {
          try
          {
            tenant = requestContext.GetService<AadService>().GetTenant(requestContext, new GetTenantRequest()).Tenant;
          }
          catch (Exception ex)
          {
            requestContext.TraceException(599999, "WebAccess", TfsTraceLayers.Content, ex);
          }
        }
      }
      return tenant;
    }

    private static bool IsAADAccount(
      IVssRequestContext organizationRequestContext,
      TeamFoundationIdentity identity)
    {
      return identity.IsExternalUser;
    }
  }
}
