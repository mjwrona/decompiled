// Decompiled with JetBrains decompiler
// Type: DACWebServiceClient
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.SqlServer.Management.Dac.ServiceTypes;
using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

[DebuggerStepThrough]
[GeneratedCode("System.ServiceModel", "4.0.0.0")]
public class DACWebServiceClient : ClientBase<IDACWebService>, IDACWebService
{
  public DACWebServiceClient()
  {
  }

  public DACWebServiceClient(string endpointConfigurationName)
    : base(endpointConfigurationName)
  {
  }

  public DACWebServiceClient(string endpointConfigurationName, string remoteAddress)
    : base(endpointConfigurationName, remoteAddress)
  {
  }

  public DACWebServiceClient(string endpointConfigurationName, EndpointAddress remoteAddress)
    : base(endpointConfigurationName, remoteAddress)
  {
  }

  public DACWebServiceClient(Binding binding, EndpointAddress remoteAddress)
    : base(binding, remoteAddress)
  {
  }

  public Guid Export(ExportInput exportInput) => this.Channel.Export(exportInput);

  public Task<Guid> ExportAsync(ExportInput exportInput) => this.Channel.ExportAsync(exportInput);

  public Guid SelectiveExport(SelectiveExportInput exportInput) => this.Channel.SelectiveExport(exportInput);

  public Task<Guid> SelectiveExportAsync(SelectiveExportInput exportInput) => this.Channel.SelectiveExportAsync(exportInput);

  public Guid Import(ImportInput importInput) => this.Channel.Import(importInput);

  public Task<Guid> ImportAsync(ImportInput importInput) => this.Channel.ImportAsync(importInput);

  public StatusInfo[] PostStatus(StatusInput statusInput) => this.Channel.PostStatus(statusInput);

  public Task<StatusInfo[]> PostStatusAsync(StatusInput statusInput) => this.Channel.PostStatusAsync(statusInput);

  public StatusInfo[] GetStatus(
    string serverName,
    string userName,
    string password,
    string requestId)
  {
    return this.Channel.GetStatus(serverName, userName, password, requestId);
  }

  public Task<StatusInfo[]> GetStatusAsync(
    string serverName,
    string userName,
    string password,
    string requestId)
  {
    return this.Channel.GetStatusAsync(serverName, userName, password, requestId);
  }

  public int Test() => this.Channel.Test();

  public Task<int> TestAsync() => this.Channel.TestAsync();
}
