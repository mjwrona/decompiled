// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Account.AccountNameValidator
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using System;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Account
{
  public class AccountNameValidator
  {
    internal static void ValidateAccountNameCharacters(string accountName)
    {
      foreach (char ch in accountName)
      {
        if (ch != '-' && (ch < 'a' || ch > 'z') && (ch < 'A' || ch > 'Z') && (ch < '0' || ch > '9'))
          throw new AccountPropertyException(AccountResources.AccountNameForbiddenCharacters());
      }
      if (accountName.First<char>() == '-' || accountName.Last<char>() == '-')
        throw new AccountPropertyException(AccountResources.AccountNameForbiddenBeginOrEndDash());
    }

    internal static void ValidateAccountNameNonInternal(string accountName)
    {
      if (accountName.Length == 37 && (accountName[0].Equals('A') || accountName[0].Equals('a')) && Guid.TryParse(accountName.Substring(1), out Guid _))
        throw new AccountPropertyException(AccountResources.AccountNameReserved());
    }

    internal static void ValidateAccountNameLength(string accountName)
    {
      if (accountName == null || accountName.Length == 0)
        throw new AccountPropertyException(TFCommonResources.EmptyStringNotAllowed());
      if (accountName.Length > 50)
        throw new AccountPropertyException(AccountResources.AccountNameTooLong((object) 50));
    }

    internal static void ValidateAccountNameNotGuid(string accountName)
    {
      if (Guid.TryParse(accountName, out Guid _))
        throw new AccountPropertyException(AccountResources.AccountNameForbiddenGuid((object) accountName));
    }

    public static void ValidateAccountName(string accountName)
    {
      AccountNameValidator.ValidateAccountNameLength(accountName);
      AccountNameValidator.ValidateAccountNameCharacters(accountName);
      AccountNameValidator.ValidateAccountNameNonInternal(accountName);
      AccountNameValidator.ValidateAccountNameNotGuid(accountName);
    }
  }
}
