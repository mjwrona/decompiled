// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.FrameworkIdentityIdentifierConversionService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.Identity
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal sealed class FrameworkIdentityIdentifierConversionService : 
    IdentityIdentifierConversionServiceBase
  {
    protected override IEnumerable<IIdentityIdentifierRepository> GetRepositories(
      IVssRequestContext requestContext)
    {
      if (requestContext.ServiceHost.IsOnly(TeamFoundationHostType.ProjectCollection))
        return (IEnumerable<IIdentityIdentifierRepository>) Array.Empty<IIdentityIdentifierRepository>();
      return (IEnumerable<IIdentityIdentifierRepository>) new IIdentityIdentifierRepository[2]
      {
        (IIdentityIdentifierRepository) new MemoryCacheIdentityIdentifierRepository(requestContext, TeamFoundationHostType.All),
        (IIdentityIdentifierRepository) new HttpClientIdentityIdentifierRepository(TeamFoundationHostType.All)
      };
    }

    public override IdentityDescriptor GetDescriptorByLocalId(
      IVssRequestContext requestContext,
      Guid localId)
    {
      throw new NotImplementedException();
    }
  }
}
