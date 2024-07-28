// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.IncidentCustomField
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;

namespace Microsoft.AzureAd.Icm.Types
{
  public class IncidentCustomField
  {
    public string IdForConnectors => string.Format("{0}-{1}-{2}", (object) this.ContainerType, (object) this.ContainerPublicId, (object) this.Name);

    public long InternalId { get; set; }

    public string Name { get; set; }

    public long? ContainerId { get; set; }

    public IncidentCustomFieldContainerType ContainerType { get; set; }

    public Guid ContainerPublicId { get; set; }

    public string DisplayName { get; set; }

    public string Description { get; set; }

    public long? IntegerValue { get; set; }

    public DateTime? DateTimeValue { get; set; }

    public string StringValue { get; set; }

    public string EnumValue { get; set; }

    public bool? BooleanValue { get; set; }

    public string Value { get; set; }

    public IncidentCustomFieldType Type { get; set; }

    public string GroupName { get; set; }

    public bool IsModified { get; set; }

    public bool IsMandatoryForIncident { get; set; }

    public bool IsMandatoryForPir { get; set; }

    public bool IsDisabled { get; set; }
  }
}
