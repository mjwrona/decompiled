// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Profile.ProfileQueryContext
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Profile
{
  public class ProfileQueryContext
  {
    public ProfileQueryContext(AttributesScope scope, string containerName = null)
      : this(scope, CoreProfileAttributes.All, containerName)
    {
    }

    public ProfileQueryContext(
      AttributesScope scope,
      CoreProfileAttributes coreAttributes,
      string containerName = null)
    {
      this.ContainerScope = scope;
      this.CoreAttributes = coreAttributes;
      if (scope != AttributesScope.Core)
      {
        if (scope != (AttributesScope.Core | AttributesScope.Application))
          throw new ArgumentException(string.Format("The scope '{0}' is not supported for this operation.", (object) scope));
        ProfileArgumentValidation.ValidateApplicationContainerName(containerName);
        this.ContainerName = containerName;
      }
      else
        this.ContainerName = (string) null;
    }

    [DataMember(IsRequired = true)]
    public AttributesScope ContainerScope { get; private set; }

    [DataMember]
    public string ContainerName { get; private set; }

    [DataMember]
    public CoreProfileAttributes CoreAttributes { get; private set; }
  }
}
