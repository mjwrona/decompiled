// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.Exceptions.FeedException
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Cosmos.ChangeFeed.Exceptions
{
  [Serializable]
  internal abstract class FeedException : Exception
  {
    public FeedException(string message, string lastContinuation)
      : base(message)
    {
      this.LastContinuation = lastContinuation;
    }

    protected FeedException(string message, string lastContinuation, Exception innerException)
      : base(message, innerException)
    {
      this.LastContinuation = lastContinuation;
    }

    protected FeedException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.LastContinuation = (string) info.GetValue(nameof (LastContinuation), typeof (string));
    }

    public string LastContinuation { get; }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("LastContinuation", (object) this.LastContinuation);
    }
  }
}
