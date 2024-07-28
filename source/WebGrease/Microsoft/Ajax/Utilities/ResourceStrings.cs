// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.ResourceStrings
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System.Collections.Generic;

namespace Microsoft.Ajax.Utilities
{
  public class ResourceStrings
  {
    public string Name { get; set; }

    public IDictionary<string, string> NameValuePairs { get; private set; }

    public string this[string name]
    {
      get
      {
        string str;
        if (!this.NameValuePairs.TryGetValue(name, out str))
          str = (string) null;
        return str;
      }
      set => this.NameValuePairs[name] = value;
    }

    public int Count => this.NameValuePairs.Count;

    public ResourceStrings() => this.NameValuePairs = (IDictionary<string, string>) new Dictionary<string, string>();
  }
}
