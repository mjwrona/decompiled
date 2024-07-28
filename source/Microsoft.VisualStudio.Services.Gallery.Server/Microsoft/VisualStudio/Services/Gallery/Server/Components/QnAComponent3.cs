// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.QnAComponent3
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class QnAComponent3 : QnAComponent2
  {
    public virtual ExtensionQnAItemQuestions GetQnAItemList(
      Guid extensionId,
      int count,
      int? page = null,
      DateTime? afterDate = null)
    {
      string str = "Gallery.prc_GetQnAThreads";
      this.PrepareStoredProcedure(str);
      if (!afterDate.HasValue)
        afterDate = new DateTime?(GalleryConstants.BeginningOfTime);
      this.BindGuid(nameof (extensionId), extensionId);
      this.BindInt(nameof (count), count);
      this.BindNullableInt(nameof (page), page);
      this.BindDateTime(nameof (afterDate), afterDate.Value.ToUniversalTime());
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
  }
}
