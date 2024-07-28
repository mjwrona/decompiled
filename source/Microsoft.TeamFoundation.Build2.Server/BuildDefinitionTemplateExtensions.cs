// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildDefinitionTemplateExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Common.Contracts;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class BuildDefinitionTemplateExtensions
  {
    private const string c_TemplateResourcesNameField = "Name";
    private const string c_TemplateResourcesDescriptionField = "Description";
    private const string c_resourceNameStartEnclosure = "{";
    private const string c_resourceNameEndEnclosure = "}";

    public static void SetIconUrls(
      this BuildDefinitionTemplate template,
      IVssRequestContext requestContext)
    {
      if (template == null)
        return;
      TaskDefinition taskDefinition = (TaskDefinition) null;
      IDistributedTaskPoolService service = requestContext.GetService<IDistributedTaskPoolService>();
      try
      {
        taskDefinition = service.GetTaskDefinitions(requestContext, new Guid?(template.IconTaskId)).OrderByDescending<TaskDefinition, TaskVersion>((Func<TaskDefinition, TaskVersion>) (td => td.Version)).FirstOrDefault<TaskDefinition>();
      }
      catch (Exception ex)
      {
        requestContext.TraceException("DefinitionTemplateService", ex);
      }
      if (taskDefinition == null)
        return;
      string absoluteUri = requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "distributedtask", TaskResourceIds.TaskIcons, (object) new
      {
        taskId = taskDefinition.Id,
        versionString = taskDefinition.Version.ToString()
      }).AbsoluteUri;
      template.Icons["image/png"] = absoluteUri;
    }

    public static void LocalizeContent(
      this BuildDefinitionTemplate template,
      Func<string, string> getResourceString)
    {
      BuildDefinitionTemplateExtensions.LocalizeNameAndDescription(template, getResourceString);
      BuildDefinitionTemplateExtensions.LocalizeProcessParameters(template, getResourceString);
    }

    private static void LocalizeNameAndDescription(
      BuildDefinitionTemplate template,
      Func<string, string> getResourceString)
    {
      template.Name = getResourceString("Name");
      template.Description = getResourceString("Description");
    }

    private static void LocalizeProcessParameters(
      BuildDefinitionTemplate template,
      Func<string, string> getResourceString)
    {
      IList<TaskInputDefinitionBase> inputs = template?.Template?.ProcessParameters?.Inputs;
      if (inputs == null)
        return;
      foreach (TaskInputDefinitionBase input in (IEnumerable<TaskInputDefinitionBase>) inputs)
        BuildDefinitionTemplateExtensions.LocalizeInput(input, getResourceString);
    }

    private static void LocalizeInput(
      TaskInputDefinitionBase input,
      Func<string, string> getResourceString)
    {
      input.Label = BuildDefinitionTemplateExtensions.LocalizeValue(input.Label, getResourceString);
      input.HelpMarkDown = BuildDefinitionTemplateExtensions.LocalizeValue(input.HelpMarkDown, getResourceString);
    }

    private static string LocalizeValue(string value, Func<string, string> getResourceString)
    {
      value = value?.Trim();
      if (string.IsNullOrWhiteSpace(value) || !value.StartsWith("{") || !value.EndsWith("}"))
        return value;
      value = value.Trim("{"[0], "}"[0]).Trim();
      return getResourceString(value);
    }
  }
}
