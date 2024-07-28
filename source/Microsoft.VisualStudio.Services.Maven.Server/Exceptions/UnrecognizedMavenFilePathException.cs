// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Exceptions.UnrecognizedMavenFilePathException
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Maven.Server.Exceptions
{
  [Serializable]
  internal class UnrecognizedMavenFilePathException : Exception
  {
    public UnrecognizedMavenFilePathException()
    {
    }

    public UnrecognizedMavenFilePathException(string message)
      : base(message)
    {
    }

    public UnrecognizedMavenFilePathException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected UnrecognizedMavenFilePathException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
