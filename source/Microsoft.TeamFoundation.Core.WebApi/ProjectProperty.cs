// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Core.WebApi.ProjectProperty
// Assembly: Microsoft.TeamFoundation.Core.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3A82A796-05AB-42F0-97D0-CB8516E08665
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Core.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Core.WebApi
{
  [DataContract]
  public class ProjectProperty : ISecuredObject
  {
    public const int MaxPropertyNameLength = 256;
    public const int MaxPropertyValueLength = 32768;
    public const string SystemPropertyPrefix = "System.";
    public static readonly char JsonPatchOperationPathSeparator = "/"[0];
    private static readonly char[] s_invalidProjectPropertyNameChars = new char[2]
    {
      ProjectProperty.JsonPatchOperationPathSeparator,
      ','
    };

    public ProjectProperty()
    {
    }

    public ProjectProperty(string name, object value)
    {
      this.Name = name;
      this.Value = value;
    }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public object Value { get; set; }

    public ProjectProperty Clone() => new ProjectProperty(this.Name, this.Value)
    {
      ProjectId = this.ProjectId
    };

    Guid ISecuredObject.NamespaceId => TeamProjectSecurityConstants.NamespaceId;

    int ISecuredObject.RequiredPermissions => TeamProjectSecurityConstants.GenericRead;

    internal Guid ProjectId { get; set; }

    string ISecuredObject.GetToken()
    {
      ArgumentUtility.CheckForEmptyGuid(this.ProjectId, "ProjectId");
      return TeamProjectSecurityConstants.GetToken(ProjectInfo.GetProjectUri(this.ProjectId));
    }

    public static void NormalizePropertyNameFilters(
      ref IEnumerable<string> propertyNameFilters,
      string parameterName)
    {
      if (propertyNameFilters == null)
        return;
      IList<string> stringList = (IList<string>) new List<string>();
      foreach (string propertyName in propertyNameFilters)
        stringList.Add(ProjectProperty.NormalizePropertyName(propertyName));
      propertyNameFilters = (IEnumerable<string>) stringList;
    }

    public static void NormalizeProperties(
      ref IList<ProjectProperty> properties,
      string parameterName)
    {
      if (properties == null)
        return;
      for (int index = 0; index < properties.Count; ++index)
      {
        ProjectProperty property = properties[index];
        ProjectProperty.NormalizeProperty(ref property, parameterName);
      }
    }

    public static void NormalizeProperty(ref ProjectProperty property, string parameterName)
    {
      ArgumentUtility.CheckForNull<ProjectProperty>(property, parameterName);
      property.Name = ProjectProperty.NormalizePropertyName(property.Name);
      property.Value = ProjectProperty.NormalizePropertyValue(property.Value);
    }

    public static JsonPatchDocument CreateJsonPatchDocument(
      IEnumerable<ProjectProperty> projectProperties)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) projectProperties, nameof (projectProperties));
      JsonPatchDocument jsonPatchDocument = new JsonPatchDocument();
      foreach (ProjectProperty projectProperty in projectProperties)
      {
        ArgumentUtility.CheckForNull<ProjectProperty>(projectProperty, "projectProperty");
        JsonPatchOperation jsonPatchOperation = new JsonPatchOperation()
        {
          Operation = projectProperty.Value == null ? Operation.Remove : Operation.Add,
          Path = ProjectProperty.JsonPatchOperationPathSeparator.ToString() + projectProperty.Name,
          Value = projectProperty.Value
        };
        jsonPatchDocument.Add(jsonPatchOperation);
      }
      return jsonPatchDocument;
    }

    private static string NormalizePropertyName(string propertyName)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(propertyName, nameof (propertyName));
      propertyName = propertyName.Trim();
      if (propertyName.Length > 256 || propertyName.IndexOfAny(ProjectProperty.s_invalidProjectPropertyNameChars) >= 0)
        throw new ArgumentException(WebApiResources.CSS_INVALID_PROJECT_PROPERTY_NAME());
      return propertyName;
    }

    private static object NormalizePropertyValue(object propertyValue)
    {
      switch (propertyValue)
      {
        case null:
          return propertyValue;
        case string str when ((string) (propertyValue = (object) str.Trim().Replace('\t', ' '))).Length > 32768:
          throw new ArgumentException(WebApiResources.ProjectPropertyValueTooLong((object) 32768));
        case byte[] _:
          throw new ArgumentException(WebApiResources.ProjectPropertyValueTypeUnsupported((object) typeof (byte[])));
        default:
          PropertyValidation.ValidatePropertyValue(nameof (propertyValue), propertyValue);
          goto case null;
      }
    }
  }
}
