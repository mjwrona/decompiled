// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.WebServices.Registration
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Web.Services;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Server.Core.WebServices
{
  [WebService(Name = "Registration", Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Registration/03", Description = "DevOps Registration web service")]
  [ClientService(ComponentName = "RegistrationService", RegistrationName = "RegistrationService", ServerConfiguration = ServerConfiguration.TfsTeamProjectCollection, ServiceName = "RegistrationService", CollectionServiceIdentifier = "b8f97328-80d2-412d-9810-67c5a3f4190f")]
  public class Registration : FrameworkWebService
  {
    private SecuredRegistrationManager m_registrationManager;

    public Registration() => this.m_registrationManager = this.RequestContext.GetService<SecuredRegistrationManager>();

    [WebMethod]
    [return: XmlArrayItem(ElementName = "RegistrationEntry")]
    public FrameworkRegistrationEntry[] GetRegistrationEntries(string toolId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (GetRegistrationEntries), MethodType.Normal, EstimatedMethodCost.VeryLow);
        methodInformation.AddParameter(nameof (toolId), (object) toolId);
        this.EnterMethod(methodInformation);
        if (toolId == null)
          toolId = string.Empty;
        this.RequestContext.Items["IsClientOm"] = (object) true;
        FrameworkRegistrationEntry[] registrationEntries = this.m_registrationManager.GetRegistrationEntries(this.RequestContext, toolId);
        foreach (FrameworkRegistrationEntry registrationEntry in registrationEntries)
          registrationEntry.Databases = (RegistrationDatabase[]) null;
        return registrationEntries;
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }
  }
}
