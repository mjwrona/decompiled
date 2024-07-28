// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.ProjectPipelineReleaseSettingsHelper
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
  public class ProjectPipelineReleaseSettingsHelper
  {
    private Guid projectId;

    public ProjectPipelineReleaseSettingsHelper(IVssRequestContext requestContext, Guid projectId)
    {
      this.projectId = projectId;
      PipelineSettingsSecurityProvider.CheckViewProjectPermission(requestContext, projectId);
      this.HasEditPermission = PipelineSettingsSecurityProvider.HasEditProjectPermission(requestContext, projectId);
      this.LoadSettings(requestContext);
    }

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Correct Spelling")]
    public void UpdateSettings(IVssRequestContext requestContext, bool enforceJobAuthScope)
    {
      if (!PipelineSettingsSecurityProvider.HasEditProjectPermission(requestContext, this.projectId))
        throw new UnauthorizedAccessException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.AccessDeniedMessage, (object) requestContext.GetUserIdentity().DisplayName));
      this.LoadSettings(requestContext);
      if (this.EnforceJobAuthScope == enforceJobAuthScope)
        return;
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string scopeRegistryKey = ProjectPipelineReleaseSettingsHelper.GetProjectScopeRegistryKey(this.projectId);
      IVssRequestContext requestContext1 = requestContext;
      string path = scopeRegistryKey;
      int num = enforceJobAuthScope ? 1 : 0;
      service.SetValue<bool>(requestContext1, path, num != 0);
      this.EnforceJobAuthScope = enforceJobAuthScope;
    }

    private static string GetProjectScopeRegistryKey(Guid projectId) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/ReleaseManagement/Settings/EnforceJobAuthScope/{0}", (object) projectId);

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Correct Spelling")]
    private void LoadSettings(IVssRequestContext requestContext)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string scopeRegistryKey = ProjectPipelineReleaseSettingsHelper.GetProjectScopeRegistryKey(this.projectId);
      this.EnforceJobAuthScope = service.GetValue<bool>(requestContext, (RegistryQuery) scopeRegistryKey, false);
      this.OrgEnforceJobAuthScope = service.GetValue<bool>(requestContext, (RegistryQuery) "/Service/ReleaseManagement/Settings/OrgSettings/EnforceJobAuthScope", false);
    }

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Auth", Justification = "Correct term")]
    public bool EffectiveJobAuthScope => this.OrgEnforceJobAuthScope || this.EnforceJobAuthScope;

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Auth", Justification = "Correct term")]
    public bool EnforceJobAuthScope { get; private set; }

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Auth", Justification = "Correct term")]
    public bool OrgEnforceJobAuthScope { get; private set; }

    public bool HasEditPermission { get; }
  }
}
