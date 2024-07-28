// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Requirements.Controllers.ApiFeedbackController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Requirements, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6C113FD4-8DA1-49E9-A859-47B7ED9A5698
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Requirements.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.Azure.Devops.Teams.Service;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Mail;
using Microsoft.TeamFoundation.Server.WebAccess.Requirements.Utilities;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Requirements.Controllers
{
  [SupportedRouteArea("Api", NavigationContextLevels.Project | NavigationContextLevels.Team)]
  [OutputCache(CacheProfile = "NoCache")]
  [DemandFeature("BB000720-4CF7-466A-BA47-1AB40B7A8DFB", false)]
  public class ApiFeedbackController : TfsController
  {
    private const string DefaultCollectionSeparator = "/DefaultCollection/";
    private const string InstallFeedbackToolUrl = "https://go.microsoft.com/fwlink/?LinkId=230568";
    private const string Feedbackurl = "{0}://{1}p:{2}?rid={3}";
    private const string ProvideFeedbackurl = "{0}/{1}/_feedbackResponse/?team={2}&id={3}";
    private const string HttpFeedbackClient = "mfbclient";
    private const string HttpsFeedbackClient = "mfbclients";
    private const string LineFeedCharacter = "&#10;";
    private const string BrBeginEndTag = "<br/>";
    private const string CommaCharacter = "%2c";
    private const string ThankYouNoteHtml = "<div><p> {0} <br/>{1}</p></div>";
    private const string ClickTwiceNoteHtml = "<div><p><i>{0}</i> <span id ='start-session-link-url'>{1}</span><i> {2}</i></p></div>";
    private const string StartFeedbackSessionHtmlText = "<div> <p>{0}</p> </div>";
    private const string EmailBodyMainHtmlText = "<div><p> <a href='{0}' target='_blank'>{1}</a></p> <p>{2} <a href='{3}' target='_blank'>{4}</a></p></div>";

    public override string TraceArea => "WebAccess.Feedback";

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(301000, 301010)]
    [DemandFeature("BB000720-4CF7-466A-BA47-1AB40B7A8DFB", true)]
    public ActionResult Configuration()
    {
      object obj1;
      try
      {
        obj1 = ConfigUtilities.BuildFeedbackRequestConfiguration(this.TfsRequestContext, CommonStructureProjectInfo.ConvertProjectInfo(this.TfsWebContext.Project), this.getTeam());
      }
      catch (ProjectSettingsException ex)
      {
        obj1 = (object) new
        {
          error = true,
          hasFeatureEnablePermission = !this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment,
          invalidProjectSettings = (ex is InvalidProjectSettingsException)
        };
      }
      // ISSUE: reference to a compiler-generated field
      if (ApiFeedbackController.\u003C\u003Eo__2.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        ApiFeedbackController.\u003C\u003Eo__2.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, ActionResult>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (ActionResult), typeof (ApiFeedbackController)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, ActionResult> target = ApiFeedbackController.\u003C\u003Eo__2.\u003C\u003Ep__1.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, ActionResult>> p1 = ApiFeedbackController.\u003C\u003Eo__2.\u003C\u003Ep__1;
      // ISSUE: reference to a compiler-generated field
      if (ApiFeedbackController.\u003C\u003Eo__2.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        ApiFeedbackController.\u003C\u003Eo__2.\u003C\u003Ep__0 = CallSite<Func<CallSite, ApiFeedbackController, object, JsonRequestBehavior, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "Json", (IEnumerable<Type>) null, typeof (ApiFeedbackController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj2 = ApiFeedbackController.\u003C\u003Eo__2.\u003C\u003Ep__0.Target((CallSite) ApiFeedbackController.\u003C\u003Eo__2.\u003C\u003Ep__0, this, obj1, JsonRequestBehavior.AllowGet);
      return target((CallSite) p1, obj2);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult ShouldShowAds() => (ActionResult) this.Json((object) this.TfsWebContext.FeatureContext.IsFeatureInAdvertisingMode(LicenseFeatures.FeedbackId), JsonRequestBehavior.AllowGet);

    [ValidateInput(false)]
    [AcceptVerbs(HttpVerbs.Post)]
    [DemandFeature("BB000720-4CF7-466A-BA47-1AB40B7A8DFB", true)]
    public void SendMailAsync([ModelBinder(typeof (JsonModelBinder))] MailMessage message, string[] titles, int[] feedbackIds)
    {
      string empty = string.Empty;
      string startFeedbackUrl = this.constructStartFeedbackUrl(feedbackIds);
      string str = empty + this.ConstructHtmlContent(titles, startFeedbackUrl) + EmailUtility.ConstructNotesText(message.Body) + this.ConstructThankYouNote(startFeedbackUrl);
      message.Body = str;
      MailSender.BeginSendMail(message, this.TfsRequestContext, this.Request.RequestContext.TfsWebContext().IsHosted, this.AsyncManager);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult SendMailCompleted() => (ActionResult) this.Json((object) MailSender.SendMailCompleted(this.AsyncManager));

    [ValidateInput(false)]
    [AcceptVerbs(HttpVerbs.Post)]
    [DemandFeature("BB000720-4CF7-466A-BA47-1AB40B7A8DFB", true)]
    public void SendFeedbackMailAsync(
      [ModelBinder(typeof (JsonModelBinder))] MailMessage message,
      string title,
      int id,
      string instructions)
    {
      string currentUserDisplayName = this.TfsWebContext.CurrentUserDisplayName;
      WebApiTeam team = this.getTeam();
      string str1 = this.constructProvideFeedbackLink(this.TfsWebContext.TfsRequestContext, this.TfsWebContext.ProjectContext.Name, team?.Name, id);
      string str2 = string.Format(FeedbackResources.TeamFormat, (object) this.TfsWebContext.ProjectContext.Name, (object) team?.Name);
      string str3 = string.Format(FeedbackResources.ModernFeedbackEmailTemplate.Replace(Environment.NewLine, " "), (object) str2, (object) FeedbackResources.NewFeedbackRequest, (object) string.Format(FeedbackResources.ProvideFeedbackFormat, (object) currentUserDisplayName, (object) currentUserDisplayName, (object) this.safeString(title)), (object) FeedbackResources.Instructions, string.IsNullOrEmpty(instructions) ? (object) FeedbackResources.NoInstructions : (object) this.safeString(instructions), (object) str1, (object) FeedbackResources.ProvideFeedback, (object) FeedbackResources.QuickTip, (object) FeedbackResources.EmailTip, (object) string.Format(FeedbackResources.EmailFooterHeaderFormat, (object) currentUserDisplayName, (object) currentUserDisplayName), (object) FeedbackResources.GetStarted, (object) FeedbackResources.LearnMore, (object) FeedbackResources.Support);
      message.Body = str3;
      MailSender.BeginSendMail(message, this.TfsRequestContext, this.Request.RequestContext.TfsWebContext().IsHosted, this.AsyncManager);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult SendFeedbackMailCompleted() => (ActionResult) this.Json((object) MailSender.SendMailCompleted(this.AsyncManager));

    private WebApiTeam getTeam()
    {
      WebApiTeam team = this.TfsWebContext.Team;
      if (team == null)
      {
        IWebTeamContext context;
        this.TfsRequestContext.TryGetWebTeamContextWithoutGlobalContext(out context);
        team = context?.Team;
      }
      return team;
    }

    private string constructProvideFeedbackLink(
      IVssRequestContext requestContext,
      string projectName,
      string teamName,
      int id)
    {
      return string.Format("{0}/{1}/_feedbackResponse/?team={2}&id={3}", (object) requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, Guid.Empty, AccessMappingConstants.PublicAccessMappingMoniker), (object) projectName, (object) teamName, (object) id);
    }

    private string constructStartFeedbackUrl(int[] feedbackids)
    {
      string empty1 = string.Empty;
      string empty2 = string.Empty;
      for (int index = 0; index < feedbackids.Length; ++index)
      {
        empty2 += feedbackids[index].ToString();
        if (index != feedbackids.Length - 1)
          empty2 += "%2c";
      }
      return ApiFeedbackController.GetFeedbackClientUrl(this.TfsWebContext.TfsRequestContext, this.TfsWebContext.ProjectContext.Name, empty2);
    }

    private static string GetFeedbackClientUrl(
      IVssRequestContext requestContext,
      string projectName,
      string serializedfeedbackIds)
    {
      string uriString = requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, Guid.Empty, AccessMappingConstants.PublicAccessMappingMoniker);
      if (uriString == null)
      {
        string absoluteUri = requestContext.RequestUri().AbsoluteUri;
        uriString = absoluteUri.Substring(0, absoluteUri.IndexOf(HttpRouteCollectionExtensions.DefaultRoutePrefix));
      }
      Uri uri = new Uri(uriString);
      string scheme = uri.Scheme;
      string str = uri.Authority + uri.PathAndQuery;
      if (requestContext.ExecutionEnvironment.IsHostedDeployment && (string.IsNullOrEmpty(uri.PathAndQuery) || uri.PathAndQuery.Equals("/")))
        str = uri.Authority + "/DefaultCollection/";
      return string.Format("{0}://{1}p:{2}?rid={3}", (object) (string.Equals(scheme, "https", StringComparison.OrdinalIgnoreCase) ? "mfbclients" : "mfbclient"), (object) str, (object) projectName, (object) serializedfeedbackIds);
    }

    private string ConstructThankYouNote(string startFeedbackUrl) => string.Format("<div><p> {0} <br/>{1}</p></div>", (object) FeedbackResources.EmailTemplate_Thanks, (object) this.TfsWebContext.User.Name) + string.Format("<div><p><i>{0}</i> <span id ='start-session-link-url'>{1}</span><i> {2}</i></p></div>", (object) FeedbackResources.EmailTemplate_StartSessionUrl_IfFailThenCopyUrl, (object) startFeedbackUrl, (object) FeedbackResources.EmailTemplate_StartSessionUrl_PasteAndLaunch);

    private string ConstructHtmlContent(string[] titles, string startFeedbackUrl) => string.Empty + string.Format("<div> <p>{0}</p> </div>", (object) FeedbackResources.EmailTemplate_WeWantYourFeedback) + this.GetFeedbackTitleOrderedListHtml(titles) + string.Format("<div><p> <a href='{0}' target='_blank'>{1}</a></p> <p>{2} <a href='{3}' target='_blank'>{4}</a></p></div>", (object) startFeedbackUrl, (object) FeedbackResources.EmailTemplate_StartYourSession, (object) FeedbackResources.EmailTemplate_IfNotInstalled, (object) "https://go.microsoft.com/fwlink/?LinkId=230568", (object) FeedbackResources.EmailTemplate_Install);

    private string GetFeedbackTitleOrderedListHtml(string[] titles)
    {
      string str1 = string.Empty + "<div> <ol>";
      foreach (string title in titles)
      {
        string str2 = this.safeString(title);
        str1 += string.Format("<li>{0}</li>", (object) str2);
      }
      return str1 + "</ol> </div>";
    }

    private string safeString(string input) => string.IsNullOrWhiteSpace(input) ? input : SafeHtmlWrapper.MakeSafeWithHtmlEncode(input, true).Replace("&#10;", "<br/>");
  }
}
