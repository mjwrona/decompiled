// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.ConnectedUserContext
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

namespace Microsoft.TeamFoundation.Client
{
  public class ConnectedUserContext
  {
    private static string m_namespace = "VisualStudio";
    private static string m_tokenKind = "IdeUser";

    public string Namespace
    {
      get => ConnectedUserContext.m_namespace;
      set => ConnectedUserContext.m_namespace = value;
    }

    public string TokenKind
    {
      get => ConnectedUserContext.m_tokenKind;
      set => ConnectedUserContext.m_tokenKind = value;
    }

    public string ServerUri { get; set; }
  }
}
