// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.IndexingStalenessAnalyzer.CodeIndexingStalenessAnalyzerFactory
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers;
using Microsoft.VisualStudio.Services.Search.Crawler;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.IndexingStalenessAnalyzer
{
  public class CodeIndexingStalenessAnalyzerFactory : ICodeIndexingStalenessAnalyzerFactory
  {
    private CodeIndexingStalenessAnalyzerFactory()
    {
    }

    public CodeIndexingStalenessAnalyzer GetCodeIndexingStalenessAnalyzer(
      ExecutionContext executionContext,
      TraceMetaData traceMetaData,
      IDataAccessFactory dataAccessFactory,
      string indexingUnitType)
    {
      switch (indexingUnitType)
      {
        case "Git_Repository":
          return (CodeIndexingStalenessAnalyzer) new GitCodeIndexingStalenessAnalyzer(dataAccessFactory.GetIndexingUnitDataAccess(), new GitRepositoryStalenessAnalyzer(new GitHttpClientWrapper(executionContext, traceMetaData)), traceMetaData);
        case "TFVC_Repository":
          return (CodeIndexingStalenessAnalyzer) new TfvcCodeIndexingStalenessAnalyzer(dataAccessFactory.GetIndexingUnitDataAccess(), new TfvcRepositoryStalenessAnalyzer(new TfvcHttpClientWrapper(executionContext, traceMetaData)), traceMetaData);
        default:
          throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("IndexingUnitType {0} is not supported", (object) indexingUnitType)));
      }
    }

    public static CodeIndexingStalenessAnalyzerFactory GetInstance()
    {
      if (CodeIndexingStalenessAnalyzerFactory.codeIndexingStalenessAnalyzerFactory == null)
        CodeIndexingStalenessAnalyzerFactory.codeIndexingStalenessAnalyzerFactory = new CodeIndexingStalenessAnalyzerFactory();
      return CodeIndexingStalenessAnalyzerFactory.codeIndexingStalenessAnalyzerFactory;
    }

    [StaticSafe]
    public static CodeIndexingStalenessAnalyzerFactory codeIndexingStalenessAnalyzerFactory { get; private set; }
  }
}
