// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.DataImport.DataImportStepStatusScope
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.DataImport;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Cloud.DataImport
{
  public class DataImportStepStatusScope : IDataImportStepStatusScope, IDisposable
  {
    private readonly IServicingContext m_servicingContext;
    private static readonly string s_publisherTopicName = "Microsoft.VisualStudio.Services.DataImport.Status";

    public DataImportStepStatusScope(IServicingContext servicingContext)
    {
      this.m_servicingContext = servicingContext;
      this.PublishInfoMessage(this.m_servicingContext.DeploymentRequestContext, "Start Step");
    }

    private void PublishMessage(
      IVssRequestContext requestContext,
      string infoMessage = "",
      string errorMessage = "")
    {
      try
      {
        Guid dataImportId = this.m_servicingContext.GetDataImportId();
        if (dataImportId == Guid.Empty)
        {
          requestContext.TraceAlways(43500, TraceLevel.Warning, nameof (DataImportStepStatusScope), nameof (PublishMessage), "Failed to publish dataimport status update because importid was unexpectedly empty.");
        }
        else
        {
          DataImportStatusMessage importStatusMessage = this.CreateImportStatusMessage(requestContext, dataImportId, infoMessage, errorMessage);
          requestContext.GetService<IMessageBusPublisherService>().Publish(requestContext, DataImportStepStatusScope.s_publisherTopicName, (object[]) new DataImportStatusMessage[1]
          {
            importStatusMessage
          }, false);
        }
      }
      catch (Exception ex)
      {
        requestContext.Trace(43501, TraceLevel.Warning, nameof (DataImportStepStatusScope), nameof (PublishMessage), "Failed to publish dataimport status update. Exception Type: {0}. Exception Trace: {1}", (object) ex.GetType().FullName, (object) ex.StackTrace);
      }
    }

    public void PublishInfoMessage(IVssRequestContext requestContext, string message) => this.PublishMessage(requestContext, message);

    public void PublishErrorMessage(IVssRequestContext requestContext, string message) => this.PublishMessage(requestContext, errorMessage: message);

    private DataImportStatusMessage CreateImportStatusMessage(
      IVssRequestContext requestContext,
      Guid importId,
      string infoMessage,
      string errorMessage)
    {
      Guid guid = requestContext.ServiceInstanceType();
      DataImportStatusMessage importStatusMessage = new DataImportStatusMessage();
      importStatusMessage.ImportId = importId;
      importStatusMessage.MessageCreated = DateTime.UtcNow;
      importStatusMessage.Service = guid.ToString();
      importStatusMessage.OperationName = this.m_servicingContext.CurrentServicingOperation;
      importStatusMessage.StepName = this.m_servicingContext.StepName;
      importStatusMessage.CurrentStepNumber = this.m_servicingContext.CurrentStepNumber;
      List<ServicingStep> currentStepGroupSteps = this.m_servicingContext.CurrentStepGroupSteps;
      // ISSUE: explicit non-virtual call
      importStatusMessage.TotalStepNumber = currentStepGroupSteps != null ? __nonvirtual (currentStepGroupSteps.Count) : -1;
      importStatusMessage.GroupName = this.m_servicingContext.CurrentStepGroupName;
      importStatusMessage.InfoMessage = infoMessage;
      importStatusMessage.ErrorMessage = errorMessage;
      return importStatusMessage;
    }

    public void Dispose() => this.PublishInfoMessage(this.m_servicingContext.DeploymentRequestContext, "End Step");
  }
}
