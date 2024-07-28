// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.Controls.ComBrowserHandler
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.CSharp.RuntimeBinder;
using Microsoft.VisualStudio.Services.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace Microsoft.VisualStudio.Services.Client.Controls
{
  internal class ComBrowserHandler : IDisposable
  {
    private const uint INET_E_DEFAULT_ACTION = 2148270097;
    private const uint INET_E_USE_DEFAULT_PROTOCOLHANDLER = 2148270097;
    private const uint E_NOINTERFACE = 2147500034;
    private const uint CLASS_E_NOAGGREGATION = 2147746064;
    private const uint INET_E_REDIRECT_FAILED = 2148270100;
    private const uint E_FAIL = 2147500037;
    private ComBrowserHandler.ComEventSink<ComBrowserHandler.DWebBrowserEvents2, ComBrowserHandler.ComBrowserEvents> browserEvents;
    private ComBrowserHandler.IWebBrowser2 comBrowser;
    private bool confirmedExplicitTrust;
    private ComBrowserHandler.ComDocumentHostHandler documentHostHandler;
    private Lazy<ComBrowserHandler.IInternetSecurityManager> securityManager = new Lazy<ComBrowserHandler.IInternetSecurityManager>(ComBrowserHandler.\u003C\u003EO.\u003C0\u003E__CreateInternetSecurityManager ?? (ComBrowserHandler.\u003C\u003EO.\u003C0\u003E__CreateInternetSecurityManager = new Func<ComBrowserHandler.IInternetSecurityManager>(ComBrowserHandler.CreateInternetSecurityManager)), true);
    private Lazy<HashSet<string>> trustedUrlPatterns = new Lazy<HashSet<string>>();
    private ComBrowserHandler.AsyncPluggableProtocolSession urnProtocolSession;
    private const int PUAF_NOUI = 1;
    private const int PUAF_NOUIIFLOCKED = 1048576;
    private const int URLACTION_SCRIPT_RUN = 5120;
    private const int URLPOLICY_ALLOW = 0;
    private const int URLPOLICY_DISALLOW = 3;
    private const int OLECMDID_OPTICAL_ZOOM = 63;
    private const int OLECMDEXECOPT_DONTPROMPTUSER = 2;
    private const int S_OK = 0;
    private const int S_FALSE = 1;
    private const int URLZONE_TRUSTED = 2;
    private const int URLZONE_ESC_FLAG = 256;
    private const int SZM_CREATE = 0;
    private const int E_INVALIDARG = -2147024809;
    private const int E_NOTIMPLEMENTED = -2147467263;
    private const string EmptyContent = "<html/>";
    private static readonly Guid CLSID_InternetSecurityManager = new Guid("7B8A2D94-0AC9-11D1-896C-00C04FB6BFC4");
    private static readonly Guid SID_WebBrowser = new Guid("0002DF05-0000-0000-C000-000000000046");

    [DllImport("urlmon.dll", PreserveSig = false)]
    private static extern void CoInternetGetSession(
      uint dwSessionMode,
      out ComBrowserHandler.IInternetSession ppIInternetSession,
      uint dwReserved);

    private static ComBrowserHandler.IInternetSecurityManager CreateInternetSecurityManager()
    {
      try
      {
        return Activator.CreateInstance(Type.GetTypeFromCLSID(ComBrowserHandler.CLSID_InternetSecurityManager)) as ComBrowserHandler.IInternetSecurityManager;
      }
      catch
      {
      }
      return (ComBrowserHandler.IInternetSecurityManager) null;
    }

    public ComBrowserHandler(
      WebBrowser browser,
      bool blockDragDrop,
      bool requireScript,
      object external)
    {
      this.BlockDragDrop = blockDragDrop;
      this.Browser = browser;
      this.External = external;
      this.RequireScript = requireScript;
    }

    public event HtmlContextMenuEventHandler ContextMenu;

    public event NavigationFailedEventHandler Failed;

    public event NavigatedEventHandler Initialized;

    public event NavigatedEventHandler LoadCompleted;

    public event NavigatedEventHandler Navigated;

    public event NavigatingCancelEventHandler Navigating;

    public event CustomNavigationEventHandler CustomNavigation;

    public bool BlockDragDrop { get; private set; }

    public WebBrowser Browser { get; private set; }

    public object External { get; private set; }

    public bool RequireScript { get; private set; }

    public string BodyInnerHtml => this.Browser.Document is ComBrowserHandler.IHTMLDocument2 document && document.body != null ? document.body.innerHTML : (string) null;

    public string DocumentInnerHtml => this.Browser.Document is ComBrowserHandler.IHTMLDocument3 document && document.documentElement != null ? document.documentElement.innerHTML : (string) null;

    public bool HasDocument => this.Browser.Document is ComBrowserHandler.IHTMLDocument2;

    public string Url => this.Browser.Document is ComBrowserHandler.IHTMLDocument2 document ? document.url : (string) null;

    private ComBrowserHandler.IInternetSecurityManager SecurityManager => this.securityManager.Value;

    private HashSet<string> TrustedUrlPatterns => this.trustedUrlPatterns.Value;

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    public object Evaluate(string script)
    {
      try
      {
        return ((ComBrowserHandler.IHTMLDocument2) this.Browser.Document).parentWindow.execScript(script, "JScript");
      }
      catch (Exception ex)
      {
        return (object) ex;
      }
    }

    public bool EnsureExplicitTrust(Uri uri, string urlPattern, bool otherwiseFail)
    {
      if (!this.RequireScript || this.TrustedUrlPatterns.Contains(urlPattern))
        return true;
      if (this.SecurityManager != null)
      {
        if ((!this.RequireScript ? 0 : (this.HasFullTrust(urlPattern, 5120U) ? 1 : 0)) != 0)
        {
          this.TrustedUrlPatterns.Add(urlPattern);
          return true;
        }
        uint pdwZone = 0;
        switch (this.SecurityManager.MapUrlToZone(urlPattern, ref pdwZone, 0U))
        {
          case -2147024809:
            return false;
          case 0:
            if (pdwZone != 2U && this.ConfirmExplicitTrust(uri, urlPattern))
            {
              this.SecurityManager.SetZoneMapping(2U, urlPattern, 0U);
              this.SecurityManager.SetZoneMapping(258U, urlPattern, 0U);
              if ((!this.RequireScript ? 0 : (this.HasFullTrust(urlPattern, 5120U) ? 1 : 0)) != 0)
              {
                this.TrustedUrlPatterns.Add(urlPattern);
                return true;
              }
              break;
            }
            break;
        }
      }
      if (otherwiseFail)
      {
        BrowserFlowException browserFlowException = new BrowserFlowException(BrowserFlowLayer.Client, new ErrorData()
        {
          Uri = uri,
          Details = urlPattern,
          StatusCode = -1
        }, ClientResources.BrowserScriptDisabled());
        browserFlowException.HelpLink = BrowserFlowException.FormatHelpLink(324081);
        this.Failed(this, new NavigationFailedEventArgs(uri, (Exception) browserFlowException));
      }
      return false;
    }

    private bool ConfirmExplicitTrust(Uri uri, string urlPattern)
    {
      if (this.confirmedExplicitTrust)
        return true;
      DependencyObject reference = (DependencyObject) this.Browser;
      do
      {
        reference = VisualTreeHelper.GetParent(reference);
      }
      while (!(reference is Window owner) && reference != null);
      if (MessageBox.Show(owner, ClientResources.ExplicitTrustRequired(), Process.GetCurrentProcess().MainWindowTitle, MessageBoxButton.OKCancel) != MessageBoxResult.OK)
      {
        BrowserFlowException browserFlowException = new BrowserFlowException(BrowserFlowLayer.Client, new ErrorData()
        {
          Uri = uri,
          Details = urlPattern,
          StatusCode = -1
        }, ClientResources.BrowserScriptDisabled());
        browserFlowException.HelpLink = BrowserFlowException.FormatHelpLink(324081);
        this.Failed(this, new NavigationFailedEventArgs(uri, (Exception) browserFlowException));
        return false;
      }
      this.confirmedExplicitTrust = true;
      return true;
    }

    private bool HasFullTrust(string urlPattern, uint urlAction)
    {
      IntPtr pPolicy = new IntPtr(3);
      return this.SecurityManager.ProcessUrlAction(urlPattern, urlAction, ref pPolicy, 4U, IntPtr.Zero, 0U, 1048577U, 0U) == 0 && pPolicy == new IntPtr(0);
    }

    public string GetElementInnerHtmlById(string id)
    {
      if (!(this.Browser.Document is ComBrowserHandler.IHTMLDocument3 document))
        return (string) null;
      return document.getElementById(id)?.innerHTML;
    }

    public void Initialize()
    {
      this.Browser.Navigated += new NavigatedEventHandler(this.OnNavigated);
      this.Browser.Navigating += new NavigatingCancelEventHandler(this.OnNavigating);
      this.Browser.LoadCompleted += new LoadCompletedEventHandler(this.OnInitialized);
      this.Browser.NavigateToString("<html/>");
      this.urnProtocolSession = new ComBrowserHandler.AsyncPluggableProtocolSession(typeof (ComBrowserHandler.CustomProtocolHandler), (object) this, "urn");
    }

    public void InjectStyle(string style) => this.Evaluate(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "var head = document.getElementsByTagName('head')[0]; var style = document.createElement('STYLE'); style.cssText = '{0}'; head.appendChild(style);", (object) style));

    public void InsertLocalStyleSheet(IEnumerable<PageHandlerStyle> styles)
    {
      if (!(this.Browser.Document is ComBrowserHandler.IHTMLDocument2 document))
        return;
      object styleSheet = document.createStyleSheet(string.Empty, 0);
      foreach (PageHandlerStyle style in styles)
      {
        try
        {
          // ISSUE: reference to a compiler-generated field
          if (ComBrowserHandler.\u003C\u003Eo__87.\u003C\u003Ep__0 == null)
          {
            // ISSUE: reference to a compiler-generated field
            ComBrowserHandler.\u003C\u003Eo__87.\u003C\u003Ep__0 = CallSite<Action<CallSite, object, string, string>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "addRule", (IEnumerable<Type>) null, typeof (ComBrowserHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          ComBrowserHandler.\u003C\u003Eo__87.\u003C\u003Ep__0.Target((CallSite) ComBrowserHandler.\u003C\u003Eo__87.\u003C\u003Ep__0, styleSheet, style.Selector, style.StyleRule);
        }
        catch (Exception ex)
        {
        }
      }
    }

    public bool IsActiveElementTextInput()
    {
      if (!(this.Browser.Document is ComBrowserHandler.IHTMLDocument2 document))
        return false;
      switch (document.activeElement)
      {
        case null:
          return false;
        case ComBrowserHandler.IHTMLTextAreaElement _:
          return true;
        case ComBrowserHandler.IHTMLInputElement htmlInputElement:
          if (!string.IsNullOrWhiteSpace(htmlInputElement.type))
          {
            string type = htmlInputElement.type;
            return StringComparer.OrdinalIgnoreCase.Compare(type, "text") == 0 || StringComparer.OrdinalIgnoreCase.Compare(type, "password") == 0 || StringComparer.OrdinalIgnoreCase.Compare(type, "number") == 0 || StringComparer.OrdinalIgnoreCase.Compare(type, "search") == 0 || StringComparer.OrdinalIgnoreCase.Compare(type, "email") == 0 || StringComparer.OrdinalIgnoreCase.Compare(type, "tel") == 0 || StringComparer.OrdinalIgnoreCase.Compare(type, "url") == 0;
          }
          break;
      }
      return false;
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (this.Browser != null)
        {
          this.Browser.LoadCompleted -= new LoadCompletedEventHandler(this.OnInitialized);
          this.Browser.LoadCompleted -= new LoadCompletedEventHandler(this.OnLoadCompleted);
          this.Browser.Navigating -= new NavigatingCancelEventHandler(this.OnNavigating);
          this.Browser.Navigated -= new NavigatedEventHandler(this.OnNavigated);
        }
        if (this.browserEvents != null)
        {
          this.browserEvents.Handler.NewWindow3Event -= new ComBrowserHandler.ComBrowserEvents.NewWindow3EventHandler(this.OnNewWindow);
          this.browserEvents.Dispose();
        }
        if (this.comBrowser != null)
          Marshal.ReleaseComObject((object) this.comBrowser);
        if (this.documentHostHandler != null)
        {
          this.documentHostHandler.ContextMenu -= new ComBrowserHandler.ComDocumentHostHandler.ContextMenuEventHandler(this.documentHostHandler_ContextMenu);
          this.documentHostHandler.External = (object) null;
        }
        if (this.securityManager != null && this.securityManager.IsValueCreated)
          Marshal.ReleaseComObject((object) this.securityManager.Value);
        if (this.trustedUrlPatterns != null && this.trustedUrlPatterns.IsValueCreated)
          this.trustedUrlPatterns.Value.Clear();
        if (this.urnProtocolSession != null)
          this.urnProtocolSession.Dispose();
      }
      this.Browser = (WebBrowser) null;
      this.browserEvents = (ComBrowserHandler.ComEventSink<ComBrowserHandler.DWebBrowserEvents2, ComBrowserHandler.ComBrowserEvents>) null;
      this.comBrowser = (ComBrowserHandler.IWebBrowser2) null;
      this.documentHostHandler = (ComBrowserHandler.ComDocumentHostHandler) null;
      this.securityManager = (Lazy<ComBrowserHandler.IInternetSecurityManager>) null;
      this.trustedUrlPatterns = (Lazy<HashSet<string>>) null;
      this.urnProtocolSession = (ComBrowserHandler.AsyncPluggableProtocolSession) null;
      this.External = (object) null;
    }

    protected virtual void OnInitialized(object sender, NavigationEventArgs e)
    {
      this.Browser.LoadCompleted -= new LoadCompletedEventHandler(this.OnInitialized);
      this.comBrowser = this.GetComWebBrowser();
      this.comBrowser.RegisterAsDropTarget = !this.BlockDragDrop;
      string urlPattern = "about:security_" + Process.GetCurrentProcess().MainModule.ModuleName;
      if (!this.EnsureExplicitTrust(e.Uri, urlPattern, true))
        return;
      this.browserEvents = new ComBrowserHandler.ComEventSink<ComBrowserHandler.DWebBrowserEvents2, ComBrowserHandler.ComBrowserEvents>(new ComBrowserHandler.ComBrowserEvents(), (object) this.comBrowser);
      this.browserEvents.Handler.NewWindow3Event += new ComBrowserHandler.ComBrowserEvents.NewWindow3EventHandler(this.OnNewWindow);
      this.documentHostHandler = new ComBrowserHandler.ComDocumentHostHandler(this.comBrowser.Document);
      this.documentHostHandler.ContextMenu += new ComBrowserHandler.ComDocumentHostHandler.ContextMenuEventHandler(this.documentHostHandler_ContextMenu);
      this.documentHostHandler.External = this.External;
      this.Browser.LoadCompleted += new LoadCompletedEventHandler(this.OnLoadCompleted);
      if (this.Initialized == null)
        return;
      this.Initialized((object) this, e);
    }

    protected virtual void OnLoadCompleted(object sender, NavigationEventArgs e)
    {
      if (this.LoadCompleted == null)
        return;
      this.LoadCompleted((object) this, e);
    }

    protected virtual void OnNavigated(object sender, NavigationEventArgs e)
    {
      if (this.comBrowser == null)
        return;
      if (this.Navigated != null)
        this.Navigated((object) this, e);
      if (this.comBrowser == null || !(this.comBrowser.Document is ComBrowserHandler.IOleCommandTarget document))
        return;
      IntPtr num1 = IntPtr.Zero;
      try
      {
        int num2 = (int) Math.Round(100.0 * DpiHelper.DeviceDpiX / 96.0);
        num1 = Marshal.AllocCoTaskMem(16);
        Marshal.GetNativeVariantForObject<int>(num2 - 1, num1);
        document.Exec(IntPtr.Zero, 63U, 2U, num1, IntPtr.Zero);
        Marshal.GetNativeVariantForObject<int>(num2, num1);
        document.Exec(IntPtr.Zero, 63U, 2U, num1, IntPtr.Zero);
      }
      catch (Exception ex)
      {
      }
      finally
      {
        if (num1 != IntPtr.Zero)
          Marshal.FreeCoTaskMem(num1);
      }
    }

    protected virtual void OnNavigating(object sender, NavigatingCancelEventArgs e)
    {
      if (!(e.Uri != (Uri) null))
        return;
      if (e.Uri.Scheme != Uri.UriSchemeHttp && e.Uri.Scheme != Uri.UriSchemeHttps)
      {
        if (this.CustomNavigation == null)
          return;
        this.CustomNavigation(this, new CustomNavigationEventArgs(e.Uri));
      }
      else
      {
        if (this.Navigating != null)
          this.Navigating((object) this, e);
        if (e.Cancel)
          return;
        this.EnsureExplicitTrust(e.Uri, e.Uri.GetLeftPart(UriPartial.Authority), true);
      }
    }

    protected virtual void OnCustomNavigation(string uriValue)
    {
      Uri result = (Uri) null;
      if (this.CustomNavigation == null || !Uri.TryCreate(uriValue, UriKind.Absolute, out result))
        return;
      this.CustomNavigation(this, new CustomNavigationEventArgs(result));
    }

    protected virtual void OnNewWindow(
      ref object ppDisp,
      ref bool Cancel,
      uint dwFlags,
      string bstrUrlContext,
      string bstrUrl)
    {
      Uri result;
      if (Uri.TryCreate(bstrUrl, UriKind.Absolute, out result))
        Cancel = !this.TrustedUrlPatterns.Contains(result.GetLeftPart(UriPartial.Authority));
      else
        Cancel = true;
    }

    private ComBrowserHandler.IWebBrowser2 GetComWebBrowser() => this.Browser.Document is ComBrowserHandler.IServiceProvider document ? document.QueryService(ComBrowserHandler.SID_WebBrowser, typeof (ComBrowserHandler.IWebBrowser2).GUID) as ComBrowserHandler.IWebBrowser2 : (ComBrowserHandler.IWebBrowser2) null;

    private object GetHtmlElementFromPoint(Point point) => (object) null;

    private void documentHostHandler_ContextMenu(
      ComBrowserHandler.ComDocumentHostHandler sender,
      ComBrowserHandler.ComDocumentHostHandler.ContextMenuEventArgs e)
    {
      if (this.ContextMenu == null)
        return;
      Point point = this.Browser.PointFromScreen(e.Point);
      this.ContextMenu(this, new HtmlContextMenuEventArgs(point, this.GetHtmlElementFromPoint(point)));
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("00000001-0000-0000-C000-000000000046")]
    [ComImport]
    private interface IClassFactory
    {
      [MethodImpl(MethodImplOptions.PreserveSig)]
      uint CreateInstance([MarshalAs(UnmanagedType.IUnknown), In] object pUnkOuter, [In] ref Guid riid, out IntPtr ppvObject);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      uint LockServer([MarshalAs(UnmanagedType.Bool), In] bool fLock);
    }

    [Guid("79EAC9E1-BAF9-11CE-8C82-00AA004BA90B")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    private interface IInternetBindInfo
    {
      [MethodImpl(MethodImplOptions.PreserveSig)]
      uint GetBindInfo(out uint grfBINDF, [In, Out] ref ComBrowserHandler.BINDINFO pbindinfo);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      uint GetBindString([In] uint ulStringType, [MarshalAs(UnmanagedType.LPWStr), In, Out] ref string ppwzStr, [In] uint cEl, [In, Out] ref uint pcElFetched);
    }

    [Guid("79EAC9E4-BAF9-11CE-8C82-00AA004BA90B")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    private interface IInternetProtocol
    {
      [MethodImpl(MethodImplOptions.PreserveSig)]
      uint Start(
        [MarshalAs(UnmanagedType.LPWStr), In] string szUrl,
        [In] ComBrowserHandler.IInternetProtocolSink pOIProtSink,
        [In] ComBrowserHandler.IInternetBindInfo pOIBindInfo,
        [In] uint grfPI,
        [In] IntPtr dwReserved);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      uint Continue([In] ref ComBrowserHandler.PROTOCOLDATA pProtocolData);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      uint Abort([In] uint hrReason, [In] uint dwOptions);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      uint Terminate([In] uint dwOptions);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      uint Suspend();

      [MethodImpl(MethodImplOptions.PreserveSig)]
      uint Resume();

      [MethodImpl(MethodImplOptions.PreserveSig)]
      uint Read([In, Out] IntPtr pv, [In] uint cb, out uint pcbRead);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      uint Seek([In] long dlibMove, [In] uint dwOrigin, out ulong plibNewPosition);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      uint LockRequest([In] uint dwOptions);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      uint UnlockRequest();
    }

    [Guid("79EAC9EC-BAF9-11CE-8C82-00AA004BA90B")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    private interface IInternetProtocolInfo
    {
      [MethodImpl(MethodImplOptions.PreserveSig)]
      uint ParseUrl(
        [MarshalAs(UnmanagedType.LPWStr), In] string pwzUrl,
        [MarshalAs(UnmanagedType.I4), In] uint parseAction,
        [In] uint dwParseFlags,
        [MarshalAs(UnmanagedType.LPWStr)] out string pwzResult,
        [In] uint cchResult,
        out uint pcchResult,
        [In] uint dwReserved);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      uint CombineUrl(
        [MarshalAs(UnmanagedType.LPWStr), In] string pwzBaseUrl,
        [MarshalAs(UnmanagedType.LPWStr), In] string pwzRelativeUrl,
        [In] uint dwCombineFlags,
        [MarshalAs(UnmanagedType.LPWStr)] out string pwzResult,
        [In] uint cchResult,
        out uint pcchResult,
        [In] uint dwReserved);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      uint CompareUrl([MarshalAs(UnmanagedType.LPWStr), In] string pwzUrl1, [MarshalAs(UnmanagedType.LPWStr), In] string pwzUrl2, [MarshalAs(UnmanagedType.I4), In] bool dwCompareFlags);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      uint QueryInfo(
        [MarshalAs(UnmanagedType.LPWStr), In] string pwzUrl,
        [MarshalAs(UnmanagedType.I4), In] ComBrowserHandler.QUERYOPTION queryOption,
        [In] uint dwQueryFlags,
        [In, Out] ref IntPtr pBuffer,
        [In] uint cbBuffer,
        [In, Out] ref uint pcbBuf,
        [In] uint dwReserved);
    }

    [Guid("79EAC9E5-BAF9-11CE-8C82-00AA004BA90B")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    private interface IInternetProtocolSink
    {
      [MethodImpl(MethodImplOptions.PreserveSig)]
      uint Switch([In] ComBrowserHandler.PROTOCOLDATA pProtocolData);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      uint ReportProgress([In] uint ulStatusCode, [MarshalAs(UnmanagedType.LPWStr), In] string szStatusText);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      uint ReportData([MarshalAs(UnmanagedType.I4), In] ComBrowserHandler.BSCF grfBSCF, [In] uint ulProgress, [In] uint ulProgressMax);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      uint ReportResult([In] uint hrResult, [In] uint dwError, [MarshalAs(UnmanagedType.LPWStr), In] string szResult);
    }

    [ComVisible(true)]
    [Guid("79EAC9E7-BAF9-11CE-8C82-00AA004BA90B")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IInternetSession
    {
      [MethodImpl(MethodImplOptions.PreserveSig)]
      int RegisterNameSpace(
        [In] ComBrowserHandler.IClassFactory classFactory,
        [In] ref Guid rclsid,
        [MarshalAs(UnmanagedType.LPWStr), In] string pwzProtocol,
        [In] uint cPatterns,
        [MarshalAs(UnmanagedType.LPWStr), In] string ppwzPatterns,
        [In] uint dwReserved);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int UnregisterNameSpace([In] ComBrowserHandler.IClassFactory classFactory, [MarshalAs(UnmanagedType.LPWStr), In] string pszProtocol);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int RegisterMimeFilter(
        [In] ComBrowserHandler.IClassFactory classFactory,
        [In] ref Guid rclsid,
        [MarshalAs(UnmanagedType.LPWStr), In] string pwzType);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int UnregisterMimeFilter([In] ComBrowserHandler.IClassFactory classFactory, [MarshalAs(UnmanagedType.LPWStr), In] string pwzType);

      int CreateBindingPlaceholder();

      int SetSessionOptionPlaceholder();

      int GetSessionOptionPlaceholder();
    }

    private enum BSCF
    {
      BSCF_FIRSTDATANOTIFICATION = 1,
      BSCF_INTERMEDIATEDATANOTIFICATION = 2,
      BSCF_LASTDATANOTIFICATION = 4,
      BSCF_DATAFULLYAVAILABLE = 8,
      BSCF_AVAILABLEDATASIZEUNKNOWN = 16, // 0x00000010
    }

    private struct BINDINFO
    {
      [MarshalAs(UnmanagedType.U4)]
      public uint cbSize;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string szExtraInfo;
      [MarshalAs(UnmanagedType.Struct)]
      public STGMEDIUM stgmedData;
      [MarshalAs(UnmanagedType.U4)]
      public uint grfBindInfoF;
      [MarshalAs(UnmanagedType.U4)]
      public uint dwBindVerb;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string szCustomVerb;
      [MarshalAs(UnmanagedType.U4)]
      public uint cbstgmedData;
      [MarshalAs(UnmanagedType.U4)]
      public uint dwOptions;
      [MarshalAs(UnmanagedType.U4)]
      public uint dwOptionsFlags;
      [MarshalAs(UnmanagedType.U4)]
      public uint dwCodePage;
      [MarshalAs(UnmanagedType.Struct)]
      public ComBrowserHandler.SECURITY_ATTRIBUTES securityAttributes;
      public Guid iid;
      [MarshalAs(UnmanagedType.IUnknown)]
      public object punk;
      [MarshalAs(UnmanagedType.U4)]
      public uint dwReserved;
    }

    private struct PROTOCOLDATA
    {
      public uint grfFlags;
      public uint dwState;
      public IntPtr pData;
      public uint cbData;
    }

    private enum QUERYOPTION
    {
      QUERY_EXPIRATION_DATE = 1,
      QUERY_TIME_OF_LAST_CHANGE = 2,
      QUERY_CONTENT_ENCODING = 3,
      QUERY_CONTENT_TYPE = 4,
      QUERY_REFRESH = 5,
      QUERY_RECOMBINE = 6,
      QUERY_CAN_NAVIGATE = 7,
      QUERY_USES_NETWORK = 8,
      QUERY_IS_CACHED = 9,
      QUERY_IS_INSTALLEDENTRY = 10, // 0x0000000A
      QUERY_IS_CACHED_OR_MAPPED = 11, // 0x0000000B
      QUERY_USES_CACHE = 12, // 0x0000000C
      QUERY_IS_SECURE = 13, // 0x0000000D
      QUERY_IS_SAFE = 14, // 0x0000000E
      QUERY_USES_HISTORYFOLDER = 15, // 0x0000000F
    }

    private struct SECURITY_ATTRIBUTES
    {
      [MarshalAs(UnmanagedType.U4)]
      public uint nLength;
      public IntPtr lpSecurityDescriptor;
      [MarshalAs(UnmanagedType.Bool)]
      public bool bInheritHandle;
    }

    private class AsyncPluggableProtocolSession : IDisposable
    {
      private ComBrowserHandler.AsyncProtocolClassFactory classFactory;
      private ComBrowserHandler.IInternetSession internetSession;
      private string protocol;

      public AsyncPluggableProtocolSession(Type handlerType, object state, string protocol)
      {
        this.classFactory = new ComBrowserHandler.AsyncProtocolClassFactory()
        {
          HandlerType = handlerType,
          State = state
        };
        ComBrowserHandler.CoInternetGetSession(0U, out this.internetSession, 0U);
        Guid rclsid = Guid.NewGuid();
        this.internetSession.RegisterNameSpace((ComBrowserHandler.IClassFactory) this.classFactory, ref rclsid, protocol, 0U, (string) null, 0U);
        this.protocol = protocol;
      }

      public void Dispose()
      {
        this.Dispose(true);
        GC.SuppressFinalize((object) this);
      }

      private void Dispose(bool disposing)
      {
        if (disposing)
        {
          if (this.classFactory != null && this.internetSession != null)
            this.internetSession.UnregisterNameSpace((ComBrowserHandler.IClassFactory) this.classFactory, this.protocol);
          this.classFactory?.Dispose();
        }
        this.classFactory = (ComBrowserHandler.AsyncProtocolClassFactory) null;
        this.internetSession = (ComBrowserHandler.IInternetSession) null;
        this.protocol = (string) null;
      }
    }

    [ComVisible(true)]
    [Guid("22073506-EE2F-4B76-9CA4-B99D765BB014")]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    internal class AsyncProtocolClassFactory : ComBrowserHandler.IClassFactory, IDisposable
    {
      public Type HandlerType { get; set; }

      public object State { get; set; }

      public uint CreateInstance(object pUnkOuter, ref Guid riid, out IntPtr ppvObject)
      {
        ppvObject = IntPtr.Zero;
        if (pUnkOuter != null)
          return 2147746064;
        if (riid == typeof (ComBrowserHandler.IInternetProtocol).GUID)
        {
          object instance = Activator.CreateInstance(this.HandlerType, this.State);
          ppvObject = Marshal.GetComInterfaceForObject(instance, typeof (ComBrowserHandler.IInternetProtocol));
        }
        else
        {
          if (!(riid == typeof (ComBrowserHandler.IInternetProtocolInfo).GUID))
            return 2147500034;
          object instance = Activator.CreateInstance(this.HandlerType, this.State);
          ppvObject = Marshal.GetComInterfaceForObject(instance, typeof (ComBrowserHandler.IInternetProtocolInfo));
        }
        return 0;
      }

      public uint LockServer(bool fLock) => 0;

      public void Dispose()
      {
        this.Dispose(true);
        GC.SuppressFinalize((object) this);
      }

      private void Dispose(bool disposing)
      {
        int num = disposing ? 1 : 0;
        this.HandlerType = (Type) null;
        this.State = (object) null;
      }
    }

    private class CustomProtocolHandler : 
      ComBrowserHandler.IInternetProtocol,
      ComBrowserHandler.IInternetProtocolInfo
    {
      public CustomProtocolHandler(object state) => this.BrowserHandler = state as ComBrowserHandler;

      public ComBrowserHandler BrowserHandler { get; private set; }

      public uint Read(IntPtr pv, uint cb, out uint pcbRead)
      {
        pcbRead = 0U;
        return 0;
      }

      public uint Seek(long dlibMove, uint dwOrigin, out ulong plibNewPosition)
      {
        plibNewPosition = 0UL;
        return 2147500037;
      }

      public uint LockRequest(uint dwOptions) => 0;

      public uint UnlockRequest() => 0;

      public uint Start(
        string szURL,
        ComBrowserHandler.IInternetProtocolSink sink,
        ComBrowserHandler.IInternetBindInfo pOIBindInfo,
        uint grfPI,
        IntPtr dwReserved)
      {
        if (this.BrowserHandler != null)
          this.BrowserHandler.OnCustomNavigation(szURL);
        return 0;
      }

      public uint Continue(ref ComBrowserHandler.PROTOCOLDATA pProtocolData) => 0;

      public uint Abort(uint hrReason, uint dwOptions) => 0;

      public uint Terminate(uint dwOptions) => 0;

      public uint Suspend() => 0;

      public uint Resume() => 0;

      public uint ParseUrl(
        string pwzUrl,
        uint parseAction,
        uint dwParseFlags,
        out string pwzResult,
        uint cchResult,
        out uint pcchResult,
        uint dwReserved)
      {
        pwzResult = (string) null;
        pcchResult = 0U;
        return 2148270097;
      }

      public uint CombineUrl(
        string pwzBaseUrl,
        string pwzRelativeUrl,
        uint dwCombineFlags,
        out string pwzResult,
        uint cchResult,
        out uint pcchResult,
        uint dwReserved)
      {
        pwzResult = (string) null;
        pcchResult = 0U;
        return 2148270097;
      }

      public uint CompareUrl(string pwzUrl1, string pwzUrl2, bool dwCompareFlags) => (uint) string.Compare(pwzUrl1, pwzUrl2, StringComparison.OrdinalIgnoreCase);

      public uint QueryInfo(
        string pwzUrl,
        ComBrowserHandler.QUERYOPTION queryOption,
        uint dwQueryFlags,
        ref IntPtr pBuffer,
        uint cbBuffer,
        ref uint pcbBuf,
        uint dwReserved)
      {
        return queryOption == ComBrowserHandler.QUERYOPTION.QUERY_USES_CACHE ? 1U : 2148270097U;
      }
    }

    [Guid("34A715A0-6587-11D0-924A-0020AFC7AC4D")]
    [SuppressUnmanagedCodeSecurity]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [ComImport]
    private interface DWebBrowserEvents2
    {
      [DispId(102)]
      void StatusTextChange([MarshalAs(UnmanagedType.BStr)] string Text);

      [DispId(108)]
      void ProgressChange(int Progress, int ProgressMax);

      [DispId(105)]
      void CommandStateChange(int Command, [MarshalAs(UnmanagedType.VariantBool)] bool Enable);

      [DispId(106)]
      void DownloadBegin();

      [DispId(104)]
      void DownloadComplete();

      [DispId(113)]
      void TitleChange([MarshalAs(UnmanagedType.BStr)] string Text);

      [DispId(112)]
      void PropertyChange([MarshalAs(UnmanagedType.BStr)] string szProperty);

      [DispId(250)]
      void BeforeNavigate2(
        [MarshalAs(UnmanagedType.IDispatch)] object pDisp,
        [In] ref object URL,
        [In] ref object Flags,
        [In] ref object TargetFrameName,
        [In] ref object PostData,
        [In] ref object Headers,
        [MarshalAs(UnmanagedType.VariantBool), In, Out] ref bool Cancel);

      [DispId(251)]
      void NewWindow2([MarshalAs(UnmanagedType.IDispatch), In, Out] ref object ppDisp, [MarshalAs(UnmanagedType.VariantBool), In, Out] ref bool Cancel);

      [DispId(252)]
      void NavigateComplete2([MarshalAs(UnmanagedType.IDispatch)] object pDisp, [In] ref object URL);

      [DispId(259)]
      void DocumentComplete([MarshalAs(UnmanagedType.IDispatch)] object pDisp, [In] ref object URL);

      [DispId(253)]
      void OnQuit();

      [DispId(254)]
      void OnVisible([MarshalAs(UnmanagedType.VariantBool)] bool Visible);

      [DispId(255)]
      void OnToolBar([MarshalAs(UnmanagedType.VariantBool)] bool ToolBar);

      [DispId(256)]
      void OnMenuBar([MarshalAs(UnmanagedType.VariantBool)] bool MenuBar);

      [DispId(257)]
      void OnStatusBar([MarshalAs(UnmanagedType.VariantBool)] bool StatusBar);

      [DispId(258)]
      void OnFullScreen([MarshalAs(UnmanagedType.VariantBool)] bool FullScreen);

      [DispId(260)]
      void OnTheaterMode([MarshalAs(UnmanagedType.VariantBool)] bool TheaterMode);

      [DispId(262)]
      void WindowSetResizable([MarshalAs(UnmanagedType.VariantBool)] bool Resizable);

      [DispId(264)]
      void WindowSetLeft(int Left);

      [DispId(265)]
      void WindowSetTop(int Top);

      [DispId(266)]
      void WindowSetWidth(int Width);

      [DispId(267)]
      void WindowSetHeight(int Height);

      [DispId(263)]
      void WindowClosing([MarshalAs(UnmanagedType.VariantBool)] bool IsChildWindow, [MarshalAs(UnmanagedType.VariantBool), In, Out] ref bool Cancel);

      [DispId(268)]
      void ClientToHostWindow([In, Out] ref int CX, [In, Out] ref int CY);

      [DispId(269)]
      void SetSecureLockIcon(int SecureLockIcon);

      [DispId(270)]
      void FileDownload([MarshalAs(UnmanagedType.VariantBool), Out] bool ActiveDocument, [MarshalAs(UnmanagedType.VariantBool), In, Out] ref bool Cancel);

      [DispId(271)]
      void NavigateError(
        [MarshalAs(UnmanagedType.IDispatch)] object pDisp,
        [In] ref object URL,
        [In] ref object Frame,
        [In] ref object StatusCode,
        [MarshalAs(UnmanagedType.VariantBool), In, Out] ref bool Cancel);

      [DispId(225)]
      void PrintTemplateInstantiation([MarshalAs(UnmanagedType.IDispatch)] object pDisp);

      [DispId(226)]
      void PrintTemplateTeardown([MarshalAs(UnmanagedType.IDispatch)] object pDisp);

      [DispId(227)]
      void UpdatePageStatus([MarshalAs(UnmanagedType.IDispatch)] object pDisp, [In] ref object nPage, [In] ref object fDone);

      [DispId(272)]
      void PrivacyImpactedStateChange([MarshalAs(UnmanagedType.VariantBool)] bool bImpacted);

      [DispId(273)]
      void NewWindow3(
        [MarshalAs(UnmanagedType.IDispatch), In, Out] ref object ppDisp,
        [MarshalAs(UnmanagedType.VariantBool), In, Out] ref bool Cancel,
        uint dwFlags,
        [MarshalAs(UnmanagedType.BStr)] string bstrUrlContext,
        [MarshalAs(UnmanagedType.BStr)] string bstrUrl);
    }

    [Guid("3050F3F0-98B5-11CF-BB82-00AA00BDCE0B")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    private interface ICustomDoc
    {
      void SetUIHandler(ComBrowserHandler.IDocHostUIHandler pUIHandler);
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("BD3F23C0-D43E-11CF-893B-00AA00BDCE1A")]
    [ComImport]
    private interface IDocHostUIHandler
    {
      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int ShowContextMenu(
        [MarshalAs(UnmanagedType.U4), In] uint dwID,
        [MarshalAs(UnmanagedType.Struct), In] ref ComBrowserHandler.POINT pt,
        [MarshalAs(UnmanagedType.IUnknown), In] object pcmdtReserved,
        [MarshalAs(UnmanagedType.IDispatch), In] object pdispReserved);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int GetHostInfo([MarshalAs(UnmanagedType.Struct), In, Out] ref ComBrowserHandler.DOCHOSTUIINFO info);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int ShowUI(
        [MarshalAs(UnmanagedType.I4), In] int dwID,
        [MarshalAs(UnmanagedType.Interface), In] ComBrowserHandler.IOleInPlaceActiveObject activeObject,
        [MarshalAs(UnmanagedType.Interface), In] ComBrowserHandler.IOleCommandTarget commandTarget,
        [MarshalAs(UnmanagedType.Interface), In] ComBrowserHandler.IOleInPlaceFrame frame,
        [MarshalAs(UnmanagedType.Interface), In] ComBrowserHandler.IOleInPlaceUIWindow doc);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int HideUI();

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int UpdateUI();

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int EnableModeless([MarshalAs(UnmanagedType.Bool), In] bool fEnable);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int OnDocWindowActivate([MarshalAs(UnmanagedType.Bool), In] bool fActivate);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int OnFrameWindowActivate([MarshalAs(UnmanagedType.Bool), In] bool fActivate);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int ResizeBorder(
        [MarshalAs(UnmanagedType.Struct), In] ref ComBrowserHandler.RECT rect,
        [MarshalAs(UnmanagedType.Interface), In] ComBrowserHandler.IOleInPlaceUIWindow doc,
        [MarshalAs(UnmanagedType.Bool), In] bool fFrameWindow);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int TranslateAccelerator([MarshalAs(UnmanagedType.Struct), In] ref ComBrowserHandler.MSG msg, [In] ref Guid group, [MarshalAs(UnmanagedType.U4), In] uint nCmdID);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int GetOptionKeyPath([MarshalAs(UnmanagedType.LPWStr)] out string pbstrKey, [MarshalAs(UnmanagedType.U4), In] uint dw);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int GetDropTarget(
        [MarshalAs(UnmanagedType.Interface), In] ComBrowserHandler.IDropTarget pDropTarget,
        [MarshalAs(UnmanagedType.Interface)] out ComBrowserHandler.IDropTarget ppDropTarget);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int GetExternal([MarshalAs(UnmanagedType.IDispatch)] out object ppDispatch);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int TranslateUrl([MarshalAs(UnmanagedType.U4), In] uint dwTranslate, [MarshalAs(UnmanagedType.LPWStr), In] string strURLIn, [MarshalAs(UnmanagedType.LPWStr)] out string pstrURLOut);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int FilterDataObject([MarshalAs(UnmanagedType.Interface), In] System.Runtime.InteropServices.ComTypes.IDataObject pDO, [MarshalAs(UnmanagedType.Interface)] out System.Runtime.InteropServices.ComTypes.IDataObject ppDORet);
    }

    [Guid("00000122-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    private interface IDropTarget
    {
      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int DragEnter(
        [MarshalAs(UnmanagedType.Interface), In] System.Runtime.InteropServices.ComTypes.IDataObject pDataObj,
        [MarshalAs(UnmanagedType.U4), In] uint grfKeyState,
        [MarshalAs(UnmanagedType.Struct), In] ComBrowserHandler.POINT pt,
        [MarshalAs(UnmanagedType.U4), In, Out] ref uint pdwEffect);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int DragOver([MarshalAs(UnmanagedType.U4), In] uint grfKeyState, [MarshalAs(UnmanagedType.Struct), In] ComBrowserHandler.POINT pt, [MarshalAs(UnmanagedType.U4), In, Out] ref uint pdwEffect);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int DragLeave();

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int Drop(
        [MarshalAs(UnmanagedType.Interface), In] System.Runtime.InteropServices.ComTypes.IDataObject pDataObj,
        [MarshalAs(UnmanagedType.U4), In] uint grfKeyState,
        [MarshalAs(UnmanagedType.Struct), In] ComBrowserHandler.POINT pt,
        [MarshalAs(UnmanagedType.U4), In, Out] ref uint pdwEffect);
    }

    [Guid("332C4425-26CB-11D0-B483-00C04FD90119")]
    [TypeLibType(4160)]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [ComImport]
    private interface IHTMLDocument2
    {
      [DispId(1034)]
      ComBrowserHandler.IHTMLWindow2 parentWindow { [return: MarshalAs(UnmanagedType.Interface)] get; }

      [DispId(1071)]
      [return: MarshalAs(UnmanagedType.Interface)]
      object createStyleSheet([MarshalAs(UnmanagedType.BStr)] string bstrHref, int lIndex);

      [DispId(1025)]
      string url { [return: MarshalAs(UnmanagedType.BStr)] get; set; }

      [DispId(1004)]
      ComBrowserHandler.IHTMLElement body { get; }

      [DispId(1005)]
      ComBrowserHandler.IHTMLElement activeElement { get; }
    }

    [Guid("3050F485-98B5-11CF-BB82-00AA00BDCE0B")]
    [TypeLibType(4160)]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [ComImport]
    private interface IHTMLDocument3
    {
      [DispId(1034)]
      ComBrowserHandler.IHTMLWindow2 parentWindow { [return: MarshalAs(UnmanagedType.Interface)] get; }

      [DispId(1088)]
      ComBrowserHandler.IHTMLElement getElementById([MarshalAs(UnmanagedType.BStr)] string v);

      [DispId(1075)]
      ComBrowserHandler.IHTMLElement documentElement { get; }
    }

    [Guid("3050F1FF-98B5-11CF-BB82-00AA00BDCE0B")]
    [TypeLibType(4160)]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [ComImport]
    private interface IHTMLElement
    {
      [DispId(-2147417086)]
      string innerHTML { [return: MarshalAs(UnmanagedType.BStr)] get; set; }
    }

    [Guid("3050F2AA-98B5-11CF-BB82-00AA00BDCE0B")]
    [TypeLibType(TypeLibTypeFlags.FDispatchable)]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [ComImport]
    public interface IHTMLTextAreaElement
    {
    }

    [Guid("3050F5D2-98B5-11CF-BB82-00AA00BDCE0B")]
    [TypeLibType(TypeLibTypeFlags.FDispatchable)]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [ComImport]
    public interface IHTMLInputElement
    {
      [DispId(2000)]
      string type { [return: MarshalAs(UnmanagedType.BStr)] get; set; }
    }

    [Guid("332C4427-26CB-11D0-B483-00C04FD90119")]
    [TypeLibType(4160)]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [ComImport]
    private interface IHTMLWindow2
    {
      [DispId(1165)]
      object execScript([In] string code, [In] string language);
    }

    [ComVisible(false)]
    [Guid("79EAC9EE-BAF9-11CE-8C82-00AA004BA90B")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    internal interface IInternetSecurityManager
    {
      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int SetSecuritySite([In] IntPtr pSite);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int GetSecuritySite([Out] IntPtr pSite);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int MapUrlToZone([MarshalAs(UnmanagedType.LPWStr), In] string pwszUrl, ref uint pdwZone, uint dwFlags);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int GetSecurityId(
        [MarshalAs(UnmanagedType.LPWStr)] string pwszUrl,
        IntPtr pbSecurityId,
        ref uint pcbSecurityId,
        UIntPtr dwReserved);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int ProcessUrlAction(
        [MarshalAs(UnmanagedType.LPWStr), In] string pwszUrl,
        uint dwAction,
        ref IntPtr pPolicy,
        uint cbPolicy,
        IntPtr pContext,
        uint cbContext,
        uint dwFlags,
        uint dwReserved);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int QueryCustomPolicy(
        [MarshalAs(UnmanagedType.LPWStr), In] string pwszUrl,
        ref Guid guidKey,
        IntPtr ppPolicy,
        ref uint pcbPolicy,
        IntPtr pContext,
        uint cbContext,
        uint dwReserved);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int SetZoneMapping(uint dwZone, [MarshalAs(UnmanagedType.LPWStr), In] string lpszPattern, uint dwFlags);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int GetZoneMappings(uint dwZone, out IEnumString ppenumString, uint dwFlags);
    }

    [Guid("B722BCCB-4E68-101B-A2BC-00AA00404770")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    private interface IOleCommandTarget
    {
      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int QueryStatus(
        [In] IntPtr pguidCmdGroup,
        [MarshalAs(UnmanagedType.U4), In] uint cCmds,
        [MarshalAs(UnmanagedType.Struct), In, Out] ref ComBrowserHandler.OLECMD prgCmds,
        [In, Out] IntPtr pCmdText);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int Exec([In] IntPtr pguidCmdGroup, [MarshalAs(UnmanagedType.U4), In] uint nCmdID, [MarshalAs(UnmanagedType.U4), In] uint nCmdexecopt, [In] IntPtr pvaIn, [In, Out] IntPtr pvaOut);
    }

    [Guid("00000117-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    private interface IOleInPlaceActiveObject
    {
      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int GetWindow([In, Out] ref IntPtr phwnd);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int ContextSensitiveHelp([MarshalAs(UnmanagedType.Bool), In] bool fEnterMode);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int TranslateAccelerator([MarshalAs(UnmanagedType.Struct), In] ref ComBrowserHandler.MSG lpmsg);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int OnFrameWindowActivate([MarshalAs(UnmanagedType.Bool), In] bool fActivate);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int OnDocWindowActivate([MarshalAs(UnmanagedType.Bool), In] bool fActivate);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int ResizeBorder(
        [MarshalAs(UnmanagedType.Struct), In] ref ComBrowserHandler.RECT prcBorder,
        [MarshalAs(UnmanagedType.Interface), In] ref ComBrowserHandler.IOleInPlaceUIWindow pUIWindow,
        [MarshalAs(UnmanagedType.Bool), In] bool fFrameWindow);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int EnableModeless([MarshalAs(UnmanagedType.Bool), In] bool fEnable);
    }

    [Guid("00000116-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    private interface IOleInPlaceFrame
    {
      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int GetWindow([In, Out] ref IntPtr phwnd);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int ContextSensitiveHelp([MarshalAs(UnmanagedType.Bool), In] bool fEnterMode);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int GetBorder([MarshalAs(UnmanagedType.LPStruct), Out] ComBrowserHandler.RECT lprectBorder);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int RequestBorderSpace([MarshalAs(UnmanagedType.Struct), In] ref ComBrowserHandler.RECT pborderwidths);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int SetBorderSpace([MarshalAs(UnmanagedType.Struct), In] ref ComBrowserHandler.RECT pborderwidths);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int SetActiveObject(
        [MarshalAs(UnmanagedType.Interface), In] ref ComBrowserHandler.IOleInPlaceActiveObject pActiveObject,
        [MarshalAs(UnmanagedType.LPWStr), In] string pszObjName);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int InsertMenus([In] IntPtr hmenuShared, [MarshalAs(UnmanagedType.Struct), In, Out] ref object lpMenuWidths);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int SetMenu([In] IntPtr hmenuShared, [In] IntPtr holemenu, [In] IntPtr hwndActiveObject);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int RemoveMenus([In] IntPtr hmenuShared);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int SetStatusText([MarshalAs(UnmanagedType.LPWStr), In] string pszStatusText);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int EnableModeless([MarshalAs(UnmanagedType.Bool), In] bool fEnable);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int TranslateAccelerator([MarshalAs(UnmanagedType.Struct), In] ref ComBrowserHandler.MSG lpmsg, [MarshalAs(UnmanagedType.U2), In] short wID);
    }

    [Guid("00000115-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    private interface IOleInPlaceUIWindow
    {
      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int GetWindow([In, Out] ref IntPtr phwnd);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int ContextSensitiveHelp([MarshalAs(UnmanagedType.Bool), In] bool fEnterMode);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int GetBorder([MarshalAs(UnmanagedType.Struct), In, Out] ref ComBrowserHandler.RECT lprectBorder);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int RequestBorderSpace([MarshalAs(UnmanagedType.Struct), In] ref ComBrowserHandler.RECT pborderwidths);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int SetBorderSpace([MarshalAs(UnmanagedType.Struct), In] ref ComBrowserHandler.RECT pborderwidths);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      [return: MarshalAs(UnmanagedType.I4)]
      int SetActiveObject(
        [MarshalAs(UnmanagedType.Interface), In] ref ComBrowserHandler.IOleInPlaceActiveObject pActiveObject,
        [MarshalAs(UnmanagedType.LPWStr), In] string pszObjName);
    }

    [Guid("6D5140C1-7436-11CE-8034-00AA006009FA")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    private interface IServiceProvider
    {
      [return: MarshalAs(UnmanagedType.IUnknown)]
      object QueryService([MarshalAs(UnmanagedType.LPStruct), In] Guid guidService, [MarshalAs(UnmanagedType.LPStruct), In] Guid riid);
    }

    [Guid("D30C1661-CDAF-11D0-8A3E-00C04FC9E26E")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [SuppressUnmanagedCodeSecurity]
    [ComImport]
    private interface IWebBrowser2
    {
      [DispId(203)]
      object Document { [return: MarshalAs(UnmanagedType.IDispatch)] get; }

      [DispId(553)]
      bool RegisterAsDropTarget { [return: MarshalAs(UnmanagedType.VariantBool)] get; set; }

      [DispId(551)]
      bool Silent { [return: MarshalAs(UnmanagedType.VariantBool)] get; set; }
    }

    private struct DOCHOSTUIINFO
    {
      [MarshalAs(UnmanagedType.U4)]
      public uint cbSize;
      [MarshalAs(UnmanagedType.U4)]
      public uint dwFlags;
      [MarshalAs(UnmanagedType.U4)]
      public uint dwDoubleClick;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string pchHostCss;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string pchHostNS;
    }

    private struct MSG
    {
      public IntPtr hwnd;
      [MarshalAs(UnmanagedType.I4)]
      public int message;
      public IntPtr wParam;
      public IntPtr lParam;
      [MarshalAs(UnmanagedType.I4)]
      public int time;
      [MarshalAs(UnmanagedType.I4)]
      public int pt_x;
      [MarshalAs(UnmanagedType.I4)]
      public int pt_y;
    }

    private struct OLECMD
    {
      [MarshalAs(UnmanagedType.U4)]
      public uint cmdID;
      [MarshalAs(UnmanagedType.U4)]
      public uint cmdf;
    }

    private struct POINT
    {
      [MarshalAs(UnmanagedType.I4)]
      public int X;
      [MarshalAs(UnmanagedType.I4)]
      public int Y;
    }

    private struct RECT
    {
      [MarshalAs(UnmanagedType.I4)]
      public int Left;
      [MarshalAs(UnmanagedType.I4)]
      public int Top;
      [MarshalAs(UnmanagedType.I4)]
      public int Right;
      [MarshalAs(UnmanagedType.I4)]
      public int Bottom;
    }

    private class ComBrowserEvents : ComBrowserHandler.DWebBrowserEvents2
    {
      public event ComBrowserHandler.ComBrowserEvents.StatusTextChangeEventHandler StatusTextChangeEvent;

      public event ComBrowserHandler.ComBrowserEvents.ProgressChangeEventHandler ProgressChangeEvent;

      public event ComBrowserHandler.ComBrowserEvents.CommandStateChangeEventHandler CommandStateChangeEvent;

      public event ComBrowserHandler.ComBrowserEvents.DownloadBeginEventHandler DownloadBeginEvent;

      public event ComBrowserHandler.ComBrowserEvents.DownloadCompleteEventHandler DownloadCompleteEvent;

      public event ComBrowserHandler.ComBrowserEvents.TitleChangeEventHandler TitleChangeEvent;

      public event ComBrowserHandler.ComBrowserEvents.PropertyChangeEventHandler PropertyChangeEvent;

      public event ComBrowserHandler.ComBrowserEvents.BeforeNavigate2EventHandler BeforeNavigate2Event;

      public event ComBrowserHandler.ComBrowserEvents.NewWindow2EventHandler NewWindow2Event;

      public event ComBrowserHandler.ComBrowserEvents.NavigateComplete2EventHandler NavigateComplete2Event;

      public event ComBrowserHandler.ComBrowserEvents.DocumentCompleteEventHandler DocumentCompleteEvent;

      public event ComBrowserHandler.ComBrowserEvents.OnQuitEventHandler OnQuitEvent;

      public event ComBrowserHandler.ComBrowserEvents.OnVisibleEventHandler OnVisibleEvent;

      public event ComBrowserHandler.ComBrowserEvents.OnToolBarEventHandler OnToolBarEvent;

      public event ComBrowserHandler.ComBrowserEvents.OnMenuBarEventHandler OnMenuBarEvent;

      public event ComBrowserHandler.ComBrowserEvents.OnStatusBarEventHandler OnStatusBarEvent;

      public event ComBrowserHandler.ComBrowserEvents.OnFullScreenEventHandler OnFullScreenEvent;

      public event ComBrowserHandler.ComBrowserEvents.OnTheaterModeEventHandler OnTheaterModeEvent;

      public event ComBrowserHandler.ComBrowserEvents.WindowSetResizableEventHandler WindowSetResizableEvent;

      public event ComBrowserHandler.ComBrowserEvents.WindowSetLeftEventHandler WindowSetLeftEvent;

      public event ComBrowserHandler.ComBrowserEvents.WindowSetTopEventHandler WindowSetTopEvent;

      public event ComBrowserHandler.ComBrowserEvents.WindowSetWidthEventHandler WindowSetWidthEvent;

      public event ComBrowserHandler.ComBrowserEvents.WindowSetHeightEventHandler WindowSetHeightEvent;

      public event ComBrowserHandler.ComBrowserEvents.WindowClosingEventHandler WindowClosingEvent;

      public event ComBrowserHandler.ComBrowserEvents.ClientToHostWindowEventHandler ClientToHostWindowEvent;

      public event ComBrowserHandler.ComBrowserEvents.SetSecureLockIconEventHandler SetSecureLockIconEvent;

      public event ComBrowserHandler.ComBrowserEvents.FileDownloadEventHandler FileDownloadEvent;

      public event ComBrowserHandler.ComBrowserEvents.NavigateErrorEventHandler NavigateErrorEvent;

      public event ComBrowserHandler.ComBrowserEvents.PrintTemplateInstantiationEventHandler PrintTemplateInstantiationEvent;

      public event ComBrowserHandler.ComBrowserEvents.PrintTemplateTeardownEventHandler PrintTemplateTeardownEvent;

      public event ComBrowserHandler.ComBrowserEvents.UpdatePageStatusEventHandler UpdatePageStatusEvent;

      public event ComBrowserHandler.ComBrowserEvents.PrivacyImpactedStateChangeEventHandler PrivacyImpactedStateChangeEvent;

      public event ComBrowserHandler.ComBrowserEvents.NewWindow3EventHandler NewWindow3Event;

      public void StatusTextChange(string Text)
      {
        if (this.StatusTextChangeEvent == null)
          return;
        this.StatusTextChangeEvent(Text);
      }

      public void ProgressChange(int Progress, int ProgressMax)
      {
        if (this.ProgressChangeEvent == null)
          return;
        this.ProgressChangeEvent(Progress, ProgressMax);
      }

      public void CommandStateChange(int Command, bool Enable)
      {
        if (this.CommandStateChangeEvent == null)
          return;
        this.CommandStateChangeEvent(Command, Enable);
      }

      public void DownloadBegin()
      {
        if (this.DownloadBeginEvent == null)
          return;
        this.DownloadBeginEvent();
      }

      public void DownloadComplete()
      {
        if (this.DownloadCompleteEvent == null)
          return;
        this.DownloadCompleteEvent();
      }

      public void TitleChange(string Text)
      {
        if (this.TitleChangeEvent == null)
          return;
        this.TitleChangeEvent(Text);
      }

      public void PropertyChange(string szProperty)
      {
        if (this.PropertyChangeEvent == null)
          return;
        this.PropertyChangeEvent(szProperty);
      }

      public void BeforeNavigate2(
        object pDisp,
        ref object URL,
        ref object Flags,
        ref object TargetFrameName,
        ref object PostData,
        ref object Headers,
        ref bool Cancel)
      {
        if (this.BeforeNavigate2Event == null)
          return;
        this.BeforeNavigate2Event(pDisp, ref URL, ref Flags, ref TargetFrameName, ref PostData, ref Headers, ref Cancel);
      }

      public void NewWindow2(ref object ppDisp, ref bool Cancel)
      {
        if (this.NewWindow2Event == null)
          return;
        this.NewWindow2Event(ref ppDisp, ref Cancel);
      }

      public void NavigateComplete2(object pDisp, ref object URL)
      {
        if (this.NavigateComplete2Event == null)
          return;
        this.NavigateComplete2Event(pDisp, ref URL);
      }

      public void DocumentComplete(object pDisp, ref object URL)
      {
        if (this.DocumentCompleteEvent == null)
          return;
        this.DocumentCompleteEvent(pDisp, ref URL);
      }

      public void OnQuit()
      {
        if (this.OnQuitEvent == null)
          return;
        this.OnQuitEvent();
      }

      public void OnVisible(bool Visible)
      {
        if (this.OnVisibleEvent == null)
          return;
        this.OnVisibleEvent(Visible);
      }

      public void OnToolBar(bool ToolBar)
      {
        if (this.OnToolBarEvent == null)
          return;
        this.OnToolBarEvent(ToolBar);
      }

      public void OnMenuBar(bool MenuBar)
      {
        if (this.OnMenuBarEvent == null)
          return;
        this.OnMenuBarEvent(MenuBar);
      }

      public void OnStatusBar(bool StatusBar)
      {
        if (this.OnStatusBarEvent == null)
          return;
        this.OnStatusBarEvent(StatusBar);
      }

      public void OnFullScreen(bool FullScreen)
      {
        if (this.OnFullScreenEvent == null)
          return;
        this.OnFullScreenEvent(FullScreen);
      }

      public void OnTheaterMode(bool TheaterMode)
      {
        if (this.OnTheaterModeEvent == null)
          return;
        this.OnTheaterModeEvent(TheaterMode);
      }

      public void WindowSetResizable(bool Resizable)
      {
        if (this.WindowSetResizableEvent == null)
          return;
        this.WindowSetResizableEvent(Resizable);
      }

      public void WindowSetLeft(int Left)
      {
        if (this.WindowSetLeftEvent == null)
          return;
        this.WindowSetLeftEvent(Left);
      }

      public void WindowSetTop(int Top)
      {
        if (this.WindowSetTopEvent == null)
          return;
        this.WindowSetTopEvent(Top);
      }

      public void WindowSetWidth(int Width)
      {
        if (this.WindowSetWidthEvent == null)
          return;
        this.WindowSetWidthEvent(Width);
      }

      public void WindowSetHeight(int Height)
      {
        if (this.WindowSetHeightEvent == null)
          return;
        this.WindowSetHeightEvent(Height);
      }

      public void WindowClosing(bool IsChildWindow, ref bool Cancel)
      {
        if (this.WindowClosingEvent == null)
          return;
        this.WindowClosingEvent(IsChildWindow, ref Cancel);
      }

      public void ClientToHostWindow(ref int CX, ref int CY)
      {
        if (this.ClientToHostWindowEvent == null)
          return;
        this.ClientToHostWindowEvent(ref CX, ref CY);
      }

      public void SetSecureLockIcon(int SecureLockIcon)
      {
        if (this.SetSecureLockIconEvent == null)
          return;
        this.SetSecureLockIconEvent(SecureLockIcon);
      }

      public void FileDownload(bool ActiveDocument, ref bool Cancel)
      {
        if (this.FileDownloadEvent == null)
          return;
        this.FileDownloadEvent(ActiveDocument, ref Cancel);
      }

      public void NavigateError(
        object pDisp,
        ref object URL,
        ref object Frame,
        ref object StatusCode,
        ref bool Cancel)
      {
        if (this.NavigateErrorEvent == null)
          return;
        this.NavigateErrorEvent(pDisp, ref URL, ref Frame, ref StatusCode, ref Cancel);
      }

      public void PrintTemplateInstantiation(object pDisp)
      {
        if (this.PrintTemplateInstantiationEvent == null)
          return;
        this.PrintTemplateInstantiationEvent(pDisp);
      }

      public void PrintTemplateTeardown(object pDisp)
      {
        if (this.PrintTemplateTeardownEvent == null)
          return;
        this.PrintTemplateTeardownEvent(pDisp);
      }

      public void UpdatePageStatus(object pDisp, ref object nPage, ref object fDone)
      {
        if (this.UpdatePageStatusEvent == null)
          return;
        this.UpdatePageStatusEvent(pDisp, ref nPage, ref fDone);
      }

      public void PrivacyImpactedStateChange(bool bImpacted)
      {
        if (this.PrivacyImpactedStateChangeEvent == null)
          return;
        this.PrivacyImpactedStateChangeEvent(bImpacted);
      }

      public void NewWindow3(
        ref object ppDisp,
        ref bool Cancel,
        uint dwFlags,
        string bstrUrlContext,
        string bstrUrl)
      {
        if (this.NewWindow3Event == null)
          return;
        this.NewWindow3Event(ref ppDisp, ref Cancel, dwFlags, bstrUrlContext, bstrUrl);
      }

      public delegate void StatusTextChangeEventHandler(string Text);

      public delegate void ProgressChangeEventHandler(int Progress, int ProgressMax);

      public delegate void CommandStateChangeEventHandler(int Command, bool Enable);

      public delegate void DownloadBeginEventHandler();

      public delegate void DownloadCompleteEventHandler();

      public delegate void TitleChangeEventHandler(string Text);

      public delegate void PropertyChangeEventHandler(string szProperty);

      public delegate void BeforeNavigate2EventHandler(
        object pDisp,
        ref object URL,
        ref object Flags,
        ref object TargetFrameName,
        ref object PostData,
        ref object Headers,
        ref bool Cancel);

      public delegate void NewWindow2EventHandler(ref object ppDisp, ref bool Cancel);

      public delegate void NavigateComplete2EventHandler(object pDisp, ref object URL);

      public delegate void DocumentCompleteEventHandler(object pDisp, ref object URL);

      public delegate void OnQuitEventHandler();

      public delegate void OnVisibleEventHandler(bool Visible);

      public delegate void OnToolBarEventHandler(bool ToolBar);

      public delegate void OnMenuBarEventHandler(bool MenuBar);

      public delegate void OnStatusBarEventHandler(bool StatusBar);

      public delegate void OnFullScreenEventHandler(bool FullScreen);

      public delegate void OnTheaterModeEventHandler(bool TheaterMode);

      public delegate void WindowSetResizableEventHandler(bool Resizable);

      public delegate void WindowSetLeftEventHandler(int Left);

      public delegate void WindowSetTopEventHandler(int Top);

      public delegate void WindowSetWidthEventHandler(int Width);

      public delegate void WindowSetHeightEventHandler(int Height);

      public delegate void WindowClosingEventHandler(bool IsChildWindow, ref bool Cancel);

      public delegate void ClientToHostWindowEventHandler(ref int CX, ref int CY);

      public delegate void SetSecureLockIconEventHandler(int SecureLockIcon);

      public delegate void FileDownloadEventHandler(bool ActiveDocument, ref bool Cancel);

      public delegate void NavigateErrorEventHandler(
        object pDisp,
        ref object URL,
        ref object Frame,
        ref object StatusCode,
        ref bool Cancel);

      public delegate void PrintTemplateInstantiationEventHandler(object pDisp);

      public delegate void PrintTemplateTeardownEventHandler(object pDisp);

      public delegate void UpdatePageStatusEventHandler(
        object pDisp,
        ref object nPage,
        ref object fDone);

      public delegate void PrivacyImpactedStateChangeEventHandler(bool bImpacted);

      public delegate void NewWindow3EventHandler(
        ref object ppDisp,
        ref bool Cancel,
        uint dwFlags,
        string bstrUrlContext,
        string bstrUrl);
    }

    private class ComDocumentHostHandler : ComBrowserHandler.IDocHostUIHandler
    {
      public ComDocumentHostHandler(object htmlDocument)
      {
        if (!(htmlDocument is ComBrowserHandler.ICustomDoc customDoc))
          return;
        customDoc.SetUIHandler((ComBrowserHandler.IDocHostUIHandler) this);
      }

      public event ComBrowserHandler.ComDocumentHostHandler.ContextMenuEventHandler ContextMenu;

      public object External { get; set; }

      public int ShowContextMenu(
        uint dwID,
        ref ComBrowserHandler.POINT pt,
        object pcmdtReserved,
        object pdispReserved)
      {
        if (this.ContextMenu != null)
        {
          ComBrowserHandler.ComDocumentHostHandler.ContextMenuEventArgs e = new ComBrowserHandler.ComDocumentHostHandler.ContextMenuEventArgs(new Point((double) pt.X, (double) pt.Y));
          this.ContextMenu(this, e);
          if (!e.Show)
            return 0;
        }
        return 1;
      }

      public int GetHostInfo(ref ComBrowserHandler.DOCHOSTUIINFO info)
      {
        info.dwDoubleClick = 0U;
        info.dwFlags = 1168588818U;
        return 0;
      }

      public int GetExternal(out object ppDispatch)
      {
        ppDispatch = this.External;
        return this.External != null ? 0 : -2147467263;
      }

      public int ShowUI(
        int dwID,
        ComBrowserHandler.IOleInPlaceActiveObject activeObject,
        ComBrowserHandler.IOleCommandTarget commandTarget,
        ComBrowserHandler.IOleInPlaceFrame frame,
        ComBrowserHandler.IOleInPlaceUIWindow doc)
      {
        return -2147467263;
      }

      public int HideUI() => -2147467263;

      public int UpdateUI() => -2147467263;

      public int EnableModeless(bool fEnable) => -2147467263;

      public int OnDocWindowActivate(bool fActivate) => -2147467263;

      public int OnFrameWindowActivate(bool fActivate) => -2147467263;

      public int ResizeBorder(
        ref ComBrowserHandler.RECT rect,
        ComBrowserHandler.IOleInPlaceUIWindow doc,
        bool fFrameWindow)
      {
        return -2147467263;
      }

      public int TranslateAccelerator(ref ComBrowserHandler.MSG msg, ref Guid group, uint nCmdID) => -2147467263;

      public int GetOptionKeyPath(out string pbstrKey, uint dw)
      {
        pbstrKey = (string) null;
        return -2147467263;
      }

      public int GetDropTarget(
        ComBrowserHandler.IDropTarget pDropTarget,
        out ComBrowserHandler.IDropTarget ppDropTarget)
      {
        ppDropTarget = (ComBrowserHandler.IDropTarget) null;
        return -2147467263;
      }

      public int TranslateUrl(uint dwTranslate, string strURLIn, out string pstrURLOut)
      {
        pstrURLOut = (string) null;
        return -2147467263;
      }

      public int FilterDataObject(System.Runtime.InteropServices.ComTypes.IDataObject pDO, out System.Runtime.InteropServices.ComTypes.IDataObject ppDORet)
      {
        ppDORet = (System.Runtime.InteropServices.ComTypes.IDataObject) null;
        return -2147467263;
      }

      public class ContextMenuEventArgs
      {
        public ContextMenuEventArgs(Point point) => this.Point = point;

        public Point Point { get; private set; }

        public bool Show { get; set; }
      }

      public delegate void ContextMenuEventHandler(
        ComBrowserHandler.ComDocumentHostHandler sender,
        ComBrowserHandler.ComDocumentHostHandler.ContextMenuEventArgs e);
    }

    private class ComEventSink<I, T> : IDisposable where T : I
    {
      private static readonly Guid InterfaceId = typeof (I).GUID;
      private List<ComBrowserHandler.ComEventSink<I, T>.Connection> connections;

      public ComEventSink(T handler, object instance = null)
      {
        this.connections = new List<ComBrowserHandler.ComEventSink<I, T>.Connection>();
        this.Handler = handler;
        if (instance == null)
          return;
        this.Attach(instance);
      }

      public T Handler { get; private set; }

      public bool Attach(object instance)
      {
        Guid interfaceId = ComBrowserHandler.ComEventSink<I, T>.InterfaceId;
        if (instance is IConnectionPointContainer connectionPointContainer)
        {
          IConnectionPoint ppCP = (IConnectionPoint) null;
          connectionPointContainer.FindConnectionPoint(ref interfaceId, out ppCP);
          if (ppCP != null)
          {
            int pdwCookie = -1;
            ppCP.Advise((object) this.Handler, out pdwCookie);
            if (pdwCookie > -1)
            {
              this.connections.Add(new ComBrowserHandler.ComEventSink<I, T>.Connection(ppCP, pdwCookie));
              return true;
            }
          }
        }
        return false;
      }

      public void Dispose()
      {
        this.Dispose(true);
        GC.SuppressFinalize((object) this);
      }

      protected virtual void Dispose(bool disposing)
      {
        if (disposing)
        {
          if (this.connections != null)
          {
            foreach (ComBrowserHandler.ComEventSink<I, T>.Connection connection in this.connections)
              connection.ConnectionPoint.Unadvise(connection.Cookie);
            this.connections.Clear();
          }
          if (this.Handler is IDisposable handler)
            handler.Dispose();
        }
        this.connections = (List<ComBrowserHandler.ComEventSink<I, T>.Connection>) null;
        this.Handler = default (T);
      }

      private class Connection
      {
        public Connection(IConnectionPoint connectionPoint, int cookie)
        {
          this.ConnectionPoint = connectionPoint;
          this.Cookie = cookie;
        }

        public IConnectionPoint ConnectionPoint { get; private set; }

        public int Cookie { get; private set; }
      }
    }
  }
}
