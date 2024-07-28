// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementProjectSettingsHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TestManagementProjectSettingsHelper
  {
    private TfsTestManagementRequestContext m_tmRequestContext;
    private Guid m_projectGuid;
    private static readonly string ProjectPrefix = "/Project/{0}/WebAccess";

    internal TestManagementProjectSettingsHelper(
      TfsTestManagementRequestContext tmRequestContext,
      Guid projectId)
    {
      this.m_tmRequestContext = tmRequestContext;
      this.m_projectGuid = projectId;
    }

    public void SetRegistryValue(string path, string value)
    {
      IVssRegistryService service = this.m_tmRequestContext.RequestContext.GetService<IVssRegistryService>();
      string str1 = this.ComposeKey(path);
      IVssRequestContext requestContext = this.m_tmRequestContext.RequestContext;
      string path1 = str1;
      string str2 = value;
      service.SetValue<string>(requestContext, path1, str2);
    }

    public string GetRegistryValue(string path)
    {
      IVssRegistryService service = this.m_tmRequestContext.RequestContext.GetService<IVssRegistryService>();
      string str = this.ComposeKey(path);
      IVssRequestContext requestContext = this.m_tmRequestContext.RequestContext;
      // ISSUE: explicit reference operation
      ref RegistryQuery local = @(RegistryQuery) str;
      return service.GetValue(requestContext, in local, "false");
    }

    protected virtual string Prefix => string.Format(TestManagementProjectSettingsHelper.ProjectPrefix, (object) this.m_projectGuid);

    private string ComposeKey(string path) => this.Prefix + path;
  }
}
