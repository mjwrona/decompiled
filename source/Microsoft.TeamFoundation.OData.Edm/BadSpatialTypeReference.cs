// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.BadSpatialTypeReference
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Validation;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm
{
  internal class BadSpatialTypeReference : EdmSpatialTypeReference, IEdmCheckable
  {
    private readonly IEnumerable<EdmError> errors;

    public BadSpatialTypeReference(
      string qualifiedName,
      bool isNullable,
      IEnumerable<EdmError> errors)
      : base((IEdmPrimitiveType) new BadPrimitiveType(qualifiedName, EdmPrimitiveTypeKind.None, errors), isNullable, new int?())
    {
      this.errors = errors;
    }

    public IEnumerable<EdmError> Errors => this.errors;

    public override string ToString()
    {
      EdmError edmError = this.Errors.FirstOrDefault<EdmError>();
      return (edmError != null ? edmError.ErrorCode.ToString() + ":" : "") + this.ToTraceString();
    }
  }
}
