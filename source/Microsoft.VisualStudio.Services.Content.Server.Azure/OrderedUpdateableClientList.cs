// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.OrderedUpdateableClientList
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Cosmos.Table;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  internal class OrderedUpdateableClientList
  {
    private readonly List<ITableClient> clients;
    private readonly Dictionary<string, int> connectionStringToClientIndexMapping = new Dictionary<string, int>();

    public OrderedUpdateableClientList(
      IEnumerable<string> connectionStrings,
      LocationMode? locationMode,
      IRetryPolicy overrideRetryPolicy)
    {
      List<string> list = connectionStrings.ToList<string>();
      this.clients = new List<ITableClient>(list.Count<string>());
      for (int index = 0; index < list.Count<string>(); ++index)
      {
        this.clients.Add((ITableClient) new AzureCloudTableClientAdapter(list[index], locationMode, overrideRetryPolicy));
        this.connectionStringToClientIndexMapping[list[index]] = index;
      }
    }

    public List<ITableClient> Clients => new List<ITableClient>((IEnumerable<ITableClient>) this.clients);

    public int Count => this.clients.Count;

    public void UpdateLocationMode(LocationMode? newLocationMode)
    {
      foreach (ITableClient client in this.Clients)
        client.LocationMode = newLocationMode;
    }

    public void UpdateConnectionString(string oldConnectionString, string newConnectionString)
    {
      if (!this.connectionStringToClientIndexMapping.Keys.Contains<string>(oldConnectionString))
        return;
      ITableClient client = this.clients[this.connectionStringToClientIndexMapping[oldConnectionString]];
      AzureCloudTableClientAdapter tableClientAdapter = new AzureCloudTableClientAdapter(newConnectionString, client.LocationMode, (IRetryPolicy) null);
      this.clients[this.connectionStringToClientIndexMapping[oldConnectionString]] = (ITableClient) tableClientAdapter;
      this.connectionStringToClientIndexMapping[newConnectionString] = this.connectionStringToClientIndexMapping[oldConnectionString];
      this.connectionStringToClientIndexMapping.Remove(oldConnectionString);
    }
  }
}
