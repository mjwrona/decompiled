// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Ssh.Server.Core.GitSshCommandInfo
// Assembly: Microsoft.TeamFoundation.Ssh.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3DF8FBEE-AA1B-4659-8650-E7C7E1E085EB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.Core.dll

namespace Microsoft.TeamFoundation.Ssh.Server.Core
{
  public struct GitSshCommandInfo
  {
    public readonly string StandardizedCommand;
    public readonly string CommandName;
    public readonly string Collection;
    public readonly string Project;
    public readonly string Repo;
    public readonly int CommandVersion;
    public readonly bool? LimitRefs;

    public GitSshCommandInfo(
      string standardizedCommand,
      string commandName,
      string collection,
      string project,
      string repo,
      bool? limitRefs,
      int commandVersion)
    {
      this.StandardizedCommand = standardizedCommand;
      this.CommandName = commandName;
      this.Collection = collection;
      this.Project = project;
      this.Repo = repo;
      this.LimitRefs = limitRefs;
      this.CommandVersion = commandVersion;
    }
  }
}
