// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Extensions.Constants
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Extensions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F602B8A6-9B4B-4971-8764-E3FEAFAB8CD5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Extensions.dll

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Extensions
{
  public static class Constants
  {
    public const string OperatorAnd = "&&";
    public const string OperatorOr = "||";
    public const string VisibilityRuleRegEx = "([a-zA-Z0-9 ]+)([!=<>]+)([a-zA-Z0-9. ]+)|([a-zA-Z0-9 ]+(?=NotContains|NotEndsWith|NotStartsWith))(NotContains|NotEndsWith|NotStartsWith)([a-zA-Z0-9. ]+)|([a-zA-Z0-9 ]+(?=Contains|EndsWith|StartsWith))(Contains|EndsWith|StartsWith)([a-zA-Z0-9. ]+)";
    public const string Assignment = "=";
    public const string EqualsTo = "==";
    public const string NotEquals = "!=";
    public const string LessThan = "<";
    public const string GreaterThan = ">";
    public const string LessThanOrEqualTo = "<=";
    public const string GreaterThanOrEqualTo = ">=";
    public const string Contains = "Contains";
    public const string StartsWith = "StartsWith";
    public const string EndsWith = "EndsWith";
    public const string NotContains = "NotContains";
    public const string NotStartsWith = "NotStartsWith";
    public const string NotEndsWith = "NotEndsWith";
    public const string VisibleRule = "visibleRule";
  }
}
