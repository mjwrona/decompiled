// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.ClientContext
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;

namespace Microsoft.TeamFoundation.Client
{
  public class ClientContext : IClientContext
  {
    private Guid m_id;
    private string m_command;

    public ClientContext() => this.m_id = Guid.NewGuid();

    public ClientContext(string command)
      : this()
    {
      this.m_command = command;
    }

    public Guid Id => this.m_id;

    public string Command
    {
      get => this.m_command;
      set => this.m_command = value;
    }
  }
}
