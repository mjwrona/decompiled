// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.OAuth.VssOAuthPasswordClientCredential
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.VisualStudio.Services.OAuth
{
  public sealed class VssOAuthPasswordClientCredential : VssOAuthClientCredential
  {
    private readonly SecureString m_clientSecret;

    public VssOAuthPasswordClientCredential(string clientId, string clientSecret)
      : this(clientId, VssOAuthPasswordClientCredential.EncryptSecret(clientSecret))
    {
    }

    public VssOAuthPasswordClientCredential(string clientId, SecureString clientSecret)
      : base(VssOAuthClientCredentialType.Password, clientId)
    {
      ArgumentUtility.CheckForNull<SecureString>(clientSecret, nameof (clientSecret));
      this.m_clientSecret = clientSecret;
    }

    private static SecureString EncryptSecret(string value)
    {
      if (string.IsNullOrEmpty(value))
        return (SecureString) null;
      SecureString secureString = new SecureString();
      foreach (char c in value)
        secureString.AppendChar(c);
      secureString.MakeReadOnly();
      return secureString;
    }

    private static string DecryptSecret(SecureString value)
    {
      if (value == null || value.Length == 0)
        return (string) null;
      IntPtr num = IntPtr.Zero;
      try
      {
        num = Marshal.SecureStringToGlobalAllocUnicode(value);
        return Marshal.PtrToStringUni(num);
      }
      finally
      {
        if (num != IntPtr.Zero)
          Marshal.ZeroFreeGlobalAllocUnicode(num);
      }
    }

    protected override void SetParameters(IDictionary<string, string> parameters)
    {
      parameters["client_id"] = this.ClientId;
      parameters["client_secret"] = VssOAuthPasswordClientCredential.DecryptSecret(this.m_clientSecret);
    }
  }
}
