// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Common.FeedModelExtensions
// Assembly: Microsoft.VisualStudio.Services.Feed.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AAC6BCA4-7F6C-4DFE-8058-1CCDD886477F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Feed.Common
{
  public static class FeedModelExtensions
  {
    public static ProjectReference ToProjectReference(this ProjectInfo project)
    {
      if (project == null)
        return (ProjectReference) null;
      return new ProjectReference()
      {
        Id = project.Id,
        Name = project.Name,
        Visibility = project.Visibility.ToString()
      };
    }

    public static ProjectReference ToProjectReference(this Guid? projectId)
    {
      if (projectId.HasValue)
      {
        Guid? nullable = projectId;
        Guid empty = Guid.Empty;
        if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() == empty ? 1 : 0) : 1) : 0) == 0)
          return new ProjectReference()
          {
            Id = projectId.Value
          };
      }
      return (ProjectReference) null;
    }

    public static ProjectReference ToProjectReference(this Guid projectId)
    {
      if (projectId == Guid.Empty)
        return (ProjectReference) null;
      return new ProjectReference() { Id = projectId };
    }

    public static Guid ToProjectIdOrEmptyGuid(this ProjectReference project) => (object) project == null ? Guid.Empty : project.Id;

    public static Guid? ToNullableProjectId(this ProjectReference project)
    {
      Guid? id = project?.Id;
      return !id.HasValue || !(id.Value != Guid.Empty) ? new Guid?() : id;
    }

    public static string ToProjectIdentifierString(this ProjectReference project) => project?.Id.ToString();

    public static string GetProjectIdentifierString(this FeedCore feed)
    {
      ProjectReference project = feed.Project;
      return (object) project == null ? (string) null : project.ToProjectIdentifierString();
    }

    public static bool IsPublicFeed(this FeedCore feed)
    {
      if (feed.Project == (ProjectReference) null)
        return false;
      if (feed.Project != (ProjectReference) null && string.IsNullOrEmpty(feed.Project.Visibility))
        throw new ArgumentException(string.Format("feed {0} needs to be hydrated with project details before checking visibility.", (object) feed.Id));
      ProjectVisibility result;
      if (!Enum.TryParse<ProjectVisibility>(feed.Project?.Visibility, out result))
        throw new ArgumentException("Invalid ProjectVisibility value '" + feed.Project?.Visibility + "'", "Visibility");
      return result == ProjectVisibility.Public;
    }
  }
}
