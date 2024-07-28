// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.QueryChangeNotificationPayload
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class QueryChangeNotificationPayload
  {
    private const string s_payloadName = "query-change-notification";
    public List<QueryChangeNotificationPayload.ProjectScopeQueryList> DeletedQueryItems;
    public List<QueryChangeNotificationPayload.ProjectScopeQueryList> PersonalizedQueryItems;

    public string Serialize() => TeamFoundationSerializationUtility.SerializeToString<QueryChangeNotificationPayload>(this, new XmlRootAttribute("query-change-notification"));

    public static QueryChangeNotificationPayload Deserialize(
      IVssRequestContext requestContext,
      string changeSummary)
    {
      QueryChangeNotificationPayload notificationPayload = TeamFoundationSerializationUtility.Deserialize<QueryChangeNotificationPayload>(changeSummary, new XmlRootAttribute("query-change-notification"));
      if (notificationPayload == null || notificationPayload.DeletedQueryItems.Count == 0 && notificationPayload.PersonalizedQueryItems.Count == 0)
      {
        QueryChangeNotificationPayload.LegacyQueryChangeNotificationPayload legacyNotificationPayload = TeamFoundationSerializationUtility.Deserialize<QueryChangeNotificationPayload.LegacyQueryChangeNotificationPayload>(changeSummary, new XmlRootAttribute("query-change-notification"));
        if (legacyNotificationPayload != null && (legacyNotificationPayload.DeletedQueryItems.Count > 0 || legacyNotificationPayload.PersonalizedQueryItems.Count > 0 || legacyNotificationPayload.UpdatedQueryItems.Count > 0))
          notificationPayload = QueryChangeNotificationPayload.TransformLegacyNotificationPayload(requestContext, legacyNotificationPayload);
      }
      return notificationPayload;
    }

    internal static QueryChangeNotificationPayload Create(
      Dictionary<Guid, ServerQueryItem> activeQueryItems,
      Dictionary<Guid, ServerQueryItem> deletedQueryItems,
      Func<int, Guid> projectNodeIdToGuid)
    {
      QueryChangeNotificationPayload notificationPayload = new QueryChangeNotificationPayload()
      {
        DeletedQueryItems = new List<QueryChangeNotificationPayload.ProjectScopeQueryList>(),
        PersonalizedQueryItems = new List<QueryChangeNotificationPayload.ProjectScopeQueryList>()
      };
      if (activeQueryItems != null && activeQueryItems.Any<KeyValuePair<Guid, ServerQueryItem>>())
      {
        IEnumerable<KeyValuePair<Guid, ServerQueryItem>> modifiedQueryItems = activeQueryItems.Where<KeyValuePair<Guid, ServerQueryItem>>((Func<KeyValuePair<Guid, ServerQueryItem>, bool>) (i => i.Value.New.IsPublic.HasValue && !i.Value.New.IsPublic.Value && i.Value.Existing.IsPublic.HasValue && i.Value.Existing.IsPublic.Value));
        notificationPayload.PersonalizedQueryItems = QueryChangeNotificationPayload.GenerateProjectScopeQueryLists(modifiedQueryItems, projectNodeIdToGuid);
      }
      notificationPayload.DeletedQueryItems = QueryChangeNotificationPayload.GenerateProjectScopeQueryLists((IEnumerable<KeyValuePair<Guid, ServerQueryItem>>) deletedQueryItems, projectNodeIdToGuid);
      return notificationPayload;
    }

    private static List<QueryChangeNotificationPayload.ProjectScopeQueryList> GenerateProjectScopeQueryLists(
      IEnumerable<KeyValuePair<Guid, ServerQueryItem>> modifiedQueryItems,
      Func<int, Guid> projectNodeIdToGuid)
    {
      List<QueryChangeNotificationPayload.ProjectScopeQueryList> source = new List<QueryChangeNotificationPayload.ProjectScopeQueryList>();
      if (modifiedQueryItems != null && modifiedQueryItems.Any<KeyValuePair<Guid, ServerQueryItem>>())
      {
        foreach (KeyValuePair<Guid, ServerQueryItem> modifiedQueryItem in modifiedQueryItems)
        {
          Guid projectId = projectNodeIdToGuid(modifiedQueryItem.Value.Existing.ProjectId);
          QueryChangeNotificationPayload.ProjectScopeQueryList projectScopeQueryList = source.Where<QueryChangeNotificationPayload.ProjectScopeQueryList>((Func<QueryChangeNotificationPayload.ProjectScopeQueryList, bool>) (i => i.ProjectId == projectId)).FirstOrDefault<QueryChangeNotificationPayload.ProjectScopeQueryList>();
          if (projectScopeQueryList == null)
          {
            projectScopeQueryList = new QueryChangeNotificationPayload.ProjectScopeQueryList()
            {
              ProjectId = projectId,
              QueryIds = new List<Guid>()
            };
            source.Add(projectScopeQueryList);
          }
          projectScopeQueryList.QueryIds.Add(modifiedQueryItem.Key);
        }
      }
      return source;
    }

    private static QueryChangeNotificationPayload TransformLegacyNotificationPayload(
      IVssRequestContext requestContext,
      QueryChangeNotificationPayload.LegacyQueryChangeNotificationPayload legacyNotificationPayload)
    {
      ITeamFoundationQueryItemService queryItemService = requestContext.GetService<ITeamFoundationQueryItemService>();
      IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem> queryItems1 = legacyNotificationPayload.DeletedQueryItems.Select<Guid, Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem>((Func<Guid, Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem>) (i => queryItemService.GetQueryById(requestContext, i, new int?(), true, true)));
      IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem> queryItems2 = legacyNotificationPayload.PersonalizedQueryItems.Select<Guid, Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem>((Func<Guid, Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem>) (i => queryItemService.GetQueryById(requestContext, i, new int?(), true)));
      Func<IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem>, List<QueryChangeNotificationPayload.ProjectScopeQueryList>> func = (Func<IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem>, List<QueryChangeNotificationPayload.ProjectScopeQueryList>>) (queryItems =>
      {
        List<QueryChangeNotificationPayload.ProjectScopeQueryList> source = new List<QueryChangeNotificationPayload.ProjectScopeQueryList>();
        if (queryItems != null)
        {
          foreach (Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem queryItem1 in queryItems)
          {
            Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem queryItem = queryItem1;
            QueryChangeNotificationPayload.ProjectScopeQueryList projectScopeQueryList = source.Where<QueryChangeNotificationPayload.ProjectScopeQueryList>((Func<QueryChangeNotificationPayload.ProjectScopeQueryList, bool>) (i => i.ProjectId == queryItem.ProjectId)).FirstOrDefault<QueryChangeNotificationPayload.ProjectScopeQueryList>();
            if (projectScopeQueryList == null)
            {
              projectScopeQueryList = new QueryChangeNotificationPayload.ProjectScopeQueryList()
              {
                ProjectId = queryItem.ProjectId,
                QueryIds = new List<Guid>()
              };
              source.Add(projectScopeQueryList);
            }
            projectScopeQueryList.QueryIds.Add(queryItem.Id);
          }
        }
        return source;
      });
      return new QueryChangeNotificationPayload()
      {
        DeletedQueryItems = func(queryItems1),
        PersonalizedQueryItems = func(queryItems2)
      };
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Serializable]
    public class ProjectScopeQueryList
    {
      public Guid ProjectId { get; set; }

      public List<Guid> QueryIds { get; set; }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Serializable]
    public class LegacyQueryChangeNotificationPayload
    {
      public List<Guid> DeletedQueryItems { get; set; }

      public List<Guid> PersonalizedQueryItems { get; set; }

      public List<Guid> UpdatedQueryItems { get; set; }
    }
  }
}
