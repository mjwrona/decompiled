// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.AspNetCompiler
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Compilation;
using System.Xml.Linq;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public class AspNetCompiler
  {
    private static readonly string s_area = "WarmUp";
    private static readonly string s_layer = "AspnetCompile";

    public void PrecompileSite(
      string appPhysicalSourceDir,
      IEnumerable<string> excludedVirtualPaths,
      bool compileIfSiteNotFound,
      bool forceCleanBuild,
      ITFLogger logger)
    {
      List<string> iisMetabasePaths = AspNetCompiler.GetIisMetabasePaths(appPhysicalSourceDir);
      if (iisMetabasePaths.Count > 0)
      {
        foreach (string applicationId in iisMetabasePaths)
          this.PrecompileApplication(applicationId, excludedVirtualPaths, forceCleanBuild, logger);
      }
      else if (compileIfSiteNotFound)
      {
        string str1 = Path.Combine(appPhysicalSourceDir, "web.config");
        try
        {
          if (File.Exists(str1))
          {
            str1 = (string) null;
          }
          else
          {
            string str2 = str1 + ".azure.template";
            if (!File.Exists(str2))
              str2 = str1 + ".template";
            File.Copy(str2, str1);
          }
          ClientBuildManagerParameter parameter = new ClientBuildManagerParameter();
          if (excludedVirtualPaths != null)
            parameter.ExcludedVirtualPaths.AddRange(excludedVirtualPaths);
          if (forceCleanBuild)
            parameter.PrecompilationFlags |= PrecompilationFlags.Clean;
          ClientBuildManager clientBuildManager = new ClientBuildManager("/", appPhysicalSourceDir, (string) null, parameter);
          logger.Info("CodeGenDir: {0}", (object) clientBuildManager.CodeGenDir);
          clientBuildManager.PrecompileApplication((ClientBuildManagerCallback) new AspNetCompiler.MyClientBuildManagerCallack(logger), forceCleanBuild);
        }
        finally
        {
          if (str1 != null)
            File.Delete(str1);
        }
      }
      else
        logger.Warning(string.Format("No sites were found for the following path: {0}", (object) appPhysicalSourceDir));
    }

    public string PrecompileApplication(
      string applicationId,
      IEnumerable<string> excludedVirtualPaths,
      bool forceCleanBuild,
      ITFLogger logger)
    {
      logger.Info("Compiling web application: {0}", (object) applicationId);
      Stopwatch stopwatch = Stopwatch.StartNew();
      ClientBuildManagerParameter parameter = new ClientBuildManagerParameter();
      if (excludedVirtualPaths != null)
        parameter.ExcludedVirtualPaths.AddRange(excludedVirtualPaths);
      if (forceCleanBuild)
        parameter.PrecompilationFlags |= PrecompilationFlags.Clean;
      ClientBuildManager clientBuildManager = new ClientBuildManager(applicationId, (string) null, (string) null, parameter);
      logger.Info("CodeGenDir: {0}", (object) clientBuildManager.CodeGenDir);
      int num1 = 0;
      while (true)
      {
        try
        {
          clientBuildManager.PrecompileApplication((ClientBuildManagerCallback) new AspNetCompiler.MyClientBuildManagerCallack(logger), forceCleanBuild);
          logger.Info("Web application compilation completed. AppId: {0}, Elapsed: {1}", (object) applicationId, (object) stopwatch.Elapsed);
          break;
        }
        catch (Exception ex)
        {
          logger.Error(ex);
          bool flag1 = true;
          bool flag2 = false;
          if (num1 <= 3)
          {
            if (ex is FileNotFoundException)
              flag1 = false;
            if (flag1 && ex is IOException && HResult.IsFileInUse(ex.HResult))
              flag1 = false;
            if (flag1)
            {
              int num2;
              if (!(ex is HttpParseException httpParseException))
              {
                num2 = 0;
              }
              else
              {
                int? nullable = httpParseException.Message?.IndexOf("Circular file references are not allowed");
                int num3 = 0;
                num2 = nullable.GetValueOrDefault() >= num3 & nullable.HasValue ? 1 : 0;
              }
              if (num2 != 0)
                flag1 = false;
            }
            if (flag1)
            {
              int num4;
              if (!(ex is NullReferenceException referenceException))
              {
                num4 = 0;
              }
              else
              {
                int? nullable = referenceException.StackTrace?.IndexOf("ReportDirectoryCompilationProgress", StringComparison.Ordinal);
                int num5 = 0;
                num4 = nullable.GetValueOrDefault() >= num5 & nullable.HasValue ? 1 : 0;
              }
              if (num4 != 0)
              {
                flag1 = false;
                flag2 = true;
                TeamFoundationTracingService.TraceRaw(10025024, TraceLevel.Warning, AspNetCompiler.s_area, AspNetCompiler.s_layer, "NR in the ReportDirectoryCompilationProgress");
              }
            }
            if (flag1 && ex is HttpCompileException compileException && compileException.HResult == -2147024891)
              flag1 = false;
          }
          if (!flag2)
            TeamFoundationTracingService.TraceExceptionRaw(10025023, AspNetCompiler.s_area, AspNetCompiler.s_layer, ex);
          if (flag1)
            throw;
          else
            Thread.Sleep(num1 * 2000 + new Random().Next(900));
        }
        ++num1;
      }
      return clientBuildManager.CodeGenDir;
    }

    public static List<string> GetIisMetabasePaths(string appPhysicalSourceDir)
    {
      List<string> iisMetabasePaths = new List<string>(1);
      foreach (XElement xelement in XDocument.Load(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "inetsrv\\config\\applicationHost.config")).XPathSelectElements("/configuration/system.applicationHost/sites/site/application/virtualDirectory").ToArray<XElement>())
      {
        if (xelement.Attribute((XName) "physicalPath").Value.Equals(appPhysicalSourceDir, StringComparison.OrdinalIgnoreCase))
        {
          XElement parent1 = xelement.Parent;
          XElement parent2 = parent1.Parent;
          string a1 = xelement.Attribute((XName) "path").Value;
          string a2 = parent1.Attribute((XName) "path").Value;
          string str = string.Format("/LM/W3SVC/{0}/ROOT", (object) parent2.Attribute((XName) "id").Value);
          if (!string.Equals(a2, "/", StringComparison.Ordinal))
            str += a2;
          if (!string.Equals(a1, "/", StringComparison.OrdinalIgnoreCase))
            str += a1;
          iisMetabasePaths.Add(str);
        }
      }
      return iisMetabasePaths;
    }

    internal class MyClientBuildManagerCallack : ClientBuildManagerCallback
    {
      private readonly ITFLogger m_logger;

      public MyClientBuildManagerCallack(ITFLogger logger) => this.m_logger = logger;

      public override void ReportProgress(string message) => this.m_logger.Info(message);

      public override void ReportCompilerError(CompilerError error) => this.DumpCompileError(error);

      public override void ReportParseError(ParserError error) => this.DumpError(error.VirtualPath, error.Line, false, "ASPPARSE", error.ErrorText);

      private void DumpCompileError(CompilerError error) => this.DumpError(error.FileName, error.Line, error.IsWarning, error.ErrorNumber, error.ErrorText);

      private void DumpError(
        string fileName,
        int line,
        bool warning,
        string errorNumber,
        string message)
      {
        StringWriter stringWriter = new StringWriter();
        if (fileName != null)
        {
          stringWriter.Write(fileName);
          stringWriter.Write("(" + line.ToString() + "): ");
        }
        if (warning)
          stringWriter.Write("warning ");
        else
          stringWriter.Write("error ");
        stringWriter.Write(errorNumber + ": ");
        stringWriter.WriteLine(message);
        string message1 = stringWriter.ToString();
        if (warning)
          this.m_logger.Warning(message1);
        else
          this.m_logger.Error(message1);
      }
    }
  }
}
