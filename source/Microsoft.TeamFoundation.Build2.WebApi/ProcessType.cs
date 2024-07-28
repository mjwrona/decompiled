// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.ProcessType
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [GenerateAllConstants(null)]
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
  }
}
