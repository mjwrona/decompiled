// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.Internals.BuildDefinitionTemplate3_2Extensions
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build.WebApi.Internals
{
  internal static class BuildDefinitionTemplate3_2Extensions
  {
    public static BuildDefinitionTemplate ToBuildDefinitionTemplate(
      this BuildDefinitionTemplate3_2 source)
    {
      if (source == null)
        return (BuildDefinitionTemplate) null;
      BuildDefinitionTemplate definitionTemplate = new BuildDefinitionTemplate()
      {
        CanDelete = source.CanDelete,
        Category = source.Category,
        DefaultHostedQueue = source.DefaultHostedQueue,
        Description = source.Description,
        IconTaskId = source.IconTaskId,
        Id = source.Id,
        Name = source.Name,
        Template = source.Template.ToBuildDefinition()
      };
      foreach (KeyValuePair<string, string> icon in (IEnumerable<KeyValuePair<string, string>>) source.Icons)
        definitionTemplate.Icons.Add(icon.Key, icon.Value);
      return definitionTemplate;
    }
  }
}
