// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Routing.ODataSegmentKinds
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

namespace Microsoft.AspNet.OData.Routing
{
  public static class ODataSegmentKinds
  {
    public const string ServiceBase = "~";
    public const string Batch = "$batch";
    public const string Ref = "$ref";
    public const string Metadata = "$metadata";
    public const string Value = "$value";
    public const string Count = "$count";
    public const string Action = "action";
    public const string Function = "function";
    public const string UnboundAction = "unboundaction";
    public const string UnboundFunction = "unboundfunction";
    public const string Cast = "cast";
    public const string EntitySet = "entityset";
    public const string Singleton = "singleton";
    public const string Key = "key";
    public const string Navigation = "navigation";
    public const string PathTemplate = "template";
    public const string Property = "property";
    public const string DynamicProperty = "dynamicproperty";
    public const string Unresolved = "unresolved";
  }
}
