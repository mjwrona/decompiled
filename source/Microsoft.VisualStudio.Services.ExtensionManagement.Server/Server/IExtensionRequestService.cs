// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.IExtensionRequestService
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server
{
  [DefaultServiceImplementation(typeof (ExtensionRequestService))]
  public interface IExtensionRequestService : IVssFrameworkService
  {
    RequestedExtension RequestExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string requestMessage);

    List<RequestedExtension> GetRequests(IVssRequestContext requestContext);

    IList<ExtensionRequest> ResolveRequests(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string requesterId,
      string rejectMessage,
      ExtensionRequestState state);

    IList<ExtensionRequest> DeleteRequests(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName);
  }
}
