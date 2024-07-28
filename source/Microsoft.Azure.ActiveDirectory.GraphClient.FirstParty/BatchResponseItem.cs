// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.BatchResponseItem
// Assembly: Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7AAC33C3-FEBB-470D-AB81-20ABE7F3618F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty.dll

using System.Net;

namespace Microsoft.Azure.ActiveDirectory.GraphClient
{
  public class BatchResponseItem
  {
    public BatchResponseItem() => this.BatchHeaders = new WebHeaderCollection();

    public bool Failed { get; set; }

    public PagedResults<GraphObject> ResultSet { get; set; }

    public GraphException Exception { get; set; }

    public WebHeaderCollection BatchHeaders { get; private set; }
  }
}
