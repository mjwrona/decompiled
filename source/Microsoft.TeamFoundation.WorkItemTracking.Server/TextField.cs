// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.TextField
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class TextField
  {
    private string nameField;
    private string refereneceNameField;
    private string valueField;
    private int maxValueLength;

    public TextField(int maxValueLength) => this.maxValueLength = maxValueLength;

    public TextField() => this.maxValueLength = int.MaxValue;

    public string Name
    {
      get => this.nameField;
      set => this.nameField = value;
    }

    public string ReferenceName
    {
      get => this.refereneceNameField;
      set => this.refereneceNameField = value;
    }

    [XmlIgnore]
    public string Value
    {
      get => this.valueField;
      set => this.valueField = value;
    }

    [XmlElement(ElementName = "Value")]
    public string TruncatedValue
    {
      get
      {
        if (this.Value == null)
          return (string) null;
        return this.Value.Length <= this.maxValueLength ? this.Value : this.Value.Substring(0, this.maxValueLength);
      }
      set => this.valueField = value;
    }

    public InternalFieldType FieldType { get; set; }
  }
}
