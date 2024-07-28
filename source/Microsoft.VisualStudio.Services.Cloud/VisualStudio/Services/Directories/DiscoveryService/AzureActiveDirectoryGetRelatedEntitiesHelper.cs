// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.AzureActiveDirectoryGetRelatedEntitiesHelper
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Aad;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal static class AzureActiveDirectoryGetRelatedEntitiesHelper
  {
    private const int c_defaultMaxManagerLevels = 6;

    internal static DirectoryInternalGetRelatedEntitiesResponse GetRelatedEntities(
      IVssRequestContext context,
      DirectoryInternalGetRelatedEntitiesRequest request)
    {
      IDictionary<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntitiesResult> results = (IDictionary<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntitiesResult>) new Dictionary<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntitiesResult>((IEqualityComparer<DirectoryEntityIdentifier>) DirectoryEntityIdentifier.Comparer);
      if (AzureActiveDirectoryRequestFilter.AllowRequest(context, (DirectoryInternalRequest) request))
      {
        bool relatedEntities = AzureActiveDirectoryGetRelatedEntitiesHelper.TryGetRelatedEntities(context, request, out results);
        if (!context.IsFeatureEnabled("VisualStudio.Services.IdentityPicker.DisableUidElevation") && !relatedEntities && !DirectoryUtils.IsRequestByAadGuestUser(context))
        {
          AzureActiveDirectory.s_elevatedRequestsPerfCounter.Increment();
          AzureActiveDirectoryGetRelatedEntitiesHelper.TryGetRelatedEntities(context.Elevate(), request, out results);
        }
      }
      return new DirectoryInternalGetRelatedEntitiesResponse()
      {
        Results = results
      };
    }

    private static bool TryGetRelatedEntities(
      IVssRequestContext context,
      DirectoryInternalGetRelatedEntitiesRequest request,
      out IDictionary<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntitiesResult> results)
    {
      results = (IDictionary<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntitiesResult>) request.EntityIds.ToDictionary<DirectoryEntityIdentifier, DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntitiesResult>((Func<DirectoryEntityIdentifier, DirectoryEntityIdentifier>) (id => id), (Func<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntitiesResult>) (id => (DirectoryInternalGetRelatedEntitiesResult) null), (IEqualityComparer<DirectoryEntityIdentifier>) DirectoryEntityIdentifier.Comparer);
      switch (request.Relation)
      {
        case "Manager":
          AzureActiveDirectoryGetRelatedEntitiesHelper.GetDirectManagerChain(context, request.PropertiesToReturn, request.Depth, results);
          break;
        case "DirectReports":
          if (request.Depth == 1)
          {
            AzureActiveDirectoryGetRelatedEntitiesHelper.GetDirectReports(context, request.PropertiesToReturn, results);
            break;
          }
          break;
        case "Member":
          if (request.Depth == 1)
          {
            AzureActiveDirectoryGetRelatedEntitiesHelper.GetDirectMembers(context, request.PropertiesToReturn, results, request.MaxResults, request.PagingToken);
            break;
          }
          break;
      }
      return !results.All<KeyValuePair<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntitiesResult>>((Func<KeyValuePair<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntitiesResult>, bool>) (kvp => kvp.Value?.Exception is DirectoryDiscoveryServiceAccessException));
    }

    private static void GetDirectManagerChain(
      IVssRequestContext context,
      IEnumerable<string> properties,
      int depth,
      IDictionary<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntitiesResult> results)
    {
      List<KeyValuePair<DirectoryEntityIdentifier, Guid>> source = new List<KeyValuePair<DirectoryEntityIdentifier, Guid>>(results.Count);
      foreach (KeyValuePair<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntitiesResult> result in (IEnumerable<KeyValuePair<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntitiesResult>>) results)
      {
        Guid oid;
        if (AzureActiveDirectoryEntityIdentifierHelper.TryGetUserOid(result.Key, out oid))
          source.Add(new KeyValuePair<DirectoryEntityIdentifier, Guid>(result.Key, oid));
      }
      if (source.Count > 1)
        return;
      try
      {
        AadService service = context.GetService<AadService>();
        List<IDirectoryEntity> directoryEntityList = new List<IDirectoryEntity>();
        int num = 1;
        if (depth > 1)
          num = Math.Min(depth, 6);
        DirectoryEntityIdentifier key = source.Select<KeyValuePair<DirectoryEntityIdentifier, Guid>, DirectoryEntityIdentifier>((Func<KeyValuePair<DirectoryEntityIdentifier, Guid>, DirectoryEntityIdentifier>) (kvp => kvp.Key)).Single<DirectoryEntityIdentifier>();
        Guid guid = source.Select<KeyValuePair<DirectoryEntityIdentifier, Guid>, Guid>((Func<KeyValuePair<DirectoryEntityIdentifier, Guid>, Guid>) (kvp => kvp.Value)).Single<Guid>();
        for (int index = 0; index < num; ++index)
        {
          AadUser aadUser = service.GetUsersWithIds<Guid>(context, new GetUsersWithIdsRequest<Guid>()
          {
            Identifiers = (IEnumerable<Guid>) new Guid[1]
            {
              guid
            },
            ExpandProperty = "Manager"
          }).Users.Values.Single<AadUser>();
          if (aadUser != null && aadUser.Manager != null)
          {
            directoryEntityList.Add((IDirectoryEntity) AzureActiveDirectoryEntityConverter.ConvertUser(context, aadUser.Manager, properties));
            guid = aadUser.Manager.ObjectId;
          }
          else
            break;
        }
        results[key] = new DirectoryInternalGetRelatedEntitiesResult()
        {
          Entities = (IEnumerable<IDirectoryEntity>) directoryEntityList
        };
      }
      catch (AadException ex)
      {
        foreach (KeyValuePair<DirectoryEntityIdentifier, Guid> keyValuePair in source)
        {
          DirectoryEntityIdentifier key = keyValuePair.Key;
          results[key] = new DirectoryInternalGetRelatedEntitiesResult()
          {
            Exception = (Exception) AzureActiveDirectoryGetRelatedEntitiesHelper.WrapAadException(ex)
          };
        }
      }
    }

    private static void GetDirectReports(
      IVssRequestContext context,
      IEnumerable<string> properties,
      IDictionary<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntitiesResult> results)
    {
      List<KeyValuePair<DirectoryEntityIdentifier, Guid>> source = new List<KeyValuePair<DirectoryEntityIdentifier, Guid>>(results.Count);
      foreach (KeyValuePair<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntitiesResult> result in (IEnumerable<KeyValuePair<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntitiesResult>>) results)
      {
        Guid oid;
        if (AzureActiveDirectoryEntityIdentifierHelper.TryGetUserOid(result.Key, out oid))
          source.Add(new KeyValuePair<DirectoryEntityIdentifier, Guid>(result.Key, oid));
      }
      if (source.Count == 0)
        return;
      try
      {
        GetUsersWithIdsResponse<Guid> usersWithIds = context.GetService<AadService>().GetUsersWithIds<Guid>(context, new GetUsersWithIdsRequest<Guid>()
        {
          Identifiers = (IEnumerable<Guid>) source.Select<KeyValuePair<DirectoryEntityIdentifier, Guid>, Guid>((Func<KeyValuePair<DirectoryEntityIdentifier, Guid>, Guid>) (kvp => kvp.Value)).ToList<Guid>(),
          ExpandProperty = "DirectReports"
        });
        foreach (KeyValuePair<DirectoryEntityIdentifier, Guid> keyValuePair in source)
        {
          DirectoryEntityIdentifier key = keyValuePair.Key;
          AadUser user = usersWithIds.Users[keyValuePair.Value];
          if (user != null)
          {
            if (user.DirectReports != null)
            {
              List<IDirectoryEntity> directoryEntityList = new List<IDirectoryEntity>();
              foreach (AadUser directReport in user.DirectReports)
                directoryEntityList.Add((IDirectoryEntity) AzureActiveDirectoryEntityConverter.ConvertUser(context, directReport, properties));
              results[key] = new DirectoryInternalGetRelatedEntitiesResult()
              {
                Entities = (IEnumerable<IDirectoryEntity>) directoryEntityList
              };
            }
            else
              results[key] = new DirectoryInternalGetRelatedEntitiesResult()
              {
                Entities = (IEnumerable<IDirectoryEntity>) Array.Empty<IDirectoryEntity>()
              };
          }
          else
            results[key] = new DirectoryInternalGetRelatedEntitiesResult()
            {
              Exception = (Exception) new DirectoryEntityNotFoundException()
            };
        }
      }
      catch (AadException ex)
      {
        foreach (KeyValuePair<DirectoryEntityIdentifier, Guid> keyValuePair in source)
        {
          DirectoryEntityIdentifier key = keyValuePair.Key;
          results[key] = new DirectoryInternalGetRelatedEntitiesResult()
          {
            Exception = (Exception) AzureActiveDirectoryGetRelatedEntitiesHelper.WrapAadException(ex)
          };
        }
      }
    }

    private static void GetDirectMembers(
      IVssRequestContext context,
      IEnumerable<string> properties,
      IDictionary<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntitiesResult> results,
      int maxResults,
      string pagingToken)
    {
      AadService service = context.GetService<AadService>();
      foreach (KeyValuePair<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntitiesResult> keyValuePair in results.ToList<KeyValuePair<DirectoryEntityIdentifier, DirectoryInternalGetRelatedEntitiesResult>>())
      {
        Guid oid;
        if (AzureActiveDirectoryEntityIdentifierHelper.TryGetGroupOid(keyValuePair.Key, out oid))
        {
          try
          {
            AadService aadService = service;
            IVssRequestContext context1 = context;
            GetDescendantsRequest<Guid> request = new GetDescendantsRequest<Guid>();
            request.Identifier = oid;
            request.Expand = 1;
            request.MaxResults = new int?(maxResults);
            request.PagingToken = pagingToken;
            GetDescendantsResponse descendants = aadService.GetDescendants<Guid>(context1, request);
            results[keyValuePair.Key] = new DirectoryInternalGetRelatedEntitiesResult()
            {
              Entities = (IEnumerable<IDirectoryEntity>) descendants.Descendants.Select<AadObject, IDirectoryEntity>((Func<AadObject, IDirectoryEntity>) (aadObject => AzureActiveDirectoryEntityConverter.ConvertObject(context, aadObject, properties))).Where<IDirectoryEntity>((Func<IDirectoryEntity, bool>) (entity => entity != null)).Distinct<IDirectoryEntity>().ToList<IDirectoryEntity>(),
              PagingToken = descendants.PagingToken
            };
          }
          catch (AadException ex)
          {
            results[keyValuePair.Key] = new DirectoryInternalGetRelatedEntitiesResult()
            {
              Exception = (Exception) AzureActiveDirectoryGetRelatedEntitiesHelper.WrapAadException(ex)
            };
          }
        }
      }
    }

    private static DirectoryDiscoveryServiceException WrapAadException(AadException e)
    {
      switch (e)
      {
        case AadAccessException _:
          return (DirectoryDiscoveryServiceException) new DirectoryDiscoveryServiceAccessException("AAD threw Access exception.", (Exception) e);
        case AadCredentialsNotFoundException _:
          return (DirectoryDiscoveryServiceException) new DirectoryDiscoveryServiceAccessException("AAD threw CredentialsNotFoundException exception.", (Exception) e);
        default:
          return (DirectoryDiscoveryServiceException) new DirectoryRelatedEntitiesUnavailableException("AADS threw exception.", (Exception) e);
      }
    }
  }
}
