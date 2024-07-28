// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IInstanceManagementServiceExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class IInstanceManagementServiceExtensions
  {
    public static bool IsRegisteredServiceDomain(
      this IInstanceManagementService instanceManagementService,
      IVssRequestContext requestContext,
      string domain)
    {
      requestContext.CheckDeploymentRequestContext();
      IList<string> registeredServiceDomains = instanceManagementService.GetRegisteredServiceDomains(requestContext);
      return registeredServiceDomains != null && registeredServiceDomains.Any<string>((Func<string, bool>) (registeredDomain => VssStringComparer.DomainName.Equals(domain, registeredDomain) || VssStringComparer.DomainName.EndsWith(domain, "." + registeredDomain)));
    }
  }
}
