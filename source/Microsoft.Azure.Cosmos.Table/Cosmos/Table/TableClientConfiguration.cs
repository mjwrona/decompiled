// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.TableClientConfiguration
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

namespace Microsoft.Azure.Cosmos.Table
{
  public class TableClientConfiguration
  {
    public TableClientConfiguration()
    {
      this.CosmosExecutorConfiguration = new CosmosExecutorConfiguration();
      this.RestExecutorConfiguration = new RestExecutorConfiguration();
    }

    public CosmosExecutorConfiguration CosmosExecutorConfiguration { get; set; }

    public RestExecutorConfiguration RestExecutorConfiguration { get; set; }

    public bool UseRestExecutorForCosmosEndpoint { get; set; }
  }
}
