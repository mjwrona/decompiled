// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.DefinitionMetrics
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [GenerateAllConstants(null)]
  public static class DefinitionMetrics
  {
    public const string SuccessfulBuilds = "SuccessfulBuilds";
    public const string FailedBuilds = "FailedBuilds";
    public const string PartiallySuccessfulBuilds = "PartiallySuccessfulBuilds";
    public const string CanceledBuilds = "CanceledBuilds";
    public const string TotalBuilds = "TotalBuilds";
    public const string CurrentBuildsInQueue = "CurrentBuildsInQueue";
    public const string CurrentBuildsInProgress = "CurrentBuildsInProgress";
  }
}
