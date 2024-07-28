// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.BatchSupportAnnotationAttribute
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  internal class BatchSupportAnnotationAttribute : EdmVocabularyAnnotationAttribute
  {
    private static readonly IEdmPropertyConstructor[] DefaultSettings = new IEdmPropertyConstructor[5]
    {
      (IEdmPropertyConstructor) new EdmPropertyConstructor("Supported", (IEdmExpression) new EdmBooleanConstant(true)),
      (IEdmPropertyConstructor) new EdmPropertyConstructor("ContinueOnErrorSupported", (IEdmExpression) new EdmBooleanConstant(false)),
      (IEdmPropertyConstructor) new EdmPropertyConstructor("ReferencesInRequestBodiesSupported", (IEdmExpression) new EdmBooleanConstant(false)),
      (IEdmPropertyConstructor) new EdmPropertyConstructor("ReferencesAcrossChangeSetsSupported", (IEdmExpression) new EdmBooleanConstant(false)),
      (IEdmPropertyConstructor) new EdmPropertyConstructor("EtagReferencesSupported", (IEdmExpression) new EdmBooleanConstant(false))
    };
    private static readonly IEdmTerm BatchSupportTerm = (IEdmTerm) new EdmTerm("Org.OData.Capabilities.V1", "BatchSupportType", (IEdmTypeReference) new EdmComplexTypeReference((IEdmComplexType) new EdmComplexType("Capabilities", "BatchSupport"), false));
    public static readonly BatchSupportAnnotationAttribute Default = new BatchSupportAnnotationAttribute();

    public BatchSupportAnnotationAttribute()
      : base(BatchSupportAnnotationAttribute.BatchSupportTerm, (IEdmExpression) new EdmRecordExpression(BatchSupportAnnotationAttribute.DefaultSettings))
    {
    }
  }
}
