// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.Models.CertificateOperation
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.KeyVault.Models
{
  public class CertificateOperation
  {
    public CertificateOperationIdentifier CertificateOperationIdentifier => !string.IsNullOrWhiteSpace(this.Id) ? new CertificateOperationIdentifier(this.Id) : (CertificateOperationIdentifier) null;

    public CertificateOperation()
    {
    }

    public CertificateOperation(
      string id = null,
      IssuerParameters issuerParameters = null,
      byte[] csr = null,
      bool? cancellationRequested = null,
      string status = null,
      string statusDetails = null,
      Error error = null,
      string target = null,
      string requestId = null)
    {
      this.Id = id;
      this.IssuerParameters = issuerParameters;
      this.Csr = csr;
      this.CancellationRequested = cancellationRequested;
      this.Status = status;
      this.StatusDetails = statusDetails;
      this.Error = error;
      this.Target = target;
      this.RequestId = requestId;
    }

    [JsonProperty(PropertyName = "id")]
    public string Id { get; private set; }

    [JsonProperty(PropertyName = "issuer")]
    public IssuerParameters IssuerParameters { get; set; }

    [JsonProperty(PropertyName = "csr")]
    public byte[] Csr { get; set; }

    [JsonProperty(PropertyName = "cancellation_requested")]
    public bool? CancellationRequested { get; set; }

    [JsonProperty(PropertyName = "status")]
    public string Status { get; set; }

    [JsonProperty(PropertyName = "status_details")]
    public string StatusDetails { get; set; }

    [JsonProperty(PropertyName = "error")]
    public Error Error { get; set; }

    [JsonProperty(PropertyName = "target")]
    public string Target { get; set; }

    [JsonProperty(PropertyName = "request_id")]
    public string RequestId { get; set; }
  }
}
