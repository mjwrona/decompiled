// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Service.Utilities.QueryParser.AccountEntitlementUserTypeFilter
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.VisualStudio.Services.Identity;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Licensing.Service.Utilities.QueryParser
{
  internal class AccountEntitlementUserTypeFilter : AccountEntitlementSinglePropertyFilter
  {
    public AccountEntitlementUserTypeFilter(ISet<IdentityMetaType> value)
      : base(AccountEntitlementFilterSinglePropertyType.UserType)
    {
      this.Value = value;
    }

    public ISet<IdentityMetaType> Value { get; private set; }
  }
}
