// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Mru.IIdentityIdMruService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Identity.Mru
{
  [DefaultServiceImplementation(typeof (FrameworkIdentityIdMruService))]
  internal interface IIdentityIdMruService : IVssFrameworkService
  {
    IList<Guid> GetMruIdentities(IVssRequestContext context, Guid identityId, Guid containerId);

    void AddMruIdentities(
      IVssRequestContext context,
      Guid identityId,
      Guid containerId,
      IList<Guid> identityIds);

    void RemoveMruIdentities(
      IVssRequestContext context,
      Guid identityId,
      Guid containerId,
      IList<Guid> identityIds);

    IList<Guid> AddMruIdentitiesAndRemoveInactive(
      IVssRequestContext context,
      Guid identityId,
      Guid containerId,
      IList<Guid> identityIds);
  }
}
