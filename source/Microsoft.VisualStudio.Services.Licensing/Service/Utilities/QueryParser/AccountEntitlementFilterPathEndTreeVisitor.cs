// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Service.Utilities.QueryParser.AccountEntitlementFilterPathEndTreeVisitor
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.OData.UriParser;

namespace Microsoft.VisualStudio.Services.Licensing.Service.Utilities.QueryParser
{
  internal class AccountEntitlementFilterPathEndTreeVisitor : 
    AccountEntitlementFilterQuerySyntacticTreeVisitorBase<string>
  {
    public override string Visit(EndPathToken tokenIn)
    {
      string identifier = ((PathToken) tokenIn).Identifier;
      return identifier == "licenseId" || identifier == "licenseStatus" || identifier == "name" || identifier == "userType" || identifier == "assignmentSource" ? ((PathToken) tokenIn).Identifier : (string) null;
    }
  }
}
