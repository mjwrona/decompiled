// Decompiled with JetBrains decompiler
// Type: Microsoft.CodeDom.Providers.DotNetCompilerPlatform.CSharpCompiler
// Assembly: Microsoft.CodeDom.Providers.DotNetCompilerPlatform, Version=4.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 4D7629DB-EBA2-4B9A-A3BA-C2E83ED1F745
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.CodeDom.Providers.DotNetCompilerPlatform.dll

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.CodeDom.Providers.DotNetCompilerPlatform
{
  internal class CSharpCompiler : Microsoft.CodeDom.Providers.DotNetCompilerPlatform.Compiler
  {
    private static volatile Regex outputRegWithFileAndLine;
    private static volatile Regex outputRegSimple;

    public CSharpCompiler(CodeDomProvider codeDomProvider, IProviderOptions providerOptions = null)
      : base(codeDomProvider, providerOptions)
    {
    }

    protected override string FileExtension => ".cs";

    protected override void ProcessCompilerOutputLine(CompilerResults results, string line)
    {
      if (CSharpCompiler.outputRegSimple == null)
      {
        CSharpCompiler.outputRegWithFileAndLine = new Regex("(^(.*)(\\(([0-9]+),([0-9]+)\\)): )(error|warning) ([A-Z]+[0-9]+) ?: (.*)");
        CSharpCompiler.outputRegSimple = new Regex("(error|warning) ([A-Z]+[0-9]+) ?: (.*)");
      }
      Match match = CSharpCompiler.outputRegWithFileAndLine.Match(line);
      bool flag;
      if (match.Success)
      {
        flag = true;
      }
      else
      {
        match = CSharpCompiler.outputRegSimple.Match(line);
        flag = false;
      }
      if (!match.Success)
        return;
      CompilerError compilerError = new CompilerError();
      if (flag)
      {
        compilerError.FileName = match.Groups[2].Value;
        compilerError.Line = int.Parse(match.Groups[4].Value, (IFormatProvider) CultureInfo.InvariantCulture);
        compilerError.Column = int.Parse(match.Groups[5].Value, (IFormatProvider) CultureInfo.InvariantCulture);
      }
      if (string.Compare(match.Groups[flag ? 6 : 1].Value, "warning", StringComparison.OrdinalIgnoreCase) == 0)
        compilerError.IsWarning = true;
      compilerError.ErrorNumber = match.Groups[flag ? 7 : 2].Value;
      compilerError.ErrorText = match.Groups[flag ? 8 : 3].Value;
      results.Errors.Add(compilerError);
    }

    protected override string FullPathsOption => " /fullpaths ";

    protected override void FixUpCompilerParameters(CompilerParameters options)
    {
      base.FixUpCompilerParameters(options);
      if (!this._providerOptions.UseAspNetSettings)
        return;
      List<string> values = new List<string>(5);
      values.AddRange((IEnumerable<string>) new string[3]
      {
        "1659",
        "1699",
        "1701"
      });
      values.Add("612");
      values.Add("618");
      CompilationUtil.PrependCompilerOption(options, "/nowarn:" + string.Join(";", (IEnumerable<string>) values));
    }

    protected override string CmdArgsFromParameters(CompilerParameters parameters)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (parameters.GenerateExecutable)
      {
        stringBuilder.Append("/t:exe ");
        if (parameters.MainClass != null && parameters.MainClass.Length > 0)
        {
          stringBuilder.Append("/main:");
          stringBuilder.Append(parameters.MainClass);
          stringBuilder.Append(" ");
        }
      }
      else
        stringBuilder.Append("/t:library ");
      stringBuilder.Append("/utf8output ");
      string location = typeof (object).Assembly.Location;
      string str1 = parameters.CoreAssemblyFileName;
      if (string.IsNullOrWhiteSpace(str1))
        str1 = location;
      if (!string.IsNullOrWhiteSpace(str1))
      {
        stringBuilder.Append("/nostdlib+ ");
        stringBuilder.Append("/R:\"").Append(str1.Trim()).Append("\" ");
      }
      string str2 = (string) null;
      try
      {
        str2 = Assembly.Load("System.Runtime, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a").Location;
      }
      catch
      {
      }
      if (str2 != null)
        stringBuilder.Append(string.Format("/R:\"{0}\" ", (object) str2));
      foreach (string referencedAssembly in parameters.ReferencedAssemblies)
      {
        stringBuilder.Append("/R:");
        stringBuilder.Append("\"");
        stringBuilder.Append(referencedAssembly);
        stringBuilder.Append("\"");
        stringBuilder.Append(" ");
      }
      stringBuilder.Append("/out:");
      stringBuilder.Append("\"");
      stringBuilder.Append(parameters.OutputAssembly);
      stringBuilder.Append("\"");
      stringBuilder.Append(" ");
      if (parameters.IncludeDebugInformation)
      {
        stringBuilder.Append("/D:DEBUG ");
        stringBuilder.Append("/debug+ ");
        stringBuilder.Append("/optimize- ");
      }
      else
      {
        stringBuilder.Append("/debug- ");
        stringBuilder.Append("/optimize+ ");
      }
      if (parameters.Win32Resource != null)
        stringBuilder.Append("/win32res:\"" + parameters.Win32Resource + "\" ");
      foreach (string embeddedResource in parameters.EmbeddedResources)
      {
        stringBuilder.Append("/res:\"");
        stringBuilder.Append(embeddedResource);
        stringBuilder.Append("\" ");
      }
      foreach (string linkedResource in parameters.LinkedResources)
      {
        stringBuilder.Append("/linkres:\"");
        stringBuilder.Append(linkedResource);
        stringBuilder.Append("\" ");
      }
      if (parameters.TreatWarningsAsErrors)
        stringBuilder.Append("/warnaserror+ ");
      else
        stringBuilder.Append("/warnaserror- ");
      if (parameters.WarningLevel >= 0)
        stringBuilder.Append("/w:" + parameters.WarningLevel.ToString() + " ");
      if (parameters.CompilerOptions != null)
        stringBuilder.Append(parameters.CompilerOptions + " ");
      return stringBuilder.ToString();
    }
  }
}
