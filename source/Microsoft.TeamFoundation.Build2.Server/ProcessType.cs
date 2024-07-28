// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.ProcessType
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

namespace Microsoft.TeamFoundation.Build2.Server
{
  public static class ProcessType
  {
    public const int Designer = 1;
    public const int Yaml = 2;
    public const int Docker = 3;
    public const int JustInTime = 4;

    public static string GetName(int type)
    {
      switch (type)
      {
        case 2:
          return "Yaml";
        case 3:
          return "Docker";
        case 4:
          return "JustInTime";
        default:
          return "Designer";
      }
    }

    public static bool SupportsCustomRetentionPolicies(int type)
    {
      switch (type)
      {
        case 2:
          return false;
        case 3:
          return false;
        case 4:
          return false;
        default:
          return true;
      }
    }
  }
}
