// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.IOrganizationPolicyService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Organization
{
  [DefaultServiceImplementation(typeof (OrganizationPolicyService))]
  public interface IOrganizationPolicyService : IVssFrameworkService
  {
    PolicyInfo GetPolicyInfo(IVssRequestContext context, string policyName);

    Policy<T> GetPolicy<T>(IVssRequestContext context, string policyName, T defaultValue);

    void SetPolicyValue<T>(IVssRequestContext context, string policyName, T value);

    void SetPolicyEnforcementValue(IVssRequestContext context, string policyName, bool enforce);
  }
}
