// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.TemplateWriter
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating
{
  internal sealed class TemplateWriter
  {
    internal static void Write(IObjectWriter objectWriter, TemplateToken value)
    {
      objectWriter.WriteStart();
      TemplateWriter.WriteValue(objectWriter, value);
      objectWriter.WriteEnd();
    }

    private static void WriteValue(IObjectWriter objectWriter, TemplateToken value)
    {
      switch (value)
      {
        case LiteralToken literalToken:
          objectWriter.WriteString(literalToken.Value, literalToken.Style);
          break;
        case SequenceToken sequenceToken:
          objectWriter.WriteSequenceStart();
          foreach (TemplateToken templateToken in sequenceToken)
            TemplateWriter.WriteValue(objectWriter, templateToken);
          objectWriter.WriteSequenceEnd();
          break;
        case MappingToken mappingToken:
          objectWriter.WriteMappingStart();
          foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in mappingToken)
          {
            TemplateWriter.WriteValue(objectWriter, (TemplateToken) keyValuePair.Key);
            TemplateWriter.WriteValue(objectWriter, keyValuePair.Value);
          }
          objectWriter.WriteMappingEnd();
          break;
        default:
          throw new NotSupportedException(string.Format("Unexpected type '{0}'", (object) value.GetType()));
      }
    }
  }
}
