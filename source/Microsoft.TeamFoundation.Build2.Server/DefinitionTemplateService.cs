// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.DefinitionTemplateService
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class DefinitionTemplateService : IDefinitionTemplateService, IVssFrameworkService
  {
    private readonly IBuildSecurityProvider Security;
    private readonly IBuildTemplateContributionProvider ContributionProvider;
    private const string c_TemplateResourcesSuffix = "Resources";
    private const string c_TemplateResourceNamePrefix = "Microsoft.TeamFoundation.Build2.Server.Templates.BuildDefinitions.";
    private const string c_NewCIWorkflow_TemplateResourceNamePrefix = "Microsoft.TeamFoundation.Build2.Server.Templates.NewCIWorkflow";
    private const string c_TemplateResourceNameFormat = "Microsoft.TeamFoundation.Build2.Server.Templates.BuildDefinitions.{0}.json";
    private const string c_NewCIWorkflow_TemplateResourceNameFormat = "Microsoft.TeamFoundation.Build2.Server.Templates.NewCIWorkflow.{0}.json";

    internal DefinitionTemplateService()
      : this((IBuildSecurityProvider) new BuildSecurityProvider(), (IBuildTemplateContributionProvider) new BuildTemplateContributionProvider())
    {
    }

    internal DefinitionTemplateService(
      IBuildSecurityProvider security,
      IBuildTemplateContributionProvider contributionProvider)
    {
      this.Security = security;
      this.ContributionProvider = contributionProvider;
    }

    public IList<BuildDefinitionTemplate> GetTemplates(
      IVssRequestContext requestContext,
      Guid projectId,
      bool withProcessParameters = false)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      using (requestContext.TraceScope(nameof (DefinitionTemplateService), nameof (GetTemplates)))
        return (IList<BuildDefinitionTemplate>) this.GetTemplatesInternal(requestContext, projectId, withProcessParameters: withProcessParameters);
    }

    public IList<BuildDefinitionTemplate> GetCustomTemplates(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      using (requestContext.TraceScope(nameof (DefinitionTemplateService), nameof (GetCustomTemplates)))
      {
        HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.Ordinal);
        foreach (BuildDefinitionTemplate definitionTemplate in this.GetBuiltInTemplates(requestContext).Concat<BuildDefinitionTemplate>((IEnumerable<BuildDefinitionTemplate>) this.GetBuiltInTemplates(requestContext, true)))
        {
          if (!stringSet.Contains(definitionTemplate.Id))
            stringSet.Add(definitionTemplate.Id);
        }
        List<BuildDefinitionTemplate> templates = new List<BuildDefinitionTemplate>();
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
        {
          foreach (BuildDefinitionTemplate definitionTemplate in component.GetDefinitionTemplates(projectId, (string) null).Where<BuildDefinitionTemplate>((Func<BuildDefinitionTemplate, bool>) (dt => dt.Template != null)))
          {
            if (!stringSet.Contains(definitionTemplate.Id))
              templates.Add(definitionTemplate);
          }
        }
        this.DeserializeTemplates(requestContext, (IList<BuildDefinitionTemplate>) templates);
        return (IList<BuildDefinitionTemplate>) templates;
      }
    }

    public BuildDefinitionTemplate GetTemplate(
      IVssRequestContext requestContext,
      Guid projectId,
      string templateId,
      bool withProcessParameters = false)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckStringForNullOrEmpty(templateId, nameof (templateId));
      using (requestContext.TraceScope(nameof (DefinitionTemplateService), nameof (GetTemplate)))
        return this.GetTemplatesInternal(requestContext, projectId, templateId, withProcessParameters).FirstOrDefault<BuildDefinitionTemplate>();
    }

    public BuildDefinitionTemplate SaveTemplate(
      IVssRequestContext requestContext,
      Guid projectId,
      BuildDefinitionTemplate template)
    {
      if (requestContext.GetService<IContributedFeatureService>().IsFeatureEnabled(requestContext, "ms.vss-build-web.disable-classic-build-pipeline-creation"))
        throw new InvalidOperationException(BuildServerResources.ClassicPipelinesDisabled());
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNull<BuildDefinitionTemplate>(template, nameof (template));
      ArgumentUtility.CheckBoundsInclusive(template.Name.Length, 1, 260, "template.Name");
      using (requestContext.TraceScope(nameof (DefinitionTemplateService), nameof (SaveTemplate)))
      {
        if (this.GetBuiltInTemplate(requestContext, template.Id) != null)
          throw new DefinitionTemplateExistsException(BuildServerResources.DefinitionTemplateCannotBeModified((object) template.Id));
        this.Security.CheckProjectPermission(requestContext, projectId, Microsoft.TeamFoundation.Build.Common.BuildPermissions.EditBuildDefinition);
        template.Template.AllSteps().FixLatestMajorVersions(requestContext);
        if (template.Template.Repository != null)
          requestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(requestContext, template.Template.Repository.Type).BeforeSerialize(requestContext, template.Template.Repository);
        template.Template.Options.RemoveAll((Predicate<BuildOption>) (o => o == null || o.Definition == null));
        if (template.Template.Options.Count > 0)
        {
          using (IDisposableReadOnlyList<IBuildOption> extensions = requestContext.GetExtensions<IBuildOption>())
          {
            List<IBuildOption> list = extensions.OrderBy<IBuildOption, int>((Func<IBuildOption, int>) (option => option.GetOrdinal())).ToList<IBuildOption>();
            template.Template.ModernizeOptions(requestContext, (IList<IBuildOption>) list);
          }
        }
        BuildDefinitionTemplate definitionTemplate;
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
          definitionTemplate = component.SaveDefinitionTemplate(projectId, template);
        if (definitionTemplate.Template.Repository != null)
          requestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(requestContext, definitionTemplate.Template.Repository.Type).AfterDeserialize(requestContext, definitionTemplate.Template.Repository);
        return definitionTemplate;
      }
    }

    public void DeleteTemplate(
      IVssRequestContext requestContext,
      Guid projectId,
      string templateId)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckStringForNullOrEmpty(templateId, nameof (templateId));
      this.DeleteTemplates(requestContext, projectId, new List<string>()
      {
        templateId
      });
    }

    public void DeleteTemplates(
      IVssRequestContext requestContext,
      Guid projectId,
      List<string> templateIds)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNull<List<string>>(templateIds, nameof (templateIds));
      using (requestContext.TraceScope(nameof (DefinitionTemplateService), nameof (DeleteTemplates)))
      {
        this.Security.CheckProjectPermission(requestContext, projectId, Microsoft.TeamFoundation.Build.Common.BuildPermissions.DeleteBuildDefinition);
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
          component.DeleteDefinitionTemplates(projectId, templateIds);
      }
    }

    private List<BuildDefinitionTemplate> GetTemplatesInternal(
      IVssRequestContext requestContext,
      Guid projectId,
      string templateId = null,
      bool withProcessParameters = false)
    {
      List<BuildDefinitionTemplate> templates = new List<BuildDefinitionTemplate>();
      if (string.IsNullOrEmpty(templateId))
      {
        HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.Ordinal);
        foreach (BuildDefinitionTemplate builtInTemplate in this.GetBuiltInTemplates(requestContext, withProcessParameters))
        {
          if (!stringSet.Contains(builtInTemplate.Id))
          {
            stringSet.Add(builtInTemplate.Id);
            templates.Add(builtInTemplate);
          }
          else
            requestContext.TraceError(nameof (DefinitionTemplateService), "Duplicated built-in template with Id {0} is found. This shouldn't happen.", (object) builtInTemplate.Id);
        }
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
        {
          foreach (BuildDefinitionTemplate definitionTemplate in component.GetDefinitionTemplates(projectId, templateId).Where<BuildDefinitionTemplate>((Func<BuildDefinitionTemplate, bool>) (dt => dt.Template != null)))
          {
            if (!stringSet.Contains(definitionTemplate.Id))
            {
              templates.Add(definitionTemplate);
              stringSet.Add(definitionTemplate.Id);
            }
            else
              requestContext.TraceWarning(12030088, nameof (DefinitionTemplateService), "Duplicated custom template with Id {0} is found. This shouldn't happen.", (object) definitionTemplate.Id);
          }
        }
        foreach (BuildDefinitionTemplate template in this.ContributionProvider.GetTemplates(requestContext, templateId))
        {
          if (!stringSet.Contains(template.Id))
          {
            templates.Add(template);
            stringSet.Add(template.Id);
          }
          else
            requestContext.TraceWarning(12030088, nameof (DefinitionTemplateService), "Duplicated custom template with Id {0} is found. This shouldn't happen.", (object) template.Id);
        }
      }
      else
      {
        BuildDefinitionTemplate definitionTemplate1 = this.GetBuiltInTemplate(requestContext, templateId, withProcessParameters);
        if (definitionTemplate1 != null)
        {
          templates.Add(definitionTemplate1);
        }
        else
        {
          using (Build2Component component = requestContext.CreateComponent<Build2Component>())
          {
            definitionTemplate1 = component.GetDefinitionTemplates(projectId, templateId).Where<BuildDefinitionTemplate>((Func<BuildDefinitionTemplate, bool>) (dt => dt.Template != null)).FirstOrDefault<BuildDefinitionTemplate>();
            if (definitionTemplate1 != null)
              templates.Add(definitionTemplate1);
          }
          if (definitionTemplate1 == null)
          {
            BuildDefinitionTemplate definitionTemplate2 = this.ContributionProvider.GetTemplates(requestContext, templateId).Where<BuildDefinitionTemplate>((Func<BuildDefinitionTemplate, bool>) (dt => dt.Template != null)).FirstOrDefault<BuildDefinitionTemplate>();
            if (definitionTemplate2 != null)
              templates.Add(definitionTemplate2);
          }
        }
      }
      this.DeserializeTemplates(requestContext, (IList<BuildDefinitionTemplate>) templates);
      return templates;
    }

    private List<BuildDefinitionTemplate> GetBuiltInTemplates(
      IVssRequestContext requestContext,
      bool withProcessParameters = false)
    {
      List<BuildDefinitionTemplate> builtInTemplates = new List<BuildDefinitionTemplate>();
      Assembly executingAssembly = Assembly.GetExecutingAssembly();
      string[] manifestResourceNames = executingAssembly.GetManifestResourceNames();
      string resourceNamePrefix = this.GetTemplateResourceNamePrefix(withProcessParameters);
      bool flag = requestContext.IsFeatureEnabled("Build2.DefaultBuildAuthorizationScope");
      foreach (string name in manifestResourceNames)
      {
        if (name.StartsWith(resourceNamePrefix, StringComparison.OrdinalIgnoreCase))
        {
          using (Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(name))
          {
            try
            {
              BuildDefinitionTemplate templateFromStream = ServerBuildDefinitionHelpers.GetTemplateFromStream(manifestResourceStream);
              if (flag)
                templateFromStream.Template.JobAuthorizationScope = BuildAuthorizationScope.Project;
              builtInTemplates.Add(templateFromStream);
            }
            catch (Exception ex)
            {
              requestContext.TraceException(nameof (DefinitionTemplateService), ex);
            }
          }
        }
      }
      foreach (BuildDefinitionTemplate template in builtInTemplates)
        this.LocalizeTemplate(requestContext, executingAssembly, template);
      return builtInTemplates;
    }

    private void LocalizeTemplate(
      IVssRequestContext requestContext,
      Assembly assembly,
      BuildDefinitionTemplate template)
    {
      try
      {
        ResourceManager rm = new ResourceManager(template.Id + "Resources", assembly);
        template.LocalizeContent((Func<string, string>) (resourceString => this.GetResourceString(requestContext, rm, resourceString)));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(nameof (DefinitionTemplateService), ex);
      }
    }

    private string GetResourceString(
      IVssRequestContext requestContext,
      ResourceManager resourceManager,
      string resourceString)
    {
      try
      {
        return resourceManager.GetString(resourceString);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(nameof (DefinitionTemplateService), ex);
        return resourceString;
      }
    }

    internal BuildDefinitionTemplate GetBuiltInTemplate(
      IVssRequestContext requestContext,
      string templateId,
      bool withProcessParameters = false)
    {
      BuildDefinitionTemplate template = (BuildDefinitionTemplate) null;
      string resourceNameFormat = this.GetTemplateResourceNameFormat(withProcessParameters);
      Assembly executingAssembly = Assembly.GetExecutingAssembly();
      Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(string.Format(resourceNameFormat, (object) templateId.ToLowerInvariant()));
      if (manifestResourceStream != null)
      {
        using (manifestResourceStream)
        {
          try
          {
            template = ServerBuildDefinitionHelpers.GetTemplateFromStream(manifestResourceStream);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(nameof (DefinitionTemplateService), ex);
          }
        }
      }
      if (template != null)
        this.LocalizeTemplate(requestContext, executingAssembly, template);
      return template;
    }

    private string GetTemplateResourceNamePrefix(bool withProcessParameters = false) => withProcessParameters ? "Microsoft.TeamFoundation.Build2.Server.Templates.NewCIWorkflow" : "Microsoft.TeamFoundation.Build2.Server.Templates.BuildDefinitions.";

    private string GetTemplateResourceNameFormat(bool withProcessParameters = false) => withProcessParameters ? "Microsoft.TeamFoundation.Build2.Server.Templates.NewCIWorkflow.{0}.json" : "Microsoft.TeamFoundation.Build2.Server.Templates.BuildDefinitions.{0}.json";

    private void DeserializeTemplates(
      IVssRequestContext requestContext,
      IList<BuildDefinitionTemplate> templates)
    {
      using (requestContext.TraceScope(nameof (DefinitionTemplateService), nameof (DeserializeTemplates)))
      {
        using (IDisposableReadOnlyList<IBuildOption> extensions = requestContext.GetExtensions<IBuildOption>())
        {
          List<IBuildOption> list = extensions.OrderBy<IBuildOption, int>((Func<IBuildOption, int>) (option => option.GetOrdinal())).ToList<IBuildOption>();
          foreach (BuildDefinitionTemplate template in (IEnumerable<BuildDefinitionTemplate>) templates)
          {
            if (template.Template.Repository != null)
              requestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(requestContext, template.Template.Repository.Type).AfterDeserialize(requestContext, template.Template.Repository);
            template.Template.ModernizeOptions(requestContext, (IList<IBuildOption>) list);
            template.SetIconUrls(requestContext);
          }
        }
      }
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }
  }
}
