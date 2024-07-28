// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Service.Utilities.QueryParser.AccountEntitlementFilterLicenseStatusLiteralTreeVisitor
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.OData.UriParser;
using Microsoft.VisualStudio.Services.Account;
using System;

namespace Microsoft.VisualStudio.Services.Licensing.Service.Utilities.QueryParser
{
  internal class AccountEntitlementFilterLicenseStatusLiteralTreeVisitor : 
    AccountEntitlementFilterQuerySyntacticTreeVisitorBase<AccountUserStatus?>
  {
    public override AccountUserStatus? Visit(LiteralToken tokenIn)
    {
      AccountUserStatus result;
      return tokenIn.Value is string && Enum.TryParse<AccountUserStatus>((string) tokenIn.Value, true, out result) && result == AccountUserStatus.Disabled ? new AccountUserStatus?(result) : new AccountUserStatus?();
    }
  }
}
