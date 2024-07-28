// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Mru.MruIdentities2Controller
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Identity.Mru
{
  [ControllerApiVersion(6.1)]
  [VersionedApiControllerCustomName(Area = "Identity", ResourceName = "MruIdentitiesV2")]
  public class MruIdentities2Controller : MruIdentitiesController
  {
    [HttpPatch]
    [ActionName("AddMruIdentities")]
    public IList<Guid> AddMruIdentitiesAndRemoveInactive(
      string identityId,
      Guid containerId,
      MruIdentitiesUpdateData updateData)
    {
      Guid guid = MruIdentitiesController.ValidateAndExtractGuid(this.TfsRequestContext, identityId);
      bool flag1 = MruIdentitiesController.IsAddOrUpdate(updateData.Op);
      bool flag2 = MruIdentitiesController.IsRemove(updateData.Op);
      if (!flag1 && !flag2)
        throw new IdentityMruBadRequestException(string.Format("Requested operation type '{0}' is not supported.", (object) updateData.Op));
      try
      {
        PlatformIdentityIdMruService service = this.TfsRequestContext.GetService<PlatformIdentityIdMruService>();
        if (flag1)
          return service.AddMruIdentitiesAndRemoveInactive(this.TfsRequestContext, guid, containerId, updateData.Value);
        service.RemoveMruIdentities(this.TfsRequestContext, guid, containerId, updateData.Value);
        return (IList<Guid>) new List<Guid>();
      }
      catch (Exception ex)
      {
        this.HandleException(ex, 451038);
        throw;
      }
    }
  }
}
