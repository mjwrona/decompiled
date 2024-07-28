// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.EmptyTraceWriter
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating
{
  internal class EmptyTraceWriter : ITraceWriter
  {
    public void Error(string format, params object[] args)
    {
    }

    public void Warning(string format, params object[] args)
    {
    }

    public void Info(string format, params object[] args)
    {
    }

    public void Verbose(string format, params object[] args)
    {
    }

    public void Telemetry(TemplateTelemetry telemetry)
    {
    }
  }
}
