// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Vocabularies.EdmVocabularyAnnotation
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

namespace Microsoft.OData.Edm.Vocabularies
{
  public class EdmVocabularyAnnotation : EdmElement, IEdmVocabularyAnnotation, IEdmElement
  {
    private readonly IEdmVocabularyAnnotatable target;
    private readonly IEdmTerm term;
    private readonly string qualifier;
    private readonly IEdmExpression value;

    public EdmVocabularyAnnotation(
      IEdmVocabularyAnnotatable target,
      IEdmTerm term,
      IEdmExpression value)
      : this(target, term, (string) null, value)
    {
    }

    public EdmVocabularyAnnotation(
      IEdmVocabularyAnnotatable target,
      IEdmTerm term,
      string qualifier,
      IEdmExpression value)
    {
      EdmUtil.CheckArgumentNull<IEdmVocabularyAnnotatable>(target, nameof (target));
      EdmUtil.CheckArgumentNull<IEdmTerm>(term, nameof (term));
      EdmUtil.CheckArgumentNull<IEdmExpression>(value, nameof (value));
      this.target = target;
      this.term = term;
      this.qualifier = qualifier;
      this.value = value;
    }

    public IEdmVocabularyAnnotatable Target => this.target;

    public IEdmTerm Term => this.term;

    public string Qualifier => this.qualifier;

    public IEdmExpression Value => this.value;
  }
}
