// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.Clause
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System.Runtime.Serialization;

namespace Microsoft.AzureAd.Icm.Types
{
  [DataContract]
  public class Clause : ClauseBase
  {
    public Clause() => this.VisitorTimeZoneId = "Pacific Standard Time";

    [DataMember(Name = "ao")]
    public int LogicalOperator { get; set; }

    [DataMember(Name = "fid")]
    public int FieldId { get; set; }

    [DataMember(Name = "op")]
    public int Operator { get; set; }

    [DataMember(Name = "val")]
    public object Value { get; set; }

    [DataMember(Name = "visitorTimeZone")]
    public string VisitorTimeZoneId { get; set; }
  }
}
