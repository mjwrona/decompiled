// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.MarkdownRenderer.WorkItemMentionParser
// Assembly: Microsoft.TeamFoundation.MarkdownRenderer.Sdk, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EF23BBEE-E2C7-4A34-A6FB-0292D3B7C69D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.MarkdownRenderer.Sdk.dll

using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Syntax.Inlines;
using Microsoft.TeamFoundation.Mention.Server;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.MarkdownRenderer
{
  public sealed class WorkItemMentionParser : InlineParser
  {
    private const int c_workItemMaxLength = 19;
    private const int c_personMentionLength = 38;
    private const int c_workItemMentionsMaxLimit = 50;
    private static readonly HashSet<char> WitMentionValidPrefixSuffixes = new HashSet<char>()
    {
      ',',
      '.',
      ':',
      ';',
      '-',
      '"',
      '\'',
      '\n'
    };
    private static readonly char[] s_openingCharacters = new char[3]
    {
      '#',
      '!',
      '@'
    };
    private readonly IDictionary<string, Microsoft.TeamFoundation.Mention.Server.Mention> m_parsedMentions = (IDictionary<string, Microsoft.TeamFoundation.Mention.Server.Mention>) new Dictionary<string, Microsoft.TeamFoundation.Mention.Server.Mention>();
    private readonly IMentionSourceContext m_sourceContext;

    public WorkItemMentionParser(IMentionSourceContext sourceContext) => this.m_sourceContext = sourceContext;

    public override void Initialize() => this.OpeningCharacters = WorkItemMentionParser.s_openingCharacters;

    public override bool Match(InlineProcessor processor, ref StringSlice slice)
    {
      if (this.m_parsedMentions.Count >= 50)
        return false;
      char c1 = slice.PeekCharExtra(-1);
      char c2 = slice.PeekCharExtra(1);
      char currentChar = slice.CurrentChar;
      int num1 = 0;
      switch (currentChar)
      {
        case '!':
        case '#':
          if (!c2.IsDigit() || !c1.IsWhiteSpaceOrZero() && !WorkItemMentionParser.WitMentionValidPrefixSuffixes.Contains(c1))
            return false;
          for (; c2.IsDigit() && num1 <= 19; c2 = slice.PeekCharExtra(num1 + 1))
            ++num1;
          if (num1 > 19 || !c2.IsWhiteSpaceOrZero() && !WorkItemMentionParser.WitMentionValidPrefixSuffixes.Contains(c2))
            return false;
          break;
        case '@':
          if (c2 != '<' || !c1.IsWhiteSpaceOrZero() && !WorkItemMentionParser.WitMentionValidPrefixSuffixes.Contains(c1))
            return false;
          int num2 = num1 + 1;
          char c3 = slice.PeekCharExtra(num2 + 1);
          while (c3.IsAlphaNumeric() && num2 <= 38)
          {
            ++num2;
            c3 = slice.PeekCharExtra(num2 + 1);
            if (num2 == 9 || num2 == 14 || num2 == 19 || num2 == 24)
            {
              if (c3 != '-')
                return false;
              ++num2;
              c3 = slice.PeekCharExtra(num2 + 1);
            }
          }
          if (c3 != '>')
            return false;
          num1 = num2 + 1;
          char c4 = slice.PeekCharExtra(num1 + 1);
          if (num1 != 38 || !c4.IsWhiteSpaceOrZero() && !WorkItemMentionParser.WitMentionValidPrefixSuffixes.Contains(c4))
            return false;
          break;
      }
      int length = num1 + 1;
      int start = slice.Start;
      slice.Start += length;
      if (num1 <= 0)
        return false;
      int lineIndex;
      int column;
      int sourcePosition = processor.GetSourcePosition(start, out lineIndex, out column);
      int num3 = sourcePosition + length;
      string key = slice.Text.Substring(start, length);
      string str1 = key.Substring(1);
      if (key[0] == '@')
      {
        str1 = key.Substring(2, 36);
        key = key[0].ToString() + str1;
      }
      string str2 = "WorkItem";
      if (key[0] == '@')
        str2 = "Person";
      else if (key[0] == '!')
        str2 = "PullRequest";
      Microsoft.TeamFoundation.Mention.Server.Mention mention;
      if (!this.m_parsedMentions.TryGetValue(key, out mention))
      {
        string str3 = str2 == "Person" ? "EMAIL_MENTION" : "None";
        mention = new Microsoft.TeamFoundation.Mention.Server.Mention()
        {
          Source = this.m_sourceContext,
          ArtifactId = str1,
          RawText = key,
          ArtifactType = str2,
          TargetId = str1,
          MentionAction = str3,
          IsNew = true
        };
        this.m_parsedMentions.Add(key, mention);
      }
      InlineProcessor inlineProcessor = processor;
      InlineMention inlineMention = new InlineMention(mention);
      inlineMention.Span.Start = sourcePosition;
      inlineMention.Span.End = num3;
      inlineMention.Line = lineIndex;
      inlineMention.Column = column;
      inlineProcessor.Inline = (Inline) inlineMention;
      return true;
    }

    public IReadOnlyList<Microsoft.TeamFoundation.Mention.Server.Mention> GetMentions() => (IReadOnlyList<Microsoft.TeamFoundation.Mention.Server.Mention>) this.m_parsedMentions.Values.ToList<Microsoft.TeamFoundation.Mention.Server.Mention>();
  }
}
