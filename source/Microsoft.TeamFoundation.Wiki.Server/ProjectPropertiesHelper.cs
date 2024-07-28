// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.ProjectPropertiesHelper
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.Wiki.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Wiki.Server
{
  public static class ProjectPropertiesHelper
  {
    public static void AddOrUpdateWikiToProperties(
      IVssRequestContext requestContext,
      Guid teamProjectId,
      WikiV2 wiki,
      bool notify = true)
    {
      if (wiki == null)
        return;
      string name = string.Format("System.Wiki.{0}", (object) wiki.Id.ToString());
      string str = JsonConvert.SerializeObject((object) wiki);
      requestContext.GetService<IProjectService>().SetProjectProperties(requestContext.Elevate(), teamProjectId, (IEnumerable<ProjectProperty>) new List<ProjectProperty>()
      {
        new ProjectProperty(name, (object) str)
      });
      if (notify)
        ProjectPropertiesHelper.PublishNotification(requestContext, wiki.ProjectId, wiki.RepositoryId);
      requestContext.GetService<LocalSecurityInvalidationService>()?.InvalidateSystemStore(requestContext, "WikiCreatedOrUpdated");
    }

    public static void DeleteWikiProperties(
      IVssRequestContext requestContext,
      Guid teamProjectId,
      Guid wikiId,
      Guid repositoryId)
    {
      string name = string.Format("System.Wiki.{0}", (object) wikiId.ToString());
      requestContext.GetService<IProjectService>().SetProjectProperties(requestContext.Elevate(), teamProjectId, (IEnumerable<ProjectProperty>) new List<ProjectProperty>()
      {
        new ProjectProperty(name, (object) null)
      });
      ProjectPropertiesHelper.PublishNotification(requestContext, teamProjectId, repositoryId);
      requestContext.GetService<LocalSecurityInvalidationService>()?.InvalidateSystemStore(requestContext, "WikiDeleted");
    }

    public static IEnumerable<WikiV2> GetAllWikisFromProperties(
      IVssRequestContext requestContext,
      Guid teamProjectId)
    {
      IProjectService service = requestContext.GetService<IProjectService>();
      if (teamProjectId.Equals(Guid.Empty))
      {
        foreach (ProjectInfo project in service.GetProjects(requestContext, ProjectState.WellFormed))
        {
          foreach (WikiV2 wikisFromProperty in ProjectPropertiesHelper.GetAllWikisInAProject(requestContext, project.Id))
            yield return wikisFromProperty;
        }
      }
      else
      {
        foreach (WikiV2 wikisFromProperty in ProjectPropertiesHelper.GetAllWikisInAProject(requestContext, teamProjectId))
          yield return wikisFromProperty;
      }
    }

    public static IEnumerable<WikiV2> GetAllWikisInAProject(
      IVssRequestContext requestContext,
      Guid teamProjectId,
      bool cleanupMissingProperties = false)
    {
      IProjectService service = requestContext.GetService<IProjectService>();
      IVssRequestContext requestContext1 = requestContext;
      Guid projectId = teamProjectId;
      string[] projectPropertyFilters = new string[1]
      {
        "System.Wiki*"
      };
      foreach (ProjectProperty projectProperty in service.GetProjectProperties(requestContext1, projectId, (IEnumerable<string>) projectPropertyFilters))
      {
        string g = projectProperty.Name == "System.WikiRepoId" ? (string) projectProperty.Value : (string) null;
        if (!string.IsNullOrEmpty(g))
        {
          WikiV2 defaultProjectWiki = WikiV2Helper.GetDefaultProjectWiki(requestContext, teamProjectId, new Guid(g));
          if (defaultProjectWiki != null)
          {
            defaultProjectWiki.PopulateUrls(requestContext);
            yield return defaultProjectWiki;
          }
        }
        else
        {
          string str = projectProperty.Name.StartsWith("System.Wiki.") ? (string) projectProperty.Value : (string) null;
          if (!string.IsNullOrEmpty(str))
          {
            WikiV2 wiki = JsonConvert.DeserializeObject<WikiV2>(str);
            wiki.MappedPath = PathHelper.TrimTrailingSeparatorInPath(wiki.MappedPath);
            using (ITfsGitRepository wikiRepository = ProjectPropertiesHelper.GetWikiRepository(requestContext, wiki))
            {
              if (wikiRepository != null)
              {
                wiki.IsDisabled = wikiRepository.IsDisabled;
                wiki.PopulateUrls(requestContext);
                yield return wiki;
              }
              else if (cleanupMissingProperties && requestContext.IsFeatureEnabled("WebAccess.Wiki.WikiPropertiesCleanup"))
              {
                ProjectPropertiesHelper.DeleteWikiProperties(requestContext, teamProjectId, wiki.Id, wiki.RepositoryId);
                using (WikiComponent component = requestContext.CreateComponent<WikiComponent>())
                {
                  foreach (GitVersionDescriptor version in wiki.Versions)
                  {
                    List<WikiPageWithId> deletedPagesInDb;
                    component.UnpublishWikiVersion(wiki.ProjectId, wiki.Id, string.Format("refs/heads/" + version.Version), out deletedPagesInDb);
                    new WikiJobHandler().QueueWikiMetaDataDeletionJob(requestContext, wiki.ProjectId, wiki.Id, version, deletedPagesInDb);
                  }
                }
                requestContext.TraceAlways(15250306, TraceLevel.Warning, "Wiki", "Service", "Wiki properties were cleared for repository that was deleted. Wiki properties: {0}", (object) str);
              }
            }
          }
        }
      }
    }

    private static ITfsGitRepository GetWikiRepository(
      IVssRequestContext requestContext,
      WikiV2 wiki)
    {
      try
      {
        return WikiV2Helper.GetWikiRepository(requestContext.Elevate(), wiki.RepositoryId);
      }
      catch (GitRepositoryNotFoundException ex)
      {
        if (wiki.Type == WikiType.CodeWiki)
          requestContext.TraceException(15250304, "Wiki", "Service", (Exception) ex);
        else
          requestContext.TraceException(15250305, "Wiki", "Service", (Exception) ex);
        return (ITfsGitRepository) null;
      }
    }

    public static void PublishNotification(
      IVssRequestContext requestContext,
      Guid teamProjectId,
      Guid repositoryId)
    {
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
        ProjectPropertiesHelper.PublishHostedNotification(requestContext, teamProjectId, repositoryId);
      else
        ProjectPropertiesHelper.PublishOnPremNotification(requestContext, teamProjectId, repositoryId);
    }

    private static void PublishOnPremNotification(
      IVssRequestContext requestContext,
      Guid teamProjectId,
      Guid repositoryId)
    {
      if (requestContext.ServiceHost.HostType != TeamFoundationHostType.ProjectCollection)
        return;
      requestContext.GetService<ITeamFoundationEventService>().PublishNotification(requestContext, (object) new WikiUpdatedNotificationMessage(requestContext.ServiceHost.InstanceId, teamProjectId, repositoryId));
    }

    private static void PublishHostedNotification(
      IVssRequestContext requestContext,
      Guid teamProjectId,
      Guid repositoryId)
    {
      ServiceEvent serviceEvent = new ServiceEvent()
      {
        Publisher = new Publisher()
        {
          Name = "Tfs",
          ServiceOwnerId = ServiceInstanceTypes.TFS
        },
        EventType = "Wiki.Updated"
      };
      serviceEvent.Resource = (object) new WikiUpdatedNotificationMessage(requestContext.ServiceHost.InstanceId, teamProjectId, repositoryId);
      requestContext.GetService<IMessageBusPublisherService>().Publish(requestContext, "Microsoft.TeamFoundation.Wiki.Server", new object[1]
      {
        (object) serviceEvent
      }, false);
    }
  }
}
