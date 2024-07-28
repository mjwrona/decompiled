// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.FrameworkSwapIdentityService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class FrameworkSwapIdentityService : ISwapIdentityService, IVssFrameworkService
  {
    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public bool SwapIdentity(IVssRequestContext requestContext, Guid id1, Guid id2)
    {
      ArgumentUtility.CheckForEmptyGuid(id1, nameof (id1));
      ArgumentUtility.CheckForEmptyGuid(id2, nameof (id2));
      HttpResponseMessage httpResponseMessage = TaskExtensions.SyncResult(requestContext.GetClient<IdentityHttpClient>().SwapIdentityAsync(id1, id2, (object) null, new CancellationToken()));
      List<Guid> identityIds = new List<Guid>() { id1, id2 };
      requestContext.GetService<IdentityService>().IdentityServiceInternalRestricted().InvalidateIdentities(requestContext, (ICollection<Guid>) identityIds);
      return httpResponseMessage.IsSuccessStatusCode;
    }
  }
}
