// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.CheckInGatedBuildException
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.Azure.Boards.Linking;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public class CheckInGatedBuildException : TeamFoundationServerException
  {
    protected Dictionary<string, object> m_propertyDictionary;

    public CheckInGatedBuildException(
      IVssRequestContext requestContext,
      ActionDeniedBySubscriberException ex)
      : base(string.Empty, (Exception) ex)
    {
      this.m_propertyDictionary = ex.PropertyCollection.ToDictionary<KeyValuePair<string, object>, string, object>((Func<KeyValuePair<string, object>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, object>, object>) (kvp => kvp.Value));
      this.AffectedBuildDefinitionUris = (string[]) this.m_propertyDictionary[nameof (AffectedBuildDefinitionUris)];
      this.AffectedBuildDefinitionNames = (string[]) this.m_propertyDictionary[nameof (AffectedBuildDefinitionNames)];
      this.ShelvesetName = (string) this.m_propertyDictionary[nameof (ShelvesetName)];
      if (!this.m_propertyDictionary.ContainsKey("QueueId"))
        return;
      this.BuildUri = requestContext.GetService<ITeamFoundationLinkingService>().GetArtifactUrlExternal(requestContext, (string) this.m_propertyDictionary[nameof (BuildUri)]);
      this.BuildId = new int?((int) this.m_propertyDictionary["QueueId"]);
    }

    public string[] AffectedBuildDefinitionUris { get; }

    public string[] AffectedBuildDefinitionNames { get; }

    public string ShelvesetName { get; }

    public int? BuildId { get; }

    public string BuildUri { get; }
  }
}
