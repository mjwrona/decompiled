// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Exceptions.Helpers.MavenBaseException`1
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Maven.Server.Exceptions.Helpers
{
  public abstract class MavenBaseException<TException> : VssServiceException where TException : VssServiceException
  {
    protected MavenBaseException(string message)
      : base(message)
    {
    }

    protected MavenBaseException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public static string CheckStringForNullOrWhiteSpace(string value, string argumentName)
    {
      if (string.IsNullOrWhiteSpace(value))
        MavenBaseException<TException>.ThrowException(argumentName);
      return value;
    }

    public static T CheckForNull<T>(T value, string argumentName)
    {
      if ((object) value == null)
        MavenBaseException<TException>.ThrowException(argumentName);
      return value;
    }

    private static void ThrowException(string argumentName) => throw (object) (TException) Activator.CreateInstance(typeof (TException), (object) Resources.Error_ArgumentNullOrEmpty((object) argumentName));
  }
}
