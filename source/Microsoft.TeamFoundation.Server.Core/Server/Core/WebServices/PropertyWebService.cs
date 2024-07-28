// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.WebServices.PropertyWebService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Services;

namespace Microsoft.TeamFoundation.Server.Core.WebServices
{
  [WebService(Namespace = "http://microsoft.com/webservices/")]
  [ClientService(ComponentName = "Framework", RegistrationName = "Framework", ServerConfiguration = ServerConfiguration.TfsConnection, ServiceName = "PropertyService", CollectionServiceIdentifier = "DFADDBDA-1BEB-425e-9F9F-41222287A177", ConfigurationServiceIdentifier = "FF4E1B3C-6351-4B9F-AA54-29AA9985CB77")]
  public sealed class PropertyWebService : FrameworkWebService
  {
    private readonly TeamFoundationPropertyService m_propertyService;

    public PropertyWebService()
    {
      if (!this.RequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        this.RequestContext.CheckOnPremisesDeployment(true);
      this.m_propertyService = this.RequestContext.GetService<TeamFoundationPropertyService>();
    }

    [WebMethod]
    public StreamingCollection<ArtifactPropertyValue> GetProperties(
      ArtifactSpec[] artifactSpecs,
      string[] propertyNameFilters,
      int options)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (GetProperties), MethodType.Normal, EstimatedMethodCost.Moderate);
        methodInformation.AddArrayParameter<ArtifactSpec>(nameof (artifactSpecs), (IList<ArtifactSpec>) artifactSpecs);
        methodInformation.AddArrayParameter<string>(nameof (propertyNameFilters), (IList<string>) propertyNameFilters);
        methodInformation.AddParameter(nameof (options), (object) options);
        this.EnterMethod(methodInformation);
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) artifactSpecs, nameof (artifactSpecs));
        ArgumentUtility.CheckForNull<ArtifactSpec>(artifactSpecs[0], "artifactSpecs[0]");
        ArtifactKind artifactKind = this.m_propertyService.GetArtifactKind(this.RequestContext, artifactSpecs[0].Kind);
        if (artifactKind.IsInternalKind)
          throw new ArtifactKindRestrictedException(artifactKind);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          PropertiesOptions options1 = (PropertiesOptions) options;
          resource = this.m_propertyService.GetProperties(this.RequestContext, (IEnumerable<ArtifactSpec>) artifactSpecs, (IEnumerable<string>) propertyNameFilters, options1);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        return resource.Current<StreamingCollection<ArtifactPropertyValue>>();
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

    [WebMethod]
    public void SetProperties(ArtifactPropertyValue[] artifactPropertyValues)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (SetProperties), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<ArtifactPropertyValue>(nameof (artifactPropertyValues), (IList<ArtifactPropertyValue>) artifactPropertyValues);
        this.EnterMethod(methodInformation);
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) artifactPropertyValues, nameof (artifactPropertyValues));
        ArgumentUtility.CheckForNull<ArtifactSpec>(artifactPropertyValues[0].Spec, "artifactPropertyValues[0].Spec");
        ArtifactKind artifactKind = this.m_propertyService.GetArtifactKind(this.RequestContext, artifactPropertyValues[0].Spec.Kind);
        if (artifactKind.IsInternalKind)
          throw new ArtifactKindRestrictedException(artifactKind);
        this.m_propertyService.SetProperties(this.RequestContext, (IEnumerable<ArtifactPropertyValue>) artifactPropertyValues);
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
