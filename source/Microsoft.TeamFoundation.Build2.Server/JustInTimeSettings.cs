// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.JustInTimeSettings
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.Azure.Pipelines.Server.ObjectModel;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class JustInTimeSettings
  {
    public JustInTimeConfiguration Configuration { get; set; }

    public Dictionary<string, string> Secrets { get; set; }

    public JustInTimeContext Context { get; set; }

    public bool PreviewRun { get; set; }

    public string YamlOverride { get; set; }
  }
}
