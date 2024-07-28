// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.Models.CertificateOperationUpdateParameter
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.KeyVault.Models
{
  public class CertificateOperationUpdateParameter
  {
    public CertificateOperationUpdateParameter()
    {
    }

    public CertificateOperationUpdateParameter(bool cancellationRequested) => this.CancellationRequested = cancellationRequested;

    [JsonProperty(PropertyName = "cancellation_requested")]
    public bool CancellationRequested { get; set; }

    public virtual void Validate()
    {
    }
  }
}
