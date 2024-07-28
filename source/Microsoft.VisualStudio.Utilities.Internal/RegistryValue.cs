// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Utilities.Internal.RegistryValue
// Assembly: Microsoft.VisualStudio.Utilities.Internal, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E02F1555-E063-4795-BC05-853CA7424F51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Utilities.Internal.dll

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Utilities.Internal
{
  [XmlType(TypeName = "value")]
  public sealed class RegistryValue
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name { get; set; }

    [XmlAttribute(AttributeName = "type")]
    public string Type { get; set; }

    [XmlText(typeof (string))]
    public string Text { get; set; }

    [XmlElement(ElementName = "string")]
    public string[] Strings { get; set; }

    [XmlIgnore]
    public object Value => this.GetObjectValue();

    public static RegistryValue FromValue(string name, object value)
    {
      RegistryValue registryValue = new RegistryValue()
      {
        Name = name
      };
      switch (value)
      {
        case string _:
          registryValue.Type = "string";
          registryValue.Text = (string) value;
          break;
        case int _:
          registryValue.Type = "int";
          registryValue.Text = value.ToString();
          break;
        case long _:
        case ulong _:
          registryValue.Type = "qword";
          registryValue.Text = value.ToString();
          break;
        case byte[] _:
          registryValue.Type = "bytearray";
          registryValue.Text = Convert.ToBase64String((byte[]) value);
          break;
        case string[] _:
          registryValue.Type = "string-array";
          string[] sourceArray = (string[]) value;
          registryValue.Strings = new string[sourceArray.Length];
          Array.Copy((Array) sourceArray, (Array) registryValue.Strings, sourceArray.Length);
          break;
        default:
          registryValue.Type = "string";
          registryValue.Text = value?.ToString() ?? string.Empty;
          break;
      }
      return registryValue;
    }

    private object GetObjectValue()
    {
      try
      {
        switch (this.Type)
        {
          case "int":
            return (object) int.Parse(this.Text);
          case "bytearray":
            return (object) Convert.FromBase64String(this.Text);
          case "string":
            return this.Text == null ? (object) string.Empty : (object) this.Text;
          case "qword":
            return (object) long.Parse(this.Text);
          case "string-array":
            return (object) new List<string>((IEnumerable<string>) this.Strings).ToArray();
          default:
            return (object) null;
        }
      }
      catch
      {
        switch (this.Type)
        {
          case "int":
            return (object) 0;
          case "bytearray":
            return (object) null;
          case "string":
            return (object) null;
          case "qword":
            return (object) 0L;
          case "string-array":
            return (object) null;
          default:
            return (object) null;
        }
      }
    }
  }
}
