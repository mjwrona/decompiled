// Decompiled with JetBrains decompiler
// Type: Nest.RemoteClusterConfiguration
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  public class RemoteClusterConfiguration : IsADictionaryBase<string, object>
  {
    private readonly Dictionary<string, object> _remoteDictionary = new Dictionary<string, object>();

    public RemoteClusterConfiguration() => this.BackingDictionary["cluster"] = (object) new Dictionary<string, object>()
    {
      {
        "remote",
        (object) this._remoteDictionary
      }
    };

    public void Add(string name, params Uri[] seeds) => this.Add(name, ((IEnumerable<Uri>) seeds).Select<Uri, string>((Func<Uri, string>) (u => string.Format("{0}:{1}", (object) u.Host, (object) u.Port))).ToArray<string>());

    public void Add(string name, params string[] seeds) => this.Add(name, new Dictionary<string, object>()
    {
      {
        nameof (seeds),
        (object) seeds
      }
    });

    public void Add(string name, Dictionary<string, object> remoteSettings) => this._remoteDictionary[name] = (object) remoteSettings;

    public static Dictionary<string, object> operator +(
      RemoteClusterConfiguration left,
      IDictionary<string, object> right)
    {
      return RemoteClusterConfiguration.Combine((IDictionary<string, object>) left, right);
    }

    public static Dictionary<string, object> operator +(
      IDictionary<string, object> left,
      RemoteClusterConfiguration right)
    {
      return RemoteClusterConfiguration.Combine(left, (IDictionary<string, object>) right);
    }

    private static Dictionary<string, object> Combine(
      IDictionary<string, object> left,
      IDictionary<string, object> right)
    {
      if (left == null && right == null)
        return (Dictionary<string, object>) null;
      if (left == null)
        return new Dictionary<string, object>(right);
      if (right == null)
        return new Dictionary<string, object>(left);
      foreach (KeyValuePair<string, object> keyValuePair in (IEnumerable<KeyValuePair<string, object>>) right)
        left[keyValuePair.Key] = keyValuePair.Value;
      return new Dictionary<string, object>(left);
    }
  }
}
