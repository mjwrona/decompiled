// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsElement
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
  internal abstract class CsdlSemanticsElement : IEdmElement, IEdmLocatable
  {
    private readonly Cache<CsdlSemanticsElement, IEnumerable<IEdmVocabularyAnnotation>> inlineVocabularyAnnotationsCache;
    private static readonly Func<CsdlSemanticsElement, IEnumerable<IEdmVocabularyAnnotation>> ComputeInlineVocabularyAnnotationsFunc = (Func<CsdlSemanticsElement, IEnumerable<IEdmVocabularyAnnotation>>) (me => me.ComputeInlineVocabularyAnnotations());
    private readonly Cache<CsdlSemanticsElement, IEnumerable<IEdmDirectValueAnnotation>> directValueAnnotationsCache;
    private static readonly Func<CsdlSemanticsElement, IEnumerable<IEdmDirectValueAnnotation>> ComputeDirectValueAnnotationsFunc = (Func<CsdlSemanticsElement, IEnumerable<IEdmDirectValueAnnotation>>) (me => me.ComputeDirectValueAnnotations());
    private static readonly IEnumerable<IEdmVocabularyAnnotation> emptyVocabularyAnnotations = Enumerable.Empty<IEdmVocabularyAnnotation>();

    protected CsdlSemanticsElement(CsdlElement element)
    {
      if (element == null)
        return;
      if (element.HasDirectValueAnnotations)
        this.directValueAnnotationsCache = new Cache<CsdlSemanticsElement, IEnumerable<IEdmDirectValueAnnotation>>();
      if (!element.HasVocabularyAnnotations)
        return;
      this.inlineVocabularyAnnotationsCache = new Cache<CsdlSemanticsElement, IEnumerable<IEdmVocabularyAnnotation>>();
    }

    public abstract CsdlSemanticsModel Model { get; }

    public abstract CsdlElement Element { get; }

    public IEnumerable<IEdmVocabularyAnnotation> InlineVocabularyAnnotations => this.inlineVocabularyAnnotationsCache == null ? CsdlSemanticsElement.emptyVocabularyAnnotations : this.inlineVocabularyAnnotationsCache.GetValue(this, CsdlSemanticsElement.ComputeInlineVocabularyAnnotationsFunc, (Func<CsdlSemanticsElement, IEnumerable<IEdmVocabularyAnnotation>>) null);

    public EdmLocation Location => this.Element == null || this.Element.Location == null ? (EdmLocation) new ObjectLocation((object) this) : this.Element.Location;

    public IEnumerable<IEdmDirectValueAnnotation> DirectValueAnnotations => this.directValueAnnotationsCache == null ? (IEnumerable<IEdmDirectValueAnnotation>) null : this.directValueAnnotationsCache.GetValue(this, CsdlSemanticsElement.ComputeDirectValueAnnotationsFunc, (Func<CsdlSemanticsElement, IEnumerable<IEdmDirectValueAnnotation>>) null);

    protected static List<T> AllocateAndAdd<T>(List<T> list, T item)
    {
      if (list == null)
        list = new List<T>();
      list.Add(item);
      return list;
    }

    protected static List<T> AllocateAndAdd<T>(List<T> list, IEnumerable<T> items)
    {
      if (list == null)
        list = new List<T>();
      list.AddRange(items);
      return list;
    }

    protected virtual IEnumerable<IEdmVocabularyAnnotation> ComputeInlineVocabularyAnnotations() => this.Model.WrapInlineVocabularyAnnotations(this, (CsdlSemanticsSchema) null);

    protected IEnumerable<IEdmDirectValueAnnotation> ComputeDirectValueAnnotations()
    {
      if (this.Element == null)
        return (IEnumerable<IEdmDirectValueAnnotation>) null;
      List<CsdlDirectValueAnnotation> list = this.Element.ImmediateValueAnnotations.ToList<CsdlDirectValueAnnotation>();
      if (list.FirstOrDefault<CsdlDirectValueAnnotation>() == null)
        return (IEnumerable<IEdmDirectValueAnnotation>) null;
      List<IEdmDirectValueAnnotation> valueAnnotations = new List<IEdmDirectValueAnnotation>();
      foreach (CsdlDirectValueAnnotation annotation in list)
        valueAnnotations.Add((IEdmDirectValueAnnotation) new CsdlSemanticsDirectValueAnnotation(annotation, this.Model));
      return (IEnumerable<IEdmDirectValueAnnotation>) valueAnnotations;
    }
  }
}
