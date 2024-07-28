// Decompiled with JetBrains decompiler
// Type: Tomlyn.Syntax.SyntaxNodeExtensions
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System;
using System.Collections.Generic;


#nullable enable
namespace Tomlyn.Syntax
{
  public static class SyntaxNodeExtensions
  {
    public static IEnumerable<SyntaxNodeBase> Tokens(
      this SyntaxNode node,
      bool includeCommentsAndWhitespaces = true)
    {
      foreach (SyntaxNodeBase descendant in node.Descendants(true))
      {
        if (descendant is SyntaxTrivia)
        {
          if (!includeCommentsAndWhitespaces)
            continue;
        }
        else if (!(descendant is SyntaxToken))
          continue;
        yield return descendant;
      }
    }

    public static IEnumerable<SyntaxNodeBase> Descendants(
      this SyntaxNode node,
      bool includeTokensCommentsAndWhitespaces = false)
    {
      if (includeTokensCommentsAndWhitespaces || !(node is SyntaxToken))
      {
        if (includeTokensCommentsAndWhitespaces && node.LeadingTrivia != null)
        {
          foreach (SyntaxNodeBase syntaxNodeBase in node.LeadingTrivia)
            yield return syntaxNodeBase;
        }
        int childrenCount = node.ChildrenCount;
        for (int i = 0; i < childrenCount; ++i)
        {
          SyntaxNode child1 = node.GetChild(i);
          if (child1 is SyntaxList list)
          {
            int subChildrenCount = list.ChildrenCount;
            for (int j = 0; j < subChildrenCount; ++j)
            {
              SyntaxNode child2 = list.GetChild(j);
              if (child2 != null)
              {
                foreach (SyntaxNodeBase descendant in child2.Descendants(includeTokensCommentsAndWhitespaces))
                  yield return descendant;
              }
            }
          }
          else if (child1 != null)
          {
            foreach (SyntaxNodeBase descendant in child1.Descendants(includeTokensCommentsAndWhitespaces))
              yield return descendant;
          }
          list = (SyntaxList) null;
        }
        yield return (SyntaxNodeBase) node;
        if (includeTokensCommentsAndWhitespaces && node.TrailingTrivia != null)
        {
          foreach (SyntaxNodeBase syntaxNodeBase in node.TrailingTrivia)
            yield return syntaxNodeBase;
        }
      }
    }

    public static void Add(this SyntaxList<KeyValueSyntax> list, string name, int value)
    {
      if (list == null)
        throw new ArgumentNullException(nameof (list));
      list.Add(new KeyValueSyntax(name, (ValueSyntax) new IntegerValueSyntax((long) value)));
    }

    public static void Add(this SyntaxList<KeyValueSyntax> list, string name, long value)
    {
      if (list == null)
        throw new ArgumentNullException(nameof (list));
      list.Add(new KeyValueSyntax(name, (ValueSyntax) new IntegerValueSyntax(value)));
    }

    public static void Add(this SyntaxList<KeyValueSyntax> list, string name, bool value)
    {
      if (list == null)
        throw new ArgumentNullException(nameof (list));
      list.Add(new KeyValueSyntax(name, (ValueSyntax) new BooleanValueSyntax(value)));
    }

    public static void Add(this SyntaxList<KeyValueSyntax> list, string name, double value)
    {
      if (list == null)
        throw new ArgumentNullException(nameof (list));
      list.Add(new KeyValueSyntax(name, (ValueSyntax) new FloatValueSyntax(value)));
    }

    public static void Add(this SyntaxList<KeyValueSyntax> list, string name, string value)
    {
      if (list == null)
        throw new ArgumentNullException(nameof (list));
      list.Add(new KeyValueSyntax(name, (ValueSyntax) new StringValueSyntax(value)));
    }

    public static void Add(this SyntaxList<KeyValueSyntax> list, string name, int[] values)
    {
      if (list == null)
        throw new ArgumentNullException(nameof (list));
      list.Add(new KeyValueSyntax(name, (ValueSyntax) new ArraySyntax(values)));
    }

    public static void Add(this SyntaxList<KeyValueSyntax> list, string name, string[] values)
    {
      if (list == null)
        throw new ArgumentNullException(nameof (list));
      list.Add(new KeyValueSyntax(name, (ValueSyntax) new ArraySyntax(values)));
    }

    public static void Add(
      this SyntaxList<KeyValueSyntax> list,
      string name,
      DateTimeValueSyntax value)
    {
      if (list == null)
        throw new ArgumentNullException(nameof (list));
      list.Add(new KeyValueSyntax(name, (ValueSyntax) value));
    }

    public static KeyValueSyntax AddTrailingComment(this KeyValueSyntax keyValue, string comment)
    {
      if (keyValue == null)
        throw new ArgumentNullException(nameof (keyValue));
      if (keyValue.Value == null)
        throw new InvalidOperationException("The Value must not be null on the KeyValueSyntax");
      keyValue.Value.AddTrailingWhitespace<ValueSyntax>().AddTrailingComment<ValueSyntax>(comment);
      return keyValue;
    }

    public static T AddLeadingWhitespace<T>(this T node) where T : SyntaxNode => node.AddLeadingTrivia<T>(SyntaxFactory.Whitespace());

    public static T AddTrailingWhitespace<T>(this T node) where T : SyntaxNode => node.AddTrailingTrivia<T>(SyntaxFactory.Whitespace());

    public static T AddLeadingTrivia<T>(this T node, SyntaxTrivia trivia) where T : SyntaxNode
    {
      List<SyntaxTrivia> syntaxTriviaList = (object) node != null ? node.LeadingTrivia : throw new ArgumentNullException(nameof (node));
      if (syntaxTriviaList == null)
      {
        syntaxTriviaList = new List<SyntaxTrivia>();
        node.LeadingTrivia = syntaxTriviaList;
      }
      syntaxTriviaList.Add(trivia);
      return node;
    }

    public static T AddTrailingTrivia<T>(this T node, SyntaxTrivia trivia) where T : SyntaxNode
    {
      List<SyntaxTrivia> syntaxTriviaList = (object) node != null ? node.TrailingTrivia : throw new ArgumentNullException(nameof (node));
      if (syntaxTriviaList == null)
      {
        syntaxTriviaList = new List<SyntaxTrivia>();
        node.TrailingTrivia = syntaxTriviaList;
      }
      syntaxTriviaList.Add(trivia);
      return node;
    }

    public static T AddLeadingComment<T>(this T node, string comment) where T : SyntaxNode => node.AddLeadingTrivia<T>(SyntaxFactory.Comment(comment));

    public static T AddTrailingComment<T>(this T node, string comment) where T : SyntaxNode => node.AddTrailingTrivia<T>(SyntaxFactory.Comment(comment));

    public static T AddLeadingTriviaNewLine<T>(this T node) where T : SyntaxNode => node.AddLeadingTrivia<T>(SyntaxFactory.NewLineTrivia());

    public static T AddTrailingTriviaNewLine<T>(this T node) where T : SyntaxNode => node.AddTrailingTrivia<T>(SyntaxFactory.NewLineTrivia());
  }
}
