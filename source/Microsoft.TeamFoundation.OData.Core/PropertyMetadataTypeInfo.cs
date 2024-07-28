// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.PropertyMetadataTypeInfo
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
using System.Collections.Generic;

namespace Microsoft.OData
{
  internal class PropertyMetadataTypeInfo
  {
    private bool derivedTypeConstraintsLoaded;
    private IEnumerable<string> derivedTypeConstraints;
    private IEdmModel model;

    public PropertyMetadataTypeInfo(IEdmModel model, string name, IEdmStructuredType owningType)
    {
      this.PropertyName = name;
      this.OwningType = owningType;
      this.EdmProperty = owningType == null ? (IEdmProperty) null : owningType.FindProperty(name);
      this.IsUndeclaredProperty = this.EdmProperty == null;
      this.IsOpenProperty = owningType != null && owningType.IsOpen && this.IsUndeclaredProperty;
      this.TypeReference = this.IsUndeclaredProperty ? (IEdmTypeReference) null : this.EdmProperty.Type;
      this.FullName = this.TypeReference == null ? (string) null : this.TypeReference.Definition.AsActualType().FullTypeName();
      this.model = model;
      this.derivedTypeConstraintsLoaded = false;
    }

    public string PropertyName { get; private set; }

    public IEdmProperty EdmProperty { get; private set; }

    public bool IsOpenProperty { get; private set; }

    public bool IsUndeclaredProperty { get; private set; }

    public IEdmStructuredType OwningType { get; private set; }

    public IEdmTypeReference TypeReference { get; private set; }

    public string FullName { get; private set; }

    public IEnumerable<string> DerivedTypeConstraints
    {
      get
      {
        if (!this.derivedTypeConstraintsLoaded)
        {
          this.derivedTypeConstraints = this.EdmProperty == null ? (IEnumerable<string>) null : this.model.GetDerivedTypeConstraints((IEdmVocabularyAnnotatable) this.EdmProperty);
          this.derivedTypeConstraintsLoaded = true;
        }
        return this.derivedTypeConstraints;
      }
    }
  }
}
