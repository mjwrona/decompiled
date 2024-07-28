// Decompiled with JetBrains decompiler
// Type: Microsoft.CodeDom.Providers.DotNetCompilerPlatform.VBCompiler
// Assembly: Microsoft.CodeDom.Providers.DotNetCompilerPlatform, Version=4.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 4D7629DB-EBA2-4B9A-A3BA-C2E83ED1F745
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.CodeDom.Providers.DotNetCompilerPlatform.dll

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Configuration;

namespace Microsoft.CodeDom.Providers.DotNetCompilerPlatform
{
  internal class VBCompiler : Microsoft.CodeDom.Providers.DotNetCompilerPlatform.Compiler
  {
    internal static string MySupport = "/define:_MYTYPE=\\\"Web\\\"";
    internal static string VBImportsString;
    private static volatile Regex outputReg;

    public VBCompiler(CodeDomProvider codeDomProvider, IProviderOptions providerOptions = null)
      : base(codeDomProvider, providerOptions)
    {
    }

    protected override string FileExtension => ".vb";

    protected override void ProcessCompilerOutputLine(CompilerResults results, string line)
    {
      if (VBCompiler.outputReg == null)
        VBCompiler.outputReg = new Regex("^([^(]*)\\(?([0-9]*)\\)? ?:? ?(error|warning) ([A-Z]+[0-9]+) ?: ((.|\\n)*)");
      Match match = VBCompiler.outputReg.Match(line);
      if (!match.Success)
        return;
      CompilerError compilerError = new CompilerError();
      compilerError.FileName = match.Groups[1].Value;
      string s = match.Groups[2].Value;
      if (s != null && s.Length > 0)
        compilerError.Line = int.Parse(s, (IFormatProvider) CultureInfo.InvariantCulture);
      if (string.Compare(match.Groups[3].Value, "warning", StringComparison.OrdinalIgnoreCase) == 0)
        compilerError.IsWarning = true;
      compilerError.ErrorNumber = match.Groups[4].Value;
      compilerError.ErrorText = match.Groups[5].Value;
      results.Errors.Add(compilerError);
    }

    protected override void FixUpCompilerParameters(CompilerParameters options)
    {
      base.FixUpCompilerParameters(options);
      if (!this._providerOptions.UseAspNetSettings)
        return;
      CompilationUtil.PrependCompilerOption(options, " /optionInfer+");
      List<string> values = new List<string>(3);
      VBCompiler.AddVBGlobalNamespaceImports(options);
      VBCompiler.AddVBMyFlags(options);
      values.Add("41008");
      values.Add("40000");
      values.Add("40008");
      if (values.Count <= 0)
        return;
      CompilationUtil.PrependCompilerOption(options, "/nowarn:" + string.Join(",", (IEnumerable<string>) values));
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
      string str1 = parameters.CoreAssemblyFileName;
      string coreAssemblyFilePath;
      if (string.IsNullOrWhiteSpace(parameters.CoreAssemblyFileName) && VBCompiler.TryGetProbableCoreAssemblyFilePath(parameters, out coreAssemblyFilePath))
        str1 = coreAssemblyFilePath;
      if (!string.IsNullOrWhiteSpace(str1))
      {
        string path = str1.Trim();
        string directoryName = Path.GetDirectoryName(path);
        stringBuilder.Append("/nostdlib ");
        stringBuilder.Append("/sdkpath:\"").Append(directoryName).Append("\" ");
        stringBuilder.Append("/R:\"").Append(path).Append("\" ");
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
        string fileName = Path.GetFileName(referencedAssembly);
        if (string.Compare(fileName, "Microsoft.VisualBasic.dll", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(fileName, "mscorlib.dll", StringComparison.OrdinalIgnoreCase) != 0)
        {
          stringBuilder.Append("/R:");
          stringBuilder.Append("\"");
          stringBuilder.Append(referencedAssembly);
          stringBuilder.Append("\"");
          stringBuilder.Append(" ");
        }
      }
      stringBuilder.Append("/out:");
      stringBuilder.Append("\"");
      stringBuilder.Append(parameters.OutputAssembly);
      stringBuilder.Append("\"");
      stringBuilder.Append(" ");
      if (parameters.IncludeDebugInformation)
      {
        stringBuilder.Append("/D:DEBUG=1 ");
        stringBuilder.Append("/debug+ ");
      }
      else
        stringBuilder.Append("/debug- ");
      if (parameters.Win32Resource != null)
        stringBuilder.Append("/win32resource:\"" + parameters.Win32Resource + "\" ");
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
      if (parameters.CompilerOptions != null)
        stringBuilder.Append(parameters.CompilerOptions + " ");
      return stringBuilder.ToString();
    }

    protected override string FullPathsOption => "";

    internal static bool TryGetProbableCoreAssemblyFilePath(
      CompilerParameters parameters,
      out string coreAssemblyFilePath)
    {
      string str1 = (string) null;
      char[] separator = new char[1]
      {
        Path.DirectorySeparatorChar
      };
      string str2 = Path.Combine("Reference Assemblies", "Microsoft", "Framework");
      foreach (string referencedAssembly in parameters.ReferencedAssemblies)
      {
        if (Path.GetFileName(referencedAssembly).Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase))
        {
          coreAssemblyFilePath = string.Empty;
          return false;
        }
        if (referencedAssembly.IndexOf(str2, StringComparison.OrdinalIgnoreCase) >= 0)
        {
          string[] strArray = referencedAssembly.Split(separator, StringSplitOptions.RemoveEmptyEntries);
          for (int index = 0; index < strArray.Length - 5; ++index)
          {
            if (string.Equals(strArray[index], "Reference Assemblies", StringComparison.OrdinalIgnoreCase) && strArray[index + 4].StartsWith("v", StringComparison.OrdinalIgnoreCase))
            {
              if (str1 != null)
              {
                if (!string.Equals(str1, Path.GetDirectoryName(referencedAssembly), StringComparison.OrdinalIgnoreCase))
                {
                  coreAssemblyFilePath = string.Empty;
                  return false;
                }
              }
              else
                str1 = Path.GetDirectoryName(referencedAssembly);
            }
          }
        }
      }
      if (str1 != null)
      {
        coreAssemblyFilePath = Path.Combine(str1, "mscorlib.dll");
        return true;
      }
      coreAssemblyFilePath = string.Empty;
      return false;
    }

    private static void AddVBMyFlags(CompilerParameters compilParams)
    {
      if (compilParams.CompilerOptions == null)
        compilParams.CompilerOptions = VBCompiler.MySupport;
      else
        compilParams.CompilerOptions = VBCompiler.MySupport + " " + compilParams.CompilerOptions;
    }

    private static void AddVBGlobalNamespaceImports(CompilerParameters compilParams)
    {
      if (VBCompiler.VBImportsString == null)
      {
        PagesSection section = (PagesSection) WebConfigurationManager.GetSection("system.web/pages");
        if (section.Namespaces == null)
        {
          VBCompiler.VBImportsString = string.Empty;
        }
        else
        {
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.Append("/imports:");
          bool flag = false;
          if (section.Namespaces.AutoImportVBNamespace)
          {
            stringBuilder.Append("Microsoft.VisualBasic");
            flag = true;
          }
          foreach (NamespaceInfo namespaceInfo in (ConfigurationElementCollection) section.Namespaces)
          {
            if (flag)
              stringBuilder.Append(',');
            stringBuilder.Append(namespaceInfo.Namespace);
            flag = true;
          }
          VBCompiler.VBImportsString = stringBuilder.ToString();
        }
      }
      if (VBCompiler.VBImportsString.Length <= 0)
        return;
      CompilationUtil.PrependCompilerOption(compilParams, VBCompiler.VBImportsString);
    }
  }
}
