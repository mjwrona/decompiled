// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Common.CodeElementIdentity
// Assembly: Microsoft.TeamFoundation.CodeSense.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 77D96756-A6EC-4CC5-958E-440F0412CE7F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Common.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.CodeSense.Common
{
  [Serializable]
  public sealed class CodeElementIdentity : 
    Dictionary<string, string>,
    IEquatable<CodeElementIdentity>
  {
    private const string Language = "Language";

    public CodeElementIdentity()
      : base((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
    }

    public CodeElementIdentity(string language)
      : this(language, (IDictionary<string, string>) new Dictionary<string, string>())
    {
    }

    public CodeElementIdentity(string language, IDictionary<string, string> dictionary)
      : base(dictionary, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      this[nameof (Language)] = language;
    }

    private CodeElementIdentity(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    private StringComparer ValueComparer
    {
      get
      {
        string str;
        return this.TryGetValue("Language", out str) && str == "Visual Basic" ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
      }
    }

    public bool Equals(IDictionary<string, string> other)
    {
      if (other == null || this.Count != other.Count)
        return false;
      StringComparer valueComparer = this.ValueComparer;
      foreach (string key in this.Keys)
      {
        string y = (string) null;
        if (!other.TryGetValue(key, out y) || !valueComparer.Equals(this[key], y))
          return false;
      }
      return true;
    }

    public override bool Equals(object obj) => this.Equals((IDictionary<string, string>) (obj as CodeElementIdentity));

    public override int GetHashCode()
    {
      string str;
      return !this.TryGetValue("Name", out str) ? 0 : this.ValueComparer.GetHashCode(str);
    }

    bool IEquatable<CodeElementIdentity>.Equals(CodeElementIdentity other) => this.Equals((IDictionary<string, string>) other);
  }
}
