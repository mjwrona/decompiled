// Decompiled with JetBrains decompiler
// Type: WebGrease.LogManager
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using WebGrease.Activities;

namespace WebGrease
{
  public class LogManager
  {
    private static readonly object MessageLockObject = new object();
    private readonly Action<string, MessageImportance> information;
    private readonly LogExtendedError extendedWarning;
    private readonly Action<string> warning;
    private readonly LogError error;
    private readonly Action<string> errorMessage;
    private readonly LogExtendedError extendedError;

    public LogManager(
      Action<string, MessageImportance> logInformation,
      Action<string> logWarning,
      LogExtendedError logExtendedWarning,
      Action<string> logErrorMessage,
      LogError logError,
      LogExtendedError logExtendedError,
      bool? treatWarningsAsErrors = false)
    {
      this.TreatWarningsAsErrors = true;
      if (treatWarningsAsErrors.HasValue)
      {
        bool? nullable = treatWarningsAsErrors;
        this.TreatWarningsAsErrors = nullable.GetValueOrDefault() && nullable.HasValue;
      }
      this.information = logInformation;
      this.warning = logWarning;
      this.extendedWarning = logExtendedWarning;
      this.error = logError;
      this.errorMessage = logErrorMessage;
      this.extendedError = logExtendedError;
      this.HasExtendedErrorHandler = logExtendedError != null;
    }

    public event EventHandler ErrorOccurred;

    public bool TreatWarningsAsErrors { get; set; }

    public bool HasExtendedErrorHandler { get; set; }

    public void Information(string message, MessageImportance messageImportance = MessageImportance.Normal)
    {
      if (this.information == null)
        return;
      this.information(message, messageImportance);
    }

    public void Warning(string message)
    {
      if (this.TreatWarningsAsErrors)
      {
        this.Error(message);
      }
      else
      {
        if (this.warning == null)
          return;
        lock (LogManager.MessageLockObject)
          this.warning(message);
      }
    }

    public void Warning(
      string subcategory,
      string errorCode,
      string helpKeyword,
      string file,
      int? lineNumber,
      int? columnNumber,
      int? endLineNumber,
      int? endColumnNumber,
      string message)
    {
      if (this.TreatWarningsAsErrors)
      {
        this.Error(subcategory, errorCode, helpKeyword, file, lineNumber, columnNumber, endLineNumber, endColumnNumber, message);
      }
      else
      {
        if (this.extendedWarning == null)
          return;
        lock (LogManager.MessageLockObject)
          this.extendedWarning(subcategory, errorCode, helpKeyword, file, lineNumber, columnNumber, endLineNumber, endColumnNumber, message);
      }
    }

    public void Error(string message)
    {
      this.ErrorHasOccurred();
      if (this.errorMessage == null)
        return;
      lock (LogManager.MessageLockObject)
        this.errorMessage(message);
    }

    public void Error(Exception exception, string customMessage = null, string file = null)
    {
      this.ErrorHasOccurred();
      if (exception is BuildWorkflowException workflowException && this.extendedError != null)
      {
        lock (LogManager.MessageLockObject)
          this.extendedError(workflowException.Subcategory, workflowException.ErrorCode, workflowException.HelpKeyword, workflowException.File, new int?(workflowException.LineNumber), new int?(workflowException.ColumnNumber), new int?(workflowException.EndLineNumber), new int?(workflowException.EndColumnNumber), workflowException.Message);
      }
      else
      {
        if (this.error == null)
          return;
        lock (LogManager.MessageLockObject)
          this.error(exception, customMessage, file);
      }
    }

    public void Error(
      string subcategory,
      string errorCode,
      string helpKeyword,
      string file,
      int? lineNumber,
      int? columnNumber,
      int? endLineNumber,
      int? endColumnNumber,
      string message)
    {
      if (this.extendedError == null)
        return;
      this.ErrorHasOccurred();
      lock (LogManager.MessageLockObject)
        this.extendedError(subcategory, errorCode, helpKeyword, file, lineNumber, columnNumber, endLineNumber, endColumnNumber, message);
    }

    private void ErrorHasOccurred()
    {
      if (this.ErrorOccurred == null)
        return;
      lock (LogManager.MessageLockObject)
        this.ErrorOccurred((object) this, EventArgs.Empty);
    }
  }
}
