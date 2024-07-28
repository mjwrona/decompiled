// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeedProcessorUserException
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Azure.Core.Pipeline;
using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Cosmos
{
  [Serializable]
  public class ChangeFeedProcessorUserException : Exception
  {
    private static readonly string DefaultMessage = "Exception has been thrown by the change feed processor delegate.";

    public ChangeFeedProcessorUserException(
      Exception originalException,
      ChangeFeedProcessorContext context)
      : base(ChangeFeedProcessorUserException.DefaultMessage, originalException)
    {
      this.ChangeFeedProcessorContext = context;
    }

    protected ChangeFeedProcessorUserException(SerializationInfo info, StreamingContext context)
      : this((Exception) info.GetValue("InnerException", typeof (Exception)), (ChangeFeedProcessorContext) null)
    {
    }

    public ChangeFeedProcessorContext ChangeFeedProcessorContext { get; private set; }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) => base.GetObjectData(info, context);

    internal static void RecordOtelAttributes(
      ChangeFeedProcessorUserException exception,
      DiagnosticScope scope)
    {
      scope.AddAttribute("exception.message", exception.Message);
    }
  }
}
