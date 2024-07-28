// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Admin.GenericAccountInfo
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using System.Security;

namespace Microsoft.TeamFoundation.Admin
{
  public class GenericAccountInfo : AccountInfo
  {
    public GenericAccountInfo()
    {
    }

    public GenericAccountInfo(string userName)
      : base(userName)
    {
    }

    public GenericAccountInfo(string userName, SecureString password)
      : base(AccountInfo.GetAccountType(userName), userName, password)
    {
    }

    public GenericAccountInfo(AccountType accountType, string userName)
      : base(accountType, userName, (SecureString) null)
    {
    }

    public GenericAccountInfo(AccountType accountType, string userName, SecureString password)
      : base(accountType, userName, password)
    {
    }

    public GenericAccountInfo(
      AccountType accountType,
      string userName,
      SecureString password,
      bool requirePassword)
      : base(accountType, userName, password, requirePassword)
    {
    }

    public static GenericAccountInfo Create(string userName, SecureString password) => (AccountInfo.IsServiceAccountName(userName) ? 0 : (!AccountInfo.IsRemoteNetworkServiceAccount(userName) ? 1 : 0)) != 0 ? new GenericAccountInfo(userName, password) : new GenericAccountInfo(AccountInfo.GetAccountType(userName), userName);
  }
}
