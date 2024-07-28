// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Infrastructure.DefaultProtectedData
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.AspNet.SignalR.Infrastructure
{
  public class DefaultProtectedData : IProtectedData
  {
    private static readonly UTF8Encoding _encoding = new UTF8Encoding(false, true);

    public string Protect(string data, string purpose)
    {
      byte[] bytes = DefaultProtectedData._encoding.GetBytes(purpose);
      return Convert.ToBase64String(ProtectedData.Protect(DefaultProtectedData._encoding.GetBytes(data), bytes, DataProtectionScope.CurrentUser));
    }

    public string Unprotect(string protectedValue, string purpose)
    {
      byte[] bytes1 = DefaultProtectedData._encoding.GetBytes(purpose);
      byte[] bytes2 = ProtectedData.Unprotect(Convert.FromBase64String(protectedValue), bytes1, DataProtectionScope.CurrentUser);
      return DefaultProtectedData._encoding.GetString(bytes2);
    }
  }
}
