// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.Charts.ChartsException
// Assembly: Microsoft.Azure.Boards.Charts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EABADF19-3537-403E-8E3C-4185CE6D1F3B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.Charts.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Boards.Charts
{
  public class ChartsException : TeamFoundationServiceException
  {
    public ChartsException(string message)
      : base(message)
    {
    }

    public ChartsException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected ChartsException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
