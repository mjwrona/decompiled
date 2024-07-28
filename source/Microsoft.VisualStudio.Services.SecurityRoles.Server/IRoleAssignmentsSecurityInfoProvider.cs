// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SecurityRoles.IRoleAssignmentsSecurityInfoProvider
// Assembly: Microsoft.VisualStudio.Services.SecurityRoles.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BBA245E2-CEA0-4262-9E17-EB6FDFC84F54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SecurityRoles.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.SecurityRoles
{
  public interface IRoleAssignmentsSecurityInfoProvider
  {
    Guid GetRoleAssignmentSecurityNamespace();

    string GetRoleAssignmentSecurityToken(IVssRequestContext requestContext, string resourceId);
  }
}
