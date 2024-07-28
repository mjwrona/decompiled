// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.Incidents.ActivityAttachment
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System.IO;

namespace Microsoft.AzureAd.Icm.Types.Incidents
{
  public class ActivityAttachment
  {
    public ActivityAttachment(long incidentId, string type, Stream contents)
    {
      this.IncidentId = incidentId;
      this.Type = type;
      this.Contents = contents;
    }

    public long IncidentId { get; private set; }

    public string Type { get; private set; }

    public Stream Contents { get; private set; }
  }
}
