// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.RegexQueryExpressionConstructor
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Query
{
  public class RegexQueryExpressionConstructor : IRegexQueryExpressionConstructor
  {
    public virtual IExpression ConstructRegexQueryExpression(
      IVssRequestContext requestContext,
      TermExpression regexExpression)
    {
      int configValue = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/MaxTrigramCount", TeamFoundationHostType.ProjectCollection, 1000);
      int num = 3;
      if (!regexExpression.IsOfType("regex"))
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("Expected type {0} but found {1}", (object) "regex", (object) regexExpression.Type)));
      Stopwatch timer = Stopwatch.StartNew();
      string simplifyCharacterClass;
      try
      {
        simplifyCharacterClass = this.ParserToSimplifyCharacterClass(regexExpression.Value, num);
      }
      finally
      {
        timer.Stop();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfoConditionally(1083115, "Search Engine", "Query Builder", (Func<string>) (() => FormattableString.Invariant(FormattableStringFactory.Create("RegexQueryConstructorParsingUnit.AddRecords took {0}ms", (object) timer.ElapsedMilliseconds))));
      }
      timer = Stopwatch.StartNew();
      RegexAutomaton startState;
      try
      {
        startState = this.MakeAutomaton(simplifyCharacterClass, num);
        startState = this.RemoveLoopsFromAutomaton(startState);
      }
      finally
      {
        timer.Stop();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfoConditionally(1083116, "Search Engine", "Query Builder", (Func<string>) (() => FormattableString.Invariant(FormattableStringFactory.Create("RegexQueryConstructorMakeAutomatonUnit.AddRecords took {0}ms", (object) timer.ElapsedMilliseconds))));
      }
      timer = Stopwatch.StartNew();
      try
      {
        return this.MakeNGrams(startState, num, configValue);
      }
      finally
      {
        timer.Stop();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfoConditionally(1083117, "Search Engine", "Query Builder", (Func<string>) (() => FormattableString.Invariant(FormattableStringFactory.Create("RegexQueryConstructorMakeNGramsUnit.AddRecords took {0}ms", (object) timer.ElapsedMilliseconds))));
      }
    }

    [SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId = "index+1", Justification = "There is argument check for the value of index to avoid overflow")]
    public int GetExactCountFromString(string regex, int index, int n)
    {
      if (index == int.MaxValue)
        throw new ArgumentOutOfRangeException(nameof (index), FormattableString.Invariant(FormattableStringFactory.Create("Value of index must be less than {0}", (object) int.MaxValue)));
      int exactCountFromString = -1;
      int index1 = index + 1;
      if (regex[index1] != ',')
      {
        int index2 = index1;
        while (regex[index2] >= '0' && regex[index2] <= '9')
          checked { ++index2; }
        if (regex[index2] == '}')
        {
          if (index2 - index1 > 1)
          {
            exactCountFromString = n;
          }
          else
          {
            exactCountFromString = (int) regex[index1] - 48;
            if (exactCountFromString > n)
              exactCountFromString = n;
          }
        }
      }
      return exactCountFromString;
    }

    public int GetMinCountFromString(string regex, int index, int n)
    {
      char ch = index <= 2147483645 ? regex[index + 1] : throw new ArgumentOutOfRangeException(nameof (index), FormattableString.Invariant(FormattableStringFactory.Create("Value of index must be less than {0}", (object) 2147483646)));
      int minCountFromString = -1;
      switch (ch)
      {
        case ',':
          minCountFromString = 0;
          break;
        case '0':
        case '1':
        case '2':
        case '3':
        case '4':
        case '5':
        case '6':
        case '7':
        case '8':
        case '9':
          int index1 = index + 1;
          while (regex[index1] >= '0' && regex[index1] <= '9')
            checked { ++index1; }
          if (regex[index1] == ',')
          {
            if (index1 != index + 2)
            {
              minCountFromString = n;
              break;
            }
            minCountFromString = (int) ch - 48;
            if (minCountFromString > n)
            {
              minCountFromString = n;
              break;
            }
            break;
          }
          break;
      }
      return minCountFromString;
    }

    [SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId = "index+1", Justification = "There is argument check for the value of index to avoid overflow")]
    public int GetMaxCountFromString(string regex, int index, int n)
    {
      int num = index != int.MaxValue ? (int) regex[index + 1] : throw new ArgumentOutOfRangeException(nameof (index), FormattableString.Invariant(FormattableStringFactory.Create("Value of index must be less than {0}", (object) int.MaxValue)));
      int maxCountFromString = -1;
      int index1 = index;
      while (regex[index1] != ',' && regex[index1] != '}')
        checked { ++index1; }
      if (regex[index1] != '}')
      {
        int index2 = checked (index1 + 1);
        if (regex[index2] == '}')
        {
          maxCountFromString = n;
        }
        else
        {
          int index3 = index2;
          while (regex[index3] >= '0' && regex[index3] <= '9')
            checked { ++index3; }
          if (index3 - index2 > 1)
          {
            maxCountFromString = n;
          }
          else
          {
            maxCountFromString = (int) regex[index2] - 48;
            if (maxCountFromString > n)
              maxCountFromString = n;
          }
        }
      }
      return maxCountFromString;
    }

    public string[] GetAlternations(string regex, int index)
    {
      if (index >= regex.Length)
        throw new ArgumentOutOfRangeException(nameof (index), FormattableString.Invariant(FormattableStringFactory.Create("GetAlternations: Indexing out of range. Regex length = {0}, index = {1}", (object) regex.Length, (object) index)));
      string[] alternations = new string[100];
      char ch1 = regex[index];
      bool flag = false;
      int num1 = 0;
      int num2 = -1;
      while (index != 0)
      {
        --index;
        ch1 = regex[index];
        if (ch1 == '(')
        {
          ++num2;
          if (num2 == 0)
            break;
        }
        if (ch1 == ')')
          --num2;
      }
      if (ch1 == '(')
      {
        int num3 = 1;
        while (num3 != 0)
        {
          StringBuilder stringBuilder = new StringBuilder("");
          ++index;
          if (index >= regex.Length)
            throw new SearchException("Unmatched brackets");
          char ch2 = regex[index];
          while (ch2 != '|')
          {
            stringBuilder.Append(ch2);
            ++index;
            if (index < regex.Length)
            {
              ch2 = regex[index];
              if (ch2 == '(')
                ++num3;
              if (ch2 == ')')
              {
                --num3;
                if (num3 == 0)
                  break;
              }
            }
            else
              throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("Syntax of query is not correct. Current nestingLevel = {0} and alternationFound = {1}.", (object) num3, (object) flag)));
          }
          alternations[num1++] = stringBuilder.ToString();
          flag = true;
        }
      }
      else
        alternations = regex.Split('|');
      return alternations;
    }

    public string ParserToSimplifyCharacterClass(string re, int n)
    {
      StringBuilder stringBuilder = new StringBuilder("");
      for (int index1 = 0; index1 < re.Length; ++index1)
      {
        char ch1 = re[index1];
        switch (ch1)
        {
          case '*':
            if (re[index1 - 1] == ']')
            {
              stringBuilder.Append("{0,3}");
              stringBuilder = new StringBuilder(this.ParserToSimplifyCharacterClass(stringBuilder.ToString(), n));
              break;
            }
            stringBuilder.Append(ch1);
            break;
          case '+':
            if (re[index1 - 1] == ']')
            {
              stringBuilder.Append("{1,3}");
              stringBuilder = new StringBuilder(this.ParserToSimplifyCharacterClass(stringBuilder.ToString(), n));
              break;
            }
            stringBuilder.Append(ch1);
            break;
          case '?':
            if (re[index1 - 1] == ']')
            {
              stringBuilder.Append("{0,1}");
              stringBuilder = new StringBuilder(this.ParserToSimplifyCharacterClass(stringBuilder.ToString(), n));
              break;
            }
            stringBuilder.Append(ch1);
            break;
          case '[':
            int index2 = index1;
            while (re[index2] != ']')
            {
              if (re[index2] == '-' && re[index2 - 1] != '[' && re[index2 - 1] != '\\' && re[index2 + 1] != ']')
              {
                char ch2 = (char) ((uint) re[index2 - 1] + 1U);
                char ch3 = re[index2 + 1];
                while ((int) ch2 < (int) ch3)
                  stringBuilder.Append(ch2++);
                ++index2;
              }
              if (re[index2] == '\\' && (re[index2 + 1] == '-' || re[index2 + 1] == '\\'))
              {
                ++index2;
              }
              else
              {
                stringBuilder.Append(re[index2]);
                ++index2;
              }
            }
            stringBuilder.Append(re[index2]);
            index1 = index2;
            break;
          case '{':
            if (re[index1 - 1] == ']')
            {
              int minCountFromString = this.GetMinCountFromString(re, index1, n);
              int maxCountFromString = this.GetMaxCountFromString(re, index1, n);
              int exactCountFromString = this.GetExactCountFromString(re, index1, n);
              int num = index1;
              while (re[num] != '[')
                --num;
              string simplifyCharacterClass = this.ParserToSimplifyCharacterClass(re.Substring(num, index1 - num), n);
              if (exactCountFromString != -1)
              {
                if (exactCountFromString > 1)
                {
                  for (int index3 = exactCountFromString - 1; index3 > 0; --index3)
                    stringBuilder.Append(simplifyCharacterClass);
                }
              }
              else if (minCountFromString != 1 || maxCountFromString != 1)
              {
                stringBuilder.Remove(num, index1 - num);
                stringBuilder.Append('(');
                for (; minCountFromString <= maxCountFromString; ++minCountFromString)
                {
                  for (int index4 = minCountFromString; index4 > 0; --index4)
                    stringBuilder.Append(simplifyCharacterClass);
                  if (minCountFromString != maxCountFromString)
                    stringBuilder.Append('|');
                }
                stringBuilder.Append(')');
              }
              while (re[index1] != '}')
                ++index1;
              break;
            }
            stringBuilder.Append(ch1);
            break;
          default:
            stringBuilder.Append(ch1);
            break;
        }
      }
      return stringBuilder.ToString();
    }

    private IExpression MakeAndNGrams(RegexAutomaton startState, int n)
    {
      StringBuilder stringBuilder = new StringBuilder("");
      int num = 0;
      List<IExpression> set = new List<IExpression>();
      IExpression expression = (IExpression) null;
      RegexAutomaton regexAutomaton1 = startState.NextNonBranchNode();
      bool flag1 = false;
      bool flag2 = true;
      while (!regexAutomaton1.IsStopState())
      {
        RegexAutomaton regexAutomaton2 = regexAutomaton1.NextNonBranchNode();
        if (regexAutomaton1.GetState() == "&")
        {
          regexAutomaton1 = regexAutomaton2;
          if (!regexAutomaton1.IsStopState())
          {
            regexAutomaton2 = regexAutomaton1.NextNonBranchNode();
          }
          else
          {
            flag2 = false;
            flag1 = true;
          }
        }
        while (!regexAutomaton1.IsStopState() && num < n)
        {
          stringBuilder.Append(regexAutomaton1.GetName());
          ++num;
          regexAutomaton1 = regexAutomaton1.NextNonBranchNode();
          if (regexAutomaton1.IsStopState())
          {
            flag1 = true;
            if (num != n)
            {
              flag2 = false;
              break;
            }
          }
          else if (regexAutomaton1.GetState() == "&")
          {
            if (num != n)
            {
              flag2 = false;
              break;
            }
            break;
          }
        }
        num = 0;
        if (flag1)
        {
          if (!flag2 && set.Count == 0)
            return expression;
          if (!flag2 && set.Count > 0)
            return set.Count != 1 ? (IExpression) new AndExpression((IEnumerable<IExpression>) set) : set[0];
          if (flag2 && set.Count == 0)
            return (IExpression) new TermExpression(CodeFileContract.CodeContractQueryableElement.Regex.InlineFilterName(), Operator.Matches, stringBuilder.ToString());
          TermExpression termExpression = new TermExpression(CodeFileContract.CodeContractQueryableElement.Regex.InlineFilterName(), Operator.Matches, stringBuilder.ToString());
          if (!set.Contains((IExpression) termExpression))
            set.Add((IExpression) termExpression);
          return (IExpression) new AndExpression((IEnumerable<IExpression>) set);
        }
        if (regexAutomaton1.GetState() == "&" && !flag2)
        {
          stringBuilder = new StringBuilder("");
          regexAutomaton1 = regexAutomaton2;
          flag2 = true;
        }
        else
        {
          TermExpression termExpression = new TermExpression(CodeFileContract.CodeContractQueryableElement.Regex.InlineFilterName(), Operator.Matches, stringBuilder.ToString());
          if (!set.Contains((IExpression) termExpression))
            set.Add((IExpression) termExpression);
          stringBuilder = new StringBuilder("");
          regexAutomaton1 = regexAutomaton2;
        }
      }
      return expression;
    }

    private RegexAutomaton MakeAutomatonForAndNGram(Stack<RegexAutomaton> AndNGramStack)
    {
      RegexAutomaton[] array = AndNGramStack.ToArray();
      int num = AndNGramStack.Count - 1;
      RegexAutomaton[] regexAutomatonArray = array;
      int index1 = num;
      int index2 = index1 - 1;
      RegexAutomaton regexAutomaton1 = regexAutomatonArray[index1];
      RegexAutomaton regexAutomaton2 = regexAutomaton1;
      while (index2 >= 0)
      {
        RegexAutomaton regexAutomaton3 = array[index2];
        if (regexAutomaton3.GetState() == "X" || regexAutomaton3.GetState() == "G")
        {
          while (regexAutomaton3.GetState() == "X" || regexAutomaton3.GetState() == "G")
            regexAutomaton3 = array[index2--];
        }
        else
          regexAutomaton3 = array[index2--];
        regexAutomaton2 = regexAutomaton2.SetNext(new RegexAutomaton(regexAutomaton3.GetName(), regexAutomaton3.GetState()));
      }
      return regexAutomaton1;
    }

    private IExpression MakeNGrams(RegexAutomaton startState, int sizeOfNgram, int maxTrigramCount)
    {
      Stack<RegexAutomaton> regexAutomatonStack = new Stack<RegexAutomaton>();
      Stack<RegexAutomaton> AndNGramStack = new Stack<RegexAutomaton>();
      AndNGramStack.Push(startState);
      bool flag1 = false;
      IExpression expression1 = (IExpression) null;
      List<IExpression> expressionList = new List<IExpression>();
      RegexAutomaton regexAutomaton = AndNGramStack.Peek();
      bool flag2 = false;
      while (AndNGramStack.Count != 0)
      {
        while (!regexAutomaton.IsStopState())
        {
          if (regexAutomaton.HasBranch)
          {
            foreach (RegexAutomaton branch in regexAutomaton.GetBranches())
            {
              if (regexAutomatonStack.Count == 0 || !regexAutomatonStack.Contains(branch))
              {
                regexAutomaton = branch;
                regexAutomatonStack.Push(branch);
                flag1 = true;
                break;
              }
            }
            if (!flag1)
            {
              flag2 = true;
              for (int index = 0; index < regexAutomaton.GetBranches().Count; ++index)
                regexAutomatonStack.Pop();
              AndNGramStack.Pop();
              break;
            }
            flag1 = false;
          }
          else
            regexAutomaton = regexAutomaton.NextNonBranchNode();
          AndNGramStack.Push(regexAutomaton);
        }
        if (!flag2)
        {
          IExpression expression2 = this.MakeAndNGrams(this.MakeAutomatonForAndNGram(AndNGramStack), sizeOfNgram);
          if (expression2 != null)
          {
            if (expressionList.Count >= maxTrigramCount)
              throw new SearchException(FormattableString.Invariant(FormattableStringFactory.Create("Too many terms generated. Can handle upto {0} trigrams, but found {1}", (object) maxTrigramCount, (object) expressionList.Count)));
            expressionList.Add(expression2);
          }
        }
        flag2 = false;
        regexAutomaton = AndNGramStack.Pop();
        while (!regexAutomaton.HasBranch && AndNGramStack.Count != 0)
          regexAutomaton = AndNGramStack.Pop();
        if (AndNGramStack.Count > 0)
          AndNGramStack.Push(regexAutomaton);
      }
      if (expressionList.Count == 1)
        expression1 = expressionList[0];
      else if (expressionList.Count > 1)
        expression1 = (IExpression) new OrExpression(expressionList.ToArray());
      return expression1;
    }

    private RegexAutomaton RemoveLoopsFromAutomaton(RegexAutomaton startState)
    {
      RegexAutomaton regexAutomaton1 = startState;
      while (!regexAutomaton1.IsStopState() && regexAutomaton1.GetState() != "X")
      {
        if (regexAutomaton1.GetState() == "R")
        {
          int loopCountExact = regexAutomaton1.GetLoopCountExact();
          int loopCountMin = regexAutomaton1.GetLoopCountMin();
          int loopCountMax = regexAutomaton1.GetLoopCountMax();
          RegexAutomaton regexAutomaton2 = new RegexAutomaton("X");
          if (regexAutomaton1.PrevNode().GetState() == "G")
          {
            RegexAutomaton loopBackNode = regexAutomaton1.PrevNode().GetLoopBackNode();
            RegexAutomaton regexAutomaton3 = new RegexAutomaton("S");
            RegexAutomaton regexAutomaton4 = loopBackNode.NextNonBranchNode();
            RegexAutomaton regexAutomaton5 = regexAutomaton3;
            for (; regexAutomaton4.GetState() != "G"; regexAutomaton4 = regexAutomaton4.NextNonBranchNode())
              regexAutomaton5 = regexAutomaton5.SetNext(regexAutomaton4.GetName());
            regexAutomaton5.SetNext(new RegexAutomaton("F"));
            RegexAutomaton nextNonBranch = regexAutomaton1.NextNonBranchNode();
            if (loopCountExact != -1)
            {
              RegexAutomaton regexAutomaton6 = loopBackNode;
              RegexAutomaton regexAutomaton7 = regexAutomaton3.NextNonBranchNode();
              for (; loopCountExact > 0; --loopCountExact)
              {
                for (; !regexAutomaton7.IsStopState(); regexAutomaton7 = regexAutomaton7.NextNonBranchNode())
                  regexAutomaton6 = regexAutomaton6.SetNext(regexAutomaton7.GetName());
                regexAutomaton7 = regexAutomaton3.NextNonBranchNode();
              }
              regexAutomaton6.SetNext(nextNonBranch);
            }
            else
            {
              loopBackNode.HasBranch = true;
              for (; loopCountMin <= loopCountMax; ++loopCountMin)
              {
                RegexAutomaton regexAutomaton8 = loopBackNode;
                RegexAutomaton regexAutomaton9 = regexAutomaton3.NextNonBranchNode();
                RegexAutomaton regexAutomaton10 = loopCountMin == 0 ? regexAutomaton8.AddBranch(regexAutomaton2) : regexAutomaton8.AddBranch(new RegexAutomaton(regexAutomaton9.GetName(), "I"));
                for (int index = loopCountMin; index > 0; --index)
                {
                  for (RegexAutomaton regexAutomaton11 = index != loopCountMin ? regexAutomaton3.NextNonBranchNode() : regexAutomaton3.NextNonBranchNode().NextNonBranchNode(); !regexAutomaton11.IsStopState(); regexAutomaton11 = regexAutomaton11.NextNonBranchNode())
                    regexAutomaton10 = regexAutomaton10.SetNext(regexAutomaton11.GetName());
                }
                if (loopCountMin != 0)
                  regexAutomaton10.SetNext(regexAutomaton2);
              }
              regexAutomaton2.SetNext(nextNonBranch);
            }
          }
          else
          {
            RegexAutomaton regexAutomaton12 = regexAutomaton1.PrevNode().PrevNode();
            RegexAutomaton regexAutomaton13 = regexAutomaton1.PrevNode();
            RegexAutomaton nextNonBranch = regexAutomaton1.NextNonBranchNode();
            if (loopCountExact != -1)
            {
              for (; loopCountExact != 0; --loopCountExact)
                regexAutomaton12 = regexAutomaton12.SetNext(new RegexAutomaton(regexAutomaton13.GetName(), "I"));
              regexAutomaton12.SetNext(nextNonBranch);
            }
            else
            {
              regexAutomaton12.SetNext((RegexAutomaton) null);
              regexAutomaton12.HasBranch = true;
              for (; loopCountMin <= loopCountMax; ++loopCountMin)
              {
                int num = loopCountMin;
                RegexAutomaton regexAutomaton14;
                if (loopCountMin != 0)
                {
                  regexAutomaton14 = regexAutomaton12.AddBranch(new RegexAutomaton(regexAutomaton13.GetName(), "I"));
                  --num;
                }
                else
                  regexAutomaton14 = regexAutomaton12.AddBranch(regexAutomaton2);
                for (; num > 0; --num)
                  regexAutomaton14 = regexAutomaton14.SetNext(new RegexAutomaton(regexAutomaton13.GetName(), "I"));
                if (loopCountMin != 0)
                  regexAutomaton14.SetNext(regexAutomaton2);
              }
              regexAutomaton2.SetNext(nextNonBranch);
            }
          }
        }
        if (!regexAutomaton1.HasBranch)
        {
          regexAutomaton1 = regexAutomaton1.NextNonBranchNode();
        }
        else
        {
          List<RegexAutomaton> branches = regexAutomaton1.GetBranches();
          foreach (RegexAutomaton startState1 in branches)
            this.RemoveLoopsFromAutomaton(startState1);
          int nestingLevel = regexAutomaton1.GetNestingLevel();
          RegexAutomaton regexAutomaton15 = branches[0];
          while (regexAutomaton15.GetNestingLevel() != nestingLevel)
            regexAutomaton15 = !regexAutomaton15.HasBranch ? regexAutomaton15.NextNonBranchNode() : regexAutomaton15.GetBranches()[0];
          regexAutomaton1 = regexAutomaton15.NextNonBranchNode();
        }
      }
      return startState;
    }

    private RegexAutomaton MakeAutomaton(string re, int n)
    {
      RegexAutomaton regexAutomaton1 = new RegexAutomaton("S");
      if (re.Length == 0)
      {
        regexAutomaton1.SetNext(new RegexAutomaton("F"));
        return regexAutomaton1;
      }
      RegexAutomaton node1 = regexAutomaton1;
      int index = 0;
      int count1 = 0;
      char ch = re[index];
      for (; index < re.Length; ++index)
      {
        char name = re[index];
        switch (name)
        {
          case '(':
            ++count1;
            node1 = node1.SetNext(new RegexAutomaton("G"));
            node1.SetNestingLevel(count1);
            break;
          case ')':
            node1 = node1.SetNext(new RegexAutomaton("G"));
            node1.SetNestingLevel(count1);
            RegexAutomaton node2 = node1.PrevNode();
            while (node2.GetNestingLevel() != count1)
              node2 = node2.PrevNode();
            node1.SetLoopBackNode(node2);
            --count1;
            break;
          case '*':
            if (node1.GetState() != "G")
              node1.SetLoopBackNode(node1);
            node1 = node1.SetNext(new RegexAutomaton("R"));
            node1.SetLoopCountMin(0);
            if (node1.PrevNode().GetState() == "G")
            {
              node1.SetLoopCountMax(n - 1);
              break;
            }
            node1.SetLoopCountMax(n);
            break;
          case '+':
            if (node1.GetState() != "G")
              node1.SetLoopBackNode(node1);
            node1 = node1.SetNext(new RegexAutomaton("R"));
            node1.SetLoopCountMin(1);
            if (node1.PrevNode().GetState() == "G")
            {
              node1.SetLoopCountMax(n - 1);
              break;
            }
            node1.SetLoopCountMax(n);
            break;
          case '.':
            node1 = node1.SetNext(new RegexAutomaton("&"));
            for (; name == '.' || name == '*'; name = re[index])
            {
              ++index;
              if (index >= re.Length)
                break;
            }
            --index;
            break;
          case '?':
            if (node1.GetState() != "G")
              node1.SetLoopBackNode(node1);
            node1 = node1.SetNext(new RegexAutomaton("R"));
            node1.SetLoopCountMin(0);
            node1.SetLoopCountMax(1);
            break;
          case '[':
            int count2 = count1 + 1;
            RegexAutomaton regexAutomaton2 = new RegexAutomaton("G");
            regexAutomaton2.IsCharacterClass = true;
            RegexAutomaton regexAutomaton3 = node1.SetNext(regexAutomaton2);
            regexAutomaton3.SetNestingLevel(count2);
            RegexAutomaton nextNonBranch1 = new RegexAutomaton("G");
            nextNonBranch1.SetNestingLevel(count2);
            nextNonBranch1.SetLoopBackNode(regexAutomaton2);
            count1 = count2 - 1;
            ++index;
            RegexAutomaton nextNonBranch2 = new RegexAutomaton("X");
            for (; re[index] != ']'; ++index)
            {
              regexAutomaton3.AddBranch(new RegexAutomaton(re[index], "I")).SetNext(nextNonBranch2);
              regexAutomaton3 = regexAutomaton2;
            }
            nextNonBranch2.SetNext(nextNonBranch1);
            node1 = nextNonBranch1;
            break;
          case '\\':
            ++index;
            node1 = node1.SetNext(re[index]);
            break;
          case '{':
            node1 = node1.SetNext(new RegexAutomaton("R"));
            int minCountFromString = this.GetMinCountFromString(re, index, n);
            int maxCountFromString = this.GetMaxCountFromString(re, index, n);
            int exactCountFromString = this.GetExactCountFromString(re, index, n);
            if (exactCountFromString != -1)
            {
              node1.SetLoopCountExact(exactCountFromString);
            }
            else
            {
              node1.SetLoopCountMin(minCountFromString);
              node1.SetLoopCountMax(maxCountFromString);
            }
            while (re[index] != '}')
              ++index;
            break;
          case '|':
            int num1 = -1;
            int num2 = 0;
            bool flag = false;
            for (; node1.GetState() != "S"; node1 = node1.PrevNode())
            {
              if (node1.GetState() == "G")
              {
                if (node1.GetLoopBackNode() != null)
                {
                  --num1;
                }
                else
                {
                  ++num1;
                  if (num1 == 0)
                    break;
                }
              }
            }
            node1.SetNext((RegexAutomaton) null);
            string[] alternations = this.GetAlternations(re, index);
            RegexAutomaton branchEndNode = new RegexAutomaton("X");
            RegexAutomaton nextNonBranch3 = new RegexAutomaton("G");
            if (node1.IsStartState())
            {
              node1 = node1.SetNext(new RegexAutomaton("G"));
              node1.SetNestingLevel(1);
              num2 = re.Length;
              flag = true;
            }
            foreach (string re1 in alternations)
            {
              if (re1 != null)
              {
                if (!flag && alternations[0] != re1)
                  num2 = num2 + re1.Length + 1;
                RegexAutomaton startNode = this.MakeAutomaton(this.ParserToSimplifyCharacterClass(re1, n), n);
                node1.AppendAutomatonAsBranch(startNode, branchEndNode);
              }
              else
                break;
            }
            int num3 = num2 - 1;
            nextNonBranch3.SetNestingLevel(node1.GetNestingLevel());
            nextNonBranch3.SetLoopBackNode(node1);
            branchEndNode.SetNext(nextNonBranch3);
            node1 = nextNonBranch3;
            int num4 = index + num3;
            if (!flag)
              num4 += 2;
            index = num4 - 1;
            break;
          default:
            node1 = node1.SetNext(name);
            break;
        }
      }
      node1.SetNext(new RegexAutomaton("F"));
      return regexAutomaton1;
    }
  }
}
