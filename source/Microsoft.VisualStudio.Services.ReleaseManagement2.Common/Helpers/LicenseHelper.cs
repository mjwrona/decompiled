// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers.LicenseHelper
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C3F75541-7C8A-4AF6-A47E-709CEEE7550D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers
{
  public static class LicenseHelper
  {
    public static bool HasNoneOrStakeholderLicense(this IVssRequestContext requestContext)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      bool? nullable = requestContext.To(TeamFoundationHostType.Deployment).Elevate().GetExtension<IStakeholderLicenseAdapter>(ExtensionLifetime.Service).HasStakeholderRights(requestContext, requestContext.RootContext.UserContext);
      return nullable.HasValue && nullable.HasValue && nullable.Value;
    }
  }
}
