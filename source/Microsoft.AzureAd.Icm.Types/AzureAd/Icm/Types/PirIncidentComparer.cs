// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.PirIncidentComparer
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Collections.Generic;

namespace Microsoft.AzureAd.Icm.Types
{
  public class PirIncidentComparer : IEqualityComparer<PirIncident>
  {
    public bool Equals(PirIncident i1, PirIncident i2)
    {
      if (i1 == null || i2 == null)
        return false;
      long? nullable = i1.IncidentId;
      if (!nullable.HasValue)
      {
        nullable = i2.IncidentId;
        if (!nullable.HasValue)
        {
          long? sourceTypeId = i1.SourceTypeId;
          nullable = i2.SourceTypeId;
          return (sourceTypeId.GetValueOrDefault() == nullable.GetValueOrDefault() ? (sourceTypeId.HasValue == nullable.HasValue ? 1 : 0) : 0) != 0 && string.Compare(i1.SourceId, i2.SourceId, StringComparison.InvariantCultureIgnoreCase) == 0;
        }
      }
      nullable = i1.IncidentId;
      long? incidentId = i2.IncidentId;
      return nullable.GetValueOrDefault() == incidentId.GetValueOrDefault() && nullable.HasValue == incidentId.HasValue;
    }

    public int GetHashCode(PirIncident incident)
    {
      if (incident == null)
        return -1;
      long? nullable = incident.IncidentId;
      if (!nullable.HasValue)
      {
        int hashCode = string.IsNullOrWhiteSpace(incident.SourceId) ? 0 : incident.SourceId.GetHashCode();
        nullable = incident.SourceTypeId;
        int num;
        if (nullable.HasValue)
        {
          nullable = incident.SourceTypeId;
          num = nullable.Value.GetHashCode();
        }
        else
          num = 0;
        return hashCode ^ num;
      }
      nullable = incident.IncidentId;
      return nullable.Value.GetHashCode();
    }
  }
}
