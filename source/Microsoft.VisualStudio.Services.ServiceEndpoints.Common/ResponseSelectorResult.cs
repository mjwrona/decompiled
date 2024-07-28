// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.Common.ResponseSelectorResult
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 762B8E87-3651-4560-BE0D-F9006FB93C96
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.Common.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.Common
{
  public class ResponseSelectorResult
  {
    public ResponseSelectorResult()
    {
    }

    public ResponseSelectorResult(
      IList<string> result,
      IDictionary<string, string> callbackContext,
      bool callbackRequired)
    {
      this.Result = result;
      this.CallbackContext = callbackContext;
      this.CallbackRequired = callbackRequired;
    }

    public IList<string> Result { get; set; }

    public IDictionary<string, string> CallbackContext { get; set; }

    public bool CallbackRequired { get; set; }
  }
}
