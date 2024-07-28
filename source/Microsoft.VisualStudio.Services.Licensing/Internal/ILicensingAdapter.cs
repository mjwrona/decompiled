// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Internal.ILicensingAdapter
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Licensing.Internal
{
  public interface ILicensingAdapter
  {
    void Start(
      IVssRequestContext requestContext,
      LicensingServiceConfiguration serviceSettings,
      IVssDateTimeProvider dateTimeProvider);

    void Stop(IVssRequestContext requestContext);

    IList<IUsageRight> GetRights(
      IVssRequestContext requestContext,
      IRightsQueryContext queryContext);
  }
}
