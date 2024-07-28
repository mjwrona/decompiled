// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.Server.PolicyImplementationException
// Assembly: Microsoft.TeamFoundation.Policy.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C7B03386-B27B-4823-BB4F-89F7D7E42DDD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Policy.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.Policy.Server
{
  [Serializable]
  public class PolicyImplementationException : TeamFoundationServiceException
  {
    public PolicyImplementationException(string message)
      : base(message)
    {
    }

    public PolicyImplementationException(string policyTypeName, int? configurationId, Exception ex)
      : this(policyTypeName, configurationId, PolicyResources.Format("PolicyThrewAnException", (object) ex.Message), ex)
    {
    }

    public PolicyImplementationException(string policyTypeName, Exception ex)
      : this(policyTypeName, new int?(), PolicyResources.Format("PolicyThrewAnException", (object) ex.Message), ex)
    {
    }

    public PolicyImplementationException(string policyTypeName, string errorMessage)
      : this(policyTypeName, new int?(), errorMessage, (Exception) null)
    {
    }

    public PolicyImplementationException(
      string policyTypeName,
      int? configurationId,
      string errorMessage)
      : this(policyTypeName, configurationId, errorMessage, (Exception) null)
    {
    }

    public PolicyImplementationException(
      string policyTypeName,
      int? configurationId,
      string message,
      Exception exception)
      : base(PolicyImplementationException.BuildMessage(policyTypeName, configurationId, message), exception)
    {
    }

    public PolicyImplementationException(PolicyConfigurationRecord uninitializedPolicy)
      : base(PolicyImplementationException.BuildMessage(uninitializedPolicy))
    {
    }

    private static string BuildMessage(string policyTypeName, int? configurationId, string message) => !configurationId.HasValue ? PolicyResources.Format("PolicyImplementationExceptionNoId", (object) policyTypeName, (object) message) : PolicyResources.Format(nameof (PolicyImplementationException), (object) policyTypeName, (object) configurationId.Value, (object) message);

    private static string BuildMessage(PolicyConfigurationRecord configuration) => PolicyResources.Format("PolicyFailedToInitialize", (object) configuration.ConfigurationId);
  }
}
