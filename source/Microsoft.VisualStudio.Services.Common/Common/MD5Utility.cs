// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.MD5Utility
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.ComponentModel;
using System.Security.Cryptography;

namespace Microsoft.VisualStudio.Services.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal static class MD5Utility
  {
    private static int s_fipsAlgorithmPolicyEnabled = -1;
    private const int c_enabled = 1;
    private const int c_disabled = 0;
    private const int c_unknown = -1;

    public static MD5 TryCreateMD5Provider()
    {
      MD5 md5Provider = (MD5) null;
      if (MD5Utility.s_fipsAlgorithmPolicyEnabled == 0 || MD5Utility.s_fipsAlgorithmPolicyEnabled == -1)
      {
        if (Environment.GetEnvironmentVariable("DD_SUITES_FIPS") != null)
        {
          MD5Utility.s_fipsAlgorithmPolicyEnabled = 1;
        }
        else
        {
          try
          {
            md5Provider = (MD5) new MD5CryptoServiceProvider();
            MD5Utility.s_fipsAlgorithmPolicyEnabled = 0;
          }
          catch (InvalidOperationException ex)
          {
            MD5Utility.s_fipsAlgorithmPolicyEnabled = 1;
          }
        }
      }
      return md5Provider;
    }
  }
}
