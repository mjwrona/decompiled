// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.IArmAdapterServiceConfigurationHelper
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public interface IArmAdapterServiceConfigurationHelper
  {
    bool ArmApiEndpointVersionsUpdateEnabled(IVssRequestContext requestContext);

    string ArmAgreementApiVersion(IVssRequestContext requestContext);

    string ArmApiVersion(IVssRequestContext requestContext);

    string ArmResourceAccountApiVersion(IVssRequestContext requestContext);

    string ArmResourceAccountExtensionsApiVersion(IVssRequestContext requestContext);

    string ArmResourceGroupApiVersion(IVssRequestContext requestContext);

    string ClassicAdministratorsVersion(IVssRequestContext requestContext);

    string RateCardApiVersion(IVssRequestContext requestContext);

    string RegisterApiVersion(IVssRequestContext requestContext);

    string RoleAssignmentsVersion(IVssRequestContext requestContext);

    string RoleDefinitionsVersion(IVssRequestContext requestContext);
  }
}
