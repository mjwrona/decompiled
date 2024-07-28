// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage.NodeParameters
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4A32169-9B8B-4726-A9F6-41569B7C3273
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage.dll

using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage
{
  public class NodeParameters
  {
    private IList<NodeItem> m_arguments = (IList<NodeItem>) new List<NodeItem>();

    public double Offset { get; set; }

    public IList<NodeItem> Arguments
    {
      get => this.m_arguments;
      set => this.m_arguments = value ?? (IList<NodeItem>) new List<NodeItem>();
    }
  }
}
