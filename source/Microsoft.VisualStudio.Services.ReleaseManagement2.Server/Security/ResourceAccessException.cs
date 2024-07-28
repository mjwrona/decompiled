// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security.ResourceAccessException
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security
{
  [Serializable]
  public class ResourceAccessException : TeamFoundationServiceException
  {
    public ResourceAccessException()
    {
    }

    public ResourceAccessException(string message)
      : base(message)
    {
    }

    public ResourceAccessException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected ResourceAccessException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Required.")]
    public ResourceAccessException(
      string identityName,
      ReleaseManagementSecurityPermissions permission)
      : base(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.SecurityErrorMessage, (object) identityName, SecurityConstants.PermissionResourceMap.ContainsKey(permission) ? (object) SecurityConstants.PermissionResourceMap[permission] : (object) permission.ToString()))
    {
    }
  }
}
