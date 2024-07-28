// Decompiled with JetBrains decompiler
// Type: Azure.Core.Diagnostics.AzureEventSource
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;


#nullable enable
namespace Azure.Core.Diagnostics
{
  internal abstract class AzureEventSource : EventSource
  {
    private const string SharedDataKey = "_AzureEventSourceNamesInUse";
    private static readonly HashSet<string> NamesInUse;
    private static readonly string[] MainEventSourceTraits = new string[2]
    {
      nameof (AzureEventSource),
      "true"
    };

    static AzureEventSource()
    {
      if (!(AppDomain.CurrentDomain.GetData("_AzureEventSourceNamesInUse") is HashSet<string> data))
      {
        data = new HashSet<string>();
        AppDomain.CurrentDomain.SetData("_AzureEventSourceNamesInUse", (object) data);
      }
      AzureEventSource.NamesInUse = data;
    }

    protected AzureEventSource(string eventSourceName)
      : base(AzureEventSource.DeduplicateName(eventSourceName), EventSourceSettings.Default, AzureEventSource.MainEventSourceTraits)
    {
    }

    private static string DeduplicateName(string eventSourceName)
    {
      try
      {
        lock (AzureEventSource.NamesInUse)
        {
          foreach (EventSource source in EventSource.GetSources())
            AzureEventSource.NamesInUse.Add(source.Name);
          if (!AzureEventSource.NamesInUse.Contains(eventSourceName))
          {
            AzureEventSource.NamesInUse.Add(eventSourceName);
            return eventSourceName;
          }
          int num = 1;
          string str;
          while (true)
          {
            str = string.Format("{0}-{1}", (object) eventSourceName, (object) num);
            if (AzureEventSource.NamesInUse.Contains(str))
              ++num;
            else
              break;
          }
          AzureEventSource.NamesInUse.Add(str);
          return str;
        }
      }
      catch (NotImplementedException ex)
      {
      }
      return eventSourceName;
    }
  }
}
