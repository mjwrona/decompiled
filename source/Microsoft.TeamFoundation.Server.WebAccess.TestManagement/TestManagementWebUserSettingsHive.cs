// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestManagementWebUserSettingsHive
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.Azure.Boards.Settings;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  public class TestManagementWebUserSettingsHive : IWebUserSettingsHive, IDisposable
  {
    private WebUserSettingsHive hive;

    public TestManagementWebUserSettingsHive(IVssRequestContext requestContext) => this.hive = new WebUserSettingsHive(requestContext);

    public void Cache(string pathPattern) => this.hive.Cache(pathPattern);

    public T ReadSetting<T>(string path, T defaultValue) => this.hive.ReadSetting<T>(path, defaultValue);

    public void WriteSetting<T>(string path, T value) => this.hive.WriteSetting<T>(path, value);

    public void Dispose() => this.hive?.Dispose();
  }
}
