// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.YamlObjectReader
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating;
using System;
using System.IO;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml
{
  internal sealed class YamlObjectReader : IObjectReader
  {
    private readonly Parser m_parser;
    private ParsingEvent m_current;

    internal YamlObjectReader(TextReader input) => this.m_parser = new Parser(input);

    public bool AllowScalar(
      out int? line,
      out int? column,
      out string value,
      out Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens.ScalarStyle? style)
    {
      if (this.EvaluateCurrent() is Scalar current)
      {
        line = new int?(current.Start.Line);
        column = new int?(current.Start.Column);
        value = current.Value;
        switch (current.Style)
        {
          case YamlDotNet.Core.ScalarStyle.Plain:
            style = new Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens.ScalarStyle?(Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens.ScalarStyle.Plain);
            break;
          case YamlDotNet.Core.ScalarStyle.SingleQuoted:
            style = new Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens.ScalarStyle?(Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens.ScalarStyle.SingleQuoted);
            break;
          case YamlDotNet.Core.ScalarStyle.DoubleQuoted:
            style = new Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens.ScalarStyle?(Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens.ScalarStyle.DoubleQuoted);
            break;
          case YamlDotNet.Core.ScalarStyle.Literal:
            style = new Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens.ScalarStyle?(Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens.ScalarStyle.Literal);
            break;
          case YamlDotNet.Core.ScalarStyle.Folded:
            style = new Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens.ScalarStyle?(Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens.ScalarStyle.Folded);
            break;
          default:
            style = new Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens.ScalarStyle?();
            break;
        }
        this.MoveNext();
        return true;
      }
      line = new int?();
      column = new int?();
      value = (string) null;
      style = new Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens.ScalarStyle?();
      return false;
    }

    public bool AllowSequenceStart(out int? line, out int? column)
    {
      if (this.EvaluateCurrent() is SequenceStart current)
      {
        line = new int?(current.Start.Line);
        column = new int?(current.Start.Column);
        this.MoveNext();
        return true;
      }
      line = new int?();
      column = new int?();
      return false;
    }

    public bool AllowSequenceEnd()
    {
      if (!(this.EvaluateCurrent() is SequenceEnd))
        return false;
      this.MoveNext();
      return true;
    }

    public bool AllowMappingStart(out int? line, out int? column)
    {
      if (this.EvaluateCurrent() is MappingStart current)
      {
        line = new int?(current.Start.Line);
        column = new int?(current.Start.Column);
        this.MoveNext();
        return true;
      }
      line = new int?();
      column = new int?();
      return false;
    }

    public bool AllowMappingEnd()
    {
      if (!(this.EvaluateCurrent() is MappingEnd))
        return false;
      this.MoveNext();
      return true;
    }

    public void ValidateEnd()
    {
      if (!(this.EvaluateCurrent() is DocumentEnd))
        throw new InvalidOperationException(YamlStrings.ExpectedDocumentEnd());
      this.MoveNext();
      if (!(this.EvaluateCurrent() is StreamEnd))
        throw new InvalidOperationException(YamlStrings.ExpectedStreamEnd());
      this.MoveNext();
      if (this.MoveNext())
        throw new InvalidOperationException(YamlStrings.ExpectedEndOfParseEvents());
    }

    public void ValidateStart()
    {
      if (this.EvaluateCurrent() != null)
        throw new InvalidOperationException("Unexpected parser state");
      if (!this.MoveNext())
        throw new InvalidOperationException(YamlStrings.ExpectedParseEvent());
      if (!(this.EvaluateCurrent() is StreamStart))
        throw new InvalidOperationException(YamlStrings.ExpectedStreamStart());
      this.MoveNext();
      if (!(this.EvaluateCurrent() is DocumentStart))
        throw new InvalidOperationException(YamlStrings.ExpectedDocumentStart());
      this.MoveNext();
    }

    private ParsingEvent EvaluateCurrent()
    {
      if (this.m_current == null)
      {
        this.m_current = this.m_parser.Current;
        if (this.m_current != null)
        {
          if (this.m_current is Scalar current3)
          {
            if (current3.Anchor != null)
              throw new InvalidOperationException(YamlStrings.AnchorsNotSupported((object) current3.Anchor));
          }
          else if (this.m_current is MappingStart current2)
          {
            if (current2.Anchor != null)
              throw new InvalidOperationException(YamlStrings.AnchorsNotSupported((object) current2.Anchor));
          }
          else if (this.m_current is SequenceStart current1)
          {
            if (current1.Anchor != null)
              throw new InvalidOperationException(YamlStrings.AnchorsNotSupported((object) current1.Anchor));
          }
          else if (!(this.m_current is MappingEnd) && !(this.m_current is SequenceEnd) && !(this.m_current is DocumentStart) && !(this.m_current is DocumentEnd) && !(this.m_current is StreamStart) && !(this.m_current is StreamEnd))
            throw new InvalidOperationException(YamlStrings.UnexpectedParsingEventType((object) this.m_current.GetType().Name));
        }
      }
      return this.m_current;
    }

    private bool MoveNext()
    {
      this.m_current = (ParsingEvent) null;
      return this.m_parser.MoveNext();
    }
  }
}
