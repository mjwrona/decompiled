// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Linq.DocumentQueryException
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Net;

namespace Microsoft.Azure.Documents.Linq
{
  [Serializable]
  public sealed class DocumentQueryException : DocumentClientException
  {
    public DocumentQueryException(string message)
      : base(message, (Exception) null, new HttpStatusCode?())
    {
    }

    public DocumentQueryException(string message, Exception innerException)
      : base(message, innerException, new HttpStatusCode?())
    {
    }
  }
}
