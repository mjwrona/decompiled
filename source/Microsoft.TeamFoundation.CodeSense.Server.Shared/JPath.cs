// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.JPath
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  public sealed class JPath
  {
    public JPath(string propertyName)
    {
      this.Paths = new Dictionary<string, JPath>()
      {
        {
          propertyName,
          new JPath()
        }
      };
      this.Conditions = new Dictionary<string, HashSet<object>>();
    }

    public JPath(string propertyName, JPath propertyPath)
    {
      this.Paths = new Dictionary<string, JPath>()
      {
        {
          propertyName,
          propertyPath
        }
      };
      this.Conditions = new Dictionary<string, HashSet<object>>();
    }

    public JPath(
      string propertyName,
      string conditionPropertyName,
      object conditionPropertyValue,
      JPath propertyPath)
    {
      conditionPropertyValue = conditionPropertyValue is int num ? (object) (long) num : conditionPropertyValue;
      this.Paths = new Dictionary<string, JPath>()
      {
        {
          propertyName,
          propertyPath
        }
      };
      this.Conditions = new Dictionary<string, HashSet<object>>()
      {
        {
          conditionPropertyName,
          new HashSet<object>() { conditionPropertyValue }
        }
      };
    }

    private JPath()
    {
      this.Paths = new Dictionary<string, JPath>();
      this.Conditions = new Dictionary<string, HashSet<object>>();
    }

    [JsonProperty]
    private Dictionary<string, JPath> Paths { get; set; }

    [JsonProperty]
    private Dictionary<string, HashSet<object>> Conditions { get; set; }

    public static JPath FromCondition(string conditionPropertyName, object conditionPropertyValue)
    {
      conditionPropertyValue = conditionPropertyValue is int num ? (object) (long) num : conditionPropertyValue;
      return new JPath()
      {
        Paths = new Dictionary<string, JPath>(),
        Conditions = new Dictionary<string, HashSet<object>>()
        {
          {
            conditionPropertyName,
            new HashSet<object>() { conditionPropertyValue }
          }
        }
      };
    }

    public void Merge(JPath other)
    {
      if (new DictionaryComparer<string, HashSet<object>>((IEqualityComparer<HashSet<object>>) SetComparer<object>.Default).Equals(this.Conditions, other.Conditions))
      {
        foreach (KeyValuePair<string, JPath> path in other.Paths)
        {
          JPath jpath;
          if (this.Paths.TryGetValue(path.Key, out jpath))
            jpath.Merge(path.Value);
          else
            this.Paths[path.Key] = path.Value;
        }
      }
      else
      {
        if (!DictionaryComparer<string, JPath>.Default.Equals(this.Paths, other.Paths))
          throw new InvalidOperationException("Cannot merge path without matching conditions or subpaths.");
        foreach (KeyValuePair<string, HashSet<object>> condition in other.Conditions)
        {
          HashSet<object> objectSet;
          if (this.Conditions.TryGetValue(condition.Key, out objectSet))
            objectSet.UnionWith((IEnumerable<object>) condition.Value);
          else
            this.Conditions[condition.Key] = condition.Value;
        }
      }
    }

    public void RemoveFrom(JToken token)
    {
      foreach (JToken token1 in this.FindTokens(token))
        token1.Remove();
    }

    public IEnumerable<JToken> FindTokens(JToken token)
    {
      if (this.Conditions.Any<KeyValuePair<string, HashSet<object>>>() && (token.Type != JTokenType.Object ? 0 : (this.Conditions.Any<KeyValuePair<string, HashSet<object>>>((Func<KeyValuePair<string, HashSet<object>>, bool>) (condition => token[(object) condition.Key] is JValue jvalue && condition.Value.Contains(jvalue.Value))) ? 1 : 0)) == 0)
        return Enumerable.Empty<JToken>();
      if (this.Paths.Any<KeyValuePair<string, JPath>>())
      {
        IEnumerable<JToken> first = Enumerable.Empty<JToken>();
        switch (token.Type)
        {
          case JTokenType.Object:
            using (Dictionary<string, JPath>.Enumerator enumerator = this.Paths.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                KeyValuePair<string, JPath> current = enumerator.Current;
                JToken token1 = token[(object) current.Key];
                if (token1 != null)
                  first = first.Concat<JToken>(current.Value.FindTokens(token1));
              }
              break;
            }
          case JTokenType.Array:
            JPath jpath;
            if (this.Paths.TryGetValue("[]", out jpath))
            {
              using (IEnumerator<JToken> enumerator = ((IEnumerable<JToken>) token).GetEnumerator())
              {
                while (enumerator.MoveNext())
                {
                  JToken current = enumerator.Current;
                  first = first.Concat<JToken>(jpath.FindTokens(current));
                }
                break;
              }
            }
            else
              break;
        }
        return first;
      }
      return token.Parent.Type == JTokenType.Property ? (IEnumerable<JToken>) new JContainer[1]
      {
        token.Parent
      } : (IEnumerable<JToken>) new JToken[1]{ token };
    }
  }
}
