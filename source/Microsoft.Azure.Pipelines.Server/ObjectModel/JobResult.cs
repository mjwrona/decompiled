// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Server.ObjectModel.JobResult
// Assembly: Microsoft.Azure.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC20940E-746B-4985-9936-F8ACD7ADA1DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Server.dll

namespace Microsoft.Azure.Pipelines.Server.ObjectModel
{
  public enum JobResult
  {
    Succeeded = 1,
    Skipped = 2,
    Failed = 4,
    Canceled = 8,
    Rejected = 16, // 0x00000010
  }
}
