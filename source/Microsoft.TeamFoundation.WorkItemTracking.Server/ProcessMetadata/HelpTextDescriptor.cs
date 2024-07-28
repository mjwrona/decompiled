// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata.HelpTextDescriptor
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata
{
  [XmlType("help-text-descriptor")]
  public class HelpTextDescriptor
  {
    [XmlAttribute("field-reference-name")]
    public string FieldReferenceName { get; set; }

    [XmlAttribute("field-id")]
    [DefaultValue(0)]
    public int FieldId { get; set; }

    [XmlAttribute("help-text")]
    public string HelpText { get; set; }

    public HelpTextDescriptor Clone() => (HelpTextDescriptor) this.MemberwiseClone();
  }
}
