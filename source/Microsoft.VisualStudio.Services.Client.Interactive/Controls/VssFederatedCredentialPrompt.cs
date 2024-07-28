// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.Controls.VssFederatedCredentialPrompt
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Contracts;
using Microsoft.VisualStudio.Services.Common.Utility;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace Microsoft.VisualStudio.Services.Client.Controls
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class VssFederatedCredentialPrompt : IVssCredentialPrompt
  {
    static VssFederatedCredentialPrompt() => BrowserEmulationVersion.TrySetBrowserVersion();

    public VssFederatedCredentialPrompt()
      : this(new VssCredentialPromptContext())
    {
    }

    public VssFederatedCredentialPrompt(IDialogHost host)
      : this(new VssCredentialPromptContext(host))
    {
    }

    public VssFederatedCredentialPrompt(VssCredentialPromptContext context)
    {
      ArgumentUtility.CheckForNull<VssCredentialPromptContext>(context, nameof (context));
      this.CompactSize = true;
      this.Context = context;
    }

    public VssCredentialPromptContext Context { get; set; }

    public bool CompactSize { get; set; }

    public VssConnectMode ConnectMode
    {
      get
      {
        string str = (string) null;
        VssConnectMode result;
        if (this.Parameters == null || !this.Parameters.TryGetValue("vssConnectionMode", out str) || !System.Enum.TryParse<VssConnectMode>(str, out result))
          result = VssConnectMode.Automatic;
        return result;
      }
      set
      {
        if (this.Parameters == null)
          this.Parameters = (IDictionary<string, string>) new Dictionary<string, string>();
        this.Parameters["vssConnectionMode"] = value.ToString();
      }
    }

    public IDictionary<string, string> TokenProperties { get; set; }

    public IDictionary<string, string> Parameters { get; set; }

    Task<IssuedToken> IVssCredentialPrompt.GetTokenAsync(
      IssuedTokenProvider provider,
      IssuedToken failedToken)
    {
      this.TokenProperties = (IDictionary<string, string>) null;
      return this.GetCookieAsync(provider.SignInUrl).ContinueWith<IssuedToken>((Func<Task<CookieCollection>, IssuedToken>) (t =>
      {
        if (t.Result == null)
          return (IssuedToken) null;
        return (IssuedToken) new VssFederatedToken(t.Result)
        {
          Properties = this.TokenProperties
        };
      }));
    }

    public async Task<CookieCollection> GetCookieAsync(Uri signInUrl)
    {
      if (signInUrl.Query.IndexOf("compact=", StringComparison.OrdinalIgnoreCase) < 0)
        signInUrl = signInUrl.AppendQuery("compact", this.CompactSize ? "1" : "0");
      signInUrl = signInUrl.AppendQuery("mode", this.ConnectMode.ToString().ToLower(CultureInfo.InvariantCulture));
      string str1 = (string) null;
      if (this.Parameters != null && this.Parameters.TryGetValue("user", out str1) && !string.IsNullOrWhiteSpace(str1))
        signInUrl = signInUrl.AppendQuery("user", str1);
      if (signInUrl.Query.IndexOf("mkt=", StringComparison.OrdinalIgnoreCase) < 0)
        signInUrl = signInUrl.AppendQuery("mkt", CultureInfo.CurrentUICulture.Name);
      IEnumerable<string> signInCookieDomains = VssFederatedCredentialPrompt.GetSignInCookieDomains(signInUrl.Fragment);
      if (signInCookieDomains != null)
      {
        foreach (string uriString in signInCookieDomains)
          CookieUtility.DeleteAllCookies(new Uri(uriString));
      }
      signInUrl = VssFederatedCredentialPrompt.RemoveFragmentFromUri(signInUrl);
      CookieUtility.DeleteWindowsLiveCookies();
      VssFederatedCredentialPrompt.DialogInvokeParameters dialogParameters = new VssFederatedCredentialPrompt.DialogInvokeParameters()
      {
        Location = signInUrl,
        CompactSize = this.CompactSize,
        Visibility = true,
        IsReauthenticating = !string.IsNullOrWhiteSpace(str1),
        ConnectMode = this.ConnectMode
      };
      string str2 = (string) null;
      if (this.Parameters != null && this.Parameters.TryGetValue("accessToken", out str2) && !string.IsNullOrWhiteSpace(str2))
      {
        dialogParameters.AccessToken = str2;
        dialogParameters.Visibility = false;
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      bool? nullable = await this.Context.DialogHost.InvokeDialogAsync(VssFederatedCredentialPrompt.\u003C\u003EO.\u003C0\u003E__InvokeDialog ?? (VssFederatedCredentialPrompt.\u003C\u003EO.\u003C0\u003E__InvokeDialog = new InvokeDialogFunc(VssFederatedCredentialPrompt.InvokeDialog)), (object) dialogParameters).ConfigureAwait(false);
      if (this.Parameters != null && this.Parameters.ContainsKey("user"))
        this.Parameters.Remove("user");
      if (dialogParameters.Cancelled)
      {
        if (dialogParameters.Exception != null)
          throw dialogParameters.Exception;
        throw new OperationCanceledException(ClientResources.SignInCancelled());
      }
      CookieCollection federatedCookies;
      if (dialogParameters.TokenData != null)
      {
        federatedCookies = CookieUtility.GetFederatedCookies(dialogParameters.TokenData.SecurityToken);
        this.TokenProperties = dialogParameters.TokenProperties;
      }
      CookieCollection cookieAsync = federatedCookies;
      dialogParameters = (VssFederatedCredentialPrompt.DialogInvokeParameters) null;
      return cookieAsync;
    }

    private static bool? InvokeDialog(IntPtr owner, object state)
    {
      if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
        throw new InvalidOperationException(ClientResources.STAThreadRequired());
      VssFederatedCredentialPrompt.DialogInvokeParameters parameters = (VssFederatedCredentialPrompt.DialogInvokeParameters) state;
      JavascriptNotifyInterop javascriptNotifyInterop = new JavascriptNotifyInterop();
      BrowserWindow window = new BrowserWindow((object) javascriptNotifyInterop, parameters.CompactSize ? BrowserWindow.CompactSize : BrowserWindow.FullSize);
      string str = "";
      bool flag = false;
      if (!string.IsNullOrEmpty(parameters.AccessToken))
      {
        str += string.Format("Authorization: Bearer {0}", (object) parameters.AccessToken);
        flag = true;
      }
      VssFederatedSignInWindow signInWindow = new VssFederatedSignInWindow(window, parameters.Location, javascriptNotifyInterop);
      signInWindow.AdditionalHeaders = str;
      signInWindow.IsSilentSignInFlow = flag;
      if (!parameters.Visibility)
      {
        window.AllowsTransparency = true;
        window.Opacity = 0.0;
      }
      try
      {
        window.BrowserHandler.Initialize();
      }
      catch (Exception ex1)
      {
        try
        {
          window.Close();
        }
        catch (Exception ex2)
        {
        }
        BrowserFlowException browserFlowException = new BrowserFlowException(BrowserFlowLayer.Client, (ErrorData) null, ClientResources.UnknownClientError(), ex1);
        browserFlowException.HelpLink = BrowserFlowException.FormatHelpLink(324098);
        throw browserFlowException;
      }
      EventHandler eventHandler1 = (EventHandler) ((sender, e) =>
      {
        int layer = signInWindow.Error.StatusCode > 0 ? 2 : 3;
        string message = signInWindow.Error.Message;
        if (layer == 2)
          message = BrowserFlowException.GetServerExceptionMessage((HttpStatusCode) signInWindow.Error.StatusCode);
        else if (signInWindow.Error.StatusCode == 0)
          message = ClientResources.UnknownClientError();
        int helpLinkId = layer == 3 ? 324098 : BrowserFlowException.GetServerHelpLinkId((HttpStatusCode) signInWindow.Error.StatusCode);
        parameters.Exception = (Exception) new BrowserFlowException((BrowserFlowLayer) layer, signInWindow.Error, message)
        {
          HelpLink = BrowserFlowException.FormatHelpLink(helpLinkId)
        };
        window.Close();
      });
      NavigationFailedEventHandler failedEventHandler = (NavigationFailedEventHandler) ((sender, e) =>
      {
        parameters.Exception = e.Exception;
        window.Close();
      });
      EventHandler eventHandler2 = (EventHandler) ((sender, e) =>
      {
        parameters.Cancelled = false;
        window.Close();
      });
      EventHandler eventHandler3 = (EventHandler) ((sender, e) =>
      {
        Size size = BrowserWindow.FullSize;
        if (SystemParameters.PrimaryScreenWidth > BrowserWindow.MaxResizeCheckSize.Width && SystemParameters.PrimaryScreenHeight > BrowserWindow.MaxResizeCheckSize.Height)
          size = BrowserWindow.MaxResize;
        double num1 = window.Top - (size.Height - window.Height) / 2.0;
        if (num1 > 0.0)
          window.Top = num1;
        double num2 = window.Left - (size.Width - window.Width) / 2.0;
        if (num2 > 0.0)
          window.Left = num2 + size.Width > SystemParameters.VirtualScreenWidth ? SystemParameters.VirtualScreenWidth - size.Width : num2;
        window.Width = size.Width;
        window.Height = size.Height;
      });
      signInWindow.ProcessingFailed += eventHandler1;
      signInWindow.NavigationFailed += failedEventHandler;
      signInWindow.TokenReceived += eventHandler2;
      signInWindow.WindowResizing += eventHandler3;
      parameters.Cancelled = true;
      if (owner == IntPtr.Zero)
        owner = ClientNativeMethods.GetDefaultParentWindow();
      if (owner != IntPtr.Zero)
      {
        new WindowInteropHelper((Window) window).Owner = owner;
      }
      else
      {
        window.ShowInTaskbar = true;
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        window.Activated += VssFederatedCredentialPrompt.\u003C\u003EO.\u003C1\u003E__BlinkHandler ?? (VssFederatedCredentialPrompt.\u003C\u003EO.\u003C1\u003E__BlinkHandler = new EventHandler(VssFederatedCredentialPrompt.BlinkHandler));
      }
      VssFederatedCredentialPrompt.OnShowingDialog((Window) window);
      if (VssFederatedCredentialPrompt.InteractiveAuthentication != null)
        VssFederatedCredentialPrompt.InteractiveAuthentication(new InteractiveAuthenticationEvent()
        {
          IsReauthenticating = parameters.IsReauthenticating,
          ConnectMode = parameters.ConnectMode
        });
      bool? nullable = window.ShowDialog();
      signInWindow.ProcessingFailed -= eventHandler1;
      signInWindow.NavigationFailed -= failedEventHandler;
      signInWindow.TokenReceived -= eventHandler2;
      signInWindow.WindowResizing -= eventHandler3;
      if (parameters.Cancelled)
        return nullable;
      parameters.TokenData = signInWindow.TokenData;
      ClientTokenData clientTokenData = signInWindow.ClientTokenData;
      if (clientTokenData == null)
        return nullable;
      parameters.TokenProperties = clientTokenData.TokenProperties;
      return nullable;
    }

    private static void BlinkHandler(object sender, EventArgs e)
    {
      Window window = (Window) sender;
      IntPtr handle = new WindowInteropHelper(window).Handle;
      IntPtr foregroundWindow = ClientNativeMethods.GetForegroundWindow();
      if (IntPtr.Zero != handle && foregroundWindow != handle)
      {
        int num = 1;
        ClientNativeMethods.SystemParametersInfo(8196, 0, ref num, 0);
        ClientNativeMethods.FlashWindowEx(ref new ClientNativeMethods.FLASHWINFO()
        {
          cbSize = (uint) Marshal.SizeOf(typeof (ClientNativeMethods.FLASHWINFO)),
          dwFlags = 3U,
          dwTimeout = 0U,
          uCount = (uint) num,
          hwnd = handle
        });
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      window.Activated -= VssFederatedCredentialPrompt.\u003C\u003EO.\u003C1\u003E__BlinkHandler ?? (VssFederatedCredentialPrompt.\u003C\u003EO.\u003C1\u003E__BlinkHandler = new EventHandler(VssFederatedCredentialPrompt.BlinkHandler));
    }

    private static IEnumerable<string> GetSignInCookieDomains(string urlFragment)
    {
      if (urlFragment == null)
        return (IEnumerable<string>) null;
      string str1 = urlFragment;
      char[] chArray1 = new char[1]{ '&' };
      foreach (string str2 in str1.Split(chArray1))
      {
        char[] chArray2 = new char[1]{ '=' };
        string[] strArray = str2.Split(chArray2);
        if (strArray.Length == 2 && strArray[0].Replace("#", "") == "ctx")
          return (IEnumerable<string>) JsonConvert.DeserializeObject<VssSignInContextModel>(UrlEncodingUtility.UrlTokenDecodeToString(strArray[1])).SignInCookieDomains;
      }
      return (IEnumerable<string>) null;
    }

    private static Uri RemoveFragmentFromUri(Uri existingUri) => new UriBuilder(existingUri)
    {
      Fragment = ""
    }.Uri;

    private static void OnShowingDialog(Window window)
    {
      try
      {
        Action<Window> dialogShowing = VssFederatedCredentialPrompt.DialogShowing;
        if (dialogShowing == null)
          return;
        dialogShowing(window);
      }
      catch (Exception ex)
      {
      }
    }

    public static event Action<Window> DialogShowing;

    internal static event Action<InteractiveAuthenticationEvent> InteractiveAuthentication;

    private class DialogInvokeParameters
    {
      public Uri Location;
      public bool CompactSize;
      public bool Cancelled;
      public Exception Exception;
      public TokenData TokenData;
      public IDictionary<string, string> TokenProperties;
      public string AccessToken;
      public bool Visibility;
      public bool IsReauthenticating;
      public VssConnectMode ConnectMode;
    }
  }
}
