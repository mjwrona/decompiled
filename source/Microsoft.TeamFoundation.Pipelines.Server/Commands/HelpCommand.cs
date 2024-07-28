// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.Commands.HelpCommand
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Pipelines.Server.Providers;
using Microsoft.VisualStudio.Services.ExternalEvent;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Pipelines.Server.Commands
{
  internal class HelpCommand : ICommentCommand
  {
    private static readonly string s_commandDescription = "Get descriptions, examples and documentation about supported commands";
    private static readonly string s_commandExample = "help \"command_name\"";
    private const string c_commandDocumentationUrl = "https://go.microsoft.com/fwlink/?linkid=2056279";

    public string CommandKeyword => CommandNames.Help;

    public string ShortDescription => HelpCommand.s_commandDescription;

    public string ExampleUsage => HelpCommand.s_commandExample;

    public bool IsValid(
      IVssRequestContext requestContext,
      string providerId,
      JObject authentication,
      ExternalPullRequestCommentEvent commentEvent,
      MergeJobStatus mergeJobStatus,
      out string responseMessage)
    {
      responseMessage = string.Empty;
      return true;
    }

    public bool TryExecute(
      IVssRequestContext requestContext,
      string providerId,
      JObject authentication,
      ExternalPullRequestCommentEvent commentEvent,
      MergeJobStatus mergeJobStatus,
      out string responseMessage,
      out List<Exception> exceptions)
    {
      exceptions = new List<Exception>();
      IPipelineSourceProvider provider = requestContext.GetService<IPipelineSourceProviderService>().GetProvider(providerId);
      responseMessage = this.GetHelpCommandResponse(commentEvent.Command.RemainingParameters, provider);
      return true;
    }

    private string GetHelpCommandResponse(string arguments, IPipelineSourceProvider provider)
    {
      CommentResponseBuilder builder = new CommentResponseBuilder();
      if (string.IsNullOrWhiteSpace(arguments))
      {
        IReadOnlyList<ICommentCommand> commands = CommentCommandProvider.GetCommands();
        this.AddAllCommandsInformation(builder, commands);
      }
      else
      {
        string keyword = arguments.Trim().Split(new char[1]
        {
          ' '
        }, 2)[0];
        ICommentCommand command = CommentCommandProvider.GetCommand(keyword);
        if (command != null)
        {
          builder.StartList();
          this.AddCommandInformation(builder, command);
          builder.EndList();
        }
        else
        {
          IReadOnlyList<ICommentCommand> commands = CommentCommandProvider.GetCommands();
          builder.AppendLine("Command '" + keyword + "' is not supported by Azure Pipelines.").AppendLine();
          this.AddAllCommandsInformation(builder, commands);
        }
      }
      builder.AppendLine().AppendText("See ").AppendLink("https://go.microsoft.com/fwlink/?linkid=2056279", "additional documentation.");
      return builder.ToString();
    }

    private void AddAllCommandsInformation(
      CommentResponseBuilder builder,
      IReadOnlyList<ICommentCommand> commands)
    {
      builder.AppendText("Supported commands", true).AppendLine().StartList();
      foreach (ICommentCommand command in (IEnumerable<ICommentCommand>) commands)
        this.AddCommandInformation(builder, command);
      builder.EndList();
    }

    private void AddCommandInformation(CommentResponseBuilder builder, ICommentCommand command) => builder.AppendListItem(command.CommandKeyword + ":", true).StartList().AppendListItem(command.ShortDescription).StartListItem().AppendText("Example: ", true).AppendText(command.ExampleUsage).EndListItem().EndList();
  }
}
