// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.WebApi.TCMServiceDataMigrationHttpClientBase
// Assembly: Microsoft.VisualStudio.Services.Tcm.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DCD48481-6B90-4012-9254-BC9E7077DAC8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.WebApi.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Tcm.WebApi
{
  [ResourceArea("C2112469-ADF5-45F2-8AB5-4764540113B6")]
  public abstract class TCMServiceDataMigrationHttpClientBase : VssHttpClientBase
  {
    public TCMServiceDataMigrationHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public TCMServiceDataMigrationHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public TCMServiceDataMigrationHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public TCMServiceDataMigrationHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public TCMServiceDataMigrationHttpClientBase(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual async Task SyncBuildRefAsync(
      IEnumerable<BuildReference2> references,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("a8710559-b314-4c4a-ad18-94a0011e2ca2");
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<BuildReference2>>(references, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase2 = migrationHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await migrationHttpClientBase2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task SyncCodeCoverageSummaryAsync(
      IEnumerable<CodeCoverageSummary2> codeCoverageSummary2,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("fe29f2e2-4381-4198-ba74-2386e4a225a0");
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<CodeCoverageSummary2>>(codeCoverageSummary2, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase2 = migrationHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await migrationHttpClientBase2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task SyncCodeCoverageAsync(
      IEnumerable<Coverage2> coverage2,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("3a8a85c8-74f6-4d60-9ead-93a0a5c53a9e");
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<Coverage2>>(coverage2, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase2 = migrationHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await migrationHttpClientBase2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task SyncFunctionCoverageAsync(
      IEnumerable<FunctionCoverage2> functionCoverages,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("dff9e465-770b-47d9-888f-c3f1e9921531");
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<FunctionCoverage2>>(functionCoverages, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase2 = migrationHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await migrationHttpClientBase2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task SyncMigrationStatusAsync(
      TCMServiceDataMigrationStatus migrationStatus,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase = this;
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("58935008-f4a4-4f3b-b2b8-1122f20321e1");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (migrationStatus), migrationStatus.ToString());
      using (await migrationHttpClientBase.SendAsync(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task SyncModuleCoverageAsync(
      IEnumerable<ModuleCoverage2> moduleCoverages,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("8c4130de-1aea-4e42-8261-cd362423d712");
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<ModuleCoverage2>>(moduleCoverages, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase2 = migrationHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await migrationHttpClientBase2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual Task<List<PointsResults2>> FetchPointResultsAsync(
      IEnumerable<PointsReference2> pointReferences,
      int batchSize,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("58ac0a52-fb0f-404f-926f-5ed1a72c4c22");
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<PointsReference2>>(pointReferences, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (batchSize), batchSize.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<PointsResults2>>(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task SyncPointResultsAsync(
      IEnumerable<PointsResults2> pointResults,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("67da20ab-6e4c-4f24-a9a1-1b2deee46215");
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<PointsResults2>>(pointResults, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase2 = migrationHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await migrationHttpClientBase2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task SyncReleaseRefAsync(
      IEnumerable<ReleaseReference2> references,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("32b77c5b-d44d-4b2e-a30f-9f6414e71413");
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<ReleaseReference2>>(references, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase2 = migrationHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await migrationHttpClientBase2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task SyncRequirementsToTestsMappingAsync(
      IEnumerable<RequirementsToTestsMapping2> requirementsMapping,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("e048ea20-c08e-48cf-82e7-3b930f4cf2a0");
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<RequirementsToTestsMapping2>>(requirementsMapping, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase2 = migrationHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await migrationHttpClientBase2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task SyncTCMPropertyBagAsync(
      IEnumerable<TCMPropertyBag2> tcmPropertyBag,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("5c7fee2b-3d87-4d29-a0d3-63025e9e4aca");
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<TCMPropertyBag2>>(tcmPropertyBag, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase2 = migrationHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await migrationHttpClientBase2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task SyncTestActionResultAsync(
      IEnumerable<TestActionResult2> testActionResults,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("012ae012-8cbf-42b7-aaa3-c47804f10afe");
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<TestActionResult2>>(testActionResults, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase2 = migrationHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await migrationHttpClientBase2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task SyncTestCaseMetadataAsync(
      IEnumerable<TestCaseMetadata2> testCaseMetadata,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("98b880e3-97a4-43ae-8039-ea2943505d23");
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<TestCaseMetadata2>>(testCaseMetadata, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase2 = migrationHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await migrationHttpClientBase2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task SyncTestCaseReferenceAsync(
      IEnumerable<TestCaseReference2> testCaseReferences,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("e4cfbe7e-dce3-4d05-a60d-cede65e40399");
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<TestCaseReference2>>(testCaseReferences, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase2 = migrationHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await migrationHttpClientBase2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task SyncTestMessageLogAsync(
      IEnumerable<TestMessageLog2> testMessageLogs,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("eae679f6-799e-4eb8-afe6-b5121aefdb2e");
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<TestMessageLog2>>(testMessageLogs, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase2 = migrationHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await migrationHttpClientBase2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task SyncTestMessageLogEntryAsync(
      IEnumerable<TestMessageLogEntry2> testMessageLogEntries,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("22ce9ecd-9906-4646-9de8-2b805fc5ef4e");
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<TestMessageLogEntry2>>(testMessageLogEntries, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase2 = migrationHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await migrationHttpClientBase2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task SyncTestParameterAsync(
      IEnumerable<TestParameter2> testParameter,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("0f386a08-3a70-42fd-8d54-b4af3cc0ffb7");
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<TestParameter2>>(testParameter, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase2 = migrationHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await migrationHttpClientBase2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task SyncTestResultAsync(
      IEnumerable<TestResult2> testResults,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("aeb42fc5-2e0b-4229-9be9-c8f84894a0f1");
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<TestResult2>>(testResults, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase2 = migrationHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await migrationHttpClientBase2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task SyncTestResultResetAsync(
      IEnumerable<TestResultReset2> testResultResets,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("0420e6b5-4013-4bb0-8dcc-f4d1cee35911");
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<TestResultReset2>>(testResultResets, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase2 = migrationHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await migrationHttpClientBase2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task SyncTestResultsExAsync(
      IEnumerable<TestResultsEx2> testResultsEx,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("e652fd1b-97c9-419f-9758-fa35a803922e");
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<TestResultsEx2>>(testResultsEx, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase2 = migrationHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await migrationHttpClientBase2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task SyncTestRunAsync(
      IEnumerable<TestRun2> testRuns,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("27df61f5-6a07-4d72-b81c-8f16866d324f");
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<TestRun2>>(testRuns, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase2 = migrationHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await migrationHttpClientBase2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task SyncTestRunContextAsync(
      IEnumerable<TestRunContext2> testRunContexts,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("0de60aad-37d2-4ffe-9518-44719053838b");
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<TestRunContext2>>(testRunContexts, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase2 = migrationHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await migrationHttpClientBase2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task SyncTestRunExAsync(
      IEnumerable<TestRunEx2> testRunEx,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("eed0b1cb-1274-453d-ac25-14d47dfc90ee");
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<TestRunEx2>>(testRunEx, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase2 = migrationHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await migrationHttpClientBase2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task SyncTestRunExtendedAsync(
      IEnumerable<TestRunExtended2> testRunExtended,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("acd864db-db98-449f-ac5b-fa5688ae4125");
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<TestRunExtended2>>(testRunExtended, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase2 = migrationHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await migrationHttpClientBase2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task SyncTestRun2Async(
      IEnumerable<TestRun2> testRuns,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("d6878655-8644-49ff-87de-89ab72df076b");
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<TestRun2>>(testRuns, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase2 = migrationHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await migrationHttpClientBase2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task SyncTestRunSummaryAsync(
      IEnumerable<TestRunSummary2> testRunSummary,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("042c98db-8804-4035-9548-d32d70171fd5");
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<TestRunSummary2>>(testRunSummary, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      TCMServiceDataMigrationHttpClientBase migrationHttpClientBase2 = migrationHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await migrationHttpClientBase2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }
  }
}
