// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.QueryOperators
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using System;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess
{
  public static class QueryOperators
  {
    public const string Equal = "eq";
    public const string GreaterThan = "gt";
    public const string GreaterThanOrEqual = "ge";
    public const string LessThan = "lt";
    public const string LessThanOrEqual = "le";
    public const string NotEqual = "ne";

    public static bool Operate(string property, string operation, string propertyValue)
    {
      switch (operation)
      {
        case "eq":
          return property == propertyValue;
        case "ne":
          return property != propertyValue;
        default:
          throw new NotImplementedException();
      }
    }
  }
}
