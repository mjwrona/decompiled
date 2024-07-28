// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Approval.WebApi.ApprovalPermissions
// Assembly: Microsoft.Azure.Pipelines.Approval.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CD73FA2-1519-4C73-96D3-90013EEE8275
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Approval.WebApi.dll

using System;

namespace Microsoft.Azure.Pipelines.Approval.WebApi
{
  [Flags]
  public enum ApprovalPermissions
  {
    None = 0,
    View = 1,
    Update = 2,
    Reassign = 4,
    ResourceAdmin = 8,
    QueueBuild = 16, // 0x00000010
  }
}
