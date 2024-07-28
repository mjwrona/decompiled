// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.AccessControlEntryExtendedData
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public sealed class AccessControlEntryExtendedData
  {
    public AccessControlEntryExtendedData()
    {
    }

    internal AccessControlEntryExtendedData(AceExtendedInformation extendedInfo)
    {
      this.InheritedAllow = extendedInfo.InheritedAllow;
      this.InheritedDeny = extendedInfo.InheritedDeny;
      this.EffectiveAllow = extendedInfo.EffectiveAllow;
      this.EffectiveDeny = extendedInfo.EffectiveDeny;
    }

    [XmlAttribute]
    public int InheritedAllow { get; set; }

    [XmlAttribute]
    public int InheritedDeny { get; set; }

    [XmlAttribute]
    public int EffectiveAllow { get; set; }

    [XmlAttribute]
    public int EffectiveDeny { get; set; }
  }
}
