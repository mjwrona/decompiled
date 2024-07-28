// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Json.JsonParseException
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;
using System;
using System.Net;

namespace Microsoft.Azure.Cosmos.Json
{
  internal abstract class JsonParseException : DocumentClientException
  {
    protected JsonParseException(string message)
      : base(message, (Exception) null, new HttpStatusCode?(HttpStatusCode.BadRequest))
    {
    }
  }
}
