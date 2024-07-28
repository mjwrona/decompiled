// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.ODataUriParserSettings
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

namespace Microsoft.OData.UriParser
{
  public sealed class ODataUriParserSettings
  {
    internal const int DefaultFilterLimit = 800;
    internal const int DefaultOrderByLimit = 800;
    internal const int DefaultSelectExpandLimit = 800;
    internal const int DefaultPathLimit = 100;
    internal const int DefaultSearchLimit = 100;
    private int filterLimit;
    private int orderByLimit;
    private int pathLimit;
    private int selectExpandLimit;
    private int maxExpandDepth;
    private int maxExpandCount;
    private int searchLimit;

    public ODataUriParserSettings()
    {
      this.FilterLimit = 800;
      this.OrderByLimit = 800;
      this.PathLimit = 100;
      this.SelectExpandLimit = 800;
      this.SearchLimit = 100;
      this.MaximumExpansionDepth = int.MaxValue;
      this.MaximumExpansionCount = int.MaxValue;
    }

    public int MaximumExpansionDepth
    {
      get => this.maxExpandDepth;
      set => this.maxExpandDepth = value >= 0 ? value : throw new ODataException(Strings.UriParser_NegativeLimit);
    }

    public int MaximumExpansionCount
    {
      get => this.maxExpandCount;
      set => this.maxExpandCount = value >= 0 ? value : throw new ODataException(Strings.UriParser_NegativeLimit);
    }

    internal int SelectExpandLimit
    {
      get => this.selectExpandLimit;
      set => this.selectExpandLimit = value >= 0 ? value : throw new ODataException(Strings.UriParser_NegativeLimit);
    }

    internal int FilterLimit
    {
      get => this.filterLimit;
      set => this.filterLimit = value >= 0 ? value : throw new ODataException(Strings.UriParser_NegativeLimit);
    }

    internal int OrderByLimit
    {
      get => this.orderByLimit;
      set => this.orderByLimit = value >= 0 ? value : throw new ODataException(Strings.UriParser_NegativeLimit);
    }

    internal int PathLimit
    {
      get => this.pathLimit;
      set => this.pathLimit = value >= 0 ? value : throw new ODataException(Strings.UriParser_NegativeLimit);
    }

    internal int SearchLimit
    {
      get => this.searchLimit;
      set => this.searchLimit = value >= 0 ? value : throw new ODataException(Strings.UriParser_NegativeLimit);
    }
  }
}
