// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Parser.LASets
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Azure.Cosmos.Query.Core.Parser
{
  internal class LASets
  {
    private readonly Dictionary<Pair<ATNState, int>, bool> visited = new Dictionary<Pair<ATNState, int>, bool>();
    private readonly bool logParse;
    private readonly bool logClosure;
    private Antlr4.Runtime.Parser parser;
    private CommonTokenStream tokenStream;
    private List<IToken> input;
    private int cursor;
    private HashSet<ATNState> stopStates;
    private HashSet<ATNState> startStates;
    private int entryValue;

    public IntervalSet Compute(Antlr4.Runtime.Parser parser, CommonTokenStream token_stream, int line, int col)
    {
      this.input = new List<IToken>();
      this.parser = parser;
      this.tokenStream = token_stream;
      this.stopStates = new HashSet<ATNState>();
      foreach (ATNState atnState in ((IEnumerable<RuleStopState>) parser.Atn.ruleToStopState).Select<RuleStopState, ATNState>((Func<RuleStopState, ATNState>) (t => parser.Atn.states[t.stateNumber])))
        this.stopStates.Add(atnState);
      this.startStates = new HashSet<ATNState>();
      foreach (ATNState atnState in ((IEnumerable<RuleStartState>) parser.Atn.ruleToStartState).Select<RuleStartState, ATNState>((Func<RuleStartState, ATNState>) (t => parser.Atn.states[t.stateNumber])))
        this.startStates.Add(atnState);
      int index = this.tokenStream.Index;
      this.tokenStream.Seek(0);
      int num = 1;
      IToken token;
      do
      {
        token = this.tokenStream.LT(num++);
        this.input.Add(token);
        this.cursor = token.TokenIndex;
      }
      while (token.Type != -1 && (token.Line < line || token.Column < col));
      this.tokenStream.Seek(index);
      List<List<LASets.Edge>> edgeListList1 = this.EnterState(new LASets.Edge()
      {
        index = 0,
        indexAtTransition = 0,
        to = this.parser.Atn.states[0],
        type = TransitionType.EPSILON
      });
      this.input.RemoveAt(this.input.Count - 1);
      List<List<LASets.Edge>> edgeListList2 = new List<List<LASets.Edge>>();
      if (edgeListList1 != null)
      {
        foreach (List<LASets.Edge> parse in edgeListList1)
        {
          if (this.Validate(parse, this.input))
            edgeListList2.Add(parse);
        }
      }
      List<List<LASets.Edge>> edgeListList3 = edgeListList2;
      if (edgeListList3 != null && this.logClosure)
      {
        foreach (List<LASets.Edge> parse in edgeListList3)
          Console.Error.WriteLine("Path " + this.PrintSingle(parse));
      }
      IntervalSet intervalSet = new IntervalSet(Array.Empty<int>());
      if (edgeListList3 != null)
      {
        foreach (List<LASets.Edge> parse in edgeListList3)
        {
          HashSet<ATNState> single = this.ComputeSingle(parse);
          if (this.logClosure)
            Console.Error.WriteLine("All states for path " + string.Join<ATNState>(" ", (IEnumerable<ATNState>) single.ToList<ATNState>()));
          foreach (ATNState atnState in single)
          {
            foreach (Transition transitions in atnState.TransitionsArray)
            {
              switch (transitions.TransitionType)
              {
                case TransitionType.RULE:
                case TransitionType.PREDICATE:
                case TransitionType.WILDCARD:
                  continue;
                default:
                  if (!transitions.IsEpsilon)
                  {
                    intervalSet.AddAll((IIntSet) transitions.Label);
                    continue;
                  }
                  continue;
              }
            }
          }
        }
      }
      return intervalSet;
    }

    private bool CheckPredicate(PredicateTransition transition) => transition.Predicate.Eval<IToken, ParserATNSimulator>((Recognizer<IToken, ParserATNSimulator>) this.parser, (RuleContext) ParserRuleContext.EmptyContext);

    private List<List<LASets.Edge>> EnterState(LASets.Edge t)
    {
      int num1 = ++this.entryValue;
      int indexAtTransition = t.indexAtTransition;
      int index1 = t.index;
      ATNState to1 = t.to;
      IToken token = this.input[index1];
      if (this.logParse)
        Console.Error.WriteLine("Entry " + num1.ToString() + " State " + to1?.ToString() + " tokenIndex " + index1.ToString() + " " + token.Text);
      if (token.TokenIndex >= this.cursor)
      {
        if (this.logParse)
          Console.Error.Write("Entry " + num1.ToString() + " return ");
        List<List<LASets.Edge>> all_parses = new List<List<LASets.Edge>>()
        {
          new List<LASets.Edge>() { t }
        };
        if (this.logParse)
          Console.Error.WriteLine(this.PrintResult(all_parses));
        return all_parses;
      }
      if (this.visited.ContainsKey(new Pair<ATNState, int>(to1, index1)))
        return (List<List<LASets.Edge>>) null;
      this.visited[new Pair<ATNState, int>(to1, index1)] = true;
      List<List<LASets.Edge>> all_parses1 = new List<List<LASets.Edge>>();
      if (this.stopStates.Contains(to1))
      {
        if (this.logParse)
          Console.Error.Write("Entry " + num1.ToString() + " return ");
        List<List<LASets.Edge>> all_parses2 = new List<List<LASets.Edge>>()
        {
          new List<LASets.Edge>() { t }
        };
        if (this.logParse)
          Console.Error.WriteLine(this.PrintResult(all_parses2));
        return all_parses2;
      }
      foreach (Transition transitions in to1.TransitionsArray)
      {
        List<List<LASets.Edge>> edgeListList1 = (List<List<LASets.Edge>>) null;
        switch (transitions.TransitionType)
        {
          case TransitionType.RULE:
            RuleTransition ruleTransition = (RuleTransition) transitions;
            ATNState target = ruleTransition.target;
            edgeListList1 = this.EnterState(new LASets.Edge()
            {
              from = to1,
              to = ruleTransition.target,
              follow = ruleTransition.followState,
              label = ruleTransition.Label,
              type = ruleTransition.TransitionType,
              index = index1,
              indexAtTransition = index1
            });
            if (edgeListList1 != null && edgeListList1.Count == 0)
              throw new Exception();
            if (edgeListList1 != null)
            {
              List<List<LASets.Edge>> edgeListList2 = new List<List<LASets.Edge>>();
              foreach (List<LASets.Edge> source1 in edgeListList1)
              {
                LASets.Edge edge1 = source1.First<LASets.Edge>();
                source1.Last<LASets.Edge>();
                int num2 = this.stopStates.Contains(edge1.to) ? 1 : 0;
                int index2 = edge1.index;
                int cursor = this.cursor;
                if (num2 == 0)
                {
                  edgeListList2.Add(source1);
                }
                else
                {
                  List<List<LASets.Edge>> edgeListList3 = this.EnterState(new LASets.Edge()
                  {
                    from = edge1.to,
                    to = ruleTransition.followState,
                    label = (IntervalSet) null,
                    type = TransitionType.EPSILON,
                    index = edge1.index,
                    indexAtTransition = edge1.index
                  });
                  if (edgeListList3 != null && edgeListList3.Count == 0)
                    throw new Exception();
                  if (edgeListList3 != null)
                  {
                    foreach (IEnumerable<LASets.Edge> source2 in edgeListList3)
                    {
                      List<LASets.Edge> list = source2.ToList<LASets.Edge>();
                      foreach (LASets.Edge edge2 in source1)
                        list.Add(edge2);
                      edgeListList2.Add(list);
                    }
                  }
                }
              }
              edgeListList1 = edgeListList2;
              break;
            }
            break;
          case TransitionType.PREDICATE:
            if (this.CheckPredicate((PredicateTransition) transitions))
            {
              edgeListList1 = this.EnterState(new LASets.Edge()
              {
                from = to1,
                to = transitions.target,
                label = transitions.Label,
                type = transitions.TransitionType,
                index = index1,
                indexAtTransition = index1
              });
              if (edgeListList1 != null && edgeListList1.Count == 0)
                throw new Exception();
              break;
            }
            break;
          case TransitionType.WILDCARD:
            edgeListList1 = this.EnterState(new LASets.Edge()
            {
              from = to1,
              to = transitions.target,
              label = transitions.Label,
              type = transitions.TransitionType,
              index = index1 + 1,
              indexAtTransition = index1
            });
            if (edgeListList1 != null && edgeListList1.Count == 0)
              throw new Exception();
            break;
          default:
            if (transitions.IsEpsilon)
            {
              edgeListList1 = this.EnterState(new LASets.Edge()
              {
                from = to1,
                to = transitions.target,
                label = transitions.Label,
                type = transitions.TransitionType,
                index = index1,
                indexAtTransition = index1
              });
              if (edgeListList1 != null && edgeListList1.Count == 0)
                throw new Exception();
              break;
            }
            IntervalSet intervalSet = transitions.Label;
            if (intervalSet != null && intervalSet.Count > 0)
            {
              if (transitions.TransitionType == TransitionType.NOT_SET)
                intervalSet = intervalSet.Complement((IIntSet) IntervalSet.Of(1, this.parser.Atn.maxTokenType));
              if (intervalSet.Contains(token.Type))
              {
                edgeListList1 = this.EnterState(new LASets.Edge()
                {
                  from = to1,
                  to = transitions.target,
                  label = transitions.Label,
                  type = transitions.TransitionType,
                  index = index1 + 1,
                  indexAtTransition = index1
                });
                if (edgeListList1 != null && edgeListList1.Count == 0)
                  throw new Exception();
                break;
              }
              break;
            }
            break;
        }
        if (edgeListList1 != null)
        {
          foreach (IEnumerable<LASets.Edge> source in edgeListList1)
          {
            List<LASets.Edge> list = source.ToList<LASets.Edge>();
            if (t != null)
            {
              list.Add(t);
              LASets.Edge edge3 = (LASets.Edge) null;
              foreach (LASets.Edge edge4 in list)
              {
                ATNState to2 = edge4.to;
                if (edge3 != null && edge3.from != to2)
                  Console.Error.WriteLine("Fail " + this.PrintSingle(list));
                edge3 = edge4;
              }
            }
            all_parses1.Add(list);
          }
        }
      }
      if (all_parses1.Count == 0)
        return (List<List<LASets.Edge>>) null;
      if (this.logParse)
      {
        Console.Error.Write("Entry " + num1.ToString() + " return ");
        Console.Error.WriteLine(this.PrintResult(all_parses1));
      }
      return all_parses1;
    }

    private HashSet<ATNState> closure(ATNState start)
    {
      if (start == null)
        throw new Exception();
      HashSet<ATNState> atnStateSet = new HashSet<ATNState>();
      Stack<ATNState> source1 = new Stack<ATNState>();
      source1.Push(start);
      while (source1.Any<ATNState>())
      {
        ATNState atnState = source1.Pop();
        if (!atnStateSet.Contains(atnState))
        {
          atnStateSet.Add(atnState);
          foreach (Transition transitions in atnState.TransitionsArray)
          {
            switch (transitions.TransitionType)
            {
              case TransitionType.RULE:
                RuleTransition ruleTransition = (RuleTransition) transitions;
                ATNState sub_state = ruleTransition.target;
                HashSet<ATNState> source2 = this.closure(sub_state);
                if (source2.Where<ATNState>((Func<ATNState, bool>) (s => this.stopStates.Contains(s) && s.atn == sub_state.atn)).Any<ATNState>())
                {
                  HashSet<ATNState> other = this.closure(ruleTransition.followState);
                  source2.UnionWith((IEnumerable<ATNState>) other);
                }
                using (HashSet<ATNState>.Enumerator enumerator = source2.GetEnumerator())
                {
                  while (enumerator.MoveNext())
                  {
                    ATNState current = enumerator.Current;
                    atnStateSet.Add(current);
                  }
                  continue;
                }
              case TransitionType.PREDICATE:
                if (this.CheckPredicate((PredicateTransition) transitions))
                {
                  if (transitions.target == null)
                    throw new Exception();
                  source1.Push(transitions.target);
                  continue;
                }
                continue;
              case TransitionType.WILDCARD:
                continue;
              default:
                if (transitions.IsEpsilon)
                {
                  if (transitions.target == null)
                    throw new Exception();
                  source1.Push(transitions.target);
                  continue;
                }
                continue;
            }
          }
        }
      }
      return atnStateSet;
    }

    private HashSet<ATNState> ComputeSingle(List<LASets.Edge> parse)
    {
      List<LASets.Edge> list = parse.ToList<LASets.Edge>();
      HashSet<ATNState> single = new HashSet<ATNState>();
      if (this.logClosure)
      {
        Console.Error.WriteLine("Computing closure for the following parse:");
        Console.Error.Write(this.PrintSingle(parse));
        Console.Error.WriteLine();
      }
      if (!list.Any<LASets.Edge>())
        return single;
      LASets.Edge edge1 = list.First<LASets.Edge>();
      if (edge1 == null)
        return single;
      ATNState start = edge1.to;
      if (start == null)
        throw new Exception();
label_8:
      do
      {
        LASets.Edge edge2;
        do
        {
          do
          {
            if (this.logClosure)
              Console.Error.WriteLine("Getting closure of " + start.stateNumber.ToString());
            HashSet<ATNState> source = this.closure(start);
            if (this.logClosure)
              Console.Error.WriteLine("closure " + string.Join<int>(" ", source.Select<ATNState, int>((Func<ATNState, int>) (s => s.stateNumber))));
            bool flag1 = false;
            ATN atn = start.atn;
            int ruleIndex = start.ruleIndex;
            RuleStartState ruleStartState = atn.ruleToStartState[ruleIndex];
            RuleStopState ruleStopState = atn.ruleToStopState[ruleIndex];
            bool flag2 = false;
            foreach (ATNState atnState in source)
            {
              if (!single.Contains(atnState))
              {
                flag2 = true;
                single.Add(atnState);
                if (atnState == ruleStopState)
                  flag1 = true;
              }
            }
            if (flag2 && flag1)
            {
              LASets.Edge edge3;
              do
              {
                if (list.Any<LASets.Edge>())
                {
                  list.RemoveAt(0);
                  if (list.Any<LASets.Edge>())
                    edge3 = list.First<LASets.Edge>();
                  else
                    goto label_8;
                }
                else
                  goto label_8;
              }
              while (ruleStartState != edge3.from);
              list.RemoveAt(0);
            }
            else
              goto label_27;
          }
          while (!list.Any<LASets.Edge>());
          edge2 = list.First<LASets.Edge>();
        }
        while (edge2.from == null);
        start = edge2.follow;
      }
      while (start != null);
      throw new Exception();
label_27:
      return single;
    }

    private bool Validate(List<LASets.Edge> parse, List<IToken> i)
    {
      List<LASets.Edge> list = parse.ToList<LASets.Edge>();
      list.Reverse();
      List<IToken>.Enumerator enumerator1 = this.input.GetEnumerator();
      List<LASets.Edge>.Enumerator enumerator2 = list.GetEnumerator();
      IToken current1;
      IntervalSet label1;
      do
      {
        LASets.Edge current2;
        IntervalSet label2;
        do
        {
          int num = enumerator1.MoveNext() ? 1 : 0;
          current1 = enumerator1.Current;
          if (num != 0)
          {
            bool flag = true;
            while (flag && enumerator2.MoveNext())
            {
              switch (enumerator2.Current.type)
              {
                case TransitionType.INVALID:
                  flag = true;
                  continue;
                case TransitionType.EPSILON:
                  flag = true;
                  continue;
                case TransitionType.RULE:
                  flag = true;
                  continue;
                case TransitionType.PREDICATE:
                  flag = true;
                  continue;
                case TransitionType.ATOM:
                  flag = false;
                  continue;
                case TransitionType.ACTION:
                  flag = true;
                  continue;
                case TransitionType.SET:
                  flag = false;
                  continue;
                case TransitionType.NOT_SET:
                  flag = false;
                  continue;
                case TransitionType.WILDCARD:
                  flag = false;
                  continue;
                case TransitionType.PRECEDENCE:
                  flag = true;
                  continue;
                default:
                  throw new Exception();
              }
            }
            current2 = enumerator2.Current;
            if (current2 == null && current1 == null)
              return true;
            if (current2 == null || current1 == null)
              return false;
            switch (current2.type)
            {
              case TransitionType.ATOM:
                label2 = current2.label;
                continue;
              case TransitionType.SET:
                goto label_25;
              case TransitionType.NOT_SET:
                goto label_23;
              case TransitionType.WILDCARD:
                continue;
              default:
                goto label_27;
            }
          }
          else
            goto label_28;
        }
        while (label2 == null || label2.Count <= 0 || label2.Contains(current1.Type));
        return false;
label_23:
        IntervalSet intervalSet = current2.label.Complement((IIntSet) IntervalSet.Of(1, this.parser.Atn.maxTokenType));
        if (intervalSet != null && intervalSet.Count > 0 && !intervalSet.Contains(current1.Type))
          return false;
        continue;
label_25:
        label1 = current2.label;
      }
      while (label1 == null || label1.Count <= 0 || label1.Contains(current1.Type));
      return false;
label_27:
      throw new Exception();
label_28:
      return true;
    }

    private string PrintSingle(List<LASets.Edge> parse)
    {
      StringBuilder stringBuilder = new StringBuilder();
      List<LASets.Edge> list = parse.ToList<LASets.Edge>();
      list.Reverse();
      foreach (LASets.Edge edge in list)
      {
        string str = string.Empty;
        switch (edge.type)
        {
          case TransitionType.INVALID:
            str = "invalid (eps)";
            break;
          case TransitionType.EPSILON:
            str = "on eps";
            break;
          case TransitionType.RANGE:
            str = "on " + edge.label.ToString() + " ('" + this.input[edge.indexAtTransition].Text + "')";
            break;
          case TransitionType.RULE:
            str = "on " + this.parser.RuleNames[edge.to.ruleIndex] + " (eps)";
            break;
          case TransitionType.PREDICATE:
            str = "on pred (eps)";
            break;
          case TransitionType.ATOM:
            str = "on " + edge.label.ToString() + " ('" + this.input[edge.indexAtTransition].Text + "')";
            break;
          case TransitionType.ACTION:
            str = "on action (eps)";
            break;
          case TransitionType.SET:
            str = "on " + edge.label.ToString() + " ('" + this.input[edge.indexAtTransition].Text + "')";
            break;
          case TransitionType.NOT_SET:
            str = "on not " + edge.label.ToString();
            break;
          case TransitionType.WILDCARD:
            str = "on wildcard ('" + this.input[edge.indexAtTransition].Text + "')";
            break;
          case TransitionType.PRECEDENCE:
            str = "on prec (eps)";
            break;
        }
        stringBuilder.Append(" / " + edge.from?.ToString() + " => " + edge.to?.ToString() + " " + str);
      }
      return stringBuilder.ToString();
    }

    private string PrintResult(List<List<LASets.Edge>> all_parses)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (List<LASets.Edge> allParse in all_parses)
        stringBuilder.Append("||| " + this.PrintSingle(allParse));
      return stringBuilder.ToString();
    }

    private class Edge
    {
      public ATNState from;
      public ATNState to;
      public ATNState follow;
      public TransitionType type;
      public IntervalSet label;
      public int indexAtTransition;
      public int index;
    }
  }
}
