// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ServiceIdentity
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;

namespace Microsoft.Azure.Documents
{
  internal sealed class ServiceIdentity : IServiceIdentity
  {
    private ServiceIdentity()
    {
    }

    public ServiceIdentity(string federationId, Uri serviceName, bool isMasterService)
    {
      this.FederationId = federationId;
      this.ServiceName = serviceName;
      this.IsMasterService = isMasterService;
    }

    public string FederationId { get; private set; }

    public Uri ServiceName { get; private set; }

    public bool IsMasterService { get; private set; }

    public string ApplicationName => this.ServiceName == (Uri) null ? string.Empty : this.ServiceName.AbsoluteUri.Substring(0, this.ServiceName.AbsoluteUri.LastIndexOf('/'));

    public string GetFederationId() => this.FederationId;

    public Uri GetServiceUri() => this.ServiceName;

    public long GetPartitionKey() => 0;

    public override bool Equals(object obj) => obj is ServiceIdentity serviceIdentity && string.Compare(this.FederationId, serviceIdentity.FederationId, StringComparison.OrdinalIgnoreCase) == 0 && Uri.Compare(this.ServiceName, serviceIdentity.ServiceName, UriComponents.AbsoluteUri, UriFormat.UriEscaped, StringComparison.OrdinalIgnoreCase) == 0;

    public override int GetHashCode() => (this.FederationId == null ? 0 : this.FederationId.GetHashCode()) ^ (this.ServiceName == (Uri) null ? 0 : this.ServiceName.GetHashCode());

    public override string ToString() => string.Format("FederationId:{0},ServiceName:{1},IsMasterService:{2}", (object) this.FederationId, (object) this.ServiceName, (object) this.IsMasterService);
  }
}
