// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.CredentialsProviderHelper
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.ComponentModel;
using System.Net;
using System.Text;

namespace Microsoft.VisualStudio.Services.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class CredentialsProviderHelper
  {
    public static string FailedUserName(Uri uri, ICredentials failedCredentials)
    {
      string str = string.Empty;
      if (failedCredentials != null)
      {
        NetworkCredential credential = failedCredentials.GetCredential(uri, (string) null);
        if (credential != null)
        {
          str = credential.Domain.Length <= 0 || credential.UserName.Length <= 0 ? credential.UserName : credential.Domain + "\\" + credential.UserName;
          if (str.Length > 513)
            str = str.Substring(0, 513);
        }
      }
      return str;
    }

    public static NetworkCredential GetNetworkCredential(string username, StringBuilder password)
    {
      StringBuilder pszUser = new StringBuilder(string.Empty, 513);
      StringBuilder pszDomain = new StringBuilder(string.Empty, 513);
      int userName = Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.CredUIParseUserName(username, pszUser, (uint) (pszUser.Capacity + 1), pszDomain, (uint) (pszDomain.Capacity + 1));
      string str = password.ToString();
      try
      {
        return userName == 0 ? new NetworkCredential(pszUser.ToString(), str, pszDomain.ToString()) : new NetworkCredential(username, str);
      }
      finally
      {
        CredentialsProviderHelper.ZeroString(str);
      }
    }

    public static void ZeroStringBuilder(StringBuilder toZero)
    {
      if (toZero == null)
        return;
      toZero.Length = 0;
      int capacity = toZero.Capacity;
      for (int index = 0; index < capacity; ++index)
        toZero.Append(char.MinValue);
      toZero.Length = 0;
    }

    public static unsafe void ZeroString(string toZero)
    {
      fixed (char* chPtr = toZero)
      {
        int length = toZero.Length;
        for (int index = 0; index < length; ++index)
          chPtr[index] = char.MinValue;
      }
    }
  }
}
