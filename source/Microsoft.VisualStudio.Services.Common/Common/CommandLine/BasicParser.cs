// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.CommandLine.BasicParser
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using Microsoft.VisualStudio.Services.Common.CommandLine.Validation;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Common.CommandLine
{
  public class BasicParser : OptionParser
  {
    private PositionalOption positionalOption;
    private Argument lastOptionArgument;
    private OptionReader responseFileReader;
    private Collection<Argument> parsedArguments;

    public BasicParser()
    {
    }

    public BasicParser(OptionReader responseFileRetriever) => this.responseFileReader = responseFileRetriever;

    protected OptionReader ResponseFileReader => this.responseFileReader;

    public override IEnumerable<Argument> Parse(
      IEnumerable<string> commandLine,
      IEnumerable<Option> options)
    {
      return this.Parse(commandLine, options, (IEnumerable<IOptionValidation>) null);
    }

    public override IEnumerable<Argument> Parse(
      IEnumerable<string> commandLine,
      IEnumerable<Option> options,
      IEnumerable<IOptionValidation> optionValidators)
    {
      if (options == null)
        throw new ArgumentNullException(nameof (options));
      this.lastOptionArgument = (Argument) null;
      this.positionalOption = (PositionalOption) null;
      if (this.parsedArguments != null)
      {
        this.parsedArguments.Clear();
        this.parsedArguments = (Collection<Argument>) null;
      }
      if (commandLine != null)
      {
        this.parsedArguments = new Collection<Argument>();
        PositionalOption positionalOption;
        if (this.ArePositionalArgumentsSupported(options, out positionalOption))
          this.positionalOption = positionalOption;
        IEnumerable<string> args = this.ExpandOptions(commandLine, options);
        if (Option.HasHelpOption(args))
        {
          this.parsedArguments.Add(new Argument(Option.CreateHelpOption()));
        }
        else
        {
          foreach (string str in args)
          {
            if (this.IsOption(str, options))
            {
              this.SetLastOptionDefaultValue();
              if (!this.DeriveOptionRun(str, options))
                this.DeriveOption(str, options);
            }
            else
            {
              if (!this.SetLastOptionValue(str))
                this.DerivePositionalOption(str);
              this.lastOptionArgument = (Argument) null;
            }
          }
        }
        if (this.parsedArguments != null)
        {
          foreach (Argument parsedArgument in this.parsedArguments)
          {
            if (parsedArgument != null && parsedArgument.Option != null && parsedArgument.Option.Converter != null && parsedArgument.Value != null && parsedArgument.Option != null)
              parsedArgument.Value = parsedArgument.Option.Converter.Convert(parsedArgument.Value.ToString());
          }
          if (optionValidators != null)
          {
            foreach (IOptionValidation optionValidator in optionValidators)
              optionValidator.Validate(options, (IEnumerable<Argument>) this.parsedArguments);
          }
        }
      }
      return (IEnumerable<Argument>) new List<Argument>((IEnumerable<Argument>) this.parsedArguments);
    }

    private bool ArePositionalArgumentsSupported(
      IEnumerable<Option> options,
      out PositionalOption positionalOption)
    {
      positionalOption = options.FirstOrDefault<Option>((Func<Option, bool>) (o => o is PositionalOption)) as PositionalOption;
      return positionalOption != null;
    }

    private bool DeriveOption(string value, IEnumerable<Option> options)
    {
      bool flag = false;
      if (this.parsedArguments != null && Option.HasSwitch(value))
      {
        string option = this.ParseOption(value);
        Argument obj = new Argument(options.Get(option) ?? throw new ArgumentException(CommonResources.ErrorOptionNotRecognized((object) option)));
        this.parsedArguments.Add(obj);
        this.lastOptionArgument = obj;
        flag = true;
      }
      return flag;
    }

    private bool DeriveOptionRun(string value, IEnumerable<Option> options)
    {
      bool flag = false;
      if (this.parsedArguments != null && value != null && options != null)
      {
        Collection<Option> optionRun;
        flag = this.IsOptionRun(value, options, out optionRun);
        if (flag && optionRun != null)
        {
          foreach (Option description in optionRun)
            this.parsedArguments.Add(new Argument(description, OptionMetadata.OptionRun));
          this.lastOptionArgument = this.parsedArguments.Last<Argument>();
        }
      }
      return flag;
    }

    protected virtual bool DerivePositionalOption(string value)
    {
      bool flag = false;
      if (this.positionalOption != null)
      {
        this.parsedArguments.Add(new Argument((Option) this.positionalOption, (object) BasicParser.TrimValue(value), OptionMetadata.Positional));
        flag = true;
      }
      return flag;
    }

    private void ExpandInlineOption(
      string commandLineValue,
      Collection<string> expandedOptions,
      IEnumerable<Option> options)
    {
      if (this.IsOption(commandLineValue, options))
      {
        string optionArgument = this.ParseOptionArgument(commandLineValue);
        if (string.IsNullOrEmpty(optionArgument))
        {
          expandedOptions.Add(commandLineValue);
        }
        else
        {
          string option = this.ParseOption(commandLineValue, true);
          expandedOptions.Add(option);
          expandedOptions.Add(optionArgument);
        }
      }
      else
        expandedOptions.Add(commandLineValue);
    }

    private IEnumerable<string> ExpandOptions(
      IEnumerable<string> commandLine,
      IEnumerable<Option> options)
    {
      Collection<string> collection = (Collection<string>) null;
      if (commandLine != null)
      {
        collection = new Collection<string>();
        foreach (string commandLineValue in commandLine)
        {
          if (this.IsResponseFileOption(commandLineValue))
            this.ExpandResponseFileOption(commandLineValue, collection, options);
          else
            this.ExpandInlineOption(commandLineValue, collection, options);
        }
      }
      return CommandLineLexer.Escape((IEnumerable<string>) collection).Lex();
    }

    private void ExpandResponseFileOption(
      string commandLineValue,
      Collection<string> expandedOptions,
      IEnumerable<Option> options)
    {
      IEnumerable<string> strings = this.ResponseFileReader != null ? this.ResponseFileReader.GetOptions(commandLineValue) : throw new NotSupportedException(CommonResources.ErrorResponseFileOptionNotSupported());
      if (strings == null)
        return;
      foreach (string commandLineValue1 in strings)
        this.ExpandInlineOption(commandLineValue1, expandedOptions, options);
    }

    private bool SetLastOptionValue(string value)
    {
      bool flag = false;
      if (this.lastOptionArgument != null)
      {
        Option option = this.lastOptionArgument.Option;
        if (option != null && option.ArgumentType != OptionArgumentType.None)
        {
          if (value != null)
            this.lastOptionArgument.Value = (object) BasicParser.TrimValue(value);
          flag = true;
        }
      }
      return flag;
    }

    private bool SetLastOptionDefaultValue()
    {
      bool flag = false;
      if (this.lastOptionArgument != null)
      {
        Option option = this.lastOptionArgument.Option;
        if (option != null && option.DefaultValue != null)
        {
          this.lastOptionArgument.Value = option.DefaultValue;
          this.lastOptionArgument.Metadata &= OptionMetadata.DefaultValue;
          flag = true;
        }
      }
      return flag;
    }

    private static string TrimValue(string value)
    {
      string str = value;
      if (value != null)
        str = value.Trim(' ', '\'', '"');
      return str;
    }
  }
}
