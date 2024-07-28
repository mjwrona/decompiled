// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.CommandLine.ArgumentParser
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Internal;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Client.CommandLine
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ArgumentParser
  {
    private Options.ID[] m_optionIDs;
    private Arguments m_arguments;

    public ArgumentParser(Options.ID[] globalOptionIDs, Options.ID[] localOptionIDs) => this.m_optionIDs = TFCommonUtil.CombineArrays<Options.ID>(globalOptionIDs, localOptionIDs);

    public void Parse(string commandName, int start, string[] rawArguments)
    {
      this.m_arguments = new Arguments(commandName);
      for (int index1 = start; index1 < rawArguments.Length; ++index1)
      {
        string rawArgument = rawArguments[index1];
        if (rawArgument.Length != 0)
        {
          if (rawArgument[0] != '/' && rawArgument[0] != '-')
          {
            this.m_arguments.FreeArguments.Add(rawArgument);
          }
          else
          {
            int num = rawArgument.IndexOf(":", StringComparison.Ordinal);
            string str = num <= 0 ? rawArgument.Substring(1) : rawArgument.Substring(1, num - 1);
            if (TFStringComparer.CommandLineOptionName.Equals(str, string.Empty))
              throw new Command.ArgumentListException(ClientResources.UnrecognizedOption((object) rawArgument));
            Options.ID id;
            Options.Style style;
            Options.GetIDAndStyle(str, out id, out style);
            if (id == Options.ID.UnknownOption)
            {
              string alias = str.Substring(0, 1);
              if (num == 2)
                Options.GetIDAndStyle(alias, out id, out style);
              throw new Command.ArgumentListException(ClientResources.UnrecognizedOption((object) str));
            }
            int index2 = 0;
            if (this.m_optionIDs != null)
            {
              while (index2 < this.m_optionIDs.Length && this.m_optionIDs[index2] != id)
                ++index2;
              if (this.m_optionIDs == null || index2 >= this.m_optionIDs.Length)
                throw new Command.ArgumentListException(ClientResources.UnrecognizedOption((object) str));
            }
            Option option = !this.m_arguments.Contains(id) || Options.GetOccurrencesFromID(id) == Options.Occurrences.Multiple ? new Option(id, str) : throw new Command.ArgumentListException(ClientResources.DuplicateOption((object) str));
            if (num > 0)
              this.ParseValues(id, style, str, option, rawArgument, num + 1);
            else if (style != Options.Style.NoValue)
              throw new Command.ArgumentListException(ClientResources.MissingOptionValue((object) str));
            this.m_arguments.AppendOption(option);
          }
        }
      }
    }

    private void ParseValues(
      Options.ID id,
      Options.Style style,
      string invokedAs,
      Option option,
      string argument,
      int startIndex)
    {
      if (style == Options.Style.NoValue)
        throw new Command.ArgumentListException(ClientResources.DoesNotAcceptAValue((object) invokedAs));
      if (startIndex >= argument.Length)
      {
        if (style != Options.Style.String)
          throw new Command.ArgumentListException(ClientResources.MissingOptionValue((object) invokedAs));
        option.Values.Add(string.Empty);
      }
      else
      {
        switch (style)
        {
          case Options.Style.OneValue:
            option.Values.Add(argument.Substring(startIndex));
            break;
          case Options.Style.TwoValues:
            int num1 = argument.IndexOf(',');
            if (num1 == -1)
            {
              option.Values.Add(argument.Substring(startIndex));
              break;
            }
            if (num1 - startIndex == 0)
              throw new Command.ArgumentListException(ClientResources.ExtraCommaInOption((object) invokedAs));
            option.Values.Add(argument.Substring(startIndex, num1 - startIndex));
            option.Values.Add(argument.Substring(num1 + 1));
            break;
          case Options.Style.MultipleValues:
            int num2;
            for (; (num2 = argument.IndexOf(',', startIndex)) != -1; startIndex = num2 + 1)
            {
              if (num2 - startIndex == 0)
                throw new Command.ArgumentListException(ClientResources.ExtraCommaInOption((object) invokedAs));
              option.Values.Add(argument.Substring(startIndex, num2 - startIndex));
            }
            if (argument.Substring(startIndex).Length == 0)
              throw new Command.ArgumentListException(ClientResources.ExtraCommaInOption((object) invokedAs));
            option.Values.Add(argument.Substring(startIndex));
            break;
          case Options.Style.String:
            option.Values.Add(argument.Substring(startIndex));
            break;
          case Options.Style.NameValuePair:
            int num3 = argument.IndexOf('=');
            if (num3 == -1)
            {
              option.Values.Add(argument.Substring(startIndex));
              break;
            }
            if (num3 - startIndex == 0)
              throw new Command.ArgumentListException(ClientResources.ExtraCommaInOption((object) invokedAs));
            option.Values.Add(argument.Substring(startIndex, num3 - startIndex));
            option.Values.Add(argument.Substring(num3 + 1));
            break;
        }
      }
    }

    public Arguments Arguments => this.m_arguments;
  }
}
