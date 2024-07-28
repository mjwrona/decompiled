// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ReadManyRequestOptions
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

namespace Microsoft.Azure.Cosmos
{
  public class ReadManyRequestOptions : RequestOptions
  {
    public Microsoft.Azure.Cosmos.ConsistencyLevel? ConsistencyLevel
    {
      get => this.BaseConsistencyLevel;
      set => this.BaseConsistencyLevel = value;
    }

    public string SessionToken { get; set; }

    internal QueryRequestOptions ConvertToQueryRequestOptions()
    {
      QueryRequestOptions queryRequestOptions = new QueryRequestOptions();
      queryRequestOptions.ConsistencyLevel = this.ConsistencyLevel;
      queryRequestOptions.SessionToken = this.SessionToken;
      queryRequestOptions.IfMatchEtag = this.IfMatchEtag;
      queryRequestOptions.IfNoneMatchEtag = this.IfNoneMatchEtag;
      queryRequestOptions.Properties = this.Properties;
      queryRequestOptions.AddRequestHeaders = this.AddRequestHeaders;
      return queryRequestOptions;
    }
  }
}
