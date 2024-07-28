// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.FilterFunctionsAnnotationAttribute
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
  public class FilterFunctionsAnnotationAttribute : EdmVocabularyAnnotationAttribute
  {
    public static readonly string[] FilterFunctions = new string[24]
    {
      "contains",
      "endswith",
      "startswith",
      "length",
      "indexof",
      "substring",
      "tolower",
      "toupper",
      "trim",
      "concat",
      "year",
      "month",
      "day",
      "hour",
      "minute",
      "second",
      "fractionalseconds",
      "round",
      "floor",
      "ceiling",
      "date",
      "time",
      "isof",
      "cast"
    };
    private static readonly IEdmTerm FilterFunctionsTerm = (IEdmTerm) new EdmTerm("Org.OData.Capabilities.V1", nameof (FilterFunctions), (IEdmTypeReference) new EdmCollectionTypeReference((IEdmCollectionType) new EdmCollectionType((IEdmTypeReference) new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), false))));
    public static readonly FilterFunctionsAnnotationAttribute Default = new FilterFunctionsAnnotationAttribute((IEnumerable<string>) FilterFunctionsAnnotationAttribute.FilterFunctions);

    public FilterFunctionsAnnotationAttribute(IEnumerable<string> filterFunctions)
      : base(FilterFunctionsAnnotationAttribute.FilterFunctionsTerm, (IEdmExpression) new EdmCollectionExpression((IEnumerable<IEdmExpression>) filterFunctions.Select<string, EdmStringConstant>((Func<string, EdmStringConstant>) (f => new EdmStringConstant(f)))))
    {
    }
  }
}
