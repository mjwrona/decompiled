// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.VisualStudioDirectoryGetEntitiesHelper
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
  internal static class VisualStudioDirectoryGetEntitiesHelper
  {
    internal static DirectoryInternalGetEntitiesResponse GetEntities(
      IVssRequestContext context,
      DirectoryInternalGetEntitiesRequest request)
    {
      DirectoryInternalGetEntitiesResponse entities = new DirectoryInternalGetEntitiesResponse()
      {
        Results = (IDictionary<DirectoryEntityIdentifier, DirectoryInternalGetEntityResult>) request.EntityIds.ToDictionary<DirectoryEntityIdentifier, DirectoryEntityIdentifier, DirectoryInternalGetEntityResult>((Func<DirectoryEntityIdentifier, DirectoryEntityIdentifier>) (id => id), (Func<DirectoryEntityIdentifier, DirectoryInternalGetEntityResult>) (id => (DirectoryInternalGetEntityResult) null), (IEqualityComparer<DirectoryEntityIdentifier>) DirectoryEntityIdentifier.Comparer)
      };
      if (VisualStudioDirectoryRequestFilter.AllowRequest(context, (DirectoryInternalRequest) request))
      {
        List<KeyValuePair<DirectoryEntityIdentifier, Guid>> list = VisualStudioDirectoryVsidResolver.Instance.ResolveVsids(context, request.EntityIds).Where<KeyValuePair<DirectoryEntityIdentifier, Guid>>((Func<KeyValuePair<DirectoryEntityIdentifier, Guid>, bool>) (kvp => kvp.Value != Guid.Empty)).ToList<KeyValuePair<DirectoryEntityIdentifier, Guid>>();
        if (list.Count > 0)
        {
          IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = context.GetService<IdentityService>().ReadIdentities(context, (IList<Guid>) list.Select<KeyValuePair<DirectoryEntityIdentifier, Guid>, Guid>((Func<KeyValuePair<DirectoryEntityIdentifier, Guid>, Guid>) (kvp => kvp.Value)).ToList<Guid>(), QueryMembership.None, (IEnumerable<string>) null);
          for (int index = 0; index < list.Count; ++index)
          {
            DirectoryEntityIdentifier key = list[index].Key;
            Microsoft.VisualStudio.Services.Identity.Identity identity = identityList[index];
            if (identity != null)
              entities.Results[key] = new DirectoryInternalGetEntityResult()
              {
                Entity = VisualStudioDirectoryEntityConverter.ConvertIdentity(context, identity, request.PropertiesToReturn)
              };
            else
              entities.Results[key] = new DirectoryInternalGetEntityResult()
              {
                Exception = (Exception) new DirectoryEntityNotFoundException()
              };
          }
        }
      }
      return entities;
    }

    private static void ResolveImsIds(
      IVssRequestContext context,
      IEnumerable<string> properties,
      IDictionary<DirectoryEntityIdentifierV1, DirectoryInternalGetEntityResult> results)
    {
      List<KeyValuePair<DirectoryEntityIdentifierV1, Guid>> source = new List<KeyValuePair<DirectoryEntityIdentifierV1, Guid>>(results.Count);
      foreach (KeyValuePair<DirectoryEntityIdentifierV1, DirectoryInternalGetEntityResult> result1 in (IEnumerable<KeyValuePair<DirectoryEntityIdentifierV1, DirectoryInternalGetEntityResult>>) results)
      {
        Guid result2;
        if (result1.Value != null && "ims".Equals(result1.Key.Source) && Guid.TryParseExact(result1.Key.Id, "N", out result2))
          source.Add(new KeyValuePair<DirectoryEntityIdentifierV1, Guid>(result1.Key, result2));
      }
      if (source.Count == 0)
        return;
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = context.GetService<IdentityService>().ReadIdentities(context, (IList<Guid>) source.Select<KeyValuePair<DirectoryEntityIdentifierV1, Guid>, Guid>((Func<KeyValuePair<DirectoryEntityIdentifierV1, Guid>, Guid>) (kvp => kvp.Value)).ToList<Guid>(), QueryMembership.None, (IEnumerable<string>) null);
      for (int index = 0; index < source.Count; ++index)
      {
        DirectoryEntityIdentifierV1 key = source[index].Key;
        Microsoft.VisualStudio.Services.Identity.Identity identity = identityList[index];
        if (identity != null)
          results[key] = new DirectoryInternalGetEntityResult()
          {
            Entity = VisualStudioDirectoryEntityConverter.ConvertIdentity(context, identity, properties)
          };
        else
          results[key] = new DirectoryInternalGetEntityResult()
          {
            Exception = (Exception) new DirectoryEntityNotFoundException()
          };
      }
    }

    private static void ResolveAadIds(
      IVssRequestContext context,
      IEnumerable<string> properties,
      IDictionary<DirectoryEntityIdentifierV1, DirectoryInternalGetEntityResult> results)
    {
      List<KeyValuePair<DirectoryEntityIdentifierV1, Guid>> source = new List<KeyValuePair<DirectoryEntityIdentifierV1, Guid>>(results.Count);
      foreach (KeyValuePair<DirectoryEntityIdentifierV1, DirectoryInternalGetEntityResult> result1 in (IEnumerable<KeyValuePair<DirectoryEntityIdentifierV1, DirectoryInternalGetEntityResult>>) results)
      {
        Guid result2;
        if (result1.Value != null && "ims".Equals(result1.Key.Source) && Guid.TryParseExact(result1.Key.Id, "N", out result2))
          source.Add(new KeyValuePair<DirectoryEntityIdentifierV1, Guid>(result1.Key, result2));
      }
      if (source.Count == 0)
        return;
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = context.GetService<IdentityService>().ReadIdentities(context, (IList<Guid>) source.Select<KeyValuePair<DirectoryEntityIdentifierV1, Guid>, Guid>((Func<KeyValuePair<DirectoryEntityIdentifierV1, Guid>, Guid>) (kvp => kvp.Value)).ToList<Guid>(), QueryMembership.None, (IEnumerable<string>) null);
      for (int index = 0; index < source.Count; ++index)
      {
        DirectoryEntityIdentifierV1 key = source[index].Key;
        Microsoft.VisualStudio.Services.Identity.Identity identity = identityList[index];
        if (identity != null)
          results[key] = new DirectoryInternalGetEntityResult()
          {
            Entity = VisualStudioDirectoryEntityConverter.ConvertIdentity(context, identity, properties)
          };
        else
          results[key] = new DirectoryInternalGetEntityResult()
          {
            Exception = (Exception) new DirectoryEntityNotFoundException()
          };
      }
    }
  }
}
