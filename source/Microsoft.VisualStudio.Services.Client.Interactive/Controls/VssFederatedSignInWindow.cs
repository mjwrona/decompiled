// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.Controls.VssFederatedSignInWindow
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.Identity.Client;
using Microsoft.VisualStudio.Services.Client.AccountManagement;
using Microsoft.VisualStudio.Services.Client.Keychain;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Contracts;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Identity.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace Microsoft.VisualStudio.Services.Client.Controls
{
  internal class VssFederatedSignInWindow
  {
    private PageHandler[] pageHandlers;
    private static readonly Regex StatusCodePattern = new Regex("HTTP (?<statusCode>[1-5]\\d{2})", RegexOptions.IgnoreCase);
    private const string StatusCodeGroupName = "statusCode";

    public VssFederatedSignInWindow(
      BrowserWindow browserWindow,
      Uri signInUrl,
      JavascriptNotifyInterop javascriptInterop)
    {
      browserWindow.Closing += new CancelEventHandler(this.OnClosing);
      javascriptInterop.ProcessingFailed += new EventHandler<ErrorData>(this.OnProcessingFailed);
      javascriptInterop.TokenDataReceived += new EventHandler<TokenData>(this.OnTokenDataReceived);
      javascriptInterop.WindowResizing += new EventHandler(this.OnWindowResizing);
      browserWindow.BrowserHandler.Failed += new NavigationFailedEventHandler(this.OnNavigationFailed);
      browserWindow.BrowserHandler.Initialized += new NavigatedEventHandler(this.OnInitialized);
      browserWindow.BrowserHandler.LoadCompleted += new NavigatedEventHandler(this.OnLoadCompleted);
      browserWindow.BrowserHandler.Navigated += new NavigatedEventHandler(this.OnFirstNavigated);
      this.SignInUrl = signInUrl;
      SessionCounterManager.EnsureSessionCounterSet();
    }

    public Uri SignInUrl { get; }

    public string AdditionalHeaders { get; set; }

    public bool IsSilentSignInFlow { get; set; }

    private void OnInitialized(object sender, NavigationEventArgs e)
    {
      if (!(sender is ComBrowserHandler comBrowserHandler))
        return;
      comBrowserHandler.Initialized -= new NavigatedEventHandler(this.OnInitialized);
      comBrowserHandler.Browser.Navigate(this.SignInUrl, (string) null, (byte[]) null, this.AdditionalHeaders);
    }

    private void OnNavigationFailed(ComBrowserHandler sender, NavigationFailedEventArgs e)
    {
      NavigationFailedEventHandler navigationFailed = this.NavigationFailed;
      if (navigationFailed == null)
        return;
      navigationFailed(sender, e);
    }

    private void OnLoadCompleted(object sender, NavigationEventArgs e) => this.MatchPageHandlers(sender, e, new VssFederatedSignInWindow.PageHandlerApplicator(this.ApplyPageHandler));

    private void OnFirstNavigated(object sender, NavigationEventArgs e)
    {
      if (!(sender is ComBrowserHandler browserHandler))
        return;
      browserHandler.Navigated -= new NavigatedEventHandler(this.OnFirstNavigated);
      string elementInnerHtmlById = browserHandler.GetElementInnerHtmlById("page-handlers");
      if (string.IsNullOrWhiteSpace(elementInnerHtmlById))
        return;
      string str = Encoding.UTF8.GetString(Convert.FromBase64String(elementInnerHtmlById));
      if (string.IsNullOrWhiteSpace(str))
        return;
      try
      {
        this.pageHandlers = JsonConvert.DeserializeObject<PageHandler[]>(str);
        foreach (PageHandler pageHandler in ((IEnumerable<PageHandler>) this.pageHandlers).Where<PageHandler>((Func<PageHandler, bool>) (handler => string.IsNullOrWhiteSpace(handler.UrlPattern))))
        {
          if (!VssFederatedSignInWindow.EnsureExplicitTrustOfSiteDependencies(e.Uri, browserHandler, pageHandler))
            return;
        }
      }
      catch
      {
      }
      this.OnNavigated(sender, e);
      browserHandler.Navigated += new NavigatedEventHandler(this.OnNavigated);
    }

    private void OnNavigated(object sender, NavigationEventArgs e)
    {
      if (!(sender is ComBrowserHandler sender1))
        return;
      bool hasDocument = sender1.HasDocument;
      if (!hasDocument || sender1.Url.StartsWith("res://"))
      {
        if (this.NavigationFailed == null)
          return;
        string input = (string) null;
        if (hasDocument)
          input = sender1.DocumentInnerHtml;
        ErrorData error = new ErrorData()
        {
          Details = ClientResources.BrowserNavigationToUrlFailed() + (e.Uri == (Uri) null ? ClientResources.None() : e.Uri.AbsoluteUri),
          Uri = e.Uri
        };
        int helpLinkId = 0;
        HttpStatusCode result = HttpStatusCode.Unused;
        if (!string.IsNullOrWhiteSpace(input))
        {
          error.Content = input;
          Match match = VssFederatedSignInWindow.StatusCodePattern.Match(input);
          if (match.Success && System.Enum.TryParse<HttpStatusCode>(match.Groups["statusCode"].Value, out result))
          {
            error.Message = BrowserFlowException.GetNavigationExceptionMessage(result);
            helpLinkId = BrowserFlowException.GetNavigationHelpLinkId(result);
            error.StatusCode = (int) result;
          }
        }
        if (string.IsNullOrWhiteSpace(error.Message))
        {
          error.Message = BrowserFlowException.GetNavigationExceptionMessage(result);
          helpLinkId = BrowserFlowException.GetNavigationHelpLinkId(result);
        }
        BrowserFlowException browserFlowException = new BrowserFlowException(BrowserFlowLayer.Navigation, error, error.Message);
        if (helpLinkId > 0)
          browserFlowException.HelpLink = BrowserFlowException.FormatHelpLink(helpLinkId);
        this.NavigationFailed(sender1, new NavigationFailedEventArgs(e.Uri, (Exception) browserFlowException));
      }
      else
        this.MatchPageHandlers(sender, e, new VssFederatedSignInWindow.PageHandlerApplicator(this.ApplyNavigationPageHandler));
    }

    private void ApplyNavigationPageHandler(
      Uri uri,
      ComBrowserHandler browserHandler,
      PageHandler pageHandler)
    {
      if (!VssFederatedSignInWindow.EnsureExplicitTrustOfSiteDependencies(uri, browserHandler, pageHandler))
        return;
      if (pageHandler.Styles != null && pageHandler.Styles.Length != 0)
        browserHandler.InsertLocalStyleSheet((IEnumerable<PageHandlerStyle>) pageHandler.Styles);
      if (string.IsNullOrWhiteSpace(pageHandler.NavigationJavaScript))
        return;
      browserHandler.Evaluate(pageHandler.NavigationJavaScript);
    }

    private void ApplyPageHandler(
      Uri uri,
      ComBrowserHandler browserHandler,
      PageHandler pageHandler)
    {
      if (string.IsNullOrWhiteSpace(pageHandler.JavaScript))
        return;
      browserHandler.Evaluate(pageHandler.JavaScript);
    }

    private void MatchPageHandlers(
      object sender,
      NavigationEventArgs e,
      VssFederatedSignInWindow.PageHandlerApplicator applicator)
    {
      if (!(sender is ComBrowserHandler browserHandler) || !(e.Uri != (Uri) null) || this.pageHandlers == null || this.pageHandlers.Length == 0)
        return;
      string absoluteUri = e.Uri.AbsoluteUri;
      foreach (PageHandler pageHandler in this.pageHandlers)
      {
        try
        {
          if (!string.IsNullOrWhiteSpace(pageHandler.UrlPattern))
          {
            if (Regex.IsMatch(absoluteUri, pageHandler.UrlPattern, RegexOptions.IgnoreCase))
            {
              applicator(e.Uri, browserHandler, pageHandler);
              if (pageHandler.Terminate.HasValue)
              {
                if (pageHandler.Terminate.Value)
                  break;
              }
            }
          }
        }
        catch (Exception ex)
        {
        }
      }
    }

    private static bool EnsureExplicitTrustOfSiteDependencies(
      Uri uri,
      ComBrowserHandler browserHandler,
      PageHandler pageHandler)
    {
      if (pageHandler.SiteDependencies != null && pageHandler.SiteDependencies.Length != 0)
      {
        foreach (string siteDependency in pageHandler.SiteDependencies)
        {
          if (!string.IsNullOrWhiteSpace(siteDependency) && !browserHandler.EnsureExplicitTrust(uri, siteDependency, true))
            return false;
        }
      }
      return true;
    }

    public event NavigationFailedEventHandler NavigationFailed;

    public event EventHandler ProcessingFailed;

    public event EventHandler TokenReceived;

    public event EventHandler WindowResizing;

    public ClientTokenData ClientTokenData { get; private set; }

    public ErrorData Error { get; private set; }

    public TokenData TokenData { get; private set; }

    private void OnProcessingFailed(object sender, ErrorData error)
    {
      this.Error = error;
      EventHandler processingFailed = this.ProcessingFailed;
      if (processingFailed == null)
        return;
      processingFailed((object) this, EventArgs.Empty);
    }

    private async void OnTokenDataReceived(object sender, TokenData data)
    {
      VssFederatedSignInWindow sender1 = this;
      sender1.TokenData = data;
      try
      {
        if (data != null)
        {
          if (!sender1.IsSilentSignInFlow)
            await Task.Run((Func<Task>) (async () =>
            {
              string accountName = string.Empty;
              string homeTenant = VssAadSettings.ApplicationTenant;
              IdentitySelf identitySelfAsync = await new IdentityHttpClient(AccountManager.VsoEndpoint, (VssCredentials) (FederatedCredential) new VssFederatedCredential(initialToken: new VssFederatedToken(CookieUtility.GetFederatedCookies(data.SecurityToken)))).GetIdentitySelfAsync((object) null, new CancellationToken());
              IAccountCache cache = AccountManager.DefaultInstance.GetCache<IAccountCache>();
              if (identitySelfAsync != null)
              {
                accountName = identitySelfAsync.AccountName;
                IEnumerable<TenantInfo> source = identitySelfAsync.Tenants.Where<TenantInfo>((Func<TenantInfo, bool>) (user => user.HomeTenant));
                if (source.Any<TenantInfo>())
                  homeTenant = source.First<TenantInfo>().TenantId.ToString();
              }
              IAccountCacheItem authenticationResult;
              if (string.IsNullOrEmpty(accountName))
                authenticationResult = await cache.AcquireTokenInteractiveAsync(VssAadSettings.DefaultScopes, Prompt.Never, homeTenant);
              else
                authenticationResult = await cache.AcquireTokenSilentAsync(VssAadSettings.DefaultScopes, accountName, homeTenant);
              if (authenticationResult == null)
              {
                accountName = (string) null;
                homeTenant = (string) null;
              }
              else if (!(AccountManager.DefaultInstance.GetAccountProvider(VSAccountProvider.AccountProviderIdentifier) is VSAccountProvider accountProvider2))
              {
                accountName = (string) null;
                homeTenant = (string) null;
              }
              else
              {
                string str = await accountProvider2.ProcessAuthenticationResult(authenticationResult);
                accountName = (string) null;
                homeTenant = (string) null;
              }
            }));
        }
      }
      catch (Exception ex)
      {
        Exception exception = ex;
        if (exception is AggregateException)
          exception = ((AggregateException) exception).Flatten().InnerException;
        string errorCode = exception.HResult.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        if (exception is MsalClientException msalClientException)
          errorCode = msalClientException.ErrorCode;
        AccountManager.Logger.LogEvent("VssFederatedSignInWindowOnTokenDataReceived", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            "ExceptionMessage",
            (object) exception.Message
          },
          {
            "ExceptionErrorCode",
            (object) errorCode
          },
          {
            "ExceptionType",
            (object) exception.GetType().FullName
          }
        });
      }
      EventHandler tokenReceived = sender1.TokenReceived;
      if (tokenReceived == null)
        return;
      tokenReceived((object) sender1, EventArgs.Empty);
    }

    private void OnWindowResizing(object sender, EventArgs e)
    {
      EventHandler windowResizing = this.WindowResizing;
      if (windowResizing == null)
        return;
      windowResizing((object) this, EventArgs.Empty);
    }

    private void OnClosing(object sender, EventArgs e)
    {
      BrowserWindow browserWindow = (BrowserWindow) sender;
      if (browserWindow.BrowserHandler != null)
      {
        browserWindow.BrowserHandler.Failed -= new NavigationFailedEventHandler(this.OnNavigationFailed);
        browserWindow.BrowserHandler.Initialized -= new NavigatedEventHandler(this.OnInitialized);
        browserWindow.BrowserHandler.LoadCompleted -= new NavigatedEventHandler(this.OnLoadCompleted);
        browserWindow.BrowserHandler.Navigated -= new NavigatedEventHandler(this.OnFirstNavigated);
        browserWindow.BrowserHandler.Navigated -= new NavigatedEventHandler(this.OnNavigated);
      }
      browserWindow.Closing -= new CancelEventHandler(this.OnClosing);
      JavascriptNotifyInterop external = (JavascriptNotifyInterop) browserWindow.BrowserHandler.External;
      external.ProcessingFailed -= new EventHandler<ErrorData>(this.OnProcessingFailed);
      external.TokenDataReceived -= new EventHandler<TokenData>(this.OnTokenDataReceived);
      external.WindowResizing -= new EventHandler(this.OnWindowResizing);
    }

    private delegate void PageHandlerApplicator(
      Uri uri,
      ComBrowserHandler browserHandler,
      PageHandler pageHandler);
  }
}
