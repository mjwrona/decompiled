// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SecurityRoles.RoleNotFoundException
// Assembly: Microsoft.VisualStudio.Services.SecurityRoles.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BBA245E2-CEA0-4262-9E17-EB6FDFC84F54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SecurityRoles.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.SecurityRoles
{
  [Serializable]
  public class RoleNotFoundException : VssServiceException
  {
    public RoleNotFoundException(string roleName)
      : base(SecurityRolesResources.RoleNotFoundException((object) roleName))
    {
    }

    public RoleNotFoundException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public RoleNotFoundException(string message, Exception innerException, int errorNumber)
      : base(message, innerException)
    {
    }
  }
}
