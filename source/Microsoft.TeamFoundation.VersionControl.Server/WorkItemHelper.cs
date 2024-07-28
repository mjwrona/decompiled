// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.WorkItemHelper
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.Azure.Boards.Linking;
using Microsoft.Azure.Devops.Work.RemoteServices;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class WorkItemHelper
  {
    private LinkFilter[] m_queryLinkFilter;
    private ArtifactId m_artifactId;
    private object m_lazyInitializationLock = new object();
    private int[] m_columnNames;
    private IVssRequestContext m_requestContext;
    private ITeamFoundationWorkItemService m_witService;

    public WorkItemHelper(IVssRequestContext requestContext)
    {
      this.m_requestContext = requestContext;
      this.m_witService = this.m_requestContext.GetService<ITeamFoundationWorkItemService>();
    }

    internal CheckinWorkItemInfo[] QueryWorkItemInformation(string csUri)
    {
      this.m_requestContext.Trace(700207, TraceLevel.Verbose, TraceArea.WorkItemHelper, TraceLayer.BusinessLogic, "Calling GetReferencingArtifacts({0}", (object) csUri);
      return this.QueryWorkItemInformation(new string[1]
      {
        csUri
      });
    }

    internal IEnumerable<int> QueryWorkItemIds(string[] csUris)
    {
      IWorkItemArtifactUriQueryRemotableService service = this.m_requestContext.GetService<IWorkItemArtifactUriQueryRemotableService>();
      ArtifactUriQuery artifactUriQuery1 = new ArtifactUriQuery()
      {
        ArtifactUris = (IEnumerable<string>) csUris
      };
      this.m_requestContext.Trace(700208, TraceLevel.Verbose, TraceArea.WorkItemHelper, TraceLayer.BusinessLogic, "Calling QueryWorkItemsForArtifactUris with csUri list, length of csUri array:{0}", (object) csUris.Length);
      IVssRequestContext requestContext = this.m_requestContext;
      ArtifactUriQuery artifactUriQuery2 = artifactUriQuery1;
      IEnumerable<int> source = service.QueryWorkItemsForArtifactUris(requestContext, artifactUriQuery2).ArtifactUrisQueryResult.Values.SelectMany<IEnumerable<WorkItemReference>, int>((Func<IEnumerable<WorkItemReference>, IEnumerable<int>>) (wie => wie.Select<WorkItemReference, int>((Func<WorkItemReference, int>) (wi => wi.Id))));
      this.m_requestContext.Trace(700209, TraceLevel.Verbose, TraceArea.WorkItemHelper, TraceLayer.BusinessLogic, "QueryWorkItemInformation - received {0} items", (object) source.Count<int>());
      return source;
    }

    internal CheckinWorkItemInfo[] QueryWorkItemInformation(string[] csUris) => this.QueryWorkItemInformation(this.QueryWorkItemIds(csUris).ToArray<int>());

    internal CheckinWorkItemInfo[] QueryWorkItemInformation(VersionControlLink[] links)
    {
      if (links == null)
        return (CheckinWorkItemInfo[]) null;
      List<int> intList1 = new List<int>(links.Length);
      List<int> intList2 = new List<int>(links.Length);
      foreach (VersionControlLink link in links)
      {
        if ((link.LinkType & 1024) == 1024)
        {
          ArtifactId artifactId = LinkingUtilities.DecodeUri(link.Url);
          intList1.Add(int.Parse(artifactId.ToolSpecificId, (IFormatProvider) CultureInfo.InvariantCulture));
          intList2.Add(link.LinkType);
        }
      }
      CheckinWorkItemInfo[] checkinWorkItemInfoArray = this.QueryWorkItemInformation(intList1.ToArray());
      for (int index = 0; index < intList1.Count; ++index)
        checkinWorkItemInfoArray[index].CheckinAction = (intList2[index] & 1025) != 1025 ? CheckinWorkItemCheckinAction.Associate : CheckinWorkItemCheckinAction.Resolve;
      return checkinWorkItemInfoArray;
    }

    internal CheckinWorkItemInfo[] QueryWorkItemInformation(int[] workItemIdList)
    {
      List<CheckinWorkItemInfo> checkinWorkItemInfoList = new List<CheckinWorkItemInfo>(workItemIdList.Length);
      try
      {
        IEnumerable<WorkItemFieldData> workItemFieldValues = this.m_witService.GetWorkItemFieldValues(this.m_requestContext, (IEnumerable<int>) workItemIdList, (IEnumerable<int>) this.Columns);
        ITswaServerHyperlinkService hyperlinkService = this.m_requestContext.GetService<ITswaServerHyperlinkService>();
        foreach (WorkItemFieldData workItemFieldData in workItemFieldValues)
        {
          CheckinWorkItemInfo checkinWorkItemInfo = new CheckinWorkItemInfo()
          {
            Id = workItemFieldData.Id,
            Type = workItemFieldData.WorkItemType,
            State = workItemFieldData.State,
            AssignedTo = workItemFieldData.AssignedTo,
            Title = workItemFieldData.Title
          };
          checkinWorkItemInfo.CheckinAction = !VssStringComparer.XmlAttributeName.Equals(checkinWorkItemInfo.State, "Closed") ? CheckinWorkItemCheckinAction.Associate : CheckinWorkItemCheckinAction.Resolve;
          checkinWorkItemInfo.WorkItemUrl = (string) null;
          if (hyperlinkService != null)
          {
            try
            {
              checkinWorkItemInfo.WorkItemUrl = hyperlinkService.GetWorkItemEditorUrl(this.m_requestContext, checkinWorkItemInfo.Id).ToString();
            }
            catch (InvalidOperationException ex)
            {
              this.m_requestContext.TraceException(700210, TraceLevel.Info, TraceArea.Linking, TraceLayer.BusinessLogic, (Exception) ex);
              hyperlinkService = (ITswaServerHyperlinkService) null;
            }
          }
          if (checkinWorkItemInfo.WorkItemUrl == null)
            checkinWorkItemInfo.WorkItemUrl = this.WorkItemUrl(checkinWorkItemInfo.Id.ToString((IFormatProvider) CultureInfo.InvariantCulture));
          checkinWorkItemInfoList.Add(checkinWorkItemInfo);
        }
      }
      catch (RequestCanceledException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        this.m_requestContext.TraceException(700211, TraceLevel.Info, TraceArea.WorkItemHelper, TraceLayer.BusinessLogic, ex);
        for (int index = 0; index < workItemIdList.Length; ++index)
        {
          CheckinWorkItemInfo checkinWorkItemInfo = new CheckinWorkItemInfo()
          {
            Id = workItemIdList[index]
          };
          checkinWorkItemInfo.WorkItemUrl = this.WorkItemUrl(checkinWorkItemInfo.Id.ToString((IFormatProvider) CultureInfo.InvariantCulture));
          checkinWorkItemInfoList.Add(checkinWorkItemInfo);
        }
      }
      return checkinWorkItemInfoList.ToArray();
    }

    private string WorkItemUrl(string Id)
    {
      ArtifactId artifactId = this.ArtifactId;
      artifactId.ToolSpecificId = Id;
      return this.m_requestContext.GetService<TeamFoundationLinkingService>().GetArtifactUrlExternal(this.m_requestContext, artifactId);
    }

    private LinkFilter[] QueryLinkFilter
    {
      get
      {
        if (this.m_queryLinkFilter == null)
          this.m_queryLinkFilter = new LinkFilter[1]
          {
            new LinkFilter()
            {
              FilterType = FilterType.ToolType,
              FilterValues = new string[1]
              {
                "WorkItemTracking"
              }
            }
          };
        return this.m_queryLinkFilter;
      }
    }

    private ArtifactId ArtifactId
    {
      get
      {
        if (this.m_artifactId == null)
        {
          this.m_artifactId = new ArtifactId();
          this.m_artifactId.ArtifactType = "WorkItem";
          this.m_artifactId.Tool = "WorkItemTracking";
        }
        return this.m_artifactId;
      }
    }

    internal int[] Columns
    {
      get
      {
        if (this.m_columnNames == null)
        {
          lock (this.m_lazyInitializationLock)
          {
            if (this.m_columnNames == null)
            {
              this.m_columnNames = new int[5];
              this.m_columnNames[0] = -3;
              this.m_columnNames[1] = 25;
              this.m_columnNames[2] = 2;
              this.m_columnNames[3] = 24;
              this.m_columnNames[4] = 1;
            }
          }
        }
        return this.m_columnNames;
      }
    }
  }
}
