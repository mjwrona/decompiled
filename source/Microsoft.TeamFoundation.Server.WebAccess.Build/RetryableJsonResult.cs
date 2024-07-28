// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Build.RetryableJsonResult
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Build, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5B90139F-AF48-436C-9A4F-5104A3D8571F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Build.dll

using System;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Build
{
  public class RetryableJsonResult : SecureJsonResult
  {
    private readonly Func<object> m_retryFunction;

    public RetryableJsonResult(Func<object> retryFunction) => this.m_retryFunction = retryFunction;

    public override void ExecuteResult(ControllerContext context)
    {
      try
      {
        base.ExecuteResult(context);
      }
      catch (InvalidOperationException ex)
      {
        this.Data = this.m_retryFunction();
        base.ExecuteResult(context);
      }
    }
  }
}
