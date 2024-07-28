// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.Common.NoneResponseSelector
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 762B8E87-3651-4560-BE0D-F9006FB93C96
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.Common.dll

using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.Common
{
  public class NoneResponseSelector : ResponseSelector
  {
    public override void AddHeaders(HttpWebRequest request)
    {
    }

    protected override ResponseSelectorResult SelectInternal(HttpResponseMessage response) => new ResponseSelectorResult((IList<string>) new List<string>(), (IDictionary<string, string>) null, false);
  }
}
