// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.StorageException
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Text;

namespace Microsoft.Azure.Cosmos.Table
{
  public class StorageException : Exception
  {
    public RequestResult RequestInformation { get; private set; }

    internal bool IsRetryable { get; set; }

    public StorageException()
      : this((RequestResult) null, (string) null, (Exception) null)
    {
    }

    public StorageException(string message)
      : this((RequestResult) null, message, (Exception) null)
    {
    }

    public StorageException(string message, Exception innerException)
      : this((RequestResult) null, message, innerException)
    {
    }

    public StorageException(RequestResult res, string message, Exception inner)
      : base(message, inner)
    {
      this.RequestInformation = res;
      this.IsRetryable = true;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder1 = new StringBuilder();
      stringBuilder1.AppendLine(base.ToString());
      if (this.RequestInformation != null)
      {
        stringBuilder1.AppendLine("Request Information");
        stringBuilder1.AppendLine("RequestID:" + this.RequestInformation.ServiceRequestID);
        double? requestCharge = this.RequestInformation.RequestCharge;
        if (requestCharge.HasValue)
        {
          StringBuilder stringBuilder2 = stringBuilder1;
          requestCharge = this.RequestInformation.RequestCharge;
          string str = "RequestCharge:" + (object) requestCharge.Value;
          stringBuilder2.AppendLine(str);
        }
        stringBuilder1.AppendLine("RequestDate:" + this.RequestInformation.RequestDate);
        stringBuilder1.AppendLine("StatusMessage:" + this.RequestInformation.HttpStatusMessage);
        stringBuilder1.AppendLine("ErrorCode:" + this.RequestInformation.ErrorCode);
        if (this.RequestInformation.ExtendedErrorInformation != null)
          stringBuilder1.AppendLine("ErrorMessage:" + this.RequestInformation.ExtendedErrorInformation.ErrorMessage);
      }
      return stringBuilder1.ToString();
    }
  }
}
