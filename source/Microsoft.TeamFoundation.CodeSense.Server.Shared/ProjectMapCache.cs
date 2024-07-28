// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.ProjectMapCache
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  public class ProjectMapCache
  {
    private IDictionary<string, Guid> _reverseLookup = (IDictionary<string, Guid>) new Dictionary<string, Guid>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public ProjectMapCache()
      : this((IDictionary<Guid, string>) new Dictionary<Guid, string>())
    {
    }

    public ProjectMapCache(IDictionary<Guid, string> projectmap) => this.ProjectMap = projectmap;

    public IDictionary<Guid, string> ProjectMap { get; set; }

    public bool TryGetValue(Guid projectGuid, out string projectName) => this.ProjectMap.TryGetValue(projectGuid, out projectName);

    public bool TryGetValue(string projectName, out Guid projectGuid) => this._reverseLookup.TryGetValue(projectName, out projectGuid);

    public void AddByGuid(Guid projectGuid, string projectName) => this.Add(projectName, projectGuid);

    public void AddByName(string projectName, Guid projectGuid) => this.Add(projectName, projectGuid);

    private void Add(string projectName, Guid projectGuid)
    {
      if (!this._reverseLookup.ContainsKey(projectName))
        this._reverseLookup.Add(projectName, projectGuid);
      if (this.ProjectMap.ContainsKey(projectGuid))
        return;
      this.ProjectMap.Add(projectGuid, projectName);
    }
  }
}
