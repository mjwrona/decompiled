// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.ServicingOrchestrationExceptionExtensions
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.WebApi
{
  public static class ServicingOrchestrationExceptionExtensions
  {
    private const string c_dontRetryServicingOrchestrationJobMarker = "{89814AE3-E489-41F4-A022-B541E889F582}";
    private const string c_blockedServicingOrchestrationJobMarker = "{7012936B-1443-49C4-899E-D4BE91F648F3}";
    private const string c_userErrorServicingOrchestrationJobMarker = "{8EAF91AE-F583-435D-AA1E-058AC1B7200D}";

    public static void MarkAsFatalServicingOrchestrationException(this Exception ex) => ex.Data[(object) "{89814AE3-E489-41F4-A022-B541E889F582}"] = (object) null;

    public static T AsFatalServicingOrchestrationException<T>(this T ex) where T : Exception
    {
      ex.MarkAsFatalServicingOrchestrationException();
      return ex;
    }

    public static bool IsFatalServicingOrchestrationException(this Exception ex) => ex.Data.Contains((object) "{89814AE3-E489-41F4-A022-B541E889F582}");

    public static void MarkAsBlockedServicingOrchestrationException(this Exception ex) => ex.Data[(object) "{7012936B-1443-49C4-899E-D4BE91F648F3}"] = (object) null;

    public static T AsBlockedServicingOrchestrationException<T>(this T ex) where T : Exception
    {
      ex.MarkAsBlockedServicingOrchestrationException();
      return ex;
    }

    public static bool IsBlockedServicingOrchestrationException(this Exception ex) => ex.Data.Contains((object) "{7012936B-1443-49C4-899E-D4BE91F648F3}");

    public static void MarkAsUserErrorServicingOrchestrationException(
      this Exception ex,
      string errorMessage = null)
    {
      ex.Data[(object) "{8EAF91AE-F583-435D-AA1E-058AC1B7200D}"] = (object) errorMessage;
      ex.MarkAsFatalServicingOrchestrationException();
    }

    public static T AsUserErrorServicingOrchestrationException<T>(this T ex, string errorMessage) where T : Exception
    {
      ex.MarkAsUserErrorServicingOrchestrationException(errorMessage);
      ex.MarkAsFatalServicingOrchestrationException();
      return ex;
    }

    public static bool IsUserErrorServicingOrchestrationException(this Exception ex) => ex.Data.Contains((object) "{8EAF91AE-F583-435D-AA1E-058AC1B7200D}");

    public static string GetUserErrorServicingOrchestrationExceptionMessage(this Exception ex)
    {
      if (ex.IsUserErrorServicingOrchestrationException())
        return ex.Message;
      Exception innerException = ex.InnerException;
      return innerException == null ? (string) null : innerException.GetUserErrorServicingOrchestrationExceptionMessage();
    }

    public static bool CheckServicingOrchestrationExceptionForError(this Exception ex, string error)
    {
      if (ex.Message.IndexOf(error, StringComparison.OrdinalIgnoreCase) >= 0)
        return true;
      return ex.InnerException != null && ex.InnerException.CheckServicingOrchestrationExceptionForError(error);
    }
  }
}
