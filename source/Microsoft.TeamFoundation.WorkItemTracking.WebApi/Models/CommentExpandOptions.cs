// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.CommentExpandOptions
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FA6C797-B300-46B2-A8C9-CFED891348F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.WebApi.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models
{
  [Flags]
  public enum CommentExpandOptions
  {
    None = 0,
    Reactions = 1,
    RenderedText = 8,
    RenderedTextOnly = 16, // 0x00000010
    All = -17, // 0xFFFFFFEF
  }
}
