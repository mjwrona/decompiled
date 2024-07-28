// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmBinaryTypeReference
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System;

namespace Microsoft.OData.Edm
{
  public class EdmBinaryTypeReference : 
    EdmPrimitiveTypeReference,
    IEdmBinaryTypeReference,
    IEdmPrimitiveTypeReference,
    IEdmTypeReference,
    IEdmElement
  {
    private readonly bool isUnbounded;
    private readonly int? maxLength;

    public EdmBinaryTypeReference(IEdmPrimitiveType definition, bool isNullable)
      : this(definition, isNullable, false, new int?())
    {
    }

    public EdmBinaryTypeReference(
      IEdmPrimitiveType definition,
      bool isNullable,
      bool isUnbounded,
      int? maxLength)
      : base(definition, isNullable)
    {
      if (isUnbounded && maxLength.HasValue)
        throw new InvalidOperationException(Strings.EdmModel_Validator_Semantic_IsUnboundedCannotBeTrueWhileMaxLengthIsNotNull);
      this.isUnbounded = isUnbounded;
      this.maxLength = maxLength;
    }

    public bool IsUnbounded => this.isUnbounded;

    public int? MaxLength => this.maxLength;
  }
}
