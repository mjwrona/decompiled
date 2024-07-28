// Decompiled with JetBrains decompiler
// Type: Microsoft.CodeDom.Providers.DotNetCompilerPlatform.CompilationUtil
// Assembly: Microsoft.CodeDom.Providers.DotNetCompilerPlatform, Version=4.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 4D7629DB-EBA2-4B9A-A3BA-C2E83ED1F745
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.CodeDom.Providers.DotNetCompilerPlatform.dll

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Microsoft.CodeDom.Providers.DotNetCompilerPlatform
{
  internal static class CompilationUtil
  {
    private const int DefaultCompilerServerTTL = 10;
    private const int DefaultCompilerServerTTLInDevEnvironment = 900;

    static CompilationUtil()
    {
      if (!CompilationUtil.IsDebuggerAttached)
        return;
      Environment.SetEnvironmentVariable("IN_DEBUG_MODE", "1", EnvironmentVariableTarget.Process);
    }

    public static IProviderOptions CSC2 { get; } = CompilationUtil.GetProviderOptionsFor(".cs");

    public static IProviderOptions VBC2 { get; } = CompilationUtil.GetProviderOptionsFor(".vb");

    internal static IProviderOptions CreateProviderOptions(
      IDictionary<string, string> options,
      IProviderOptions baseOptions)
    {
      Dictionary<string, string> dictionary = (Dictionary<string, string>) null;
      ProviderOptions providerOptions = new ProviderOptions(baseOptions);
      foreach (KeyValuePair<string, string> option in (IEnumerable<KeyValuePair<string, string>>) options)
      {
        if (!string.IsNullOrWhiteSpace(option.Key))
        {
          switch (option.Key)
          {
            case "CompilerFullPath":
              providerOptions.CompilerFullPath = option.Value;
              continue;
            case "CompilerServerTimeToLive":
              int result1;
              if (int.TryParse(option.Value, out result1))
              {
                providerOptions.CompilerServerTimeToLive = result1;
                continue;
              }
              continue;
            case "CompilerVersion":
              providerOptions.CompilerVersion = option.Value;
              continue;
            case "WarnAsError":
              bool result2;
              if (bool.TryParse(option.Value, out result2))
              {
                providerOptions.WarnAsError = result2;
                continue;
              }
              continue;
            case "AllOptions":
              dictionary = dictionary ?? new Dictionary<string, string>(providerOptions.AllOptions);
              dictionary.Remove(option.Key);
              dictionary.Add(option.Key, option.Value);
              continue;
            default:
              continue;
          }
        }
      }
      if (dictionary != null)
        providerOptions.AllOptions = (IDictionary<string, string>) dictionary;
      return (IProviderOptions) providerOptions;
    }

    public static IProviderOptions GetProviderOptionsFor(string fileExt)
    {
      IDictionary<string, string> optionsCollection = CompilationUtil.GetProviderOptionsCollection(fileExt);
      string path1 = Environment.GetEnvironmentVariable("ROSLYN_COMPILER_LOCATION");
      if (string.IsNullOrEmpty(path1))
        path1 = AppSettings.RoslynCompilerLocation;
      if (string.IsNullOrEmpty(path1))
        optionsCollection.TryGetValue("CompilerLocation", out path1);
      if (string.IsNullOrEmpty(path1))
        path1 = CompilationUtil.CompilerDefaultPath();
      if (!string.IsNullOrWhiteSpace(fileExt))
      {
        if (fileExt.Equals(".cs", StringComparison.InvariantCultureIgnoreCase) || fileExt.Equals("cs", StringComparison.InvariantCultureIgnoreCase))
          path1 = Path.Combine(path1, "csc.exe");
        else if (fileExt.Equals(".vb", StringComparison.InvariantCultureIgnoreCase) || fileExt.Equals("vb", StringComparison.InvariantCultureIgnoreCase))
          path1 = Path.Combine(path1, "vbc.exe");
      }
      string environmentVariable = Environment.GetEnvironmentVariable("VBCSCOMPILER_TTL");
      if (string.IsNullOrEmpty(environmentVariable))
        optionsCollection.TryGetValue("CompilerServerTTL", out environmentVariable);
      int result1;
      if (!int.TryParse(environmentVariable, out result1))
      {
        result1 = 10;
        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DEV_ENVIRONMENT")) || !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("IN_DEBUG_MODE")) || CompilationUtil.IsDebuggerAttached)
          result1 = 900;
      }
      string str1;
      optionsCollection.TryGetValue("CompilerVersion", out str1);
      bool result2 = false;
      string str2;
      if (optionsCollection.TryGetValue("WarnAsError", out str2))
        bool.TryParse(str2, out result2);
      bool result3 = true;
      string str3;
      if (optionsCollection.TryGetValue("UseAspNetSettings", out str3) && !bool.TryParse(str3, out result3))
        result3 = true;
      return (IProviderOptions) new ProviderOptions()
      {
        CompilerFullPath = path1,
        CompilerServerTimeToLive = result1,
        CompilerVersion = str1,
        WarnAsError = result2,
        UseAspNetSettings = result3,
        AllOptions = optionsCollection
      };
    }

    internal static IDictionary<string, string> GetProviderOptionsCollection(string fileExt)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      if (!CodeDomProvider.IsDefinedExtension(fileExt))
        return (IDictionary<string, string>) new ReadOnlyDictionary<string, string>((IDictionary<string, string>) dictionary);
      CompilerInfo compilerInfo = CodeDomProvider.GetCompilerInfo(CodeDomProvider.GetLanguageFromExtension(fileExt));
      if (compilerInfo == null)
        return (IDictionary<string, string>) new ReadOnlyDictionary<string, string>((IDictionary<string, string>) dictionary);
      PropertyInfo property = compilerInfo.GetType().GetProperty("ProviderOptions", BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      return property == (PropertyInfo) null ? (IDictionary<string, string>) new ReadOnlyDictionary<string, string>((IDictionary<string, string>) dictionary) : (IDictionary<string, string>) new ReadOnlyDictionary<string, string>((IDictionary<string, string>) property.GetValue((object) compilerInfo, (object[]) null));
    }

    internal static void PrependCompilerOption(
      CompilerParameters compilParams,
      string compilerOptions)
    {
      if (compilParams.CompilerOptions == null)
        compilParams.CompilerOptions = compilerOptions;
      else
        compilParams.CompilerOptions = compilerOptions + " " + compilParams.CompilerOptions;
    }

    internal static string CompilerDefaultPath()
    {
      string path2_1 = "bin\\roslyn";
      string path2_2 = "roslyn";
      string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path2_1);
      if (!Directory.Exists(path))
        path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path2_2);
      return path;
    }

    internal static bool IsDebuggerAttached => CompilationUtil.IsDebuggerPresent() || Debugger.IsAttached;

    [DllImport("kernel32.dll")]
    private static extern bool IsDebuggerPresent();
  }
}
