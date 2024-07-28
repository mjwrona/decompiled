// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Linq.DocumentQueryException
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;
using System;
using System.Net;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Cosmos.Linq
{
  [Serializable]
  internal sealed class DocumentQueryException : DocumentClientException
  {
    public DocumentQueryException(string message)
      : base(message, (Exception) null, new HttpStatusCode?())
    {
    }

    public DocumentQueryException(string message, Exception innerException)
      : base(message, innerException, new HttpStatusCode?())
    {
    }

    private DocumentQueryException(SerializationInfo info, StreamingContext context)
      : base(info, context, new HttpStatusCode?())
    {
    }
  }
}
