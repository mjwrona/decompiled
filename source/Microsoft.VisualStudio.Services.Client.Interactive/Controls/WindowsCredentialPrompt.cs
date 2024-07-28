// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.Controls.WindowsCredentialPrompt
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Client.Controls
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class WindowsCredentialPrompt : IVssCredentialPrompt
  {
    private IntPtr m_parentWindow;
    private CachedCredentialsType m_credentialsType;

    public WindowsCredentialPrompt()
      : this(CachedCredentialsType.Windows)
    {
    }

    public WindowsCredentialPrompt(CachedCredentialsType credentialsType)
      : this(credentialsType, IntPtr.Zero)
    {
    }

    public WindowsCredentialPrompt(CachedCredentialsType credentialsType, IntPtr parentWindowHandle)
    {
      this.m_credentialsType = credentialsType;
      if (parentWindowHandle != IntPtr.Zero)
        this.m_parentWindow = parentWindowHandle;
      else
        this.m_parentWindow = ClientNativeMethods.GetDefaultParentWindow();
    }

    public IDictionary<string, string> Parameters { get; set; }

    Task<IssuedToken> IVssCredentialPrompt.GetTokenAsync(
      IssuedTokenProvider provider,
      IssuedToken failedToken)
    {
      ICredentials failedCredentials = (ICredentials) null;
      if (failedToken != null)
        failedCredentials = this.ToCredential(failedToken);
      ICredentials token = this.GetToken(provider.SignInUrl, failedCredentials);
      IssuedToken result = (IssuedToken) null;
      if (token != null)
        result = this.ToToken(token);
      return Task.FromResult<IssuedToken>(result);
    }

    public ICredentials GetToken(Uri uri, ICredentials failedCredentials)
    {
      ArgumentUtility.CheckForNull<Uri>(uri, nameof (uri));
      StringBuilder pszUserName = new StringBuilder(string.Empty, 513);
      StringBuilder pszDomainName = new StringBuilder(string.Empty, 513);
      StringBuilder stringBuilder = new StringBuilder(string.Empty, 256);
      bool pfSave = true;
      uint pulAuthPackage = 0;
      ClientNativeMethods.CREDUI_INFO pUiInfo = new ClientNativeMethods.CREDUI_INFO();
      pUiInfo.cbSize = Marshal.SizeOf<ClientNativeMethods.CREDUI_INFO>(pUiInfo);
      pUiInfo.hwndParent = this.m_parentWindow;
      pUiInfo.pszCaptionText = ClientResources.UICredProvider_TitleText((object) uri.Host);
      pUiInfo.pszMessageText = ClientResources.UICredProvider_MessageText();
      if (pUiInfo.pszCaptionText.Length > 128)
        pUiInfo.pszCaptionText = pUiInfo.pszCaptionText.Substring(0, 128);
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
        if (ClientNativeMethods.CredUIPromptForWindowsCredentials(ref pUiInfo, 0, ref pulAuthPackage, primerCredentialsBlob, length, out ppvOutAuthBuffer, out pulOutAuthBufferSize, ref pfSave, dwFlags) != 0)
          return (ICredentials) null;
        uint capacity1 = (uint) pszUserName.Capacity;
        uint capacity2 = (uint) pszDomainName.Capacity;
        uint capacity3 = (uint) stringBuilder.Capacity;
        if (!ClientNativeMethods.CredUnPackAuthenticationBuffer(1, ppvOutAuthBuffer, pulOutAuthBufferSize, pszUserName, ref capacity1, pszDomainName, ref capacity2, stringBuilder, ref capacity3) && 775 == Marshal.GetLastWin32Error())
        {
          uint capacity4 = (uint) pszUserName.Capacity;
          uint capacity5 = (uint) pszDomainName.Capacity;
          uint capacity6 = (uint) stringBuilder.Capacity;
          if (!ClientNativeMethods.CredUnPackAuthenticationBuffer(0, ppvOutAuthBuffer, pulOutAuthBufferSize, pszUserName, ref capacity4, pszDomainName, ref capacity5, stringBuilder, ref capacity6))
            return (ICredentials) null;
        }
        string str = pszDomainName.Length <= 0 || pszUserName.Length <= 0 ? pszUserName.ToString() : pszDomainName.ToString() + "\\" + pszUserName.ToString();
        Uri uri1 = new Uri(uri.GetLeftPart(UriPartial.Authority));
        string qualifiedDnsName = this.GetFullyQualifiedDnsName(uri.Host);
        CredentialsCacheManager credentialsCacheManager = new CredentialsCacheManager();
        credentialsCacheManager.DeleteCredentials(uri1);
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
          ClientNativeMethods.ZeroMemory(ppvOutAuthBuffer, pulOutAuthBufferSize);
          Marshal.FreeCoTaskMem(ppvOutAuthBuffer);
        }
      }
    }

    private byte[] GetPrimerCredentialsBlob(string userName)
    {
      if (userName == null || userName.Length == 0)
        return (byte[]) null;
      uint pcbPackedCredentials = 0;
      if (ClientNativeMethods.CredPackAuthenticationBuffer(0, userName, string.Empty, (byte[]) null, ref pcbPackedCredentials))
        return (byte[]) null;
      byte[] pPackedCredentials = new byte[(int) pcbPackedCredentials];
      return !ClientNativeMethods.CredPackAuthenticationBuffer(0, userName, string.Empty, pPackedCredentials, ref pcbPackedCredentials) ? (byte[]) null : pPackedCredentials;
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
      }
      return hostName;
    }

    private IssuedToken ToToken(ICredentials credential)
    {
      switch (this.m_credentialsType)
      {
        case CachedCredentialsType.Windows:
          return (IssuedToken) new WindowsToken(credential);
        case CachedCredentialsType.Basic:
          return (IssuedToken) new VssBasicToken(credential);
        default:
          return (IssuedToken) null;
      }
    }

    private ICredentials ToCredential(IssuedToken token)
    {
      switch (this.m_credentialsType)
      {
        case CachedCredentialsType.Windows:
          return ((WindowsToken) token).Credentials;
        case CachedCredentialsType.Basic:
          return ((VssBasicToken) token).Credentials;
        default:
          return (ICredentials) null;
      }
    }
  }
}
