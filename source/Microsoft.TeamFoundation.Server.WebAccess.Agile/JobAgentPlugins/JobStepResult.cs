// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.JobAgentPlugins.JobStepResult
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.JobAgentPlugins
{
  public class JobStepResult
  {
    private Stopwatch _watch;

    public JobStepResult(string stepName)
    {
      this.JobStepName = stepName;
      this._watch = new Stopwatch();
    }

    public TeamFoundationJobExecutionResult ExecutionResult { get; set; }

    public string Message { get; set; }

    public string JobStepName { get; set; }

    public double ElapsedMs => this._watch.Elapsed.TotalMilliseconds;

    internal void StartTimer() => this._watch.Restart();

    internal void StopTimer() => this._watch.Stop();
  }
}
