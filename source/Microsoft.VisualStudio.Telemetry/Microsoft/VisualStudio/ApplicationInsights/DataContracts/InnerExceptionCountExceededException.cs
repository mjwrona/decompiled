// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.DataContracts.InnerExceptionCountExceededException
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.ApplicationInsights.DataContracts
{
  [Serializable]
  internal class InnerExceptionCountExceededException : Exception
  {
    public InnerExceptionCountExceededException()
    {
    }

    public InnerExceptionCountExceededException(string message)
      : base(message)
    {
    }

    public InnerExceptionCountExceededException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected InnerExceptionCountExceededException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
