// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.IBuildSourceProviderExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public static class IBuildSourceProviderExtensions
  {
    private const string c_layer = "IBuildSourceProviderExtensions";

    public static bool SupportsRepositoryType(
      this IBuildSourceProvider sourceProvider,
      IVssRequestContext requestContext,
      BuildDefinition definition)
    {
      return sourceProvider.SupportsRepositoryType(requestContext, definition?.Repository);
    }

    public static bool SupportsRepositoryType(
      this IBuildSourceProvider sourceProvider,
      IVssRequestContext requestContext,
      BuildRepository repository)
    {
      return repository != null && repository.Type != null && string.Equals(repository.Type, sourceProvider.GetAttributes(requestContext).Name, StringComparison.OrdinalIgnoreCase);
    }

    internal static bool SupportsTriggerType(
      this IBuildSourceProvider sourceProvider,
      IVssRequestContext requestContext,
      DefinitionTriggerType triggerType)
    {
      return sourceProvider != null && (triggerType & sourceProvider.GetAttributes(requestContext).SupportedTriggerTypes) == triggerType;
    }

    internal static bool TryCalculateChangesWithValidation(
      this IBuildSourceProvider sourceProvider,
      IVssRequestContext requestContext,
      BuildData build,
      BuildDefinition definition,
      int maxChanges,
      out List<Change> changes)
    {
      bool changesWithValidation;
      if (build.IsTriggeredByResourceRepository(requestContext, definition))
      {
        changes = build.GetChangesFromResourceRepository(requestContext, definition);
        changesWithValidation = true;
      }
      else
        changesWithValidation = sourceProvider.TryCalculateChanges(requestContext, build, definition, maxChanges, out changes);
      if (changesWithValidation && changes.Any<Change>() && build.Reason != BuildReason.PullRequest)
      {
        int changeIndex = SourceVersionHelper.FindChangeIndex(requestContext, sourceProvider, changes, build.SourceVersion);
        if (changeIndex != 0)
          requestContext.TraceError(12030234, nameof (IBuildSourceProviderExtensions), "TryCalculateChanges didn't return the source version change '{0}' at index 0. Found index: {1}, Changes returned: {2}, RepositoryType: {3}.", (object) build.SourceVersion, (object) changeIndex, (object) changes.Count, (object) build.Repository?.Type);
      }
      return changesWithValidation;
    }

    internal static IReadOnlyList<ChangeData> GetChangeData(
      this IBuildSourceProvider sourceProvider,
      IVssRequestContext requestContext,
      IReadOnlyList<Change> changes,
      Lazy<Change> sourceChange)
    {
      IEnumerable<Change> source = changes.Where<Change>((Func<Change, bool>) (c => !string.IsNullOrEmpty(c.Id)));
      List<ChangeData> changeData1 = sourceProvider.GetAttributes(requestContext).IsExternal ? source.Select<Change, ChangeData>((Func<Change, ChangeData>) (c => new ChangeData()
      {
        SourceChangeOnly = false,
        Descriptor = c.Id,
        ExternalData = c.Serialize<Change>()
      })).ToList<ChangeData>() : source.Select<Change, ChangeData>((Func<Change, ChangeData>) (c => new ChangeData()
      {
        SourceChangeOnly = false,
        Descriptor = c.Id
      })).ToList<ChangeData>();
      if (changeData1.Count < changes.Count)
        requestContext.TraceError(nameof (IBuildSourceProviderExtensions), "Some changes do not have an Id. Changes without ids will not be cached, and will not be returned by subsequent calls to GetChanges.");
      if (changeData1.Count == 0 && sourceChange?.Value != null)
      {
        if (!string.IsNullOrEmpty(sourceChange.Value.Id))
        {
          ChangeData changeData2;
          if (!sourceProvider.GetAttributes(requestContext).IsExternal)
          {
            changeData2 = new ChangeData()
            {
              SourceChangeOnly = true,
              Descriptor = sourceChange.Value.Id
            };
          }
          else
          {
            changeData2 = new ChangeData();
            changeData2.SourceChangeOnly = true;
            changeData2.Descriptor = sourceChange.Value.Id;
            changeData2.ExternalData = sourceChange.Value.Serialize<Change>();
          }
          ChangeData changeData3 = changeData2;
          changeData1.Add(changeData3);
        }
        else
          requestContext.TraceError(nameof (IBuildSourceProviderExtensions), "SourceChange doesn't have an Id. SourceChange will not be persisted, and will have be retrieved from the source provider every time.");
      }
      return (IReadOnlyList<ChangeData>) changeData1;
    }

    internal static bool TryDeserializeChanges(
      this IBuildSourceProvider sourceProvider,
      IVssRequestContext requestContext,
      IEnumerable<ChangeData> changeData,
      out IEnumerable<Change> changes)
    {
      if (sourceProvider.GetAttributes(requestContext).IsExternal && changeData.All<ChangeData>((Func<ChangeData, bool>) (c => !string.IsNullOrEmpty(c.ExternalData))))
      {
        changes = changeData.Select<ChangeData, Change>((Func<ChangeData, Change>) (c => JsonUtilities.Deserialize<Change>(c.ExternalData)));
        return true;
      }
      changes = (IEnumerable<Change>) null;
      return false;
    }
  }
}
