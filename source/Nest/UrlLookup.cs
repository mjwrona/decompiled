// Decompiled with JetBrains decompiler
// Type: Nest.UrlLookup
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nest
{
  internal class UrlLookup
  {
    private readonly string[] _parts;
    private readonly string _route;
    private readonly string[] _tokenized;
    private readonly int _length;

    public UrlLookup(string route)
    {
      this._route = route;
      this._tokenized = route.Replace("{", "{@").Split(new char[2]
      {
        '{',
        '}'
      }, StringSplitOptions.RemoveEmptyEntries);
      this._parts = ((IEnumerable<string>) this._tokenized).Where<string>((Func<string, bool>) (p => p.StartsWith("@"))).Select<string, string>((Func<string, string>) (p => p.Remove(0, 1))).ToArray<string>();
      this._length = this._route.Length + this._parts.Length * 4;
    }

    public bool Matches(ResolvedRouteValues values)
    {
      for (int index = 0; index < this._parts.Length; ++index)
      {
        if (!values.ContainsKey(this._parts[index]))
          return false;
      }
      return true;
    }

    public string ToUrl(ResolvedRouteValues values)
    {
      if (values.Count == 0 && this._tokenized.Length == 1 && this._tokenized[0][0] != '@')
        return this._tokenized[0];
      StringBuilder stringBuilder = new StringBuilder(this._length);
      int index1 = 0;
      for (int index2 = 0; index2 < this._tokenized.Length; ++index2)
      {
        string str = this._tokenized[index2];
        if (str[0] == '@')
        {
          string stringToEscape;
          if (!values.TryGetValue(this._parts[index1], out stringToEscape))
            throw new Exception("No value provided for '" + this._parts[index1] + "' on url: " + this._route);
          if (string.IsNullOrEmpty(stringToEscape))
            throw new Exception("'" + this._parts[index1] + "' defined but is empty on url: " + this._route);
          stringBuilder.Append(Uri.EscapeDataString(stringToEscape));
          ++index1;
        }
        else
          stringBuilder.Append(str);
      }
      return stringBuilder.ToString();
    }
  }
}
