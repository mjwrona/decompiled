// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Server.Exceptions.InvalidReorderOperationException
// Assembly: Microsoft.TeamFoundation.Agile.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4912F51-3FCA-4D2B-A7B5-CF15E2F3B46B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Net;

namespace Microsoft.TeamFoundation.Agile.Server.Exceptions
{
  [Serializable]
  public class InvalidReorderOperationException : TeamFoundationServiceException
  {
    public InvalidReorderOperationException(string message)
      : base(message, HttpStatusCode.BadRequest)
    {
    }
  }
}
