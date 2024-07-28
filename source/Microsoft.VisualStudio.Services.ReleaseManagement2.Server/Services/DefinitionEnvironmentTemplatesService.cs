// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services.DefinitionEnvironmentTemplatesService
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.EnvironmentTemplates;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Validations;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services
{
  public class DefinitionEnvironmentTemplatesService : ReleaseManagement2ServiceBase
  {
    private const string CTemplateResourcesSuffix = "Resources";
    private const string CTemplateResourcesNameField = "Name";
    private const string CTemplateResourcesDescriptionField = "Description";
    private readonly Assembly executingAssembly;
    private readonly Func<IVssRequestContext, Guid, Guid, ReleaseDefinitionEnvironmentTemplate> getCustomTemplate;
    private readonly Func<IVssRequestContext, Guid, bool, IEnumerable<ReleaseDefinitionEnvironmentTemplate>> listCustomTemplates;
    private readonly Func<IVssRequestContext, IEnumerable<ReleaseDefinitionEnvironmentTemplate>> listInstalledTemplates;
    public const string TemplateResourceNamePrefix = "Microsoft.VisualStudio.Services.ReleaseManagement.Server.Templates.DefinitionEnvironments.";

    public DefinitionEnvironmentTemplatesService()
      : this(Assembly.GetExecutingAssembly(), DefinitionEnvironmentTemplatesService.\u003C\u003EO.\u003C0\u003E__GetCustomTemplate ?? (DefinitionEnvironmentTemplatesService.\u003C\u003EO.\u003C0\u003E__GetCustomTemplate = new Func<IVssRequestContext, Guid, Guid, ReleaseDefinitionEnvironmentTemplate>(DefinitionEnvironmentTemplatesService.GetCustomTemplate)), DefinitionEnvironmentTemplatesService.\u003C\u003EO.\u003C1\u003E__ListCustomTemplates ?? (DefinitionEnvironmentTemplatesService.\u003C\u003EO.\u003C1\u003E__ListCustomTemplates = new Func<IVssRequestContext, Guid, bool, IEnumerable<ReleaseDefinitionEnvironmentTemplate>>(DefinitionEnvironmentTemplatesService.ListCustomTemplates)), DefinitionEnvironmentTemplatesService.\u003C\u003EO.\u003C2\u003E__GetExtensionEnvironmentTemplates ?? (DefinitionEnvironmentTemplatesService.\u003C\u003EO.\u003C2\u003E__GetExtensionEnvironmentTemplates = new Func<IVssRequestContext, IEnumerable<ReleaseDefinitionEnvironmentTemplate>>(EnvironmentTemplateExtensionsRetriever.GetExtensionEnvironmentTemplates)))
    {
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Needed for testability")]
    protected DefinitionEnvironmentTemplatesService(
      Assembly executingAssembly,
      Func<IVssRequestContext, Guid, Guid, ReleaseDefinitionEnvironmentTemplate> getCustomTemplate,
      Func<IVssRequestContext, Guid, bool, IEnumerable<ReleaseDefinitionEnvironmentTemplate>> listCustomTemplates,
      Func<IVssRequestContext, IEnumerable<ReleaseDefinitionEnvironmentTemplate>> listInstalledTemplates)
    {
      this.executingAssembly = executingAssembly;
      this.getCustomTemplate = getCustomTemplate;
      this.listCustomTemplates = listCustomTemplates;
      this.listInstalledTemplates = listInstalledTemplates;
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional")]
    public IEnumerable<ReleaseDefinitionEnvironmentTemplate> ListDefinitionEnvironmentTemplates(
      IVssRequestContext context,
      Guid projectId,
      bool isDeleted = false)
    {
      List<ReleaseDefinitionEnvironmentTemplate> first = context != null ? DefinitionEnvironmentTemplatesService.GetBuiltInTemplates(context, this.executingAssembly).ToList<ReleaseDefinitionEnvironmentTemplate>() : throw new ArgumentNullException(nameof (context));
      first.ForEach((Action<ReleaseDefinitionEnvironmentTemplate>) (t => t.Environment = (ReleaseDefinitionEnvironment) null));
      IEnumerable<ReleaseDefinitionEnvironmentTemplate> second = this.listCustomTemplates(context, projectId, isDeleted);
      List<ReleaseDefinitionEnvironmentTemplate> list = this.listInstalledTemplates(context).ToList<ReleaseDefinitionEnvironmentTemplate>();
      list.ForEach((Action<ReleaseDefinitionEnvironmentTemplate>) (t => t.Environment = (ReleaseDefinitionEnvironment) null));
      return first.Concat<ReleaseDefinitionEnvironmentTemplate>(second).Concat<ReleaseDefinitionEnvironmentTemplate>((IEnumerable<ReleaseDefinitionEnvironmentTemplate>) list);
    }

    public ReleaseDefinitionEnvironmentTemplate GetDefinitionEnvironmentTemplate(
      IVssRequestContext context,
      Guid projectId,
      Guid templateId)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      ReleaseDefinitionEnvironmentTemplate template = (DefinitionEnvironmentTemplatesService.GetBuiltInTemplates(context, this.executingAssembly).SingleOrDefault<ReleaseDefinitionEnvironmentTemplate>((Func<ReleaseDefinitionEnvironmentTemplate, bool>) (t => t.Id == templateId)) ?? this.getCustomTemplate(context, projectId, templateId)) ?? this.listInstalledTemplates(context).SingleOrDefault<ReleaseDefinitionEnvironmentTemplate>((Func<ReleaseDefinitionEnvironmentTemplate, bool>) (t => t.Id == templateId));
      if (template == null)
        throw new ReleaseDefinitionEnvironmentTemplateNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.DefinitionEnvironmentTemplateNotFound, (object) templateId));
      Func<Task<List<TaskAgentQueue>>> func = (Func<Task<List<TaskAgentQueue>>>) (() => context.GetClient<TaskAgentHttpClient>().GetAgentQueuesAsync(projectId, actionFilter: new TaskAgentQueueActionFilter?(TaskAgentQueueActionFilter.Use)));
      List<TaskAgentQueue> agentQueues = context.ExecuteAsyncAndSyncResult<List<TaskAgentQueue>>(func);
      if (template.Environment.DeployPhases.Any<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase>())
        DefinitionEnvironmentTemplatesService.UpdateDeployPhaseQueueIdIfRequired(context, template, agentQueues);
      else
        DefinitionEnvironmentTemplatesService.UpdateEnvironmentQueueIdIfRequired(context, template, agentQueues);
      return template;
    }

    private static void UpdateEnvironmentQueueIdIfRequired(
      IVssRequestContext context,
      ReleaseDefinitionEnvironmentTemplate template,
      List<TaskAgentQueue> agentQueues)
    {
      bool flag = agentQueues.Select<TaskAgentQueue, int>((Func<TaskAgentQueue, int>) (queue => queue.Id)).Contains<int>(template.Environment.QueueId);
      if (template.Category == "Custom" & flag)
        return;
      int num1 = 0;
      if (context.ExecutionEnvironment.IsHostedDeployment)
      {
        int firstMatchingHostedPoolId = AgentQueueUtility.GetFirstMatchingHostedPoolId(context, (IEnumerable<TaskAgentQueue>) agentQueues);
        if (firstMatchingHostedPoolId != 0)
        {
          int num2 = agentQueues.Where<TaskAgentQueue>((Func<TaskAgentQueue, bool>) (x => x.Pool.Id == firstMatchingHostedPoolId)).Select<TaskAgentQueue, int>((Func<TaskAgentQueue, int>) (x => x.Id)).FirstOrDefault<int>();
          if (num2 != 0)
            num1 = num2;
        }
      }
      else
      {
        TaskAgentQueue matchingDefaultQueue = AgentQueueUtility.GetFirstMatchingDefaultQueue((IEnumerable<TaskAgentQueue>) agentQueues);
        num1 = matchingDefaultQueue != null ? matchingDefaultQueue.Id : 0;
      }
      if (num1 == 0)
        return;
      template.Environment.QueueId = num1;
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "By Design")]
    private static void UpdateDeployPhaseQueueIdIfRequired(
      IVssRequestContext context,
      ReleaseDefinitionEnvironmentTemplate template,
      List<TaskAgentQueue> agentQueues)
    {
      IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase> source = template.Environment.DeployPhases.Where<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase, bool>) (phase => phase.PhaseType == DeployPhaseTypes.AgentBasedDeployment));
      if (!source.Any<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase>())
        return;
      List<int> invalidQueueIds = source.Select<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase, int>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase, int>) (phase => ((DeploymentInput) phase.GetDeploymentInput()).QueueId)).Distinct<int>().Except<int>(agentQueues.Select<TaskAgentQueue, int>((Func<TaskAgentQueue, int>) (queue => queue.Id))).ToList<int>();
      if (template.Category == "Custom" && !invalidQueueIds.Any<int>())
        return;
      IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase> deployPhases = source;
      int num1 = 0;
      if (context.ExecutionEnvironment.IsHostedDeployment)
      {
        int firstMatchingHostedPoolId = AgentQueueUtility.GetFirstMatchingHostedPoolId(context, (IEnumerable<TaskAgentQueue>) agentQueues);
        if (firstMatchingHostedPoolId != 0)
        {
          int num2 = agentQueues.Where<TaskAgentQueue>((Func<TaskAgentQueue, bool>) (x => x.Pool.Id == firstMatchingHostedPoolId)).Select<TaskAgentQueue, int>((Func<TaskAgentQueue, int>) (x => x.Id)).FirstOrDefault<int>();
          if (num2 != 0)
            num1 = num2;
        }
      }
      else
      {
        TaskAgentQueue matchingDefaultQueue = AgentQueueUtility.GetFirstMatchingDefaultQueue((IEnumerable<TaskAgentQueue>) agentQueues);
        num1 = matchingDefaultQueue != null ? matchingDefaultQueue.Id : 0;
      }
      if (template.Category == "Custom")
        deployPhases = source.Where<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase, bool>) (phase => invalidQueueIds.Contains(((DeploymentInput) phase.GetDeploymentInput()).QueueId)));
      if (num1 == 0)
        return;
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase deployPhase in deployPhases)
        ((DeploymentInput) deployPhase.GetDeploymentInput()).QueueId = num1;
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf sdk needs it to be non-static")]
    public ReleaseDefinitionEnvironmentTemplate SaveDefinitionEnvironmentTemplate(
      IVssRequestContext context,
      Guid projectId,
      ReleaseDefinitionEnvironmentTemplate template)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (template == null)
        throw new ArgumentNullException(nameof (template));
      Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions.ReleaseDefinitionExtensions.EnsureEnvironmentDefaults(context, projectId, template.Environment);
      ReleaseDefinitionEnvironmentTemplate webApiTemplate = template.Validate(context, projectId);
      webApiTemplate.Environment.ValidateWorkflowTasks(context, projectId);
      DefinitionEnvironmentTemplatesService.FixLatestMajorTaskVersions(context, webApiTemplate.Environment);
      DefinitionEnvironmentTemplate serverTemplate = webApiTemplate.FromWebApi();
      Func<DefinitionEnvironmentTemplateSqlComponent, DefinitionEnvironmentTemplate> action = (Func<DefinitionEnvironmentTemplateSqlComponent, DefinitionEnvironmentTemplate>) (component => component.AddEnvironmentTemplate(projectId, serverTemplate));
      return context.ExecuteWithinUsingWithComponent<DefinitionEnvironmentTemplateSqlComponent, DefinitionEnvironmentTemplate>(action).ToWebApi();
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These are optional parameters")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf sdk needs it to be non-static")]
    public void SoftDeleteDefinitionEnvironmentTemplate(
      IVssRequestContext context,
      Guid projectId,
      Guid? templateId = null)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      Action<DefinitionEnvironmentTemplateSqlComponent> action = (Action<DefinitionEnvironmentTemplateSqlComponent>) (component => component.SoftDeleteEnvironmentTemplate(projectId, templateId));
      context.ExecuteWithinUsingWithComponent<DefinitionEnvironmentTemplateSqlComponent>(action);
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These are optional parameters")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf sdk needs it to be non-static")]
    public void HardDeleteDefinitionEnvironmentTemplates(
      IVssRequestContext context,
      int daysToRetain)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      Action<DefinitionEnvironmentTemplateSqlComponent> action = (Action<DefinitionEnvironmentTemplateSqlComponent>) (component => component.HardDeleteDefinitionEnvironmentTemplates(daysToRetain));
      context.ExecuteWithinUsingWithComponent<DefinitionEnvironmentTemplateSqlComponent>(action);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf sdk needs it to be non-static")]
    public ReleaseDefinitionEnvironmentTemplate UndeleteDefinitionEnvironmentTemplate(
      IVssRequestContext context,
      Guid projectId,
      Guid templateId)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      context.GetUserId(true);
      Action<DefinitionEnvironmentTemplateSqlComponent> action = (Action<DefinitionEnvironmentTemplateSqlComponent>) (component => component.UndeleteEnvironmentTemplate(projectId, templateId));
      context.ExecuteWithinUsingWithComponent<DefinitionEnvironmentTemplateSqlComponent>(action);
      return this.GetDefinitionEnvironmentTemplate(context, projectId, templateId);
    }

    private static void FixLatestMajorTaskVersions(
      IVssRequestContext context,
      ReleaseDefinitionEnvironment environment)
    {
      if (environment == null)
        return;
      IDistributedTaskPoolService service = context.GetService<IDistributedTaskPoolService>();
      if (environment.DeployPhases == null)
        return;
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase deployPhase in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase>) environment.DeployPhases)
      {
        if (deployPhase.WorkflowTasks != null)
        {
          foreach (WorkflowTask workflowTask in (IEnumerable<WorkflowTask>) deployPhase.WorkflowTasks)
          {
            if (workflowTask.Version == "*")
            {
              TaskDefinition latestMajorVersion = service.GetLatestMajorVersion(context, workflowTask.TaskId);
              if (latestMajorVersion != null)
                workflowTask.Version = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.*", (object) latestMajorVersion.Version.Major);
            }
          }
        }
      }
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Calling into location service")]
    private static IEnumerable<ReleaseDefinitionEnvironmentTemplate> GetBuiltInTemplates(
      IVssRequestContext context,
      Assembly assembly)
    {
      List<ReleaseDefinitionEnvironmentTemplate> builtInTemplates = new List<ReleaseDefinitionEnvironmentTemplate>();
      List<string> stringList = new List<string>();
      string[] manifestResourceNames = assembly.GetManifestResourceNames();
      IEnumerable<string> strings = ((IEnumerable<string>) manifestResourceNames).Where<string>((Func<string, bool>) (name => name.StartsWith("Microsoft.VisualStudio.Services.ReleaseManagement.Server.Templates.DefinitionEnvironments.", StringComparison.OrdinalIgnoreCase)));
      TaskDefinitionResolver definitionResolver = new TaskDefinitionResolver(context.GetService<IDistributedTaskPoolService>().GetTaskDefinitions(context));
      ILocationDataProvider locationData = context.GetService<ILocationService>().GetLocationData(context, new Guid("A85B8835-C1A1-4AAC-AE97-1C3D0BA72DBD"));
      foreach (string str in strings)
      {
        using (StreamReader streamReader = new StreamReader(assembly.GetManifestResourceStream(str)))
        {
          string end = streamReader.ReadToEnd();
          ReleaseDefinitionBuiltInEnvironmentTemplate environmentTemplate1 = JsonConvert.DeserializeObject<ReleaseDefinitionBuiltInEnvironmentTemplate>(end);
          bool flag = true;
          if (environmentTemplate1.FeatureFlags != null)
          {
            foreach (string featureFlag in (IEnumerable<string>) environmentTemplate1.FeatureFlags)
            {
              if (!context.IsFeatureEnabled(featureFlag))
              {
                flag = false;
                break;
              }
            }
          }
          if (flag)
          {
            stringList.Add(Path.GetFileNameWithoutExtension(str).Replace("Microsoft.VisualStudio.Services.ReleaseManagement.Server.Templates.DefinitionEnvironments.", string.Empty) + "Resources");
            ReleaseDefinitionEnvironmentTemplate environmentTemplate2 = JsonConvert.DeserializeObject<ReleaseDefinitionEnvironmentTemplate>(end);
            builtInTemplates.Add(environmentTemplate2);
            TaskDefinition taskDefinition;
            if (definitionResolver.TryResolveTaskReference(environmentTemplate2.IconTaskId, "*", out taskDefinition))
              environmentTemplate2.IconUri = locationData.GetResourceUri(context, "distributedtask", TaskResourceIds.TaskIcons, (object) new
              {
                taskId = taskDefinition.Id,
                versionString = taskDefinition.Version.ToString()
              });
          }
        }
      }
      int index = 0;
      foreach (ReleaseDefinitionEnvironmentTemplate template in builtInTemplates)
      {
        if (((IEnumerable<string>) manifestResourceNames).Contains<string>(stringList[index] + "." + "Resources".ToLower(CultureInfo.CurrentCulture)))
          template.LocalizeContent(stringList[index]);
        ++index;
      }
      return (IEnumerable<ReleaseDefinitionEnvironmentTemplate>) builtInTemplates;
    }

    private static ReleaseDefinitionEnvironmentTemplate GetCustomTemplate(
      IVssRequestContext context,
      Guid projectId,
      Guid templateId)
    {
      try
      {
        Func<DefinitionEnvironmentTemplateSqlComponent, DefinitionEnvironmentTemplate> action = (Func<DefinitionEnvironmentTemplateSqlComponent, DefinitionEnvironmentTemplate>) (component => component.GetEnvironmentTemplate(projectId, templateId));
        return context.ExecuteWithinUsingWithComponent<DefinitionEnvironmentTemplateSqlComponent, DefinitionEnvironmentTemplate>(action).ToWebApi();
      }
      catch (ReleaseDefinitionEnvironmentTemplateNotFoundException ex)
      {
        return (ReleaseDefinitionEnvironmentTemplate) null;
      }
    }

    private static IEnumerable<ReleaseDefinitionEnvironmentTemplate> ListCustomTemplates(
      IVssRequestContext context,
      Guid projectId,
      bool isDeleted)
    {
      Func<DefinitionEnvironmentTemplateSqlComponent, IEnumerable<DefinitionEnvironmentTemplate>> action = (Func<DefinitionEnvironmentTemplateSqlComponent, IEnumerable<DefinitionEnvironmentTemplate>>) (component => component.ListEnvironmentTemplates(projectId, isDeleted));
      return context.ExecuteWithinUsingWithComponent<DefinitionEnvironmentTemplateSqlComponent, IEnumerable<DefinitionEnvironmentTemplate>>(action).Select<DefinitionEnvironmentTemplate, ReleaseDefinitionEnvironmentTemplate>((Func<DefinitionEnvironmentTemplate, ReleaseDefinitionEnvironmentTemplate>) (t => t.ToWebApi()));
    }
  }
}
