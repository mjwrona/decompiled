// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Settings.ISettingsScopePlugin
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Services.Settings
{
  [InheritedExport]
  public interface ISettingsScopePlugin
  {
    string Name { get; }

    string GetRequestScopeValue(IVssRequestContext requestContext);

    bool IsValidScopeValue(IVssRequestContext requestContext, string scopeValue);

    void CheckReadPermission(IVssRequestContext requestContext, string scopeValue);

    void CheckWritePermission(IVssRequestContext requestContext, string scopeValue);

    bool HasReadPermission(IVssRequestContext requestContext, string scopeValue);

    bool HasWritePermission(IVssRequestContext requestContext, string scopeValue);

    bool IsScopeDirty(IVssRequestContext requestContext, string scopeValue);
  }
}
