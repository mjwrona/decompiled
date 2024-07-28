// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Account.AccountNotFoundException
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Account
{
  [ExceptionMapping("0.0", "3.0", "AccountNotFoundException", "Microsoft.VisualStudio.Services.Account.AccountNotFoundException, Microsoft.VisualStudio.Services.WebApi, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class AccountNotFoundException : AccountException
  {
    public AccountNotFoundException()
      : base(AccountResources.AccountNotFound())
    {
      this.Data.Add((object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}", (object) true);
    }

    public AccountNotFoundException(Guid accountId)
      : this(accountId.ToString((string) null, (IFormatProvider) CultureInfo.InvariantCulture))
    {
    }

    public AccountNotFoundException(string accountId)
      : base(AccountResources.AccountNotFoundByIdError((object) accountId))
    {
    }

    public AccountNotFoundException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
