// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security.ReleaseManagementSecurityPermissionAttribute
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using System;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security
{
  [AttributeUsage(AttributeTargets.Method)]
  public sealed class ReleaseManagementSecurityPermissionAttribute : Attribute
  {
    public ReleaseManagementSecurityPermissions Permission { get; private set; }

    public string ApiArgName { get; private set; }

    public ReleaseManagementSecurityArgumentType ApiArgType { get; private set; }

    public bool AlwaysAllowAdministrators { get; private set; }

    public bool AllowByePassDataspaceFaultIn { get; private set; }

    public ReleaseManagementSecurityPermissionAttribute(
      string apiArgName,
      ReleaseManagementSecurityArgumentType apiArgType,
      ReleaseManagementSecurityPermissions permission)
      : this(apiArgName, apiArgType, permission, false)
    {
    }

    public ReleaseManagementSecurityPermissionAttribute(
      string apiArgName,
      ReleaseManagementSecurityArgumentType apiArgType,
      ReleaseManagementSecurityPermissions permission,
      bool alwaysAllowAdministrators)
    {
      this.ApiArgName = apiArgName;
      this.ApiArgType = apiArgType;
      this.Permission = permission;
      this.AlwaysAllowAdministrators = alwaysAllowAdministrators;
      this.AllowByePassDataspaceFaultIn = false;
    }

    public ReleaseManagementSecurityPermissionAttribute(
      ReleaseManagementSecurityPermissions permission)
      : this((string) null, ReleaseManagementSecurityArgumentType.Project, permission)
    {
    }

    public ReleaseManagementSecurityPermissionAttribute(
      ReleaseManagementSecurityPermissions permission,
      bool allowByePassDataspaceFaultIn)
    {
      this.ApiArgName = (string) null;
      this.ApiArgType = ReleaseManagementSecurityArgumentType.Project;
      this.Permission = permission;
      this.AlwaysAllowAdministrators = false;
      this.AllowByePassDataspaceFaultIn = allowByePassDataspaceFaultIn;
    }
  }
}
