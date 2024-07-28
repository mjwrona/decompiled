// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.LayoutUserSettingsUtility
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.Azure.Boards.Settings;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.VisualStudio.Services.Settings;
using Newtonsoft.Json;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class LayoutUserSettingsUtility
  {
    private string c_layoutUserSettingsPath = "/WorkItemTracking/LayoutUserSettings";
    private string c_settingsServiceLayoutUserSettingsPath = "WorkItemTracking/LayoutUserSettings";
    private LayoutUserSettings m_layoutUserSettings;

    public string CreateLayoutUserSettingsToken(Guid projectId) => string.Format("/{0}/{1}/{2}", (object) "WorkItemTracking", (object) projectId, (object) "WorkItemFormUserLayoutSettings");

    public LayoutUserSettings GetSettings(IVssRequestContext requestContext, Guid projectId)
    {
      if (this.m_layoutUserSettings == null)
      {
        IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, WorkItemTrackingNamespaceSecurityConstants.NamespaceId);
        if (securityNamespace != null)
          securityNamespace.CheckPermission(requestContext, this.CreateLayoutUserSettingsToken(projectId), 1);
        ISettingsService service = requestContext.GetService<ISettingsService>();
        LayoutUserSettings layoutUserSettings = service.GetValue<LayoutUserSettings>(requestContext, SettingsUserScope.User, this.c_settingsServiceLayoutUserSettingsPath, (LayoutUserSettings) null, false);
        if (layoutUserSettings != null)
        {
          this.m_layoutUserSettings = layoutUserSettings;
          return this.m_layoutUserSettings;
        }
        using (WebUserSettingsHive userSettingsHive = new WebUserSettingsHive(requestContext))
        {
          string str = userSettingsHive.ReadSetting<string>(this.c_layoutUserSettingsPath, string.Empty);
          this.m_layoutUserSettings = !string.IsNullOrWhiteSpace(str) ? JsonConvert.DeserializeObject<LayoutUserSettings>(str) : new LayoutUserSettings();
        }
        if (this.m_layoutUserSettings != null)
          service.SetValue(requestContext, SettingsUserScope.User, this.c_settingsServiceLayoutUserSettingsPath, (object) this.m_layoutUserSettings);
      }
      return this.m_layoutUserSettings;
    }
  }
}
