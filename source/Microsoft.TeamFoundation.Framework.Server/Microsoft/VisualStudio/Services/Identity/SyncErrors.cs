// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.SyncErrors
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Microsoft.VisualStudio.Services.Identity
{
  public class SyncErrors
  {
    private const int c_DisplayErrorLimit = 15;
    private string m_displayName;
    private string[] m_memberIds = new string[15];
    private Exception[] m_errors = new Exception[15];
    private readonly ITFLogger m_logger;
    private int m_current;
    private const int c_summaryErrorLimit = 250;
    private const int c_summaryErrorMessageSizeLimit = 15000;
    private Dictionary<Type, ErrorInformation> m_summaryErrorInformation = new Dictionary<Type, ErrorInformation>();

    internal SyncErrors()
      : this((ITFLogger) null)
    {
    }

    internal SyncErrors(ITFLogger logger) => this.m_logger = logger ?? (ITFLogger) new ServerTraceLogger();

    internal void Initialize(string displayName)
    {
      this.m_displayName = displayName;
      this.m_current = 0;
      this.Count = 0;
    }

    public int Count { get; private set; }

    internal Dictionary<Type, ErrorInformation> SummaryErrorInformation => this.m_summaryErrorInformation;

    public void Add(string memberId, Exception exception)
    {
      if (exception is IdentityProviderUnavailableException)
        return;
      if (this.m_current < 15)
      {
        if (memberId == null)
          memberId = string.Empty;
        this.m_memberIds[this.m_current] = memberId;
        this.m_errors[this.m_current] = exception;
        ++this.m_current;
      }
      ++this.Count;
      this.AddErrorInformation(memberId, exception);
    }

    internal void LogErrors()
    {
      if (this.Count <= 0)
        return;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine(FrameworkResources.IDENTITY_SYNC_ERRORS((object) this.Count, (object) this.m_displayName));
      stringBuilder.AppendLine("++++++++++++++++++++++");
      for (int index = 0; index < this.m_current; ++index)
      {
        stringBuilder.AppendLine(FrameworkResources.IDENTITY_SYNC_ERROR((object) this.m_memberIds[index]));
        Exception error = this.m_errors[index];
        stringBuilder.AppendLine(error.Message);
        if (error.Data != null)
        {
          foreach (DictionaryEntry dictionaryEntry in error.Data)
          {
            stringBuilder.Append(dictionaryEntry.Key.ToString());
            stringBuilder.Append(": ");
            stringBuilder.AppendLine(dictionaryEntry.Value.ToString());
          }
        }
        stringBuilder.AppendLine(error.StackTrace);
        Exception innerException = error.InnerException;
        if (innerException != null)
        {
          stringBuilder.AppendLine(innerException.Message);
          if (innerException.Data != null)
          {
            foreach (DictionaryEntry dictionaryEntry in innerException.Data)
            {
              stringBuilder.Append(dictionaryEntry.Key.ToString());
              stringBuilder.Append(": ");
              stringBuilder.AppendLine(dictionaryEntry.Value.ToString());
            }
          }
          stringBuilder.AppendLine(innerException.StackTrace);
        }
        TeamFoundationTrace.TraceException(error);
      }
      if (this.Count > this.m_current)
        stringBuilder.AppendLine(FrameworkResources.IDENTITY_SYNC_ERRORS_TRUNCATED());
      string message = stringBuilder.ToString();
      TeamFoundationEventLog.Default.Log(message, TeamFoundationEventId.ActiveDirectorySyncErrors, EventLogEntryType.Warning);
      this.m_logger.Error(message);
    }

    internal void LogErrorsSummary()
    {
      foreach (KeyValuePair<Type, ErrorInformation> keyValuePair in this.m_summaryErrorInformation)
      {
        List<string> memberIds1 = keyValuePair.Value?.MemberIds;
        Exception exception = keyValuePair.Value?.Exception;
        if (!memberIds1.IsNullOrEmpty<string>() && exception != null)
        {
          foreach (IList<string> memberIds2 in memberIds1.Batch<string>(250))
            this.SafeLogError(memberIds2, exception);
        }
      }
    }

    private void AddErrorInformation(string memberId, Exception exception)
    {
      Type type = exception.GetType();
      ErrorInformation errorInformation;
      if (!this.m_summaryErrorInformation.TryGetValue(type, out errorInformation))
      {
        errorInformation = new ErrorInformation()
        {
          Exception = exception,
          MemberIds = new List<string>()
        };
        this.m_summaryErrorInformation[type] = errorInformation;
      }
      errorInformation.MemberIds.Add(memberId);
      errorInformation.Exception = exception;
    }

    internal string SafeLogError(IList<string> memberIds, Exception exception)
    {
      string message = this.BuildErrorMessage(memberIds, exception);
      TeamFoundationEventLog.Default.Log(message, TeamFoundationEventId.ActiveDirectorySyncErrors, EventLogEntryType.Warning);
      this.m_logger.Error(message);
      return message;
    }

    private string BuildErrorMessage(IList<string> memberIds, Exception exception)
    {
      StringBuilder stringBuilder1 = new StringBuilder();
      StringBuilder stringBuilder2 = new StringBuilder();
      StringBuilder stringBuilder3 = new StringBuilder();
      stringBuilder2.Append(memberIds[0]);
      for (int index = 1; index < memberIds.Count; ++index)
        stringBuilder2.Append(", " + memberIds[index]);
      stringBuilder3.AppendLine(exception.GetType().FullName);
      stringBuilder3.AppendLine(exception.Message);
      if (exception.Data != null)
      {
        foreach (DictionaryEntry dictionaryEntry in exception.Data)
        {
          stringBuilder3.Append(dictionaryEntry.Key.ToString());
          stringBuilder3.Append(": ");
          stringBuilder3.AppendLine(dictionaryEntry.Value.ToString());
        }
      }
      stringBuilder3.AppendLine(exception.StackTrace);
      Exception innerException = exception.InnerException;
      if (innerException != null)
      {
        stringBuilder3.AppendLine(innerException.Message);
        if (innerException.Data != null)
        {
          foreach (DictionaryEntry dictionaryEntry in innerException.Data)
          {
            stringBuilder3.Append(dictionaryEntry.Key.ToString());
            stringBuilder3.Append(": ");
            stringBuilder3.AppendLine(dictionaryEntry.Value.ToString());
          }
        }
        stringBuilder3.AppendLine(innerException.StackTrace);
      }
      stringBuilder1.AppendLine(FrameworkResources.IDENTITY_SYNC_ERRORS((object) memberIds.Count, (object) stringBuilder2));
      stringBuilder1.AppendLine("++++++++++++++++++++++");
      stringBuilder1.AppendLine(stringBuilder3.ToString());
      string str = stringBuilder1.ToString();
      if (str.Length > 15000)
        str = stringBuilder1.ToString().Remove(15000) + "\n" + FrameworkResources.IDENTITY_SYNC_ERRORS_TRUNCATED();
      return str;
    }
  }
}
