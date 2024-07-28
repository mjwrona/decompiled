// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ElementGroup
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class ElementGroup
  {
    private List<Element> m_elements;

    public ElementGroup() => this.m_elements = new List<Element>();

    public int AddElementToGroup(int outputsExpected)
    {
      this.m_elements.Add(new Element(outputsExpected));
      return this.m_elements.Count - 1;
    }

    public void AppendSql(int index, string newSql) => this.m_elements[index].AppendSql(newSql);

    public int ElementCount => this.m_elements.Count;

    public string GetSql(int index) => this.m_elements[index].Sql;

    public int GetExpectedOutputs(int index) => this.m_elements[index].Outputs;

    public void PutOutputIndex(int index, int outputIndex) => this.m_elements[index].OutputIndex = outputIndex;

    public int GetOutputIndex(int index) => this.m_elements[index].OutputIndex;

    public Element GetElement(int index) => this.m_elements[index];
  }
}
