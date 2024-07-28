// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Ssh.Server.Core.TunneledCommandAttribute
// Assembly: Microsoft.TeamFoundation.Ssh.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3DF8FBEE-AA1B-4659-8650-E7C7E1E085EB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.Core.dll

using System;

namespace Microsoft.TeamFoundation.Ssh.Server.Core
{
  public class TunneledCommandAttribute : Attribute
  {
    public string[] SupportedCommands;

    public TunneledCommandAttribute(string supportedCommand) => this.SupportedCommands = new string[1]
    {
      supportedCommand
    };

    public TunneledCommandAttribute(params string[] supportedCommands) => this.SupportedCommands = supportedCommands;
  }
}
