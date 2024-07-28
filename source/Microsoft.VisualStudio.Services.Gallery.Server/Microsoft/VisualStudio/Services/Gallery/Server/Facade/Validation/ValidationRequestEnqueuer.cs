// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Facade.Validation.ValidationRequestEnqueuer
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.VisualStudio.Services.Gallery.Server.Facade.Validation.Serializers;
using NuGet.Services.ServiceBus;
using NuGet.Services.Validation;
using System;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Facade.Validation
{
  public class ValidationRequestEnqueuer : IValidationRequestEnqueuer
  {
    private readonly ITopicClient _topicClient;
    private readonly ICommandSerializer<PackageValidationMessageData, IBrokeredMessage> _serializer;

    public ValidationRequestEnqueuer(ITopicClient topicClient)
    {
      this._topicClient = topicClient;
      this._serializer = (ICommandSerializer<PackageValidationMessageData, IBrokeredMessage>) new ValidationCommandSerializer();
    }

    public void SendMessage(PackageValidationMessageData message) => this.SendMessage(message, DateTimeOffset.MinValue);

    public void SendMessage(
      PackageValidationMessageData message,
      DateTimeOffset postponeProcessingTill)
    {
      IBrokeredMessage ibrokeredMessage = this._serializer.Serialize(message);
      ibrokeredMessage.ScheduledEnqueueTimeUtc = postponeProcessingTill;
      this._topicClient.SendAsync(ibrokeredMessage);
    }
  }
}
