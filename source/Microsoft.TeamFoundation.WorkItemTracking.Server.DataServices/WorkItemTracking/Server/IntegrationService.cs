// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.IntegrationService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 929F0284-16B2-4277-9F4A-B615689A77D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Build.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [WebServiceBinding(Name = "IProjectMaintenanceBinding", Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Admin/03")]
  [WebService(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Linking/03", Description = "Team Foundation WorkItemTracking Integration web service")]
  public class IntegrationService : WorkItemTrackingWebService
  {
    [WebMethod]
    [SoapHeader("requestHeader", Direction = SoapHeaderDirection.In)]
    public Artifact[] GetReferencingArtifacts(string[] uriList)
    {
      Artifact[] artifacts = (Artifact[]) null;
      DataAccessLayerImpl dal = (DataAccessLayerImpl) null;
      MethodInformation methodInformation = new MethodInformation(nameof (GetReferencingArtifacts), MethodType.Normal, EstimatedMethodCost.Low);
      methodInformation.AddArrayParameter<string>(nameof (uriList), (IList<string>) uriList);
      this.EnterMethod(methodInformation);
      IVssIdentity user = (IVssIdentity) this.RequestContext.GetUserIdentity();
      this.ExecuteWebMethod(methodInformation, nameof (IntegrationService), 900389, 900605, AccessIntent.Read, (Action) (() =>
      {
        if (!this.User.Identity.IsAuthenticated)
          throw new SoapException(ResourceStrings.Get("BadServerConfig"), Soap12FaultCodes.ReceiverFaultCode, WorkItemTrackingFaultCodes.BadServerConfig);
        dal = new DataAccessLayerImpl(this.RequestContext);
        artifacts = dal.GetReferencingArtifacts(user, uriList);
      }), (Action<Exception>) (e => ExceptionManager.HandleStaleViewsException(new DataAccessLayerImpl(this.RequestContext), this.RequestContext, (XmlElement) null, user, e)));
      return artifacts;
    }

    [WebMethod]
    [SoapHeader("requestHeader", Direction = SoapHeaderDirection.In)]
    public Artifact[] GetReferencingArtifactsWithFilter(string[] uriList, LinkFilter[] filters)
    {
      this.CheckAndBlockWitSoapAccess();
      DataAccessLayerImpl dal = (DataAccessLayerImpl) null;
      Artifact[] artifacts = (Artifact[]) null;
      MethodInformation methodInformation = new MethodInformation(nameof (GetReferencingArtifactsWithFilter), MethodType.Normal, EstimatedMethodCost.Low);
      methodInformation.AddArrayParameter<string>(nameof (uriList), (IList<string>) uriList);
      this.EnterMethod(methodInformation);
      IVssIdentity user = (IVssIdentity) this.RequestContext.GetUserIdentity();
      this.ExecuteWebMethod(methodInformation, nameof (IntegrationService), 900390, 900606, AccessIntent.Read, (Action) (() =>
      {
        if (!this.User.Identity.IsAuthenticated)
          throw new SoapException(ResourceStrings.Get("BadServerConfig"), Soap12FaultCodes.ReceiverFaultCode, WorkItemTrackingFaultCodes.BadServerConfig);
        dal = new DataAccessLayerImpl(this.RequestContext);
        artifacts = dal.GetReferencingArtifacts(user, uriList, filters);
      }), (Action<Exception>) (e => ExceptionManager.HandleStaleViewsException(new DataAccessLayerImpl(this.RequestContext), this.RequestContext, (XmlElement) null, user, e)));
      return artifacts;
    }

    [WebMethod]
    [SoapDocumentMethod(Binding = "IProjectMaintenanceBinding", Action = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Admin/03/DeleteProject")]
    [SoapHeader("requestHeader", Direction = SoapHeaderDirection.In)]
    public bool DeleteProject(string projectUri)
    {
      this.CheckAndBlockWitSoapAccess();
      bool returnVal = false;
      MethodInformation methodInformation = new MethodInformation(nameof (DeleteProject), MethodType.Admin, EstimatedMethodCost.Low);
      methodInformation.AddParameter(nameof (projectUri), (object) projectUri);
      this.EnterMethod(methodInformation);
      this.ExecuteWebMethod(methodInformation, nameof (IntegrationService), 900391, 900607, AccessIntent.Write, (Action) (() =>
      {
        if (!this.User.Identity.IsAuthenticated)
          throw new SoapException(ResourceStrings.Get("BadServerConfig"), Soap12FaultCodes.ReceiverFaultCode, WorkItemTrackingFaultCodes.BadServerConfig);
        if (!this.RequestContext.GetExtension<IAuthorizationProviderFactory>().IsPermitted(this.RequestContext, PermissionNamespaces.Project + projectUri, "DELETE", this.RequestContext.UserContext))
          throw new UnauthorizedAccessException();
        returnVal = new DataAccessLayerImpl(this.RequestContext).ProjectDelete(projectUri, false);
      }));
      return returnVal;
    }

    [WebMethod]
    [SoapHeader("requestHeader", Direction = SoapHeaderDirection.In)]
    public Artifact[] GetArtifacts(string[] artifactUris)
    {
      this.CheckAndBlockWitSoapAccess();
      Artifact[] artifacts = (Artifact[]) null;
      DataAccessLayerImpl dal = (DataAccessLayerImpl) null;
      MethodInformation methodInformation = new MethodInformation(nameof (GetArtifacts), MethodType.Normal, EstimatedMethodCost.Low);
      methodInformation.AddArrayParameter<string>(nameof (artifactUris), (IList<string>) artifactUris);
      this.EnterMethod(methodInformation);
      IVssIdentity user = (IVssIdentity) this.RequestContext.GetUserIdentity();
      this.ExecuteWebMethod(methodInformation, nameof (IntegrationService), 900394, 900608, AccessIntent.Read, (Action) (() =>
      {
        if (!this.User.Identity.IsAuthenticated)
          throw new SoapException(ResourceStrings.Get("BadServerConfig"), Soap12FaultCodes.ReceiverFaultCode, WorkItemTrackingFaultCodes.BadServerConfig);
        dal = new DataAccessLayerImpl(this.RequestContext);
        artifacts = dal.GetArtifacts(user, artifactUris);
      }), (Action<Exception>) (e => ExceptionManager.HandleStaleViewsException(new DataAccessLayerImpl(this.RequestContext), this.RequestContext, (XmlElement) null, user, e)));
      return artifacts;
    }

    [SoapDocumentMethod("http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Notification/03/Notify", RequestNamespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Notification/03")]
    [WebMethod]
    [SoapHeader("requestHeader", Direction = SoapHeaderDirection.In)]
    public void Notify(string eventXml, string tfsIdentityXml)
    {
      this.CheckAndBlockWitSoapAccess();
      this.RequestContext.Trace(900396, TraceLevel.Info, "WebServices", nameof (IntegrationService), "BisEvent: {0}", (object) eventXml);
      MethodInformation methodInformation = new MethodInformation(nameof (Notify), MethodType.Tool, EstimatedMethodCost.Low);
      methodInformation.AddParameter(nameof (eventXml), (object) eventXml);
      methodInformation.AddParameter(nameof (tfsIdentityXml), (object) tfsIdentityXml);
      this.EnterMethod(methodInformation);
      this.ExecuteWebMethod(methodInformation, nameof (IntegrationService), 900395, 900612, AccessIntent.Read, (Action) (() =>
      {
        if (!this.User.Identity.IsAuthenticated)
          throw new SoapException(ResourceStrings.Get("BadServerConfig"), Soap12FaultCodes.ReceiverFaultCode, WorkItemTrackingFaultCodes.BadServerConfig);
        BuildCompletionEvent buildCompletionEvent = (BuildCompletionEvent) new XmlSerializer(typeof (BuildCompletionEvent)).Deserialize((XmlReader) new XmlNodeReader((XmlNode) XmlUtility.GetDocument(eventXml)));
        this.RequestContext.Trace(900609, TraceLevel.Info, "WebServices", nameof (IntegrationService), buildCompletionEvent.Url);
        this.RequestContext.Trace(900609, TraceLevel.Info, "WebServices", nameof (IntegrationService), buildCompletionEvent.Id);
        this.RequestContext.Trace(900609, TraceLevel.Info, "WebServices", nameof (IntegrationService), buildCompletionEvent.TeamProject);
        if (!this.RequestContext.GetService<IdentityService>().IsMember(this.RequestContext, GroupWellKnownIdentityDescriptors.ServiceUsersGroup, this.RequestContext.UserContext))
          throw new SoapException(ResourceStrings.Get("UserNotInServiceGroup"), Soap12FaultCodes.ReceiverFaultCode, WorkItemTrackingFaultCodes.CallerNotServiceAccount);
        new DataAccessLayerImpl(this.RequestContext).AddNewBuild(buildCompletionEvent.Id, buildCompletionEvent.TeamProject, buildCompletionEvent.Configuration);
      }));
    }
  }
}
