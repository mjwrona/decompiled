// Decompiled with JetBrains decompiler
// Type: Nest.StoredScript
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  public class StoredScript : IStoredScript
  {
    internal StoredScript()
    {
    }

    protected StoredScript(string lang, string source)
    {
      IStoredScript storedScript = (IStoredScript) this;
      storedScript.Lang = lang;
      storedScript.Source = source;
    }

    [DataMember(Name = "lang")]
    string IStoredScript.Lang { get; set; }

    [DataMember(Name = "source")]
    string IStoredScript.Source { get; set; }
  }
}
