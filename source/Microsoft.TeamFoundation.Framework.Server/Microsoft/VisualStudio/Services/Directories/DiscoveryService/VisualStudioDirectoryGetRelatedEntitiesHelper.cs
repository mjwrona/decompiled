// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.VisualStudioDirectoryGetRelatedEntitiesHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal static class VisualStudioDirectoryGetRelatedEntitiesHelper
  {
    internal static DirectoryInternalGetRelatedEntitiesResponse GetRelatedEntities(
      IVssRequestContext context,
      DirectoryInternalGetRelatedEntitiesRequest request)
    {
      Dictionary<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntitiesResult> dictionary = request.EntityIds.ToDictionary<DirectoryEntityIdentifier, DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntitiesResult>((Func<DirectoryEntityIdentifier, DirectoryEntityIdentifier>) (id => id), (Func<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntitiesResult>) (id => (DirectoryInternalGetRelatedEntitiesResult) null), (IEqualityComparer<DirectoryEntityIdentifier>) DirectoryEntityIdentifier.Comparer);
      if (VisualStudioDirectoryRequestFilter.AllowRequest(context, (DirectoryInternalRequest) request) && request.Relation == "Member" && request.Depth == 1)
        VisualStudioDirectoryGetRelatedEntitiesHelper.GetDirectMembers(context, request.PropertiesToReturn, (IDictionary<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntitiesResult>) dictionary, request);
      return new DirectoryInternalGetRelatedEntitiesResponse()
      {
        Results = (IDictionary<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntitiesResult>) dictionary
      };
    }

    private static void GetDirectMembers(
      IVssRequestContext context,
      IEnumerable<string> properties,
      IDictionary<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntitiesResult> results,
      DirectoryInternalGetRelatedEntitiesRequest request)
    {
      List<KeyValuePair<DirectoryEntityIdentifier, Guid>> list1 = VisualStudioDirectoryVsidResolver.Instance.ResolveVsids(context, (IEnumerable<DirectoryEntityIdentifier>) results.Keys.ToList<DirectoryEntityIdentifier>()).Where<KeyValuePair<DirectoryEntityIdentifier, Guid>>((Func<KeyValuePair<DirectoryEntityIdentifier, Guid>, bool>) (kvp => kvp.Value != Guid.Empty)).ToList<KeyValuePair<DirectoryEntityIdentifier, Guid>>();
      if (list1.Count == 0)
        return;
      IdentityService service = context.GetService<IdentityService>();
      IList<Microsoft.VisualStudio.Services.Identity.Identity> source1 = service.ReadIdentities(context, (IList<Guid>) list1.Select<KeyValuePair<DirectoryEntityIdentifier, Guid>, Guid>((Func<KeyValuePair<DirectoryEntityIdentifier, Guid>, Guid>) (kvp => kvp.Value)).ToList<Guid>(), QueryMembership.Direct, (IEnumerable<string>) null);
      HashSet<IdentityDescriptor> source2 = new HashSet<IdentityDescriptor>(source1.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (identity => identity != null)).SelectMany<Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor>((Func<Microsoft.VisualStudio.Services.Identity.Identity, IEnumerable<IdentityDescriptor>>) (identity => (IEnumerable<IdentityDescriptor>) identity.Members)), (IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
      Dictionary<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity> memberDescriptorToMember = service.ReadIdentities(context, (IList<IdentityDescriptor>) source2.ToList<IdentityDescriptor>(), QueryMembership.None, (IEnumerable<string>) null).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (member => member != null)).ToDictionary<Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor>) (member => member.Descriptor), (Func<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>) (member => member), (IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
      for (int index = 0; index < list1.Count; ++index)
      {
        DirectoryEntityIdentifier key = list1[index].Key;
        Microsoft.VisualStudio.Services.Identity.Identity identity = source1[index];
        if (identity != null)
        {
          (int count, bool flag) = VisualStudioDirectoryGetRelatedEntitiesHelper.TryGetSkipFromPagingToken(context, request.PagingToken);
          List<IDirectoryEntity> list2 = identity.Members.Where<IdentityDescriptor>((Func<IdentityDescriptor, bool>) (memberDescriptor => memberDescriptorToMember.ContainsKey(memberDescriptor))).Select<IdentityDescriptor, IDirectoryEntity>((Func<IdentityDescriptor, IDirectoryEntity>) (memberDescriptor => VisualStudioDirectoryEntityConverter.ConvertIdentity(context, memberDescriptorToMember[memberDescriptor], properties))).Distinct<IDirectoryEntity>().OrderBy<IDirectoryEntity, string>((Func<IDirectoryEntity, string>) (entity => entity.DisplayName)).Skip<IDirectoryEntity>(count).Take<IDirectoryEntity>(request.MaxResults).ToList<IDirectoryEntity>();
          results[key] = new DirectoryInternalGetRelatedEntitiesResult()
          {
            Entities = flag ? (IEnumerable<IDirectoryEntity>) list2 : (IEnumerable<IDirectoryEntity>) new List<IDirectoryEntity>(),
            PagingToken = list2.Count == request.MaxResults & flag ? (request.MaxResults + count).ToString() : (string) null
          };
        }
        else
          results[key] = new DirectoryInternalGetRelatedEntitiesResult()
          {
            Exception = (Exception) new DirectoryEntityNotFoundException()
          };
      }
    }

    private static (int, bool) TryGetSkipFromPagingToken(
      IVssRequestContext requestContext,
      string pagingToken)
    {
      if (string.IsNullOrEmpty(pagingToken))
        return (0, true);
      int result;
      if (int.TryParse(pagingToken, out result))
        return (result, true);
      requestContext.Trace(862178, TraceLevel.Error, "DirectoryInternalGetRelatedEntitiesResponse", "DirectoryInternalGetRelatedEntitiesResponse", FrameworkResources.VSDInvalidPagingToken((object) pagingToken));
      return (0, false);
    }
  }
}
