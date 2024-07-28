// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationHostReadyState
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public sealed class TeamFoundationHostReadyState
  {
    private bool m_isReady;
    private string m_message;
    private string m_details;
    private static readonly TeamFoundationHostReadyState s_ready = new TeamFoundationHostReadyState(true, string.Empty, string.Empty);

    public TeamFoundationHostReadyState()
      : this(true, string.Empty, string.Empty)
    {
    }

    public TeamFoundationHostReadyState(bool isReady, string message, string details)
    {
      this.m_isReady = isReady;
      this.m_message = message;
      this.m_details = details;
    }

    public bool IsReady => this.m_isReady;

    public string Message => this.m_message;

    public string Details => this.m_details;

    public static TeamFoundationHostReadyState Ready => TeamFoundationHostReadyState.s_ready;
  }
}
