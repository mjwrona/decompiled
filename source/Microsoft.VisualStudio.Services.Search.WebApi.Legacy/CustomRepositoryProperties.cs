// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebApi.CustomRepositoryProperties
// Assembly: Microsoft.VisualStudio.Services.Search.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5D4CB2D3-3C08-46C7-B9C5-51E638F57F9E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.WebApi.Legacy.dll

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.WebApi
{
  [DataContract]
  [KnownType(typeof (SDRepositoryProperties))]
  [JsonConverter(typeof (CustomRepositoryPropertiesJsonConverter))]
  public abstract class CustomRepositoryProperties : IExtensibleDataObject
  {
    private ExtensionDataObject m_extensionDataValue;

    protected CustomRepositoryProperties(CustomRepositoryPropertiesType propertiesType) => this.PropertiesType = propertiesType;

    [DataMember(Name = "propertiesType")]
    public CustomRepositoryPropertiesType PropertiesType { get; private set; }

    public ExtensionDataObject ExtensionData
    {
      get => this.m_extensionDataValue;
      set => this.m_extensionDataValue = value;
    }

    public virtual string ToString(int indentLevel) => string.Empty;

    public override string ToString() => this.ToString(0);
  }
}
