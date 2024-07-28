// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.VisualStudioDirectoryVsidResolver
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal class VisualStudioDirectoryVsidResolver
  {
    internal const string DisableAadTokenElevation = "VisualStudio.Services.IdentityPicker.DisableUidElevation";

    public static VisualStudioDirectoryVsidResolver Instance
    {
      get => VisualStudioDirectoryVsidResolver.Nested.Instance;
      internal set => VisualStudioDirectoryVsidResolver.Nested.Instance = value;
    }

    internal virtual IDictionary<DirectoryEntityIdentifier, Guid> ResolveVsids(
      IVssRequestContext context,
      IEnumerable<DirectoryEntityIdentifier> entityIds)
    {
      Dictionary<DirectoryEntityIdentifier, Guid> dictionary = entityIds.ToDictionary<DirectoryEntityIdentifier, DirectoryEntityIdentifier, Guid>((Func<DirectoryEntityIdentifier, DirectoryEntityIdentifier>) (objectId => objectId), (Func<DirectoryEntityIdentifier, Guid>) (objectId => Guid.Empty), (IEqualityComparer<DirectoryEntityIdentifier>) DirectoryEntityIdentifier.Comparer);
      List<DirectoryEntityIdentifierV1> ids = new List<DirectoryEntityIdentifierV1>(entityIds.Count<DirectoryEntityIdentifier>());
      List<DirectoryEntityIdentifierV1> groupIds = new List<DirectoryEntityIdentifierV1>(entityIds.Count<DirectoryEntityIdentifier>());
      List<DirectoryEntityIdentifierV1> gitHubUserEntityIds = new List<DirectoryEntityIdentifierV1>();
      List<(DirectoryEntityIdentifier, IdentityDescriptor)> valueTupleList1 = new List<(DirectoryEntityIdentifier, IdentityDescriptor)>(entityIds.Count<DirectoryEntityIdentifier>());
      List<(DirectoryEntityIdentifier, SubjectDescriptor)> valueTupleList2 = new List<(DirectoryEntityIdentifier, SubjectDescriptor)>(entityIds.Count<DirectoryEntityIdentifier>());
      foreach (DirectoryEntityIdentifier entityId in entityIds)
      {
        if (entityId.Version == 1 && entityId is DirectoryEntityIdentifierV1 entityIdentifierV1)
        {
          switch (entityIdentifierV1.Source)
          {
            case "ims":
              Guid vsid;
              if (DirectoryConvertKeysHelper.TryGetVsid(entityIdentifierV1, out vsid))
              {
                dictionary[(DirectoryEntityIdentifier) entityIdentifierV1] = vsid;
                continue;
              }
              continue;
            case "aad":
              switch (entityIdentifierV1.Type)
              {
                case "user":
                case "servicePrincipal":
                  ids.Add(entityIdentifierV1);
                  continue;
                case "group":
                  groupIds.Add(entityIdentifierV1);
                  continue;
                default:
                  continue;
              }
            case "ghb":
              if (entityIdentifierV1.Type == "user")
              {
                gitHubUserEntityIds.Add(entityIdentifierV1);
                continue;
              }
              continue;
            case "ad":
              string objectSid1 = (string) null;
              if (ActiveDirectoryHelper.TryGetObjectSid(entityId, out objectSid1))
              {
                valueTupleList1.Add(((DirectoryEntityIdentifier) entityIdentifierV1, IdentityHelper.CreateWindowsDescriptor(objectSid1)));
                continue;
              }
              continue;
            case "wmd":
              SecurityIdentifier objectSid2 = (SecurityIdentifier) null;
              if (WindowsMachineDirectoryHelper.TryGetObjectSid(entityId, out objectSid2))
              {
                valueTupleList1.Add(((DirectoryEntityIdentifier) entityIdentifierV1, IdentityHelper.CreateWindowsDescriptor(objectSid2)));
                continue;
              }
              continue;
            default:
              continue;
          }
        }
      }
      VisualStudioDirectoryVsidResolver.ResolveAadMembersToIdentityDescriptors(context, ids, valueTupleList1, valueTupleList2);
      VisualStudioDirectoryVsidResolver.ResolveAadGroupsToIdentityDescriptors(context, groupIds, valueTupleList1);
      IdentityService service = context.GetService<IdentityService>();
      if (valueTupleList1.Count > 0)
      {
        IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = service.ReadIdentities(context, (IList<IdentityDescriptor>) valueTupleList1.Select<(DirectoryEntityIdentifier, IdentityDescriptor), IdentityDescriptor>((Func<(DirectoryEntityIdentifier, IdentityDescriptor), IdentityDescriptor>) (kvp => kvp.descriptor)).ToList<IdentityDescriptor>(), QueryMembership.None, (IEnumerable<string>) null);
        for (int index = 0; index < valueTupleList1.Count; ++index)
        {
          DirectoryEntityIdentifier key = valueTupleList1[index].Item1;
          Microsoft.VisualStudio.Services.Identity.Identity identity = identityList[index];
          if (identity != null)
            dictionary[key] = identity.Id;
        }
      }
      if (valueTupleList2.Count > 0)
      {
        int batchSize = context.GetService<IVssRegistryService>().GetValue<int>(context, (RegistryQuery) "/Service/Licensing/VsidConversionReadIdentityBatchSize", 50);
        foreach (IList<(DirectoryEntityIdentifier, SubjectDescriptor)> source in valueTupleList2.Batch<(DirectoryEntityIdentifier, SubjectDescriptor)>(batchSize))
        {
          IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = service.ReadIdentities(context, (IList<SubjectDescriptor>) source.Select<(DirectoryEntityIdentifier, SubjectDescriptor), SubjectDescriptor>((Func<(DirectoryEntityIdentifier, SubjectDescriptor), SubjectDescriptor>) (kvp => kvp.subjectDescriptor)).ToList<SubjectDescriptor>(), QueryMembership.None, (IEnumerable<string>) null);
          for (int index = 0; index < source.Count; ++index)
          {
            DirectoryEntityIdentifier key = source[index].Item1;
            Microsoft.VisualStudio.Services.Identity.Identity identity = identityList[index];
            if (identity != null)
              dictionary[key] = identity.Id;
          }
        }
      }
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList1 = VisualStudioDirectoryVsidResolver.ReadGitHubUserIdentitiesInIMS(context, gitHubUserEntityIds);
      for (int index = 0; index < gitHubUserEntityIds.Count; ++index)
      {
        DirectoryEntityIdentifierV1 key = gitHubUserEntityIds[index];
        Microsoft.VisualStudio.Services.Identity.Identity identity = identityList1[index];
        if (identity != null)
          dictionary[(DirectoryEntityIdentifier) key] = identity.Id;
      }
      return (IDictionary<DirectoryEntityIdentifier, Guid>) dictionary;
    }

    private static void ResolveAadMembersToIdentityDescriptors(
      IVssRequestContext context,
      List<DirectoryEntityIdentifierV1> ids,
      List<(DirectoryEntityIdentifier identifier, IdentityDescriptor descriptor)> descriptors,
      List<(DirectoryEntityIdentifier identifier, SubjectDescriptor subjectDescriptor)> subjectDescriptors)
    {
      Guid tenantId;
      if (ids.IsNullOrEmpty<DirectoryEntityIdentifierV1>() || !DirectoryUtils.TryGetOrganizationTenantId(context, out tenantId))
        return;
      string domain = tenantId.ToString();
      List<KeyValuePair<DirectoryEntityIdentifierV1, string>> list = ids.Select<DirectoryEntityIdentifierV1, KeyValuePair<DirectoryEntityIdentifierV1, string>>((Func<DirectoryEntityIdentifierV1, KeyValuePair<DirectoryEntityIdentifierV1, string>>) (id => new KeyValuePair<DirectoryEntityIdentifierV1, string>(id, id.Encode()))).ToList<KeyValuePair<DirectoryEntityIdentifierV1, string>>();
      DirectoryDiscoveryService service = context.GetService<DirectoryDiscoveryService>();
      DirectoryGetEntitiesInternalRequest entitiesInternalRequest = new DirectoryGetEntitiesInternalRequest();
      entitiesInternalRequest.Directories = (IEnumerable<string>) new string[1]
      {
        "aad"
      };
      entitiesInternalRequest.EntityIds = list.Select<KeyValuePair<DirectoryEntityIdentifierV1, string>, string>((Func<KeyValuePair<DirectoryEntityIdentifierV1, string>, string>) (kvp => kvp.Value));
      entitiesInternalRequest.PropertiesToReturn = (IEnumerable<string>) new string[2]
      {
        "SignInAddress",
        "SubjectDescriptor"
      };
      DirectoryGetEntitiesInternalRequest request = entitiesInternalRequest;
      IVssRequestContext context1 = (context.IsFeatureEnabled("VisualStudio.Services.IdentityPicker.DisableUidElevation") ? 0 : (!DirectoryUtils.IsRequestByAadGuestUser(context) ? 1 : 0)) != 0 ? context.Elevate() : context;
      DirectoryGetEntitiesResponse entitiesInternal = service.GetEntitiesInternal(context1, request);
      foreach (KeyValuePair<DirectoryEntityIdentifierV1, string> keyValuePair in list)
      {
        IDirectoryEntity entity = entitiesInternal.Results[keyValuePair.Value].Entity;
        if (entity != null && "User".Equals(entity.EntityType))
        {
          DirectoryUser directoryUser = (DirectoryUser) entity;
          if (directoryUser.SubjectDescriptor.HasValue)
            subjectDescriptors.Add(((DirectoryEntityIdentifier) keyValuePair.Key, directoryUser.SubjectDescriptor.Value));
          else if (directoryUser.SignInAddress != null)
          {
            IdentityDescriptor descriptorFromAccountName1 = IdentityHelper.CreateDescriptorFromAccountName(domain, directoryUser.SignInAddress, true);
            descriptors.Add(((DirectoryEntityIdentifier) keyValuePair.Key, descriptorFromAccountName1));
            IdentityDescriptor descriptorFromAccountName2 = IdentityHelper.CreateDescriptorFromAccountName(domain, directoryUser.SignInAddress);
            descriptors.Add(((DirectoryEntityIdentifier) keyValuePair.Key, descriptorFromAccountName2));
          }
        }
        else if (AadServicePrincipalConfigurationHelper.Instance.IsGroupRulesForServicePrincipalsEnabled(context) && entity != null && "ServicePrincipal".Equals(entity.EntityType))
        {
          DirectoryServicePrincipal servicePrincipal = (DirectoryServicePrincipal) entity;
          IdentityDescriptor principalDescriptor = IdentityHelper.CreateAadServicePrincipalDescriptor(domain, servicePrincipal.OriginId);
          descriptors.Add(((DirectoryEntityIdentifier) keyValuePair.Key, principalDescriptor));
        }
      }
    }

    private static void ResolveAadGroupsToIdentityDescriptors(
      IVssRequestContext context,
      List<DirectoryEntityIdentifierV1> groupIds,
      List<(DirectoryEntityIdentifier identifier, IdentityDescriptor descriptor)> descriptors)
    {
      foreach (DirectoryEntityIdentifierV1 groupId in groupIds)
      {
        Guid result;
        if (Guid.TryParseExact(groupId.Id, "N", out result))
        {
          IdentityDescriptor descriptorFromSid = IdentityHelper.CreateDescriptorFromSid(SidIdentityHelper.ConstructAadGroupSid(result));
          descriptors.Add(((DirectoryEntityIdentifier) groupId, descriptorFromSid));
        }
      }
    }

    private static IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadGitHubUserIdentitiesInIMS(
      IVssRequestContext context,
      List<DirectoryEntityIdentifierV1> gitHubUserEntityIds)
    {
      List<SocialDescriptor> list = gitHubUserEntityIds.Select<DirectoryEntityIdentifierV1, SocialDescriptor>((Func<DirectoryEntityIdentifierV1, SocialDescriptor>) (x => new SocialDescriptor("ghb", x.Id))).ToList<SocialDescriptor>();
      return context.GetService<IdentityService>().ReadIdentities(context, (IList<SocialDescriptor>) list, QueryMembership.None, (IEnumerable<string>) null);
    }

    private class Nested
    {
      internal static VisualStudioDirectoryVsidResolver Instance = new VisualStudioDirectoryVsidResolver();
    }
  }
}
