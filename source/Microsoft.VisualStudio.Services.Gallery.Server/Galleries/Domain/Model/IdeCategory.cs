// Decompiled with JetBrains decompiler
// Type: Galleries.Domain.Model.IdeCategory
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Galleries.Domain.Model
{
  [DataContract]
  public class IdeCategory
  {
    public IdeCategory() => this.Children = (IList<IdeCategory>) new List<IdeCategory>();

    [DataMember]
    public Guid Id { get; set; }

    [DataMember]
    public string Title { get; set; }

    [DataMember]
    public IdeCategory Parent { get; set; }

    [DataMember]
    public IList<IdeCategory> Children { get; set; }

    [DataMember]
    public bool HasMore { get; set; }
  }
}
