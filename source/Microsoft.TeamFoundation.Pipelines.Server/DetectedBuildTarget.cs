// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.DetectedBuildTarget
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public class DetectedBuildTarget
  {
    public string Type { get; }

    public string Path { get; }

    public IReadOnlyDictionary<string, string> Settings { get; }

    public DetectedBuildTarget(
      string type,
      string path,
      IReadOnlyDictionary<string, string> settings = null)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(type, nameof (type));
      ArgumentUtility.CheckStringForNullOrEmpty(path, nameof (path));
      this.Type = type;
      this.Path = path;
      this.Settings = settings ?? (IReadOnlyDictionary<string, string>) new Dictionary<string, string>();
    }
  }
}
