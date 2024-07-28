// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.RequestHeader
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 929F0284-16B2-4277-9F4A-B615689A77D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.dll

using System.Web.Services.Protocols;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class RequestHeader : SoapHeader
  {
    private string m_id;
    private bool m_useDisambiguatedIdentityString;

    public string Id
    {
      get => this.m_id;
      set => this.m_id = value;
    }

    public bool UseDisambiguatedIdentityString
    {
      get => this.m_useDisambiguatedIdentityString;
      set => this.m_useDisambiguatedIdentityString = value;
    }
  }
}
