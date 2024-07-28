// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.ResourceLimitUtil
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using System;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal static class ResourceLimitUtil
  {
    public const string DefaultThrottlingType = "DEFAULTPARALLELISM";
    private const string DefaultThrottlingTypePretty = "Default";
    private const string Suffix = "PARALLELISM";
    private const string MicrosoftHosted = "MICROSOFTHOSTED";
    private const string MicrosoftHostedPretty = "Microsoft-Hosted";
    private const string SelfHosted = "SELFHOSTED";
    private const string SelfHostedPretty = "Self-Hosted";

    internal static string GetResourceThrottlingType(string parallelismTag, bool isHosted)
    {
      string str = isHosted ? "MICROSOFTHOSTED" : "SELFHOSTED";
      if (string.Equals(parallelismTag, "Public", StringComparison.InvariantCultureIgnoreCase))
        return (str + "PublicPARALLELISM").ToUpperInvariant();
      return string.Equals(parallelismTag, "Private", StringComparison.InvariantCultureIgnoreCase) ? (str + "PrivatePARALLELISM").ToUpperInvariant() : "DEFAULTPARALLELISM";
    }

    internal static string GetPrettyResourceThrottlingType(string parallelismTag, bool isHosted)
    {
      string str = isHosted ? "Microsoft-Hosted" : "Self-Hosted";
      if (string.Equals(parallelismTag, "Public", StringComparison.InvariantCultureIgnoreCase))
        return str + " Public";
      return string.Equals(parallelismTag, "Private", StringComparison.InvariantCultureIgnoreCase) ? str + " Private" : "Default";
    }
  }
}
