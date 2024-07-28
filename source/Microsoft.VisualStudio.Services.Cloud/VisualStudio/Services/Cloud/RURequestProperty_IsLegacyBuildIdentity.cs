// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.RURequestProperty_IsLegacyBuildIdentity
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class RURequestProperty_IsLegacyBuildIdentity : RURequestProperty_Bool
  {
    private const string c_legacyBuildIdentity = "LegacyBuildIdentity";
    private static readonly string[] s_acsBuildUserAgentPrefixes = new string[3]
    {
      "Team Foundation (TFSBuildServiceHost.exe",
      "Team Foundation (QTController.exe",
      "Team Foundation (MSBuild.exe"
    };
    private static readonly string[] s_acsBuildUserAgentSuffixes = new string[1]
    {
      "(TFSBuildServiceHost.exe)"
    };

    public override bool ShouldOutputEntityToTelemetry { get; protected set; }

    public override object GetRequestValue(IVssRequestContext requestContext)
    {
      bool requestValue;
      if (!requestContext.RootContext.TryGetItem<bool>("LegacyBuildIdentity", out requestValue))
      {
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        requestValue = userIdentity != null && StringComparer.OrdinalIgnoreCase.Equals(userIdentity.GetProperty<string>("Domain", string.Empty), "LOCAL AUTHORITY") && !string.IsNullOrEmpty(requestContext.UserAgent) && (((IEnumerable<string>) RURequestProperty_IsLegacyBuildIdentity.s_acsBuildUserAgentPrefixes).Any<string>((Func<string, bool>) (buildUserAgentPrefix => requestContext.UserAgent.StartsWith(buildUserAgentPrefix, StringComparison.OrdinalIgnoreCase))) || ((IEnumerable<string>) RURequestProperty_IsLegacyBuildIdentity.s_acsBuildUserAgentSuffixes).Any<string>((Func<string, bool>) (buildUserAgentSuffix => requestContext.UserAgent.EndsWith(buildUserAgentSuffix, StringComparison.OrdinalIgnoreCase))));
        requestContext.RootContext.Items["LegacyBuildIdentity"] = (object) requestValue;
      }
      return (object) requestValue;
    }
  }
}
