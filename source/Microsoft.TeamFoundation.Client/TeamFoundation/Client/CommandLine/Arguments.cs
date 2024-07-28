// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.CommandLine.Arguments
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Microsoft.TeamFoundation.Client.CommandLine
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class Arguments
  {
    public List<string> FreeArguments;
    private string m_commandName;
    private List<Option> m_options;

    public Arguments(string commandName)
    {
      this.m_commandName = commandName;
      this.FreeArguments = new List<string>();
      this.m_options = new List<Option>();
    }

    public bool Contains(Microsoft.TeamFoundation.Client.CommandLine.Options.ID optionID) => this.FindOptionByID(optionID) != null;

    public string GetOptionValue(Microsoft.TeamFoundation.Client.CommandLine.Options.ID optionID) => this.GetOptionValue(optionID, 0);

    private string GetOptionValue(Microsoft.TeamFoundation.Client.CommandLine.Options.ID optionID, int index)
    {
      Option option = this.GetOption(optionID);
      if (option == null)
        return (string) null;
      return option.Values.Count <= index ? (string) null : option.Values[index];
    }

    public string[] GetAllOptionValues(Microsoft.TeamFoundation.Client.CommandLine.Options.ID optionID)
    {
      Option option = this.GetOption(optionID);
      return option == null ? Array.Empty<string>() : option.Values.ToArray();
    }

    public Option[] GetAllOptionsByID(Microsoft.TeamFoundation.Client.CommandLine.Options.ID optionID)
    {
      List<Option> optionList = new List<Option>();
      foreach (Option option in this.m_options)
      {
        if (option.ID == optionID)
          optionList.Add(option);
      }
      return optionList.ToArray();
    }

    public Option GetOption(Microsoft.TeamFoundation.Client.CommandLine.Options.ID optionID) => this.FindOptionByID(optionID);

    public void AppendOption(Option option) => this.m_options.Add(option);

    private Option FindOptionByID(Microsoft.TeamFoundation.Client.CommandLine.Options.ID optionID)
    {
      foreach (Option option in this.m_options)
      {
        if (option.ID == optionID)
          return option;
      }
      return (Option) null;
    }

    public string GetFreeArgument(int index) => this.FreeArguments.Count <= index ? (string) null : this.FreeArguments[index];

    public string[] GetFreeArgumentsAsArray() => this.FreeArguments.ToArray();

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("Command: ");
      stringBuilder.Append(this.m_commandName);
      stringBuilder.Append(Environment.NewLine);
      stringBuilder.Append("Options: ");
      foreach (Option option in this.m_options)
      {
        stringBuilder.Append(option.InvokedAs);
        stringBuilder.Append(',');
      }
      stringBuilder.Append(Environment.NewLine);
      stringBuilder.Append("Free arguments: ");
      foreach (string freeArgument in this.FreeArguments)
      {
        stringBuilder.Append(freeArgument);
        stringBuilder.Append(',');
      }
      stringBuilder.Append(Environment.NewLine);
      return stringBuilder.ToString();
    }

    public string CommandName => this.m_commandName;

    public IEnumerable<Option> Options => (IEnumerable<Option>) this.m_options;
  }
}
