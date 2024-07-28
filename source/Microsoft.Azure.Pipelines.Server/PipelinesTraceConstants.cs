// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Server.PipelinesTraceConstants
// Assembly: Microsoft.Azure.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC20940E-746B-4985-9936-F8ACD7ADA1DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Server.dll

namespace Microsoft.Azure.Pipelines.Server
{
  public static class PipelinesTraceConstants
  {
    public const string Area = "Pipelines";
    public const int SlowCalls = 2000000;
    public const int LeakingSecret = 2000001;
    public const int SignalRLive = 2000002;

    public class TfsPipelines
    {
      public const int CreateTeamProject = 15287205;
      public const int Operations = 15287210;
    }
  }
}
