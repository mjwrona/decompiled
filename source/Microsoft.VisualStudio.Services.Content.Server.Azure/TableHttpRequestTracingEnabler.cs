// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.TableHttpRequestTracingEnabler
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public static class TableHttpRequestTracingEnabler
  {
    private const string RegistryPath = "/Configuration/ArtifactServices/TraceTableCallsToHttpOutgoingRequests";

    public static bool TraceTableCallsToHttpOutgoingRequests { get; set; }

    public static void TrackEnabledStateInRegistry(IVssRequestContext requestContext)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      service.RegisterNotification(requestContext, TableHttpRequestTracingEnabler.\u003C\u003EO.\u003C0\u003E__OnRegistrySettingsChanged ?? (TableHttpRequestTracingEnabler.\u003C\u003EO.\u003C0\u003E__OnRegistrySettingsChanged = new RegistrySettingsChangedCallback(TableHttpRequestTracingEnabler.OnRegistrySettingsChanged)), false, "/Configuration/ArtifactServices/TraceTableCallsToHttpOutgoingRequests");
      TableHttpRequestTracingEnabler.OnRegistrySettingsChanged(requestContext, service.ReadEntries(requestContext, (RegistryQuery) "/Configuration/ArtifactServices/TraceTableCallsToHttpOutgoingRequests"));
    }

    private static void OnRegistrySettingsChanged(
      IVssRequestContext context,
      RegistryEntryCollection entries)
    {
      TableHttpRequestTracingEnabler.TraceTableCallsToHttpOutgoingRequests = entries.GetValueFromPath<bool>("/Configuration/ArtifactServices/TraceTableCallsToHttpOutgoingRequests", false);
    }
  }
}
