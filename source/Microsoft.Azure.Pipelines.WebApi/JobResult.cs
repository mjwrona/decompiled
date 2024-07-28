// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebApi.JobResult
// Assembly: Microsoft.Azure.Pipelines.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9955A178-37CB-46CB-B455-32EA2A66C5BA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebApi.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.Pipelines.WebApi
{
  [JsonConverter(typeof (UnknownEnumJsonConverter))]
  public enum JobResult
  {
    Unknown = 0,
    Succeeded = 1,
    Skipped = 2,
    Failed = 4,
    Canceled = 8,
    Rejected = 16, // 0x00000010
  }
}
