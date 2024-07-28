// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DeploymentEnvironmentCreationData
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server
{
  [ClassVisibility(ClientVisibility.Public)]
  [CallOnSerialization("BeforeSerialize")]
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  public class DeploymentEnvironmentCreationData : IValidatable
  {
    private List<InformationField> m_environmentProperties;

    public DeploymentEnvironmentCreationData()
    {
    }

    public DeploymentEnvironmentCreationData(
      string name,
      string teamProject,
      string connectedServiceName,
      DeploymentEnvironmentKind kind,
      string friendlyName,
      string description,
      Dictionary<string, string> environmentProperties)
    {
      this.EnvironmentMetadata = new DeploymentEnvironmentMetadata(name, teamProject, connectedServiceName, kind, friendlyName, description);
      this.SetEnvironmentProperties(environmentProperties);
    }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public DeploymentEnvironmentMetadata EnvironmentMetadata { get; set; }

    internal void SetEnvironmentProperties(Dictionary<string, string> environmentProperties)
    {
      if (environmentProperties != null && environmentProperties.Count > 0)
        this.m_environmentProperties = DeploymentEnvironmentCreationData.ConvertDictionaryToFields(environmentProperties);
      else
        this.m_environmentProperties = new List<InformationField>();
    }

    [ClientProperty(ClientVisibility.Private, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public List<InformationField> EnvironmentPropertiesValue
    {
      get
      {
        if (this.m_environmentProperties == null)
          this.m_environmentProperties = new List<InformationField>();
        return this.m_environmentProperties;
      }
    }

    void IValidatable.Validate(
      IVssRequestContext requestContext,
      ValidationContext validationContext)
    {
      this.Validate(requestContext, validationContext);
    }

    internal virtual void Validate(
      IVssRequestContext requestContext,
      ValidationContext validationContext)
    {
      Validation.CheckValidatable(requestContext, "EnvironmentMetadata", (IValidatable) this.EnvironmentMetadata, false, validationContext);
    }

    private static List<InformationField> ConvertDictionaryToFields(
      Dictionary<string, string> dictionary)
    {
      List<InformationField> fields = new List<InformationField>(dictionary.Count);
      foreach (KeyValuePair<string, string> keyValuePair in dictionary)
        fields.Add(new InformationField(keyValuePair.Key, keyValuePair.Value));
      return fields;
    }
  }
}
