// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ReleaseManagement.Server.WebServices.ReleaseManagementProjectService
// Assembly: Microsoft.TeamFoundation.ReleaseManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4E542756-E4AD-41D1-B4C6-D5898225A0D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ReleaseManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core.WebServices;
using Microsoft.VisualStudio.Services.ReleaseManagement.Project.Plugins;
using System.Web.Services;
using System.Web.Services.Protocols;

namespace Microsoft.TeamFoundation.ReleaseManagement.Server.WebServices
{
  [ClientService(ComponentName = "ReleaseManagement", RegistrationName = "ReleaseManagement", ServiceName = "ReleaseManagement", CollectionServiceIdentifier = "D8BE0E0C-341A-4C68-988D-13E96485A23F")]
  [WebService(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Linking/03", Description = "DevOps Release Management Integration web service")]
  [WebServiceBinding(Name = "IProjectMaintenanceBinding", Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Admin/03")]
  internal class ReleaseManagementProjectService : TeamFoundationWebService
  {
    [WebMethod]
    [SoapDocumentMethod(Binding = "IProjectMaintenanceBinding", Action = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Admin/03/DeleteProject")]
    public bool DeleteProject(string projectUri)
    {
      MethodInformation methodInformation = new MethodInformation(nameof (DeleteProject), MethodType.Tool, EstimatedMethodCost.Moderate);
      this.RequestContext.GetService<IReleaseManagementProjectService>().DeleteProject(this.RequestContext, projectUri);
      return true;
    }
  }
}
