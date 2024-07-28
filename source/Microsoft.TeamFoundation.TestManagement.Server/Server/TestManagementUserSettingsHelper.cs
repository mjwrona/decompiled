// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementUserSettingsHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Server.WebAccess;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal abstract class TestManagementUserSettingsHelper
  {
    private TfsTestManagementRequestContext m_tmRequestContext;
    private const string c_testManagementMoniker = "TestManagement";
    private const string c_testHubMoniker = "TestHub";
    private string m_projectName;
    private string m_teamIdentityString;
    private Guid m_projectGuid;

    internal TestManagementUserSettingsHelper(
      IVssRequestContext requestContext,
      string projectName,
      string teamIdentityString)
    {
      this.m_tmRequestContext = new TfsTestManagementRequestContext(requestContext);
      this.m_projectName = projectName;
      this.m_teamIdentityString = teamIdentityString;
      this.m_projectGuid = this.GetProjectGuid(projectName);
    }

    private string ComposeKey(string property) => "TestManagement" + "/" + (object) this.m_projectGuid + "/" + this.m_teamIdentityString + "/" + "TestHub" + "/" + property;

    protected virtual string ToWebRegistryPath(string path) => this.Prefix + path;

    protected string GetPropertyValue(string property)
    {
      List<RegistryEntry> registryEntryList = this.m_tmRequestContext.RequestContext.GetService<ISecuredRegistryManager>().QueryUserEntries(this.m_tmRequestContext.RequestContext, this.ToWebRegistryPath(this.ComposeKey(property)), false);
      return registryEntryList.Count == 0 ? string.Empty : registryEntryList[0].Value;
    }

    protected virtual string Prefix => WebAccessRegistryConstants.Prefix;

    private Guid GetProjectGuid(string projectName) => new Guid(LinkingUtilities.DecodeUri(this.m_tmRequestContext.RequestContext.Elevate().GetService<IProjectService>().GetProject(this.m_tmRequestContext.RequestContext, projectName).Uri.Trim()).ToolSpecificId);

    protected abstract string GetMoniker();
  }
}
