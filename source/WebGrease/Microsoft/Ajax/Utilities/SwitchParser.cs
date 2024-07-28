// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.SwitchParser
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.Ajax.Utilities
{
  public class SwitchParser
  {
    private bool m_isMono;
    private bool m_noPretty;

    public CodeSettings JSSettings { get; private set; }

    public CssSettings CssSettings { get; private set; }

    public bool AnalyzeMode { get; private set; }

    public string ReportFormat { get; private set; }

    public string ReportPath { get; private set; }

    public bool PrettyPrint { get; private set; }

    public int WarningLevel { get; set; }

    public ExistingFileTreatment Clobber { get; set; }

    public string EncodingOutputName { get; private set; }

    public string EncodingInputName { get; private set; }

    public event EventHandler<InvalidSwitchEventArgs> InvalidSwitch;

    public event EventHandler<UnknownParameterEventArgs> UnknownParameter;

    public event EventHandler JSOnlyParameter;

    public event EventHandler CssOnlyParameter;

    public SwitchParser()
    {
      this.JSSettings = new CodeSettings();
      this.CssSettings = new CssSettings();
      this.m_isMono = Type.GetType("Mono.Runtime") != (Type) null;
    }

    public SwitchParser(CodeSettings scriptSettings, CssSettings cssSettings)
    {
      this.JSSettings = scriptSettings ?? new CodeSettings();
      this.CssSettings = cssSettings ?? new CssSettings();
    }

    public SwitchParser Clone() => new SwitchParser(this.JSSettings.Clone(), this.CssSettings.Clone())
    {
      AnalyzeMode = this.AnalyzeMode,
      EncodingInputName = this.EncodingInputName,
      EncodingOutputName = this.EncodingOutputName,
      PrettyPrint = this.PrettyPrint,
      ReportFormat = this.ReportFormat,
      ReportPath = this.ReportPath,
      WarningLevel = this.WarningLevel
    };

    public static string[] ToArguments(string commandLine)
    {
      List<string> stringList = new List<string>();
      if (!string.IsNullOrEmpty(commandLine))
      {
        int length = commandLine.Length;
        for (int index = 0; index < length; ++index)
        {
          while (index < length && char.IsWhiteSpace(commandLine[index]))
            ++index;
          StringBuilder stringBuilder = (StringBuilder) null;
          if (index < length)
          {
            bool flag = commandLine[index] == '"';
            if (flag)
              stringBuilder = new StringBuilder();
            int startIndex = flag ? index + 1 : index;
            while (++index < length)
            {
              char c = commandLine[index];
              if (flag)
              {
                if (c == '"')
                {
                  if (index + 1 < length && commandLine[index + 1] == '"')
                  {
                    if (index > startIndex)
                      stringBuilder.Append(commandLine.Substring(startIndex, index - startIndex));
                    stringBuilder.Append('"');
                    startIndex = ++index + 1;
                  }
                  else
                  {
                    flag = false;
                    if (index > startIndex)
                      stringBuilder.Append(commandLine.Substring(startIndex, index - startIndex));
                    startIndex = index + 1;
                  }
                }
              }
              else if (!char.IsWhiteSpace(c))
              {
                if (c == '"')
                {
                  flag = true;
                  if (stringBuilder == null)
                    stringBuilder = new StringBuilder();
                  stringBuilder.Append(commandLine.Substring(startIndex, index - startIndex));
                  startIndex = index + 1;
                }
              }
              else
                break;
            }
            if (stringBuilder != null)
            {
              if (index > startIndex)
                stringBuilder.Append(commandLine.Substring(startIndex, index - startIndex));
              stringList.Add(stringBuilder.ToString());
            }
            else
              stringList.Add(commandLine.Substring(startIndex, index - startIndex));
          }
        }
      }
      return stringList.ToArray();
    }

    public void Parse(string commandLine)
    {
      if (string.IsNullOrEmpty(commandLine))
        return;
      this.Parse(SwitchParser.ToArguments(commandLine));
    }

    public void Parse(string[] args)
    {
      char[] separator = new char[2]{ ',', ';' };
      if (args == null)
        return;
      bool flag1 = false;
      bool flag2 = false;
      bool killSpecified = false;
      bool flag3 = false;
      for (int index1 = 0; index1 < args.Length; ++index1)
      {
        string str1 = args[index1];
        if (str1.Length > 1 && (str1.StartsWith("-", StringComparison.Ordinal) || str1.StartsWith("–", StringComparison.Ordinal) || !this.m_isMono && str1.StartsWith("/", StringComparison.Ordinal)))
        {
          string[] strArray1 = str1.Substring(1).Split(':');
          string upperInvariant1 = strArray1[0].ToUpperInvariant();
          string str2 = strArray1.Length == 1 ? (string) null : strArray1[1];
          string upperInvariant2 = str2 == null ? (string) null : str2.ToUpperInvariant();
          int num1;
          bool booleanValue;
          switch (upperInvariant1)
          {
            case "ANALYZE":
            case "A":
              this.AnalyzeMode = true;
              this.ReportFormat = (string) null;
              if (upperInvariant2 != null)
              {
                foreach (string strA in upperInvariant2.Split(separator, StringSplitOptions.RemoveEmptyEntries))
                {
                  if (string.CompareOrdinal(strA, "OUT") == 0)
                  {
                    if (index1 >= args.Length - 1)
                      this.OnInvalidSwitch(upperInvariant1, str2);
                    else
                      this.ReportPath = args[++index1];
                  }
                  else
                    this.ReportFormat = strA;
                }
              }
              if (!flag1)
              {
                this.WarningLevel = int.MaxValue;
                continue;
              }
              continue;
            case "ASPNET":
              if (SwitchParser.BooleanSwitch(upperInvariant2, true, out booleanValue))
              {
                this.JSSettings.AllowEmbeddedAspNetBlocks = this.CssSettings.AllowEmbeddedAspNetBlocks = booleanValue;
                continue;
              }
              this.OnInvalidSwitch(upperInvariant1, str2);
              continue;
            case "BRACES":
              switch (upperInvariant2)
              {
                case "NEW":
                  this.JSSettings.BlocksStartOnSameLine = this.CssSettings.BlocksStartOnSameLine = BlockStart.NewLine;
                  continue;
                case "SAME":
                  this.JSSettings.BlocksStartOnSameLine = this.CssSettings.BlocksStartOnSameLine = BlockStart.SameLine;
                  continue;
                case "SOURCE":
                  this.JSSettings.BlocksStartOnSameLine = this.CssSettings.BlocksStartOnSameLine = BlockStart.UseSource;
                  continue;
                default:
                  this.OnInvalidSwitch(upperInvariant1, str2);
                  continue;
              }
            case "CC":
              if (SwitchParser.BooleanSwitch(upperInvariant2, true, out booleanValue))
                this.JSSettings.IgnoreConditionalCompilation = !booleanValue;
              else
                this.OnInvalidSwitch(upperInvariant1, str2);
              this.OnJSOnlyParameter();
              continue;
            case "CLOBBER":
              if (upperInvariant2 == null)
              {
                this.Clobber = ExistingFileTreatment.Overwrite;
                continue;
              }
              if (SwitchParser.BooleanSwitch(upperInvariant2, true, out booleanValue))
              {
                this.Clobber = booleanValue ? ExistingFileTreatment.Overwrite : ExistingFileTreatment.Auto;
                continue;
              }
              this.OnInvalidSwitch(upperInvariant1, str2);
              continue;
            case "COLORS":
              switch (upperInvariant2)
              {
                case "HEX":
                  this.CssSettings.ColorNames = CssColor.Hex;
                  break;
                case "STRICT":
                  this.CssSettings.ColorNames = CssColor.Strict;
                  break;
                case "MAJOR":
                  this.CssSettings.ColorNames = CssColor.Major;
                  break;
                default:
                  this.OnInvalidSwitch(upperInvariant1, str2);
                  break;
              }
              this.OnCssOnlyParameter();
              continue;
            case "COMMENTS":
              switch (upperInvariant2)
              {
                case "NONE":
                  this.CssSettings.CommentMode = CssComment.None;
                  this.JSSettings.PreserveImportantComments = false;
                  continue;
                case "ALL":
                  this.CssSettings.CommentMode = CssComment.All;
                  this.OnCssOnlyParameter();
                  continue;
                case "IMPORTANT":
                  this.CssSettings.CommentMode = CssComment.Important;
                  this.JSSettings.PreserveImportantComments = true;
                  continue;
                case "HACKS":
                  this.CssSettings.CommentMode = CssComment.Hacks;
                  this.OnCssOnlyParameter();
                  continue;
                default:
                  this.OnInvalidSwitch(upperInvariant1, str2);
                  continue;
              }
            case "CONST":
              switch (upperInvariant2)
              {
                case "MOZ":
                  this.JSSettings.ConstStatementsMozilla = true;
                  break;
                case "ES6":
                  this.JSSettings.ConstStatementsMozilla = false;
                  break;
                default:
                  this.OnInvalidSwitch(upperInvariant1, str2);
                  break;
              }
              this.OnJSOnlyParameter();
              continue;
            case "CSS":
              this.OnCssOnlyParameter();
              if (upperInvariant2 != null)
              {
                switch (upperInvariant2)
                {
                  case "FULL":
                    this.CssSettings.CssType = CssType.FullStyleSheet;
                    continue;
                  case "DECLS":
                    this.CssSettings.CssType = CssType.DeclarationList;
                    continue;
                  default:
                    this.OnInvalidSwitch(upperInvariant1, str2);
                    continue;
                }
              }
              else
                continue;
            case "DEBUG":
              this.m_noPretty = true;
              if (this.PrettyPrint)
                this.OnInvalidSwitch(upperInvariant1, str2);
              if (upperInvariant2 != null && upperInvariant2.IndexOf(',') >= 0)
              {
                string[] strArray2 = str2.Split(separator);
                if (SwitchParser.BooleanSwitch(strArray2[0].ToUpperInvariant(), true, out booleanValue))
                {
                  this.JSSettings.StripDebugStatements = !booleanValue;
                  SwitchParser.AlignDebugDefine(this.JSSettings.StripDebugStatements, this.JSSettings.PreprocessorValues);
                }
                else
                  this.OnInvalidSwitch(upperInvariant1, str2);
                this.JSSettings.DebugLookupList = (string) null;
                for (int index2 = 1; index2 < strArray2.Length; ++index2)
                {
                  string str3 = strArray2[index2];
                  if (!str3.IsNullOrWhiteSpace() && !this.JSSettings.AddDebugLookup(str3))
                    this.OnInvalidSwitch(upperInvariant1, str3);
                }
              }
              else if (SwitchParser.BooleanSwitch(upperInvariant2, true, out booleanValue))
              {
                this.JSSettings.StripDebugStatements = !booleanValue;
                SwitchParser.AlignDebugDefine(this.JSSettings.StripDebugStatements, this.JSSettings.PreprocessorValues);
              }
              this.OnJSOnlyParameter();
              continue;
            case "DEFINE":
              if (string.IsNullOrEmpty(upperInvariant2))
              {
                this.OnInvalidSwitch(upperInvariant1, str2);
                continue;
              }
              foreach (string parameterPart in str2.Split(separator, StringSplitOptions.RemoveEmptyEntries))
              {
                int length = parameterPart.IndexOf('=');
                string str4;
                string str5;
                if (length < 0)
                {
                  str4 = parameterPart.Trim();
                  str5 = string.Empty;
                }
                else
                {
                  str4 = parameterPart.Substring(0, length).Trim();
                  str5 = parameterPart.Substring(length + 1);
                }
                if (!JSScanner.IsValidIdentifier(str4))
                {
                  this.OnInvalidSwitch(upperInvariant1, parameterPart);
                }
                else
                {
                  this.JSSettings.PreprocessorValues[str4] = str5;
                  this.CssSettings.PreprocessorValues[str4] = str5;
                }
                if (string.Compare(str4, "DEBUG", StringComparison.OrdinalIgnoreCase) == 0)
                  this.JSSettings.StripDebugStatements = false;
              }
              continue;
            case "ENC":
              if (index1 >= args.Length - 1)
              {
                this.OnInvalidSwitch(upperInvariant1, str2);
                continue;
              }
              string str6 = args[++index1];
              switch (upperInvariant2)
              {
                case "IN":
                  this.EncodingInputName = str6;
                  continue;
                case "OUT":
                  this.EncodingOutputName = str6;
                  continue;
                default:
                  this.OnInvalidSwitch(upperInvariant1, str2);
                  continue;
              }
            case "ESC":
              if (SwitchParser.BooleanSwitch(upperInvariant2, true, out booleanValue))
                this.JSSettings.AlwaysEscapeNonAscii = booleanValue;
              else
                this.OnInvalidSwitch(upperInvariant1, str2);
              this.OnJSOnlyParameter();
              continue;
            case "EVALS":
              switch (upperInvariant2)
              {
                case "IGNORE":
                  this.JSSettings.EvalTreatment = EvalTreatment.Ignore;
                  break;
                case "IMMEDIATE":
                  this.JSSettings.EvalTreatment = EvalTreatment.MakeImmediateSafe;
                  break;
                case "SAFEALL":
                  this.JSSettings.EvalTreatment = EvalTreatment.MakeAllSafe;
                  break;
                default:
                  this.OnInvalidSwitch(upperInvariant1, str2);
                  break;
              }
              this.OnJSOnlyParameter();
              continue;
            case "EXPR":
              switch (upperInvariant2)
              {
                case "MINIFY":
                  this.CssSettings.MinifyExpressions = true;
                  break;
                case "RAW":
                  this.CssSettings.MinifyExpressions = false;
                  break;
                default:
                  this.OnInvalidSwitch(upperInvariant1, str2);
                  break;
              }
              this.OnCssOnlyParameter();
              continue;
            case "FNAMES":
              switch (upperInvariant2)
              {
                case "LOCK":
                  this.JSSettings.RemoveFunctionExpressionNames = false;
                  this.JSSettings.PreserveFunctionNames = true;
                  break;
                case "KEEP":
                  this.JSSettings.RemoveFunctionExpressionNames = false;
                  this.JSSettings.PreserveFunctionNames = false;
                  break;
                case "ONLYREF":
                  this.JSSettings.RemoveFunctionExpressionNames = true;
                  this.JSSettings.PreserveFunctionNames = false;
                  this.m_noPretty = true;
                  if (this.PrettyPrint)
                  {
                    this.OnInvalidSwitch(upperInvariant1, str2);
                    break;
                  }
                  break;
                default:
                  this.OnInvalidSwitch(upperInvariant1, str2);
                  break;
              }
              this.OnJSOnlyParameter();
              continue;
            case "GLOBAL":
            case "G":
              if (string.IsNullOrEmpty(upperInvariant2))
              {
                this.OnInvalidSwitch(upperInvariant1, str2);
              }
              else
              {
                foreach (string str7 in str2.Split(separator, StringSplitOptions.RemoveEmptyEntries))
                {
                  if (!this.JSSettings.AddKnownGlobal(str7))
                    this.OnInvalidSwitch(upperInvariant1, str7);
                }
              }
              this.OnJSOnlyParameter();
              continue;
            case "IGNORE":
              if (string.IsNullOrEmpty(upperInvariant2))
              {
                this.OnInvalidSwitch(upperInvariant1, str2);
                continue;
              }
              foreach (string strA in str2.Split(separator, StringSplitOptions.RemoveEmptyEntries))
              {
                if (string.Compare(strA, "ALL", StringComparison.OrdinalIgnoreCase) == 0)
                {
                  this.JSSettings.IgnoreAllErrors = this.CssSettings.IgnoreAllErrors = true;
                }
                else
                {
                  this.JSSettings.IgnoreErrorCollection.Add(strA);
                  this.CssSettings.IgnoreErrorCollection.Add(strA);
                }
              }
              continue;
            case "INLINE":
              if (string.IsNullOrEmpty(str2))
              {
                this.JSSettings.InlineSafeStrings = true;
              }
              else
              {
                foreach (string str8 in upperInvariant2.Split(separator, StringSplitOptions.RemoveEmptyEntries))
                {
                  if (string.CompareOrdinal(str8, "FORCE") == 0)
                  {
                    this.JSSettings.ErrorIfNotInlineSafe = true;
                    this.JSSettings.InlineSafeStrings = true;
                  }
                  else if (string.CompareOrdinal(str8, "NOFORCE") == 0)
                    this.JSSettings.ErrorIfNotInlineSafe = false;
                  else if (SwitchParser.BooleanSwitch(str8, true, out booleanValue))
                    this.JSSettings.InlineSafeStrings = booleanValue;
                  else
                    this.OnInvalidSwitch(upperInvariant1, str2);
                }
              }
              this.OnJSOnlyParameter();
              continue;
            case "JS":
              if (str2 == null)
              {
                this.JSSettings.SourceMode = JavaScriptSourceMode.Program;
                this.JSSettings.Format = JavaScriptFormat.Normal;
              }
              else
              {
                string[] strArray3 = upperInvariant2.Split(',', ';');
                foreach (string str9 in strArray3)
                {
                  switch (str9)
                  {
                    case "JSON":
                      if (strArray3.Length > 1)
                        this.OnInvalidSwitch(upperInvariant1, str2);
                      this.JSSettings.MinifyCode = false;
                      this.JSSettings.SourceMode = JavaScriptSourceMode.Expression;
                      this.JSSettings.Format = JavaScriptFormat.JSON;
                      break;
                    case "PROG":
                    case "PROGRAM":
                      this.JSSettings.SourceMode = JavaScriptSourceMode.Program;
                      break;
                    case "MOD":
                    case "MODULE":
                      this.JSSettings.SourceMode = JavaScriptSourceMode.Module;
                      break;
                    case "EXPR":
                    case "EXPRESSION":
                      this.JSSettings.SourceMode = JavaScriptSourceMode.Expression;
                      break;
                    case "EVT":
                    case "EVENT":
                      this.JSSettings.SourceMode = JavaScriptSourceMode.EventHandler;
                      break;
                    case "ES5":
                      this.JSSettings.ScriptVersion = ScriptVersion.EcmaScript5;
                      break;
                    case "ES6":
                      this.JSSettings.ScriptVersion = ScriptVersion.EcmaScript6;
                      break;
                    default:
                      this.OnInvalidSwitch(upperInvariant1, str2);
                      break;
                  }
                }
              }
              this.OnJSOnlyParameter();
              continue;
            case "KILL":
              killSpecified = true;
              if (upperInvariant2 == null)
              {
                this.OnInvalidSwitch(upperInvariant1, str2);
                continue;
              }
              long number1;
              if (upperInvariant2.StartsWith("0X", StringComparison.OrdinalIgnoreCase))
              {
                if (upperInvariant2.Substring(2).TryParseLongInvariant(NumberStyles.AllowHexSpecifier, out number1))
                {
                  this.JSSettings.KillSwitch = this.CssSettings.KillSwitch = number1;
                  if ((number1 & 1L) != 0L)
                  {
                    this.CssSettings.CommentMode = CssComment.None;
                    continue;
                  }
                  continue;
                }
                this.OnInvalidSwitch(upperInvariant1, str2);
                continue;
              }
              if (upperInvariant2.TryParseLongInvariant(NumberStyles.AllowLeadingSign, out number1))
              {
                this.JSSettings.KillSwitch = this.CssSettings.KillSwitch = number1;
                if ((number1 & 1L) != 0L)
                {
                  this.CssSettings.CommentMode = CssComment.None;
                  continue;
                }
                continue;
              }
              this.OnInvalidSwitch(upperInvariant1, str2);
              continue;
            case "LINE":
            case "LINES":
              if (string.IsNullOrEmpty(upperInvariant2))
              {
                CodeSettings jsSettings1 = this.JSSettings;
                this.CssSettings.LineBreakThreshold = num1 = 2147482647;
                int num2 = num1;
                jsSettings1.LineBreakThreshold = num2;
                this.JSSettings.OutputMode = this.CssSettings.OutputMode = OutputMode.SingleLine;
                CodeSettings jsSettings2 = this.JSSettings;
                this.CssSettings.IndentSize = num1 = 4;
                int num3 = num1;
                jsSettings2.IndentSize = num3;
                continue;
              }
              string[] strArray4 = upperInvariant2.Split(separator, StringSplitOptions.RemoveEmptyEntries);
              int index3 = 1;
              if (strArray4.Length <= 3)
              {
                if (!string.IsNullOrEmpty(strArray4[0]))
                {
                  int number2;
                  if (strArray4[0].TryParseIntInvariant(NumberStyles.None, out number2))
                  {
                    CodeSettings jsSettings = this.JSSettings;
                    this.CssSettings.LineBreakThreshold = num1 = number2;
                    int num4 = num1;
                    jsSettings.LineBreakThreshold = num4;
                  }
                  else if (strArray4[0][0] == 'S')
                  {
                    this.JSSettings.OutputMode = this.CssSettings.OutputMode = OutputMode.SingleLine;
                    index3 = 0;
                  }
                  else if (strArray4[0][0] == 'M')
                  {
                    this.JSSettings.OutputMode = this.CssSettings.OutputMode = OutputMode.MultipleLines;
                    index3 = 0;
                  }
                  else
                    this.OnInvalidSwitch(upperInvariant1, strArray4[0]);
                }
                else
                {
                  CodeSettings jsSettings = this.JSSettings;
                  this.CssSettings.LineBreakThreshold = num1 = 2147482647;
                  int num5 = num1;
                  jsSettings.LineBreakThreshold = num5;
                }
                if (strArray4.Length > index3)
                {
                  if (index3 > 0)
                  {
                    if (string.IsNullOrEmpty(strArray4[index3]) || strArray4[index3][0] == 'S')
                      this.JSSettings.OutputMode = this.CssSettings.OutputMode = OutputMode.SingleLine;
                    else if (strArray4[index3][0] == 'M')
                      this.JSSettings.OutputMode = this.CssSettings.OutputMode = OutputMode.MultipleLines;
                    else
                      this.OnInvalidSwitch(upperInvariant1, strArray4[index3]);
                  }
                  int index4 = index3 + 1;
                  if (strArray4.Length > index4)
                  {
                    if (!string.IsNullOrEmpty(strArray4[index4]))
                    {
                      int number3;
                      if (strArray4[index4].TryParseIntInvariant(NumberStyles.None, out number3))
                      {
                        CodeSettings jsSettings = this.JSSettings;
                        this.CssSettings.IndentSize = num1 = number3;
                        int num6 = num1;
                        jsSettings.IndentSize = num6;
                        continue;
                      }
                      this.OnInvalidSwitch(upperInvariant1, strArray4[index4]);
                      continue;
                    }
                    CodeSettings jsSettings3 = this.JSSettings;
                    this.CssSettings.IndentSize = num1 = 4;
                    int num7 = num1;
                    jsSettings3.IndentSize = num7;
                    continue;
                  }
                  continue;
                }
                continue;
              }
              this.OnInvalidSwitch(upperInvariant1, str2);
              continue;
            case "LITERALS":
              switch (upperInvariant2)
              {
                case "KEEP":
                case "COMBINE":
                  this.OnJSOnlyParameter();
                  continue;
                case "EVAL":
                  this.JSSettings.EvalLiteralExpressions = true;
                  this.m_noPretty = true;
                  if (this.PrettyPrint)
                  {
                    this.OnInvalidSwitch(upperInvariant1, str2);
                    goto case "KEEP";
                  }
                  else
                    goto case "KEEP";
                case "NOEVAL":
                  this.JSSettings.EvalLiteralExpressions = false;
                  goto case "KEEP";
                default:
                  this.OnInvalidSwitch(upperInvariant1, str2);
                  goto case "KEEP";
              }
            case "MAC":
              if (SwitchParser.BooleanSwitch(upperInvariant2, true, out booleanValue))
                this.JSSettings.MacSafariQuirks = booleanValue;
              else
                this.OnInvalidSwitch(upperInvariant1, str2);
              this.OnJSOnlyParameter();
              continue;
            case "MINIFY":
              flag3 = true;
              if (flag2 && this.JSSettings.LocalRenaming != LocalRenaming.KeepAll)
                this.OnInvalidSwitch(upperInvariant1, str2);
              else if (SwitchParser.BooleanSwitch(upperInvariant2, true, out booleanValue))
              {
                this.JSSettings.MinifyCode = booleanValue;
                if (booleanValue)
                {
                  this.m_noPretty = true;
                  if (this.PrettyPrint)
                    this.OnInvalidSwitch(upperInvariant1, str2);
                }
              }
              else
                this.OnInvalidSwitch(upperInvariant1, str2);
              this.OnJSOnlyParameter();
              continue;
            case "NEW":
              switch (upperInvariant2)
              {
                case "KEEP":
                  this.JSSettings.CollapseToLiteral = false;
                  break;
                case "COLLAPSE":
                  this.JSSettings.CollapseToLiteral = true;
                  this.m_noPretty = true;
                  if (this.PrettyPrint)
                  {
                    this.OnInvalidSwitch(upperInvariant1, str2);
                    break;
                  }
                  break;
                default:
                  this.OnInvalidSwitch(upperInvariant1, str2);
                  break;
              }
              this.OnJSOnlyParameter();
              continue;
            case "NFE":
              switch (upperInvariant2)
              {
                case "KEEPALL":
                  this.JSSettings.RemoveFunctionExpressionNames = false;
                  break;
                case "ONLYREF":
                  this.JSSettings.RemoveFunctionExpressionNames = true;
                  this.m_noPretty = true;
                  if (this.PrettyPrint)
                  {
                    this.OnInvalidSwitch(upperInvariant1, str2);
                    break;
                  }
                  break;
                default:
                  this.OnInvalidSwitch(upperInvariant1, str2);
                  break;
              }
              this.OnJSOnlyParameter();
              continue;
            case "NOCLOBBER":
              if (upperInvariant2 == null)
              {
                this.Clobber = ExistingFileTreatment.Preserve;
                continue;
              }
              if (SwitchParser.BooleanSwitch(upperInvariant2, true, out booleanValue))
              {
                this.Clobber = booleanValue ? ExistingFileTreatment.Preserve : ExistingFileTreatment.Auto;
                continue;
              }
              this.OnInvalidSwitch(upperInvariant1, str2);
              continue;
            case "NORENAME":
              if (string.IsNullOrEmpty(upperInvariant2))
              {
                this.OnInvalidSwitch(upperInvariant1, str2);
              }
              else
              {
                foreach (string str10 in str2.Split(separator, StringSplitOptions.RemoveEmptyEntries))
                {
                  if (!this.JSSettings.AddNoAutoRename(str10))
                    this.OnInvalidSwitch(upperInvariant1, str10);
                }
              }
              this.OnJSOnlyParameter();
              continue;
            case "OBJ":
              this.m_noPretty = true;
              if (this.PrettyPrint)
                this.OnInvalidSwitch(upperInvariant1, str2);
              switch (upperInvariant2)
              {
                case "MIN":
                  this.JSSettings.QuoteObjectLiteralProperties = false;
                  break;
                case "QUOTE":
                  this.JSSettings.QuoteObjectLiteralProperties = true;
                  break;
                default:
                  this.OnInvalidSwitch(upperInvariant1, str2);
                  break;
              }
              this.OnJSOnlyParameter();
              continue;
            case "PPONLY":
              this.m_noPretty = true;
              if (this.PrettyPrint)
                this.OnInvalidSwitch(upperInvariant1, str2);
              if (str2 == null)
                this.JSSettings.PreprocessOnly = true;
              else if (SwitchParser.BooleanSwitch(upperInvariant2, true, out booleanValue))
                this.JSSettings.PreprocessOnly = booleanValue;
              else
                this.OnInvalidSwitch(upperInvariant1, str2);
              this.OnJSOnlyParameter();
              continue;
            case "PRETTY":
            case "P":
              this.PrettyPrint = true;
              if (this.m_noPretty)
                this.OnInvalidSwitch(upperInvariant1, str2);
              this.JSSettings.MinifyCode = false;
              this.JSSettings.OutputMode = this.CssSettings.OutputMode = OutputMode.MultipleLines;
              this.CssSettings.KillSwitch = -2L;
              if (upperInvariant2 != null)
              {
                int number4;
                if (str2.TryParseIntInvariant(NumberStyles.None, out number4))
                {
                  CodeSettings jsSettings = this.JSSettings;
                  this.CssSettings.IndentSize = num1 = number4;
                  int num8 = num1;
                  jsSettings.IndentSize = num8;
                  continue;
                }
                this.OnInvalidSwitch(upperInvariant1, str2);
                continue;
              }
              continue;
            case "RENAME":
              if (upperInvariant2 == null)
                index1 = this.OnUnknownParameter((IList<string>) args, index1, upperInvariant1, str2);
              else if (upperInvariant2.IndexOf('=') > 0)
              {
                this.m_noPretty = true;
                if (this.PrettyPrint)
                  this.OnInvalidSwitch(upperInvariant1, str2);
                foreach (string str11 in str2.Split(separator, StringSplitOptions.RemoveEmptyEntries))
                {
                  char[] chArray = new char[1]{ '=' };
                  string[] strArray5 = str11.Split(chArray);
                  if (strArray5.Length == 2)
                  {
                    string str12 = strArray5[0];
                    string str13 = strArray5[1];
                    bool flag4 = JSScanner.IsValidIdentifier(str12);
                    bool flag5 = JSScanner.IsValidIdentifier(str13);
                    if (flag4 && flag5)
                    {
                      string newName = this.JSSettings.GetNewName(str12);
                      if (newName == null)
                        this.JSSettings.AddRenamePair(str12, str13);
                      else if (string.CompareOrdinal(str13, newName) != 0)
                        this.OnInvalidSwitch(upperInvariant1, str12);
                    }
                    else
                    {
                      if (flag4)
                        this.OnInvalidSwitch(upperInvariant1, str13);
                      if (flag5)
                        this.OnInvalidSwitch(upperInvariant1, str12);
                    }
                  }
                  else
                    this.OnInvalidSwitch(upperInvariant1, str2);
                }
              }
              else
              {
                switch (upperInvariant2)
                {
                  case "ALL":
                    this.JSSettings.LocalRenaming = LocalRenaming.CrunchAll;
                    flag2 = true;
                    this.m_noPretty = true;
                    if (this.PrettyPrint)
                    {
                      this.OnInvalidSwitch(upperInvariant1, str2);
                      break;
                    }
                    break;
                  case "LOCALIZATION":
                    this.JSSettings.LocalRenaming = LocalRenaming.KeepLocalizationVars;
                    flag2 = true;
                    this.m_noPretty = true;
                    if (this.PrettyPrint)
                    {
                      this.OnInvalidSwitch(upperInvariant1, str2);
                      break;
                    }
                    break;
                  case "NONE":
                    this.JSSettings.LocalRenaming = LocalRenaming.KeepAll;
                    flag2 = true;
                    break;
                  case "NOPROPS":
                    this.JSSettings.ManualRenamesProperties = false;
                    break;
                  default:
                    this.OnInvalidSwitch(upperInvariant1, str2);
                    break;
                }
              }
              if (this.JSSettings.LocalRenaming != LocalRenaming.KeepAll)
              {
                this.ResetRenamingKill(killSpecified);
                if (flag3 && !this.JSSettings.MinifyCode)
                  this.OnInvalidSwitch(upperInvariant1, str2);
              }
              this.OnJSOnlyParameter();
              continue;
            case "REORDER":
              if (SwitchParser.BooleanSwitch(upperInvariant2, true, out booleanValue))
              {
                this.JSSettings.ReorderScopeDeclarations = booleanValue;
                if (booleanValue)
                {
                  this.m_noPretty = true;
                  if (this.PrettyPrint)
                    this.OnInvalidSwitch(upperInvariant1, str2);
                }
              }
              else
                this.OnInvalidSwitch(upperInvariant1, str2);
              this.OnJSOnlyParameter();
              continue;
            case "STRICT":
              if (SwitchParser.BooleanSwitch(upperInvariant2, true, out booleanValue))
                this.JSSettings.StrictMode = booleanValue;
              else
                this.OnInvalidSwitch(upperInvariant1, str2);
              this.OnJSOnlyParameter();
              continue;
            case "TERM":
              if (SwitchParser.BooleanSwitch(upperInvariant2, true, out booleanValue))
              {
                this.JSSettings.TermSemicolons = this.CssSettings.TermSemicolons = booleanValue;
                continue;
              }
              this.OnInvalidSwitch(upperInvariant1, str2);
              continue;
            case "UNUSED":
              switch (upperInvariant2)
              {
                case "KEEP":
                  this.JSSettings.RemoveUnneededCode = false;
                  break;
                case "REMOVE":
                  this.JSSettings.RemoveUnneededCode = true;
                  this.m_noPretty = true;
                  if (this.PrettyPrint)
                  {
                    this.OnInvalidSwitch(upperInvariant1, str2);
                    break;
                  }
                  break;
                default:
                  this.OnInvalidSwitch(upperInvariant1, str2);
                  break;
              }
              this.OnJSOnlyParameter();
              continue;
            case "VAR":
              this.m_noPretty = true;
              if (this.PrettyPrint || string.IsNullOrEmpty(upperInvariant2))
              {
                this.OnInvalidSwitch(upperInvariant1, str2);
              }
              else
              {
                string str14 = str2;
                string str15 = (string) null;
                int length = str2.IndexOf(',');
                if (length == 0)
                {
                  str14 = (string) null;
                  str15 = str2.Substring(length + 1);
                }
                else if (length > 0)
                {
                  str14 = str2.Substring(0, length);
                  str15 = str2.Substring(length + 1);
                }
                if (!string.IsNullOrEmpty(str14))
                  CrunchEnumerator.FirstLetters = str14;
                if (!string.IsNullOrEmpty(str15))
                  CrunchEnumerator.PartLetters = str15;
                else if (!string.IsNullOrEmpty(str14))
                  CrunchEnumerator.PartLetters = str14;
              }
              this.OnJSOnlyParameter();
              continue;
            case "WARN":
            case "W":
              if (string.IsNullOrEmpty(upperInvariant2))
              {
                this.WarningLevel = int.MaxValue;
              }
              else
              {
                int number5;
                if (str2.TryParseIntInvariant(NumberStyles.None, out number5))
                  this.WarningLevel = number5;
                else
                  this.OnInvalidSwitch(upperInvariant1, str2);
              }
              flag1 = true;
              continue;
            case "D":
              this.m_noPretty = true;
              if (this.PrettyPrint)
                this.OnInvalidSwitch(upperInvariant1, str2);
              this.JSSettings.StripDebugStatements = true;
              this.OnJSOnlyParameter();
              continue;
            case "E":
            case "EO":
              if (strArray1.Length < 2)
                this.OnInvalidSwitch(upperInvariant1, str2);
              this.EncodingOutputName = str2;
              continue;
            case "EI":
              if (strArray1.Length < 2)
                this.OnInvalidSwitch(upperInvariant1, str2);
              this.EncodingInputName = str2;
              continue;
            case "H":
            case "HC":
              this.m_noPretty = true;
              if (this.PrettyPrint)
                this.OnInvalidSwitch(upperInvariant1, str2);
              this.JSSettings.LocalRenaming = LocalRenaming.CrunchAll;
              this.JSSettings.RemoveUnneededCode = true;
              this.OnJSOnlyParameter();
              flag2 = true;
              this.ResetRenamingKill(killSpecified);
              if (flag3 && !this.JSSettings.MinifyCode)
              {
                this.OnInvalidSwitch(upperInvariant1, str2);
                continue;
              }
              continue;
            case "HL":
            case "HLC":
            case "HCL":
              this.m_noPretty = true;
              if (this.PrettyPrint)
                this.OnInvalidSwitch(upperInvariant1, str2);
              this.JSSettings.LocalRenaming = LocalRenaming.KeepLocalizationVars;
              this.JSSettings.RemoveUnneededCode = true;
              this.OnJSOnlyParameter();
              flag2 = true;
              this.ResetRenamingKill(killSpecified);
              if (flag3 && !this.JSSettings.MinifyCode)
              {
                this.OnInvalidSwitch(upperInvariant1, str2);
                continue;
              }
              continue;
            case "J":
              this.JSSettings.EvalTreatment = EvalTreatment.Ignore;
              this.OnJSOnlyParameter();
              continue;
            case "K":
              this.JSSettings.InlineSafeStrings = true;
              this.OnJSOnlyParameter();
              continue;
            case "L":
              this.JSSettings.CollapseToLiteral = false;
              this.OnJSOnlyParameter();
              continue;
            case "M":
              this.JSSettings.MacSafariQuirks = true;
              this.OnJSOnlyParameter();
              continue;
            case "Z":
              this.JSSettings.TermSemicolons = this.CssSettings.TermSemicolons = true;
              continue;
            default:
              index1 = this.OnUnknownParameter((IList<string>) args, index1, upperInvariant1, str2);
              continue;
          }
        }
        else
          index1 = this.OnUnknownParameter((IList<string>) args, index1, (string) null, (string) null);
      }
    }

    protected virtual int OnUnknownParameter(
      IList<string> arguments,
      int index,
      string switchPart,
      string parameterPart)
    {
      if (this.UnknownParameter != null)
      {
        UnknownParameterEventArgs e = new UnknownParameterEventArgs(arguments)
        {
          Index = index,
          SwitchPart = switchPart,
          ParameterPart = parameterPart
        };
        this.UnknownParameter((object) this, e);
        if (e.Index > index)
          index = e.Index;
      }
      return index;
    }

    protected virtual void OnInvalidSwitch(string switchPart, string parameterPart)
    {
      if (this.InvalidSwitch == null)
        return;
      this.InvalidSwitch((object) this, new InvalidSwitchEventArgs()
      {
        SwitchPart = switchPart,
        ParameterPart = parameterPart
      });
    }

    protected virtual void OnJSOnlyParameter()
    {
      if (this.JSOnlyParameter == null)
        return;
      this.JSOnlyParameter((object) this, new EventArgs());
    }

    protected virtual void OnCssOnlyParameter()
    {
      if (this.CssOnlyParameter == null)
        return;
      this.CssOnlyParameter((object) this, new EventArgs());
    }

    private static void AlignDebugDefine(
      bool stripDebugStatements,
      IDictionary<string, string> defines)
    {
      if (stripDebugStatements)
      {
        if (!defines.ContainsKey("DEBUG"))
          return;
        defines.Remove("DEBUG");
      }
      else
      {
        if (defines.ContainsKey("DEBUG"))
          return;
        defines.Add("debug", string.Empty);
      }
    }

    public static bool BooleanSwitch(string booleanText, bool defaultValue, out bool booleanValue)
    {
      bool flag = true;
      switch (booleanText)
      {
        case "Y":
        case "YES":
        case "T":
        case "TRUE":
        case "ON":
        case "1":
          booleanValue = true;
          break;
        case "N":
        case "NO":
        case "NONE":
        case "F":
        case "FALSE":
        case "OFF":
        case "0":
          booleanValue = false;
          break;
        case "":
        case null:
          booleanValue = defaultValue;
          break;
        default:
          booleanValue = defaultValue;
          flag = false;
          break;
      }
      return flag;
    }

    private void ResetRenamingKill(bool killSpecified)
    {
      if (killSpecified || this.JSSettings.KillSwitch == 0L)
        return;
      this.JSSettings.KillSwitch &= -16777217L;
    }
  }
}
