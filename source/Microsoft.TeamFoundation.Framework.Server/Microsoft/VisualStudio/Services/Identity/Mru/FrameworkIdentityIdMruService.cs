// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Mru.FrameworkIdentityIdMruService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity.Mru.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Identity.Mru
{
  internal class FrameworkIdentityIdMruService : IIdentityIdMruService, IVssFrameworkService
  {
    public const string RemoveInvalidIdentitiesFeatureFlag = "VisualStudio.Services.MRU.RemoveInvalidIdentities";
    private Guid m_serviceHostId;
    private const string s_area = "Microsoft.VisualStudio.Services.Identity.Mru";
    private const string s_layer = "IdentityIdMruService";

    public void ServiceStart(IVssRequestContext context)
    {
      context.CheckProjectCollectionRequestContext();
      this.m_serviceHostId = context.ServiceHost.InstanceId;
      context.GetService<IImsCacheService>().AddSupportForOperations(context, ImsOperation.None, ImsOperation.MruIdentityIds);
    }

    public void ServiceEnd(IVssRequestContext context)
    {
    }

    public IList<Guid> GetMruIdentities(
      IVssRequestContext context,
      Guid identityId,
      Guid containerId)
    {
      this.ValidateRequestContext(context);
      IdentityMruValidator.ValidateMruContainerId(containerId);
      try
      {
        IList<Guid> mruIdentityIds = context.GetService<IImsCacheService>().GetMruIdentityIds(context, identityId, containerId);
        if (mruIdentityIds != null)
          return mruIdentityIds;
      }
      catch (Exception ex)
      {
        context.TraceException(451304, "Microsoft.VisualStudio.Services.Identity.Mru", "IdentityIdMruService", ex);
      }
      List<Guid> values = this.GetHttpClient(context).GetMruIdentitiesAsync(identityId.ToString(), containerId).SyncResult<List<Guid>>() ?? new List<Guid>();
      try
      {
        context.GetService<IImsCacheService>().SetMruIdentityIds(context, identityId, containerId, values);
      }
      catch (Exception ex)
      {
        context.TraceException(451308, "Microsoft.VisualStudio.Services.Identity.Mru", "IdentityIdMruService", ex);
      }
      return (IList<Guid>) values;
    }

    public void AddMruIdentities(
      IVssRequestContext context,
      Guid identityId,
      Guid containerId,
      IList<Guid> identityIds)
    {
      this.ValidateRequestContext(context);
      IdentityMruValidator.ValidateMruContainerId(containerId);
      IdentityMruValidator.ValidateMruValue(identityIds);
      this.GetHttpClient(context).UpdateMruIdentitiesAsync(new MruIdentitiesUpdateData(identityIds, "add"), identityId.ToString(), containerId).SyncResult();
      try
      {
        context.GetService<IImsCacheService>().RemoveMruIdentityIds(context, identityId, containerId);
      }
      catch (Exception ex)
      {
        context.TraceException(451318, "Microsoft.VisualStudio.Services.Identity.Mru", "IdentityIdMruService", ex);
      }
    }

    public IList<Guid> AddMruIdentitiesAndRemoveInactive(
      IVssRequestContext context,
      Guid identityId,
      Guid containerId,
      IList<Guid> identityIds)
    {
      this.ValidateRequestContext(context);
      IdentityMruValidator.ValidateMruContainerId(containerId);
      IdentityMruValidator.ValidateMruValue(identityIds);
      List<Guid> guidList = this.GetHttpClient(context).AddMruIdentitiesAndRemoveInactiveAsync(new MruIdentitiesUpdateData(identityIds, "add"), identityId.ToString(), containerId).SyncResult<List<Guid>>();
      try
      {
        context.GetService<IImsCacheService>().RemoveMruIdentityIds(context, identityId, containerId);
      }
      catch (Exception ex)
      {
        context.TraceException(451318, "Microsoft.VisualStudio.Services.Identity.Mru", "IdentityIdMruService", ex);
      }
      return (IList<Guid>) guidList;
    }

    public void RemoveMruIdentities(
      IVssRequestContext context,
      Guid identityId,
      Guid containerId,
      IList<Guid> identityIds)
    {
      this.ValidateRequestContext(context);
      IdentityMruValidator.ValidateMruContainerId(containerId);
      IdentityMruValidator.ValidateMruValue(identityIds);
      IdentityMruHttpClient httpClient = this.GetHttpClient(context);
      MruIdentitiesUpdateData updateData = new MruIdentitiesUpdateData(identityIds, "remove");
      if (context.IsFeatureEnabled("VisualStudio.Services.MRU.RemoveInvalidIdentities"))
        httpClient.AddMruIdentitiesAndRemoveInactiveAsync(updateData, identityId.ToString(), containerId).SyncResult<List<Guid>>();
      else
        httpClient.UpdateMruIdentitiesAsync(updateData, identityId.ToString(), containerId).SyncResult();
      try
      {
        context.GetService<IImsCacheService>().RemoveMruIdentityIds(context, identityId, containerId);
      }
      catch (Exception ex)
      {
        context.TraceException(451328, "Microsoft.VisualStudio.Services.Identity.Mru", "IdentityIdMruService", ex);
      }
    }

    private void ValidateRequestContext(IVssRequestContext requestContext) => requestContext.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);

    internal virtual IdentityMruHttpClient GetHttpClient(IVssRequestContext context) => context.GetClient<IdentityMruHttpClient>();
  }
}
