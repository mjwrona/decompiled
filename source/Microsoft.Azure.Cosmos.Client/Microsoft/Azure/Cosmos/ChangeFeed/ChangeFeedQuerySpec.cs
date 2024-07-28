// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.ChangeFeedQuerySpec
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Cosmos.ChangeFeed
{
  [DataContract]
  internal sealed class ChangeFeedQuerySpec
  {
    public ChangeFeedQuerySpec(string queryText, bool enableQueryOnPreviousImage)
      : this(queryText)
    {
      this.EnableQueryOnPreviousImage = enableQueryOnPreviousImage;
    }

    public ChangeFeedQuerySpec(string queryText) => this.QueryText = queryText ?? throw new ArgumentNullException(nameof (queryText));

    [DataMember(Name = "query")]
    private string QueryText { get; set; }

    [DataMember(Name = "enableQueryOnPreviousImage")]
    private bool EnableQueryOnPreviousImage { get; set; }

    internal bool ShouldSerializeQueryText() => this.QueryText.Length > 0;
  }
}
