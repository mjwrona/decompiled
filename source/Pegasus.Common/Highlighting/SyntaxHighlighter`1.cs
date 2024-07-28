// Decompiled with JetBrains decompiler
// Type: Pegasus.Common.Highlighting.SyntaxHighlighter`1
// Assembly: Pegasus.Common, Version=4.0.14.0, Culture=neutral, PublicKeyToken=28c69b6c6d100f4a
// MVID: 081C50C0-D236-41F7-86F9-E3F2168B7118
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Pegasus.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Pegasus.Common.Highlighting
{
  public class SyntaxHighlighter<T>
  {
    private readonly IList<HighlightRule<T>> list;

    public SyntaxHighlighter(IEnumerable<HighlightRule<T>> rules) => this.list = (IList<HighlightRule<T>>) rules.ToList<HighlightRule<T>>();

    public List<HighlightedSegment<T>> AddDefaultTokens(
      IList<HighlightedSegment<T>> tokens,
      int subjectLength,
      T defaultValue)
    {
      if (tokens == null)
        throw new ArgumentNullException(nameof (tokens));
      List<HighlightedSegment<T>> highlightedSegmentList = new List<HighlightedSegment<T>>();
      int start = 0;
      foreach (HighlightedSegment<T> token in (IEnumerable<HighlightedSegment<T>>) tokens)
      {
        if (token.Start > start)
          highlightedSegmentList.Add(new HighlightedSegment<T>(start, token.Start, defaultValue));
        highlightedSegmentList.Add(token);
        start = token.End;
      }
      if (start < subjectLength)
        highlightedSegmentList.Add(new HighlightedSegment<T>(start, subjectLength, defaultValue));
      return highlightedSegmentList;
    }

    public List<HighlightedSegment<T>> GetTokens(IList<LexicalElement> lexicalElements) => SyntaxHighlighter<T>.SimplifyHighlighting((IList<HighlightedSegment<T>>) this.HighlightLexicalElements(lexicalElements));

    public List<HighlightedSegment<T>> SplitOnWhiteSpace(
      List<HighlightedSegment<T>> tokens,
      string subject)
    {
      if (tokens == null)
        throw new ArgumentNullException(nameof (tokens));
      if (subject == null)
        throw new ArgumentNullException(nameof (subject));
      List<HighlightedSegment<T>> highlightedSegmentList = new List<HighlightedSegment<T>>();
      foreach (HighlightedSegment<T> token in tokens)
      {
        if (token.End - token.Start < 2)
        {
          highlightedSegmentList.Add(token);
        }
        else
        {
          int num1 = token.Start;
          int num2 = token.End - 1;
          bool flag1 = char.IsWhiteSpace(subject[num1]);
          for (int index = num1 + 1; index < num2; ++index)
          {
            bool flag2 = char.IsWhiteSpace(subject[index]);
            if (flag1 != flag2)
            {
              highlightedSegmentList.Add(new HighlightedSegment<T>(num1, index, token.Value));
              num1 = index;
            }
            flag1 = flag2;
          }
          highlightedSegmentList.Add(num1 == token.Start ? token : new HighlightedSegment<T>(num1, token.End, token.Value));
        }
      }
      return highlightedSegmentList;
    }

    private static List<HighlightedSegment<T>> SimplifyHighlighting(
      IList<HighlightedSegment<T>> tokens)
    {
      List<HighlightedSegment<T>> highlightedSegmentList = new List<HighlightedSegment<T>>(tokens.Count);
      Stack<HighlightedSegment<T>> highlightedSegmentStack = new Stack<HighlightedSegment<T>>();
      foreach (HighlightedSegment<T> token in (IEnumerable<HighlightedSegment<T>>) tokens)
      {
        while (highlightedSegmentStack.Count != 0)
        {
          HighlightedSegment<T> highlightedSegment1 = highlightedSegmentStack.Pop();
          if (highlightedSegment1.Start >= token.End)
          {
            highlightedSegmentList.Add(highlightedSegment1);
          }
          else
          {
            if (highlightedSegment1.End > token.End)
              highlightedSegmentList.Add(new HighlightedSegment<T>(token.End, highlightedSegment1.End, highlightedSegment1.Value));
            HighlightedSegment<T> highlightedSegment2 = new HighlightedSegment<T>(highlightedSegment1.Start, token.Start, highlightedSegment1.Value);
            if (highlightedSegment2.Start < highlightedSegment2.End)
            {
              highlightedSegmentStack.Push(highlightedSegment2);
              break;
            }
          }
        }
        highlightedSegmentStack.Push(token);
      }
      while (highlightedSegmentStack.Count > 0)
        highlightedSegmentList.Add(highlightedSegmentStack.Pop());
      highlightedSegmentList.Reverse();
      return highlightedSegmentList;
    }

    private Tuple<int, T> Highlight(string key, int? maxRule = null)
    {
      if (maxRule.HasValue && (maxRule.Value > this.list.Count || maxRule.Value < 0))
        throw new ArgumentOutOfRangeException(nameof (maxRule));
      maxRule = new int?(maxRule ?? this.list.Count);
      for (int index = 0; index < maxRule.Value; ++index)
      {
        HighlightRule<T> highlightRule = this.list[index];
        if (highlightRule.Pattern.IsMatch(key))
          return Tuple.Create<int, T>(index, highlightRule.Value);
      }
      return (Tuple<int, T>) null;
    }

    private List<HighlightedSegment<T>> HighlightLexicalElements(
      IList<LexicalElement> lexicalElements)
    {
      List<HighlightedSegment<T>> highlightedSegmentList = new List<HighlightedSegment<T>>(lexicalElements.Count);
      if (lexicalElements.Count == 0)
        return highlightedSegmentList;
      Stack<Tuple<int, LexicalElement>> source = new Stack<Tuple<int, LexicalElement>>();
      foreach (LexicalElement lexicalElement in lexicalElements.Reverse<LexicalElement>().Where<LexicalElement>((Func<LexicalElement, bool>) (e => e.StartCursor.Location != e.EndCursor.Location)))
      {
        int? maxRule = new int?();
        while (source.Count != 0)
        {
          Tuple<int, LexicalElement> tuple = source.Peek();
          if (lexicalElement.EndCursor.Location > tuple.Item2.StartCursor.Location && lexicalElement.StartCursor.Location < tuple.Item2.EndCursor.Location)
          {
            maxRule = new int?(tuple.Item1);
            break;
          }
          source.Pop();
        }
        int? nullable = maxRule;
        int num = 0;
        if ((nullable.GetValueOrDefault() == num ? (nullable.HasValue ? 1 : 0) : 0) == 0)
        {
          Stack<Tuple<int, LexicalElement>> tupleStack = source;
          nullable = maxRule;
          Tuple<int, LexicalElement> tuple1 = Tuple.Create<int, LexicalElement>(nullable ?? this.list.Count, lexicalElement);
          tupleStack.Push(tuple1);
          Tuple<int, T> tuple2 = this.Highlight(string.Join(" ", source.Select<Tuple<int, LexicalElement>, string>((Func<Tuple<int, LexicalElement>, string>) (d => d.Item2.Name))), maxRule);
          if (tuple2 != null)
          {
            source.Pop();
            source.Push(Tuple.Create<int, LexicalElement>(tuple2.Item1, lexicalElement));
            highlightedSegmentList.Add(new HighlightedSegment<T>(lexicalElement.StartCursor.Location, lexicalElement.EndCursor.Location, tuple2.Item2));
          }
        }
      }
      return highlightedSegmentList;
    }
  }
}
