// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Account.PublicKeyController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Account, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC21A176-69BE-407E-B3DD-80612369F784
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Account.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Html;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.TeamFoundation.Ssh.Server.External.Eldos;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Account
{
  [SupportedRouteArea(NavigationContextLevels.All)]
  [DemandFeature("00000000-0000-0000-0000-000000000000", false)]
  public class PublicKeyController : AccountAreaController
  {
    private string s_TraceLayer = "PublicKey";
    private const string PrivateKeyIdentifier = "PRIVATE";

    private IPublicKeyUtility PublicKeyUtility { get; set; }

    public PublicKeyController()
    {
      this.PublicKeyUtility = (IPublicKeyUtility) new DefaultPublicKeyUtility();
      this.m_executeContributedRequestHandlers = true;
    }

    public PublicKeyController(IPublicKeyUtility publicKeyUtility)
    {
      this.PublicKeyUtility = publicKeyUtility ?? (IPublicKeyUtility) new DefaultPublicKeyUtility();
      this.m_executeContributedRequestHandlers = true;
    }

    [HttpGet]
    [TfsTraceFilter(506001, 506009)]
    [TfsHandleFeatureFlag("WebAccess.SshUI", null)]
    public ActionResult Index()
    {
      PublicKeyIndexModel model = new PublicKeyIndexModel();
      model.PublicKeysDisabled = this.GetDisallowSecureShellPolicyEffectiveValue();
      this.ConfigureLeftHubSplitter(AccountServerResources.SecurityDetailsNavigationSplitter, toggleButtonCollapsedTooltip: AccountServerResources.SecurityDetailsNavigationSplitterCollapsed, toggleButtonExpandedTooltip: AccountServerResources.SecurityDetailsNavigationSplitterExpanded);
      return (ActionResult) this.View((object) model);
    }

    [HttpGet]
    [TfsTraceFilter(506010, 506019)]
    [TfsHandleFeatureFlag("WebAccess.SshUI", null)]
    public ActionResult Edit(Guid? authorizationId = null)
    {
      this.PreventActionIfPolicyDisallowsSecureShell();
      IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
      PublicKeyEditModel model = new PublicKeyEditModel();
      model.PublicKey = (PublicKeyModel) null;
      if (authorizationId.HasValue && authorizationId.HasValue && authorizationId.Value != Guid.Empty)
      {
        SessionToken sessionToken = vssRequestContext.GetService<IDelegatedAuthorizationService>().GetSessionToken(vssRequestContext, authorizationId.Value, true);
        model.PublicKey = new PublicKeyModel(sessionToken);
      }
      return (ActionResult) this.View((object) model);
    }

    [HttpPost]
    [ValidateInput(false)]
    [TfsTraceFilter(506020, 506029)]
    [TfsHandleFeatureFlag("WebAccess.SshUI", null)]
    public ActionResult Edit(PublicKeyModel publicKey)
    {
      this.PreventActionIfPolicyDisallowsSecureShell();
      IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      DateTime maxValue = DateTime.MaxValue;
      string base64String = Convert.ToBase64String(this.ExtractAndVerifySSHRSAKey(publicKey));
      System.Collections.Generic.List<Guid> guidList1 = new System.Collections.Generic.List<Guid>();
      IVssRequestContext context = vssRequestContext;
      try
      {
        if (this.TfsRequestContext.IsHosted())
        {
          context = this.TfsRequestContext;
          guidList1.Add(this.TfsRequestContext.ServiceHost.InstanceId);
        }
      }
      catch (InvalidOperationException ex)
      {
      }
      try
      {
        IDelegatedAuthorizationService service = context.GetService<IDelegatedAuthorizationService>();
        IVssRequestContext requestContext = context;
        string description = publicKey.Description;
        DateTime? nullable = new DateTime?(maxValue);
        IList<Guid> guidList2 = (IList<Guid>) guidList1;
        string str = base64String;
        Guid? clientId = new Guid?();
        Guid? userId = new Guid?();
        string name = description;
        DateTime? validTo = nullable;
        IList<Guid> targetAccounts = guidList2;
        string publicData = str;
        Guid? authorizationId = new Guid?();
        Guid? accessId = new Guid?();
        SessionTokenResult data = service.IssueSessionToken(requestContext, clientId, userId, name, validTo, targetAccounts: targetAccounts, tokenType: SessionTokenType.Compact, isPublic: true, publicData: publicData, authorizationId: authorizationId, accessId: accessId);
        if (!data.HasError)
          return (ActionResult) this.Json((object) data);
        if (data.SessionTokenError == SessionTokenError.DuplicateHash)
          throw new SessionTokenCreateException(data.SessionTokenError.ToString());
        throw new HttpException(500, data.SessionTokenError.ToString());
      }
      catch (SessionTokenCreateException ex)
      {
        this.TfsRequestContext.TraceException(506028, this.AreaName, this.s_TraceLayer, (Exception) ex);
        SessionTokenError result = SessionTokenError.None;
        if (Enum.TryParse<SessionTokenError>(ex.Message, out result) && result == SessionTokenError.DuplicateHash)
          throw new HttpException(400, AccountServerResources.SSH_DuplicateKeyAdd);
        throw new HttpException(400, AccountServerResources.SSH_AddFailed);
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(506028, this.AreaName, this.s_TraceLayer, ex);
        throw new HttpException(500, AccountServerResources.SSH_AddFailed);
      }
    }

    [HttpGet]
    [TfsTraceFilter(506030, 506039)]
    [TfsHandleFeatureFlag("WebAccess.SshUI", null)]
    public ActionResult List()
    {
      if (this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
        this.TfsRequestContext.CheckProjectCollectionRequestContext();
      this.PreventActionIfPolicyDisallowsSecureShell();
      IVssRequestContext vssRequestContext1 = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
      IVssRequestContext vssRequestContext2 = vssRequestContext1;
      Guid organizationId = Guid.Empty;
      Guid collectionId = Guid.Empty;
      UserPreferences userPreferences = vssRequestContext1.GetService<IUserPreferencesService>().GetUserPreferences(vssRequestContext1);
      try
      {
        organizationId = this.TfsRequestContext.To(TeamFoundationHostType.Application).ServiceHost.InstanceId;
        vssRequestContext2 = this.TfsRequestContext;
        collectionId = this.TfsRequestContext.ServiceHost.InstanceId;
      }
      catch (InvalidOperationException ex)
      {
      }
      return (ActionResult) this.Json((object) vssRequestContext2.GetService<IDelegatedAuthorizationService>().ListSessionTokens(vssRequestContext2, true, true).Where<SessionToken>((Func<SessionToken, bool>) (x =>
      {
        if (x.TargetAccounts == null || !x.TargetAccounts.Any<Guid>() || organizationId != Guid.Empty && x.TargetAccounts.Contains(organizationId))
          return true;
        return collectionId != Guid.Empty && x.TargetAccounts.Contains(collectionId);
      })).Select<SessionToken, PublicKeyModel>((Func<SessionToken, PublicKeyModel>) (x => new PublicKeyModel(x, datePattern: userPreferences.DatePattern))), JsonRequestBehavior.AllowGet);
    }

    [HttpDelete]
    [TfsBypassAntiForgeryValidation]
    [TfsTraceFilter(506040, 506049)]
    [TfsHandleFeatureFlag("WebAccess.SshUI", null)]
    public ActionResult Revoke(Guid authorizationId)
    {
      this.PreventActionIfPolicyDisallowsSecureShell();
      IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetService<IDelegatedAuthorizationService>().RevokeSessionToken(vssRequestContext, authorizationId, true);
      return (ActionResult) new HttpStatusCodeResult(200);
    }

    [HttpGet]
    [TfsTraceFilter(506050, 506059)]
    [TfsHandleFeatureFlag("WebAccess.SshUI", null)]
    public ActionResult GetServerFingerprint()
    {
      this.PreventActionIfPolicyDisallowsSecureShell();
      IVssRequestContext context = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
      TeamFoundationSshKeyService service = context.GetService<TeamFoundationSshKeyService>();
      string sha256Fingerprint = service.GetSha256Fingerprint(context.Elevate());
      return (ActionResult) this.Json((object) new System.Collections.Generic.List<ServerFingerprintModel>()
      {
        new ServerFingerprintModel(service.GetMD5Fingerprint(context.Elevate()), "MD5", "RSA"),
        new ServerFingerprintModel(sha256Fingerprint, "SHA256", "RSA")
      }, JsonRequestBehavior.AllowGet);
    }

    private void PreventActionIfPolicyDisallowsSecureShell()
    {
      if (this.GetDisallowSecureShellPolicyEffectiveValue())
        throw new HttpException(404, PlatformResources.PageNotFound);
    }

    private bool GetDisallowSecureShellPolicyEffectiveValue()
    {
      bool policyEffectiveValue = false;
      try
      {
        if (this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
          policyEffectiveValue = this.TfsRequestContext.GetService<IOrganizationPolicyService>().GetPolicy<bool>(this.TfsRequestContext.Elevate(), "Policy.DisallowSecureShell", false).EffectiveValue;
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(506008, this.TraceArea, this.s_TraceLayer, ex);
      }
      return policyEffectiveValue;
    }

    internal byte[] ExtractAndVerifySSHRSAKey(PublicKeyModel publicKey)
    {
      KeyFormat format;
      byte[] publicKey1 = this.PublicKeyUtility.ExtractPublicKey(publicKey.Data, out format);
      if (publicKey1 == null)
      {
        if (publicKey.Data.IndexOf("PRIVATE", 0, StringComparison.OrdinalIgnoreCase) != -1)
          throw new InvalidPublicKeyException(AccountServerResources.SSH_PrivateKeyUpload);
        throw new InvalidPublicKeyException(AccountServerResources.SSH_InvalidKey);
      }
      if (format == KeyFormat.OpenSSH && this.PublicKeyUtility.HasSshRsaHeader(publicKey.Data))
        return publicKey1;
      throw new InvalidPublicKeyException(AccountServerResources.SSH_InvalidKey);
    }
  }
}
