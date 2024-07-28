// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.JTokenExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml
{
  public static class JTokenExtensions
  {
    public static string ToYaml(this JToken token)
    {
      using (StringWriter writer = new StringWriter())
      {
        YamlWriter yamlWriter = new YamlWriter(writer);
        yamlWriter.WriteStart();
        JTokenExtensions.WriteToken((IObjectWriter) yamlWriter, token);
        writer.Flush();
        return Regex.Replace(writer.ToString(), "\\A---\\s*", "");
      }
    }

    private static void WriteToken(IObjectWriter objectWriter, JToken token)
    {
      switch (token.Type)
      {
        case JTokenType.Object:
          JTokenExtensions.WriteMapping(objectWriter, token);
          break;
        case JTokenType.Array:
          JTokenExtensions.WriteSequence(objectWriter, token);
          break;
        case JTokenType.Integer:
        case JTokenType.Float:
          objectWriter.WriteString(token.ToString());
          break;
        case JTokenType.String:
          string str = token.ToString();
          if (str.Contains("\n"))
          {
            objectWriter.WriteString(str, new ScalarStyle?(ScalarStyle.Literal));
            break;
          }
          if (str.StartsWith("0"))
          {
            objectWriter.WriteString(str, new ScalarStyle?(ScalarStyle.SingleQuoted));
            break;
          }
          objectWriter.WriteString(str);
          break;
        case JTokenType.Boolean:
          objectWriter.WriteString(token.ToString().ToLowerInvariant());
          break;
        default:
          throw new NotSupportedException(string.Format("Unexpected type '{0}'", (object) token.Type));
      }
    }

    private static void WriteSequence(IObjectWriter objectWriter, JToken token)
    {
      JArray jarray = token as JArray;
      objectWriter.WriteSequenceStart();
      foreach (JToken token1 in jarray)
        JTokenExtensions.WriteToken(objectWriter, token1);
      objectWriter.WriteSequenceEnd();
    }

    private static void WriteMapping(IObjectWriter objectWriter, JToken token)
    {
      JObject jobject = token as JObject;
      objectWriter.WriteMappingStart();
      foreach (KeyValuePair<string, JToken> keyValuePair in jobject)
      {
        JTokenExtensions.WriteToken(objectWriter, (JToken) keyValuePair.Key);
        JTokenExtensions.WriteToken(objectWriter, keyValuePair.Value);
      }
      objectWriter.WriteMappingEnd();
    }
  }
}
