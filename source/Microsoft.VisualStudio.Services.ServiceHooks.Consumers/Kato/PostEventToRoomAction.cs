// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Kato.PostEventToRoomAction
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.Consumers.DataDriven;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Kato
{
  [Export(typeof (ConsumerActionImplementation))]
  public class PostEventToRoomAction : DataDrivenConsumerAction<KatoConsumer>
  {
    private const string c_id = "postEventToRoom";

    public PostEventToRoomAction()
      : base("postEventToRoom")
    {
    }

    public ActionTask HandleEvent(
      IVssRequestContext requestContext,
      Event raisedEvent,
      HandleEventArgs eventArgs)
    {
      return this.MyHandleEvent(requestContext, raisedEvent, eventArgs);
    }
  }
}
