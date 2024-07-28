// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.TeamFoundationSecurityNamespaceExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class TeamFoundationSecurityNamespaceExtensions
  {
    internal static IAccessControlEntry GetAccessControlEntry(
      this IVssSecurityNamespace securityNamespace,
      IVssRequestContext requestContext,
      string token,
      IdentityDescriptor descriptor)
    {
      return securityNamespace.QueryAccessControlList(requestContext, token, (IEnumerable<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        descriptor
      }, false)?.QueryAccessControlEntry(descriptor);
    }
  }
}
