// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.LocationSecurityConstants
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class LocationSecurityConstants
  {
    public static readonly Guid NamespaceId = new Guid("2725D2BC-7520-4AF4-B0E3-8D876494731F");
    public static readonly char PathSeparator = '/';
    public static readonly string NamespaceRootToken = LocationSecurityConstants.PathSeparator.ToString();
    public static readonly string ServiceDefinitionsToken = LocationSecurityConstants.NamespaceRootToken + "ServiceDefinitions";
    public static readonly string AccessMappingsToken = LocationSecurityConstants.NamespaceRootToken + "AccessMappings";
    public const int Read = 1;
    public const int Write = 2;
    public const int AllPermissions = 3;
  }
}
