// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.AllowedQueryOptions
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using System;

namespace Microsoft.AspNet.OData.Query
{
  [Flags]
  public enum AllowedQueryOptions
  {
    None = 0,
    Filter = 1,
    Expand = 2,
    Select = 4,
    OrderBy = 8,
    Top = 16, // 0x00000010
    Skip = 32, // 0x00000020
    Count = 64, // 0x00000040
    Format = 128, // 0x00000080
    SkipToken = 256, // 0x00000100
    DeltaToken = 512, // 0x00000200
    Apply = 1024, // 0x00000400
    Compute = 2048, // 0x00000800
    Supported = Compute | Apply | SkipToken | Format | Count | Skip | Top | OrderBy | Select | Expand | Filter, // 0x00000DFF
    All = Supported | DeltaToken, // 0x00000FFF
  }
}
