// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Mru.IdentityMruService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity.Mru
{
  internal class IdentityMruService : IVssFrameworkService
  {
    public const string RemoveInvalidIdentitiesFeatureFlag = "VisualStudio.Services.MRU.RemoveInvalidIdentities";
    private Guid m_serviceHostId;
    private const string s_area = "Microsoft.VisualStudio.Services.Identity.Mru";
    private const string s_layer = "IdentityMruService";
    public static readonly Guid SharedDefaultContainerId = new Guid("6AB44F9E-B431-4969-A773-D7445C92E8F9");

    public void ServiceStart(IVssRequestContext context)
    {
      context.CheckProjectCollectionRequestContext();
      this.m_serviceHostId = context.ServiceHost.InstanceId;
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadMruIdentities(
      IVssRequestContext context,
      Guid identityId,
      Guid containerId)
    {
      this.ValidateRequestContext(context);
      IdentityMruValidator.ValidateMruContainerId(containerId);
      IdentityMruValidator.ValidateIdentityExists(context, identityId);
      IList<Guid> mruIdentities = context.GetService<IIdentityIdMruService>().GetMruIdentities(context, identityId, containerId);
      return mruIdentities.IsNullOrEmpty<Guid>() ? (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[0] : (IList<Microsoft.VisualStudio.Services.Identity.Identity>) context.GetService<IdentityService>().ReadIdentities(context, mruIdentities, QueryMembership.None, (IEnumerable<string>) null).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null)).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
    }

    public void AddMruIdentities(
      IVssRequestContext context,
      Guid identityId,
      Guid containerId,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities)
    {
      this.ValidateRequestContext(context);
      IdentityMruValidator.ValidateMruValue(identities);
      List<Guid> identityIds = IdentityMruService.ExtractIdentityIds(identities);
      this.AddMruIdentities(context, identityId, containerId, (IList<Guid>) identityIds);
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
      IdentityMruValidator.ValidateIdentityExists(context, identityId);
      IIdentityIdMruService service = context.GetService<IIdentityIdMruService>();
      if (context.IsFeatureEnabled("VisualStudio.Services.MRU.RemoveInvalidIdentities"))
      {
        if ((service.AddMruIdentitiesAndRemoveInactive(context, identityId, containerId, identityIds) ?? (IList<Guid>) new List<Guid>()).Count < identityIds.Count)
          throw new IdentityMruResourceNotFoundException(FrameworkResources.UserIsDeletedFromAccount());
      }
      else
        service.AddMruIdentities(context, identityId, containerId, identityIds);
    }

    public void RemoveMruIdentities(
      IVssRequestContext context,
      Guid identityId,
      Guid containerId,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities)
    {
      this.ValidateRequestContext(context);
      IdentityMruValidator.ValidateMruValue(identities);
      List<Guid> identityIds = IdentityMruService.ExtractIdentityIds(identities);
      this.RemoveMruIdentities(context, identityId, containerId, (IList<Guid>) identityIds);
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
      IdentityMruValidator.ValidateIdentityExists(context, identityId);
      context.GetService<IIdentityIdMruService>().RemoveMruIdentities(context, identityId, containerId, identityIds);
    }

    private void ValidateRequestContext(IVssRequestContext requestContext) => requestContext.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);

    private static List<Guid> ExtractIdentityIds(IList<Microsoft.VisualStudio.Services.Identity.Identity> identities) => identities.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null)).Select<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (x => x.Id)).ToList<Guid>();
  }
}
