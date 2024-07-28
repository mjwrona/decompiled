// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.ChangeSetData
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [SoapType("ChangeSetData")]
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  public class ChangeSetData
  {
    private int m_changeSetId;
    private string m_comment;
    private string m_checkedInBy;
    private string m_changeSetUri;

    public int ChangeSetId
    {
      get => this.m_changeSetId;
      set => this.m_changeSetId = value;
    }

    public string Comment
    {
      get => this.m_comment;
      set => this.m_comment = value;
    }

    public string ChangeSetUri
    {
      get => this.m_changeSetUri;
      set => this.m_changeSetUri = value;
    }

    public string CheckedInBy
    {
      get => this.m_checkedInBy;
      set => this.m_checkedInBy = value;
    }
  }
}
