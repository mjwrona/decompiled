// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.DetectedBuildFramework
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public class DetectedBuildFramework
  {
    public string Id { get; }

    public string Version { get; }

    public int Weight { get; set; }

    public IReadOnlyDictionary<string, string> Settings { get; }

    public IReadOnlyList<DetectedBuildTarget> BuildTargets { get; }

    public DetectedBuildFramework(string id, string version)
      : this(id, version, (IReadOnlyDictionary<string, string>) null, (IReadOnlyList<DetectedBuildTarget>) null)
    {
    }

    public DetectedBuildFramework(
      string id,
      string version,
      IReadOnlyList<DetectedBuildTarget> buildTargets)
      : this(id, version, (IReadOnlyDictionary<string, string>) null, buildTargets)
    {
    }

    public DetectedBuildFramework(
      string id,
      string version,
      IReadOnlyDictionary<string, string> settings)
      : this(id, version, settings, (IReadOnlyList<DetectedBuildTarget>) null)
    {
    }

    public DetectedBuildFramework(
      string id,
      string version,
      IReadOnlyDictionary<string, string> settings,
      IReadOnlyList<DetectedBuildTarget> buildTargets)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(id, nameof (id));
      this.Id = id;
      this.Version = version;
      this.Settings = settings ?? (IReadOnlyDictionary<string, string>) new Dictionary<string, string>();
      this.BuildTargets = buildTargets ?? (IReadOnlyList<DetectedBuildTarget>) new List<DetectedBuildTarget>();
    }
  }
}
