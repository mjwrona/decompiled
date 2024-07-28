// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.Utils
// Assembly: Microsoft.VisualStudio.Services.Orchestration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C0C603F4-BE31-455B-860A-9FD3B046611C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.dll

using System;

namespace Microsoft.VisualStudio.Services.Orchestration
{
  internal static class Utils
  {
    public static readonly DateTime DateTimeSafeMaxValue;

    public static string EscapeJson(string inputJson)
    {
      inputJson = inputJson.Replace("{", "{{");
      inputJson = inputJson.Replace("}", "}}");
      inputJson = inputJson.Replace(";", "%3B");
      inputJson = inputJson.Replace("=", "%3D");
      return inputJson;
    }

    public static bool IsFatal(Exception exception)
    {
      switch (exception)
      {
        case OutOfMemoryException _:
        case StackOverflowException _:
          return true;
        default:
          return false;
      }
    }

    public static Exception RetrieveCause(string details, OrchestrationSerializer serializer)
    {
      if (serializer == null)
        throw new ArgumentNullException(nameof (serializer));
      Exception exception = (Exception) null;
      try
      {
        if (!string.IsNullOrWhiteSpace(details))
          exception = serializer.Deserialize<Exception>(details);
      }
      catch (Exception ex)
      {
        exception = (Exception) new TaskFailedExceptionDeserializationException(details, ex);
      }
      return exception;
    }

    public static string SerializeCause(
      Exception originalException,
      OrchestrationSerializer serializer)
    {
      if (originalException == null)
        throw new ArgumentNullException(nameof (originalException));
      if (serializer == null)
        throw new ArgumentNullException(nameof (serializer));
      try
      {
        return serializer.Serialize((object) originalException);
      }
      catch
      {
        throw originalException;
      }
    }

    static Utils()
    {
      DateTime dateTime = DateTime.MaxValue;
      dateTime = dateTime.Subtract(TimeSpan.FromDays(1.0));
      Utils.DateTimeSafeMaxValue = dateTime.ToUniversalTime();
    }
  }
}
