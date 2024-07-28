// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.ITeamProjectCollectionServicing
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public interface ITeamProjectCollectionServicing
  {
    TeamProjectCollection WaitForCollectionServicingToComplete(ServicingJobDetail jobDetail);

    TeamProjectCollection WaitForCollectionServicingToComplete(
      ServicingJobDetail jobDetail,
      TimeSpan timeout);

    ServicingJobDetail QueueCreateCollection(
      string name,
      string description,
      bool isDefault,
      string virtualDirectory,
      TeamFoundationServiceHostStatus state,
      IDictionary<string, string> servicingTokens);

    ServicingJobDetail QueueCreateCollection(
      string name,
      string description,
      bool isDefault,
      string virtualDirectory,
      TeamFoundationServiceHostStatus state,
      IDictionary<string, string> servicingTokens,
      string dataTierConnectionString,
      string defaultConnectionString,
      IDictionary<string, string> databaseCategoryConnectionStrings);

    ServicingJobDetail QueueAttachCollection(
      string databaseConnectionString,
      IDictionary<string, string> servicingTokens,
      bool cloneCollection);

    ServicingJobDetail QueueAttachCollection(
      string databaseConnectionString,
      IDictionary<string, string> servicingTokens,
      bool cloneCollection,
      string name,
      string description,
      string virtualDirectory);

    ServicingJobDetail QueueDetachCollection(
      Guid collectionId,
      IDictionary<string, string> servicingTokens,
      string collectionStoppedMessage,
      out string detachedConnectionString);

    ServicingJobDetail QueueDetachCollection(
      TeamProjectCollection teamProjectCollection,
      IDictionary<string, string> servicingTokens,
      string collectionStoppedMessage,
      out string detachedConnectionString);

    ServicingJobDetail DeleteProject(
      Guid collectionId,
      string projectUri,
      Dictionary<string, string> servicingTokens);

    ServicingJobDetail QueueDeleteProject(
      Guid collectionId,
      string projectUri,
      IDictionary<string, string> servicingTokens);
  }
}
