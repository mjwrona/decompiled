// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions.ReleaseDefinitionEnvironmentTemplateExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Common.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions
{
  public static class ReleaseDefinitionEnvironmentTemplateExtensions
  {
    private const string TemplateResourcesNameField = "Name";
    private const string TemplateResourcesDescriptionField = "Description";
    private const string ResourceNameStartEnclosure = "{";
    private const string ResourceNameEndEnclosure = "}";

    public static void LocalizeContent(
      this ReleaseDefinitionEnvironmentTemplate template,
      string resourceName)
    {
      Assembly executingAssembly = Assembly.GetExecutingAssembly();
      ResourceManager resourceManager = new ResourceManager(resourceName, executingAssembly);
      ReleaseDefinitionEnvironmentTemplateExtensions.LocalizeNameAndDescription(template, resourceManager);
      ReleaseDefinitionEnvironmentTemplateExtensions.LocalizeProcessParameters(template, resourceManager);
    }

    private static void LocalizeNameAndDescription(
      ReleaseDefinitionEnvironmentTemplate template,
      ResourceManager resourceManager)
    {
      template.Name = resourceManager.GetString("Name");
      template.Description = resourceManager.GetString("Description");
    }

    private static void LocalizeProcessParameters(
      ReleaseDefinitionEnvironmentTemplate template,
      ResourceManager resourceManager)
    {
      IList<TaskInputDefinitionBase> inputs = template?.Environment?.ProcessParameters?.Inputs;
      if (inputs == null)
        return;
      foreach (TaskInputDefinitionBase input in (IEnumerable<TaskInputDefinitionBase>) inputs)
        ReleaseDefinitionEnvironmentTemplateExtensions.LocalizeInput(input, resourceManager);
    }

    private static void LocalizeInput(
      TaskInputDefinitionBase input,
      ResourceManager resourceManager)
    {
      input.Label = ReleaseDefinitionEnvironmentTemplateExtensions.LocalizeValue(input.Label, resourceManager);
      input.HelpMarkDown = ReleaseDefinitionEnvironmentTemplateExtensions.LocalizeValue(input.HelpMarkDown, resourceManager);
    }

    private static string LocalizeValue(string value, ResourceManager resourceManager)
    {
      value = value?.Trim();
      StringComparison comparisonType = StringComparison.OrdinalIgnoreCase;
      if (string.IsNullOrWhiteSpace(value) || !value.StartsWith("{", comparisonType) || !value.EndsWith("}", comparisonType))
        return value;
      value = value.Trim("{"[0], "}"[0]);
      return resourceManager.GetString(value);
    }
  }
}
