// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage`1
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.WebAccess.Platform;
using System.Web.UI;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class TfsViewPage<TModel> : PlatformViewPage<TModel>
  {
    public TfsWebContext TfsWebContext => this.ViewContext.TfsWebContext();

    public override void InitHelpers()
    {
      this.TfsWebContext.TfsRequestContext.TraceEnter(0, "WebAccess", TfsTraceLayers.Framework, "TfsViewPage.InitHelpers");
      try
      {
        base.InitHelpers();
        if (this is IChangesMRUNavigationContext)
          MRUNavigationContextEntryManager.UpdateMRUNavigationContextAsync(this.TfsWebContext);
        this.InitTrialMessage();
      }
      finally
      {
        this.TfsWebContext.TfsRequestContext.TraceLeave(0, "WebAccess", TfsTraceLayers.Framework, "TfsViewPage.InitHelpers");
      }
    }

    public override void RenderControl(HtmlTextWriter writer)
    {
      IVssRequestContext tfsRequestContext = this.TfsWebContext.TfsRequestContext;
      tfsRequestContext.TraceEnter(0, "WebAccess", TfsTraceLayers.Framework, "TfsViewPage.RenderControl");
      base.RenderControl(writer);
      tfsRequestContext.TraceLeave(0, "WebAccess", TfsTraceLayers.Framework, "TfsViewPage.RenderControl");
    }

    protected override void Render(HtmlTextWriter writer)
    {
      IVssRequestContext tfsRequestContext = this.TfsWebContext.TfsRequestContext;
      tfsRequestContext.TraceEnter(0, "WebAccess", TfsTraceLayers.Framework, "TfsViewPage.Render");
      base.Render(writer);
      tfsRequestContext.TraceLeave(0, "WebAccess", TfsTraceLayers.Framework, "TfsViewPage.Render");
    }

    private void InitTrialMessage()
    {
      IVssRequestContext tfsRequestContext = this.TfsWebContext.TfsRequestContext;
      if (!tfsRequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return;
      IVssRequestContext vssRequestContext = tfsRequestContext.To(TeamFoundationHostType.Deployment);
      string trialBannerMessage = vssRequestContext.GetService<TeamFoundationTrialService>().GetTrialBannerMessage(vssRequestContext);
      if (string.IsNullOrEmpty(trialBannerMessage))
        return;
      this.Html.AddGlobalMessage(trialBannerMessage);
    }
  }
}
