// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.TaskConditions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  public static class TaskConditions
  {
    public const string Succeeded = "succeeded()";
    public const string SucceededOrFailed = "succeededOrFailed()";
    public const string Failed = "failed()";
    public const string Always = "always()";
    private static readonly HashSet<string> StandardConditions = new HashSet<string>((IEqualityComparer<string>) StringComparer.Ordinal)
    {
      "succeeded()",
      "succeededOrFailed()",
      "failed()",
      "always()"
    };

    public static bool IsLegacyAlwaysRun(string condition) => string.Equals(condition, "succeededOrFailed()", StringComparison.Ordinal);

    public static bool RequiresAgentSupport(string condition) => !string.IsNullOrEmpty(condition) && !string.Equals(condition, "succeeded()", StringComparison.Ordinal) && !string.Equals(condition, "succeededOrFailed()", StringComparison.Ordinal);

    public static bool IsCustomCondition(string condition) => !string.IsNullOrWhiteSpace(condition) && !TaskConditions.StandardConditions.Contains(condition);
  }
}
