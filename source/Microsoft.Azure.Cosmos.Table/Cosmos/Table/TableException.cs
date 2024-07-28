// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.TableException
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Net;

namespace Microsoft.Azure.Cosmos.Table
{
  internal class TableException : Exception
  {
    public HttpStatusCode HttpStatusCode { get; private set; }

    public string HttpStatusMessage { get; private set; }

    public string ErrorCode { get; private set; }

    public string ErrorMessage { get; private set; }

    internal TableException(
      HttpStatusCode httpStatusCode,
      string httpStatusMessage,
      string errorCode,
      string errorMessage)
    {
      this.HttpStatusCode = httpStatusCode;
      this.HttpStatusMessage = httpStatusMessage;
      this.ErrorCode = errorCode;
      this.ErrorMessage = errorMessage;
    }
  }
}
