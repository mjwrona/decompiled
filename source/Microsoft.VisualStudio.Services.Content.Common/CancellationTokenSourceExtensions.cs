// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.CancellationTokenSourceExtensions
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Collections;
using System.Reflection;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public static class CancellationTokenSourceExtensions
  {
    public static int GetCallbackCount(this CancellationTokenSource tokenSource)
    {
      Type type1 = typeof (CancellationTokenSource);
      FieldInfo field1 = type1.GetField("m_registeredCallbacksLists", BindingFlags.Instance | BindingFlags.NonPublic);
      Type type2 = type1.Assembly.GetType("System.Threading.SparselyPopulatedArray`1");
      Type type3 = type1.Assembly.GetType("System.Threading.SparselyPopulatedArrayFragment`1");
      Type type4 = type1.Assembly.GetType("System.Threading.CancellationCallbackInfo");
      Type type5 = type2.MakeGenericType(type4);
      Type type6 = type3.MakeGenericType(type4);
      FieldInfo field2 = type5.GetField("m_head", BindingFlags.Instance | BindingFlags.NonPublic);
      PropertyInfo property = type6.GetProperty("Length", BindingFlags.Instance | BindingFlags.NonPublic);
      FieldInfo field3 = type6.GetField("m_next", BindingFlags.Instance | BindingFlags.NonPublic);
      MethodInfo method = type6.GetMethod("get_Item", BindingFlags.Instance | BindingFlags.NonPublic);
      object obj1 = field1.GetValue((object) tokenSource);
      if (obj1 == null)
        return 0;
      int callbackCount = 0;
      foreach (object obj2 in (IEnumerable) obj1)
      {
        if (obj2 != null)
        {
          for (object obj3 = field2.GetValue(obj2); obj3 != null; obj3 = field3.GetValue(obj3))
          {
            int num = (int) property.GetValue(obj3);
            for (int index = 0; index < num; ++index)
            {
              if (method.Invoke(obj3, new object[1]
              {
                (object) index
              }) != null)
                ++callbackCount;
            }
          }
        }
      }
      return callbackCount;
    }
  }
}
