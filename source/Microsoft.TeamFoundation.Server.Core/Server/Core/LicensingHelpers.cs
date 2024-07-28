// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.LicensingHelpers
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Authorization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core
{
  public static class LicensingHelpers
  {
    private static readonly string s_licenseIdentityType = "System:" + "License";
    private static readonly string s_hostedStakeholderIdentifier = HostedLicenseName.Stakeholder.ToString("D");
    private static readonly string s_onPremStakeholderIdentifier = OnPremLicenseName.Limited.ToString("D");
    private const string c_area = "Licensing";
    private const string c_layer = "LicensingHelpers";

    public static bool IsStakeholder(this IVssRequestContext requestContext)
    {
      IReadOnlyList<IRequestActor> actors = requestContext.RequestContextInternal().Actors;
      IRequestActor requestActor = actors != null ? actors.LastOrDefault<IRequestActor>() : (IRequestActor) null;
      bool flag = false;
      EvaluationPrincipal principal;
      if (requestActor != null && requestActor.TryGetPrincipal(SubjectType.License, out principal) && StringComparer.Ordinal.Equals(principal.PrimaryDescriptor.IdentityType, LicensingHelpers.s_licenseIdentityType))
        flag = !requestContext.ExecutionEnvironment.IsHostedDeployment ? StringComparer.OrdinalIgnoreCase.Equals(principal.PrimaryDescriptor.Identifier, LicensingHelpers.s_onPremStakeholderIdentifier) : StringComparer.OrdinalIgnoreCase.Equals(principal.PrimaryDescriptor.Identifier, LicensingHelpers.s_hostedStakeholderIdentifier);
      if (flag)
        requestContext.Trace(66511, TraceLevel.Info, "Licensing", nameof (LicensingHelpers), "LicensingHelpers.IsStakeholder: Returning true");
      else
        requestContext.Trace(66511, TraceLevel.Info, "Licensing", nameof (LicensingHelpers), "LicensingHelpers.IsStakeholder: Returning false");
      return flag;
    }
  }
}
