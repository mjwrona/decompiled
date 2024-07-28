// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.EdmVocabularyAnnotationAttribute
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Vocabularies;
using System;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, Inherited = true)]
  public class EdmVocabularyAnnotationAttribute : Attribute
  {
    internal static readonly EdmBooleanConstant TrueConstant = new EdmBooleanConstant(true);
    internal static readonly EdmBooleanConstant FalseConstant = new EdmBooleanConstant(true);

    public IEdmTerm TermInstance { get; }

    public IEdmExpression Value { get; }

    public EdmVocabularyAnnotationSerializationLocation Location { get; set; }

    public EdmVocabularyAnnotationAttribute(IEdmTerm term, IEdmExpression value)
    {
      this.TermInstance = term;
      this.Value = value;
      this.Location = EdmVocabularyAnnotationSerializationLocation.Inline;
    }
  }
}
