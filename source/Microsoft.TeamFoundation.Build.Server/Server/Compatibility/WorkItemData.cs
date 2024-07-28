// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.WorkItemData
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [SoapType("WorkItemData")]
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  public class WorkItemData
  {
    private int m_workItemId;
    private string m_workItemUri;
    private string m_title;
    private string m_status;
    private string m_assignedTo;

    public int WorkItemId
    {
      get => this.m_workItemId;
      set => this.m_workItemId = value;
    }

    public string WorkItemUri
    {
      get => this.m_workItemUri;
      set => this.m_workItemUri = value;
    }

    public string Title
    {
      get => this.m_title;
      set => this.m_title = value;
    }

    public string Status
    {
      get => this.m_status;
      set => this.m_status = value;
    }

    public string AssignedTo
    {
      get => this.m_assignedTo;
      set => this.m_assignedTo = value;
    }
  }
}
