// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Compliance.SecureFlowLocationExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.VisualStudio.Services.Compliance
{
  internal static class SecureFlowLocationExtensions
  {
    private const string CompactMode = "1";
    private const string CompactParameter = "compact";
    private const string ModeParameter = "mode";
    private const string SourceParameter = "source";

    public static SecureFlowLocation ForContext(
      this SecureFlowLocation location,
      IVssRequestContext requestContext,
      bool overrideAuthority)
    {
      if (location == null)
        return (SecureFlowLocation) null;
      SecureFlowLocation location1 = location;
      if (overrideAuthority)
      {
        location1 = location.Clone();
        SecureFlowLocation.SetHostToDefault(location1, requestContext);
      }
      if (location1.NextLocation != null)
      {
        string realm = location1.Realm;
        if (realm != null && !VssStringComparer.DomainName.Equals(realm, location1.NextLocation.HostName))
          location1.Realm = (string) null;
      }
      return location1;
    }

    public static SecureFlowLocation ForFlow(
      this SecureFlowLocation location,
      SecureFlowLocation sourceLocation)
    {
      if (location == null || sourceLocation == null)
        return location;
      SecureFlowLocation secureFlowLocation = location.Clone();
      if (sourceLocation.Parameters["compact"] == "1" || sourceLocation.Parameters["protocol"] == "javascriptnotify")
      {
        secureFlowLocation.Parameters["compact"] = "1";
        if (!string.IsNullOrEmpty(sourceLocation.Parameters["mode"]))
          secureFlowLocation.Parameters["mode"] = sourceLocation.Parameters["mode"];
      }
      if (!string.IsNullOrEmpty(sourceLocation.Parameters["source"]))
        secureFlowLocation.Parameters["source"] = sourceLocation.Parameters["source"];
      return secureFlowLocation;
    }
  }
}
