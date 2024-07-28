// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationStepPerformerBase
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.AuditLog;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public abstract class TeamFoundationStepPerformerBase : IStepPerformer
  {
    private string m_name;
    private Dictionary<string, IServicingStep> m_steps = new Dictionary<string, IServicingStep>();
    private static readonly object[] s_emptyObjectArray = Array.Empty<object>();

    protected TeamFoundationStepPerformerBase() => this.Initialize();

    string IStepPerformer.Name => this.m_name;

    public virtual IServicingStep GetServicingStep(string stepType)
    {
      IServicingStep servicingStep;
      this.m_steps.TryGetValue(stepType, out servicingStep);
      return servicingStep;
    }

    public virtual void PerformStep(
      string servicingOperation,
      ServicingOperationTarget target,
      string stepType,
      string stepData,
      ServicingContext servicingContext)
    {
      IServicingStep servicingStep = this.GetServicingStep(stepType);
      this.PerformStep(servicingOperation, target, servicingStep, stepType, stepData, servicingContext);
    }

    protected virtual void PerformStep(
      string servicingOperation,
      ServicingOperationTarget target,
      IServicingStep servicingStep,
      string stepType,
      string stepData,
      ServicingContext servicingContext)
    {
      if (servicingStep == null)
        throw new InvalidServicingStepTypeException(servicingOperation, (IStepPerformer) this, stepType);
      switch (target)
      {
        case ServicingOperationTarget.PartitionDatabase:
        case ServicingOperationTarget.ConfigurationDatabase:
          this.PerformDatabaseStep(servicingOperation, target, servicingStep, stepData, servicingContext);
          break;
        case ServicingOperationTarget.DeploymentHost:
        case ServicingOperationTarget.OrganizationHost:
        case ServicingOperationTarget.CollectionHost:
          this.PerformHostStep(servicingOperation, target, servicingStep, stepData, servicingContext);
          break;
        default:
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unknown ServicingOperationTarget in PerformStep: {0}.", (object) target), nameof (target));
      }
    }

    private void PerformDatabaseStep(
      string servicingOperation,
      ServicingOperationTarget target,
      IServicingStep servicingStep,
      string stepData,
      ServicingContext servicingContext)
    {
      if (!(servicingStep is IBasicServicingStep basicServicingStep))
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The following step cannot be used during database servicing, since it is host or partition specific. Step: {0}. Type: {1}.", (object) servicingStep.StepType, (object) servicingStep.GetType().Name));
      basicServicingStep.Execute(servicingContext, stepData);
    }

    private void PerformHostStep(
      string servicingOperation,
      ServicingOperationTarget target,
      IServicingStep servicingStep,
      string stepData,
      ServicingContext servicingContext)
    {
      switch (servicingStep)
      {
        case IBasicServicingStep basicServicingStep:
          basicServicingStep.Execute(servicingContext, stepData);
          break;
        case IRequestContextServicingStep contextServicingStep:
          IVssRequestContext vssRequestContext = (IVssRequestContext) null;
          if (contextServicingStep.TargetRequestContextOptional)
          {
            bool flag;
            if (!servicingContext.TryGetItem<bool>(ServicingItemConstants.CollectionDatabaseReachable, out flag) | flag)
              vssRequestContext = servicingContext.GetTargetRequestContext();
          }
          else
            vssRequestContext = servicingContext.GetTargetRequestContext();
          if (vssRequestContext != null)
          {
            vssRequestContext.Items[RequestContextItemsKeys.CurrentServicingOperation] = (object) servicingOperation;
            vssRequestContext.SetAuditCorrelationId(servicingContext.Tokens);
          }
          contextServicingStep.Execute(vssRequestContext, servicingContext, stepData);
          break;
        case IPartitionServicingStep partitionServicingStep:
          int targetPartitionId = servicingContext.GetTargetPartitionId();
          partitionServicingStep.Execute(targetPartitionId, servicingContext, stepData);
          break;
        default:
          throw new InvalidOperationException(string.Format("Unsupported servicing step for PerformHostStep. Step: {0}. Type:{1}.", (object) servicingStep.StepType, (object) servicingStep.GetType().Name));
      }
    }

    protected void LogObjectProperties(IServicingContext servicingContext, object stepDataObject)
    {
      if (stepDataObject == null)
      {
        servicingContext.LogInfo("No data provided.");
      }
      else
      {
        try
        {
          StringBuilder stringBuilder = new StringBuilder();
          PropertyInfo[] properties = stepDataObject.GetType().GetProperties();
          int num = 0;
          foreach (PropertyInfo element in properties)
          {
            MethodInfo getMethod = element.GetGetMethod();
            if (getMethod != (MethodInfo) null)
            {
              object connectionString = getMethod.Invoke(stepDataObject, TeamFoundationStepPerformerBase.s_emptyObjectArray);
              if (char.MinValue.Equals(connectionString))
                stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "Step Data: {0} => {1}", (object) element.Name, (object) "'\\0'").AppendLine();
              else if (connectionString is Array)
              {
                Array array = (Array) connectionString;
                for (int index = 0; index < array.Length; ++index)
                  stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "Step Data: {0}[{1}] => {2}", (object) element.Name, (object) index, array.GetValue(index)).AppendLine();
              }
              else if (connectionString != null && !connectionString.Equals((object) string.Empty) && element.GetCustomAttribute<ConnectionStringAttribute>() != null)
                stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "Step Data: {0} => {1}", (object) element.Name, (object) ConnectionStringUtility.MaskPassword((string) connectionString)).AppendLine();
              else
                stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "Step Data: {0} => {1}", (object) element.Name, connectionString).AppendLine();
              ++num;
            }
          }
          if (stringBuilder.Length > 2)
            stringBuilder.Remove(stringBuilder.Length - 2, 2);
          servicingContext.LogInfo(stringBuilder.ToString());
        }
        catch (Exception ex)
        {
          servicingContext.LogInfo("Unable to reflect on step data object.");
        }
      }
    }

    private void Initialize()
    {
      object[] customAttributes1 = this.GetType().GetCustomAttributes(typeof (StepPerformerAttribute), true);
      this.m_name = customAttributes1.Length != 0 ? ((StepPerformerAttribute) customAttributes1[0]).Name : throw new InvalidOperationException("StepPerformer attribute is missing.");
      foreach (MethodInfo method in this.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
      {
        object[] customAttributes2 = method.GetCustomAttributes(typeof (ServicingStepAttribute), false);
        if (customAttributes2.Length != 0)
        {
          ServicingStepAttribute servicingStepAttribute = (ServicingStepAttribute) customAttributes2[0];
          bool flag = method.ReturnType == typeof (Task);
          if (!flag && method.ReturnType != typeof (void))
            throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Method {0} has a ServicingStep attribute. Its return type must be void or System.Threading.Tasks.Task", (object) method.Name));
          ParameterInfo[] parameters = method.GetParameters();
          string name = method.Name;
          IServicingStep servicingStep = (IServicingStep) null;
          if (parameters.Length == 1)
          {
            if (!TeamFoundationStepPerformerBase.IsValidServicingContextType(parameters[0].ParameterType))
              throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Method {0} has a ServicingStep attribute and one parameter. Parameter type must be Microsoft.TeamFoundation.Server.Core.ServicingContext.", (object) name));
            if (flag)
              TeamFoundationStepPerformerBase.ThrowAsyncNotSupportedException(name);
            TeamFoundationStepPerformerBase.BasicStepDelegate stepDelegate = (TeamFoundationStepPerformerBase.BasicStepDelegate) Delegate.CreateDelegate(typeof (TeamFoundationStepPerformerBase.BasicStepDelegate), (object) this, method);
            servicingStep = (IServicingStep) new TeamFoundationStepPerformerBase.BasicServicingStep(name, stepDelegate);
          }
          else if (parameters.Length == 2)
          {
            if (parameters[0].ParameterType == typeof (IVssRequestContext))
            {
              if (!TeamFoundationStepPerformerBase.IsValidServicingContextType(parameters[1].ParameterType))
                throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Method {0} has a ServicingStep attribute and two parameters. First parameter type is IVssRequestContext. Second parameter type must be Microsoft.TeamFoundation.Server.Core.ServicingContext.", (object) name));
              if (flag)
              {
                if (servicingStepAttribute.TargetRequestContextOptional)
                  throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Method {0} has a ServicingStep attribute and returns a Task. The TargetRequestContextOptional setting must be false to use asynchronous execution.", (object) name));
                TeamFoundationStepPerformerBase.RequestContextAsyncStepDelegate stepDelegate = (TeamFoundationStepPerformerBase.RequestContextAsyncStepDelegate) Delegate.CreateDelegate(typeof (TeamFoundationStepPerformerBase.RequestContextAsyncStepDelegate), (object) this, method);
                servicingStep = (IServicingStep) new TeamFoundationStepPerformerBase.RequestContextServicingStep(name, servicingStepAttribute.TargetRequestContextOptional, stepDelegate);
              }
              else
              {
                TeamFoundationStepPerformerBase.RequestContextStepDelegate stepDelegate = (TeamFoundationStepPerformerBase.RequestContextStepDelegate) Delegate.CreateDelegate(typeof (TeamFoundationStepPerformerBase.RequestContextStepDelegate), (object) this, method);
                servicingStep = (IServicingStep) new TeamFoundationStepPerformerBase.RequestContextServicingStep(name, servicingStepAttribute.TargetRequestContextOptional, stepDelegate);
              }
            }
            else if (parameters[0].ParameterType == typeof (int))
            {
              if (!string.Equals(parameters[0].Name, "partitionId", StringComparison.Ordinal))
                throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Method {0} has a ServicingStep attribute. Since first parameter type is System.Int32, its name must be partitionId.", (object) name));
              if (!TeamFoundationStepPerformerBase.IsValidServicingContextType(parameters[1].ParameterType))
                throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Method {0} has a ServicingStep attribute and two parameters. First parameter is partitionId. Second parameter type must be Microsoft.TeamFoundation.Server.Core.ServicingContext.", (object) name));
              if (flag)
                TeamFoundationStepPerformerBase.ThrowAsyncNotSupportedException(name);
              TeamFoundationStepPerformerBase.DatabasePartitionStepDelegate stepDelegate = (TeamFoundationStepPerformerBase.DatabasePartitionStepDelegate) Delegate.CreateDelegate(typeof (TeamFoundationStepPerformerBase.DatabasePartitionStepDelegate), (object) this, method);
              servicingStep = (IServicingStep) new TeamFoundationStepPerformerBase.DatabasePartitionServicingStep(name, stepDelegate);
            }
            else
            {
              if (!TeamFoundationStepPerformerBase.IsValidServicingContextType(parameters[0].ParameterType))
                throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Method {0} has a ServicingStep attribute and two parameters. First parameter type must be Microsoft.TeamFoundation.Server.Core.ServicingContext.", (object) name));
              if (parameters[1].ParameterType == typeof (IVssRequestContext))
                throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Method {0} has a ServicingStep attribute and two parameter. Second parameter type is TeamFoundationRequest. TeamFoundationRequest parameter must be first parameter.", (object) name));
              if (flag)
                TeamFoundationStepPerformerBase.ThrowAsyncNotSupportedException(name);
              Delegate @delegate = Delegate.CreateDelegate(typeof (TeamFoundationStepPerformerBase.BasicStepDelegateWithData<>).MakeGenericType(parameters[1].ParameterType), (object) this, method);
              servicingStep = (IServicingStep) Activator.CreateInstance(typeof (TeamFoundationStepPerformerBase.BasicServicingStepWithData<>).MakeGenericType(parameters[1].ParameterType), (object) name, (object) @delegate);
            }
          }
          else if (parameters.Length == 3)
          {
            if (!TeamFoundationStepPerformerBase.IsValidServicingContextType(parameters[1].ParameterType))
              throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Method {0} has a ServicingStep attribute and three parameters. Second parameter type must be Microsoft.TeamFoundation.Server.Core.ServicingContext.", (object) name));
            if (parameters[0].ParameterType == typeof (IVssRequestContext))
            {
              Delegate @delegate;
              if (flag)
              {
                if (servicingStepAttribute.TargetRequestContextOptional)
                  throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Method {0} has a ServicingStep attribute and returns a Task. The TargetRequestContextOptional setting must be false to use asynchronous execution.", (object) name));
                @delegate = Delegate.CreateDelegate(typeof (TeamFoundationStepPerformerBase.RequestContextAsyncStepDelegateWithData<>).MakeGenericType(parameters[2].ParameterType), (object) this, method);
              }
              else
                @delegate = Delegate.CreateDelegate(typeof (TeamFoundationStepPerformerBase.RequestContextStepDelegateWithData<>).MakeGenericType(parameters[2].ParameterType), (object) this, method);
              servicingStep = (IServicingStep) Activator.CreateInstance(typeof (TeamFoundationStepPerformerBase.RequestContextServicingStepWithData<>).MakeGenericType(parameters[2].ParameterType), (object) name, (object) servicingStepAttribute.TargetRequestContextOptional, (object) @delegate);
            }
            else if (parameters[0].ParameterType == typeof (int))
            {
              if (!string.Equals(parameters[0].Name, "partitionId", StringComparison.Ordinal))
                throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Method {0} has a ServicingStep attribute. Since first parameter type is System.Int32, its name must be partitionId.", (object) name));
              if (flag)
                TeamFoundationStepPerformerBase.ThrowAsyncNotSupportedException(name);
              Delegate @delegate = Delegate.CreateDelegate(typeof (TeamFoundationStepPerformerBase.DatabasePartitionStepDelegateWithData<>).MakeGenericType(parameters[2].ParameterType), (object) this, method);
              servicingStep = (IServicingStep) Activator.CreateInstance(typeof (TeamFoundationStepPerformerBase.DatabasePartitionServicingStepWithData<>).MakeGenericType(parameters[2].ParameterType), (object) name, (object) @delegate);
            }
          }
          if (servicingStep == null)
            throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Method {0} has ServicingStep attribute, but its signature is not supported.", (object) name));
          try
          {
            this.m_steps.Add(name, servicingStep);
          }
          catch (ArgumentException ex)
          {
            throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "2 or more methods with name {0} are registered as servicing step.", (object) name));
          }
        }
      }
    }

    private static bool IsValidServicingContextType(Type type) => type == typeof (ServicingContext) || type == typeof (IServicingContext);

    private static void ThrowAsyncNotSupportedException(string stepType) => throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Method {0} has a ServicingStep attribute and returns a Task. Asynchronous step performers are not supported for methods which do not accept an IVssRequestContext as the first parameter.", (object) stepType));

    private class ServicingStepBase : IServicingStep
    {
      public ServicingStepBase(string stepType) => this.StepType = stepType;

      public string StepType { get; private set; }
    }

    private class BasicServicingStep : 
      TeamFoundationStepPerformerBase.ServicingStepBase,
      IBasicServicingStep,
      IServicingStep
    {
      private readonly TeamFoundationStepPerformerBase.BasicStepDelegate m_stepDelegate;

      public BasicServicingStep(
        string stepType,
        TeamFoundationStepPerformerBase.BasicStepDelegate stepDelegate)
        : base(stepType)
      {
        this.m_stepDelegate = stepDelegate;
      }

      public void Execute(ServicingContext servicingContext, string stepData)
      {
        if (!string.IsNullOrEmpty(stepData))
          throw new ArgumentException(FrameworkResources.StepDataNotRequired());
        this.m_stepDelegate(servicingContext);
      }
    }

    private class BasicServicingStepWithData<TStepData> : 
      TeamFoundationStepPerformerBase.ServicingStepBase,
      IBasicServicingStep,
      IServicingStep
    {
      private readonly TeamFoundationStepPerformerBase.BasicStepDelegateWithData<TStepData> m_stepDelegate;

      public BasicServicingStepWithData(
        string stepType,
        TeamFoundationStepPerformerBase.BasicStepDelegateWithData<TStepData> stepDelegate)
        : base(stepType)
      {
        this.m_stepDelegate = stepDelegate;
      }

      public void Execute(ServicingContext servicingContext, string stepData)
      {
        if (typeof (TStepData) == typeof (string))
        {
          this.m_stepDelegate(servicingContext, (TStepData) stepData);
        }
        else
        {
          ArgumentUtility.CheckStringForNullOrEmpty(stepData, nameof (stepData));
          TStepData stepData1 = ServicingStep.DeserializeStepData<TStepData>(stepData);
          this.m_stepDelegate(servicingContext, stepData1);
        }
      }
    }

    private class RequestContextServicingStep : 
      TeamFoundationStepPerformerBase.ServicingStepBase,
      IRequestContextServicingStep,
      IServicingStep
    {
      private readonly TeamFoundationStepPerformerBase.RequestContextStepDelegate m_stepDelegate;
      private readonly TeamFoundationStepPerformerBase.RequestContextAsyncStepDelegate m_asyncStepDelegate;

      public RequestContextServicingStep(
        string stepType,
        bool targetRequestContextOptional,
        TeamFoundationStepPerformerBase.RequestContextStepDelegate stepDelegate)
        : base(stepType)
      {
        this.m_stepDelegate = stepDelegate;
        this.TargetRequestContextOptional = targetRequestContextOptional;
      }

      public RequestContextServicingStep(
        string stepType,
        bool targetRequestContextOptional,
        TeamFoundationStepPerformerBase.RequestContextAsyncStepDelegate stepDelegate)
        : base(stepType)
      {
        this.m_asyncStepDelegate = stepDelegate;
        this.TargetRequestContextOptional = targetRequestContextOptional;
      }

      public bool TargetRequestContextOptional { get; private set; }

      public void Execute(
        IVssRequestContext targetRequestContext,
        ServicingContext servicingContext,
        string stepData)
      {
        if (!string.IsNullOrEmpty(stepData))
          throw new ArgumentException(FrameworkResources.StepDataNotRequired());
        if (this.m_stepDelegate != null)
          this.m_stepDelegate(targetRequestContext, servicingContext);
        else
          targetRequestContext.RunSynchronously((Func<Task>) (() => this.m_asyncStepDelegate(targetRequestContext, servicingContext)));
      }
    }

    private class RequestContextServicingStepWithData<TStepData> : 
      TeamFoundationStepPerformerBase.ServicingStepBase,
      IRequestContextServicingStep,
      IServicingStep
    {
      private readonly TeamFoundationStepPerformerBase.RequestContextStepDelegateWithData<TStepData> m_stepDelegate;
      private readonly TeamFoundationStepPerformerBase.RequestContextAsyncStepDelegateWithData<TStepData> m_asyncStepDelegate;

      public RequestContextServicingStepWithData(
        string stepType,
        bool targetRequestContextOptional,
        TeamFoundationStepPerformerBase.RequestContextStepDelegateWithData<TStepData> stepDelegate)
        : base(stepType)
      {
        this.TargetRequestContextOptional = targetRequestContextOptional;
        this.m_stepDelegate = stepDelegate;
      }

      public RequestContextServicingStepWithData(
        string stepType,
        bool targetRequestContextOptional,
        TeamFoundationStepPerformerBase.RequestContextAsyncStepDelegateWithData<TStepData> stepDelegate)
        : base(stepType)
      {
        this.TargetRequestContextOptional = targetRequestContextOptional;
        this.m_asyncStepDelegate = stepDelegate;
      }

      public bool TargetRequestContextOptional { get; private set; }

      public void Execute(
        IVssRequestContext targetRequestContext,
        ServicingContext servicingContext,
        string stepData)
      {
        TStepData data;
        if (typeof (TStepData) == typeof (string))
        {
          data = (TStepData) stepData;
        }
        else
        {
          ArgumentUtility.CheckStringForNullOrEmpty(stepData, nameof (stepData));
          data = ServicingStep.DeserializeStepData<TStepData>(stepData);
        }
        if (this.m_stepDelegate != null)
          this.m_stepDelegate(targetRequestContext, servicingContext, data);
        else
          targetRequestContext.RunSynchronously((Func<Task>) (() => this.m_asyncStepDelegate(targetRequestContext, servicingContext, data)));
      }
    }

    private class DatabasePartitionServicingStep : 
      TeamFoundationStepPerformerBase.ServicingStepBase,
      IPartitionServicingStep,
      IServicingStep
    {
      private readonly TeamFoundationStepPerformerBase.DatabasePartitionStepDelegate m_stepDelegate;

      public DatabasePartitionServicingStep(
        string stepType,
        TeamFoundationStepPerformerBase.DatabasePartitionStepDelegate stepDelegate)
        : base(stepType)
      {
        this.m_stepDelegate = stepDelegate;
      }

      public void Execute(int partitionId, ServicingContext servicingContext, string stepData)
      {
        if (!string.IsNullOrEmpty(stepData))
          throw new ArgumentException(FrameworkResources.StepDataNotRequired());
        this.m_stepDelegate(partitionId, servicingContext);
      }
    }

    private class DatabasePartitionServicingStepWithData<TStepData> : 
      TeamFoundationStepPerformerBase.ServicingStepBase,
      IPartitionServicingStep,
      IServicingStep
    {
      private readonly TeamFoundationStepPerformerBase.DatabasePartitionStepDelegateWithData<TStepData> m_stepDelegate;

      public DatabasePartitionServicingStepWithData(
        string stepType,
        TeamFoundationStepPerformerBase.DatabasePartitionStepDelegateWithData<TStepData> stepDelegate)
        : base(stepType)
      {
        this.m_stepDelegate = stepDelegate;
      }

      public void Execute(int partitionId, ServicingContext servicingContext, string stepData)
      {
        if (typeof (TStepData) == typeof (string))
        {
          this.m_stepDelegate(partitionId, servicingContext, (TStepData) stepData);
        }
        else
        {
          ArgumentUtility.CheckStringForNullOrEmpty(stepData, nameof (stepData));
          TStepData stepData1 = ServicingStep.DeserializeStepData<TStepData>(stepData);
          this.m_stepDelegate(partitionId, servicingContext, stepData1);
        }
      }
    }

    private delegate void BasicStepDelegate(ServicingContext servicingContext);

    private delegate void BasicStepDelegateWithData<T>(
      ServicingContext servicingContext,
      T stepData);

    private delegate void RequestContextStepDelegate(
      IVssRequestContext targetRequestContext,
      ServicingContext servicingContext);

    private delegate void RequestContextStepDelegateWithData<T>(
      IVssRequestContext targetRequestContext,
      ServicingContext servicingContext,
      T stepData);

    private delegate Task RequestContextAsyncStepDelegate(
      IVssRequestContext targetRequestContext,
      ServicingContext servicingContext);

    private delegate Task RequestContextAsyncStepDelegateWithData<T>(
      IVssRequestContext targetRequestContext,
      ServicingContext servicingContext,
      T stepData);

    private delegate void DatabasePartitionStepDelegate(
      int partitionId,
      ServicingContext servicingContext);

    private delegate void DatabasePartitionStepDelegateWithData<T>(
      int partitionId,
      ServicingContext servicingContext,
      T stepData);
  }
}
