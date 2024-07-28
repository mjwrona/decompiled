// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.EntitySearchQueryTagger
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Arriba;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Query
{
  internal abstract class EntitySearchQueryTagger
  {
    protected readonly IDictionary<string, IEnumerable<string>> m_searchFilters;
    protected readonly IExpression m_expression;
    private int m_numUnfilteredWords;
    protected const string And = "And";
    protected const string Or = "Or";
    protected const string Not = "Not";
    protected const string NoWords = "NoWords";
    protected const string Phrase = "Phrase";
    protected const char WildcardAsteriskCharacter = '*';
    protected const char WildcardQuestionCharacter = '?';
    protected const string WildcardAsterisk = "WildcardAsterisk";
    protected const string WildcardQuestion = "WildcardQuestion";
    protected const string SingleUnfilteredWord = "SingleUnfilteredWord";
    protected const string MultipleUnfilteredWords = "MultipleUnfilteredWords";
    protected const string UnfilteredPostfixWildcard = "UnfilteredPostfixWildcard";
    protected const string AsteriskWildcardStringLength = "AsteriskWildcardLength";
    protected const string QuestionWildcardStringLength = "QuestionWildcardLength";
    protected const string InlineFiltersFormatString = "{0}InlineFilter";
    protected const string BoardTypeFilter = "BoardTypeFilter";
    protected const string CollectionsFilter = "CollectionsFilter";
    protected const string TagsFilter = "TagsFilter";
    protected const string LanguagesFilter = "LanguagesFilter";
    protected const string ProjectsFilter = "ProjectsFilter";
    protected const string WikisFilter = "WikisFilter";
    protected const string VisibilityFilter = "VisibilityFilter";
    protected const string CITagProjectEntity = "ProjectSearchRequestTags";
    protected const string CITagRepositoryEntity = "RepositorySearchRequestTags";
    protected const string CITagWikiEntity = "WikiSearchRequestTags";
    protected const string CITagBoardEntity = "BoardSearchRequestTags";

    public SortedSet<string> Tags { get; }

    public EntitySearchQueryTagger(
      IExpression expression,
      IDictionary<string, IEnumerable<string>> searchFilters)
    {
      this.m_expression = expression;
      this.m_searchFilters = searchFilters;
      this.Tags = new SortedSet<string>((IComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    public override string ToString() => string.Join(", ", (IEnumerable<string>) this.Tags);

    public void Compute()
    {
      this.TagFilters();
      this.TagExpression();
      this.TagForUnfilteredWords();
    }

    public void Publish()
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add(this.GetTaggerCIEntityTag(), this.ToString());
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishCi("Query Pipeline", "Query Pipeline", properties);
    }

    protected virtual string GetTaggerCIEntityTag() => throw new NotImplementedException("To be implemented by the child class.");

    protected virtual IDictionary<string, string> GetSupportedFilterCategoriesWithTagsMapping() => throw new NotImplementedException("To be implemented by the child class.");

    protected virtual void TagExpression() => throw new NotImplementedException("To be implemented by the child class.");

    protected void TagTermExpression(TermExpression termExpression, bool logInlineFilters = false)
    {
      bool flag1 = termExpression.IsOfType("*");
      if (flag1)
        ++this.m_numUnfilteredWords;
      else if (logInlineFilters)
        this.Tags.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}InlineFilter", (object) termExpression.Type));
      if (termExpression.Value.ContainsWhitespace())
        this.Tags.Add("Phrase");
      if (termExpression.Operator != Operator.Matches)
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1081333, "Query Pipeline", "Query", FormattableString.Invariant(FormattableStringFactory.Create("Unexpected operator [{0}] found in {1}.", (object) termExpression.Operator, (object) this.GetType().Name)));
      int num1 = termExpression.Value.IndexOf('*');
      bool flag2 = num1 >= 0;
      if (flag2)
        this.Tags.Add("WildcardAsterisk");
      if (num1 > 0)
        this.Tags.Add("AsteriskWildcardLength" + ":" + (object) num1);
      int num2 = termExpression.Value.IndexOf('?');
      bool flag3 = num2 >= 0;
      if (flag3)
        this.Tags.Add("WildcardQuestion");
      if (num2 > 0)
        this.Tags.Add("QuestionWildcardLength" + ":" + (object) num2);
      if (!flag1 || !(flag2 | flag3))
        return;
      switch (termExpression.Value[0])
      {
        case '*':
        case '?':
          this.Tags.Add(EntitySearchQueryTagger.UnfilteredPrefixWildcard);
          break;
      }
      switch (termExpression.Value[termExpression.Value.Length - 1])
      {
        case '*':
        case '?':
          this.Tags.Add("UnfilteredPostfixWildcard");
          break;
      }
    }

    private void TagFilters()
    {
      IDictionary<string, string> categoriesWithTagsMapping = this.GetSupportedFilterCategoriesWithTagsMapping();
      int num = 0;
      foreach (string key in (IEnumerable<string>) categoriesWithTagsMapping.Keys)
      {
        if (this.m_searchFilters.ContainsKey(key))
        {
          this.Tags.Add(categoriesWithTagsMapping[key]);
          ++num;
        }
      }
      if (this.m_searchFilters.Count == num)
        return;
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1081332, "Query Pipeline", "Query", FormattableString.Invariant(FormattableStringFactory.Create("[{0}] contains unrecognized search filter category(ies) found in {1}.", (object) string.Join(", ", (IEnumerable<string>) this.m_searchFilters.Keys), (object) this.GetType().Name)));
    }

    private void TagForUnfilteredWords()
    {
      if (this.m_numUnfilteredWords == 1)
      {
        this.Tags.Add("SingleUnfilteredWord");
      }
      else
      {
        if (this.m_numUnfilteredWords <= 1)
          return;
        this.Tags.Add("MultipleUnfilteredWords");
      }
    }

    public static string UnfilteredPrefixWildcard { get; } = nameof (UnfilteredPrefixWildcard);
  }
}
