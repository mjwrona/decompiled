// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.CommentCommandProvider
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Pipelines.Server.Commands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public static class CommentCommandProvider
  {
    private static readonly IReadOnlyList<ICommentCommand> s_commands = (IReadOnlyList<ICommentCommand>) new List<ICommentCommand>()
    {
      (ICommentCommand) new HelpCommand(),
      (ICommentCommand) new ListCommand(),
      (ICommentCommand) new RunCommand(),
      (ICommentCommand) new WhereCommand()
    };
    private static readonly IDictionary<string, ICommentCommand> s_dictionary = (IDictionary<string, ICommentCommand>) CommentCommandProvider.s_commands.ToDictionary<ICommentCommand, string>((Func<ICommentCommand, string>) (c => c.CommandKeyword), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public static IReadOnlyList<ICommentCommand> GetCommands(string[] keywords = null)
    {
      if (keywords == null)
        return CommentCommandProvider.s_commands;
      List<ICommentCommand> commands = new List<ICommentCommand>();
      foreach (string keyword in keywords)
      {
        ICommentCommand commentCommand;
        if (CommentCommandProvider.s_dictionary.TryGetValue(keyword, out commentCommand))
          commands.Add(commentCommand);
      }
      return (IReadOnlyList<ICommentCommand>) commands;
    }

    public static ICommentCommand GetCommand(string keyword)
    {
      ICommentCommand commentCommand;
      return CommentCommandProvider.s_dictionary.TryGetValue(keyword, out commentCommand) ? commentCommand : (ICommentCommand) null;
    }

    public static bool IsValidCommandName(string commandName, string[] supportedCommands = null) => CommentCommandProvider.GetCommand(commandName) != null && (supportedCommands == null || ((IEnumerable<string>) supportedCommands).FirstOrDefault<string>((Func<string, bool>) (c => string.Equals(commandName, c, StringComparison.OrdinalIgnoreCase))) != null);
  }
}
