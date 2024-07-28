// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.FieldFlags
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  [Flags]
  public enum FieldFlags
  {
    None = 0,
    Sortable = 1,
    Computed = 2,
    Ignored = 8,
    Queryable = 16, // 0x00000010
    Reportable = 32, // 0x00000020
    PersonField = 64, // 0x00000040
    Cloneable = 128, // 0x00000080
    LongText = 256, // 0x00000100
    SupportsTextQuery = 512, // 0x00000200
  }
}
