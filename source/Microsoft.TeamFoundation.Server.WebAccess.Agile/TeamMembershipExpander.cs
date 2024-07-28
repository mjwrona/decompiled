// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.TeamMembershipExpander
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Directories;
using Microsoft.VisualStudio.Services.Directories.DiscoveryService;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile
{
  internal class TeamMembershipExpander
  {
    private const int DEFAULT_MAX_AAD_MEMBERS = 200;
    private const int DEFAULT_MAX_AAD_REQUESTS = 10;
    private const string c_backlogsTeamSettingsRootPath = "/Configuration/Application/Backlogs/Team";
    private const string c_maxAADMembersToRead = "/MaxAADMembersToRead";
    private const string c_maxAADRequests = "/MaxAADRequests";

    public virtual IList<Microsoft.VisualStudio.Services.Identity.Identity> GetMaterializedTeamMembers(
      IVssRequestContext requestContext,
      Guid teamTeamFoundationId)
    {
      requestContext.TraceEnter(290261, "Agile", TfsTraceLayers.Controller, "GetAllMembersIncludingAADGroups");
      try
      {
        RegistryEntryCollection registryEntryCollection = requestContext.GetService<CachedRegistryService>().ReadEntriesFallThru(requestContext, (RegistryQuery) "/Configuration/Application/Backlogs/Team/*");
        int valueFromPath1 = registryEntryCollection.GetValueFromPath<int>("/Configuration/Application/Backlogs/Team/MaxAADMembersToRead", 200);
        int valueFromPath2 = registryEntryCollection.GetValueFromPath<int>("/Configuration/Application/Backlogs/Team/MaxAADRequests", 10);
        IList<Guid> list = (IList<Guid>) ((IEnumerable<string>) this.GetMaterializedUsersFromGroup(requestContext, teamTeamFoundationId, valueFromPath1, valueFromPath2)).Select<string, Guid>((Func<string, Guid>) (m => Guid.Parse(m))).ToList<Guid>();
        return requestContext.GetService<IdentityService>().ReadIdentities(requestContext, list, Microsoft.VisualStudio.Services.Identity.QueryMembership.None, (IEnumerable<string>) null);
      }
      finally
      {
        requestContext.TraceLeave(290265, "Agile", TfsTraceLayers.Controller, "GetAllMembersIncludingAADGroups");
      }
    }

    public virtual string[] GetMaterializedUsersFromGroup(
      IVssRequestContext requestContext,
      Guid groupTeamFoundationId,
      int maxAADMembers,
      int maxAADRequests)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      int num1 = 0;
      int num2 = 0;
      string id = groupTeamFoundationId.ToString();
      List<string> stringList1 = new List<string>();
      List<string> stringList2 = new List<string>();
      stringList2.Add(this.ConvertKey(requestContext, id, "VisualStudioIdentifier", "DirectoryEntityIdentifier"));
      int num3 = 0;
      while (stringList2.Count > 0 && num3 < 1000)
      {
        ++num3;
        string[] array = stringList2.ToArray();
        stringList2.Clear();
        foreach (IDirectoryEntity directMember in this.GetDirectMembers(requestContext, array))
        {
          switch (directMember.OriginDirectory)
          {
            case "vsd":
            case "ad":
            case "wmd":
              if ("User".Equals(directMember.EntityType))
              {
                stringList1.Add(directMember.EntityId);
                continue;
              }
              if ("Group".Equals(directMember.EntityType))
              {
                stringList2.Add(directMember.EntityId);
                continue;
              }
              continue;
            case "aad":
              if ("User".Equals(directMember.EntityType))
              {
                if (num1 < maxAADMembers)
                {
                  ++num1;
                  stringList1.Add(directMember.EntityId);
                  continue;
                }
                continue;
              }
              if ("Group".Equals(directMember.EntityType) && num2 < maxAADRequests && num1 < maxAADMembers)
              {
                ++num2;
                stringList2.Add(directMember.EntityId);
                continue;
              }
              continue;
            default:
              continue;
          }
        }
      }
      return this.ConvertKeys(requestContext, stringList1.ToArray(), "DirectoryEntityIdentifier", "VisualStudioIdentifier", typeof (DirectoryKeyNotFoundException));
    }

    public virtual IEnumerable<IDirectoryEntity> GetDirectMembers(
      IVssRequestContext requestContext,
      params string[] groupIds)
    {
      DirectoryDiscoveryService service = requestContext.GetService<DirectoryDiscoveryService>();
      string[] strArray = new string[2]{ "vsd", "aad" };
      DirectoryDiscoveryService discoveryService = service;
      IVssRequestContext context = requestContext;
      DirectoryGetRelatedEntitiesRequest relatedEntitiesRequest = new DirectoryGetRelatedEntitiesRequest();
      relatedEntitiesRequest.Directories = (IEnumerable<string>) strArray;
      relatedEntitiesRequest.Depth = 1;
      relatedEntitiesRequest.PropertiesToReturn = (IEnumerable<string>) new string[1]
      {
        "DisplayName"
      };
      relatedEntitiesRequest.Relation = "Member";
      relatedEntitiesRequest.EntityIds = (IEnumerable<string>) groupIds;
      DirectoryGetRelatedEntitiesRequest request = relatedEntitiesRequest;
      IDictionary<string, DirectoryGetRelatedEntitiesResult> results = discoveryService.GetRelatedEntities(context, request).Results;
      IEnumerable<Exception> exceptions = results.Where<KeyValuePair<string, DirectoryGetRelatedEntitiesResult>>((Func<KeyValuePair<string, DirectoryGetRelatedEntitiesResult>, bool>) (kv => kv.Value.Exception != null)).Select<KeyValuePair<string, DirectoryGetRelatedEntitiesResult>, Exception>((Func<KeyValuePair<string, DirectoryGetRelatedEntitiesResult>, Exception>) (kv => kv.Value.Exception));
      if (exceptions.Any<Exception>())
        throw new AggregateException(exceptions);
      List<IDirectoryEntity> directMembers = new List<IDirectoryEntity>();
      foreach (KeyValuePair<string, DirectoryGetRelatedEntitiesResult> keyValuePair in (IEnumerable<KeyValuePair<string, DirectoryGetRelatedEntitiesResult>>) results)
        directMembers.AddRange(keyValuePair.Value.Entities);
      return (IEnumerable<IDirectoryEntity>) directMembers;
    }

    public virtual string ConvertKey(
      IVssRequestContext requestContext,
      string id,
      string from,
      string to,
      System.Type expectedException = null)
    {
      return this.ConvertKeys(requestContext, new string[1]
      {
        id
      }, from, to, expectedException)[0];
    }

    public virtual string[] ConvertKeys(
      IVssRequestContext requestContext,
      string[] ids,
      string from,
      string to,
      System.Type expectedException = null)
    {
      IEnumerable<KeyValuePair<string, DirectoryConvertKeyResult>> source = requestContext.GetService<DirectoryDiscoveryService>().ConvertKeys(requestContext, new DirectoryConvertKeysRequest()
      {
        ConvertFrom = from,
        ConvertTo = to,
        Keys = (IEnumerable<string>) ids
      }).Results.Where<KeyValuePair<string, DirectoryConvertKeyResult>>((Func<KeyValuePair<string, DirectoryConvertKeyResult>, bool>) (r => r.Value.Exception == null || r.Value.Exception.GetType() != expectedException));
      IEnumerable<Exception> exceptions = source.Where<KeyValuePair<string, DirectoryConvertKeyResult>>((Func<KeyValuePair<string, DirectoryConvertKeyResult>, bool>) (r => r.Value.Exception != null)).Select<KeyValuePair<string, DirectoryConvertKeyResult>, Exception>((Func<KeyValuePair<string, DirectoryConvertKeyResult>, Exception>) (r => r.Value.Exception));
      if (exceptions.Any<Exception>())
        throw new AggregateException(exceptions);
      return source.Select<KeyValuePair<string, DirectoryConvertKeyResult>, string>((Func<KeyValuePair<string, DirectoryConvertKeyResult>, string>) (r => r.Value.Key)).ToArray<string>();
    }
  }
}
