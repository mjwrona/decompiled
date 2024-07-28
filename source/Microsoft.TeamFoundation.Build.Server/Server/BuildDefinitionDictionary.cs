// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildDefinitionDictionary
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build.Server
{
  internal class BuildDefinitionDictionary : Dictionary<string, BuildDefinition>
  {
    public BuildDefinitionDictionary()
      : base((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
    }

    public BuildDefinitionDictionary(IEnumerable<BuildDefinition> collection)
      : this()
    {
      foreach (BuildDefinition buildDefinition in collection)
        this[buildDefinition.Uri] = buildDefinition;
    }

    public void Add(BuildDefinition item) => this[item.Uri] = item;

    public void RemoveByTeamProject(TeamProject project)
    {
      List<BuildDefinition> buildDefinitionList = new List<BuildDefinition>();
      foreach (BuildDefinition buildDefinition in this.Values)
      {
        if (project.Uri.Equals(buildDefinition.TeamProject.Uri, StringComparison.OrdinalIgnoreCase))
          buildDefinitionList.Add(buildDefinition);
      }
      foreach (BuildDefinition buildDefinition in buildDefinitionList)
        this.Remove(buildDefinition.Uri);
    }
  }
}
