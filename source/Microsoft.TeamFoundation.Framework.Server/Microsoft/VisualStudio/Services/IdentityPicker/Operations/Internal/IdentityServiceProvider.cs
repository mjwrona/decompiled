// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal.IdentityServiceProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal
{
  internal sealed class IdentityServiceProvider : Microsoft.VisualStudio.Services.Identity.SearchFilter.IIdentityProvider
  {
    internal static IdentityServiceProvider Instance { get; } = new IdentityServiceProvider();

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IList<IdentityDescriptor> descriptors)
    {
      return requestContext.GetService<IdentityService>().ReadIdentities(requestContext, descriptors, QueryMembership.None, (IEnumerable<string>) null);
    }

    public bool IsMember(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor memberDescriptor)
    {
      return requestContext.GetService<IdentityService>().IsMember(requestContext, groupDescriptor, memberDescriptor);
    }
  }
}
