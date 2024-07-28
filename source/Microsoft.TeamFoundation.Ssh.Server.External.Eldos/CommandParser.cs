// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Ssh.Server.External.Eldos.CommandParser
// Assembly: Microsoft.TeamFoundation.Ssh.Server.External.Eldos, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76A7154E-5D66-408C-AA1C-E130B17CCD4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.External.Eldos.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Ssh.Server.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.Ssh.Server.External.Eldos
{
  internal static class CommandParser
  {
    private const string c_layer = "CommandParser";

    public static void ParseCommand(
      IVssRequestContext requestContext,
      string command,
      string virtualPathRoot,
      out GitSshCommandInfo commandInfo,
      string userName,
      int port)
    {
      string command1 = CommandParser.NormalizeCommand(command);
      if (command != command1)
        requestContext.TraceAlways(13001142, TraceLevel.Verbose, "Ssh", nameof (CommandParser), "command: '" + command + "', normalizedCommand: '" + command1 + "'");
      string source = CommandParser.RemovePort22IfBogusScpStyleUrl(requestContext.RequestTracer, command1, port);
      string commandName = source.Split(' ')[0];
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      string str1 = Uri.UnescapeDataString(new string(source.SkipWhile<char>((Func<char, bool>) (x => !char.IsWhiteSpace(x))).SkipWhile<char>(CommandParser.\u003C\u003EO.\u003C0\u003E__IsWhiteSpace ?? (CommandParser.\u003C\u003EO.\u003C0\u003E__IsWhiteSpace = new Func<char, bool>(char.IsWhiteSpace))).ToArray<char>()).Trim('\'').Replace("'", "").Replace("\\", ""));
      if (str1.StartsWith("v3", StringComparison.OrdinalIgnoreCase) || str1.StartsWith("/v3", StringComparison.OrdinalIgnoreCase))
      {
        int commandVersion = 3;
        string str2 = str1.TrimStart('/');
        int index1 = 3;
        int index2 = 4;
        string[] strArray = str2.Split('/');
        if (strArray.Length != 4 && strArray.Length != 5)
          throw new FormatException("Command " + command + " is not in expected format.");
        if (strArray.Length == 4)
        {
          --index2;
          index1 = -1;
        }
        string standardizedCommand = commandName + " '" + str2 + "'";
        commandInfo = new GitSshCommandInfo(standardizedCommand, commandName, strArray[1], strArray[2], strArray[index2], index1 > 0 ? CommandParser.IsLimitedRefs(strArray[index1]) : new bool?(), commandVersion);
      }
      else
      {
        int commandVersion;
        if (str1.IndexOf("/_ssh/", StringComparison.OrdinalIgnoreCase) != -1)
        {
          commandVersion = 2;
          str1 = Regex.Replace(str1, "/_ssh/", "/_git/", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        }
        else
          commandVersion = 1;
        string command2 = commandName + " '" + str1 + "'";
        TeamFoundationExecutionEnvironment executionEnvironment = requestContext.ExecutionEnvironment;
        string collection1;
        string project;
        string repo;
        bool? limitRefs;
        CommandParser.GetPreV3UrlParts(executionEnvironment.IsOnPremisesDeployment, userName, command2, str1, virtualPathRoot, out collection1, out project, out repo, out limitRefs);
        string collection2 = !string.IsNullOrEmpty(collection1) ? collection1 : userName;
        string str3 = string.Empty;
        if (limitRefs.HasValue)
          str3 = !limitRefs.Value ? "/_full" : "/_optimized";
        string standardizedCommand = command2;
        executionEnvironment = requestContext.ExecutionEnvironment;
        if (executionEnvironment.IsHostedDeployment)
          standardizedCommand = commandName + " '/v3/" + collection2 + "/" + project + str3 + "/" + repo + "'";
        commandInfo = new GitSshCommandInfo(standardizedCommand, commandName, collection2, project, repo, limitRefs, commandVersion);
      }
    }

    internal static void GetPreV3UrlParts(
      bool isOnPrem,
      string userName,
      string command,
      string path,
      string virtualPathRoot,
      out string collection,
      out string project,
      out string repo,
      out bool? limitRefs)
    {
      collection = string.Empty;
      if (path.StartsWith(virtualPathRoot, StringComparison.OrdinalIgnoreCase))
        path = path.Substring(virtualPathRoot.Length);
      if (!isOnPrem && path.StartsWith("DefaultCollection", StringComparison.OrdinalIgnoreCase))
        path = path.Substring("DefaultCollection".Length);
      List<string> source1;
      if (path == null)
      {
        source1 = (List<string>) null;
      }
      else
      {
        string[] source2 = path.Split('/');
        if (source2 == null)
        {
          source1 = (List<string>) null;
        }
        else
        {
          IEnumerable<string> source3 = ((IEnumerable<string>) source2).Where<string>((Func<string, bool>) (x => !string.IsNullOrEmpty(x)));
          source1 = source3 != null ? source3.ToList<string>() : (List<string>) null;
        }
      }
      int? index = source1?.FindIndex((Predicate<string>) (x => x.Equals("_git", StringComparison.OrdinalIgnoreCase)));
      int? nullable = index.HasValue ? index : throw new FormatException("Command " + command + " is not in expected format.");
      int num = 0;
      if (!(nullable.GetValueOrDefault() < num & nullable.HasValue))
      {
        IEnumerable<string> source4 = source1.Take<string>(index.Value);
        IEnumerable<string> source5 = source1.Skip<string>(index.Value + 1);
        limitRefs = new bool?();
        switch (source5.Count<string>())
        {
          case 1:
            repo = source5.Single<string>();
            break;
          case 2:
            limitRefs = CommandParser.IsLimitedRefs(source5.First<string>());
            repo = source5.Last<string>();
            break;
          default:
            throw new FormatException("Command " + command + " is not in expected format.");
        }
        if (isOnPrem)
        {
          switch (source4.Count<string>())
          {
            case 1:
              collection = source4.First<string>();
              project = repo;
              break;
            case 2:
              collection = source4.First<string>();
              project = source4.ElementAt<string>(1);
              break;
            default:
              throw new FormatException("Command " + command + " is not in expected format.");
          }
        }
        else
        {
          switch (source4.Count<string>())
          {
            case 0:
              project = repo;
              break;
            case 1:
              project = source4.First<string>();
              break;
            default:
              throw new FormatException("Command " + command + " is not in expected format.");
          }
        }
      }
    }

    private static bool? IsLimitedRefs(string segment)
    {
      if (segment.Equals("_optimized", StringComparison.OrdinalIgnoreCase))
        return new bool?(true);
      if (segment.Equals("_full", StringComparison.OrdinalIgnoreCase))
        return new bool?(false);
      throw new FormatException("Expected _full or _optimized, not '" + segment + "'.");
    }

    private static string RemovePort22IfBogusScpStyleUrl(
      ITraceRequest tracer,
      string command,
      int port)
    {
      if (port != 22)
        return command;
      string[] strArray = command.Split(' ');
      if (strArray.Length < 2)
        return command;
      string str = strArray[1];
      if (str.StartsWith("'22/"))
      {
        tracer.Trace(13001147, TraceLevel.Info, "Ssh", nameof (CommandParser), "Command: PathSpec={0}.", (object) str);
        strArray[1] = "'" + str.Substring("22/".Length);
        tracer.Trace(13001150, TraceLevel.Info, "Ssh", nameof (CommandParser), "Updated PathSpec={0}.", (object) str);
      }
      return string.Join(" ", strArray);
    }

    private static string NormalizeCommand(string command)
    {
      if (command[0] != '"')
        return command;
      if (command[command.Length - 1] != '"')
        throw new ArgumentException("The command " + command + " is not properly formatted since it should end in closing quotes.");
      StringBuilder stringBuilder = new StringBuilder();
      int num1 = 1;
      int num2 = command.Length - 1;
      while (num1 < num2)
      {
        char ch1 = command[num1++];
        switch (ch1)
        {
          case '"':
            throw new ArgumentException("The command " + command + " is not properly formatted since it has an extra quote inside of an already quoted command.");
          case '\\':
            char b0 = command[num1++];
            switch (b0)
            {
              case '"':
              case '\\':
                stringBuilder.Append(b0);
                continue;
              case '0':
              case '1':
              case '2':
              case '3':
                string str1 = command;
                int index1 = num1;
                int num3 = index1 + 1;
                char b1 = str1[index1];
                string str2 = command;
                int index2 = num3;
                num1 = index2 + 1;
                char b2 = str2[index2];
                char ch2;
                if (!CommandParser.ParseOctalTriple(b0, b1, b2, out ch2))
                  throw new ArgumentException("The command " + command + " is not properly formatted because it contains an invalid octal sequence.");
                stringBuilder.Append(ch2);
                continue;
              case 'a':
                stringBuilder.Append('\a');
                continue;
              case 'b':
                stringBuilder.Append('\b');
                continue;
              case 'f':
                stringBuilder.Append('\f');
                continue;
              case 'n':
                stringBuilder.Append('\n');
                continue;
              case 'r':
                stringBuilder.Append('\r');
                continue;
              case 't':
                stringBuilder.Append('\t');
                continue;
              case 'v':
                stringBuilder.Append('\v');
                continue;
              default:
                stringBuilder.Append(b0);
                continue;
            }
          default:
            stringBuilder.Append(ch1);
            continue;
        }
      }
      return stringBuilder.ToString();
    }

    private static bool ParseOctalTriple(char b0, char b1, char b2, out char value)
    {
      if (b0 < '0' || b0 > '3' || b1 < '0' || b1 > '7' || b2 < '0' || b2 > '7')
      {
        value = char.MinValue;
        return false;
      }
      value = (char) ((int) b0 - 48 << 6 | (int) b1 - 48 << 3 | (int) b2 - 48);
      return true;
    }
  }
}
