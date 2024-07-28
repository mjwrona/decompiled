// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Alias
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions
{
  public class Alias
  {
    public Alias(
      string indexIdentity,
      string aliasIdentity,
      string searchRouting,
      string indexRouting,
      IDictionary<string, List<string>> filters)
    {
      this.IndexIdentity = IndexIdentity.CreateIndexIdentity(indexIdentity);
      this.Identity = AliasIdentity.CreateAliasIdentity(aliasIdentity);
      this.SearchRouting = searchRouting;
      this.IndexRouting = indexRouting;
      this.Filters = filters;
    }

    public Alias()
    {
    }

    public AliasIdentity Identity { get; set; }

    public IndexIdentity IndexIdentity { get; set; }

    public string Routing { get; set; }

    public string SearchRouting { get; set; }

    public string IndexRouting { get; set; }

    public IDictionary<string, List<string>> Filters { get; set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Alias: {0} Index: {1} SearchRouting: {2} Filters: {3}", (object) this.Identity, (object) this.IndexIdentity, (object) (this.SearchRouting ?? string.Empty), this.Filters == null ? (object) string.Empty : (object) string.Join<KeyValuePair<string, List<string>>>(",", (IEnumerable<KeyValuePair<string, List<string>>>) this.Filters));
  }
}
