// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardValidatorBase
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.Azure.Boards.Agile.Common.Exceptions;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public abstract class BoardValidatorBase
  {
    protected IVssRequestContext RequestContext;

    public List<string> Errors { get; protected set; }

    public bool HasErrors => this.Errors.Count > 0;

    public void ResetError() => this.Errors.Clear();

    protected BoardValidatorBase(IVssRequestContext requestContext)
    {
      this.Errors = new List<string>();
      this.RequestContext = requestContext;
    }

    public string GetUserFriendlyErrorMessage()
    {
      StringBuilder sb = new StringBuilder();
      this.Errors.ForEach((Action<string>) (e => sb.AppendLine(e)));
      return sb.ToString();
    }

    public abstract bool Validate(bool throwOnFail = false);

    protected void AddError(string message)
    {
      this.Errors.Add(message);
      this.RequestContext.Trace(240301, TraceLevel.Error, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, message);
    }

    protected void Assert(Func<bool> evaluator, string errorFormat, params object[] args)
    {
      if (!evaluator())
      {
        this.AddError(string.Format(errorFormat, args));
        throw new BoardValidatorException();
      }
    }

    protected void Assert(Func<bool> evaluator, string message)
    {
      if (!evaluator())
      {
        this.AddError(message);
        throw new BoardValidatorException();
      }
    }

    protected void AssertException(
      Func<bool> evaluator,
      Func<BoardValidatorExceptionBase> exceptionHandler)
    {
      if (!evaluator())
      {
        BoardValidatorExceptionBase validatorExceptionBase = exceptionHandler();
        this.AddError(validatorExceptionBase.Message);
        throw validatorExceptionBase;
      }
    }

    protected bool ExecuteValidator(Action validator, bool throwOnFail = false)
    {
      try
      {
        validator();
        return !this.HasErrors;
      }
      catch (BoardValidatorException ex)
      {
        if (throwOnFail)
          throw ex;
        return false;
      }
    }
  }
}
