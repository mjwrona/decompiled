// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.Parsing.Ast.CsdlElement
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
  internal abstract class CsdlElement
  {
    protected List<object> annotations;
    protected EdmLocation location;

    public CsdlElement(CsdlLocation location) => this.location = (EdmLocation) location;

    public virtual bool HasDirectValueAnnotations => this.HasAnnotations<CsdlDirectValueAnnotation>();

    public bool HasVocabularyAnnotations => this.HasAnnotations<CsdlAnnotation>();

    public IEnumerable<CsdlDirectValueAnnotation> ImmediateValueAnnotations => this.GetAnnotations<CsdlDirectValueAnnotation>();

    public IEnumerable<CsdlAnnotation> VocabularyAnnotations => this.GetAnnotations<CsdlAnnotation>();

    public EdmLocation Location => this.location;

    public void AddAnnotation(CsdlDirectValueAnnotation annotation) => this.AddUntypedAnnotation((object) annotation);

    public void AddAnnotation(CsdlAnnotation annotation) => this.AddUntypedAnnotation((object) annotation);

    private IEnumerable<T> GetAnnotations<T>() where T : class => this.annotations == null ? Enumerable.Empty<T>() : this.annotations.OfType<T>();

    private void AddUntypedAnnotation(object annotation)
    {
      if (this.annotations == null)
        this.annotations = new List<object>();
      this.annotations.Add(annotation);
    }

    private bool HasAnnotations<T>()
    {
      if (this.annotations == null)
        return false;
      foreach (object annotation in this.annotations)
      {
        if (annotation is T)
          return true;
      }
      return false;
    }
  }
}
