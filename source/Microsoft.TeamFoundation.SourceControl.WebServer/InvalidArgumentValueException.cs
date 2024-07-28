// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.InvalidArgumentValueException
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public class InvalidArgumentValueException : ArgumentException
  {
    public InvalidArgumentValueException()
      : base(Resources.Get("InvalidArgumentValue"))
    {
    }

    public InvalidArgumentValueException(string paramName)
      : base(Resources.Get("InvalidArgumentValue"), paramName)
    {
    }

    public InvalidArgumentValueException(string paramName, string message)
      : base(message, paramName)
    {
    }

    public InvalidArgumentValueException(string paramName, Exception innerException)
      : base(Resources.Get("InvalidArgumentValue"), paramName, innerException)
    {
    }

    public InvalidArgumentValueException(
      string message,
      string paramName,
      Exception innerException)
      : base(message, paramName, innerException)
    {
    }

    protected InvalidArgumentValueException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
