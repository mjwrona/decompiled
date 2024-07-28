// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ParseResult
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens;
using System.IO;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml
{
  internal sealed class ParseResult
  {
    public TemplateContext Context { get; set; }

    public PipelineResources Resources { get; set; }

    public string SchemaType { get; set; }

    public TemplateToken Value { get; set; }

    public YamlTemplateComposition Composition { get; set; }

    public string ToYaml()
    {
      if (this.Value == null)
        return (string) null;
      using (StringWriter writer = new StringWriter())
      {
        writer.NewLine = "\n";
        TemplateWriter.Write((IObjectWriter) new YamlWriter(writer), this.Value);
        writer.Flush();
        return writer.ToString();
      }
    }
  }
}
