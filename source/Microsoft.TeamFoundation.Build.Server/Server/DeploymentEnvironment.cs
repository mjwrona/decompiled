// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DeploymentEnvironment
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server
{
  [ClassVisibility(ClientVisibility.Public, ClientVisibility.Private)]
  [RequiredClientService("TeamFoundationStrongBoxService", "StrongBox")]
  [CallOnDeserialization("AfterDeserialize")]
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  public class DeploymentEnvironment : IValidatable
  {
    private List<StrongBoxItemInfo> m_environmentProperties = new List<StrongBoxItemInfo>();
    private Dictionary<string, StrongBoxItemInfo> m_environmentPropertiesDictionary = new Dictionary<string, StrongBoxItemInfo>();

    private DeploymentEnvironment()
    {
    }

    public DeploymentEnvironment(
      DeploymentEnvironmentMetadata metadata,
      List<StrongBoxItemInfo> environmentInfo)
    {
      this.EnvironmentMetadata = metadata;
      this.InitializeProperties(environmentInfo);
    }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public DeploymentEnvironmentMetadata EnvironmentMetadata { get; set; }

    [ClientProperty(ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public List<StrongBoxItemInfo> EnvironmentPropertiesInfo => this.m_environmentProperties;

    internal string GetEnvironmentPropertyValue(IVssRequestContext requestContext, string key)
    {
      requestContext.TraceEnter(0, "Deployment", "Service", nameof (GetEnvironmentPropertyValue));
      try
      {
        return this.FetchStrongBoxValue(requestContext, this.m_environmentPropertiesDictionary[key]);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "Deployment", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(0, "Deployment", "Service", nameof (GetEnvironmentPropertyValue));
      }
    }

    internal ConnectedService GetConnectedService(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(0, "Deployment", "Service", nameof (GetConnectedService));
      try
      {
        return requestContext.GetService<TeamFoundationConnectedServicesService>().GetConnectedService(requestContext, this.EnvironmentMetadata.ConnectedServiceName, this.EnvironmentMetadata.TeamProject);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "Deployment", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(0, "Deployment", "Service", nameof (GetConnectedService));
      }
    }

    private void InitializeProperties(List<StrongBoxItemInfo> propsList)
    {
      foreach (StrongBoxItemInfo props in propsList)
      {
        this.m_environmentProperties.Add(props);
        this.m_environmentPropertiesDictionary.Add(props.LookupKey, props);
      }
    }

    private string FetchStrongBoxValue(IVssRequestContext requestContext, StrongBoxItemInfo info)
    {
      requestContext.TraceEnter(0, "Deployment", "Service", nameof (FetchStrongBoxValue));
      try
      {
        return requestContext.GetService<TeamFoundationStrongBoxService>().GetString(requestContext, info.DrawerId, info.LookupKey);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "Deployment", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(0, "Deployment", "Service", nameof (FetchStrongBoxValue));
      }
    }

    void IValidatable.Validate(
      IVssRequestContext requestContext,
      ValidationContext validationContext)
    {
      this.Validate(requestContext, validationContext);
    }

    internal void Validate(IVssRequestContext requestContext, ValidationContext validationContext)
    {
      this.EnvironmentMetadata.Validate(requestContext, validationContext);
      if (requestContext.GetService<TeamFoundationConnectedServicesService>().GetConnectedService(requestContext, this.EnvironmentMetadata.ConnectedServiceName, this.EnvironmentMetadata.TeamProject) == null)
        throw new ArgumentException(BuildTypeResource.InvalidInputParameterNull((object) "ConnectedService"));
    }
  }
}
