// Decompiled with JetBrains decompiler
// Type: IDACWebService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.SqlServer.Management.Dac.ServiceTypes;
using System;
using System.CodeDom.Compiler;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading.Tasks;

[GeneratedCode("System.ServiceModel", "4.0.0.0")]
[ServiceContract(ConfigurationName = "IDACWebService")]
public interface IDACWebService
{
  [OperationContract(Action = "http://tempuri.org/IDACWebService/Export", ReplyAction = "http://tempuri.org/IDACWebService/ExportResponse")]
  Guid Export(ExportInput exportInput);

  [OperationContract(Action = "http://tempuri.org/IDACWebService/Export", ReplyAction = "http://tempuri.org/IDACWebService/ExportResponse")]
  Task<Guid> ExportAsync(ExportInput exportInput);

  [OperationContract(Action = "http://tempuri.org/IDACWebService/SelectiveExport", ReplyAction = "http://tempuri.org/IDACWebService/SelectiveExportResponse")]
  Guid SelectiveExport(SelectiveExportInput exportInput);

  [OperationContract(Action = "http://tempuri.org/IDACWebService/SelectiveExport", ReplyAction = "http://tempuri.org/IDACWebService/SelectiveExportResponse")]
  Task<Guid> SelectiveExportAsync(SelectiveExportInput exportInput);

  [OperationContract(Action = "http://tempuri.org/IDACWebService/Import", ReplyAction = "http://tempuri.org/IDACWebService/ImportResponse")]
  Guid Import(ImportInput importInput);

  [OperationContract(Action = "http://tempuri.org/IDACWebService/Import", ReplyAction = "http://tempuri.org/IDACWebService/ImportResponse")]
  Task<Guid> ImportAsync(ImportInput importInput);

  [OperationContract(Action = "http://tempuri.org/IDACWebService/PostStatus", ReplyAction = "http://tempuri.org/IDACWebService/PostStatusResponse")]
  StatusInfo[] PostStatus(StatusInput statusInput);

  [OperationContract(Action = "http://tempuri.org/IDACWebService/PostStatus", ReplyAction = "http://tempuri.org/IDACWebService/PostStatusResponse")]
  Task<StatusInfo[]> PostStatusAsync(StatusInput statusInput);

  [OperationContract(Action = "http://tempuri.org/IDACWebService/GetStatus", ReplyAction = "http://tempuri.org/IDACWebService/GetStatusResponse")]
  [WebGet(UriTemplate = "Status?serverName={serverName}&userName={userName}&password={password}&reqId={requestId}")]
  StatusInfo[] GetStatus(string serverName, string userName, string password, string requestId);

  [OperationContract(Action = "http://tempuri.org/IDACWebService/GetStatus", ReplyAction = "http://tempuri.org/IDACWebService/GetStatusResponse")]
  Task<StatusInfo[]> GetStatusAsync(
    string serverName,
    string userName,
    string password,
    string requestId);

  [OperationContract(Action = "http://tempuri.org/IDACWebService/Test", ReplyAction = "http://tempuri.org/IDACWebService/TestResponse")]
  int Test();

  [OperationContract(Action = "http://tempuri.org/IDACWebService/Test", ReplyAction = "http://tempuri.org/IDACWebService/TestResponse")]
  Task<int> TestAsync();
}
