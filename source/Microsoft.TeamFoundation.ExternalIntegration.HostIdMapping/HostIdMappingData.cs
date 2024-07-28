// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ExternalIntegration.HostIdMapping.HostIdMappingData
// Assembly: Microsoft.TeamFoundation.ExternalIntegration.HostIdMapping, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D6C9F672-10C9-4D5D-90D5-E07E56C1D4C0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.ExternalIntegration.HostIdMapping.dll

namespace Microsoft.TeamFoundation.ExternalIntegration.HostIdMapping
{
  public class HostIdMappingData
  {
    public string PropertyName { get; set; }

    public string Id { get; set; }

    public string Qualifier { get; set; }

    public override string ToString() => "PropertyName: " + this.PropertyName + ", Id: " + this.Id + ", Qualifier: " + this.Qualifier;
  }
}
