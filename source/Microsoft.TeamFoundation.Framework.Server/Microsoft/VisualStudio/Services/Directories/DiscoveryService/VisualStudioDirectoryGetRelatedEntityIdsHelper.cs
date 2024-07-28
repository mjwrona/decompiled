// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.VisualStudioDirectoryGetRelatedEntityIdsHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal static class VisualStudioDirectoryGetRelatedEntityIdsHelper
  {
    internal static DirectoryInternalGetRelatedEntityIdsResponse GetRelatedEntityIds(
      IVssRequestContext context,
      DirectoryInternalGetRelatedEntityIdsRequest request)
    {
      Dictionary<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntityIdsResult> dictionary = request.EntityIds.ToDictionary<DirectoryEntityIdentifier, DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntityIdsResult>((Func<DirectoryEntityIdentifier, DirectoryEntityIdentifier>) (id => id), (Func<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntityIdsResult>) (id => (DirectoryInternalGetRelatedEntityIdsResult) null), (IEqualityComparer<DirectoryEntityIdentifier>) DirectoryEntityIdentifier.Comparer);
      if (VisualStudioDirectoryRequestFilter.AllowRequest(context, (DirectoryInternalRequest) request) && request.Relation == "Member" && request.Depth == 1)
        VisualStudioDirectoryGetRelatedEntityIdsHelper.GetDirectDown(context, (IDictionary<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntityIdsResult>) dictionary);
      return new DirectoryInternalGetRelatedEntityIdsResponse()
      {
        Results = (IDictionary<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntityIdsResult>) dictionary
      };
    }

    private static void GetDirectDown(
      IVssRequestContext context,
      IDictionary<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntityIdsResult> results)
    {
      List<KeyValuePair<DirectoryEntityIdentifier, Guid>> list = VisualStudioDirectoryVsidResolver.Instance.ResolveVsids(context, (IEnumerable<DirectoryEntityIdentifier>) results.Keys.ToList<DirectoryEntityIdentifier>()).Where<KeyValuePair<DirectoryEntityIdentifier, Guid>>((Func<KeyValuePair<DirectoryEntityIdentifier, Guid>, bool>) (kvp => kvp.Value != Guid.Empty)).ToList<KeyValuePair<DirectoryEntityIdentifier, Guid>>();
      if (list.Count == 0)
        return;
      IdentityService service = context.GetService<IdentityService>();
      IList<Microsoft.VisualStudio.Services.Identity.Identity> source1 = service.ReadIdentities(context, (IList<Guid>) list.Select<KeyValuePair<DirectoryEntityIdentifier, Guid>, Guid>((Func<KeyValuePair<DirectoryEntityIdentifier, Guid>, Guid>) (kvp => kvp.Value)).ToList<Guid>(), QueryMembership.Direct, (IEnumerable<string>) null);
      HashSet<Guid> source2 = new HashSet<Guid>(source1.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (identity => identity != null)).SelectMany<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, IEnumerable<Guid>>) (identity => (IEnumerable<Guid>) identity.MemberIds)));
      Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> memberIdToMember = service.ReadIdentities(context, (IList<Guid>) source2.ToList<Guid>(), QueryMembership.None, (IEnumerable<string>) null).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (member => member != null)).ToDictionary<Microsoft.VisualStudio.Services.Identity.Identity, Guid, Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (member => member.Id), (Func<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>) (member => member));
      for (int index = 0; index < list.Count; ++index)
      {
        DirectoryEntityIdentifier key = list[index].Key;
        Microsoft.VisualStudio.Services.Identity.Identity identity = source1[index];
        if (identity != null)
          results[key] = new DirectoryInternalGetRelatedEntityIdsResult()
          {
            EntityIds = (IEnumerable<string>) identity.MemberIds.Where<Guid>((Func<Guid, bool>) (memberId => memberIdToMember.ContainsKey(memberId))).Select<Guid, string>((Func<Guid, string>) (memberId => VisualStudioDirectoryEntityConverter.ConvertIdentity(context, memberIdToMember[memberId]).EntityId)).ToList<string>()
          };
        else
          results[key] = new DirectoryInternalGetRelatedEntityIdsResult()
          {
            Exception = (Exception) new DirectoryEntityNotFoundException()
          };
      }
    }
  }
}
