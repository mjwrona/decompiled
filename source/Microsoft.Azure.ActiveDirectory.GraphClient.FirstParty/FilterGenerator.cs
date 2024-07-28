// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.FilterGenerator
// Assembly: Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7AAC33C3-FEBB-470D-AB81-20ABE7F3618F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty.dll

using System.Collections.Generic;
using System.Linq.Expressions;

namespace Microsoft.Azure.ActiveDirectory.GraphClient
{
  public class FilterGenerator
  {
    public FilterGenerator()
    {
      this.QueryArguments = (IDictionary<string, string>) new Dictionary<string, string>();
      this.ExpandProperty = LinkProperty.None;
      this.OrderByProperty = GraphProperty.None;
    }

    public int Top
    {
      get
      {
        int result = -1;
        string s;
        if (this.QueryArguments.TryGetValue("$top", out s))
          int.TryParse(s, out result);
        return result;
      }
      set
      {
        if (value > 0)
        {
          this.QueryArguments["$top"] = value.ToString();
        }
        else
        {
          if (!this.QueryArguments.ContainsKey("$top"))
            return;
          this.QueryArguments.Remove("$top");
        }
      }
    }

    public Expression QueryFilter { get; set; }

    public string OverrideQueryFilter { get; set; }

    public LinkProperty ExpandProperty { get; set; }

    public GraphProperty OrderByProperty { get; set; }

    public string this[string name]
    {
      get => this.QueryArguments[name];
      set => this.QueryArguments[name] = value;
    }

    internal IEnumerable<string> Names => (IEnumerable<string>) this.QueryArguments.Keys;

    private IDictionary<string, string> QueryArguments { get; set; }
  }
}
