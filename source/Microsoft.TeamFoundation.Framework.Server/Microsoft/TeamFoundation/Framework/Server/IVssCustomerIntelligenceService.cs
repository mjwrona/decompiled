// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IVssCustomerIntelligenceService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DefaultServiceImplementation(typeof (CustomerIntelligenceService))]
  public interface IVssCustomerIntelligenceService : IVssFrameworkService
  {
    void Publish(
      IVssRequestContext requestContext,
      string area,
      string feature,
      string property,
      double value);

    void Publish(
      IVssRequestContext requestContext,
      Guid hostId,
      string user,
      Guid identityId,
      Guid identityConsistentVSID,
      DateTime timeStamp,
      string area,
      string feature,
      string property,
      double value);

    void Publish(
      IVssRequestContext requestContext,
      string area,
      string feature,
      string property,
      string value);

    void Publish(
      IVssRequestContext requestContext,
      Guid hostId,
      string user,
      Guid identityId,
      Guid identityConsistentVSID,
      DateTime timeStamp,
      string area,
      string feature,
      string property,
      string value);

    void Publish(
      IVssRequestContext requestContext,
      string area,
      string feature,
      string property,
      bool value);

    void Publish(
      IVssRequestContext requestContext,
      Guid hostId,
      string user,
      Guid identityId,
      Guid identityConsistentVSID,
      DateTime timeStamp,
      string area,
      string feature,
      string property,
      bool value);

    void Publish(
      IVssRequestContext requestContext,
      string area,
      string feature,
      CustomerIntelligenceData properties);

    void Publish(
      IVssRequestContext requestContext,
      Guid hostId,
      string area,
      string feature,
      CustomerIntelligenceData properties);

    void Publish(
      IVssRequestContext requestContext,
      Guid hostId,
      string user,
      Guid identityId,
      Guid identityConsistentVSID,
      DateTime timeStamp,
      string area,
      string feature,
      CustomerIntelligenceData properties);

    bool IsTracingEnabled(IVssRequestContext requestContext);
  }
}
