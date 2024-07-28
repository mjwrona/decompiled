// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.TaskStoreFeatureFlags
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  internal class TaskStoreFeatureFlags
  {
    private static readonly bool UseNode16BuildConfig = false;
    private static readonly bool UseNode16_225BuildConfig = false;
    private static readonly bool UseNode20_225BuildConfig = false;
    private static readonly bool UseNode20_228BuildConfig = false;
    private static readonly bool UseNode20_229BuildConfig1 = false;
    private static readonly bool UseNode20_229BuildConfig2 = false;
    private static readonly bool UseNode20_229BuildConfig3 = false;
    private static readonly bool UseNode20_229BuildConfig4 = false;
    private static readonly bool UseNode20_229BuildConfig5 = false;
    private static readonly bool UseNode20_229BuildConfig6 = false;
    private static readonly bool UseNode20_229BuildConfig7 = false;
    private static readonly bool UseNode20_229BuildConfig8 = false;
    private static readonly bool UseNode20_229BuildConfig9 = false;
    private static readonly bool UseNode20_229BuildConfig10 = false;
    private static readonly bool UseNode20_229BuildConfig11 = false;
    private static readonly bool UseNode20_229BuildConfig12 = false;
    private static readonly bool UseNode20_229BuildConfig13 = false;
    private static readonly bool UseNode20_229BuildConfig14 = false;
    private static readonly string UseNode16BuildConfigWhereAvailable = "DistributedTask.UseNode16BuildConfigWhereAvailable";
    private static readonly string UseNode16_226BuildConfigWhereAvailable = "DistributedTask.UseNode16Sprint225BuildConfigWhereAvailable";
    private static readonly string UseNode20_225BuildConfigWhereAvailable = "DistributedTask.UseNode20Sprint225BuildConfigWhereAvailable";
    private static readonly string UseNode20_228BuildConfigWhereAvailable = "DistributedTask.UseNode20Sprint228BuildConfigWhereAvailable";
    private static readonly string UseNode20_229BuildConfigWhereAvailable1 = "DistributedTask.UseNode20Sprint229BuildConfigWhereAvailable1";
    private static readonly string UseNode20_229BuildConfigWhereAvailable2 = "DistributedTask.UseNode20Sprint229BuildConfigWhereAvailable2";
    private static readonly string UseNode20_229BuildConfigWhereAvailable3 = "DistributedTask.UseNode20Sprint229BuildConfigWhereAvailable3";
    private static readonly string UseNode20_229BuildConfigWhereAvailable4 = "DistributedTask.UseNode20Sprint229BuildConfigWhereAvailable4";
    private static readonly string UseNode20_229BuildConfigWhereAvailable5 = "DistributedTask.UseNode20Sprint229BuildConfigWhereAvailable5";
    private static readonly string UseNode20_229BuildConfigWhereAvailable6 = "DistributedTask.UseNode20Sprint229BuildConfigWhereAvailable6";
    private static readonly string UseNode20_229BuildConfigWhereAvailable7 = "DistributedTask.UseNode20Sprint229BuildConfigWhereAvailable7";
    private static readonly string UseNode20_229BuildConfigWhereAvailable8 = "DistributedTask.UseNode20Sprint229BuildConfigWhereAvailable8";
    private static readonly string UseNode20_229BuildConfigWhereAvailable9 = "DistributedTask.UseNode20Sprint229BuildConfigWhereAvailable9";
    private static readonly string UseNode20_229BuildConfigWhereAvailable10 = "DistributedTask.UseNode20Sprint229BuildConfigWhereAvailable10";
    private static readonly string UseNode20_229BuildConfigWhereAvailable11 = "DistributedTask.UseNode20Sprint229BuildConfigWhereAvailable11";
    private static readonly string UseNode20_229BuildConfigWhereAvailable12 = "DistributedTask.UseNode20Sprint229BuildConfigWhereAvailable12";
    private static readonly string UseNode20_229BuildConfigWhereAvailable13 = "DistributedTask.UseNode20Sprint229BuildConfigWhereAvailable13";
    private static readonly string UseNode20_229BuildConfigWhereAvailable14 = "DistributedTask.UseNode20Sprint229BuildConfigWhereAvailable14";

    public TaskStoreFeatureFlags(
      Func<string, bool> featureCallbackUsedOnCreationNoCapture)
    {
      this.UseNode16BuildConfigState = featureCallbackUsedOnCreationNoCapture == null ? TaskStoreFeatureFlags.UseNode16BuildConfig : featureCallbackUsedOnCreationNoCapture(TaskStoreFeatureFlags.UseNode16BuildConfigWhereAvailable);
      this.UseNode16_225BuildConfigState = featureCallbackUsedOnCreationNoCapture == null ? TaskStoreFeatureFlags.UseNode16_225BuildConfig : featureCallbackUsedOnCreationNoCapture(TaskStoreFeatureFlags.UseNode16_226BuildConfigWhereAvailable);
      this.UseNode20_225BuildConfigState = featureCallbackUsedOnCreationNoCapture == null ? TaskStoreFeatureFlags.UseNode20_225BuildConfig : featureCallbackUsedOnCreationNoCapture(TaskStoreFeatureFlags.UseNode20_225BuildConfigWhereAvailable);
      this.UseNode20_228BuildConfigState = featureCallbackUsedOnCreationNoCapture == null ? TaskStoreFeatureFlags.UseNode20_228BuildConfig : featureCallbackUsedOnCreationNoCapture(TaskStoreFeatureFlags.UseNode20_228BuildConfigWhereAvailable);
      this.UseNode20_229BuildConfigState1 = featureCallbackUsedOnCreationNoCapture == null ? TaskStoreFeatureFlags.UseNode20_229BuildConfig1 : featureCallbackUsedOnCreationNoCapture(TaskStoreFeatureFlags.UseNode20_229BuildConfigWhereAvailable1);
      this.UseNode20_229BuildConfigState2 = featureCallbackUsedOnCreationNoCapture == null ? TaskStoreFeatureFlags.UseNode20_229BuildConfig2 : featureCallbackUsedOnCreationNoCapture(TaskStoreFeatureFlags.UseNode20_229BuildConfigWhereAvailable2);
      this.UseNode20_229BuildConfigState3 = featureCallbackUsedOnCreationNoCapture == null ? TaskStoreFeatureFlags.UseNode20_229BuildConfig3 : featureCallbackUsedOnCreationNoCapture(TaskStoreFeatureFlags.UseNode20_229BuildConfigWhereAvailable3);
      this.UseNode20_229BuildConfigState4 = featureCallbackUsedOnCreationNoCapture == null ? TaskStoreFeatureFlags.UseNode20_229BuildConfig4 : featureCallbackUsedOnCreationNoCapture(TaskStoreFeatureFlags.UseNode20_229BuildConfigWhereAvailable4);
      this.UseNode20_229BuildConfigState5 = featureCallbackUsedOnCreationNoCapture == null ? TaskStoreFeatureFlags.UseNode20_229BuildConfig5 : featureCallbackUsedOnCreationNoCapture(TaskStoreFeatureFlags.UseNode20_229BuildConfigWhereAvailable5);
      this.UseNode20_229BuildConfigState6 = featureCallbackUsedOnCreationNoCapture == null ? TaskStoreFeatureFlags.UseNode20_229BuildConfig6 : featureCallbackUsedOnCreationNoCapture(TaskStoreFeatureFlags.UseNode20_229BuildConfigWhereAvailable6);
      this.UseNode20_229BuildConfigState7 = featureCallbackUsedOnCreationNoCapture == null ? TaskStoreFeatureFlags.UseNode20_229BuildConfig7 : featureCallbackUsedOnCreationNoCapture(TaskStoreFeatureFlags.UseNode20_229BuildConfigWhereAvailable7);
      this.UseNode20_229BuildConfigState8 = featureCallbackUsedOnCreationNoCapture == null ? TaskStoreFeatureFlags.UseNode20_229BuildConfig8 : featureCallbackUsedOnCreationNoCapture(TaskStoreFeatureFlags.UseNode20_229BuildConfigWhereAvailable8);
      this.UseNode20_229BuildConfigState9 = featureCallbackUsedOnCreationNoCapture == null ? TaskStoreFeatureFlags.UseNode20_229BuildConfig9 : featureCallbackUsedOnCreationNoCapture(TaskStoreFeatureFlags.UseNode20_229BuildConfigWhereAvailable9);
      this.UseNode20_229BuildConfigState10 = featureCallbackUsedOnCreationNoCapture == null ? TaskStoreFeatureFlags.UseNode20_229BuildConfig10 : featureCallbackUsedOnCreationNoCapture(TaskStoreFeatureFlags.UseNode20_229BuildConfigWhereAvailable10);
      this.UseNode20_229BuildConfigState11 = featureCallbackUsedOnCreationNoCapture == null ? TaskStoreFeatureFlags.UseNode20_229BuildConfig11 : featureCallbackUsedOnCreationNoCapture(TaskStoreFeatureFlags.UseNode20_229BuildConfigWhereAvailable11);
      this.UseNode20_229BuildConfigState12 = featureCallbackUsedOnCreationNoCapture == null ? TaskStoreFeatureFlags.UseNode20_229BuildConfig12 : featureCallbackUsedOnCreationNoCapture(TaskStoreFeatureFlags.UseNode20_229BuildConfigWhereAvailable12);
      this.UseNode20_229BuildConfigState13 = featureCallbackUsedOnCreationNoCapture == null ? TaskStoreFeatureFlags.UseNode20_229BuildConfig13 : featureCallbackUsedOnCreationNoCapture(TaskStoreFeatureFlags.UseNode20_229BuildConfigWhereAvailable13);
      this.UseNode20_229BuildConfigState14 = featureCallbackUsedOnCreationNoCapture == null ? TaskStoreFeatureFlags.UseNode20_229BuildConfig14 : featureCallbackUsedOnCreationNoCapture(TaskStoreFeatureFlags.UseNode20_229BuildConfigWhereAvailable14);
    }

    internal TaskStoreFeatureFlags(
      bool useNode16BuildConfigState,
      bool useNode16_225BuildConfigState,
      bool useNode20_225BuildConfigState,
      bool useNode20_228BuildConfigState,
      bool useNode20_229BuildConfigState1,
      bool useNode20_229BuildConfigState2,
      bool useNode20_229BuildConfigState3,
      bool useNode20_229BuildConfigState4,
      bool useNode20_229BuildConfigState5,
      bool useNode20_229BuildConfigState6,
      bool useNode20_229BuildConfigState7,
      bool useNode20_229BuildConfigState8,
      bool useNode20_229BuildConfigState9,
      bool useNode20_229BuildConfigState10,
      bool useNode20_229BuildConfigState11,
      bool useNode20_229BuildConfigState12,
      bool useNode20_229BuildConfigState13,
      bool useNode20_229BuildConfigState14)
    {
      this.UseNode16BuildConfigState = useNode16BuildConfigState;
      this.UseNode16_225BuildConfigState = useNode16_225BuildConfigState;
      this.UseNode20_225BuildConfigState = useNode20_225BuildConfigState;
      this.UseNode20_228BuildConfigState = useNode20_228BuildConfigState;
      this.UseNode20_229BuildConfigState1 = useNode20_229BuildConfigState1;
      this.UseNode20_229BuildConfigState2 = useNode20_229BuildConfigState2;
      this.UseNode20_229BuildConfigState3 = useNode20_229BuildConfigState3;
      this.UseNode20_229BuildConfigState4 = useNode20_229BuildConfigState4;
      this.UseNode20_229BuildConfigState5 = useNode20_229BuildConfigState5;
      this.UseNode20_229BuildConfigState6 = useNode20_229BuildConfigState6;
      this.UseNode20_229BuildConfigState7 = useNode20_229BuildConfigState7;
      this.UseNode20_229BuildConfigState8 = useNode20_229BuildConfigState8;
      this.UseNode20_229BuildConfigState9 = useNode20_229BuildConfigState9;
      this.UseNode20_229BuildConfigState10 = useNode20_229BuildConfigState10;
      this.UseNode20_229BuildConfigState11 = useNode20_229BuildConfigState11;
      this.UseNode20_229BuildConfigState12 = useNode20_229BuildConfigState12;
      this.UseNode20_229BuildConfigState13 = useNode20_229BuildConfigState13;
      this.UseNode20_229BuildConfigState14 = useNode20_229BuildConfigState14;
    }

    public bool UseNode16BuildConfigState { get; }

    public bool UseNode16_225BuildConfigState { get; }

    public bool UseNode20_225BuildConfigState { get; }

    public bool UseNode20_228BuildConfigState { get; }

    public bool UseNode20_229BuildConfigState1 { get; }

    public bool UseNode20_229BuildConfigState2 { get; }

    public bool UseNode20_229BuildConfigState3 { get; }

    public bool UseNode20_229BuildConfigState4 { get; }

    public bool UseNode20_229BuildConfigState5 { get; }

    public bool UseNode20_229BuildConfigState6 { get; }

    public bool UseNode20_229BuildConfigState7 { get; }

    public bool UseNode20_229BuildConfigState8 { get; }

    public bool UseNode20_229BuildConfigState9 { get; }

    public bool UseNode20_229BuildConfigState10 { get; }

    public bool UseNode20_229BuildConfigState11 { get; }

    public bool UseNode20_229BuildConfigState12 { get; }

    public bool UseNode20_229BuildConfigState13 { get; }

    public bool UseNode20_229BuildConfigState14 { get; }
  }
}
