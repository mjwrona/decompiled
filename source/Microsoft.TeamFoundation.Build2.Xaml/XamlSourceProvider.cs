// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Xaml.XamlSourceProvider
// Assembly: Microsoft.TeamFoundation.Build2.Xaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 48A241AC-D20F-49E0-A581-C219E1ED7760
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Xaml.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.Server;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Xaml
{
  public class XamlSourceProvider : IXamlSourceProvider, IVssFrameworkService
  {
    public BuildRepository ConvertXamlSourceProviderToBuildRepository(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Build.Server.BuildDefinition definition,
      Microsoft.TeamFoundation.Build.Server.BuildDefinitionSourceProvider sourceProvider,
      IDictionary<string, string> repositoryMap,
      bool deep = false)
    {
      BuildRepository buildRepository = (BuildRepository) null;
      Dictionary<string, string> dictionary = sourceProvider.Fields.ToDictionary<NameValueField, string, string>((Func<NameValueField, string>) (field => field.Name), (Func<NameValueField, string>) (field => field.Value));
      if (BuildSourceProviders.IsTfGit(sourceProvider.Name))
      {
        string property1 = BuildSourceProviders.GetProperty((IDictionary<string, string>) dictionary, BuildSourceProviders.GitProperties.RepositoryName);
        if (!string.IsNullOrEmpty(property1))
        {
          string str1 = ((IEnumerable<string>) property1.Split('/')).Last<string>();
          string str2;
          if (!repositoryMap.TryGetValue(str1, out str2))
          {
            ITeamFoundationGitRepositoryService service = requestContext.GetService<ITeamFoundationGitRepositoryService>();
            try
            {
              ITfsGitRepository repositoryByName = service.FindRepositoryByName(requestContext, definition.TeamProject.Id.ToString("D"), str1);
              if (repositoryByName != null)
              {
                using (repositoryByName)
                  str2 = repositoryByName.Key.RepoId.ToString("D");
                repositoryMap.Add(str1, str2);
              }
            }
            catch (GitRepositoryNotFoundException ex)
            {
              requestContext.TraceConditionally(12030075, TraceLevel.Info, "Build2", "RestController", (Func<string>) (() => ex.ToString()));
              repositoryMap.Add(str1, string.Empty);
            }
          }
          buildRepository = new BuildRepository()
          {
            Id = str2,
            Type = "TfsGit",
            Name = str1,
            DefaultBranch = BuildSourceProviders.GetProperty((IDictionary<string, string>) dictionary, BuildSourceProviders.GitProperties.DefaultBranch)
          };
          string property2 = BuildSourceProviders.GetProperty((IDictionary<string, string>) dictionary, BuildSourceProviders.GitProperties.RepositoryUrl);
          if (!string.IsNullOrEmpty(property2))
            buildRepository.Url = new Uri(property2, UriKind.Absolute);
        }
      }
      else if (BuildSourceProviders.IsGit(sourceProvider.Name))
      {
        buildRepository = new BuildRepository()
        {
          Type = "Git",
          Name = BuildSourceProviders.GetProperty((IDictionary<string, string>) dictionary, BuildSourceProviders.GitProperties.RepositoryName),
          DefaultBranch = BuildSourceProviders.GetProperty((IDictionary<string, string>) dictionary, BuildSourceProviders.GitProperties.DefaultBranch)
        };
        string property = BuildSourceProviders.GetProperty((IDictionary<string, string>) dictionary, BuildSourceProviders.GitProperties.RepositoryUrl);
        if (!string.IsNullOrEmpty(property))
          buildRepository.Url = new Uri(property, UriKind.Absolute);
      }
      else if (BuildSourceProviders.IsTfVersionControl(sourceProvider.Name))
      {
        buildRepository = new BuildRepository()
        {
          Type = "TfsVersionControl"
        };
        if (deep && definition.WorkspaceTemplate != null)
        {
          BuildWorkspace buildWorkspace = this.ConvertXamlWorkspaceTemplateToBuildWorkspace(definition.WorkspaceTemplate);
          buildRepository.Properties["tfvcMapping"] = JsonUtility.ToString((object) buildWorkspace);
        }
      }
      return buildRepository;
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    private BuildWorkspace ConvertXamlWorkspaceTemplateToBuildWorkspace(
      Microsoft.TeamFoundation.Build.Server.WorkspaceTemplate workspaceTemplate)
    {
      BuildWorkspace buildWorkspace = new BuildWorkspace();
      buildWorkspace.Mappings.AddRange(workspaceTemplate.Mappings.Select<Microsoft.TeamFoundation.Build.Server.WorkspaceMapping, MappingDetails>((Func<Microsoft.TeamFoundation.Build.Server.WorkspaceMapping, MappingDetails>) (mapping => new MappingDetails()
      {
        ServerPath = mapping.ServerItem,
        MappingType = mapping.MappingType.ToString().ToLowerInvariant()
      })));
      return buildWorkspace;
    }
  }
}
