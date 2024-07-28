// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Infrastructure.DataProtectionProviderProtectedData
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.Owin.Security.DataProtection;
using System;
using System.Text;

namespace Microsoft.AspNet.SignalR.Infrastructure
{
  public class DataProtectionProviderProtectedData : IProtectedData
  {
    private static readonly UTF8Encoding _encoding = new UTF8Encoding(false, true);
    private readonly IDataProtectionProvider _provider;
    private readonly IDataProtector _connectionTokenProtector;
    private readonly IDataProtector _groupsProtector;

    public DataProtectionProviderProtectedData(IDataProtectionProvider provider)
    {
      this._provider = provider != null ? provider : throw new ArgumentNullException(nameof (provider));
      this._connectionTokenProtector = provider.Create("SignalR.ConnectionToken");
      this._groupsProtector = provider.Create("SignalR.Groups.v1.1");
    }

    public string Protect(string data, string purpose) => Convert.ToBase64String(this.GetDataProtector(purpose).Protect(DataProtectionProviderProtectedData._encoding.GetBytes(data)));

    public string Unprotect(string protectedValue, string purpose)
    {
      byte[] bytes = this.GetDataProtector(purpose).Unprotect(Convert.FromBase64String(protectedValue));
      return DataProtectionProviderProtectedData._encoding.GetString(bytes);
    }

    private IDataProtector GetDataProtector(string purpose)
    {
      switch (purpose)
      {
        case "SignalR.ConnectionToken":
          return this._connectionTokenProtector;
        case "SignalR.Groups.v1.1":
          return this._groupsProtector;
        default:
          return this._provider.Create(purpose);
      }
    }
  }
}
