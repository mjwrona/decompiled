// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.EsrpConverter
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using MS.Ess.EsrpClient.Contracts.Common;
using MS.Ess.EsrpClient.Contracts.Sign.V1;
using MS.Ess.EsrpClient.Contracts.Sign.V1.Response;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class EsrpConverter : IEsrpConverter
  {
    private const string RepoSignOperationName = "VsMpRepoSign";
    private const string ErrorDetailSignKey = "error";

    public EsrpOperationStatus ConvertEsrpOperationStatus(StatusCode statusCode) => statusCode <= 5 ? (EsrpOperationStatus) statusCode : throw new ArgumentException(string.Format("Unexpected values of {0}: {1}", (object) "StatusCode", (object) statusCode), nameof (statusCode));

    public IReadOnlyList<EsrpStartOperationResponse> CreateEsrpSignResponse(
      SignBatchSubmissionResponse response)
    {
      if (response == null)
        throw new ArgumentNullException(nameof (response));
      if (response.SubmissionResponses == null)
        throw new ArgumentException("SubmissionResponses property is null", nameof (response));
      List<EsrpStartOperationResponse> esrpSignResponse = new List<EsrpStartOperationResponse>();
      foreach (SignSubmissionResponse submissionResponse in response.SubmissionResponses)
      {
        EsrpStartOperationResponse operationResponse = new EsrpStartOperationResponse(this.ConvertEsrpOperationStatus(((ResponseBase) submissionResponse).StatusCode), ((ResponseBase) submissionResponse).OperationId.ToString(), ((ResponseBase) submissionResponse).ErrorInfo?.Code, EsrpConverter.GetSignErrorDetail(((ResponseBase) submissionResponse).ErrorInfo));
        esrpSignResponse.Add(operationResponse);
      }
      return (IReadOnlyList<EsrpStartOperationResponse>) esrpSignResponse;
    }

    public SignStatusRequest CreateGetSignStatusRequest(
      IReadOnlyCollection<EsrpGetOperationStatusRequest> requests)
    {
      if (requests == null)
        throw new ArgumentNullException(nameof (requests));
      if (!requests.Any<EsrpGetOperationStatusRequest>())
        throw new ArgumentException("requests argument must not be empty", nameof (requests));
      List<Guid> guidList = EsrpConverter.ConvertOperationsIdstoGuids(requests.Select<EsrpGetOperationStatusRequest, string>((Func<EsrpGetOperationStatusRequest, string>) (r => r.OperationId)));
      return new SignStatusRequest()
      {
        OperationIds = guidList
      };
    }

    public IReadOnlyList<EsrpGetOperationStatusResponse> CreateGetSignStatusResponse(
      SignBatchCompletionResponse response)
    {
      if (response == null)
        throw new ArgumentNullException(nameof (response));
      if (response.SubmissionResponses == null)
        throw new ArgumentException("SubmissionResponses property is null", nameof (response));
      List<EsrpGetOperationStatusResponse> signStatusResponse = new List<EsrpGetOperationStatusResponse>();
      foreach (SignStatusResponse submissionResponse in response.SubmissionResponses)
      {
        EsrpGetOperationStatusResponse operationStatusResponse = new EsrpGetOperationStatusResponse(((ResponseBase) submissionResponse).OperationId.ToString(), this.ConvertEsrpOperationStatus(((ResponseBase) submissionResponse).StatusCode), ((ResponseBase) submissionResponse).ErrorInfo?.Code, EsrpConverter.GetSignErrorDetail(((ResponseBase) submissionResponse).ErrorInfo), EsrpConverter.GetFileDetail(submissionResponse.FileStatusDetail));
        signStatusResponse.Add(operationStatusResponse);
      }
      return (IReadOnlyList<EsrpGetOperationStatusResponse>) signStatusResponse;
    }

    public StaticSigningData CreateSignRequest(IReadOnlyCollection<EsrpSignRequest> requests)
    {
      if (requests == null)
        throw new ArgumentNullException(nameof (requests));
      if (!requests.Any<EsrpSignRequest>())
        throw new ArgumentException("requests argument must not be empty", nameof (requests));
      StaticSigningData signRequest = new StaticSigningData();
      ((SubmissionRequest) signRequest).ContextData = new Dictionary<string, string>();
      ((SubmissionRequest) signRequest).GroupId = (string) null;
      ((SubmissionRequest) signRequest).CorrelationVector = (string) null;
      signRequest.SignBatches = requests.Select<EsrpSignRequest, StaticSigningFileBatch>(new Func<EsrpSignRequest, StaticSigningFileBatch>(this.CreateEsrpSigningBatch)).ToArray<StaticSigningFileBatch>();
      return signRequest;
    }

    private static string GetFileDetail(FileStatusDetail[] fileStatusDetails) => fileStatusDetails != null && fileStatusDetails.Length != 0 ? fileStatusDetails[0].DestinationLocation : (string) null;

    private StaticSigningFileBatch CreateEsrpSigningBatch(EsrpSignRequest request)
    {
      StaticSigningFileBatch esrpSigningBatch = new StaticSigningFileBatch();
      ((BaseBatches) esrpSigningBatch).SourceLocationType = (FileLocationType) 1;
      ((BaseBatches) esrpSigningBatch).SourceRootDirectory = (string) null;
      esrpSigningBatch.DestinationLocationType = (FileLocationType) 1;
      esrpSigningBatch.DestinationRootDirectory = (string) null;
      esrpSigningBatch.SignRequestFiles = new FileEntry[1]
      {
        EsrpConverter.CreateEsrpFileEntry(request)
      };
      StaticSigningFileBatch signingFileBatch = esrpSigningBatch;
      StaticSigningInfo staticSigningInfo1 = new StaticSigningInfo();
      StaticSigningInfo staticSigningInfo2 = staticSigningInfo1;
      StaticSigningOperation[] signingOperationArray = new StaticSigningOperation[1];
      StaticSigningOperation signingOperation = new StaticSigningOperation();
      ((Operation) signingOperation).KeyCode = request.KeyCode;
      ((Operation) signingOperation).OperationCode = "VsMpRepoSign";
      ((Operation) signingOperation).Parameters = new Dictionary<string, string>();
      signingOperationArray[0] = signingOperation;
      staticSigningInfo2.Operations = signingOperationArray;
      StaticSigningInfo staticSigningInfo3 = staticSigningInfo1;
      signingFileBatch.SigningInfo = staticSigningInfo3;
      return esrpSigningBatch;
    }

    private static FileEntry CreateEsrpFileEntry(EsrpSignRequest request)
    {
      FileEntry esrpFileEntry = new FileEntry();
      ((FileInfo) esrpFileEntry).CustomerCorrelationId = request.ValidationId.ToString();
      ((FileInfo) esrpFileEntry).SourceLocation = request.SourceFileDescription.Path;
      ((FileInfo) esrpFileEntry).SizeInBytes = request.SourceFileDescription.Size;
      ((FileInfo) esrpFileEntry).HashType = (HashType) 0;
      ((FileInfo) esrpFileEntry).SourceHash = request.SourceFileDescription.Hash;
      esrpFileEntry.DestinationLocation = request.DestinationPath;
      return esrpFileEntry;
    }

    private static string GetSignErrorDetail(InnerServiceError errorInfo) => EsrpConverter.GetErrorDetail("error", errorInfo);

    private static string GetErrorDetail(string errorDetailKey, InnerServiceError errorInfo)
    {
      string str;
      return errorInfo?.Details != null && errorInfo.Details.TryGetValue(errorDetailKey, out str) ? str : (string) null;
    }

    private static List<Guid> ConvertOperationsIdstoGuids(IEnumerable<string> inputOperationIds)
    {
      List<Guid> guidList = new List<Guid>();
      foreach (string inputOperationId in inputOperationIds)
      {
        Guid result;
        if (!Guid.TryParse(inputOperationId, out result))
          throw new ArgumentException("\"" + inputOperationId + "\" is not a proper GUID string", nameof (inputOperationIds));
        guidList.Add(result);
      }
      return guidList;
    }
  }
}
