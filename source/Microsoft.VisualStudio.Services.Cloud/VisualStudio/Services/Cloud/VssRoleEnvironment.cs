// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.VssRoleEnvironment
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.IO;
using System.Web.Configuration;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class VssRoleEnvironment : IRoleEnvironment
  {
    private DeploymentEnvironment m_deploymentEnvironment;
    private string m_deploymentId;
    private string m_currentRoleInstanceId;
    private string m_currentRoleName;
    private string m_roleRoot;
    private string m_deploymentName;

    public string CurrentRoleInstanceId
    {
      get
      {
        if (string.IsNullOrEmpty(this.m_currentRoleInstanceId))
          this.m_currentRoleInstanceId = VssRoleEnvironment.GetRootSetting(RoleEnvironmentConstants.RoleInstanceId);
        return this.m_currentRoleInstanceId;
      }
    }

    public string CurrentRoleName
    {
      get
      {
        if (string.IsNullOrEmpty(this.m_currentRoleName))
          this.m_currentRoleName = VssRoleEnvironment.GetRootSetting(RoleEnvironmentConstants.RoleName);
        return this.m_currentRoleName;
      }
    }

    public string DeploymentId
    {
      get
      {
        if (string.IsNullOrEmpty(this.m_deploymentId))
          this.m_deploymentId = VssRoleEnvironment.GetRootSetting(RoleEnvironmentConstants.DeploymentId);
        return this.m_deploymentId;
      }
    }

    public DeploymentEnvironment DeploymentEnvironment
    {
      get
      {
        if (this.m_deploymentEnvironment == DeploymentEnvironment.Unknown)
          this.m_deploymentEnvironment = DeploymentEnvironmentParser.Parse(VssRoleEnvironment.GetRootSetting(RoleEnvironmentConstants.DeploymentEnvironment));
        return this.m_deploymentEnvironment;
      }
    }

    public string RoleRoot
    {
      get
      {
        if (string.IsNullOrEmpty(this.m_roleRoot))
          this.m_roleRoot = VssRoleEnvironment.GetRootSetting(RoleEnvironmentConstants.RoleRoot);
        return this.m_roleRoot;
      }
    }

    public string DeploymentName
    {
      get
      {
        if (string.IsNullOrEmpty(this.m_deploymentName))
          this.m_deploymentName = VssRoleEnvironment.GetRootSetting(RoleEnvironmentConstants.DeploymentName);
        return this.m_deploymentName;
      }
    }

    public string GetLocalResourceRootPath(string localResourceName)
    {
      string path = Path.Combine(VssRoleEnvironment.GetRootSetting(RoleEnvironmentConstants.RoleResourceRoot), this.CurrentRoleName, localResourceName);
      if (!Directory.Exists(path))
        Directory.CreateDirectory(path);
      return path;
    }

    private static string GetRootSetting(string name) => Environment.GetEnvironmentVariable(name) ?? WebConfigurationManager.AppSettings[name];
  }
}
