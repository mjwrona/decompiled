// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.ApplySupportedAnnotationAttribute
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  [AttributeUsage(AttributeTargets.Class, Inherited = true)]
  public class ApplySupportedAnnotationAttribute : EdmVocabularyAnnotationAttribute
  {
    public static readonly string[] DefaultTransformations = new string[5]
    {
      "aggregate",
      "filter",
      "groupby",
      "compute",
      "expand"
    };
    private static readonly IEdmTerm ApplySupportedTerm = (IEdmTerm) new EdmTerm("Org.OData.Aggregation.V1", "ApplySupported", (IEdmTypeReference) new EdmComplexTypeReference((IEdmComplexType) new EdmComplexType("Aggregation", "ApplySupported"), false));
    public static readonly ApplySupportedAnnotationAttribute Default = new ApplySupportedAnnotationAttribute(ApplySupportedAnnotationAttribute.DefaultTransformations);

    public ApplySupportedAnnotationAttribute(string[] transformations)
      : base(ApplySupportedAnnotationAttribute.ApplySupportedTerm, (IEdmExpression) new EdmRecordExpression(new IEdmPropertyConstructor[2]
      {
        (IEdmPropertyConstructor) new EdmPropertyConstructor("Transformations", (IEdmExpression) new EdmCollectionExpression((IEnumerable<IEdmExpression>) ((IEnumerable<string>) transformations).Select<string, EdmStringConstant>((Func<string, EdmStringConstant>) (f => new EdmStringConstant(f))))),
        (IEdmPropertyConstructor) new EdmPropertyConstructor("CustomAggregationMethods ", (IEdmExpression) new EdmCollectionExpression(new IEdmExpression[5]
        {
          (IEdmExpression) new EdmStringConstant("ax.ApproxCountDistinct"),
          (IEdmExpression) new EdmStringConstant("ax.StandardDeviation"),
          (IEdmExpression) new EdmStringConstant("ax.StandardDeviationP"),
          (IEdmExpression) new EdmStringConstant("ax.Variance"),
          (IEdmExpression) new EdmStringConstant("ax.VarianceP")
        }))
      }))
    {
    }
  }
}
