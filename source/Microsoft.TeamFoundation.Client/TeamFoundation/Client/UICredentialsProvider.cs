// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.UICredentialsProvider
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Internal;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Microsoft.TeamFoundation.Client
{
  [Obsolete("This class has been deprecated and will be removed in a future release. Use VssClientCredentials with CredentialPromptType.PromptIfNeeded instead.", false)]
  public class UICredentialsProvider : IUICredentialsProvider, ICredentialsProvider
  {
    private IUICredentialsProvider m_windowsProvider;

    public UICredentialsProvider()
      : this(CachedCredentialsType.Windows)
    {
    }

    public UICredentialsProvider(IWin32Window parentWindow)
      : this(CachedCredentialsType.Windows, parentWindow)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public UICredentialsProvider(CachedCredentialsType credentialsType)
      : this(credentialsType, (IWin32Window) null)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public UICredentialsProvider(CachedCredentialsType credentialsType, IWin32Window parentWindow)
    {
      if (Environment.OSVersion.Version.Major >= 6)
        this.m_windowsProvider = (IUICredentialsProvider) new UICredentialsProvider.VistaCredentialsProvider(credentialsType, parentWindow);
      else
        this.m_windowsProvider = (IUICredentialsProvider) new UICredentialsProvider.PreVistaCredentialsProvider(parentWindow);
    }

    ICredentials ICredentialsProvider.GetCredentials(Uri uri, ICredentials failedCredentials) => this.m_windowsProvider.GetCredentials(uri, failedCredentials);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public ICredentials GetCredentials(
      Uri uri,
      ICredentials failedCredentials,
      string caption,
      string messageText,
      IntPtr parentWindowHandle)
    {
      return this.m_windowsProvider.GetCredentials(uri, failedCredentials, caption, messageText, parentWindowHandle);
    }

    void ICredentialsProvider.NotifyCredentialsAuthenticated(Uri uri) => this.m_windowsProvider.NotifyCredentialsAuthenticated(uri);

    private class VistaCredentialsProvider : IUICredentialsProvider, ICredentialsProvider
    {
      private IWin32Window m_parentWindow;
      private CachedCredentialsType m_credentialsType;

      public VistaCredentialsProvider(
        CachedCredentialsType credentialsType,
        IWin32Window parentWindow)
      {
        this.m_credentialsType = credentialsType;
        this.m_parentWindow = parentWindow;
      }

      ICredentials ICredentialsProvider.GetCredentials(Uri uri, ICredentials failedCredentials)
      {
        IWin32Window win32Window = this.m_parentWindow ?? UIHost.DefaultParentWindow;
        return ((IUICredentialsProvider) this).GetCredentials(uri, failedCredentials, string.Empty, string.Empty, win32Window != null ? win32Window.Handle : IntPtr.Zero);
      }

      ICredentials IUICredentialsProvider.GetCredentials(
        Uri uri,
        ICredentials failedCredentials,
        string caption,
        string messageText,
        IntPtr parentWindowHandle)
      {
        ArgumentUtility.CheckForNull<Uri>(uri, nameof (uri));
        StringBuilder pszUserName = new StringBuilder(string.Empty, 513);
        StringBuilder pszDomainName = new StringBuilder(string.Empty, 513);
        StringBuilder stringBuilder = new StringBuilder(string.Empty, 256);
        bool pfSave = true;
        uint pulAuthPackage = 0;
        Microsoft.TeamFoundation.Common.Internal.NativeMethods.CREDUI_INFO pUiInfo = new Microsoft.TeamFoundation.Common.Internal.NativeMethods.CREDUI_INFO();
        pUiInfo.cbSize = Marshal.SizeOf<Microsoft.TeamFoundation.Common.Internal.NativeMethods.CREDUI_INFO>(pUiInfo);
        pUiInfo.hwndParent = parentWindowHandle;
        pUiInfo.pszCaptionText = string.IsNullOrEmpty(caption) ? ClientResources.UICredProvider_TitleText((object) uri.Host) : caption;
        if (pUiInfo.pszCaptionText.Length > 128)
          pUiInfo.pszCaptionText = pUiInfo.pszCaptionText.Substring(0, 128);
        pUiInfo.pszMessageText = string.IsNullOrEmpty(messageText) ? ClientResources.UICredProvider_MessageText() : messageText;
        if (pUiInfo.pszMessageText.Length > (int) short.MaxValue)
          pUiInfo.pszMessageText = pUiInfo.pszMessageText.Substring(0, (int) short.MaxValue);
        pUiInfo.hbmBanner = IntPtr.Zero;
        int dwFlags = 18;
        IntPtr ppvOutAuthBuffer = IntPtr.Zero;
        uint pulOutAuthBufferSize = 0;
        byte[] primerCredentialsBlob = this.GetPrimerCredentialsBlob(CredentialsProviderHelper.FailedUserName(uri, failedCredentials));
        try
        {
          uint length = primerCredentialsBlob != null ? (uint) primerCredentialsBlob.Length : 0U;
          if (Microsoft.TeamFoundation.Common.Internal.NativeMethods.CredUIPromptForWindowsCredentials(ref pUiInfo, 0, ref pulAuthPackage, primerCredentialsBlob, length, out ppvOutAuthBuffer, out pulOutAuthBufferSize, ref pfSave, dwFlags) != 0)
            return (ICredentials) null;
          uint capacity1 = (uint) pszUserName.Capacity;
          uint capacity2 = (uint) pszDomainName.Capacity;
          uint capacity3 = (uint) stringBuilder.Capacity;
          if (!Microsoft.TeamFoundation.Common.Internal.NativeMethods.CredUnPackAuthenticationBuffer(1, ppvOutAuthBuffer, pulOutAuthBufferSize, pszUserName, ref capacity1, pszDomainName, ref capacity2, stringBuilder, ref capacity3) && 775 == Marshal.GetLastWin32Error())
          {
            uint capacity4 = (uint) pszUserName.Capacity;
            uint capacity5 = (uint) pszDomainName.Capacity;
            uint capacity6 = (uint) stringBuilder.Capacity;
            if (!Microsoft.TeamFoundation.Common.Internal.NativeMethods.CredUnPackAuthenticationBuffer(0, ppvOutAuthBuffer, pulOutAuthBufferSize, pszUserName, ref capacity4, pszDomainName, ref capacity5, stringBuilder, ref capacity6))
              return (ICredentials) null;
          }
          string str = pszDomainName.Length <= 0 || pszUserName.Length <= 0 ? pszUserName.ToString() : pszDomainName.ToString() + "\\" + pszUserName.ToString();
          Uri uri1 = new Uri(uri.GetLeftPart(UriPartial.Authority));
          string qualifiedDnsName = this.GetFullyQualifiedDnsName(uri.Host);
          CredentialsCacheManager credentialsCacheManager = new CredentialsCacheManager();
          credentialsCacheManager.DeleteCredentials(uri1);
          if (this.m_credentialsType == CachedCredentialsType.Windows)
            credentialsCacheManager.DeleteWindowsCredentials(qualifiedDnsName);
          NetworkCredential networkCredential = CredentialsProviderHelper.GetNetworkCredential(str, stringBuilder);
          if (pfSave)
          {
            credentialsCacheManager.StoreCredentials(uri1, str, stringBuilder.ToString(), this.m_credentialsType, false);
            if (this.m_credentialsType == CachedCredentialsType.Windows)
              credentialsCacheManager.StoreWindowsCredentials(qualifiedDnsName, str, stringBuilder.ToString());
          }
          return (ICredentials) networkCredential;
        }
        finally
        {
          if (IntPtr.Zero != ppvOutAuthBuffer && pulOutAuthBufferSize > 0U)
          {
            CredentialsProviderHelper.ZeroStringBuilder(stringBuilder);
            Microsoft.TeamFoundation.Common.Internal.NativeMethods.ZeroMemory(ppvOutAuthBuffer, pulOutAuthBufferSize);
            Marshal.FreeCoTaskMem(ppvOutAuthBuffer);
          }
        }
      }

      private byte[] GetPrimerCredentialsBlob(string userName)
      {
        if (userName == null || userName.Length == 0)
          return (byte[]) null;
        uint pcbPackedCredentials = 0;
        if (Microsoft.TeamFoundation.Common.Internal.NativeMethods.CredPackAuthenticationBuffer(0, userName, string.Empty, (byte[]) null, ref pcbPackedCredentials))
          return (byte[]) null;
        byte[] pPackedCredentials = new byte[(int) pcbPackedCredentials];
        return !Microsoft.TeamFoundation.Common.Internal.NativeMethods.CredPackAuthenticationBuffer(0, userName, string.Empty, pPackedCredentials, ref pcbPackedCredentials) ? (byte[]) null : pPackedCredentials;
      }

      void ICredentialsProvider.NotifyCredentialsAuthenticated(Uri uri)
      {
      }

      private string GetFullyQualifiedDnsName(string hostName)
      {
        try
        {
          IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
          hostName = !string.IsNullOrEmpty(hostEntry.HostName) ? hostEntry.HostName : hostName;
        }
        catch (SocketException ex)
        {
        }
        catch (Exception ex)
        {
          TeamFoundationTrace.TraceAndDebugFailException(ex);
        }
        return hostName;
      }
    }

    private class PreVistaCredentialsProvider : IUICredentialsProvider, ICredentialsProvider
    {
      private IWin32Window m_parentWindow;

      public PreVistaCredentialsProvider(IWin32Window parentWindow) => this.m_parentWindow = parentWindow;

      ICredentials ICredentialsProvider.GetCredentials(Uri uri, ICredentials failedCredentials)
      {
        IWin32Window win32Window = this.m_parentWindow ?? UIHost.DefaultParentWindow;
        return ((IUICredentialsProvider) this).GetCredentials(uri, failedCredentials, string.Empty, string.Empty, win32Window != null ? win32Window.Handle : IntPtr.Zero);
      }

      ICredentials IUICredentialsProvider.GetCredentials(
        Uri uri,
        ICredentials failedCredentials,
        string caption,
        string messageText,
        IntPtr parentWindowHandle)
      {
        ArgumentUtility.CheckForNull<Uri>(uri, nameof (uri));
        StringBuilder pszUserName = new StringBuilder(CredentialsProviderHelper.FailedUserName(uri, failedCredentials), 513);
        StringBuilder stringBuilder = new StringBuilder(string.Empty, 256);
        bool pfSave = false;
        Microsoft.TeamFoundation.Common.Internal.NativeMethods.CREDUI_INFO pUiInfo = new Microsoft.TeamFoundation.Common.Internal.NativeMethods.CREDUI_INFO();
        pUiInfo.cbSize = Marshal.SizeOf<Microsoft.TeamFoundation.Common.Internal.NativeMethods.CREDUI_INFO>(pUiInfo);
        pUiInfo.hwndParent = parentWindowHandle;
        pUiInfo.pszCaptionText = string.IsNullOrEmpty(caption) ? ClientResources.UICredProvider_TitleText((object) uri.Host) : caption;
        if (pUiInfo.pszCaptionText.Length > 128)
          pUiInfo.pszCaptionText = pUiInfo.pszCaptionText.Substring(0, 128);
        pUiInfo.pszMessageText = string.IsNullOrEmpty(messageText) ? ClientResources.UICredProvider_MessageText() : messageText;
        if (pUiInfo.pszMessageText.Length > (int) short.MaxValue)
          pUiInfo.pszMessageText = pUiInfo.pszMessageText.Substring(0, (int) short.MaxValue);
        pUiInfo.hbmBanner = IntPtr.Zero;
        int dwFlags = 262274;
        try
        {
          return Microsoft.TeamFoundation.Common.Internal.NativeMethods.CredUIPromptForCredentials(ref pUiInfo, uri.AbsoluteUri, IntPtr.Zero, 0, pszUserName, (uint) (pszUserName.Capacity + 1), stringBuilder, (uint) (stringBuilder.Capacity + 1), ref pfSave, dwFlags) != 0 ? (ICredentials) null : (ICredentials) CredentialsProviderHelper.GetNetworkCredential(pszUserName.ToString(), stringBuilder);
        }
        finally
        {
          CredentialsProviderHelper.ZeroStringBuilder(stringBuilder);
        }
      }

      void ICredentialsProvider.NotifyCredentialsAuthenticated(Uri uri)
      {
      }
    }
  }
}
