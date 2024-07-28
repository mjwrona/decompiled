// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.IdentityPicker.Extensions.IIdentityPickerExtension
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Services.IdentityPicker.Extensions
{
  [InheritedExport]
  public interface IIdentityPickerExtension
  {
    OperationResponse Invoke(IVssRequestContext requestContext, OperationRequest request);

    Type GetExtendedOperationType();

    Guid GetExtensionId();

    bool EvaluateApplicability(IVssRequestContext context, OperationRequest request);
  }
}
