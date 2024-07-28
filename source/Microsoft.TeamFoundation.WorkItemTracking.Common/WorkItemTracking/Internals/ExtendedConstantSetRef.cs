// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Internals.ExtendedConstantSetRef
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Internals
{
  [TypeConverter(typeof (ConstantSetReferenceTypeConverter))]
  [XmlType("extended-set-reference")]
  public class ExtendedConstantSetRef : ConstantSetReference
  {
    [XmlIgnore]
    public string EntityId { get; set; }

    [XmlIgnore]
    public bool IsGroup { get; set; }

    [XmlIgnore]
    public string DisplayName { get; set; }

    [XmlIgnore]
    public string DistinctDisplayName { get; set; }
  }
}
