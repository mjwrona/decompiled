// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ActionToPermissionConverter
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.Server.Core
{
  public static class ActionToPermissionConverter
  {
    public static int Convert(IVssRequestContext requestContext, Guid namespaceId, string action)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, namespaceId);
      int num = -1;
      foreach (ActionDefinition action1 in securityNamespace.Description.Actions)
      {
        if (action1.Name.Equals(action, StringComparison.OrdinalIgnoreCase))
        {
          num = action1.Bit;
          break;
        }
      }
      return num != -1 ? num : throw new GroupSecuritySubsystemServiceException(Microsoft.Azure.Boards.CssNodes.ServerResources.GSS_SECURITYACTIONDOESNOTEXISTERROR((object) action));
    }
  }
}
