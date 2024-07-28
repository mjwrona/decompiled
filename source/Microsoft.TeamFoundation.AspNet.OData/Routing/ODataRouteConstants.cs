// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Routing.ODataRouteConstants
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

namespace Microsoft.AspNet.OData.Routing
{
  public static class ODataRouteConstants
  {
    public static readonly string ODataPath = "odataPath";
    public static readonly string ODataPathTemplate = "{*" + ODataRouteConstants.ODataPath + "}";
    public static readonly string ConstraintName = "ODataConstraint";
    public static readonly string VersionConstraintName = "ODataVersionConstraint";
    public static readonly string Action = "action";
    public static readonly string Controller = "controller";
    public static readonly string Key = "key";
    public static readonly string RelatedKey = "relatedKey";
    public static readonly string NavigationProperty = "navigationProperty";
    public static readonly string Batch = "$batch";
    public static readonly string DynamicProperty = "dynamicProperty";
    public static readonly string OptionalParameters = typeof (ODataOptionalParameter).FullName;
  }
}
