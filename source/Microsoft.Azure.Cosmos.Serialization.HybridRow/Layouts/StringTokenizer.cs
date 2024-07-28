// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts.StringTokenizer
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Microsoft.Azure.Cosmos.Core;
using Microsoft.Azure.Cosmos.Core.Utf8;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts
{
  public sealed class StringTokenizer
  {
    private readonly Dictionary<Utf8String, StringToken> tokens;
    private readonly Dictionary<string, StringToken> stringTokens;
    private readonly List<Utf8String> strings;

    public StringTokenizer()
    {
      this.tokens = new Dictionary<Utf8String, StringToken>((IEqualityComparer<Utf8String>) SamplingUtf8StringComparer.Default)
      {
        {
          Utf8String.Empty,
          new StringToken(0UL, Utf8String.Empty)
        }
      };
      this.stringTokens = new Dictionary<string, StringToken>((IEqualityComparer<string>) SamplingStringComparer.Default)
      {
        {
          string.Empty,
          new StringToken(0UL, Utf8String.Empty)
        }
      };
      this.strings = new List<Utf8String>()
      {
        Utf8String.Empty
      };
      this.Count = 1;
    }

    public int Count { get; private set; }

    public bool TryFindToken(UtfAnyString path, out StringToken token)
    {
      if (((UtfAnyString) ref path).IsNull)
      {
        token = new StringToken();
        return false;
      }
      return ((UtfAnyString) ref path).IsUtf8 ? this.tokens.TryGetValue(((UtfAnyString) ref path).ToUtf8String(), out token) : this.stringTokens.TryGetValue(path.ToString(), out token);
    }

    public bool TryFindString(ulong token, out Utf8String path)
    {
      if (token >= checked ((ulong) this.strings.Count))
      {
        path = (Utf8String) null;
        return false;
      }
      path = this.strings[checked ((int) token)];
      return true;
    }

    internal StringToken Add(Utf8String path)
    {
      Contract.Requires(Utf8String.op_Inequality(path, (Utf8String) null));
      StringToken stringToken;
      return this.tokens.TryGetValue(path, out stringToken) ? stringToken : this.AllocateToken(path);
    }

    private StringToken AllocateToken(Utf8String path)
    {
      StringToken stringToken = new StringToken(checked ((ulong) this.Count++), path);
      this.tokens.Add(path, stringToken);
      this.stringTokens.Add(path.ToString(), stringToken);
      this.strings.Add(path);
      return stringToken;
    }
  }
}
