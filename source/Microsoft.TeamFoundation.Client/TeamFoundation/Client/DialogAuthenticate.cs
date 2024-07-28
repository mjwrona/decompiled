// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.DialogAuthenticate
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Internal;
using System;
using System.ComponentModel;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Windows.Forms;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class DialogAuthenticate : BaseDialog
  {
    private FederatedAcsLogon m_federatedLoginHelper;
    private IContainer components;
    private WebBrowser connectWebBrowser;
    private Label labelStatus;

    internal DialogAuthenticate(string location, string replyToAddress)
    {
      this.InitializeComponent();
      this.m_federatedLoginHelper = new FederatedAcsLogon(new Uri(replyToAddress), new Uri(replyToAddress), (Uri) null);
      this.m_federatedLoginHelper.AuthenticationComplete += new EventHandler<AuthenticationCompleteEventArgs>(this.AuthenticationComplete);
      this.labelStatus.Width = this.Width;
      this.Resize += new EventHandler(this.DialogAuthenticate_Resize);
      this.connectWebBrowser.IsWebBrowserContextMenuEnabled = true;
      this.connectWebBrowser.Navigating += new WebBrowserNavigatingEventHandler(this.Navigating);
      this.connectWebBrowser.Navigated += new WebBrowserNavigatedEventHandler(this.Navigated);
      this.connectWebBrowser.Navigate(location);
    }

    public CookieCollection SsoCookieCollection { get; private set; }

    internal WebBrowser ConnectWebBrowser => this.connectWebBrowser;

    private void DialogAuthenticate_Resize(object sender, EventArgs e) => this.labelStatus.Width = this.Width;

    private void AuthenticationComplete(object sender, AuthenticationCompleteEventArgs e)
    {
      this.SsoCookieCollection = this.m_federatedLoginHelper.SsoCookieCollection;
      this.DialogResult = DialogResult.OK;
      this.Close();
    }

    private void Navigating(object sender, WebBrowserNavigatingEventArgs e)
    {
      string str = e.Url.ToString();
      if (str.Length > 80)
        str = str.Substring(0, 80);
      this.labelStatus.Text = ClientResources.DialogAuthenticate_Waiting((object) str);
      this.Cursor = Cursors.WaitCursor;
      this.m_federatedLoginHelper.Navigating(e.Url);
    }

    private void Navigated(object sender, WebBrowserNavigatedEventArgs e)
    {
      this.labelStatus.Text = ClientResources.DialogAuthenticate_Done();
      this.Cursor = Cursors.Default;
      this.m_federatedLoginHelper.Navigated(e.Url);
    }

    protected override void OnClosing(CancelEventArgs e)
    {
      if (this.SsoCookieCollection == null)
        this.SsoCookieCollection = this.m_federatedLoginHelper.GetFederatedCookies();
      base.OnClosing(e);
    }

    public static CookieCollection Authenticate(string location, string replyToAddress)
    {
      if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
      {
        using (DialogAuthenticate dialogAuthenticate = new DialogAuthenticate(location, replyToAddress))
          return dialogAuthenticate.ShowDialog(UIHost.DefaultParentWindow) == DialogResult.OK ? dialogAuthenticate.SsoCookieCollection : (CookieCollection) null;
      }
      else
      {
        DialogAuthenticate.DialogInvokeParameters parameter = new DialogAuthenticate.DialogInvokeParameters();
        parameter.ParentWindow = UIHost.DefaultParentWindow;
        parameter.Location = location;
        parameter.ReplyToAddress = replyToAddress;
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        Thread thread = new Thread(DialogAuthenticate.\u003C\u003EO.\u003C0\u003E__InvokeDialog ?? (DialogAuthenticate.\u003C\u003EO.\u003C0\u003E__InvokeDialog = new ParameterizedThreadStart(DialogAuthenticate.InvokeDialog)));
        thread.SetApartmentState(ApartmentState.STA);
        thread.IsBackground = true;
        thread.Start((object) parameter);
        thread.Join();
        return parameter.SsoCookieCollection;
      }
    }

    private static void InvokeDialog(object state)
    {
      DialogAuthenticate.DialogInvokeParameters invokeParameters = (DialogAuthenticate.DialogInvokeParameters) state;
      using (DialogAuthenticate dialogAuthenticate = new DialogAuthenticate(invokeParameters.Location, invokeParameters.ReplyToAddress))
      {
        if (dialogAuthenticate.ShowDialog(invokeParameters.ParentWindow) == DialogResult.OK)
          invokeParameters.SsoCookieCollection = dialogAuthenticate.SsoCookieCollection;
        else
          invokeParameters.SsoCookieCollection = (CookieCollection) null;
      }
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      this.Resize -= new EventHandler(this.DialogAuthenticate_Resize);
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (DialogAuthenticate));
      this.connectWebBrowser = (WebBrowser) new DialogAuthenticate.AuthenticateWebBrowser();
      this.connectWebBrowser.Dock = DockStyle.Fill;
      this.connectWebBrowser.ScrollBarsEnabled = false;
      this.labelStatus = new Label();
      this.labelStatus.Dock = DockStyle.Bottom;
      this.labelStatus.Margin = new Padding(0, 4, 0, 4);
      this.labelStatus.AutoSize = false;
      this.SuspendLayout();
      this.connectWebBrowser.AllowWebBrowserDrop = false;
      componentResourceManager.ApplyResources((object) this.connectWebBrowser, "connectWebBrowser");
      this.connectWebBrowser.IsWebBrowserContextMenuEnabled = false;
      this.connectWebBrowser.Name = "connectWebBrowser";
      componentResourceManager.ApplyResources((object) this.labelStatus, "labelStatus");
      this.labelStatus.Name = "labelStatus";
      this.AutoScaleMode = AutoScaleMode.None;
      componentResourceManager.ApplyResources((object) this, "$this");
      this.Controls.Add((Control) this.connectWebBrowser);
      this.Controls.Add((Control) this.labelStatus);
      this.AlwaysShowHelpButton = false;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = nameof (DialogAuthenticate);
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.ResumeLayout(false);
    }

    private class DialogInvokeParameters
    {
      public IWin32Window ParentWindow;
      public string Location;
      public string ReplyToAddress;
      public CookieCollection SsoCookieCollection;
    }

    private class AuthenticateWebBrowser : WebBrowser
    {
      private AxHost.ConnectionPointCookie m_cookie;
      private DialogAuthenticate.AuthenticateWebBrowser.SuppressFileDownloadSink m_sink;

      protected override void CreateSink()
      {
        base.CreateSink();
        object activeXinstance = this.ActiveXInstance;
        if (activeXinstance == null)
          return;
        this.m_sink = new DialogAuthenticate.AuthenticateWebBrowser.SuppressFileDownloadSink(this);
        this.m_cookie = new AxHost.ConnectionPointCookie(activeXinstance, (object) this.m_sink, typeof (DialogAuthenticate.AuthenticateWebBrowser.DWebBrowserEvents2));
      }

      protected override void DetachSink()
      {
        if (this.m_cookie != null)
        {
          this.m_cookie.Disconnect();
          this.m_cookie = (AxHost.ConnectionPointCookie) null;
        }
        base.DetachSink();
      }

      [ClassInterface(ClassInterfaceType.None)]
      private class SuppressFileDownloadSink : 
        StandardOleMarshalObject,
        DialogAuthenticate.AuthenticateWebBrowser.DWebBrowserEvents2
      {
        private readonly DialogAuthenticate.AuthenticateWebBrowser m_parent;

        public SuppressFileDownloadSink(DialogAuthenticate.AuthenticateWebBrowser parent) => this.m_parent = parent;

        public void StatusTextChange(string Text)
        {
        }

        public void ProgressChange(int Progress, int ProgressMax)
        {
        }

        public void CommandStateChange(int Command, bool Enable)
        {
        }

        public void DownloadBegin()
        {
        }

        public void DownloadComplete()
        {
        }

        public void TitleChange(string Text)
        {
        }

        public void PropertyChange(string szProperty)
        {
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
        }

        public void NewWindow2(ref object ppDisp, ref bool Cancel)
        {
        }

        public void NavigateComplete2(object pDisp, ref object URL)
        {
        }

        public void DocumentComplete(object pDisp, ref object URL)
        {
        }

        public void OnQuit()
        {
        }

        public void OnVisible(bool Visible)
        {
        }

        public void OnToolBar(bool ToolBar)
        {
        }

        public void OnMenuBar(bool MenuBar)
        {
        }

        public void OnStatusBar(bool StatusBar)
        {
        }

        public void OnFullScreen(bool FullScreen)
        {
        }

        public void OnTheaterMode(bool TheaterMode)
        {
        }

        public void WindowSetResizable(bool Resizable)
        {
        }

        public void WindowSetLeft(int Left)
        {
        }

        public void WindowSetTop(int Top)
        {
        }

        public void WindowSetWidth(int Width)
        {
        }

        public void WindowSetHeight(int Height)
        {
        }

        public void WindowClosing(bool IsChildWindow, ref bool Cancel)
        {
        }

        public void ClientToHostWindow(ref int CX, ref int CY)
        {
        }

        public void SetSecureLockIcon(int SecureLockIcon)
        {
        }

        public void PrintTemplateInstantiation(object pDisp)
        {
        }

        public void PrintTemplateTeardown(object pDisp)
        {
        }

        public void UpdatePageStatus(object pDisp, ref object nPage, ref object fDone)
        {
        }

        public void PrivacyImpactedStateChange(bool bImpacted)
        {
        }

        public void NewWindow3(
          ref object ppDisp,
          ref bool Cancel,
          uint dwFlags,
          string bstrUrlContext,
          string bstrUrl)
        {
        }

        public void FileDownload(bool ActiveDocument, ref bool Cancel) => Cancel = true;

        public void NavigateError(
          object pDisp,
          ref object URL,
          ref object Frame,
          ref object StatusCode,
          ref bool Cancel)
        {
          try
          {
            if (200 != (int) StatusCode)
              return;
            this.m_parent.OnNavigated(new WebBrowserNavigatedEventArgs(new Uri((string) URL)));
          }
          catch
          {
          }
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
    }
  }
}
