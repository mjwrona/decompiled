// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.ExtendedPropertyTracepoints
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

namespace Microsoft.VisualStudio.Services.Graph
{
  internal class ExtendedPropertyTracepoints
  {
    internal const int ControllerWritePropertiesEnter = 15282000;
    internal const int ControllerWritePropertiesLeave = 15282001;
    internal const int ControllerReadPropertiesEnter = 15282002;
    internal const int ControllerReadPropertiesLeave = 15282003;
    internal const int ReadGlobalExtendedPropertyEnter = 15282004;
    internal const int ReadGlobalExtendedPropertyLeave = 15282005;
    internal const int ReadGlobalExtendedPropertiesEnter = 15282006;
    internal const int ReadGlobalExtendedPropertiesLeave = 15282007;
    internal const int BatchReadGlobalExtendedPropertiesEnter = 15282008;
    internal const int BatchReadGlobalExtendedPropertiesLeave = 15282009;
    internal const int WriteGlobalExtendedPropertyEnter = 15282010;
    internal const int WriteGlobalExtendedPropertyLeave = 15282011;
    internal const int WriteGlobalExtendedPropertiesEnter = 15282012;
    internal const int WriteGlobalExtendedPropertiesLeave = 15282013;
    internal const int BatchWriteGlobalExtendedPropertiesEnter = 15282014;
    internal const int BatchWriteGlobalExtendedPropertiesLeave = 15282015;
    internal const int HandlePropertyReadEnter = 15282016;
    internal const int HandlePropertyReadLeave = 15282017;
    internal const int ReadFromPropertyServiceEnter = 15282019;
    internal const int ReadFromPropertyServiceLeave = 15282020;
    internal const int WriteToPropertyServiceEnter = 15282021;
    internal const int WriteToPropertyServiceLeave = 15282022;
    internal const int ExplicitlyUnsupportedPropertiesRequested = 15282023;
    internal const int IdentityWithoutMasterId = 15282018;
    internal const int TracePropertiesRequestedFromPropertyService = 15282026;
    internal const int TracePropertiesReadFromPropertyService = 15282024;
    internal const int TracePropertiesWrittenToPropertyService = 15282025;
    internal const int DeleteServicePrincipalTraceEnter = 6307640;
    internal const int DeleteServicePrincipalTraceLeave = 6307649;
    internal const int ListServicePrincipalTraceEnter = 6307660;
    internal const int ListServicePrincipalTraceLeave = 6307669;
    internal const int ListMemberUserIPagedScopedIdentityReader = 6307661;
    internal const int GetServicePrincipalTraceEnter = 6307670;
    internal const int GetServicePrincipalTraceLeave = 6307679;
    internal const int ListMemberTraceEnter = 6307680;
    internal const int ListMemberTraceLeave = 6307689;
  }
}
