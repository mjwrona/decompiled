// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.IndexSetting
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch
{
  public class IndexSetting
  {
    public string IndexCreatedVersion { get; }

    public int NumPrimaries { get; }

    public IndexSetting(string indexCreatedVersion, int numPrimaries)
    {
      this.IndexCreatedVersion = indexCreatedVersion;
      this.NumPrimaries = numPrimaries;
    }
  }
}
