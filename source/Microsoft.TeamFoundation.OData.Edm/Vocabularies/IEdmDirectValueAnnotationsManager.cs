// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Vocabularies.IEdmDirectValueAnnotationsManager
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Vocabularies
{
  public interface IEdmDirectValueAnnotationsManager
  {
    IEnumerable<IEdmDirectValueAnnotation> GetDirectValueAnnotations(IEdmElement element);

    void SetAnnotationValue(
      IEdmElement element,
      string namespaceName,
      string localName,
      object value);

    void SetAnnotationValues(
      IEnumerable<IEdmDirectValueAnnotationBinding> annotations);

    object GetAnnotationValue(IEdmElement element, string namespaceName, string localName);

    object[] GetAnnotationValues(
      IEnumerable<IEdmDirectValueAnnotationBinding> annotations);
  }
}
