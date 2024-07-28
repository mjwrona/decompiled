// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Common.ActionItem
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Security;
using System.Threading;

namespace Microsoft.Azure.NotificationHubs.Common
{
  internal abstract class ActionItem
  {
    [SecurityCritical]
    private SecurityContext context;
    private bool isScheduled;
    private bool lowPriority;

    public bool LowPriority
    {
      get => this.lowPriority;
      protected set => this.lowPriority = value;
    }

    public static void Schedule(Action<object> callback, object state) => ActionItem.Schedule(callback, state, false);

    public static void Schedule(Action<object> callback, object state, bool lowPriority)
    {
      if (PartialTrustHelpers.ShouldFlowSecurityContext || WaitCallbackActionItem.ShouldUseActivity)
        new ActionItem.DefaultActionItem(callback, state, lowPriority).Schedule();
      else
        ActionItem.ScheduleCallback(callback, state, lowPriority);
    }

    [SecurityCritical]
    protected abstract void Invoke();

    [SecurityCritical]
    protected void Schedule()
    {
      this.isScheduled = !this.isScheduled ? true : throw Fx.Exception.AsError((Exception) new InvalidOperationException(SRCore.ActionItemIsAlreadyScheduled));
      if (PartialTrustHelpers.ShouldFlowSecurityContext)
        this.context = PartialTrustHelpers.CaptureSecurityContextNoIdentityFlow();
      if (this.context != null)
        this.ScheduleCallback(ActionItem.CallbackHelper.InvokeWithContextCallback);
      else
        this.ScheduleCallback(ActionItem.CallbackHelper.InvokeWithoutContextCallback);
    }

    [SecurityCritical]
    protected void ScheduleWithContext(SecurityContext contextToSchedule)
    {
      if (contextToSchedule == null)
        throw Fx.Exception.ArgumentNull("context");
      this.isScheduled = !this.isScheduled ? true : throw Fx.Exception.AsError((Exception) new InvalidOperationException(SRCore.ActionItemIsAlreadyScheduled));
      this.context = contextToSchedule.CreateCopy();
      this.ScheduleCallback(ActionItem.CallbackHelper.InvokeWithContextCallback);
    }

    [SecurityCritical]
    protected void ScheduleWithoutContext()
    {
      this.isScheduled = !this.isScheduled ? true : throw Fx.Exception.AsError((Exception) new InvalidOperationException(SRCore.ActionItemIsAlreadyScheduled));
      this.ScheduleCallback(ActionItem.CallbackHelper.InvokeWithoutContextCallback);
    }

    [SecurityCritical]
    private static void ScheduleCallback(Action<object> callback, object state, bool lowPriority)
    {
      if (lowPriority)
        IOThreadScheduler.ScheduleCallbackLowPriNoFlow(callback, state);
      else
        IOThreadScheduler.ScheduleCallbackNoFlow(callback, state);
    }

    [SecurityCritical]
    private SecurityContext ExtractContext()
    {
      SecurityContext context = this.context;
      this.context = (SecurityContext) null;
      return context;
    }

    [SecurityCritical]
    private void ScheduleCallback(Action<object> callback) => ActionItem.ScheduleCallback(callback, (object) this, this.lowPriority);

    [SecurityCritical]
    private static class CallbackHelper
    {
      private static Action<object> invokeWithContextCallback;
      private static Action<object> invokeWithoutContextCallback;
      private static ContextCallback onContextAppliedCallback;

      public static Action<object> InvokeWithContextCallback
      {
        get
        {
          if (ActionItem.CallbackHelper.invokeWithContextCallback == null)
            ActionItem.CallbackHelper.invokeWithContextCallback = new Action<object>(ActionItem.CallbackHelper.InvokeWithContext);
          return ActionItem.CallbackHelper.invokeWithContextCallback;
        }
      }

      public static Action<object> InvokeWithoutContextCallback
      {
        get
        {
          if (ActionItem.CallbackHelper.invokeWithoutContextCallback == null)
            ActionItem.CallbackHelper.invokeWithoutContextCallback = new Action<object>(ActionItem.CallbackHelper.InvokeWithoutContext);
          return ActionItem.CallbackHelper.invokeWithoutContextCallback;
        }
      }

      public static ContextCallback OnContextAppliedCallback
      {
        get
        {
          if (ActionItem.CallbackHelper.onContextAppliedCallback == null)
            ActionItem.CallbackHelper.onContextAppliedCallback = new ContextCallback(ActionItem.CallbackHelper.OnContextApplied);
          return ActionItem.CallbackHelper.onContextAppliedCallback;
        }
      }

      private static void InvokeWithContext(object state) => SecurityContext.Run(((ActionItem) state).ExtractContext(), ActionItem.CallbackHelper.OnContextAppliedCallback, state);

      private static void InvokeWithoutContext(object state)
      {
        ActionItem actionItem = (ActionItem) state;
        actionItem.Invoke();
        actionItem.isScheduled = false;
      }

      private static void OnContextApplied(object o)
      {
        ActionItem actionItem = (ActionItem) o;
        actionItem.Invoke();
        actionItem.isScheduled = false;
      }
    }

    private class DefaultActionItem : ActionItem
    {
      [SecurityCritical]
      private Action<object> callback;
      [SecurityCritical]
      private object state;

      public DefaultActionItem(Action<object> callback, object state, bool isLowPriority)
      {
        this.LowPriority = isLowPriority;
        this.callback = callback;
        this.state = state;
      }

      [SecurityCritical]
      protected override void Invoke() => this.callback(this.state);
    }
  }
}
