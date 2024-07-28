// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.WebApi.ReviewFilesZipContent
// Assembly: Microsoft.VisualStudio.Services.CodeReview.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84DE81C5-ABF4-4E22-A82B-21BA09D9141E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.CodeReview.WebApi
{
  [DataContract]
  public class ReviewFilesZipContent
  {
    [ClientResponseHeader("x-CodeReview-NextTop")]
    public IEnumerable<string> NextTop { get; set; }

    [ClientResponseHeader("x-CodeReview-NextSkip")]
    public IEnumerable<string> NextSkip { get; set; }

    [ClientResponseContent]
    public Stream ZipStream { get; set; }
  }
}
