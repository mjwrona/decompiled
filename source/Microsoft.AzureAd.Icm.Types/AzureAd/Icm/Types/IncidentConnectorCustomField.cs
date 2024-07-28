// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.IncidentConnectorCustomField
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.AzureAd.Icm.Types
{
  [DataContract]
  public class IncidentConnectorCustomField
  {
    [DataMember]
    public IncidentCustomFieldContainerType ContainerType { get; set; }

    [DataMember]
    public Guid? ContainerPublicId { get; set; }

    [DataMember]
    public IDictionary<string, string> FieldSet { get; set; }

    public static void ThrowIfNotValid(IncidentConnectorCustomField entry)
    {
      if (entry.FieldSet == null)
        return;
      foreach (KeyValuePair<string, string> field in (IEnumerable<KeyValuePair<string, string>>) entry.FieldSet)
      {
        TypeUtility.ValidateStringParameter(field.Key, string.Format("entry.FieldSet[{0}]", (object) field.Key), 1, 10000, false);
        TypeUtility.ValidateStringParameter(field.Value, string.Format("entry.FieldSet[{0}]={1}", (object) field.Key, (object) field.Value), 1, 10000, true);
      }
    }
  }
}
