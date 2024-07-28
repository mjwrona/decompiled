// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.ICustomerSupportRequestService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [DefaultServiceImplementation(typeof (CustomerSupportRequestService))]
  public interface ICustomerSupportRequestService : IVssFrameworkService
  {
    Task CreateCustomerSupportRequestTicket(
      IVssRequestContext requestContext,
      CustomerSupportRequest customerSupportRequest);

    void SendNotificationToSupportEngineer(
      IVssRequestContext requestContext,
      CustomerSupportRequest customerSupportRequest,
      string failureReason);
  }
}
