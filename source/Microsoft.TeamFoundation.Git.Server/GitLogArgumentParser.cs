// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitLogArgumentParser
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  public static class GitLogArgumentParser
  {
    public static GitLogArguments Parse(string allArgs) => GitLogArgumentParser.Parse(GitLogArgumentParser.SplitArgs(allArgs).ToArray<string>());

    public static IEnumerable<string> SplitArgs(string allArgs)
    {
      int startIndex = 0;
      int inspectPos = 0;
      bool isSpecial = false;
      for (; inspectPos < allArgs.Length; ++inspectPos)
      {
        if (allArgs[inspectPos] == ' ')
        {
          if (isSpecial)
          {
            isSpecial = false;
          }
          else
          {
            if (startIndex < inspectPos)
              yield return allArgs.Substring(startIndex, inspectPos - startIndex);
            startIndex = inspectPos + 1;
          }
        }
        else if (allArgs[inspectPos] == '\\')
          isSpecial = !isSpecial;
      }
      if (startIndex < inspectPos)
        yield return allArgs.Substring(startIndex, inspectPos - startIndex);
    }

    public static GitLogArguments Parse(params string[] args)
    {
      GitLogArguments gitLogArguments = new GitLogArguments();
      for (int index = 0; index < args.Length; ++index)
      {
        string str = args[index];
        if (str != null)
        {
          switch (str.Length)
          {
            case 0:
              continue;
            case 2:
              if (str == "--")
              {
                if (index < args.Length - 1)
                {
                  gitLogArguments.Path = args[index + 1].Replace("\\ ", " ");
                  continue;
                }
                continue;
              }
              break;
            case 7:
              if (str == "--graph")
              {
                gitLogArguments.Order = CommitOrder.TopoOrder;
                gitLogArguments.RewriteParents = true;
                continue;
              }
              break;
            case 9:
              if (str == "--parents")
              {
                gitLogArguments.RewriteParents = true;
                continue;
              }
              break;
            case 12:
              switch (str[2])
              {
                case 'd':
                  if (str == "--date-order")
                  {
                    gitLogArguments.Order = CommitOrder.DateOrder;
                    continue;
                  }
                  break;
                case 't':
                  if (str == "--topo-order")
                  {
                    gitLogArguments.Order = CommitOrder.TopoOrder;
                    continue;
                  }
                  break;
              }
              break;
            case 14:
              switch (str[3])
              {
                case 'e':
                  if (str == "--remove-empty")
                  {
                    gitLogArguments.StopAtAdds = true;
                    continue;
                  }
                  break;
                case 'i':
                  if (str == "--first-parent")
                  {
                    gitLogArguments.HistoryMode = GitLogHistoryMode.FirstParent;
                    continue;
                  }
                  break;
                case 'u':
                  if (str == "--full-history")
                  {
                    if (gitLogArguments.HistoryMode != GitLogHistoryMode.FullHistorySimplifyMerges)
                    {
                      gitLogArguments.HistoryMode = GitLogHistoryMode.FullHistory;
                      continue;
                    }
                    continue;
                  }
                  break;
              }
              break;
            case 17:
              if (str == "--simplify-merges")
              {
                gitLogArguments.HistoryMode = GitLogHistoryMode.FullHistorySimplifyMerges;
                continue;
              }
              break;
          }
        }
        if (args[index].StartsWith("--since=") || args[index].StartsWith("--after="))
        {
          DateTime result;
          if (DateTime.TryParse(args[index].Substring(8).Replace("\\ ", " "), out result))
            gitLogArguments.FromDate = new DateTime?(result);
        }
        else if (args[index].StartsWith("--until=") || args[index].StartsWith("--before="))
        {
          DateTime result;
          if (DateTime.TryParse(args[index].Substring(args[index].IndexOf('=') + 1).Replace("\\ ", " "), out result))
            gitLogArguments.ToDate = new DateTime?(result);
        }
        else if (args[index].StartsWith("--author="))
          gitLogArguments.Author = args[index].Substring(9).Replace("\\ ", " ");
        else if (args[index].StartsWith("--committer="))
          gitLogArguments.Committer = args[index].Substring(12).Replace("\\ ", " ");
        else if (args[index].StartsWith("--skip="))
        {
          int result;
          if (int.TryParse(args[index].Substring(7), out result))
            gitLogArguments.Skip = new int?(result);
        }
        else if (args[index].StartsWith("--max-count="))
        {
          int result;
          if (int.TryParse(args[index].Substring(12), out result))
            gitLogArguments.MaxCount = new int?(result);
        }
        else if (args[index] == "-n" && index < args.Length - 1)
        {
          int result;
          if (int.TryParse(args[index + 1], out result))
            gitLogArguments.MaxCount = new int?(result);
        }
        else
        {
          int result;
          if (args[index][0] == '-' && int.TryParse(args[index].Substring(1), out result))
            gitLogArguments.MaxCount = new int?(result);
        }
      }
      return gitLogArguments;
    }
  }
}
