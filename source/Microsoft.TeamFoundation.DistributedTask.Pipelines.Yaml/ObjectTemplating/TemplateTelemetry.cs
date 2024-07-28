// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.TemplateTelemetry
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using System;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating
{
  public sealed class TemplateTelemetry
  {
    public string SchemaType { get; set; }

    public TimeSpan ElapsedTime { get; set; }

    public int ErrorCount { get; set; }

    public int ParserEventCount { get; set; }

    public int MaxParserEventCount { get; set; }

    public int GreatestParserDepth { get; set; }

    public int MaxParserDepth { get; set; }

    public int EstimatedMemory { get; set; }

    public int MaxMemory { get; set; }

    public int GreatestFileSize { get; set; }

    public int MaxFileSize { get; set; }

    public int FileCount { get; set; }

    public int MaxFileCount { get; set; }
  }
}
