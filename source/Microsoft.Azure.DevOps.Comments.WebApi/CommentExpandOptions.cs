// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.Comments.WebApi.CommentExpandOptions
// Assembly: Microsoft.Azure.DevOps.Comments.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A55BAA93-5FAF-48BE-A9EC-2F097131C70D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.Comments.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.DevOps.Comments.WebApi
{
  [Flags]
  [DataContract]
  public enum CommentExpandOptions
  {
    None = 0,
    Reactions = 1,
    RenderedText = 8,
    RenderedTextOnly = 16, // 0x00000010
    Children = 32, // 0x00000020
    All = -17, // 0xFFFFFFEF
  }
}
