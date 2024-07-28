// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.QnAComponent1
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class QnAComponent1 : QnAComponent
  {
    public virtual ExtensionQnAItem CreateQnAItem(
      Guid extensionId,
      Guid userId,
      string text,
      string extensionVersion,
      DateTime creationTime,
      long parentId,
      bool isPublisherCreated = false,
      bool isQuestion = false)
    {
      string str = "Gallery.prc_CreateQnAItem";
      this.PrepareStoredProcedure(str);
      this.BindLong(nameof (parentId), parentId);
      this.BindString("qnAText", text, 2048, false, SqlDbType.NVarChar);
      this.BindGuid(nameof (extensionId), extensionId);
      this.BindString(nameof (extensionVersion), extensionVersion, 43, false, SqlDbType.NVarChar);
      this.BindGuid(nameof (userId), userId);
      this.BindDateTime(nameof (creationTime), creationTime);
      this.BindBoolean(nameof (isPublisherCreated), isPublisherCreated);
      this.BindBoolean(nameof (isQuestion), isQuestion);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), str, this.RequestContext))
      {
        resultCollection.AddBinder<ExtensionQnAItem>((ObjectBinder<ExtensionQnAItem>) new QnAComponent.ExtensionQnAItemBinder());
        return resultCollection.GetCurrent<ExtensionQnAItem>().Items[0];
      }
    }

    public virtual ExtensionQnAItemQuestions GetQnAItemList(Guid extensionId, int count, int? page = null)
    {
      string str = "Gallery.prc_GetQnAThreads";
      this.PrepareStoredProcedure(str);
      this.BindGuid(nameof (extensionId), extensionId);
      this.BindInt(nameof (count), count);
      this.BindNullableInt(nameof (page), page);
      ExtensionQnAItemQuestions qnAitemList = new ExtensionQnAItemQuestions();
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), str, this.RequestContext))
      {
        resultCollection.AddBinder<ExtensionQnAItem>((ObjectBinder<ExtensionQnAItem>) new QnAComponent.ExtensionQnAItemBinder());
        qnAitemList.QuestionList = resultCollection.GetCurrent<ExtensionQnAItem>().Items;
        resultCollection.NextResult();
        resultCollection.AddBinder<ExtensionQnAItem>((ObjectBinder<ExtensionQnAItem>) new QnAComponent.ExtensionQnAItemBinder());
        qnAitemList.ResponseList = resultCollection.GetCurrent<ExtensionQnAItem>().Items;
        resultCollection.NextResult();
        resultCollection.AddBinder<bool>((ObjectBinder<bool>) new QnAComponent.QnAMetaDataBinder());
        qnAitemList.HasMoreQuestions = resultCollection.GetCurrent<bool>().Items[0];
      }
      return qnAitemList;
    }

    public virtual ExtensionQnAItem UpdateQnAItem(
      Guid extensionId,
      long parentId,
      long itemId,
      Guid userId,
      string text,
      string extensionVersion,
      DateTime modifiedTime)
    {
      string str = "Gallery.prc_UpdateQnAItem";
      this.PrepareStoredProcedure(str);
      this.BindLong(nameof (parentId), parentId);
      this.BindLong(nameof (itemId), itemId);
      this.BindGuid(nameof (extensionId), extensionId);
      this.BindGuid(nameof (userId), userId);
      this.BindString("qnAText", text, 2048, false, SqlDbType.NVarChar);
      this.BindString(nameof (extensionVersion), extensionVersion, 43, false, SqlDbType.NVarChar);
      this.BindDateTime(nameof (modifiedTime), modifiedTime);
      ExtensionQnAItem extensionQnAitem = (ExtensionQnAItem) null;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), str, this.RequestContext))
      {
        resultCollection.AddBinder<ExtensionQnAItem>((ObjectBinder<ExtensionQnAItem>) new QnAComponent.ExtensionQnAItemBinder());
        List<ExtensionQnAItem> items = resultCollection.GetCurrent<ExtensionQnAItem>().Items;
        if (items != null)
        {
          if (items.Count > 0)
            extensionQnAitem = items[0];
        }
      }
      return extensionQnAitem;
    }

    public virtual ExtensionQnAItem GetQnAItem(Guid extensionId, long parentId, long itemId)
    {
      string str = "Gallery.prc_GetQnAItem";
      this.PrepareStoredProcedure(str);
      this.BindLong(nameof (parentId), parentId);
      this.BindLong(nameof (itemId), itemId);
      this.BindGuid(nameof (extensionId), extensionId);
      ExtensionQnAItem qnAitem = (ExtensionQnAItem) null;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), str, this.RequestContext))
      {
        resultCollection.AddBinder<ExtensionQnAItem>((ObjectBinder<ExtensionQnAItem>) new QnAComponent.ExtensionQnAItemBinder());
        List<ExtensionQnAItem> items = resultCollection.GetCurrent<ExtensionQnAItem>().Items;
        if (items != null)
        {
          if (items.Count > 0)
            qnAitem = items[0];
        }
      }
      return qnAitem;
    }
  }
}
