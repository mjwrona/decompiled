// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.ILicensingService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Licensing
{
  [DefaultServiceImplementation(typeof (FrameworkLicensingService))]
  public interface ILicensingService : IVssFrameworkService
  {
    ClientRightsContainer GetClientRightsContainer(
      IVssRequestContext requestContext,
      ClientRightsQueryContext queryContext,
      ClientRightsTelemetryContext telemetryContext = null);

    IList<MsdnEntitlement> GetMsdnEntitlements(IVssRequestContext requestContext);

    IDictionary<string, bool> GetExtensionRights(
      IVssRequestContext requestContext,
      IEnumerable<string> extensionIds);
  }
}
