// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.ReIndexCppFilesPatchProvider
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Crawler.CrawlSpecs;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Memory;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Utils;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Tokenizers.SourceCode;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  public class ReIndexCppFilesPatchProvider : ReIndexPatchProvider, IIndexPatchProvider
  {
    private const string LastFetchedTimestampConfigKey = "/Service/ALMSearch/Settings/Code/{0}/LastFetchedTimestamp";
    private const int BatchSize = 1000;
    private readonly long m_endTimestamp;
    private readonly long m_startTimestamp;
    private readonly long m_batchTimePeriod;
    private long m_currentStartTimestamp;
    private long m_currentEndTimestamp;
    private readonly DocumentContractType m_documentContractType;

    public ReIndexCppFilesPatchProvider(
      IndexingExecutionContext indexingExecutionContext,
      CodeFileContract codeFileContract)
      : this(indexingExecutionContext, codeFileContract, indexingExecutionContext.RequestContext.GetConfigValue<long>("/Service/ALMSearch/Settings/Code/VcRuntimeMissingIssueStartTimestamp"), indexingExecutionContext.RequestContext.GetConfigValue<long>("/Service/ALMSearch/Settings/Code/VcRuntimeMissingIssueEndTimestamp"), indexingExecutionContext.RequestContext.GetConfigValue<long>("/Service/ALMSearch/Settings/Code/BatchTimePeriod"), 1000)
    {
    }

    public ReIndexCppFilesPatchProvider(
      IndexingExecutionContext indexingExecutionContext,
      CodeFileContract codeFileContract,
      long startTimestamp,
      long endTimestamp,
      long batchTimePeriod,
      int batchSize)
      : base(codeFileContract, batchSize)
    {
      this.m_startTimestamp = startTimestamp;
      this.m_endTimestamp = endTimestamp;
      this.m_batchTimePeriod = batchTimePeriod;
      this.m_documentContractType = indexingExecutionContext.ProvisioningContext.ContractType;
      if (this.m_startTimestamp <= 0L || this.m_endTimestamp <= 0L || this.m_batchTimePeriod <= 0L || this.m_endTimestamp <= this.m_startTimestamp)
        throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("Invalid values of time periods, StartTime: {0}, EndTime: {1}, BatchTimePeriod: {2}, can't proceed.", (object) this.m_startTimestamp, (object) this.m_endTimestamp, (object) this.m_batchTimePeriod)));
    }

    [SuppressMessage("Microsoft.Security", "CA2119:SealMethodsThatSatisfyPrivateInterfaces", Justification = "Keepint it virtual for testing")]
    public virtual IEnumerable<PatchDescription> GetPatches(
      IndexingExecutionContext iexContext,
      string branchName,
      CodeCrawlSpec codeCrawlSpec,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit)
    {
      ReIndexCppFilesPatchProvider filesPatchProvider = this;
      string configKey = string.Format("/Service/ALMSearch/Settings/Code/{0}/LastFetchedTimestamp", (object) iexContext.IndexingUnit.GetTfsEntityIdAsNormalizedString());
      filesPatchProvider.m_currentStartTimestamp = iexContext.RequestContext.GetCurrentHostConfigValue<long>(configKey);
      if (filesPatchProvider.m_currentStartTimestamp <= 0L)
        filesPatchProvider.m_currentStartTimestamp = filesPatchProvider.m_startTimestamp;
      int totalDocumentsFetched = 0;
      int totalAffectedDocuments = 0;
label_4:
      filesPatchProvider.m_currentEndTimestamp = filesPatchProvider.m_currentStartTimestamp + filesPatchProvider.m_batchTimePeriod > filesPatchProvider.m_endTimestamp ? filesPatchProvider.m_endTimestamp : filesPatchProvider.m_currentStartTimestamp + filesPatchProvider.m_batchTimePeriod;
      Tracer.TraceInfo(1080622, "Indexing Pipeline", "IndexingOperation", FormattableString.Invariant(FormattableStringFactory.Create("Fetching documents for TimePeriod: {0} - {1}", (object) filesPatchProvider.m_currentStartTimestamp, (object) filesPatchProvider.m_currentEndTimestamp)));
      foreach (CodeFileContract document in filesPatchProvider.GetDocuments(iexContext, indexingUnit))
      {
        ++totalDocumentsFetched;
        if (filesPatchProvider.ShouldReIndex((CoreIndexingExecutionContext) iexContext, document))
        {
          ++totalAffectedDocuments;
          PatchDescription patch = new PatchDescription(document.FilePathOriginal);
          Tracer.TraceWarning(1080622, "Indexing Pipeline", "IndexingOperation", FormattableString.Invariant(FormattableStringFactory.Create("Found {0}, Branches ({1}) with only text tokens.", (object) document.FilePathOriginal, (object) filesPatchProvider.GetBranchNames(document))));
          yield return patch;
        }
      }
      iexContext.RequestContext.SetCurrentHostConfigValue<long>(configKey, filesPatchProvider.m_currentStartTimestamp);
      filesPatchProvider.m_currentStartTimestamp = filesPatchProvider.m_currentEndTimestamp;
      if (filesPatchProvider.m_currentEndTimestamp >= filesPatchProvider.m_endTimestamp)
        iexContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Successfully retrieved {0} records from Elasticsearch returned {1} documents to patch.", (object) totalDocumentsFetched, (object) totalAffectedDocuments)));
      else
        goto label_4;
    }

    public void PostPatchOperation(
      IndexingExecutionContext iexContext,
      string branchName,
      IEnumerable<PatchDescription> patchDescriptions)
    {
      string key = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/ALMSearch/Settings/Code/{0}/LastFetchedTimestamp", (object) iexContext.IndexingUnit.GetTfsEntityIdAsNormalizedString());
      iexContext.RequestContext.DeleteCurrentHostConfigValue(key);
    }

    public override IExpression GetQueryExpression(IndexingExecutionContext indexingExecutionContext)
    {
      TermExpression termExpression1 = new TermExpression(CodeContractField.CodeSearchFieldDesc.IndexedTimeStamp.ElasticsearchFieldName(), Operator.GreaterThanOrEqual, this.m_currentStartTimestamp.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      TermExpression termExpression2 = new TermExpression(CodeContractField.CodeSearchFieldDesc.IndexedTimeStamp.ElasticsearchFieldName(), Operator.LessThan, this.m_currentEndTimestamp.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      TermExpression termExpression3 = new TermExpression("repositoryId", Operator.Equals, indexingExecutionContext.RepositoryIndexingUnit.GetTfsEntityIdAsNormalizedString());
      HashSet<string> stringSet = new HashSet<string>();
      stringSet.AddRange<string, HashSet<string>>((IEnumerable<string>) ProgrammingLanguages.GetProgrammingLanguageExtensions(ProgrammingLanguages.ProgrammingLanguage.C));
      stringSet.AddRange<string, HashSet<string>>((IEnumerable<string>) ProgrammingLanguages.GetProgrammingLanguageExtensions(ProgrammingLanguages.ProgrammingLanguage.Cpp));
      stringSet.AddRange<string, HashSet<string>>((IEnumerable<string>) ProgrammingLanguages.GetProgrammingLanguageExtensions(ProgrammingLanguages.ProgrammingLanguage.CWeb));
      stringSet.AddRange<string, HashSet<string>>((IEnumerable<string>) ProgrammingLanguages.GetProgrammingLanguageExtensions(ProgrammingLanguages.ProgrammingLanguage.Lex));
      List<string> list = stringSet.Select<string, string>((Func<string, string>) (x => x.Replace(".", string.Empty))).ToList<string>();
      TermsExpression termsExpression = new TermsExpression(CodeContractField.CodeSearchFieldDesc.FileExtension.ElasticsearchFieldName(), Operator.In, (IEnumerable<string>) list);
      IExpression scopePathExpression = this.GetScopePathExpression(indexingExecutionContext);
      IExpression branchExpression = this.GetBranchExpression(indexingExecutionContext);
      return (IExpression) new AndExpression(new IExpression[6]
      {
        (IExpression) termExpression1,
        (IExpression) termExpression2,
        (IExpression) termExpression3,
        (IExpression) termsExpression,
        scopePathExpression,
        branchExpression
      });
    }

    public override List<string> GetFieldsToQuery(CodeFileContract codeFileContract) => new List<string>()
    {
      codeFileContract.GetSearchStoredFieldForType(CodeFileContract.CodeContractQueryableElement.FilePath).ElasticsearchFieldName,
      CodeFileContract.CodeContractQueryableElement.Content.InlineFilterName(),
      AbstractSearchDocumentContract.GetBranchNameOriginalFieldName(this.m_documentContractType)
    };

    internal virtual IExpression GetScopePathExpression(
      IndexingExecutionContext indexingExecutionContext)
    {
      string scopedIuElseNull = indexingExecutionContext.IndexingUnit.GetScopePathFromTFSAttributesIfScopedIUElseNull();
      if (string.IsNullOrWhiteSpace(scopedIuElseNull))
        return (IExpression) new EmptyExpression();
      string str = this.CodeFileContract.CorrectFilePath(scopedIuElseNull);
      return string.IsNullOrWhiteSpace(str) || !(str != CommonConstants.DirectorySeparatorString) ? (IExpression) new EmptyExpression() : (IExpression) new TermExpression(CodeContractField.CodeSearchFieldDesc.FilePath.ElasticsearchFieldName(), Operator.Equals, this.CodeFileContract.CorrectFilePath(scopedIuElseNull));
    }

    internal virtual IExpression GetBranchExpression(
      IndexingExecutionContext indexingExecutionContext)
    {
      if (!(indexingExecutionContext.RepositoryIndexingUnit.IndexingUnitType == "Git_Repository"))
        return (IExpression) new EmptyExpression();
      List<string> branchesToIndex = ((GitCodeRepoTFSAttributes) indexingExecutionContext.RepositoryIndexingUnit.TFSEntityAttributes).BranchesToIndex;
      return (IExpression) new TermsExpression(AbstractSearchDocumentContract.GetBranchNameOriginalFieldName(this.m_documentContractType), Operator.In, branchesToIndex.Select<string, string>((Func<string, string>) (s => CustomUtils.GetBranchNameWithoutPrefix("refs/heads/", s))));
    }

    internal string GetBranchNames(CodeFileContract codeFileContract)
    {
      switch (codeFileContract)
      {
        case DedupeFileContractBase fileContractBase1:
          FileGroup paths = fileContractBase1.Paths;
          return string.Join(",", paths.BranchNameOriginal.Count > 5 ? (IEnumerable<string>) paths.BranchNameOriginal.Take<string>(5).ToList<string>() : (IEnumerable<string>) paths.BranchNameOriginal);
        case SourceNoDedupeFileContractBase fileContractBase2:
          return fileContractBase2.BranchNameOriginal;
        default:
          return string.Empty;
      }
    }

    internal virtual bool ShouldReIndex(
      CoreIndexingExecutionContext indexingExecutionContext,
      CodeFileContract documentContract)
    {
      try
      {
        string content = documentContract.Content;
        byte[] array = new byte[content.Length];
        for (int index = 0; index < content.Length; ++index)
          array[index] = (byte) content[index];
        bool flag = true;
        using (ByteArray byteArray = new ByteArray(array))
        {
          CodeSymbol codeSymbol = new CodeSymbol();
          int num1 = byteArray.ReadInt32();
          if (num1 == 2)
          {
            byteArray.ReadVInt64();
            num1 = byteArray.ReadInt32();
          }
          DocumentType documentType = (DocumentType) num1;
          while (codeSymbol.CharacterOffset < uint.MaxValue)
          {
            codeSymbol.CharacterOffset = byteArray.ReadVUInt32();
            if (codeSymbol.CharacterOffset < uint.MaxValue)
            {
              int num2 = (int) byteArray.ReadVUInt32();
              int length = (int) byteArray.ReadVUInt32();
              byte[] numArray = new byte[length];
              int num3 = (int) byteArray.ReadBytes(numArray, 0U, (uint) length);
              char[] destinationArray = new char[numArray.Length];
              Array.Copy((Array) numArray, (Array) destinationArray, length);
              switch (documentType)
              {
                case DocumentType.Text:
                  codeSymbol.SymbolType = CodeTokenKind.Text;
                  break;
                case DocumentType.Cpp:
                  codeSymbol.SymbolType = (CodeTokenKind) byteArray.ReadVUInt32();
                  if (codeSymbol.SymbolType == CodeTokenKind.ExtendedSymbol)
                  {
                    codeSymbol.SymbolLengthExtended = byteArray.ReadVUInt32();
                    codeSymbol.SymbolType = (CodeTokenKind) byteArray.ReadVUInt32();
                  }
                  if (CodeSymbol.HasScope(codeSymbol.SymbolType))
                  {
                    byteArray.ReadVInt32();
                    byteArray.ReadVInt32();
                    break;
                  }
                  break;
                case DocumentType.TextDocumentWithFields:
                  codeSymbol.SymbolType = CodeTokenKind.Text;
                  codeSymbol.FieldIndex = byteArray.ReadVUInt32();
                  break;
              }
              if (codeSymbol.SymbolType != CodeTokenKind.Text && codeSymbol.SymbolType != CodeTokenKind.Unknown)
              {
                flag = false;
                break;
              }
            }
          }
        }
        return flag;
      }
      catch (Exception ex)
      {
        if (documentContract == null)
          Tracer.TraceError(1080622, "Indexing Pipeline", "IndexingOperation", FormattableString.Invariant(FormattableStringFactory.Create("Null Document retrieved from Elasticsearch, can't index this file.")));
        else
          Tracer.TraceError(1080622, "Indexing Pipeline", "IndexingOperation", FormattableString.Invariant(FormattableStringFactory.Create("Exception {0} occurred while analyzing {1}", (object) ex, (object) documentContract.FilePathOriginal)));
        return true;
      }
    }
  }
}
