// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.AzureFunctionAppDetector
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  internal class AzureFunctionAppDetector
  {
    private const string c_hostJsonFile = "host.json";
    private const string c_functionJsonFile = "function.json";

    public IEnumerable<AzureFunctionAppDetector.FunctionAppInfo> GetAzureFunctionApps(
      IVssRequestContext requestContext,
      TreeAnalysis treeAnalysis)
    {
      IReadOnlyList<FilePath> filePaths1;
      if (!treeAnalysis.TryGetFilePaths("host.json", out filePaths1))
        return Enumerable.Empty<AzureFunctionAppDetector.FunctionAppInfo>();
      IReadOnlyList<FilePath> filePaths2;
      if (treeAnalysis.TryGetFilePaths("function.json", out filePaths2))
        return filePaths1.GroupJoin<FilePath, FilePath, FilePath, AzureFunctionAppDetector.FunctionAppInfo>((IEnumerable<FilePath>) filePaths2, (Func<FilePath, FilePath>) (hostPath => hostPath.Folder), (Func<FilePath, FilePath>) (functionPath => functionPath.Folder?.Folder), (Func<FilePath, IEnumerable<FilePath>, AzureFunctionAppDetector.FunctionAppInfo>) ((hostPath, functionPaths) => new AzureFunctionAppDetector.FunctionAppInfo(hostPath, (IReadOnlyList<FilePath>) functionPaths.ToList<FilePath>()))).Where<AzureFunctionAppDetector.FunctionAppInfo>((Func<AzureFunctionAppDetector.FunctionAppInfo, bool>) (info => info.Functions.Any<FilePath>()));
      IEnumerable<FilePath> functionCsFilePaths = this.GetFunctionCsFilePaths(requestContext, treeAnalysis.GetMatchingFilesByExtension(".cs"), filePaths1);
      return filePaths1.GroupJoin<FilePath, FilePath, FilePath, AzureFunctionAppDetector.FunctionAppInfo>(functionCsFilePaths, (Func<FilePath, FilePath>) (hostPath => hostPath.Folder), (Func<FilePath, FilePath>) (functionPath => functionPath.Folder), (Func<FilePath, IEnumerable<FilePath>, AzureFunctionAppDetector.FunctionAppInfo>) ((hostPath, functionPaths) => new AzureFunctionAppDetector.FunctionAppInfo(hostPath, (IReadOnlyList<FilePath>) functionPaths.ToList<FilePath>())), (IEqualityComparer<FilePath>) new AzureFunctionAppDetector.HostFunctionPathComparer()).Where<AzureFunctionAppDetector.FunctionAppInfo>((Func<AzureFunctionAppDetector.FunctionAppInfo, bool>) (info => info.Functions.Any<FilePath>()));
    }

    public IEnumerable<FilePath> GetFunctionCsFilePaths(
      IVssRequestContext requestContext,
      IEnumerable<FilePath> files,
      IReadOnlyList<FilePath> hostFilePaths)
    {
      foreach (FilePath file1 in files)
      {
        FilePath file = file1;
        if (hostFilePaths.Select<FilePath, bool>((Func<FilePath, bool>) (path => file.ToString().Contains(path.ToString()))).Any<bool>())
          yield return file;
      }
    }

    public class FunctionAppInfo
    {
      public FilePath Host { get; }

      public IReadOnlyList<FilePath> Functions { get; }

      public FunctionAppInfo(FilePath host, IReadOnlyList<FilePath> functions)
      {
        this.Host = host;
        this.Functions = functions;
      }
    }

    private class HostFunctionPathComparer : IEqualityComparer<FilePath>
    {
      bool IEqualityComparer<FilePath>.Equals(FilePath x, FilePath y) => x.ToString().Contains(y.ToString()) || y.ToString().Contains(x.ToString());

      int IEqualityComparer<FilePath>.GetHashCode(FilePath obj) => 0;
    }
  }
}
