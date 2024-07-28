// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExternalEvent.ExternalGitPushConstants
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ExternalEvent
{
  public static class ExternalGitPushConstants
  {
    public const string NoCiMessage = "***NO_CI***";
    public const string HasNoCiPropertyId = "HasNoCi";
    public const string HasCommitsPropertyId = "HasCommits";

    public static IReadOnlyList<string> SkipCICheckInKeywords => (IReadOnlyList<string>) new List<string>()
    {
      "***no_ci***",
      "[skip ci]",
      "[ci skip]",
      "skip-checks: true",
      "skip-checks:true",
      "[skip azp]",
      "[azp skip]",
      "[skip azpipelines]",
      "[azpipelines skip]",
      "[skip azurepipelines]",
      "[azurepipelines skip]"
    };
  }
}
