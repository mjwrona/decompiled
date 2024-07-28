// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Jira.JiraAuthentication
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using System;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Jira
{
  public class JiraAuthentication
  {
    public JiraAuthentication(Uri jiraBaseUrl, string clientKey, string sharedSecret)
    {
      this.Scheme = JiraAuthScheme.JiraConnectApp;
      this.JiraAppKey = "com.azure.devops.integration.jira";
      this.SharedSecret = sharedSecret;
      this.JiraBaseUrl = jiraBaseUrl;
      this.ClientKey = clientKey;
    }

    public JiraAuthScheme Scheme { get; }

    public string JiraAppKey { get; }

    public string SharedSecret { get; }

    public Uri JiraBaseUrl { get; }

    public string ClientKey { get; }
  }
}
