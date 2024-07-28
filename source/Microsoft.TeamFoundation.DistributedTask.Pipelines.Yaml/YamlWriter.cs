// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.YamlWriter
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating;
using System.IO;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml
{
  internal sealed class YamlWriter : IObjectWriter
  {
    private readonly IEmitter m_emitter;

    internal YamlWriter(StringWriter writer) => this.m_emitter = (IEmitter) new Emitter((TextWriter) writer);

    public void WriteString(string value, Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens.ScalarStyle? style = null)
    {
      Scalar @event;
      if (style.HasValue)
      {
        switch (style.GetValueOrDefault())
        {
          case Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens.ScalarStyle.Plain:
            @event = new Scalar((string) null, (string) null, value, YamlDotNet.Core.ScalarStyle.Plain, true, false, Mark.Empty, Mark.Empty);
            goto label_8;
          case Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens.ScalarStyle.SingleQuoted:
            @event = new Scalar((string) null, (string) null, value, YamlDotNet.Core.ScalarStyle.SingleQuoted, false, true, Mark.Empty, Mark.Empty);
            goto label_8;
          case Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens.ScalarStyle.DoubleQuoted:
            @event = new Scalar((string) null, (string) null, value, YamlDotNet.Core.ScalarStyle.DoubleQuoted, false, true, Mark.Empty, Mark.Empty);
            goto label_8;
          case Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens.ScalarStyle.Literal:
            @event = new Scalar((string) null, (string) null, value, YamlDotNet.Core.ScalarStyle.Literal, true, true, Mark.Empty, Mark.Empty);
            goto label_8;
          case Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens.ScalarStyle.Folded:
            @event = new Scalar((string) null, (string) null, value, YamlDotNet.Core.ScalarStyle.Folded, true, true, Mark.Empty, Mark.Empty);
            goto label_8;
        }
      }
      @event = new Scalar(value ?? string.Empty);
label_8:
      this.m_emitter.Emit((ParsingEvent) @event);
    }

    public void WriteSequenceStart() => this.m_emitter.Emit((ParsingEvent) new SequenceStart((string) null, (string) null, true, SequenceStyle.Block));

    public void WriteSequenceEnd() => this.m_emitter.Emit((ParsingEvent) new SequenceEnd());

    public void WriteMappingStart() => this.m_emitter.Emit((ParsingEvent) new MappingStart());

    public void WriteMappingEnd() => this.m_emitter.Emit((ParsingEvent) new MappingEnd());

    public void WriteStart()
    {
      this.m_emitter.Emit((ParsingEvent) new StreamStart());
      this.m_emitter.Emit((ParsingEvent) new DocumentStart());
    }

    public void WriteEnd()
    {
      this.m_emitter.Emit((ParsingEvent) new DocumentEnd(true));
      this.m_emitter.Emit((ParsingEvent) new StreamEnd());
    }
  }
}
