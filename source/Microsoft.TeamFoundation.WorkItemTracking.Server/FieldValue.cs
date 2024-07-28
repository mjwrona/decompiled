// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.FieldValue
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class FieldValue
  {
    private bool isMultiValueField;
    private string[] values;

    public FieldValue()
    {
    }

    public FieldValue(string fieldName, object value, bool isMultiValueField = false)
    {
      this.Name = fieldName;
      this.isMultiValueField = isMultiValueField;
      if (isMultiValueField && value is string[])
        this.Values = value as string[];
      else
        this.Value = value?.ToString();
    }

    [XmlAttribute]
    public string Name { get; set; }

    [XmlAttribute]
    public bool IsIdentity { get; set; }

    [XmlText]
    public string Value { get; set; }

    [XmlElement(ElementName = "Value")]
    public string[] Values
    {
      get
      {
        if (this.values != null || !this.isMultiValueField)
          return this.values;
        return new string[1]{ string.Empty };
      }
      set => this.values = value;
    }
  }
}
