// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.ProviderInstallationEvent
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.VisualStudio.Services.ExternalEvent;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public class ProviderInstallationEvent
  {
    public string AppName { get; set; }

    public string Action { get; set; }

    public ExternalAppInstallationInfo Installation { get; set; }

    public IEnumerable<ExternalGitRepo> Repositories { get; set; }

    public bool IsAllRepoSelected { get; set; }
  }
}
