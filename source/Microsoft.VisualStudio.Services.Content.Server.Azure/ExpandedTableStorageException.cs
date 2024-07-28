// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.ExpandedTableStorageException
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.VisualStudio.Services.Content.Server.Azure.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Net;
using System.Text;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public class ExpandedTableStorageException : StorageException, IExceptionToStatusCodeMapper
  {
    public static void Throw(string message, StorageException e, string accountName = null)
    {
      if (e == null)
        throw new ExpandedTableStorageException(message, accountName);
      throw new ExpandedTableStorageException(message, e, accountName);
    }

    public ExpandedTableStorageException(StorageException e, string accountName = null)
      : base(e == null ? (RequestResult) null : e.RequestInformation, ExpandedTableStorageException.CreateMessage((Exception) e, accountName), (Exception) e)
    {
    }

    public ExpandedTableStorageException(string message, StorageException e, string accountName = null)
      : base(e == null ? (RequestResult) null : e.RequestInformation, ExpandedTableStorageException.CreateMessage((Exception) e, message, accountName), (Exception) e)
    {
    }

    public ExpandedTableStorageException()
    {
    }

    public ExpandedTableStorageException(string message, string accountName = null)
      : base(ExpandedTableStorageException.CreateMessage(message, accountName))
    {
    }

    public ExpandedTableStorageException(
      string message,
      Exception innerException,
      string accountName = null)
      : base(ExpandedTableStorageException.CreateMessage(message, accountName), innerException)
    {
    }

    private static string CreateMessage(string message, string accountName) => ExpandedTableStorageException.CreateMessage((Exception) null, message, accountName);

    private static string CreateMessage(Exception ex, string accountName) => ExpandedTableStorageException.CreateMessage(ex, string.Empty, accountName);

    private static string CreateMessage(Exception ex, string additionalMessage, string accountName)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (ex is ExpandedTableStorageException)
      {
        int num = ex.Message.IndexOf("\r\n");
        stringBuilder.Append(ex.Message.Substring(0, num));
        if (!string.IsNullOrEmpty(additionalMessage))
        {
          stringBuilder.AppendLine();
          stringBuilder.Append(additionalMessage);
        }
        stringBuilder.Append(ex.Message.Substring(num));
        return stringBuilder.ToString();
      }
      if (!string.IsNullOrEmpty(accountName))
        stringBuilder.AppendFormat("Storage Account: {0}, ", (object) accountName);
      if (ex is StorageException storageException && storageException.RequestInformation != null)
      {
        stringBuilder.AppendFormat("httpStatusCode: {0}", (object) storageException.RequestInformation.HttpStatusCode);
        if (storageException.RequestInformation.ExtendedErrorInformation != null)
        {
          stringBuilder.AppendFormat(", ErrorCode: {0}", (object) storageException.RequestInformation.ExtendedErrorInformation.ErrorCode);
          stringBuilder.AppendFormat(", ErrorMessage: {0}", (object) storageException.RequestInformation.ExtendedErrorInformation.ErrorMessage?.Replace("\r\n", ","));
        }
        stringBuilder.AppendFormat(", ServiceRequestID: {0}", (object) storageException.RequestInformation.ServiceRequestID);
      }
      if (!string.IsNullOrEmpty(additionalMessage))
      {
        stringBuilder.AppendLine();
        stringBuilder.Append(additionalMessage);
      }
      if (ex != null)
      {
        stringBuilder.AppendLine();
        stringBuilder.Append(ex.Message);
      }
      return stringBuilder.ToString();
    }

    public HttpStatusCode MapToStatusCode() => StorageExceptionMapper.MapStatusCode(this.RequestInformation?.HttpStatusCode);

    public Exception WrapInException() => StorageExceptionMapper.IsExpectedResponseCode(this.MapToStatusCode()) ? (Exception) new ExpectedExpandedTableStorageException(this) : (Exception) this;
  }
}
