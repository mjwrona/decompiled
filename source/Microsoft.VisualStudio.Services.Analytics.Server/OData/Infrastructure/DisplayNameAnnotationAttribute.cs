// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.DisplayNameAnnotationAttribute
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
using System;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  [AttributeUsage(AttributeTargets.Class, Inherited = true)]
  public class DisplayNameAnnotationAttribute : EdmVocabularyAnnotationAttribute
  {
    private static readonly EdmTerm DisplayNameTerm = new EdmTerm("Org.OData.Display.V1", "DisplayName", EdmPrimitiveTypeKind.String);

    public DisplayNameAnnotationAttribute(string value)
      : base((IEdmTerm) DisplayNameAnnotationAttribute.DisplayNameTerm, (IEdmExpression) new EdmStringConstant(AnalyticsResources.Manager.GetString(value) ?? value))
    {
    }
  }
}
