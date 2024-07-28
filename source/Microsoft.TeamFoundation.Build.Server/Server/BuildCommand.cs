// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildCommand
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Build.Server
{
  internal abstract class BuildCommand : Command
  {
    private Dictionary<string, Dictionary<int, bool>> m_buildPermissions = new Dictionary<string, Dictionary<int, bool>>(10);

    protected BuildCommand(IVssRequestContext requestContext, Guid projectId)
      : base(requestContext)
    {
      this.BuildHost = requestContext.GetService<TeamFoundationBuildHost>();
      this.ProjectId = projectId;
    }

    internal TeamFoundationBuildHost BuildHost { get; private set; }

    internal Guid ProjectId { get; private set; }

    internal bool HasBuildPermission(
      IVssRequestContext requestContext,
      string buildDefinitionUri,
      string buildDefinitionSecurityToken,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false)
    {
      requestContext.Trace(0, TraceLevel.Verbose, "Build", "Security", "Checking permissions '{0}' on definition '{1}'. AllowAdministrators='{2}'", (object) requiredPermissions, (object) buildDefinitionUri, (object) alwaysAllowAdministrators);
      Dictionary<int, bool> dictionary = (Dictionary<int, bool>) null;
      if (!this.m_buildPermissions.TryGetValue(buildDefinitionUri, out dictionary))
      {
        dictionary = new Dictionary<int, bool>(4);
        this.m_buildPermissions[buildDefinitionUri] = dictionary;
      }
      bool flag = false;
      if (!dictionary.TryGetValue(requiredPermissions, out flag))
      {
        requestContext.Trace(0, TraceLevel.Verbose, "Build", "Security", "Build definition permissions were not in the cache");
        flag = this.BuildHost.SecurityManager.BuildSecurity.HasPermission(requestContext, buildDefinitionSecurityToken, requiredPermissions, alwaysAllowAdministrators);
        dictionary[requiredPermissions] = flag;
      }
      return flag;
    }
  }
}
