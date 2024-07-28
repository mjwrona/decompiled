// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.GetWorkItemTypeExpand
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 141BFF87-1CDC-4DC5-9A7C-F7D6EA031F34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models
{
  [Flags]
  public enum GetWorkItemTypeExpand
  {
    None = 0,
    States = 1,
    Behaviors = 2,
    Layout = 4,
  }
}
