// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.AllowedLogicalOperators
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using System;

namespace Microsoft.AspNet.OData.Query
{
  [Flags]
  public enum AllowedLogicalOperators
  {
    None = 0,
    Or = 1,
    And = 2,
    Equal = 4,
    NotEqual = 8,
    GreaterThan = 16, // 0x00000010
    GreaterThanOrEqual = 32, // 0x00000020
    LessThan = 64, // 0x00000040
    LessThanOrEqual = 128, // 0x00000080
    Not = 256, // 0x00000100
    Has = 512, // 0x00000200
    All = Has | Not | LessThanOrEqual | LessThan | GreaterThanOrEqual | GreaterThan | NotEqual | Equal | And | Or, // 0x000003FF
  }
}
