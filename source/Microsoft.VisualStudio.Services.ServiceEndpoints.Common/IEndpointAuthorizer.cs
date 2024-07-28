// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.Common.IEndpointAuthorizer
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 762B8E87-3651-4560-BE0D-F9006FB93C96
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.Common.dll

using System.ComponentModel.Composition;
using System.Net;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.Common
{
  [InheritedExport]
  public interface IEndpointAuthorizer
  {
    bool SupportsAbsoluteEndpoint { get; }

    void AuthorizeRequest(HttpWebRequest request, string resourceUrl);

    string GetEndpointUrl();

    string GetServiceEndpointType();
  }
}
