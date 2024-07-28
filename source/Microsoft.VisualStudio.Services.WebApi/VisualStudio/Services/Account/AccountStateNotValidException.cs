// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Account.AccountStateNotValidException
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Account
{
  [ExceptionMapping("0.0", "3.0", "AccountStateNotValidException", "Microsoft.VisualStudio.Services.Account.AccountStateNotValidException, Microsoft.VisualStudio.Services.WebApi, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class AccountStateNotValidException : AccountException
  {
    public AccountStateNotValidException(string message)
      : base(message)
    {
    }

    public AccountStateNotValidException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
