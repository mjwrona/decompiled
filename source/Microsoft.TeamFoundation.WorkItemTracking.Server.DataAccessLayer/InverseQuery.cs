// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.InverseQuery
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class InverseQuery : ILinkingProvider, ILinkingConsumer
  {
    private IVssRequestContext m_requestContext;
    private IVssIdentity m_user;
    private ArtifactId m_artifactId;

    public InverseQuery(IVssRequestContext requestContext, IVssIdentity user)
    {
      requestContext.TraceEnter(900198, "DataAccessLayer", nameof (InverseQuery), nameof (InverseQuery));
      this.m_requestContext = requestContext;
      this.m_user = user;
      this.m_artifactId = new ArtifactId();
      this.m_artifactId.ArtifactType = "Workitem";
      this.m_artifactId.Tool = "WorkItemTracking";
      this.m_artifactId.ToolSpecificId = "";
      this.m_artifactId.VisualStudioServerNamespace = "VSTF";
      requestContext.TraceLeave(900559, "DataAccessLayer", nameof (InverseQuery), nameof (InverseQuery));
    }

    public Artifact[] GetArtifacts(string[] ArtifactUriList)
    {
      this.m_requestContext.TraceEnter(900199, "DataAccessLayer", nameof (InverseQuery), nameof (GetArtifacts));
      List<Artifact> artifactList = new List<Artifact>();
      if (ArtifactUriList == null || ArtifactUriList.Length == 0)
        return artifactList.ToArray();
      foreach (string artifactUri in ArtifactUriList)
      {
        this.m_requestContext.Trace(900504, TraceLevel.Verbose, "DataAccessLayer", nameof (InverseQuery), "DAL InverseQuery:ArtifactURI" + artifactUri);
        Artifact artifact = (Artifact) null;
        ArtifactId uriData;
        try
        {
          uriData = this.ExtractUriData(artifactUri);
        }
        catch (ArgumentException ex)
        {
          artifactList.Add(artifact);
          continue;
        }
        if (this.string2ArtifactType(uriData.ArtifactType) == CurrituckArtifactType.Workitem)
        {
          Payload payload;
          try
          {
            payload = this.FetchWorkItemData(uriData.ToolSpecificId);
          }
          catch (LegacyServerException ex)
          {
            artifactList.Add(artifact);
            TeamFoundationEventLog.Default.LogException(this.m_requestContext, ex.Message, (Exception) ex, TeamFoundationEventId.WitBaseEventId, EventLogEntryType.Error);
            continue;
          }
          if (payload.Tables.Count == 2)
          {
            PayloadTable table = payload.Tables[0];
            if (table.Rows.Count > 0)
            {
              artifact = new Artifact();
              artifact.Uri = artifactUri;
              artifact.ExternalId = uriData.ToolSpecificId;
              PayloadTable.PayloadRow row1 = table.Rows[0];
              this.UpdateArtifactWithWorkItemData(artifact, row1);
              List<OutboundLink> outboundLinkList = new List<OutboundLink>();
              foreach (PayloadTable.PayloadRow row2 in payload.Tables[1].Rows)
                outboundLinkList.Add(this.GetOutboundLinkWithLinkData(row2));
              artifact.OutboundLinks = outboundLinkList.ToArray();
            }
          }
        }
        artifactList.Add(artifact);
      }
      this.m_requestContext.TraceLeave(900560, "DataAccessLayer", nameof (InverseQuery), nameof (GetArtifacts));
      return artifactList.ToArray();
    }

    public Artifact[] GetReferencingArtifacts(string[] ArtifactUriList)
    {
      List<Artifact> artifactList = new List<Artifact>();
      this.m_requestContext.TraceEnter(900200, "DataAccessLayer", nameof (InverseQuery), nameof (GetReferencingArtifacts));
      if (ArtifactUriList != null)
      {
        if (ArtifactUriList.Length != 0)
        {
          try
          {
            foreach (string artifactUri in ArtifactUriList)
            {
              Artifact artifact1 = new Artifact();
              PayloadTable payloadTable = (PayloadTable) null;
              ArtifactId uriData = this.ExtractUriData(artifactUri);
              artifact1.Uri = artifactUri;
              artifact1.ExternalId = uriData.ToolSpecificId;
              this.m_requestContext.Trace(900505, TraceLevel.Verbose, "DataAccessLayer", nameof (InverseQuery), "DAL InverseQuery:ArtifactURI" + artifactUri);
              switch (this.string2ArtifactType(uriData.ArtifactType))
              {
                case CurrituckArtifactType.Workitem:
                  payloadTable = this.FetchWorkItemDataByReferenceUri(uriData.ToolSpecificId, CurrituckArtifactType.Workitem);
                  break;
                case CurrituckArtifactType.Hyperlink:
                  payloadTable = this.FetchWorkItemDataByReferenceUri(uriData.ToolSpecificId, CurrituckArtifactType.Hyperlink);
                  break;
                case CurrituckArtifactType.External:
                  payloadTable = this.FetchWorkItemDataByReferenceUri(artifactUri, CurrituckArtifactType.External);
                  break;
              }
              if (payloadTable != null)
              {
                foreach (PayloadTable.PayloadRow row in payloadTable.Rows)
                {
                  Artifact artifact2 = new Artifact();
                  this.UpdateArtifactWithWorkItemData(artifact2, row);
                  artifactList.Add(artifact2);
                }
              }
            }
          }
          catch (LegacyServerException ex)
          {
            if (ex.ErrorId != 600019)
              throw;
          }
        }
      }
      this.m_requestContext.TraceLeave(900561, "DataAccessLayer", nameof (InverseQuery), nameof (GetReferencingArtifacts));
      return artifactList.ToArray();
    }

    public Artifact[] GetReferencingArtifacts(string[] ArtifactUriList, LinkFilter[] linkFilterList)
    {
      if (linkFilterList == null || linkFilterList.Length == 0)
        return this.GetReferencingArtifacts(ArtifactUriList);
      List<Artifact> artifactList = new List<Artifact>();
      this.m_requestContext.TraceEnter(900201, "DataAccessLayer", nameof (InverseQuery), "GetReferencingArtifacts(workItemUri, LinkFilter)");
      if (ArtifactUriList != null)
      {
        if (ArtifactUriList.Length != 0)
        {
          try
          {
            foreach (string artifactUri in ArtifactUriList)
            {
              Artifact artifact1 = new Artifact();
              PayloadTable payloadTable = (PayloadTable) null;
              ArtifactId uriData = this.ExtractUriData(artifactUri);
              string str = artifactUri;
              artifact1.Uri = str;
              CurrituckArtifactType currituckArtifactType = this.string2ArtifactType(uriData.ArtifactType);
              this.m_requestContext.Trace(900506, TraceLevel.Verbose, "DataAccessLayer", nameof (InverseQuery), "ArtifactURI: {0}", (object) artifactUri);
              foreach (LinkFilter linkFilter in linkFilterList)
              {
                if (linkFilter.FilterType == FilterType.LinkType)
                {
                  foreach (string filterValue in linkFilter.FilterValues)
                  {
                    switch (currituckArtifactType)
                    {
                      case CurrituckArtifactType.Workitem:
                        payloadTable = this.FetchWorkItemDataByLinkFilter(uriData.ToolSpecificId, CurrituckArtifactType.Workitem, filterValue);
                        break;
                      case CurrituckArtifactType.Hyperlink:
                        payloadTable = this.FetchWorkItemDataByLinkFilter(uriData.ToolSpecificId, CurrituckArtifactType.Hyperlink, filterValue);
                        break;
                      case CurrituckArtifactType.External:
                        payloadTable = this.FetchWorkItemDataByLinkFilter(artifactUri, CurrituckArtifactType.External, filterValue);
                        break;
                    }
                  }
                }
                else if (linkFilter.FilterType == FilterType.ToolType)
                {
                  foreach (string filterValue in linkFilter.FilterValues)
                  {
                    if (filterValue.Equals("WorkItemTracking", StringComparison.OrdinalIgnoreCase))
                    {
                      switch (currituckArtifactType)
                      {
                        case CurrituckArtifactType.Workitem:
                          payloadTable = this.FetchWorkItemDataByReferenceUri(uriData.ToolSpecificId, CurrituckArtifactType.Workitem);
                          continue;
                        case CurrituckArtifactType.Hyperlink:
                          payloadTable = this.FetchWorkItemDataByReferenceUri(uriData.ToolSpecificId, CurrituckArtifactType.Hyperlink);
                          continue;
                        case CurrituckArtifactType.External:
                          payloadTable = this.FetchWorkItemDataByReferenceUri(artifactUri, CurrituckArtifactType.External);
                          continue;
                        default:
                          continue;
                      }
                    }
                  }
                }
              }
              if (payloadTable != null)
              {
                foreach (PayloadTable.PayloadRow row in payloadTable.Rows)
                {
                  Artifact artifact2 = new Artifact();
                  this.UpdateArtifactWithWorkItemData(artifact2, row);
                  artifactList.Add(artifact2);
                }
              }
            }
          }
          catch (LegacyServerException ex)
          {
            if (ex.ErrorId != 600019)
              throw;
          }
        }
      }
      this.m_requestContext.TraceLeave(900562, "DataAccessLayer", nameof (InverseQuery), "GetReferencingArtifacts(workItemUri, LinkFilter)");
      return artifactList.ToArray();
    }

    public string[] GetReferencingWorkitemUris(string artifactUri)
    {
      List<string> stringList = new List<string>();
      this.m_requestContext.TraceEnter(900202, "DataAccessLayer", nameof (InverseQuery), nameof (GetReferencingWorkitemUris));
      if (artifactUri != null)
      {
        if (artifactUri.Trim().Length != 0)
        {
          try
          {
            PayloadTable payloadTable = (PayloadTable) null;
            ArtifactId uriData = this.ExtractUriData(artifactUri);
            switch (this.string2ArtifactType(uriData.ArtifactType))
            {
              case CurrituckArtifactType.Workitem:
                payloadTable = this.FetchWorkItemDataByReferenceUri(uriData.ToolSpecificId, CurrituckArtifactType.Workitem);
                break;
              case CurrituckArtifactType.Hyperlink:
                payloadTable = this.FetchWorkItemDataByReferenceUri(uriData.ToolSpecificId, CurrituckArtifactType.Hyperlink);
                break;
              case CurrituckArtifactType.External:
                payloadTable = this.FetchWorkItemDataByReferenceUri(artifactUri, CurrituckArtifactType.External);
                break;
            }
            if (payloadTable != null)
            {
              foreach (PayloadTable.PayloadRow row in payloadTable.Rows)
                stringList.Add(row["System.Id"].ToString());
            }
          }
          catch (LegacyServerException ex)
          {
            if (ex.ErrorId != 600019)
              throw;
          }
        }
      }
      this.m_requestContext.TraceLeave(900563, "DataAccessLayer", nameof (InverseQuery), nameof (GetReferencingWorkitemUris));
      return stringList.ToArray();
    }

    internal ArtifactId ExtractUriData(string BisUri)
    {
      this.m_requestContext.Trace(900203, TraceLevel.Verbose, "DataAccessLayer", nameof (InverseQuery), "ExtractUriData{0}", (object) BisUri);
      return LinkingUtilities.DecodeUri(BisUri);
    }

    internal CurrituckArtifactType string2ArtifactType(string artifactType)
    {
      this.m_requestContext.Trace(900204, TraceLevel.Verbose, "DataAccessLayer", nameof (InverseQuery), "-->DAL InverseQuery:string2ArtifactType");
      if (TFStringComparer.WorkItemArtifactType.Equals(artifactType, "Workitem"))
        return CurrituckArtifactType.Workitem;
      if (TFStringComparer.WorkItemArtifactType.Equals(artifactType, "Hyperlink"))
        return CurrituckArtifactType.Hyperlink;
      this.m_requestContext.Trace(900507, TraceLevel.Verbose, "DataAccessLayer", nameof (InverseQuery), "-->DAL InverseQuery:External Artifact Type - " + artifactType);
      return CurrituckArtifactType.External;
    }

    internal Payload FetchWorkItemData(string toolSpecificId)
    {
      this.m_requestContext.Trace(900205, TraceLevel.Verbose, "DataAccessLayer", nameof (InverseQuery), "-->DAL InverseQuery:FetchworkItemData()");
      int result;
      if (!int.TryParse(toolSpecificId, out result))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, DalResourceStrings.Get("InvalidWorkItemId"), (object) toolSpecificId));
      DalGetWorkItemDetailsElement element;
      using (SqlBatchBuilder sqlBatch = (SqlBatchBuilder) new WorkItemBatchBuilder(this.m_requestContext))
      {
        element = DalSqlElement.GetElement<DalGetWorkItemDetailsElement>(sqlBatch);
        element.JoinBatch(result, this.m_user);
        sqlBatch.ExecuteBatch();
      }
      return element.GetResults();
    }

    internal PayloadTable FetchWorkItemDataByReferenceUri(
      string toolSpecificId,
      CurrituckArtifactType artifactType)
    {
      this.m_requestContext.Trace(900206, TraceLevel.Verbose, "DataAccessLayer", nameof (InverseQuery), "-->DAL InverseQuery:FetchworkItemDataByReferenceUri()");
      DalGetWorkItemDetailsByReferencingUriElement element;
      using (SqlBatchBuilder sqlBatch = (SqlBatchBuilder) new WorkItemBatchBuilder(this.m_requestContext))
      {
        element = DalSqlElement.GetElement<DalGetWorkItemDetailsByReferencingUriElement>(sqlBatch);
        element.JoinBatch(toolSpecificId, Convert.ToInt32((object) artifactType, (IFormatProvider) CultureInfo.InvariantCulture), this.m_user);
        sqlBatch.ExecuteBatch();
      }
      return element.GetResultTable();
    }

    internal PayloadTable FetchWorkItemDataByLinkFilter(
      string artifactUri,
      CurrituckArtifactType artifactType,
      string linkTypeName)
    {
      this.m_requestContext.Trace(900207, TraceLevel.Verbose, "DataAccessLayer", nameof (InverseQuery), "-->DAL InverseQuery:FetchWorkItemDataByLinkFilter()");
      DalGetWorkItemDetailsByLinkFilterElement element;
      using (SqlBatchBuilder sqlBatch = (SqlBatchBuilder) new WorkItemBatchBuilder(this.m_requestContext))
      {
        element = DalSqlElement.GetElement<DalGetWorkItemDetailsByLinkFilterElement>(sqlBatch);
        element.JoinBatch(artifactUri, Convert.ToInt32((object) artifactType, (IFormatProvider) CultureInfo.InvariantCulture), linkTypeName, this.m_user);
        sqlBatch.ExecuteBatch();
      }
      return element.GetResultTable();
    }

    internal void UpdateArtifactWithWorkItemData(
      Artifact artifact,
      PayloadTable.PayloadRow workItemRow)
    {
      this.m_requestContext.TraceEnter(900208, "DataAccessLayer", nameof (InverseQuery), nameof (UpdateArtifactWithWorkItemData));
      if (!(workItemRow["System.Title"] is DBNull) && workItemRow["System.Title"] != null)
        artifact.ArtifactTitle = workItemRow["System.Title"].ToString();
      this.m_artifactId.ToolSpecificId = workItemRow["System.Id"].ToString();
      artifact.Uri = LinkingUtilities.EncodeUri(this.m_artifactId);
      List<ExtendedAttribute> extendedAttributeList = new List<ExtendedAttribute>();
      for (int index = 0; index < workItemRow.Table.Columns.Count; ++index)
      {
        ExtendedAttribute extendedAttribute = new ExtendedAttribute();
        extendedAttribute.Name = workItemRow.Table.Columns[index].Name;
        if (!(workItemRow[index] is DBNull) && workItemRow[index] != null)
          extendedAttribute.Value = workItemRow[index].ToString();
        extendedAttributeList.Add(extendedAttribute);
      }
      artifact.ExtendedAttributes = extendedAttributeList.ToArray();
      this.m_requestContext.TraceLeave(900564, "DataAccessLayer", nameof (InverseQuery), nameof (UpdateArtifactWithWorkItemData));
    }

    internal OutboundLink GetOutboundLinkWithLinkData(PayloadTable.PayloadRow linkItemRow)
    {
      this.m_requestContext.TraceEnter(900209, "DataAccessLayer", nameof (InverseQuery), nameof (GetOutboundLinkWithLinkData));
      OutboundLink linkWithLinkData = new OutboundLink();
      if (!(linkItemRow["System.Description"] is DBNull))
        linkWithLinkData.LinkType = linkItemRow["System.Description"].ToString();
      if (linkWithLinkData.LinkType == "Related Workitem")
      {
        if (!(linkItemRow["System.Id"] is DBNull))
        {
          this.m_artifactId.ToolSpecificId = linkItemRow["System.Id"].ToString();
          linkWithLinkData.ReferencedUri = LinkingUtilities.EncodeUri(this.m_artifactId);
        }
      }
      else if (!(linkItemRow["System.Id"] is DBNull))
        linkWithLinkData.ReferencedUri = linkItemRow["System.Id"].ToString();
      this.m_requestContext.TraceLeave(900565, "DataAccessLayer", nameof (InverseQuery), nameof (GetOutboundLinkWithLinkData));
      return linkWithLinkData;
    }
  }
}
