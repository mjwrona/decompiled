// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Compatibility.QueryResultSerializer
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 929F0284-16B2-4277-9F4A-B615689A77D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.dll

using Microsoft.VisualStudio.Services.Common;
using System.IO;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Compatibility
{
  public static class QueryResultSerializer
  {
    public static XmlElement Serialize(QueryResult queryResult, bool countOnly = false)
    {
      ArgumentUtility.CheckForNull<QueryResult>(queryResult, nameof (queryResult));
      StringBuilder output = new StringBuilder(1024);
      XmlWriter writer = XmlWriter.Create(output);
      if (queryResult.QueryType == QueryType.WorkItems | countOnly)
      {
        writer.WriteStartElement("QueryIds");
        if (countOnly)
        {
          writer.WriteStartElement("id");
          writer.WriteAttributeString("s", XmlConvert.ToString(queryResult.Count));
        }
        else
        {
          int num1 = -1;
          int num2 = 0;
          foreach (int workItemId in queryResult.WorkItemIds)
          {
            if (num1 != -1 && workItemId == num1 + 1)
            {
              num1 = workItemId;
            }
            else
            {
              if (num1 != -1)
              {
                if (num1 != num2)
                  writer.WriteAttributeString("e", XmlConvert.ToString(num1));
                writer.WriteEndElement();
              }
              writer.WriteStartElement("id");
              writer.WriteAttributeString("s", XmlConvert.ToString(workItemId));
              num2 = workItemId;
              num1 = workItemId;
            }
          }
          if (num1 != -1)
          {
            if (num1 != num2)
              writer.WriteAttributeString("e", XmlConvert.ToString(num1));
            writer.WriteEndElement();
          }
        }
        writer.WriteEndElement();
      }
      else
      {
        QueryResultSerializer.LinkResultContext context = new QueryResultSerializer.LinkResultContext();
        bool isFirstLink = true;
        writer.WriteStartElement("WorkItemLinkRelations");
        if (queryResult.QueryType == QueryType.LinksOneHopDoesNotContain)
        {
          foreach (int workItemId in queryResult.WorkItemIds)
          {
            context.ApplyCurrentLink(workItemId, 0, (short) 0, 0);
            QueryResultSerializer.FlushLink(writer, context, isFirstLink);
            if (isFirstLink)
              isFirstLink = false;
          }
        }
        else
        {
          LinkQueryResultEntry queryResultEntry = (LinkQueryResultEntry) null;
          foreach (LinkQueryResultEntry workItemLink in queryResult.WorkItemLinks)
          {
            if (queryResultEntry != null)
            {
              if (queryResultEntry.TargetId != workItemLink.SourceId)
              {
                context.ApplyCurrentLink(queryResultEntry.TargetId, 0, (short) 0, 0);
                QueryResultSerializer.FlushLink(writer, context, isFirstLink);
                if (isFirstLink)
                  isFirstLink = false;
              }
              queryResultEntry = (LinkQueryResultEntry) null;
            }
            if (queryResult.QueryType == QueryType.LinksRecursiveMayContain || workItemLink.SourceId > 0)
            {
              context.ApplyCurrentLink(workItemLink.SourceId, workItemLink.TargetId, workItemLink.LinkTypeId, workItemLink.IsLocked ? 1 : 0);
              QueryResultSerializer.FlushLink(writer, context, isFirstLink);
              if (isFirstLink)
                isFirstLink = false;
            }
            else
              queryResultEntry = workItemLink;
          }
          if (queryResultEntry != null)
          {
            context.ApplyCurrentLink(queryResultEntry.TargetId, 0, (short) 0, 0);
            QueryResultSerializer.FlushLink(writer, context, isFirstLink);
          }
        }
        QueryResultSerializer.FlushRun(writer, ref context.RunStart, ref context.RunEnd, context.IsSourceRun);
        writer.WriteEndElement();
      }
      writer.Close();
      XmlReaderSettings settings = new XmlReaderSettings();
      settings.DtdProcessing = DtdProcessing.Prohibit;
      settings.XmlResolver = (XmlResolver) null;
      XmlDocument xmlDocument = new XmlDocument();
      using (StringReader input = new StringReader(output.ToString()))
      {
        using (XmlReader reader = XmlReader.Create((TextReader) input, settings))
          xmlDocument.Load(reader);
      }
      return xmlDocument.DocumentElement;
    }

    private static void FlushLink(
      XmlWriter writer,
      QueryResultSerializer.LinkResultContext context,
      bool isFirstLink)
    {
      if (((context.SourceId == 0 ? 0 : (context.TargetId != 0 ? 1 : 0)) | (isFirstLink ? 1 : 0)) != 0)
      {
        QueryResultSerializer.FlushRun(writer, ref context.RunStart, ref context.RunEnd, context.IsSourceRun);
        writer.WriteStartElement("R");
        if (context.SourceId != context.LastSourceId | isFirstLink)
        {
          writer.WriteAttributeString("S", XmlConvert.ToString(context.SourceId));
          context.LastSourceId = context.SourceId;
        }
        if (context.TargetId != context.LastTargetId | isFirstLink)
        {
          writer.WriteAttributeString("T", XmlConvert.ToString(context.TargetId));
          context.LastTargetId = context.TargetId;
        }
        if ((int) context.LinkType != (int) context.LastLinkType | isFirstLink)
        {
          writer.WriteAttributeString("L", XmlConvert.ToString(context.LinkType));
          context.LastLinkType = context.LinkType;
        }
        if (context.EnumValue != context.LastEnumValue | isFirstLink)
        {
          writer.WriteAttributeString("E", XmlConvert.ToString(context.EnumValue));
          context.LastEnumValue = context.EnumValue;
        }
        writer.WriteEndElement();
      }
      else if (context.TargetId == 0)
      {
        if (context.RunStart > 0 && context.SourceId == context.LastSourceId + 1 && context.IsSourceRun)
        {
          context.RunEnd = context.SourceId;
        }
        else
        {
          QueryResultSerializer.FlushRun(writer, ref context.RunStart, ref context.RunEnd, context.IsSourceRun);
          context.RunStart = context.SourceId;
        }
        context.LastSourceId = context.SourceId;
        context.LastTargetId = 0;
        context.LastLinkType = (short) 0;
        context.LastEnumValue = 0;
        context.IsSourceRun = true;
      }
      else
      {
        if (context.RunStart > 0 && context.TargetId == context.LastTargetId + 1 && !context.IsSourceRun)
        {
          context.RunEnd = context.TargetId;
        }
        else
        {
          QueryResultSerializer.FlushRun(writer, ref context.RunStart, ref context.RunEnd, context.IsSourceRun);
          context.RunStart = context.TargetId;
        }
        context.LastSourceId = 0;
        context.LastTargetId = context.TargetId;
        context.LastLinkType = (short) 0;
        context.LastEnumValue = 0;
        context.IsSourceRun = false;
      }
    }

    private static void FlushRun(
      XmlWriter writer,
      ref int runStart,
      ref int runEnd,
      bool isSourceRun)
    {
      if (runStart <= 0)
        return;
      writer.WriteStartElement(isSourceRun ? "S" : "T");
      writer.WriteAttributeString("S", XmlConvert.ToString(runStart));
      if (runEnd > 0)
      {
        writer.WriteAttributeString("E", XmlConvert.ToString(runEnd));
        runEnd = 0;
      }
      runStart = 0;
      writer.WriteEndElement();
    }

    private class LinkResultContext
    {
      public int SourceId;
      public int TargetId;
      public short LinkType;
      public int EnumValue;
      public int LastSourceId;
      public int LastTargetId;
      public short LastLinkType;
      public int LastEnumValue;
      public int RunStart;
      public int RunEnd;
      public bool IsSourceRun;

      public LinkResultContext() => this.IsSourceRun = true;

      public void ApplyCurrentLink(int sourceId, int targetId, short linkType, int enumValue)
      {
        this.SourceId = sourceId;
        this.TargetId = targetId;
        this.LinkType = linkType;
        this.EnumValue = enumValue;
      }
    }
  }
}
