// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.WebApi.PropertyPair
// Assembly: Microsoft.VisualStudio.Services.Cloud.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52A8E326-8E84-4175-AE92-8ED7AF376B63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Cloud.WebApi
{
  [DataContract]
  public class PropertyPair
  {
    public PropertyPair()
    {
    }

    public PropertyPair(PropertyPair other)
    {
      this.Name = other.Name;
      this.Value = other.Value;
    }

    public PropertyPair(string name, string value)
    {
      this.Name = name;
      this.Value = value;
    }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string Value { get; set; }

    public override string ToString() => "Name=[" + this.Name + "], Value=[" + this.Value + "]";
  }
}
