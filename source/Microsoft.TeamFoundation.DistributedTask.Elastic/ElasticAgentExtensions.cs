// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Elastic.ElasticAgentExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Elastic, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6202E83A-3164-4101-8FDA-8C4FB25E62EC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Elastic.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;

namespace Microsoft.TeamFoundation.DistributedTask.Elastic
{
  public static class ElasticAgentExtensions
  {
    public static bool IsEnabled(this ElasticAgentState state) => (state & ElasticAgentState.Enabled) == ElasticAgentState.Enabled;

    public static bool IsDisabled(this ElasticAgentState state) => !state.IsEnabled();

    public static bool IsOnline(this ElasticAgentState state) => (state & ElasticAgentState.Online) == ElasticAgentState.Online;

    public static bool IsOffline(this ElasticAgentState state) => !state.IsOnline();

    public static bool IsAssigned(this ElasticAgentState state) => (state & ElasticAgentState.Assigned) == ElasticAgentState.Assigned;

    public static bool IsUnassigned(this ElasticAgentState state) => !state.IsUnassigned();
  }
}
