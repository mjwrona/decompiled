// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationHostCreationValidState
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public sealed class TeamFoundationHostCreationValidState
  {
    private bool m_isValid;
    private string m_message;
    private static readonly TeamFoundationHostCreationValidState s_valid = new TeamFoundationHostCreationValidState(true, string.Empty);

    public TeamFoundationHostCreationValidState()
      : this(true, string.Empty)
    {
    }

    public TeamFoundationHostCreationValidState(bool isValid, string message)
    {
      this.m_isValid = isValid;
      this.m_message = message;
    }

    public bool IsValid => this.m_isValid;

    public string Message => this.m_message;

    public static TeamFoundationHostCreationValidState Valid => TeamFoundationHostCreationValidState.s_valid;
  }
}
