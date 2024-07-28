// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.ExportHtmlDialogSettings
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.Azure.Boards.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  internal class ExportHtmlDialogSettings
  {
    private string[] m_dialogSettings;
    private TestManagerRequestContext m_testContext;

    public ExportHtmlDialogSettings(TestManagerRequestContext testContext)
    {
      this.m_testContext = testContext;
      this.InitializeExportHtmlSettings();
    }

    public ExportHtmlDialogSettings(List<string> dialogSettings) => this.m_dialogSettings = dialogSettings.ToArray();

    public List<string> GetDialogSettings()
    {
      if (this.m_dialogSettings == null || this.m_dialogSettings.Length == 0)
        this.InitializeExportHtmlSettings();
      return ((IEnumerable<string>) this.m_dialogSettings).ToList<string>();
    }

    public bool isCheckboxChecked(string checkboxName)
    {
      bool flag = false;
      string[] array = ((IEnumerable<string>) this.m_dialogSettings).Where<string>((Func<string, bool>) (s => s == checkboxName)).ToArray<string>();
      if (array != null && array.Length != 0)
        flag = true;
      return flag;
    }

    private void InitializeExportHtmlSettings()
    {
      string path = "/ExportHtmlSettings";
      string str = string.Empty;
      using (WebUserSettingsHive userSettingsHive = new WebUserSettingsHive(this.m_testContext.TfsRequestContext))
        str = userSettingsHive.ReadSetting<string>(path, string.Empty);
      this.m_dialogSettings = str.Split('&');
    }
  }
}
