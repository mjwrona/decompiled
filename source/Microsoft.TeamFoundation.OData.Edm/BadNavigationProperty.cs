// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.BadNavigationProperty
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm
{
  internal class BadNavigationProperty : 
    BadElement,
    IEdmNavigationProperty,
    IEdmProperty,
    IEdmNamedElement,
    IEdmElement,
    IEdmVocabularyAnnotatable
  {
    private readonly string name;
    private readonly IEdmStructuredType declaringType;
    private readonly Cache<BadNavigationProperty, IEdmTypeReference> type = new Cache<BadNavigationProperty, IEdmTypeReference>();
    private static readonly Func<BadNavigationProperty, IEdmTypeReference> ComputeTypeFunc = (Func<BadNavigationProperty, IEdmTypeReference>) (me => me.ComputeType());

    public BadNavigationProperty(
      IEdmStructuredType declaringType,
      string name,
      IEnumerable<EdmError> errors)
      : base(errors)
    {
      this.name = name ?? string.Empty;
      this.declaringType = declaringType;
    }

    public string Name => this.name;

    public IEdmStructuredType DeclaringType => this.declaringType;

    public IEdmTypeReference Type => this.type.GetValue(this, BadNavigationProperty.ComputeTypeFunc, (Func<BadNavigationProperty, IEdmTypeReference>) null);

    public EdmPropertyKind PropertyKind => EdmPropertyKind.None;

    public IEdmNavigationProperty Partner => (IEdmNavigationProperty) null;

    public EdmOnDeleteAction OnDelete => EdmOnDeleteAction.None;

    public IEdmReferentialConstraint ReferentialConstraint => (IEdmReferentialConstraint) null;

    public bool ContainsTarget => false;

    public override string ToString() => this.Errors.FirstOrDefault<EdmError>().ErrorCode.ToString() + ":" + this.ToTraceString();

    private IEdmTypeReference ComputeType() => (IEdmTypeReference) new BadTypeReference(new BadType(this.Errors), true);
  }
}
