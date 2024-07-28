// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.UserMapping.IUserAccountMappingChangeExtension
// Assembly: Microsoft.VisualStudio.Services.Licensing.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F3070F25-7414-49A0-9C00-005379F04A49
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Licensing;
using System;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Services.UserMapping
{
  [InheritedExport]
  public interface IUserAccountMappingChangeExtension
  {
    void ProcessUserStatusChange(
      IVssRequestContext requestContext,
      Guid userId,
      Guid collectionId,
      UserStatusChangeKind newStatus);

    void ProcessUserLicenseChange(
      IVssRequestContext requestContext,
      Guid userId,
      Guid collectionId,
      License newLicense);

    void ProcessUserExtensionChange(
      IVssRequestContext requestContext,
      Guid userId,
      Guid collectionId,
      string newExtension);
  }
}
