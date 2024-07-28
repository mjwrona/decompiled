// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Server.JobConstants
// Assembly: Microsoft.Azure.Pipelines.Checks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D08C7285-654E-4A4D-BA46-748F0D1E96AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Server.dll

namespace Microsoft.Azure.Pipelines.Checks.Server
{
  public static class JobConstants
  {
    public static string ExpirationJobName = "Expiration Job";
    public static string TimeoutJobName = "Checks Timeout Job";
    public static string CheckReEvaluationJobExtension = "Microsoft.Azure.Pipelines.Checks.Server.Plugins.Jobs.CheckReEvaluationJob";
    public static string CheckTimeoutJobExtension = "Microsoft.Azure.Pipelines.Checks.Server.Plugins.Jobs.CheckTimeoutJob";
  }
}
