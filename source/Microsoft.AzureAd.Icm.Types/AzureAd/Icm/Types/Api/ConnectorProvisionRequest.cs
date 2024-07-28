// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.Api.ConnectorProvisionRequest
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

namespace Microsoft.AzureAd.Icm.Types.Api
{
  public class ConnectorProvisionRequest
  {
    public long TenantId { get; set; }

    public string ConnectorName { get; set; }

    public long AlertSourceTypeId { get; set; }

    public long AlertSourceId { get; set; }

    public long ConnectorOwningTeamId { get; set; }

    public long? DefaultSiloId { get; set; }
  }
}
