// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.GraphSettings
// Assembly: Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7AAC33C3-FEBB-470D-AB81-20ABE7F3618F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.ActiveDirectory.GraphClient
{
  public class GraphSettings
  {
    private bool isRetryEnabled = true;
    private HashSet<string> retryOnExceptions = new HashSet<string>()
    {
      "Microsoft.Azure.ActiveDirectory.GraphClient.ServiceUnavailableException",
      "Microsoft.Azure.ActiveDirectory.GraphClient.InternalServerErrorException"
    };
    private TimeSpan waitBeforeRetry = TimeSpan.FromSeconds(3.0);
    private int totalAttempts = 3;
    private string apiVersion = Constants.DefaultApiVersion;
    private string graphDomainName = "graph.windows.net";

    public bool IsRetryEnabled
    {
      get => this.isRetryEnabled;
      set => this.isRetryEnabled = value;
    }

    public HashSet<string> RetryOnExceptions
    {
      get => this.retryOnExceptions;
      set => this.retryOnExceptions = value;
    }

    public TimeSpan WaitBeforeRetry
    {
      get => this.waitBeforeRetry;
      set => this.waitBeforeRetry = value;
    }

    public int TotalAttempts
    {
      get => this.totalAttempts;
      set => this.totalAttempts = value;
    }

    public string ApiVersion
    {
      get => this.apiVersion;
      set => this.apiVersion = value;
    }

    public string GraphDomainName
    {
      get => this.graphDomainName;
      set => this.graphDomainName = value;
    }
  }
}
