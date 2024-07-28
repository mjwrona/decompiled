// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.IncidentCustomFieldsGroup
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AzureAd.Icm.Types
{
  [Serializable]
  public class IncidentCustomFieldsGroup
  {
    public IncidentCustomFieldsGroup() => this.CustomFields = (IList<IncidentCustomField>) new List<IncidentCustomField>();

    public string Name { get; set; }

    public Guid ContainerPublicId { get; set; }

    public long? ContainerId { get; set; }

    public IList<IncidentCustomField> CustomFields { get; set; }

    public IList<IncidentCustomField> EnabledCustomFields => this.CustomFields == null || this.CustomFields.Count == 0 ? this.CustomFields : (IList<IncidentCustomField>) this.CustomFields.Where<IncidentCustomField>((Func<IncidentCustomField, bool>) (a => !a.IsDisabled)).ToList<IncidentCustomField>();

    public IncidentCustomFieldContainerType ContainerType { get; set; }
  }
}
