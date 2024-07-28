// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.Charts.VelocityChartInputs
// Assembly: Microsoft.Azure.Boards.Charts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EABADF19-3537-403E-8E3C-4185CE6D1F3B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.Charts.dll

using System.Collections.Generic;

namespace Microsoft.Azure.Boards.Charts
{
  public class VelocityChartInputs : ChartInputs
  {
    private List<string> m_iterations;
    private List<string> m_workItemTypes;
    private List<string> m_completedStates;
    private List<string> m_inProgressStates;

    public List<string> Iterations
    {
      get
      {
        if (this.m_iterations == null)
          this.m_iterations = new List<string>();
        return this.m_iterations;
      }
      set => this.m_iterations = value;
    }

    public List<string> WorkItemTypes
    {
      get
      {
        if (this.m_workItemTypes == null)
          this.m_workItemTypes = new List<string>();
        return this.m_workItemTypes;
      }
      set => this.m_workItemTypes = value;
    }

    public List<string> CompletedStates
    {
      get
      {
        if (this.m_completedStates == null)
          this.m_completedStates = new List<string>();
        return this.m_completedStates;
      }
      set => this.m_completedStates = value;
    }

    public List<string> InProgressStates
    {
      get
      {
        if (this.m_inProgressStates == null)
          this.m_inProgressStates = new List<string>();
        return this.m_inProgressStates;
      }
      set => this.m_inProgressStates = value;
    }

    public string EffortField { get; set; }
  }
}
