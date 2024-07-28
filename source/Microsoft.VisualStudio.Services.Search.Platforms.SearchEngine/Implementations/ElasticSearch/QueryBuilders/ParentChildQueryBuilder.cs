// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.ParentChildQueryBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders
{
  internal class ParentChildQueryBuilder
  {
    private string m_parentQueryString;
    private string m_childQueryString;
    private DocumentContractType m_childContractType;
    private int m_maxInnerHits;

    public ParentChildQueryBuilder(
      string parentQueryString,
      string childQueryString,
      DocumentContractType childContractType,
      int maxInnerHits)
    {
      if (parentQueryString == null)
        throw new ArgumentNullException(nameof (parentQueryString));
      if (childQueryString == null)
        throw new ArgumentNullException(nameof (childQueryString));
      this.m_parentQueryString = parentQueryString;
      this.m_childQueryString = childQueryString;
      this.m_childContractType = childContractType;
      this.m_maxInnerHits = maxInnerHits;
    }

    public string GetParentChildQueryRawString()
    {
      string childQueryRawString;
      if (string.IsNullOrEmpty(this.m_parentQueryString) && string.IsNullOrEmpty(this.m_childQueryString))
        childQueryRawString = string.Empty;
      else if (string.IsNullOrEmpty(this.m_parentQueryString))
        childQueryRawString = this.m_childQueryString;
      else if (string.IsNullOrEmpty(this.m_childQueryString))
        childQueryRawString = this.m_parentQueryString;
      else
        childQueryRawString = FormattableString.Invariant(FormattableStringFactory.Create("{{")) + FormattableString.Invariant(FormattableStringFactory.Create("  \"bool\": {{")) + FormattableString.Invariant(FormattableStringFactory.Create("      \"should\": [")) + FormattableString.Invariant(FormattableStringFactory.Create("          {0},", (object) this.m_parentQueryString)) + FormattableString.Invariant(FormattableStringFactory.Create("          {0}", (object) this.CreateHasChildQueryString())) + FormattableString.Invariant(FormattableStringFactory.Create("          ],")) + FormattableString.Invariant(FormattableStringFactory.Create("      \"minimum_should_match\": {0}", (object) 1)) + FormattableString.Invariant(FormattableStringFactory.Create("  }}")) + FormattableString.Invariant(FormattableStringFactory.Create("}}")).PrettyJson();
      return childQueryRawString;
    }

    private string CreateHasChildQueryString() => FormattableString.Invariant(FormattableStringFactory.Create("{{")) + FormattableString.Invariant(FormattableStringFactory.Create("  \"has_child\": {{")) + FormattableString.Invariant(FormattableStringFactory.Create("      \"type\":\"{0}\",", (object) this.m_childContractType.ToString())) + FormattableString.Invariant(FormattableStringFactory.Create("      \"score_mode\": \"{0}\",", (object) "sum")) + FormattableString.Invariant(FormattableStringFactory.Create("      \"query\": {0},", (object) this.m_childQueryString)) + FormattableString.Invariant(FormattableStringFactory.Create("      \"inner_hits\": {{")) + FormattableString.Invariant(FormattableStringFactory.Create("          \"size\": {0},", (object) this.m_maxInnerHits)) + FormattableString.Invariant(FormattableStringFactory.Create("          \"highlight\": {0}", (object) ParentChildQueryBuilder.GetInnerHitsHighlightPropertiesString())) + FormattableString.Invariant(FormattableStringFactory.Create("      }}")) + FormattableString.Invariant(FormattableStringFactory.Create(" }}")) + FormattableString.Invariant(FormattableStringFactory.Create("}}")).ToLowerInvariant();

    private static string GetInnerHitsHighlightPropertiesString()
    {
      string str1 = "unified";
      string str2 = FormattableString.Invariant(FormattableStringFactory.Create(""));
      return FormattableString.Invariant(FormattableStringFactory.Create("{{")) + FormattableString.Invariant(FormattableStringFactory.Create("  \"fields\": {{")) + FormattableString.Invariant(FormattableStringFactory.Create("      \"{0}\": {{\"fragment_size\": 150, \"number_of_fragments\": 0}},", (object) "name")) + FormattableString.Invariant(FormattableStringFactory.Create("      \"{0}\": {{\"fragment_size\": 150, \"number_of_fragments\": 0}},", (object) "name.casechangeanalyzed")) + FormattableString.Invariant(FormattableStringFactory.Create("      \"{0}\": {{\"fragment_size\": 150, \"number_of_fragments\": 3}}", (object) "readme")) + str2 + FormattableString.Invariant(FormattableStringFactory.Create("  }},")) + FormattableString.Invariant(FormattableStringFactory.Create("  \"pre_tags\": [\"{0}\"],", (object) "<highlighthit>")) + FormattableString.Invariant(FormattableStringFactory.Create("  \"post_tags\": [\"{0}\"],", (object) "</highlighthit>")) + FormattableString.Invariant(FormattableStringFactory.Create("  \"type\": \"{0}\"", (object) str1)) + FormattableString.Invariant(FormattableStringFactory.Create("}}"));
    }
  }
}
