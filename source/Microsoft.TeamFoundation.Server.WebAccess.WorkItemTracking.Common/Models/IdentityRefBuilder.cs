// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.IdentityRefBuilder
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models
{
  public static class IdentityRefBuilder
  {
    internal static IdentityRef Create(
      IVssRequestContext requestContext,
      Guid vsid,
      bool includeUrls = false,
      bool includeInactive = false)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
      {
        vsid
      }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      return identity == null ? (IdentityRef) null : identity.ToIdentityRef(requestContext, includeUrls, includeInactive);
    }

    public static IDictionary<Guid, IdentityRef> Create(
      IVssRequestContext requestContext,
      IEnumerable<Guid> vsids,
      bool includeUrls = false,
      bool includeInactive = false)
    {
      List<Guid> list = vsids.Where<Guid>((Func<Guid, bool>) (x => x != Guid.Empty)).Distinct<Guid>().ToList<Guid>();
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) list, QueryMembership.None, (IEnumerable<string>) null);
      Dictionary<Guid, IdentityRef> dictionary = new Dictionary<Guid, IdentityRef>();
      for (int index = 0; index < list.Count; ++index)
      {
        if (identityList[index] != null)
        {
          IdentityRef identityRef = identityList[index].ToIdentityRef(requestContext, includeUrls, includeInactive);
          dictionary.Add(list[index], identityRef);
        }
      }
      return (IDictionary<Guid, IdentityRef>) dictionary;
    }

    public static IdentityRef CreateFromConstantId(
      IVssRequestContext requestContext,
      int constantId,
      bool includeUrls = false,
      bool includeInactive = false)
    {
      TeamFoundationWorkItemTrackingMetadataService service1 = requestContext.GetService<TeamFoundationWorkItemTrackingMetadataService>();
      IdentityService service2 = requestContext.GetService<IdentityService>();
      IVssRequestContext requestContext1 = requestContext;
      int constantId1 = constantId;
      WitReadReplicaContext? readReplicaContext = new WitReadReplicaContext?();
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.ConstantRecord constantRecord = service1.GetConstantRecord(requestContext1, constantId1, readReplicaContext);
      Microsoft.VisualStudio.Services.Identity.Identity identity = service2.ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
      {
        constantRecord.TeamFoundationId
      }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      return identity == null ? (IdentityRef) null : identity.ToIdentityRef(requestContext, includeUrls, includeInactive);
    }
  }
}
