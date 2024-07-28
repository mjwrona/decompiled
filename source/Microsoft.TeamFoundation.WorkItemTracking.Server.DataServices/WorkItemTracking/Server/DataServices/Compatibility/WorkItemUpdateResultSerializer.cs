// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.Compatibility.WorkItemUpdateResultSerializer
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 929F0284-16B2-4277-9F4A-B615689A77D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.Compatibility
{
  public class WorkItemUpdateResultSerializer
  {
    public readonly int ClientVersion;
    protected readonly bool isBulk;

    public WorkItemUpdateResultSerializer(int clientVersion, bool isBulk = false)
    {
      this.ClientVersion = clientVersion;
      this.isBulk = isBulk;
    }

    public XElement Serialize(
      IVssRequestContext requestContext,
      WorkItemUpdateDeserializeResult deserializedPackage,
      IEnumerable<WorkItemUpdateResult> results)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<WorkItemUpdateDeserializeResult>(deserializedPackage, nameof (deserializedPackage));
      ArgumentUtility.CheckForNull<IEnumerable<WorkItemUpdateResult>>(results, nameof (results));
      if (!deserializedPackage.HasUpdates)
        throw new ArgumentException("The deserialized package has no updates");
      if (!results.Any<WorkItemUpdateResult>())
        throw new ArgumentException("No results contained in the update results from the server");
      WorkItemUpdateWrapperOrderedResultCollection source1 = new WorkItemUpdateWrapperOrderedResultCollection(requestContext, results, deserializedPackage);
      IEnumerable<KeyValuePair<int, IEnumerable<Tuple<int, string>>>> source2 = source1.Select<KeyValuePair<WorkItemUpdateWrapper, WorkItemUpdateResult>, KeyValuePair<int, IEnumerable<Tuple<int, string>>>>((Func<KeyValuePair<WorkItemUpdateWrapper, WorkItemUpdateResult>, int, KeyValuePair<int, IEnumerable<Tuple<int, string>>>>) ((wrapperResultPair, i) => new KeyValuePair<int, IEnumerable<Tuple<int, string>>>(i, this.GetErrorsFromUpdate(wrapperResultPair)))).Where<KeyValuePair<int, IEnumerable<Tuple<int, string>>>>((Func<KeyValuePair<int, IEnumerable<Tuple<int, string>>>, bool>) (kvp => kvp.Value.Any<Tuple<int, string>>()));
      bool hasErrors = source2.Any<KeyValuePair<int, IEnumerable<Tuple<int, string>>>>();
      IFieldTypeDictionary fieldTypeDictionary = (IFieldTypeDictionary) null;
      if (!hasErrors)
      {
        fieldTypeDictionary = requestContext.GetService<WorkItemTrackingFieldService>().GetFieldsSnapshot(requestContext);
        if (fieldTypeDictionary == null)
          throw new ArgumentException("The field type dictionary can not be located");
      }
      XElement xelement = new XElement((XNamespace) "" + "UpdateResults");
      List<KeyValuePair<string, XElement>> source3 = new List<KeyValuePair<string, XElement>>();
      Lazy<HashSet<Tuple<int, int, int>>> lazy = new Lazy<HashSet<Tuple<int, int, int>>>();
      foreach (KeyValuePair<WorkItemUpdateWrapper, WorkItemUpdateResult> keyValuePair in source1)
      {
        if (keyValuePair.Key.IsWorkItem)
        {
          XElement workItemElement = this.CreateWorkItemElement(keyValuePair.Value, keyValuePair.Key, fieldTypeDictionary, hasErrors);
          source3.Add(new KeyValuePair<string, XElement>(keyValuePair.Key.CorrelationId, workItemElement));
          if (!hasErrors && keyValuePair.Key.HasResourceLinkUpdates)
          {
            foreach (WorkItemResourceLinkUpdateResult result in (IEnumerable<WorkItemResourceLinkUpdateResult>) keyValuePair.Value.ResourceLinkUpdates.OrderBy<WorkItemResourceLinkUpdateResult, string>((Func<WorkItemResourceLinkUpdateResult, string>) (x => x.CorrelationId)))
              workItemElement.Add((object) this.CreateResourceLinkElement(result, keyValuePair.Key, fieldTypeDictionary));
          }
        }
        if (keyValuePair.Key.HasLinkUpdates)
        {
          if (hasErrors)
          {
            foreach (WorkItemLinkUpdate linkUpdate in keyValuePair.Key.LinkUpdates)
              source3.Add(new KeyValuePair<string, XElement>(linkUpdate.CorrelationId, this.CreateLinkUpdateElement(linkUpdate)));
          }
          else
          {
            foreach (WorkItemLinkUpdateResult linkUpdateResult in (IEnumerable<WorkItemLinkUpdateResult>) keyValuePair.Value.LinkUpdates.OrderBy<WorkItemLinkUpdateResult, string>((Func<WorkItemLinkUpdateResult, string>) (x => x.CorrelationId)))
            {
              Tuple<int, int, int> key = linkUpdateResult.ToKey();
              if (source1.ContainsLink(key) && lazy.Value.Add(key))
                source3.Add(new KeyValuePair<string, XElement>(linkUpdateResult.CorrelationId, this.CreateLinkUpdateElement(linkUpdateResult)));
            }
          }
        }
      }
      foreach (KeyValuePair<string, XElement> keyValuePair in (IEnumerable<KeyValuePair<string, XElement>>) source3.OrderBy<KeyValuePair<string, XElement>, string>((Func<KeyValuePair<string, XElement>, string>) (x => x.Key)))
        xelement.Add((object) keyValuePair.Value);
      foreach (KeyValuePair<int, IEnumerable<Tuple<int, string>>> keyValuePair in source2)
      {
        foreach (Tuple<int, string> tuple in keyValuePair.Value)
        {
          XElement content = new XElement((XName) "FailedWorkItem");
          content.SetAttributeValue((XName) "BatchIndex", (object) keyValuePair.Key);
          content.SetAttributeValue((XName) "ErrorCode", (object) tuple.Item1);
          content.SetAttributeValue((XName) "ErrorMessage", (object) tuple.Item2);
          xelement.Add((object) content);
        }
      }
      return xelement;
    }

    private IEnumerable<Tuple<int, string>> GetErrorsFromUpdate(
      KeyValuePair<WorkItemUpdateWrapper, WorkItemUpdateResult> wrapperResultPair)
    {
      return this.GetErrorsFromException(wrapperResultPair.Value.Exception, wrapperResultPair.Key);
    }

    private IEnumerable<Tuple<int, string>> GetErrorsFromException(
      TeamFoundationServiceException exception,
      WorkItemUpdateWrapper workItem)
    {
      if (workItem != null && !workItem.IsWorkItem)
      {
        WorkItemLinkUpdate workItemLinkUpdate = workItem.LinkUpdates.First<WorkItemLinkUpdate>();
        if (exception is WorkItemUnauthorizedAccessException)
          exception = (TeamFoundationServiceException) new WorkItemLinkEndUnauthorizedAccessException(workItemLinkUpdate.LinkType, workItemLinkUpdate.SourceWorkItemId, workItemLinkUpdate.TargetWorkItemId, new WorkItemUnauthorizedAccessException(0, AccessType.Read), (WorkItemUnauthorizedAccessException) null);
      }
      if (exception == null || exception is WorkItemsBatchSaveFailedException || exception is LegacyBatchSaveException)
        return Enumerable.Empty<Tuple<int, string>>();
      if (exception is WorkItemLinkInvalidException || exception is WorkItemLinkNotFoundException || exception is LegacyWorkItemLinkException || exception is WorkItemLinkUnauthorizedAccessException || exception is WorkItemLinkEndUnauthorizedAccessException)
        throw exception;
      if (!this.isBulk)
        throw exception;
      if (exception is WorkItemTrackingAggregateException aggregateException)
        return aggregateException.AllExceptions.SelectMany<TeamFoundationServiceException, Tuple<int, string>>((Func<TeamFoundationServiceException, IEnumerable<Tuple<int, string>>>) (e => this.GetErrorsFromException(e, workItem)));
      return (IEnumerable<Tuple<int, string>>) new List<Tuple<int, string>>()
      {
        Tuple.Create<int, string>(exception.ErrorCode, exception.Message)
      };
    }

    protected virtual XElement CreateWorkItemElement(
      WorkItemUpdateResult result,
      WorkItemUpdateWrapper wrapper,
      IFieldTypeDictionary fieldTypeDictionary,
      bool hasErrors)
    {
      if (wrapper == null)
        throw new ArgumentNullException(nameof (wrapper));
      if (!hasErrors && result == null)
        throw new ArgumentNullException(nameof (result));
      if (!hasErrors && fieldTypeDictionary == null)
        throw new ArgumentNullException(nameof (fieldTypeDictionary));
      return result.ToElement(wrapper, fieldTypeDictionary, hasErrors);
    }

    protected virtual XElement CreateLinkUpdateElement(WorkItemLinkUpdateResult result) => result != null ? result.ToElement() : throw new ArgumentNullException(nameof (result));

    protected virtual XElement CreateLinkUpdateElement(WorkItemLinkUpdate result) => result != null ? result.ToElement() : throw new ArgumentNullException(nameof (result));

    protected virtual XElement CreateResourceLinkElement(
      WorkItemResourceLinkUpdateResult result,
      WorkItemUpdateWrapper wrapper,
      IFieldTypeDictionary fieldTypeDictionary)
    {
      if (result == null)
        throw new ArgumentNullException(nameof (result));
      if (wrapper == null)
        throw new ArgumentNullException(nameof (wrapper));
      if (fieldTypeDictionary == null)
        throw new ArgumentNullException(nameof (fieldTypeDictionary));
      return result.ToElement(wrapper, fieldTypeDictionary);
    }
  }
}
