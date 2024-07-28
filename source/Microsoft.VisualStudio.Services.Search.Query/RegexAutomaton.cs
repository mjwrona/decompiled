// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.RegexAutomaton
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Exceptions;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Query
{
  public class RegexAutomaton
  {
    private char m_name;
    private string m_state;
    private List<RegexAutomaton> m_branches;
    private RegexAutomaton m_next;
    private RegexAutomaton m_prev;
    private RegexAutomaton m_loopBackNode;
    private int m_loopCountExact;
    private int m_loopCountMin;
    private int m_loopCountMax;
    private int m_nestingLevel;

    public bool HasBranch { get; set; }

    public bool IsCharacterClass { get; set; }

    public RegexAutomaton(string state)
    {
      this.SetState(state);
      this.m_branches = new List<RegexAutomaton>();
      this.HasBranch = false;
      this.IsCharacterClass = false;
    }

    public RegexAutomaton(char name, string state)
    {
      this.SetName(name);
      this.SetState(state);
      this.m_branches = new List<RegexAutomaton>();
      this.HasBranch = false;
      this.IsCharacterClass = false;
    }

    public void SetNestingLevel(int count) => this.m_nestingLevel = count;

    public int GetNestingLevel() => this.m_nestingLevel;

    public void SetLoopBackNode(RegexAutomaton node) => this.m_loopBackNode = node;

    public RegexAutomaton GetLoopBackNode() => this.m_loopBackNode;

    public void SetLoopCountExact(int count)
    {
      this.m_loopCountExact = count;
      this.m_loopCountMin = -1;
      this.m_loopCountMax = -1;
    }

    public int GetLoopCountExact() => this.m_loopCountExact;

    public void SetLoopCountMin(int count)
    {
      this.m_loopCountExact = -1;
      this.m_loopCountMin = count;
    }

    public int GetLoopCountMin() => this.m_loopCountMin;

    public void SetLoopCountMax(int count)
    {
      this.m_loopCountExact = -1;
      this.m_loopCountMax = count;
    }

    public int GetLoopCountMax() => this.m_loopCountMax;

    public List<RegexAutomaton> GetBranches() => this.m_branches;

    public string GetState() => this.m_state;

    public void SetState(string state) => this.m_state = state;

    public void SetName(char name) => this.m_name = name;

    public char GetName() => this.m_name;

    public void AppendAutomatonAsBranch(RegexAutomaton startNode, RegexAutomaton branchEndNode)
    {
      this.HasBranch = true;
      this.m_next = (RegexAutomaton) null;
      RegexAutomaton regexAutomaton1 = startNode.NextNonBranchNode();
      if (regexAutomaton1 != null && !regexAutomaton1.IsStopState())
      {
        this.m_branches.Add(regexAutomaton1);
        RegexAutomaton regexAutomaton2;
        for (regexAutomaton2 = regexAutomaton1; !regexAutomaton2.IsStopState(); regexAutomaton2 = !regexAutomaton2.HasBranch ? regexAutomaton2.NextNonBranchNode() : regexAutomaton2.GetBranches()[0])
        {
          if (regexAutomaton2.GetState() == "G")
            regexAutomaton2.SetNestingLevel(regexAutomaton2.GetNestingLevel() + 1);
        }
        regexAutomaton2.PrevNode().SetNext(branchEndNode);
      }
      else
        this.m_branches.Add(branchEndNode);
    }

    public RegexAutomaton AddBranch(RegexAutomaton branchNode)
    {
      this.HasBranch = true;
      this.m_next = (RegexAutomaton) null;
      this.m_branches.Add(branchNode);
      branchNode.SetPrev(this);
      return branchNode;
    }

    public bool IsStartState() => this.m_state == "S";

    public bool IsStopState() => this.m_state == "F";

    public RegexAutomaton NextNonBranchNode() => this.m_next != null ? this.m_next : throw new SearchException();

    public RegexAutomaton SetNext(RegexAutomaton nextNonBranch)
    {
      this.m_next = nextNonBranch;
      nextNonBranch?.SetPrev(this);
      return this.m_next;
    }

    public RegexAutomaton SetNext(char name)
    {
      RegexAutomaton regexAutomaton = new RegexAutomaton(name, "I");
      this.m_next = regexAutomaton;
      regexAutomaton.m_prev = this;
      return regexAutomaton;
    }

    public void SetPrev(RegexAutomaton node) => this.m_prev = node;

    public RegexAutomaton PrevNode() => this.m_prev != null ? this.m_prev : throw new SearchException();
  }
}
