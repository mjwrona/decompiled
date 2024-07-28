// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.ProjectHelper
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public static class ProjectHelper
  {
    public static ProjectReference ConvertDataspaceToProjectReference(
      int? dataspaceId,
      TeamFoundationSqlResourceComponent sqlComponent)
    {
      Guid? nullable1 = dataspaceId.HasValue ? new Guid?(sqlComponent.GetDataspaceIdentifier(dataspaceId.Value)) : new Guid?();
      if (nullable1.HasValue)
      {
        Guid? nullable2 = nullable1;
        Guid empty = Guid.Empty;
        if ((nullable2.HasValue ? (nullable2.HasValue ? (nullable2.GetValueOrDefault() != empty ? 1 : 0) : 0) : 1) != 0)
          return new ProjectReference()
          {
            Id = nullable1.Value
          };
      }
      return (ProjectReference) null;
    }

    public static void HydrateProjectReference(Microsoft.VisualStudio.Services.Feed.WebApi.Feed targetFeed, ProjectReference projectData)
    {
      if (targetFeed?.Project == (ProjectReference) null)
        return;
      Guid id1 = targetFeed.Project.Id;
      Guid? nullable1 = projectData?.Id;
      if ((nullable1.HasValue ? (id1 != nullable1.GetValueOrDefault() ? 1 : 0) : 1) != 0)
      {
        // ISSUE: variable of a boxed type
        __Boxed<Guid> id2 = (ValueType) targetFeed.Project.Id;
        Guid? nullable2;
        if ((object) projectData == null)
        {
          nullable1 = new Guid?();
          nullable2 = nullable1;
        }
        else
          nullable2 = new Guid?(projectData.Id);
        // ISSUE: variable of a boxed type
        __Boxed<Guid?> local = (ValueType) nullable2;
        throw new InvalidOperationException(Resources.Error_MismatchedFeedProjectId((object) id2, (object) local));
      }
      targetFeed.Project = projectData;
    }

    public static void HydrateProjectReferences(
      IVssRequestContext requestContext,
      IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> feeds,
      out IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> filteredOutFeeds)
    {
      ProjectHelper.HydrateProjectReferences<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>(requestContext, feeds, (Func<Microsoft.VisualStudio.Services.Feed.WebApi.Feed, Microsoft.VisualStudio.Services.Feed.WebApi.Feed>) (feed => feed), out filteredOutFeeds);
    }

    public static void HydrateProjectReferences(
      IVssRequestContext requestContext,
      IEnumerable<FeedChange> feedChanges,
      out IEnumerable<FeedChange> filteredOutFeeds)
    {
      ProjectHelper.HydrateProjectReferences<FeedChange>(requestContext, feedChanges, (Func<FeedChange, Microsoft.VisualStudio.Services.Feed.WebApi.Feed>) (feedChange => feedChange.Feed), out filteredOutFeeds);
    }

    private static void HydrateProjectReferences<T>(
      IVssRequestContext requestContext,
      IEnumerable<T> objects,
      Func<T, Microsoft.VisualStudio.Services.Feed.WebApi.Feed> feedSelector,
      out IEnumerable<T> filteredOutFeeds)
    {
      List<T> objList = new List<T>();
      Dictionary<Guid, ProjectInfo> dictionary;
      try
      {
        dictionary = requestContext.GetService<IProjectService>().GetProjects(requestContext, ProjectState.WellFormed).Where<ProjectInfo>((Func<ProjectInfo, bool>) (x => !x.IsSoftDeleted)).ToDictionary<ProjectInfo, Guid>((Func<ProjectInfo, Guid>) (x => x.Id));
      }
      catch (VssServiceException ex) when (FeedException.IsHostShutdownResponse(ex))
      {
        throw new HostShutdownException(ex.Message);
      }
      foreach (T obj in objects)
      {
        Microsoft.VisualStudio.Services.Feed.WebApi.Feed targetFeed = feedSelector(obj);
        ProjectReference project1 = targetFeed.Project;
        if ((object) project1 != null)
        {
          ProjectInfo project2;
          if (dictionary.TryGetValue(project1.Id, out project2))
            ProjectHelper.HydrateProjectReference(targetFeed, project2.ToProjectReference());
          else
            objList.Add(obj);
        }
      }
      filteredOutFeeds = (IEnumerable<T>) objList;
    }
  }
}
