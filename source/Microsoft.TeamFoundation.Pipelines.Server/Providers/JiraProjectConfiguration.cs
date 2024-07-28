// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.Providers.JiraProjectConfiguration
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Jira;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Pipelines.Server.Providers
{
  [DataContract]
  public class JiraProjectConfiguration : ISecuredObject
  {
    private string token;
    private int permissions;

    public JiraProjectConfiguration()
    {
    }

    public JiraProjectConfiguration(string token, int permissions)
    {
      this.token = token;
      this.permissions = permissions;
    }

    [DataMember]
    public Guid ProjectId { get; set; }

    [DataMember]
    public string ProjectName { get; set; }

    Guid ISecuredObject.NamespaceId => JiraConstants.Security.NamespaceId;

    int ISecuredObject.RequiredPermissions => this.permissions;

    string ISecuredObject.GetToken() => this.token;
  }
}
