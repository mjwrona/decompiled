// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Internal.IdentityRightsTransferHelper
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Identity.Internal
{
  internal static class IdentityRightsTransferHelper
  {
    private const string c_area = "IdentityRightsTransfer";
    private const string c_layer = "IdentityRightsTransferHelper";

    internal static void TransferIdentityRights(
      IVssRequestContext collectionContext,
      Microsoft.VisualStudio.Services.Identity.Identity fromIdentity,
      Microsoft.VisualStudio.Services.Identity.Identity toIdentity)
    {
      collectionContext.CheckProjectCollectionRequestContext();
      List<Tuple<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>> identityMap = new List<Tuple<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>>();
      IdentityService service1 = collectionContext.GetService<IdentityService>();
      Microsoft.VisualStudio.Services.Identity.Identity readIdentity = service1.ReadIdentities(collectionContext, (IList<Guid>) new List<Guid>()
      {
        fromIdentity.MasterId
      }, QueryMembership.None, (IEnumerable<string>) null)[0];
      if (readIdentity == null)
        throw new Exception(string.Format("Identity with VSID {0} is not member of account", (object) fromIdentity.MasterId));
      readIdentity.SetProperty("AuthenticationCredentialValidFrom", (object) DateTime.UtcNow.Ticks);
      service1.UpdateIdentities(collectionContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
      {
        readIdentity
      });
      identityMap.Add(new Tuple<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>(readIdentity, toIdentity));
      IdentityRightsTransferHelper.TransferIdentityRights(collectionContext, (IList<Tuple<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>>) identityMap);
      ICollectionService service2 = collectionContext.GetService<ICollectionService>();
      Collection collection = service2.GetCollection(collectionContext.Elevate(), (IEnumerable<string>) null);
      if (collection.Owner == fromIdentity.MasterId)
        service2.UpdateCollectionOwner(collectionContext, toIdentity.MasterId);
      IdentityRightsTransferHelper.InvalidateIdentities(collectionContext, collection.Id, (IList<Tuple<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>>) identityMap);
    }

    internal static void TransferIdentityRights(
      IVssRequestContext requestContext,
      IList<Tuple<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>> identityMap,
      bool validateSourceData = false)
    {
      requestContext.CheckProjectCollectionRequestContext();
      ITransferIdentityRightsService service1 = requestContext.GetService<ITransferIdentityRightsService>();
      IdentityService service2 = requestContext.GetService<IdentityService>();
      foreach (Tuple<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity> identity in (IEnumerable<Tuple<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>>) identityMap)
      {
        if (service2.ReadIdentities(requestContext, (IList<Guid>) new List<Guid>()
        {
          identity.Item2.Id
        }, QueryMembership.None, (IEnumerable<string>) null)[0] != null && identity.Item1 != identity.Item2)
          requestContext.Trace(5001246, TraceLevel.Error, "IdentityRightsTransfer", nameof (IdentityRightsTransferHelper), "Master Id with VSID {0} is already a member of the account with Local Id {1}.", (object) identity.Item2.Id, (object) identity.Item1.Id);
      }
      service1.TransferIdentityRights(requestContext, identityMap, validateSourceData);
    }

    internal static void InvalidateIdentities(
      IVssRequestContext requestContext,
      Guid collectionId,
      IList<Tuple<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>> identityMap)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
      IdentityService service = vssRequestContext.GetService<IdentityService>();
      List<Guid> identityIds = identityMap.SelectMany<Tuple<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>, Guid>((Func<Tuple<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>, IEnumerable<Guid>>) (tuple => flattenTupleToIds(tuple))).Distinct<Guid>().ToList<Guid>();
      vssRequestContext.TraceConditionally(80363, TraceLevel.Info, "IdentityRightsTransfer", nameof (IdentityRightsTransferHelper), (Func<string>) (() => string.Format("{0}: Invalidating identities: {1}.", (object) collectionId, (object) IdentityRightsTransferHelper.GetTraceMessageForIdentityIds((IList<Guid>) identityIds))));
      service.IdentityServiceInternalRestricted().InvalidateIdentities(vssRequestContext, (ICollection<Guid>) identityIds);

      static IEnumerable<Guid> flattenTupleToIds(Tuple<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity> tuple)
      {
        yield return tuple.Item1.MasterId;
        yield return tuple.Item2.MasterId;
      }
    }

    private static string GetTraceMessageForIdentityIds(IList<Guid> identities)
    {
      if (identities == null || identities.Count == 0)
        return string.Empty;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("{");
      stringBuilder.AppendFormat("Total count: {0}. ", (object) identities.Count);
      int num = 0;
      foreach (Guid identity in (IEnumerable<Guid>) identities)
      {
        if (num > 0)
          stringBuilder.Append(",");
        stringBuilder.Append((object) identity);
        if (num++ > 10)
          break;
      }
      stringBuilder.Append("}");
      return stringBuilder.ToString();
    }
  }
}
