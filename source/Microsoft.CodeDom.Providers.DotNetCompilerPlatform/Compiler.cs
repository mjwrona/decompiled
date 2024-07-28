// Decompiled with JetBrains decompiler
// Type: Microsoft.CodeDom.Providers.DotNetCompilerPlatform.Compiler
// Assembly: Microsoft.CodeDom.Providers.DotNetCompilerPlatform, Version=4.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 4D7629DB-EBA2-4B9A-A3BA-C2E83ED1F745
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.CodeDom.Providers.DotNetCompilerPlatform.dll

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;

namespace Microsoft.CodeDom.Providers.DotNetCompilerPlatform
{
  internal abstract class Compiler : ICodeCompiler
  {
    private readonly CodeDomProvider _codeDomProvider;
    protected readonly IProviderOptions _providerOptions;
    private string _compilerFullPath;
    private const string CLR_PROFILING_SETTING = "COR_ENABLE_PROFILING";
    private const string DISABLE_PROFILING = "0";

    public Compiler(CodeDomProvider codeDomProvider, IProviderOptions providerOptions)
    {
      this._codeDomProvider = codeDomProvider;
      this._providerOptions = providerOptions;
    }

    public CompilerResults CompileAssemblyFromDom(
      CompilerParameters options,
      CodeCompileUnit compilationUnit)
    {
      if (options == null)
        throw new ArgumentNullException(nameof (options));
      return compilationUnit != null ? this.CompileAssemblyFromDomBatch(options, new CodeCompileUnit[1]
      {
        compilationUnit
      }) : throw new ArgumentNullException(nameof (compilationUnit));
    }

    public CompilerResults CompileAssemblyFromDomBatch(
      CompilerParameters options,
      CodeCompileUnit[] compilationUnits)
    {
      if (options == null)
        throw new ArgumentNullException(nameof (options));
      if (compilationUnits == null)
        throw new ArgumentNullException(nameof (compilationUnits));
      try
      {
        IEnumerable<string> source = ((IEnumerable<CodeCompileUnit>) compilationUnits).Select<CodeCompileUnit, string>((Func<CodeCompileUnit, string>) (c =>
        {
          StringWriter writer = new StringWriter();
          this._codeDomProvider.GenerateCodeFromCompileUnit(c, (TextWriter) writer, new CodeGeneratorOptions());
          return writer.ToString();
        }));
        return this.FromSourceBatch(options, source.ToArray<string>());
      }
      finally
      {
        options.TempFiles.Delete();
      }
    }

    public CompilerResults CompileAssemblyFromFile(CompilerParameters options, string fileName)
    {
      if (options == null)
        throw new ArgumentNullException(nameof (options));
      return fileName != null ? this.CompileAssemblyFromFileBatch(options, new string[1]
      {
        fileName
      }) : throw new ArgumentNullException(nameof (fileName));
    }

    public CompilerResults CompileAssemblyFromFileBatch(
      CompilerParameters options,
      string[] fileNames)
    {
      if (options == null)
        throw new ArgumentNullException(nameof (options));
      if (fileNames == null)
        throw new ArgumentNullException(nameof (fileNames));
      try
      {
        foreach (string fileName in fileNames)
        {
          using (File.OpenRead(fileName))
            ;
        }
        return this.FromFileBatch(options, fileNames);
      }
      finally
      {
        options.TempFiles.Delete();
      }
    }

    public CompilerResults CompileAssemblyFromSource(CompilerParameters options, string source)
    {
      if (options == null)
        throw new ArgumentNullException(nameof (options));
      return source != null ? this.CompileAssemblyFromSourceBatch(options, new string[1]
      {
        source
      }) : throw new ArgumentNullException(nameof (source));
    }

    public CompilerResults CompileAssemblyFromSourceBatch(
      CompilerParameters options,
      string[] sources)
    {
      if (options == null)
        throw new ArgumentNullException(nameof (options));
      if (sources == null)
        throw new ArgumentNullException(nameof (sources));
      try
      {
        return this.FromSourceBatch(options, sources);
      }
      finally
      {
        options.TempFiles.Delete();
      }
    }

    protected abstract string FileExtension { get; }

    protected virtual string CompilerName
    {
      get
      {
        if (this._compilerFullPath == null)
        {
          this._compilerFullPath = this._providerOptions.CompilerFullPath;
          using (File.OpenRead(this._compilerFullPath))
            ;
        }
        return this._compilerFullPath;
      }
    }

    protected abstract void ProcessCompilerOutputLine(CompilerResults results, string line);

    protected abstract string CmdArgsFromParameters(CompilerParameters options);

    protected abstract string FullPathsOption { get; }

    protected virtual void FixUpCompilerParameters(CompilerParameters options) => this.FixTreatWarningsAsErrors(options);

    private string GetCompilationArgumentString(CompilerParameters options)
    {
      this.FixUpCompilerParameters(options);
      return this.CmdArgsFromParameters(options);
    }

    private void FixTreatWarningsAsErrors(CompilerParameters parameters) => parameters.TreatWarningsAsErrors = this._providerOptions.WarnAsError;

    private CompilerResults FromSourceBatch(CompilerParameters options, string[] sources)
    {
      if (options == null)
        throw new ArgumentNullException(nameof (options));
      if (sources == null)
        throw new ArgumentNullException(nameof (sources));
      new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
      string[] fileNames = new string[sources.Length];
      try
      {
        WindowsImpersonationContext impersonation = Microsoft.CodeDom.Providers.DotNetCompilerPlatform.Compiler.RevertImpersonation();
        try
        {
          for (int index = 0; index < sources.Length; ++index)
          {
            string path = options.TempFiles.AddExtension(index.ToString() + this.FileExtension);
            FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read);
            try
            {
              using (StreamWriter streamWriter = new StreamWriter((Stream) fileStream, Encoding.UTF8))
              {
                streamWriter.Write(sources[index]);
                streamWriter.Flush();
              }
            }
            finally
            {
              fileStream.Close();
            }
            fileNames[index] = path;
          }
          return this.FromFileBatch(options, fileNames);
        }
        finally
        {
          Microsoft.CodeDom.Providers.DotNetCompilerPlatform.Compiler.ReImpersonate(impersonation);
        }
      }
      catch
      {
        throw;
      }
    }

    private CompilerResults FromFileBatch(CompilerParameters options, string[] fileNames)
    {
      if (options == null)
        throw new ArgumentNullException(nameof (options));
      if (fileNames == null)
        throw new ArgumentNullException(nameof (fileNames));
      new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
      string outputFile = (string) null;
      int nativeReturnValue = 0;
      CompilerResults results = new CompilerResults(options.TempFiles);
      new SecurityPermission(SecurityPermissionFlag.ControlEvidence).Assert();
      try
      {
        results.Evidence = options.Evidence;
      }
      finally
      {
        CodeAccessPermission.RevertAssert();
      }
      bool flag1 = false;
      if (options.OutputAssembly == null || options.OutputAssembly.Length == 0)
      {
        string fileExtension = options.GenerateExecutable ? "exe" : "dll";
        options.OutputAssembly = results.TempFiles.AddExtension(fileExtension, !options.GenerateInMemory);
        new FileStream(options.OutputAssembly, FileMode.Create, FileAccess.ReadWrite).Close();
        flag1 = true;
      }
      string fileExtension1 = "pdb";
      if (options.CompilerOptions != null && -1 != CultureInfo.InvariantCulture.CompareInfo.IndexOf(options.CompilerOptions, "/debug:pdbonly", CompareOptions.IgnoreCase))
        results.TempFiles.AddExtension(fileExtension1, true);
      else
        results.TempFiles.AddExtension(fileExtension1);
      string str1 = this.GetCompilationArgumentString(options) + " " + Microsoft.CodeDom.Providers.DotNetCompilerPlatform.Compiler.JoinStringArray(fileNames, " ");
      string responseFileCmdArgs = this.GetResponseFileCmdArgs(options, str1);
      string str2 = (string) null;
      if (responseFileCmdArgs != null)
      {
        str2 = str1;
        str1 = responseFileCmdArgs;
      }
      if (this._providerOptions.CompilerServerTimeToLive > 0)
        str1 = string.Format("/shared /keepalive:\"{0}\" {1}", (object) this._providerOptions.CompilerServerTimeToLive, (object) str1);
      this.Compile(options, this.CompilerName, str1, ref outputFile, ref nativeReturnValue);
      results.NativeCompilerReturnValue = nativeReturnValue;
      if (nativeReturnValue != 0 || options.WarningLevel > 0)
      {
        string[] strArray = Microsoft.CodeDom.Providers.DotNetCompilerPlatform.Compiler.ReadAllLines(outputFile, Encoding.UTF8, FileShare.ReadWrite);
        bool flag2 = false;
        foreach (string line in strArray)
        {
          if (!flag2 && str2 != null && line.Contains(str1))
          {
            flag2 = true;
            string str3 = string.Format("{0}>{1} {2}", (object) Environment.CurrentDirectory, (object) this.CompilerName, (object) str2);
            results.Output.Add(str3);
          }
          else
            results.Output.Add(line);
          this.ProcessCompilerOutputLine(results, line);
        }
        if (nativeReturnValue != 0 & flag1)
          File.Delete(options.OutputAssembly);
      }
      if (nativeReturnValue != 0 || results.Errors.HasErrors || !options.GenerateInMemory)
      {
        results.PathToAssembly = options.OutputAssembly;
        return results;
      }
      byte[] rawAssembly = File.ReadAllBytes(options.OutputAssembly);
      byte[] rawSymbolStore = (byte[]) null;
      try
      {
        string path = options.TempFiles.BasePath + "." + fileExtension1;
        if (File.Exists(path))
          rawSymbolStore = File.ReadAllBytes(path);
      }
      catch
      {
        rawSymbolStore = (byte[]) null;
      }
      new SecurityPermission(SecurityPermissionFlag.ControlEvidence).Assert();
      try
      {
        results.CompiledAssembly = Assembly.Load(rawAssembly, rawSymbolStore, options.Evidence);
      }
      finally
      {
        CodeAccessPermission.RevertAssert();
      }
      return results;
    }

    private static void ReImpersonate(WindowsImpersonationContext impersonation) => impersonation.Undo();

    [PermissionSet(SecurityAction.LinkDemand, Unrestricted = true)]
    [SecurityPermission(SecurityAction.Assert, ControlPrincipal = true, UnmanagedCode = true)]
    private static WindowsImpersonationContext RevertImpersonation() => WindowsIdentity.Impersonate(new IntPtr(0));

    private static string[] ReadAllLines(string file, Encoding encoding, FileShare share)
    {
      using (FileStream fileStream = File.Open(file, FileMode.Open, FileAccess.Read, share))
      {
        List<string> stringList = new List<string>();
        using (StreamReader streamReader = new StreamReader((Stream) fileStream, encoding))
        {
          string str;
          while ((str = streamReader.ReadLine()) != null)
            stringList.Add(str);
        }
        return stringList.ToArray();
      }
    }

    private void Compile(
      CompilerParameters options,
      string compilerFullPath,
      string arguments,
      ref string outputFile,
      ref int nativeReturnValue)
    {
      string errorName = (string) null;
      string cmd = "\"" + compilerFullPath + "\" " + arguments;
      outputFile = options.TempFiles.AddExtension("out");
      bool flag = false;
      string str = (string) null;
      if (AppSettings.DisableProfilingDuringCompilation)
      {
        str = Environment.GetEnvironmentVariable("COR_ENABLE_PROFILING", EnvironmentVariableTarget.Process);
        if (str != "0")
        {
          Environment.SetEnvironmentVariable("COR_ENABLE_PROFILING", "0", EnvironmentVariableTarget.Process);
          flag = true;
        }
      }
      nativeReturnValue = Executor.ExecWaitWithCapture(options.UserToken, cmd, Environment.CurrentDirectory, options.TempFiles, ref outputFile, ref errorName);
      if (!flag)
        return;
      Environment.SetEnvironmentVariable("COR_ENABLE_PROFILING", str, EnvironmentVariableTarget.Process);
    }

    private string GetResponseFileCmdArgs(CompilerParameters options, string cmdArgs)
    {
      string path = options.TempFiles.AddExtension("cmdline");
      FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read);
      try
      {
        using (StreamWriter streamWriter = new StreamWriter((Stream) fileStream, Encoding.UTF8))
        {
          streamWriter.Write(cmdArgs);
          streamWriter.Flush();
        }
      }
      finally
      {
        fileStream.Close();
      }
      return "/noconfig " + this.FullPathsOption + "@\"" + path + "\"";
    }

    private static string JoinStringArray(string[] sa, string separator)
    {
      if (sa == null || sa.Length == 0)
        return string.Empty;
      if (sa.Length == 1)
        return "\"" + sa[0] + "\"";
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < sa.Length - 1; ++index)
      {
        stringBuilder.Append("\"");
        stringBuilder.Append(sa[index]);
        stringBuilder.Append("\"");
        stringBuilder.Append(separator);
      }
      stringBuilder.Append("\"");
      stringBuilder.Append(sa[sa.Length - 1]);
      stringBuilder.Append("\"");
      return stringBuilder.ToString();
    }
  }
}
