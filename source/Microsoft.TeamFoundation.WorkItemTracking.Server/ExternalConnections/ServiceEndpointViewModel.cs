// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections.ServiceEndpointViewModel
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections
{
  [DataContract]
  public class ServiceEndpointViewModel
  {
    public ServiceEndpointViewModel(ServiceEndpoint serviceEndpoint)
    {
      ArgumentUtility.CheckForNull<ServiceEndpoint>(serviceEndpoint, nameof (serviceEndpoint));
      this.RawServiceEndpoint = serviceEndpoint;
    }

    [IgnoreDataMember]
    public ServiceEndpoint RawServiceEndpoint { get; set; }

    [DataMember]
    public Guid Id => this.RawServiceEndpoint.Id;

    [DataMember]
    public string Name => this.RawServiceEndpoint.Name;

    [DataMember]
    public string Type => this.RawServiceEndpoint.Type;

    [DataMember]
    public Uri Url => this.RawServiceEndpoint.Url;

    [DataMember]
    public string AuthorizationScheme => this.RawServiceEndpoint.Authorization?.Scheme;

    [DataMember]
    public IdentityRef CreatedBy => this.RawServiceEndpoint.CreatedBy;

    [DataMember]
    public string GitHubHandle
    {
      get
      {
        string gitHubHandle;
        this.RawServiceEndpoint.Data.TryGetValue(nameof (GitHubHandle), out gitHubHandle);
        return gitHubHandle;
      }
    }

    [DataMember]
    public bool IsEndpointValid
    {
      get
      {
        if (this.RawServiceEndpoint.Authorization?.Scheme != "InstallationToken")
          return true;
        string str;
        return this.RawServiceEndpoint.Data.TryGetValue("IsValid", out str) && str == "true";
      }
    }

    [DataMember]
    public int OrgIntId
    {
      get
      {
        string s;
        this.RawServiceEndpoint.Data.TryGetValue("orgIntId", out s);
        return int.Parse(s);
      }
    }
  }
}
