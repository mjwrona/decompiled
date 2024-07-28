// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.AllowedArithmeticOperators
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using System;

namespace Microsoft.AspNet.OData.Query
{
  [Flags]
  public enum AllowedArithmeticOperators
  {
    None = 0,
    Add = 1,
    Subtract = 2,
    Multiply = 4,
    Divide = 8,
    Modulo = 16, // 0x00000010
    All = Modulo | Divide | Multiply | Subtract | Add, // 0x0000001F
  }
}
