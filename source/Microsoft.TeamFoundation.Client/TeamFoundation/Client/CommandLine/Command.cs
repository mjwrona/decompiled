// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.CommandLine.Command
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Internal;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Diff;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Client.CommandLine
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class Command : IDisposable
  {
    public static readonly string IgnoreStdOutRedirection = "TFS_IGNORESTDOUTREDIRECT";
    private TfsConnection m_tfsConnection;
    protected Arguments m_arguments;
    private Stopwatch m_commandTimer;
    private Dictionary<QuestionType, string> m_questionSuffixes;
    private bool m_outputGenerated;

    protected Command(Arguments arguments)
    {
      this.m_arguments = arguments;
      this.m_commandTimer = new Stopwatch();
      this.m_commandTimer.Start();
    }

    public abstract void Run();

    public virtual void Dispose() => GC.SuppressFinalize((object) this);

    public void WriteSpacerLine()
    {
      if (!this.m_outputGenerated)
        return;
      this.WriteLine();
    }

    public void WriteErrorSpacerLine()
    {
      if (!this.m_outputGenerated)
        return;
      this.WriteError(string.Empty);
    }

    public void WriteError(string message)
    {
      this.m_outputGenerated = true;
      UIHost.WriteError(this.LogCategory, message);
    }

    public void WriteError(string messageFormat, params object[] args)
    {
      this.m_outputGenerated = true;
      UIHost.WriteError(this.LogCategory, messageFormat, args);
    }

    public void WriteWarning(string message)
    {
      this.m_outputGenerated = true;
      UIHost.WriteWarning(this.LogCategory, message);
    }

    public void WriteWarning(string messageFormat, params object[] args)
    {
      this.m_outputGenerated = true;
      UIHost.WriteWarning(this.LogCategory, messageFormat, args);
    }

    public void WriteInfo(string message)
    {
      this.m_outputGenerated = true;
      UIHost.WriteInfo(this.LogCategory, message);
    }

    public void WriteInfo(string messageFormat, params object[] args)
    {
      this.m_outputGenerated = true;
      UIHost.WriteInfo(this.LogCategory, messageFormat, args);
    }

    public void WriteIndented(string indent, string messageFormat, params object[] args)
    {
      this.m_outputGenerated = true;
      UIHost.WriteIndented(this.LogCategory, indent, messageFormat, args);
    }

    public void WriteIndented(string indent, string message)
    {
      this.m_outputGenerated = true;
      UIHost.WriteIndented(this.LogCategory, indent, message);
    }

    public void WriteXml(Action<XmlWriter> action)
    {
      StringBuilder output = new StringBuilder();
      XmlWriterSettings settings = new XmlWriterSettings()
      {
        Indent = true,
        IndentChars = "  ",
        NewLineChars = Environment.NewLine,
        NewLineOnAttributes = false,
        OmitXmlDeclaration = true
      };
      using (XmlWriter xmlWriter = XmlWriter.Create(output, settings))
      {
        action(xmlWriter);
        xmlWriter.Flush();
        this.WriteLine(output.ToString());
      }
    }

    public void WriteLine(string messageFormat, params object[] args)
    {
      this.m_outputGenerated = true;
      UIHost.WriteLine(this.LogCategory, messageFormat, args);
    }

    public void WriteLine(string message)
    {
      this.m_outputGenerated = true;
      UIHost.WriteLine(this.LogCategory, message);
    }

    public void WriteLine()
    {
      this.m_outputGenerated = true;
      UIHost.WriteLine(this.LogCategory);
    }

    public void Write(string messageFormat, params object[] args)
    {
      this.m_outputGenerated = true;
      UIHost.Write(this.LogCategory, messageFormat, args);
    }

    public void Write(string message)
    {
      this.m_outputGenerated = true;
      UIHost.Write(this.LogCategory, message);
    }

    public virtual LogCategory LogCategory => LogCategory.General;

    public void ReportBadOptionIfPresent(Options.ID conflictingID, string messageFormat)
    {
      Option option = this.m_arguments.GetOption(conflictingID);
      if (option != null)
        throw new Command.ArgumentListException(StringUtil.Format(messageFormat, (object) option.InvokedAs));
    }

    public void ReportBadOptionCombinationIfPresent(Options.ID optionId1, Options.ID optionId2)
    {
      Option option1 = this.m_arguments.GetOption(optionId1);
      Option option2 = this.m_arguments.GetOption(optionId2);
      if (option1 != null && option2 != null)
        throw new Command.ArgumentListException(ClientResources.InvalidOptionCombination((object) option1.InvokedAs, (object) option2.InvokedAs));
    }

    public void ReportBadOptionCombinationIfPresent(
      Options.ID optionId1,
      IEnumerable<Options.ID> optionList)
    {
      if (this.m_arguments.GetOption(optionId1) == null)
        return;
      foreach (Options.ID option in optionList)
        this.ReportBadOptionCombinationIfPresent(optionId1, option);
    }

    public void ReportMissingAssociatedOption(Options.ID option, Options.ID associatedOption)
    {
      Option option1 = this.m_arguments.GetOption(option);
      Option option2 = this.m_arguments.GetOption(associatedOption);
      if (option1 != null && option2 == null)
        throw new Command.ArgumentListException(ClientResources.MissingAssociatedOption((object) option1.InvokedAs, (object) associatedOption.ToString().ToLower(CultureInfo.InvariantCulture)));
    }

    public void ReportMissingAssociatedOption(
      IEnumerable<Options.ID> options,
      Options.ID associatedOption)
    {
      foreach (Options.ID option in options)
        this.ReportMissingAssociatedOption(option, associatedOption);
    }

    public void GetUserNameAndPassword(out string userName, out string password)
    {
      userName = (string) null;
      password = (string) null;
      Option option = this.m_arguments.GetOption(Options.ID.Login);
      if (option != null)
      {
        userName = option.Values[0];
        if (option.Values.Count >= 2 && option.Values[1] != null)
          password = option.Values[1];
      }
      if (password != null)
        return;
      password = string.Empty;
    }

    public void GetImpersonationUserName(out string impersonationUserName)
    {
      impersonationUserName = (string) null;
      Option option = this.m_arguments.GetOption(Options.ID.Impersonate);
      if (option == null)
        return;
      impersonationUserName = option.Values[0];
    }

    public virtual bool IsNoPromptSpecified()
    {
      if (this.m_arguments.Contains(Options.ID.Prompt))
        return false;
      return !VssStringComparer.EnvVar.Equals(Environment.GetEnvironmentVariable(Command.IgnoreStdOutRedirection), "1") && ConsoleHost.IsStdOutRedirected || this.m_arguments.Contains(Options.ID.NoPrompt);
    }

    public Option GetCollectionOption()
    {
      Option option1 = this.m_arguments.GetOption(Options.ID.Server);
      Option option2 = this.m_arguments.GetOption(Options.ID.Collection);
      this.ReportBadOptionCombinationIfPresent(Options.ID.Server, Options.ID.Collection);
      return option1 ?? option2;
    }

    public string GetCollectionOptionValue()
    {
      Option collectionOption = this.GetCollectionOption();
      return collectionOption != null && collectionOption.Values.Count > 0 ? collectionOption.Values[0] : (string) null;
    }

    public static void ParseString(
      string s,
      char separator,
      out string value,
      out string remainder)
    {
      StringBuilder stringBuilder = new StringBuilder(s.Length);
      int num = -1;
      for (int index = 0; index < s.Length; ++index)
      {
        if ((int) s[index] == (int) separator && index + 1 < s.Length && (int) s[index + 1] == (int) separator)
        {
          stringBuilder.Append(separator);
          ++index;
        }
        else
        {
          if ((int) s[index] == (int) separator)
          {
            num = index;
            break;
          }
          stringBuilder.Append(s[index]);
        }
      }
      value = stringBuilder.ToString();
      if (num != -1 && num != s.Length - 1)
        remainder = s.Substring(num + 1, s.Length - (num + 1));
      else
        remainder = string.Empty;
    }

    internal bool IsOptionSpecified(Options.ID optionId) => this.m_arguments.Contains(optionId);

    public string GetStringStyleOption(Options.ID optionId, int maxSize) => Command.GetStringStyleOption(this.m_arguments, optionId, maxSize);

    public static string GetStringStyleOption(
      Arguments arguments,
      Options.ID optionId,
      int maxSize)
    {
      Option option = arguments.GetOption(optionId);
      if (option == null)
        return (string) null;
      string stringStyleOption = option.Values[0];
      if (stringStyleOption.Length > 1 && stringStyleOption[0] == '@')
        return Command.ReadFile(stringStyleOption.Substring(1), maxSize);
      if (maxSize > 0 && stringStyleOption.Length > maxSize)
        throw new Command.ArgumentListException(ClientResources.ValueLengthExceedsMaxForOption((object) option.InvokedAs, (object) maxSize));
      return stringStyleOption;
    }

    private static string ReadFile(string fileName, int maxSize)
    {
      using (StreamReader streamReader = new StreamReader(fileName, FileTypeUtil.TryDetermineTextEncoding(fileName), false))
      {
        string end = streamReader.ReadToEnd();
        if (maxSize > 0 && end.Length > maxSize)
          throw new Command.ArgumentListException(ClientResources.ContentsOfFileTooBig((object) fileName, (object) maxSize));
        return end;
      }
    }

    public Options.Value ParseOption(
      Options.ID optionId,
      Options.Value[] supportedValues,
      Options.Value fallback)
    {
      string optionValue = this.m_arguments.GetOptionValue(optionId);
      if (optionValue == null)
        return fallback;
      foreach (Options.Value supportedValue in supportedValues)
      {
        if (Options.MatchesValue(optionValue, supportedValue))
          return supportedValue;
      }
      throw new Command.ArgumentListException(ClientResources.InvalidOptionValue((object) optionValue, (object) this.m_arguments.GetOption(optionId).InvokedAs));
    }

    protected ExitCode GetExitCode() => (ExitCode) Environment.ExitCode;

    protected virtual void SetExitCode(ExitCode exitCode)
    {
      if (Environment.ExitCode == -1 || exitCode > (ExitCode) Environment.ExitCode)
      {
        Environment.ExitCode = (int) exitCode;
      }
      else
      {
        if (exitCode == (ExitCode) Environment.ExitCode)
          return;
        Environment.ExitCode = 1;
      }
    }

    public int CalculateWidth(string text) => UIHost.CalculateWidth(text);

    public string Truncate(string text, int width) => UIHost.Truncate(text, width);

    public static bool IsEnvVarSet(string envVar) => string.Equals(Environment.GetEnvironmentVariable(envVar), "1", StringComparison.OrdinalIgnoreCase);

    public bool IsTfsSet() => this.m_tfsConnection != null;

    public QuestionResponse AskQuestion(QuestionType questionType, string question)
    {
      if (this.m_questionSuffixes == null)
      {
        this.m_questionSuffixes = new Dictionary<QuestionType, string>();
        this.m_questionSuffixes.Add(QuestionType.YesNo, ClientResources.QuestionYNSuffix());
        this.m_questionSuffixes.Add(QuestionType.YesNoAll, ClientResources.QuestionYNASuffix());
      }
      while (true)
      {
        ConsoleHost.ThrowIfCanceled();
        this.Write(question);
        this.Write(" ");
        this.Write(this.m_questionSuffixes[questionType]);
        string str1 = ConsoleHost.ReadLine();
        if (str1 == null)
        {
          ConsoleHost.ThrowIfCanceled();
        }
        else
        {
          string str2 = str1.Trim();
          if (!str2.Equals(ClientResources.AnswerYes(), StringComparison.CurrentCultureIgnoreCase) && !str2.Equals(ClientResources.AnswerYesFull(), StringComparison.CurrentCultureIgnoreCase))
          {
            if (!str2.Equals(ClientResources.AnswerNo(), StringComparison.CurrentCultureIgnoreCase) && !str2.Equals(ClientResources.AnswerNoFull(), StringComparison.CurrentCultureIgnoreCase))
            {
              if (questionType != QuestionType.YesNoAll || !str2.Equals(ClientResources.AnswerAll(), StringComparison.CurrentCultureIgnoreCase) && !str2.Equals(ClientResources.AnswerAllFull(), StringComparison.CurrentCultureIgnoreCase))
                this.WriteLine(ClientResources.InvalidResponse());
              else
                goto label_9;
            }
            else
              goto label_7;
          }
          else
            break;
        }
      }
      return QuestionResponse.Yes;
label_7:
      return QuestionResponse.No;
label_9:
      return QuestionResponse.All;
    }

    public TfsConnection TfsConnection
    {
      get => this.m_tfsConnection;
      protected set => this.m_tfsConnection = value;
    }

    [Serializable]
    public class ArgumentListException : ApplicationException
    {
      public ArgumentListException(string message)
        : base(message)
      {
      }

      public ArgumentListException(string message, Exception innerException)
        : base(message, innerException)
      {
      }

      protected ArgumentListException(SerializationInfo info, StreamingContext context)
        : base(info, context)
      {
      }
    }

    [Serializable]
    public class BlockedException : ApplicationException
    {
      public BlockedException(string message)
        : base(message)
      {
      }

      public BlockedException(string message, Exception innerException)
        : base(message, innerException)
      {
      }

      protected BlockedException(SerializationInfo info, StreamingContext context)
        : base(info, context)
      {
      }
    }

    [Serializable]
    public class UnrecognizedCommandException : ApplicationException
    {
      public UnrecognizedCommandException(string message)
        : base(message)
      {
      }

      public UnrecognizedCommandException(string message, Exception innerException)
        : base(message, innerException)
      {
      }

      protected UnrecognizedCommandException(SerializationInfo info, StreamingContext context)
        : base(info, context)
      {
      }
    }
  }
}
