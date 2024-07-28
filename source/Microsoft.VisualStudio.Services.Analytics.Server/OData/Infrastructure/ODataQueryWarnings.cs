// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.ODataQueryWarnings
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  [Flags]
  internal enum ODataQueryWarnings
  {
    None = 0,
    NoSelectOrApply = 1,
    ParentChildRelations = 2,
    WideSelectOrApply = 4,
    DistinctPropertyInGroupBy = 8,
    FilteringByTagNames = 16, // 0x00000010
  }
}
