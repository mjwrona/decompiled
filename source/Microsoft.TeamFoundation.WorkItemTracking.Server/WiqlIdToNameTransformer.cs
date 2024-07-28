// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WiqlIdToNameTransformer
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Identity;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class WiqlIdToNameTransformer
  {
    public const char SpecialControlChar = '\a';
    public const string TFIDPattern = "TFID:'{0}'";
    public const string AreaPathIDPattern = "ARPID:'{0}:{1:0000000000}'";
    public const string IterationPathIDPattern = "ITPID:'{0}:{1:0000000000}'";
    public const string SpecialControlCharString = "\a";
    public const string TFIDPrefix = "\aTFID:'";

    public static void Transform(IVssRequestContext requestContext, QueryItem queryItem) => WiqlIdToNameTransformer.Transform(requestContext, Enumerable.Repeat<QueryItem>(queryItem, 1));

    public static void Transform(
      IVssRequestContext requestContext,
      IEnumerable<QueryItem> queryItems)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<QueryItem>>(queryItems, nameof (queryItems));
      requestContext.TraceBlock(902776, 902777, "Services", "QueryService", "WiqlIdToNameTransformer.Transform", (Action) (() =>
      {
        IList<Query> queryList = WiqlIdToNameTransformer.RetrieveAllQueries(queryItems);
        WiqlIdToNameTransformer transformer = new WiqlIdToNameTransformer();
        List<WiqlIdToNameTransformer.ReplacementPosition> idReplacementPositions = new List<WiqlIdToNameTransformer.ReplacementPosition>();
        HashSet<Guid> tfIds = new HashSet<Guid>();
        Dictionary<Guid, string> displayIdMap = new Dictionary<Guid, string>();
        Dictionary<Guid, string> projectIdToNameMap = new Dictionary<Guid, string>();
        requestContext.TraceBlock(902780, 902781, "Services", "QueryService", "WiqlIdToNameTransformer.Transform.CollectTfId", (Action) (() =>
        {
          for (int index = 0; index < queryList.Count; ++index)
          {
            HashSet<Guid> tfIds1;
            List<WiqlIdToNameTransformer.IdReference> idReferences = transformer.GetIdReferences(requestContext, queryList[index].Wiql, out tfIds1);
            tfIds.UnionWith((IEnumerable<Guid>) tfIds1);
            idReplacementPositions.Add(new WiqlIdToNameTransformer.ReplacementPosition()
            {
              QueryPosition = index,
              References = idReferences
            });
          }
        }));
        requestContext.TraceBlock(902782, 902783, "Services", "QueryService", "WiqlIdToNameTransformer.Transform.ResolveIdentities", (Action) (() => displayIdMap = transformer.GetTfIdDisplayNameMap(requestContext, tfIds)));
        requestContext.TraceBlock(902784, 902785, "Services", "QueryService", "WiqlIdToNameTransformer.Transform.ReplaceTfId", (Action) (() =>
        {
          foreach (WiqlIdToNameTransformer.ReplacementPosition replacementPosition in idReplacementPositions)
          {
            string str = transformer.ReplaceIdWithText(requestContext, queryList[replacementPosition.QueryPosition].Wiql, replacementPosition.References, displayIdMap, projectIdToNameMap);
            queryList[replacementPosition.QueryPosition].Wiql = str;
          }
        }));
      }));
    }

    private bool TryGetAreaIterationIdReference(
      string wiql,
      int pos,
      WiqlIdToNameTransformer.EnumReferenceType referenceType,
      out WiqlIdToNameTransformer.IdReference reference)
    {
      reference = new WiqlIdToNameTransformer.IdReference();
      bool iterationIdReference = false;
      string[] strArray = wiql.Substring(pos + 9, 47).Split(':');
      string input = strArray[0];
      string s = strArray[1];
      Guid result1 = Guid.Empty;
      int result2 = -1;
      if (Guid.TryParse(input, out result1) && int.TryParse(s, out result2))
      {
        reference = new WiqlIdToNameTransformer.IdReference()
        {
          Position = pos,
          TeamfoundationId = result1,
          NodeId = result2,
          ReferenceType = referenceType
        };
        iterationIdReference = true;
      }
      return iterationIdReference;
    }

    public List<WiqlIdToNameTransformer.IdReference> GetIdReferences(
      IVssRequestContext requestContext,
      string wiql,
      out HashSet<Guid> tfIds)
    {
      List<WiqlIdToNameTransformer.IdReference> idReferences = new List<WiqlIdToNameTransformer.IdReference>();
      tfIds = new HashSet<Guid>();
      if (string.IsNullOrEmpty(wiql))
        return idReferences;
      int length = wiql.Length;
      int pos = -1;
      WiqlIdToNameTransformer.IdReference idReference1;
      while ((pos = wiql.IndexOf('\a', pos + 1)) > 0 && pos + 1 < length)
      {
        if (wiql[pos - 1] == '\'')
        {
          if (pos + 46 < length && wiql[pos + 1] == 'T' && wiql[pos + 2] == 'F' && wiql[pos + 3] == 'I' && wiql[pos + 4] == 'D' && wiql[pos + 5] == ':' && wiql[pos + 6] == '\'' && wiql[pos + 7] == '\'' && wiql[pos + 44] == '\'' && wiql[pos + 45] == '\'' && wiql[pos + 46] == '\'')
          {
            Guid result;
            if (Guid.TryParse(wiql.Substring(pos + 8, 36), out result))
            {
              tfIds.Add(result);
              List<WiqlIdToNameTransformer.IdReference> idReferenceList = idReferences;
              idReference1 = new WiqlIdToNameTransformer.IdReference();
              idReference1.Position = pos;
              idReference1.TeamfoundationId = result;
              idReference1.ReferenceType = WiqlIdToNameTransformer.EnumReferenceType.TfId;
              WiqlIdToNameTransformer.IdReference idReference2 = idReference1;
              idReferenceList.Add(idReference2);
            }
            pos += 46;
          }
          else if (pos + 7 < length && wiql[pos + 1] == 'A' && wiql[pos + 2] == 'R' && wiql[pos + 3] == 'P' && wiql[pos + 4] == 'I' && wiql[pos + 5] == 'D' && wiql[pos + 6] == ':' && wiql[pos + 7] == '\'')
          {
            if (pos + 58 < length && wiql[pos + 45] == ':' && wiql[pos + 56] == '\'')
            {
              WiqlIdToNameTransformer.IdReference reference;
              if (this.TryGetAreaIterationIdReference(wiql, pos, WiqlIdToNameTransformer.EnumReferenceType.Area, out reference))
                idReferences.Add(reference);
              pos += 56;
            }
            else
            {
              List<WiqlIdToNameTransformer.IdReference> idReferenceList = idReferences;
              idReference1 = new WiqlIdToNameTransformer.IdReference();
              idReference1.Position = pos;
              idReference1.ReferenceType = WiqlIdToNameTransformer.EnumReferenceType.InvalidPath;
              WiqlIdToNameTransformer.IdReference idReference3 = idReference1;
              idReferenceList.Add(idReference3);
              pos += 8;
            }
          }
          else if (pos + 7 < length && wiql[pos + 1] == 'I' && wiql[pos + 2] == 'T' && wiql[pos + 3] == 'P' && wiql[pos + 4] == 'I' && wiql[pos + 5] == 'D' && wiql[pos + 6] == ':' && wiql[pos + 7] == '\'')
          {
            if (pos + 58 < length && wiql[pos + 45] == ':' && wiql[pos + 56] == '\'')
            {
              WiqlIdToNameTransformer.IdReference reference;
              if (this.TryGetAreaIterationIdReference(wiql, pos, WiqlIdToNameTransformer.EnumReferenceType.Iteration, out reference))
                idReferences.Add(reference);
              pos += 56;
            }
            else
            {
              List<WiqlIdToNameTransformer.IdReference> idReferenceList = idReferences;
              idReference1 = new WiqlIdToNameTransformer.IdReference();
              idReference1.Position = pos;
              idReference1.ReferenceType = WiqlIdToNameTransformer.EnumReferenceType.InvalidPath;
              WiqlIdToNameTransformer.IdReference idReference4 = idReference1;
              idReferenceList.Add(idReference4);
              pos += 8;
            }
          }
          else
          {
            int num;
            for (num = pos; num + 1 < length && (num = wiql.IndexOf('\'', num + 1)) > 0; ++num)
            {
              if (num + 1 >= length || wiql[num + 1] != '\'')
              {
                List<WiqlIdToNameTransformer.IdReference> idReferenceList = idReferences;
                idReference1 = new WiqlIdToNameTransformer.IdReference();
                idReference1.Position = pos;
                idReference1.Text = wiql.Substring(pos + 1, num - pos - 1);
                idReference1.ReferenceType = WiqlIdToNameTransformer.EnumReferenceType.TfId;
                WiqlIdToNameTransformer.IdReference idReference5 = idReference1;
                idReferenceList.Add(idReference5);
                break;
              }
            }
            pos = num;
          }
        }
      }
      return idReferences;
    }

    public Dictionary<Guid, string> GetTfIdDisplayNameMap(
      IVssRequestContext requestContext,
      HashSet<Guid> tfIds)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<HashSet<Guid>>(tfIds, nameof (tfIds));
      Dictionary<Guid, string> idDisplayNameMap = new Dictionary<Guid, string>();
      if (tfIds.Any<Guid>())
      {
        IdentityDisplayType identityDisplayType = requestContext.GetIdentityDisplayType();
        if (identityDisplayType == IdentityDisplayType.ComboDisplayName)
        {
          foreach (Microsoft.VisualStudio.Services.Identity.Identity readIdentity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) requestContext.WitContext().IdentityService.ReadIdentities(requestContext, (IList<Guid>) tfIds.ToArray<Guid>(), QueryMembership.None, (IEnumerable<string>) null))
          {
            if (readIdentity != null)
              idDisplayNameMap[readIdentity.Id] = readIdentity.GetLegacyDistinctDisplayName();
          }
        }
        else
        {
          foreach (ConstantRecord constantRecord in requestContext.GetService<ITeamFoundationWorkItemTrackingMetadataService>().GetConstantRecords(requestContext, (IEnumerable<Guid>) tfIds, true))
          {
            switch (identityDisplayType)
            {
              case IdentityDisplayType.DisplayName:
                idDisplayNameMap[constantRecord.TeamFoundationId] = IdentityHelper.GetDisplayNameFromDistinctDisplayName(constantRecord.DisplayText);
                continue;
              case IdentityDisplayType.ComboDisplayNameWhenNeeded:
                idDisplayNameMap[constantRecord.TeamFoundationId] = constantRecord.HasUniqueIdentityDisplayName ? IdentityHelper.GetDisplayNameFromDistinctDisplayName(constantRecord.DisplayText) : constantRecord.DisplayText;
                continue;
              default:
                idDisplayNameMap[constantRecord.TeamFoundationId] = constantRecord.DisplayText;
                continue;
            }
          }
        }
      }
      return idDisplayNameMap;
    }

    public string ReplaceIdWithText(
      IVssRequestContext requestContext,
      string wiql,
      List<WiqlIdToNameTransformer.IdReference> tfIdreferences,
      Dictionary<Guid, string> displayIdMap,
      Dictionary<Guid, string> projectIdToNameMap)
    {
      if (string.IsNullOrEmpty(wiql))
        return wiql;
      ArgumentUtility.CheckForNull<List<WiqlIdToNameTransformer.IdReference>>(tfIdreferences, nameof (tfIdreferences));
      ArgumentUtility.CheckForNull<Dictionary<Guid, string>>(displayIdMap, nameof (displayIdMap));
      StringBuilder stringBuilder = new StringBuilder();
      int startIndex = 0;
      ITreeDictionary treeService = requestContext.WitContext().TreeService;
      IProjectService service = requestContext.GetService<IProjectService>();
      foreach (WiqlIdToNameTransformer.IdReference tfIdreference in tfIdreferences)
      {
        stringBuilder.Append(wiql, startIndex, tfIdreference.Position - startIndex);
        if (tfIdreference.ReferenceType == WiqlIdToNameTransformer.EnumReferenceType.Area || tfIdreference.ReferenceType == WiqlIdToNameTransformer.EnumReferenceType.Iteration)
        {
          string str1;
          if (!projectIdToNameMap.TryGetValue(tfIdreference.TeamfoundationId, out str1))
          {
            try
            {
              str1 = service.GetProjectName(requestContext.Elevate(), tfIdreference.TeamfoundationId);
            }
            catch (Exception ex)
            {
              requestContext.TraceException(909200, "Services", "QueryService", ex);
              str1 = ServerResources.UnknownProjectName();
            }
            projectIdToNameMap[tfIdreference.TeamfoundationId] = str1;
          }
          TreeNode node;
          if (treeService.TryGetTreeNode(tfIdreference.TeamfoundationId, tfIdreference.NodeId, out node))
          {
            string str2 = node.IsProject ? str1 : str1 + node.RelativePath;
            stringBuilder.Append(str2.Replace("'", "''"));
          }
          else
          {
            string str3 = str1 + ServerResources.InvalidPath().Replace("'", "''");
            stringBuilder.Append(str3);
          }
          startIndex = tfIdreference.Position + 58;
        }
        else if (tfIdreference.ReferenceType == WiqlIdToNameTransformer.EnumReferenceType.InvalidPath)
        {
          startIndex = wiql.IndexOf("''", tfIdreference.Position + 10);
          stringBuilder.Append(ServerResources.InvalidPath().Replace("'", "''"));
        }
        else if (tfIdreference.ReferenceType == WiqlIdToNameTransformer.EnumReferenceType.TfId)
        {
          if (string.IsNullOrEmpty(tfIdreference.Text))
          {
            string str;
            if (displayIdMap.TryGetValue(tfIdreference.TeamfoundationId, out str))
              stringBuilder.Append(str.Replace("'", "''"));
            else
              stringBuilder.Append((object) tfIdreference.TeamfoundationId);
            startIndex = tfIdreference.Position + 46;
          }
          else
          {
            stringBuilder.Append(tfIdreference.Text);
            startIndex = tfIdreference.Position + tfIdreference.Text.Length + 1;
          }
        }
      }
      wiql = stringBuilder.Append(wiql.Substring(startIndex)).ToString();
      return wiql;
    }

    public string ReplaceIdWithText(IVssRequestContext requestContext, string wiql)
    {
      List<WiqlIdToNameTransformer.ReplacementPosition> replacementPositionList = new List<WiqlIdToNameTransformer.ReplacementPosition>();
      HashSet<Guid> tfIds1 = new HashSet<Guid>();
      Dictionary<Guid, string> dictionary = new Dictionary<Guid, string>();
      Dictionary<Guid, string> projectIdToNameMap = new Dictionary<Guid, string>();
      HashSet<Guid> tfIds2;
      List<WiqlIdToNameTransformer.IdReference> idReferences = this.GetIdReferences(requestContext, wiql, out tfIds2);
      tfIds1.UnionWith((IEnumerable<Guid>) tfIds2);
      Dictionary<Guid, string> idDisplayNameMap = this.GetTfIdDisplayNameMap(requestContext, tfIds1);
      return this.ReplaceIdWithText(requestContext, wiql, idReferences, idDisplayNameMap, projectIdToNameMap);
    }

    private static IList<Query> RetrieveAllQueries(IEnumerable<QueryItem> queryItems)
    {
      Dictionary<Guid, Query> queryDictionary = new Dictionary<Guid, Query>();
      WiqlIdToNameTransformer.PopulateQueryDictionary(queryItems, (IDictionary<Guid, Query>) queryDictionary);
      return (IList<Query>) queryDictionary.Values.ToList<Query>();
    }

    private static void PopulateQueryDictionary(
      IEnumerable<QueryItem> queryItems,
      IDictionary<Guid, Query> queryDictionary)
    {
      if (queryItems == null)
        return;
      foreach (QueryItem queryItem in queryItems)
      {
        if (queryItem is Query)
        {
          Query query = queryItem as Query;
          queryDictionary[query.Id] = query;
        }
        else
          WiqlIdToNameTransformer.PopulateQueryDictionary((IEnumerable<QueryItem>) (queryItem as QueryFolder).Children, queryDictionary);
      }
    }

    public struct ReplacementPosition
    {
      public int QueryPosition;
      public List<WiqlIdToNameTransformer.IdReference> References;
    }

    public struct IdReference
    {
      public int Position;
      public Guid TeamfoundationId;
      public int NodeId;
      public string Text;
      public WiqlIdToNameTransformer.EnumReferenceType ReferenceType;
    }

    public enum EnumReferenceType
    {
      Area,
      Iteration,
      TfId,
      InvalidPath,
    }
  }
}
