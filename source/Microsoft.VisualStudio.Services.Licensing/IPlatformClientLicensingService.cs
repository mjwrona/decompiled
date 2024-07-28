// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.IPlatformClientLicensingService
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Licensing.Internal;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Licensing
{
  [DefaultServiceImplementation(typeof (PlatformClientLicensingService))]
  public interface IPlatformClientLicensingService : IVssFrameworkService
  {
    ClientReleaseType GetClientReleaseType(
      IVssRequestContext deploymentContext,
      Version version,
      string buildLab);

    DateTimeOffset GetClientReleaseExpirationDate(
      IVssRequestContext deploymentContext,
      IRightsQueryContext queryContext,
      DateTimeOffset defaultExpirationDate,
      ClientReleaseType releaseType);

    List<string> AddOrUpdateVisualStudioRelease(
      IVssRequestContext deploymentContext,
      ClientRelease newRelease,
      bool force);

    void RemoveVisualStudioRelease(IVssRequestContext deploymentContext, string name);
  }
}
