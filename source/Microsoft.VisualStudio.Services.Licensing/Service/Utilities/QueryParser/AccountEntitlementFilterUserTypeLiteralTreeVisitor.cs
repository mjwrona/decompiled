// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Service.Utilities.QueryParser.AccountEntitlementFilterUserTypeLiteralTreeVisitor
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.OData.UriParser;
using Microsoft.VisualStudio.Services.Graph;
using Microsoft.VisualStudio.Services.Identity;

namespace Microsoft.VisualStudio.Services.Licensing.Service.Utilities.QueryParser
{
  internal class AccountEntitlementFilterUserTypeLiteralTreeVisitor : 
    AccountEntitlementFilterQuerySyntacticTreeVisitorBase<IdentityMetaType?>
  {
    public override IdentityMetaType? Visit(LiteralToken tokenIn)
    {
      if (tokenIn.Value is string)
      {
        IdentityMetaType? userType = new IdentityMetaType?(AccountEntitlementFilterUserTypeLiteralTreeVisitor.ConvertToIdentityMetaType((string) tokenIn.Value));
        if (this.IsIdentityTypeAllowed(userType))
          return userType;
      }
      return new IdentityMetaType?();
    }

    private static IdentityMetaType ConvertToIdentityMetaType(string metaType)
    {
      try
      {
        return GraphObjectExtensionHelpers.ConvertToIdentityMetaType(metaType);
      }
      catch
      {
        return IdentityMetaType.Unknown;
      }
    }

    private bool IsIdentityTypeAllowed(IdentityMetaType? userType)
    {
      IdentityMetaType? nullable1 = userType;
      IdentityMetaType identityMetaType1 = IdentityMetaType.Guest;
      if (!(nullable1.GetValueOrDefault() == identityMetaType1 & nullable1.HasValue))
      {
        IdentityMetaType? nullable2 = userType;
        IdentityMetaType identityMetaType2 = IdentityMetaType.Member;
        if (!(nullable2.GetValueOrDefault() == identityMetaType2 & nullable2.HasValue))
        {
          IdentityMetaType? nullable3 = userType;
          IdentityMetaType identityMetaType3 = IdentityMetaType.Application;
          if (!(nullable3.GetValueOrDefault() == identityMetaType3 & nullable3.HasValue))
          {
            IdentityMetaType? nullable4 = userType;
            IdentityMetaType identityMetaType4 = IdentityMetaType.ManagedIdentity;
            if (!(nullable4.GetValueOrDefault() == identityMetaType4 & nullable4.HasValue))
            {
              IdentityMetaType? nullable5 = userType;
              IdentityMetaType identityMetaType5 = IdentityMetaType.Unknown;
              return nullable5.GetValueOrDefault() == identityMetaType5 & nullable5.HasValue;
            }
          }
        }
      }
      return true;
    }
  }
}
