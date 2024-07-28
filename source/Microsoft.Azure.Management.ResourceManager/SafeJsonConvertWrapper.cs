// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.SafeJsonConvertWrapper
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.Rest;
using Microsoft.Rest.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.Azure.Management.ResourceManager
{
  public static class SafeJsonConvertWrapper
  {
    public static string SerializeDeployment(Deployment deployment, JsonSerializerSettings settings)
    {
      SafeJsonConvertWrapper.CheckSerializationForDeploymentProperties(deployment.Properties);
      return SafeJsonConvert.SerializeObject((object) deployment, settings);
    }

    public static string SerializeScopeDeployment(
      ScopedDeployment deployment,
      JsonSerializerSettings settings)
    {
      SafeJsonConvertWrapper.CheckSerializationForDeploymentProperties(deployment.Properties);
      return SafeJsonConvert.SerializeObject((object) deployment, settings);
    }

    public static void CheckSerializationForDeploymentProperties(DeploymentProperties properties)
    {
      if (properties == null)
        return;
      if (properties.Template is string template)
      {
        try
        {
          properties.Template = (object) JObject.Parse(template);
        }
        catch (JsonException ex)
        {
          throw new SerializationException("Unable to serialize template.", (Exception) ex);
        }
      }
      if (properties.Parameters is string parameters)
      {
        try
        {
          JObject jobject = JObject.Parse(parameters);
          properties.Parameters = (object) (jobject["parameters"] ?? (JToken) jobject);
        }
        catch (JsonException ex)
        {
          throw new SerializationException("Unable to serialize template parameters.", (Exception) ex);
        }
      }
    }

    public static string SerializeDeploymentWhatIf(
      DeploymentWhatIf deploymentWhatIf,
      JsonSerializerSettings settings)
    {
      if (deploymentWhatIf.Properties != null)
      {
        if (deploymentWhatIf.Properties.Template is string template)
        {
          try
          {
            deploymentWhatIf.Properties.Template = (object) JObject.Parse(template);
          }
          catch (JsonException ex)
          {
            throw new SerializationException("Unable to serialize template.", (Exception) ex);
          }
        }
        if (deploymentWhatIf.Properties.Parameters is string parameters)
        {
          try
          {
            JObject jobject = JObject.Parse(parameters);
            deploymentWhatIf.Properties.Parameters = (object) (jobject["parameters"] ?? (JToken) jobject);
          }
          catch (JsonException ex)
          {
            throw new SerializationException("Unable to serialize template parameters.", (Exception) ex);
          }
        }
      }
      return SafeJsonConvert.SerializeObject((object) deploymentWhatIf, settings);
    }
  }
}
