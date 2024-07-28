// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions.TaskExtension
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C3F75541-7C8A-4AF6-A47E-709CEEE7550D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Common.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions
{
  public static class TaskExtension
  {
    public static T GetResult<T>(this Task<T> task, CancellationToken token)
    {
      try
      {
        if (task == null)
          throw new ArgumentNullException(nameof (task));
        task.Wait(token);
        return task.Result;
      }
      catch (AggregateException ex)
      {
        throw new ReleaseManagementExternalServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}", (object) ExceptionsUtilities.GetAllInnerExceptionsMessages(ex)), ex.InnerException);
      }
    }

    public static T SyncResult<T>(this Task<T> task, CancellationToken token)
    {
      if (task == null)
        throw new ArgumentNullException(nameof (task));
      try
      {
        task.Wait(token);
        return task.SyncResult<T>();
      }
      catch (Exception ex)
      {
        return task.SyncResult<T>();
      }
    }
  }
}
