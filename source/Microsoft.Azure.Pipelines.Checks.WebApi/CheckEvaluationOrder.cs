// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.WebApi.CheckEvaluationOrder
// Assembly: Microsoft.Azure.Pipelines.Checks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 381241F9-9196-42AF-BB4C-5187E3EFE32E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.WebApi.dll

namespace Microsoft.Azure.Pipelines.Checks.WebApi
{
  public enum CheckEvaluationOrder
  {
    System = 0,
    SanityChecks = 10, // 0x0000000A
    PreChecks = 20, // 0x00000014
    Main = 30, // 0x0000001E
    PostChecks = 40, // 0x00000028
    Final = 50, // 0x00000032
  }
}
