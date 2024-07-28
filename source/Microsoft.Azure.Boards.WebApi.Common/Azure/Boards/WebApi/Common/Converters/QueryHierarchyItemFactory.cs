// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.Converters.QueryHierarchyItemFactory
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.Azure.Boards.Linking;
using Microsoft.Azure.Boards.WebApi.Common.Extensions;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Boards.WebApi.Common.Converters
{
  public class QueryHierarchyItemFactory
  {
    public static QueryHierarchyItem Create(
      IVssRequestContext requestContext,
      QueryItem queryItem,
      bool throwOnInvalidSyntax,
      QueryResponseOptions options)
    {
      return QueryHierarchyItemFactory.Create(requestContext.WitContext(), queryItem, throwOnInvalidSyntax, options);
    }

    public static QueryHierarchyItem Create(
      WorkItemTrackingRequestContext witRequestContext,
      QueryItem queryItem,
      bool throwOnInvalidSyntax,
      QueryResponseOptions options)
    {
      IDictionary<Guid, IdentityReference> identityMap = QueryHierarchyItemFactory.BuildIdentityMap(witRequestContext.RequestContext, (IList<QueryItem>) new QueryItem[1]
      {
        queryItem
      }, (options.IncludeLinks ? 1 : 0) != 0);
      return queryItem is QueryFolder ? QueryHierarchyItemFactory.Create(witRequestContext, (QueryFolder) queryItem, throwOnInvalidSyntax, options, identityMap) : QueryHierarchyItemFactory.Create(witRequestContext, (Query) queryItem, throwOnInvalidSyntax, options, identityMap);
    }

    public static IList<QueryHierarchyItem> Create(
      WorkItemTrackingRequestContext witRequestContext,
      IList<QueryItem> queryItems,
      bool throwOnInvalidSyntax,
      QueryResponseOptions options)
    {
      List<QueryHierarchyItem> queryHierarchyItemList = new List<QueryHierarchyItem>();
      IDictionary<Guid, IdentityReference> identityMap = QueryHierarchyItemFactory.BuildIdentityMap(witRequestContext.RequestContext, queryItems, options.IncludeLinks);
      foreach (QueryItem queryItem in (IEnumerable<QueryItem>) queryItems)
      {
        if (queryItem is QueryFolder)
          queryHierarchyItemList.Add(QueryHierarchyItemFactory.Create(witRequestContext, (QueryFolder) queryItem, throwOnInvalidSyntax, options, identityMap));
        else
          queryHierarchyItemList.Add(QueryHierarchyItemFactory.Create(witRequestContext, (Query) queryItem, throwOnInvalidSyntax, options, identityMap));
      }
      return (IList<QueryHierarchyItem>) queryHierarchyItemList;
    }

    private static QueryHierarchyItem Create(
      WorkItemTrackingRequestContext witRequestContext,
      QueryFolder queryFolder,
      bool throwOnInvalidSyntax,
      QueryResponseOptions options,
      IDictionary<Guid, IdentityReference> identityMap)
    {
      QueryHierarchyItem queryHierarchyItem = new QueryHierarchyItem();
      queryHierarchyItem.Id = queryFolder.Id;
      queryHierarchyItem.Name = queryFolder.Name;
      queryHierarchyItem.Path = queryFolder.Path;
      queryHierarchyItem.Url = options.IncludeQueryUrl ? WitUrlHelper.GetQueryUrl(witRequestContext, queryFolder.ProjectId, queryFolder.Id) : (string) null;
      queryHierarchyItem.CreatedBy = options.IncludeChangeInfo ? QueryHierarchyItemFactory.GetNullableIdentityReference(witRequestContext.RequestContext, identityMap, queryFolder.CreatedById, queryFolder.CreatedByName) : (IdentityReference) null;
      queryHierarchyItem.CreatedDate = options.IncludeChangeInfo ? QueryHierarchyItemFactory.GetNullableDate(queryFolder.CreatedDate) : new DateTime?();
      queryHierarchyItem.LastModifiedBy = options.IncludeChangeInfo ? QueryHierarchyItemFactory.GetNullableIdentityReference(witRequestContext.RequestContext, identityMap, queryFolder.ModifiedById, queryFolder.ModifiedByName) : (IdentityReference) null;
      queryHierarchyItem.LastModifiedDate = options.IncludeChangeInfo ? QueryHierarchyItemFactory.GetNullableDate(queryFolder.ModifiedDate) : new DateTime?();
      queryHierarchyItem.IsFolder = new bool?(true);
      queryHierarchyItem.IsPublic = new bool?(queryFolder.IsPublic);
      queryHierarchyItem.HasChildren = new bool?(queryFolder.HasChildren);
      queryHierarchyItem.Children = queryFolder.Children.Any<QueryItem>() ? (IList<QueryHierarchyItem>) queryFolder.Children.Select<QueryItem, QueryHierarchyItem>((Func<QueryItem, QueryHierarchyItem>) (child => QueryHierarchyItemFactory.Create(witRequestContext, child, throwOnInvalidSyntax, options))).ToList<QueryHierarchyItem>() : (IList<QueryHierarchyItem>) null;
      queryHierarchyItem.Links = options.IncludeLinks ? QueryHierarchyItemFactory.GetReferenceLinks(witRequestContext, queryFolder) : (ReferenceLinks) null;
      queryHierarchyItem.IsDeleted = queryFolder.IsDeleted;
      return queryHierarchyItem;
    }

    private static QueryHierarchyItem Create(
      WorkItemTrackingRequestContext witRequestContext,
      Query query,
      bool throwOnInvalidSyntax,
      QueryResponseOptions options,
      IDictionary<Guid, IdentityReference> identityMap)
    {
      IEnumerable<WorkItemFieldReference> itemFieldReferences = (IEnumerable<WorkItemFieldReference>) null;
      IEnumerable<WorkItemQuerySortColumn> itemQuerySortColumns = (IEnumerable<WorkItemQuerySortColumn>) null;
      Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.QueryType? nullable1 = query.QueryType.HasValue ? new Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.QueryType?(query.QueryType.Value.ToQueryType()) : new Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.QueryType?();
      bool flag1 = false;
      Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.LinkQueryMode? nullable2 = new Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.LinkQueryMode?();
      Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.QueryRecursionOption? nullable3 = new Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.QueryRecursionOption?();
      WorkItemQueryClause workItemQueryClause1 = (WorkItemQueryClause) null;
      WorkItemQueryClause workItemQueryClause2 = (WorkItemQueryClause) null;
      WorkItemQueryClause workItemQueryClause3 = (WorkItemQueryClause) null;
      WorkItemQueryClause workItemQueryClause4 = (WorkItemQueryClause) null;
      if (!string.IsNullOrEmpty(query.Wiql))
      {
        IWorkItemQueryService service = witRequestContext.RequestContext.GetService<IWorkItemQueryService>();
        try
        {
          if (throwOnInvalidSyntax || options.IncludeColumns || options.IncludeClauses)
          {
            bool includeClauses = options.IncludeClauses;
            Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression queryExpression = service.ValidateWiql(witRequestContext.RequestContext, query.Wiql, query.ProjectId, forDisplay: includeClauses);
            if (options.IncludeColumns)
            {
              WorkItemTrackingRequestContext witRequestContext1 = witRequestContext;
              IEnumerable<string> displayFields = queryExpression.DisplayFields;
              bool includeLinks = options.IncludeLinks;
              Guid? projectId1 = new Guid?();
              int num1 = includeLinks ? 1 : 0;
              itemFieldReferences = WorkItemFieldReferenceFactory.Create(witRequestContext1, displayFields, projectId: projectId1, includeUrls: num1 != 0);
              WorkItemTrackingRequestContext witRequestContext2 = witRequestContext;
              IEnumerable<QuerySortField> sortFields = queryExpression.SortFields;
              bool flag2 = !options.IncludeLinks;
              Guid? projectId2 = new Guid?();
              int num2 = flag2 ? 1 : 0;
              itemQuerySortColumns = WorkItemQuerySortColumnFactory.Create(witRequestContext2, sortFields, projectId: projectId2, excludeUrls: num2 != 0);
            }
            nullable1 = new Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.QueryType?((Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.QueryType) ((int) nullable1 ?? (int) queryExpression.QueryType.ToQueryType()));
            if (options.IncludeClauses)
            {
              Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.QueryType? nullable4 = nullable1;
              Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.QueryType queryType1 = Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.QueryType.Flat;
              if (!(nullable4.GetValueOrDefault() == queryType1 & nullable4.HasValue))
              {
                nullable2 = new Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.LinkQueryMode?(QueryHierarchyItemFactory.GetLinkTypeMode(queryExpression.QueryType));
                Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.QueryType? nullable5 = nullable1;
                Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.QueryType queryType2 = Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.QueryType.Tree;
                nullable3 = nullable5.GetValueOrDefault() == queryType2 & nullable5.HasValue ? QueryHierarchyItemFactory.GetQueryRecursionOption(queryExpression.RecursionOption) : new Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.QueryRecursionOption?();
                workItemQueryClause1 = WorkItemQueryClauseFactory.BuildQueryClauses(witRequestContext, queryExpression, WorkItemQueryClauseFactory.ClauseType.SourceClause, options.IncludeLinks, options.UseIsoDateFormat);
                workItemQueryClause4 = WorkItemQueryClauseFactory.BuildQueryClauses(witRequestContext, queryExpression, WorkItemQueryClauseFactory.ClauseType.LinkClause, options.IncludeLinks, options.UseIsoDateFormat);
                workItemQueryClause3 = WorkItemQueryClauseFactory.BuildQueryClauses(witRequestContext, queryExpression, WorkItemQueryClauseFactory.ClauseType.TargetClause, options.IncludeLinks, options.UseIsoDateFormat);
              }
              else
                workItemQueryClause2 = WorkItemQueryClauseFactory.BuildQueryClauses(witRequestContext, queryExpression, WorkItemQueryClauseFactory.ClauseType.SourceClause, options.IncludeLinks, options.UseIsoDateFormat);
            }
          }
          else
          {
            Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.QueryType queryType = service.GetQueryType(witRequestContext.RequestContext, query.Wiql).ToQueryType();
            nullable1 = new Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.QueryType?((Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.QueryType) ((int) nullable1 ?? (int) queryType));
          }
        }
        catch (Exception ex)
        {
          switch (ex)
          {
            case WorkItemTrackingQueryMaxWiqlTextLengthLimitExceededException _:
            case SyntaxException _:
              flag1 = true;
              if (throwOnInvalidSyntax)
                throw;
              else
                break;
            default:
              throw;
          }
        }
      }
      QueryHierarchyItem queryHierarchyItem = new QueryHierarchyItem();
      queryHierarchyItem.Id = query.Id;
      queryHierarchyItem.Name = query.Name;
      queryHierarchyItem.Path = query.Path;
      queryHierarchyItem.Url = options.IncludeQueryUrl ? WitUrlHelper.GetQueryUrl(witRequestContext, query.ProjectId, query.Id) : (string) null;
      queryHierarchyItem.CreatedBy = options.IncludeChangeInfo ? QueryHierarchyItemFactory.GetNullableIdentityReference(witRequestContext.RequestContext, identityMap, query.CreatedById, query.CreatedByName) : (IdentityReference) null;
      queryHierarchyItem.CreatedDate = options.IncludeChangeInfo ? QueryHierarchyItemFactory.GetNullableDate(query.CreatedDate) : new DateTime?();
      queryHierarchyItem.LastModifiedBy = options.IncludeChangeInfo ? QueryHierarchyItemFactory.GetNullableIdentityReference(witRequestContext.RequestContext, identityMap, query.ModifiedById, query.ModifiedByName) : (IdentityReference) null;
      queryHierarchyItem.LastModifiedDate = options.IncludeChangeInfo ? QueryHierarchyItemFactory.GetNullableDate(query.ModifiedDate) : new DateTime?();
      queryHierarchyItem.QueryType = flag1 ? new Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.QueryType?() : nullable1;
      queryHierarchyItem.Columns = itemFieldReferences;
      queryHierarchyItem.SortColumns = itemQuerySortColumns;
      queryHierarchyItem.Wiql = options.IncludeWiql ? query.Wiql : (string) null;
      queryHierarchyItem.Links = options.IncludeLinks ? QueryHierarchyItemFactory.GetReferenceLinks(witRequestContext, query) : (ReferenceLinks) null;
      queryHierarchyItem.IsPublic = new bool?(query.IsPublic);
      queryHierarchyItem.IsDeleted = query.IsDeleted;
      queryHierarchyItem.IsInvalidSyntax = flag1;
      queryHierarchyItem.Clauses = workItemQueryClause2;
      queryHierarchyItem.SourceClauses = workItemQueryClause1;
      queryHierarchyItem.TargetClauses = workItemQueryClause3;
      queryHierarchyItem.LinkClauses = workItemQueryClause4;
      queryHierarchyItem.FilterOptions = nullable2;
      queryHierarchyItem.QueryRecursionOption = nullable3;
      IdentityReference identityReference1;
      if (options.IncludeChangeInfo)
      {
        Guid? lastExecutedById = query.LastExecutedById;
        if (lastExecutedById.HasValue)
        {
          IDictionary<Guid, IdentityReference> dictionary = identityMap;
          lastExecutedById = query.LastExecutedById;
          Guid key = lastExecutedById.Value;
          IdentityReference identityReference2;
          ref IdentityReference local = ref identityReference2;
          if (dictionary.TryGetValue(key, out local))
          {
            identityReference1 = identityReference2;
            goto label_19;
          }
        }
      }
      identityReference1 = (IdentityReference) null;
label_19:
      queryHierarchyItem.LastExecutedBy = identityReference1;
      DateTime? nullable6;
      DateTime? nullable7;
      if (options.IncludeChangeInfo)
      {
        nullable6 = query.LastExecutedDate;
        if (nullable6.HasValue)
        {
          nullable6 = query.LastExecutedDate;
          nullable7 = QueryHierarchyItemFactory.GetNullableDate(nullable6.Value);
          goto label_23;
        }
      }
      nullable6 = new DateTime?();
      nullable7 = nullable6;
label_23:
      queryHierarchyItem.LastExecutedDate = nullable7;
      return queryHierarchyItem;
    }

    private static Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.QueryRecursionOption? GetQueryRecursionOption(
      Microsoft.TeamFoundation.WorkItemTracking.Server.QueryRecursionOption recursionOption)
    {
      if (recursionOption == Microsoft.TeamFoundation.WorkItemTracking.Server.QueryRecursionOption.ParentFirst)
        return new Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.QueryRecursionOption?(Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.QueryRecursionOption.ParentFirst);
      return recursionOption == Microsoft.TeamFoundation.WorkItemTracking.Server.QueryRecursionOption.ChildFirst ? new Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.QueryRecursionOption?(Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.QueryRecursionOption.ChildFirst) : new Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.QueryRecursionOption?();
    }

    private static IDictionary<Guid, IdentityReference> BuildIdentityMap(
      IVssRequestContext requestContext,
      IList<QueryItem> queryItems,
      bool includeUrls)
    {
      List<Guid> guidList = new List<Guid>();
      foreach (QueryItem queryItem in (IEnumerable<QueryItem>) queryItems)
        QueryHierarchyItemFactory.PopulateIdentityGuids(queryItem, (IList<Guid>) guidList);
      return IdentityReferenceBuilder.Create(requestContext, (IEnumerable<Guid>) guidList, includeUrls);
    }

    private static void PopulateIdentityGuids(QueryItem queryItem, IList<Guid> identityGuids)
    {
      if (queryItem.CreatedById != Guid.Empty)
        identityGuids.Add(queryItem.CreatedById);
      if (queryItem.ModifiedById != Guid.Empty)
        identityGuids.Add(queryItem.ModifiedById);
      if (queryItem is QueryFolder)
      {
        QueryFolder queryFolder = (QueryFolder) queryItem;
        if (queryFolder.Children == null)
          return;
        foreach (QueryItem child in (IEnumerable<QueryItem>) queryFolder.Children)
          QueryHierarchyItemFactory.PopulateIdentityGuids(child, identityGuids);
      }
      else
      {
        Query query = (Query) queryItem;
        if (!query.LastExecutedById.HasValue)
          return;
        identityGuids.Add(query.LastExecutedById.Value);
      }
    }

    private static ReferenceLinks GetReferenceLinks(
      WorkItemTrackingRequestContext witRequestContext,
      QueryFolder queryFolder)
    {
      ReferenceLinks referenceLinks = new ReferenceLinks();
      referenceLinks.AddLink("self", WitUrlHelper.GetQueryUrl(witRequestContext, queryFolder.ProjectId, queryFolder.Id));
      referenceLinks.AddLink("html", QueryHierarchyItemFactory.GetQueryWebUrl(witRequestContext, (QueryItem) queryFolder));
      if (queryFolder.ParentId != Guid.Empty)
        referenceLinks.AddLink("parent", WitUrlHelper.GetQueryUrl(witRequestContext, queryFolder.ProjectId, queryFolder.ParentId));
      return referenceLinks;
    }

    private static ReferenceLinks GetReferenceLinks(
      WorkItemTrackingRequestContext witRequestContext,
      Query query)
    {
      ReferenceLinks referenceLinks = new ReferenceLinks();
      referenceLinks.AddLink("self", WitUrlHelper.GetQueryUrl(witRequestContext, query.ProjectId, query.Id));
      referenceLinks.AddLink("html", QueryHierarchyItemFactory.GetQueryWebUrl(witRequestContext, (QueryItem) query));
      if (query.ParentId != Guid.Empty)
        referenceLinks.AddLink("parent", WitUrlHelper.GetQueryUrl(witRequestContext, query.ProjectId, query.ParentId));
      referenceLinks.AddLink("wiql", WitUrlHelper.GetWiqlUrl(witRequestContext, query.ProjectId, query.Id));
      return referenceLinks;
    }

    private static string GetQueryWebUrl(
      WorkItemTrackingRequestContext witRequestContext,
      QueryItem queryItem)
    {
      IVssRequestContext tfsRequestContext = witRequestContext.RequestContext;
      IProjectService projectService = tfsRequestContext.GetService<IProjectService>();
      ITswaServerHyperlinkService service = tfsRequestContext.GetService<ITswaServerHyperlinkService>();
      Uri orAddCacheItem = witRequestContext.GetOrAddCacheItem<Uri>("projectUri_" + queryItem.ProjectId.ToString(), (Func<Uri>) (() => new Uri(projectService.GetProject(tfsRequestContext, queryItem.ProjectId).Uri)));
      IVssRequestContext requestContext = tfsRequestContext;
      Uri projectUri = orAddCacheItem;
      Guid id = queryItem.Id;
      return service.GetWorkItemQueryResultsUrl(requestContext, projectUri, id).ToString();
    }

    private static Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.LinkQueryMode GetLinkTypeMode(
      Microsoft.TeamFoundation.WorkItemTracking.Server.QueryType queryType)
    {
      switch (queryType)
      {
        case Microsoft.TeamFoundation.WorkItemTracking.Server.QueryType.WorkItems:
          return Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.LinkQueryMode.WorkItems;
        case Microsoft.TeamFoundation.WorkItemTracking.Server.QueryType.LinksOneHopMustContain:
          return Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.LinkQueryMode.LinksOneHopMustContain;
        case Microsoft.TeamFoundation.WorkItemTracking.Server.QueryType.LinksOneHopMayContain:
          return Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.LinkQueryMode.LinksOneHopMayContain;
        case Microsoft.TeamFoundation.WorkItemTracking.Server.QueryType.LinksOneHopDoesNotContain:
          return Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.LinkQueryMode.LinksOneHopDoesNotContain;
        case Microsoft.TeamFoundation.WorkItemTracking.Server.QueryType.LinksRecursiveMustContain:
          return Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.LinkQueryMode.LinksRecursiveMustContain;
        case Microsoft.TeamFoundation.WorkItemTracking.Server.QueryType.LinksRecursiveMayContain:
          return Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.LinkQueryMode.LinksRecursiveMayContain;
        case Microsoft.TeamFoundation.WorkItemTracking.Server.QueryType.LinksRecursiveDoesNotContain:
          return Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.LinkQueryMode.LinksRecursiveDoesNotContain;
        default:
          throw new NotSupportedException();
      }
    }

    private static IdentityReference GetNullableIdentityReference(
      IVssRequestContext requestContext,
      IDictionary<Guid, IdentityReference> identityMap,
      Guid id,
      string displayName)
    {
      IdentityReference identityReference;
      if (id == Guid.Empty || !identityMap.TryGetValue(id, out identityReference))
        return (IdentityReference) null;
      if (IdentityReferenceBuilder.ShouldUseProperIdentityRef(requestContext))
      {
        if (identityReference != null && !string.IsNullOrEmpty(displayName))
          identityReference.Name = displayName;
        return identityReference;
      }
      ConstantIdentityRef constantIdentityRef = new ConstantIdentityRef((ISecuredObject) null);
      constantIdentityRef.Id = id.ToString();
      return new IdentityReference((IdentityRef) constantIdentityRef, displayName);
    }

    private static DateTime? GetNullableDate(DateTime date) => !(date == DateTime.MinValue) ? new DateTime?(date) : new DateTime?();
  }
}
