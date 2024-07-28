// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.PropertySerializationInfo
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;

namespace Microsoft.OData
{
  internal class PropertySerializationInfo
  {
    private bool isTopLevel;

    public PropertySerializationInfo(IEdmModel model, string name, IEdmStructuredType owningType)
    {
      this.PropertyName = name;
      this.IsTopLevel = false;
      this.MetadataType = new PropertyMetadataTypeInfo(model, name, owningType);
    }

    public string PropertyName { get; private set; }

    public PropertyValueTypeInfo ValueType { get; set; }

    public PropertyMetadataTypeInfo MetadataType { get; private set; }

    public string TypeNameToWrite { get; set; }

    public string WireName { get; private set; }

    public bool IsTopLevel
    {
      get => this.isTopLevel;
      set
      {
        this.isTopLevel = value;
        this.WireName = this.isTopLevel ? nameof (value) : this.PropertyName;
      }
    }
  }
}
