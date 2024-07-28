// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Server.AgileConstants
// Assembly: Microsoft.TeamFoundation.Agile.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4912F51-3FCA-4D2B-A7B5-CF15E2F3B46B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Server.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.Agile.Server
{
  [GenerateSpecificConstants(null)]
  public static class AgileConstants
  {
    public const string AgileServiceLayer = "AgileService";
    public const string AgileServiceArea = "AgileService";
    public const int MaxFieldIdLength = 256;
    public const int MaxRefNameLength = 256;
    public const int MaxBacklogNameLength = 256;
    public const int InProcReconcileTimeoutInMilliseconds = 60000;
  }
}
