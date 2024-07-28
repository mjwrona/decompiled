// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Element
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System.Text;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class Element
  {
    private StringBuilder m_elementSql;
    private int m_outputsExpected;
    private int m_outputIndex;

    public Element(int outputs)
    {
      this.m_elementSql = new StringBuilder();
      this.m_outputsExpected = outputs;
    }

    public void AppendSql(string sql) => this.m_elementSql.Append(sql);

    public string Sql => this.m_elementSql.ToString();

    public int Outputs => this.m_outputsExpected;

    public int OutputIndex
    {
      set => this.m_outputIndex = value;
      get => this.m_outputIndex;
    }
  }
}
