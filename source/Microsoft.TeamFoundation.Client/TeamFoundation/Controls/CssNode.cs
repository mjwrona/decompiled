// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Controls.CssNode
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.Controls
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class CssNode
  {
    private string m_name;
    private string m_ID;

    internal CssNode(string name, string id)
    {
      this.m_name = name;
      this.m_ID = id;
    }

    internal string Name
    {
      set => this.m_name = value;
      get => this.m_name;
    }

    internal string ID
    {
      set => this.m_ID = value;
      get => this.m_ID;
    }
  }
}
