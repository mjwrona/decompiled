// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.OrgPipelineReleaseSettingsHelper
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data
{
  [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Correct Spelling")]
  public class OrgPipelineReleaseSettingsHelper
  {
    public OrgPipelineReleaseSettingsHelper(IVssRequestContext requestContext)
    {
      PipelineSettingsSecurityProvider.CheckViewCollectionPermission(requestContext);
      this.HasEditPermission = PipelineSettingsSecurityProvider.HasManageCollectionPermission(requestContext);
      this.LoadSettings(requestContext);
    }

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Correct Spelling")]
    public void UpdateSettings(IVssRequestContext requestContext, bool enforceJobAuthScope)
    {
      if (!PipelineSettingsSecurityProvider.HasManageCollectionPermission(requestContext))
        throw new UnauthorizedAccessException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.AccessDeniedMessage, (object) requestContext.GetUserIdentity().DisplayName));
      this.LoadSettings(requestContext);
      if (this.OrgEnforceJobAuthScope == enforceJobAuthScope)
        return;
      requestContext.GetService<IVssRegistryService>().SetValue<bool>(requestContext, "/Service/ReleaseManagement/Settings/OrgSettings/EnforceJobAuthScope", enforceJobAuthScope);
      this.OrgEnforceJobAuthScope = enforceJobAuthScope;
    }

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Correct Spelling")]
    private void LoadSettings(IVssRequestContext requestContext) => this.OrgEnforceJobAuthScope = requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, (RegistryQuery) "/Service/ReleaseManagement/Settings/OrgSettings/EnforceJobAuthScope", false);

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Auth", Justification = "Correct term")]
    public bool OrgEnforceJobAuthScope { get; private set; }

    public bool HasEditPermission { get; }
  }
}
