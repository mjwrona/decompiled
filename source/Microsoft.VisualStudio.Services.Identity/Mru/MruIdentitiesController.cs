// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Mru.MruIdentitiesController
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Identity.Mru
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "Identity", ResourceName = "MruIdentities", ResourceVersion = 1)]
  public class MruIdentitiesController : TfsApiController
  {
    private const string s_layer = "MruIdentitiesController";

    [HttpGet]
    [ClientLocationId("15D952A1-BB4E-436C-88CA-CFE1E9FF3331")]
    public IList<Guid> GetMruIdentities(string identityId, Guid containerId)
    {
      Guid guid = MruIdentitiesController.ValidateAndExtractGuid(this.TfsRequestContext, identityId);
      try
      {
        return this.TfsRequestContext.GetService<PlatformIdentityIdMruService>().GetMruIdentities(this.TfsRequestContext, guid, containerId);
      }
      catch (Exception ex)
      {
        this.HandleException(ex, 451008);
        throw;
      }
    }

    [HttpPut]
    [ClientLocationId("15D952A1-BB4E-436C-88CA-CFE1E9FF3331")]
    public void SetMruIdentities(string identityId, Guid containerId, IList<Guid> identityIds)
    {
      Guid guid = MruIdentitiesController.ValidateAndExtractGuid(this.TfsRequestContext, identityId);
      try
      {
        this.TfsRequestContext.GetService<PlatformIdentityIdMruService>().SetMruIdentities(this.TfsRequestContext, guid, containerId, identityIds);
      }
      catch (Exception ex)
      {
        this.HandleException(ex, 451028);
        throw;
      }
    }

    [HttpPatch]
    [Obsolete]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [ClientLocationId("15D952A1-BB4E-436C-88CA-CFE1E9FF3331")]
    public void UpdateMruIdentities(
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
          service.AddMruIdentities(this.TfsRequestContext, guid, containerId, updateData.Value);
        else
          service.RemoveMruIdentities(this.TfsRequestContext, guid, containerId, updateData.Value);
      }
      catch (Exception ex)
      {
        this.HandleException(ex, 451038);
        throw;
      }
    }

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<IdentityMruBadRequestException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<IdentityMruUnauthorizedException>(HttpStatusCode.Forbidden);
      exceptionMap.AddStatusCode<IdentityMruResourceNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<IdentityMruResourceExistsException>(HttpStatusCode.Conflict);
    }

    protected void HandleException(Exception ex, int tracePoint)
    {
      if (this.ExceptionMapping.GetStatusCode(ex.GetType()).HasValue)
        return;
      this.TfsRequestContext.TraceException(tracePoint, this.TraceArea, nameof (MruIdentitiesController), ex);
    }

    protected static Guid ValidateAndExtractGuid(IVssRequestContext requestContext, string id)
    {
      if (string.Equals(id, "me", StringComparison.OrdinalIgnoreCase))
        return requestContext.GetUserId(true);
      Guid result;
      if (!Guid.TryParse(id, out result))
        throw new IdentityMruBadRequestException(string.Format("Given id: '{0}'; The id should either be a Guid or the string literal 'me'.", (object) id));
      return result;
    }

    protected static bool IsAddOrUpdate(string opeartionType) => string.Equals(opeartionType, "add", StringComparison.OrdinalIgnoreCase) || string.Equals(opeartionType, "update", StringComparison.OrdinalIgnoreCase);

    protected static bool IsRemove(string opeartionType) => string.Equals(opeartionType, "remove", StringComparison.OrdinalIgnoreCase);

    public override string TraceArea => "Microsoft.VisualStudio.Services.Identity.Mru";

    public override string ActivityLogArea => "MruIdentities";
  }
}
