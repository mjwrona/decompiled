// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.BadStructuredType
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Validation;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm
{
  internal abstract class BadStructuredType : 
    BadType,
    IEdmStructuredType,
    IEdmType,
    IEdmElement,
    IEdmCheckable
  {
    protected BadStructuredType(IEnumerable<EdmError> errors)
      : base(errors)
    {
    }

    public IEdmStructuredType BaseType => (IEdmStructuredType) null;

    public IEnumerable<IEdmProperty> DeclaredProperties => Enumerable.Empty<IEdmProperty>();

    public bool IsAbstract => false;

    public bool IsOpen => false;

    public IEdmProperty FindProperty(string name) => (IEdmProperty) null;
  }
}
