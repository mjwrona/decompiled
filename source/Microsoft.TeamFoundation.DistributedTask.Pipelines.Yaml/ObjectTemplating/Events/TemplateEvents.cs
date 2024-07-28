// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Events.TemplateEvents
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Events
{
  internal sealed class TemplateEvents
  {
    private Dictionary<string, List<OnMappingStartEventHandler>> m_onMappingStartListeners;
    private Dictionary<string, List<OnMappingKeyEventHandler>> m_onMappingKeyListeners;
    private Dictionary<string, List<OnMappingValueEventHandler>> m_onMappingValueListeners;
    private Dictionary<string, List<OnMappingEndEventHandler>> m_onMappingEndListeners;
    private Dictionary<string, List<OnSequenceStartEventHandler>> m_onSequenceStartListeners;
    private Dictionary<string, List<OnSequenceItemEventHandler>> m_onSequenceItemListeners;
    private Dictionary<string, List<OnSequenceEndEventHandler>> m_onSequenceEndListeners;

    public void Listen(string type, OnMappingStartEventHandler listener) => this.Add<OnMappingStartEventHandler>(ref this.m_onMappingStartListeners, type, listener);

    public void Listen(string type, OnMappingKeyEventHandler listener) => this.Add<OnMappingKeyEventHandler>(ref this.m_onMappingKeyListeners, type, listener);

    public void Listen(string type, OnMappingValueEventHandler listener) => this.Add<OnMappingValueEventHandler>(ref this.m_onMappingValueListeners, type, listener);

    public void Listen(string type, OnMappingEndEventHandler listener) => this.Add<OnMappingEndEventHandler>(ref this.m_onMappingEndListeners, type, listener);

    public void Listen(string type, OnSequenceStartEventHandler listener) => this.Add<OnSequenceStartEventHandler>(ref this.m_onSequenceStartListeners, type, listener);

    public void Listen(string type, OnSequenceItemEventHandler listener) => this.Add<OnSequenceItemEventHandler>(ref this.m_onSequenceItemListeners, type, listener);

    public void Listen(string type, OnSequenceEndEventHandler listener) => this.Add<OnSequenceEndEventHandler>(ref this.m_onSequenceEndListeners, type, listener);

    internal void RaiseOnMappingStart(TemplateContext context, string type, MappingToken mapping)
    {
      try
      {
        List<OnMappingStartEventHandler> startEventHandlerList;
        if (this.m_onMappingStartListeners == null || !this.m_onMappingStartListeners.TryGetValue(type, out startEventHandlerList))
          return;
        foreach (OnMappingStartEventHandler startEventHandler in startEventHandlerList)
          startEventHandler((object) null, new OnMappingStartEventArgs(context, mapping));
      }
      catch (Exception ex)
      {
        context.Error((TemplateToken) mapping, ex);
        throw;
      }
    }

    internal void RaiseOnMappingKey(
      TemplateContext context,
      string type,
      MappingToken mapping,
      LiteralToken key)
    {
      try
      {
        List<OnMappingKeyEventHandler> mappingKeyEventHandlerList;
        if (this.m_onMappingKeyListeners == null || !this.m_onMappingKeyListeners.TryGetValue(type, out mappingKeyEventHandlerList))
          return;
        foreach (OnMappingKeyEventHandler mappingKeyEventHandler in mappingKeyEventHandlerList)
          mappingKeyEventHandler((object) null, new OnMappingKeyEventArgs(context, mapping, key));
      }
      catch (Exception ex)
      {
        context.Error((TemplateToken) key, ex);
        throw;
      }
    }

    internal void RaiseOnMappingValue(
      TemplateContext context,
      string type,
      MappingToken mapping,
      LiteralToken key,
      TemplateToken value)
    {
      try
      {
        List<OnMappingValueEventHandler> valueEventHandlerList;
        if (this.m_onMappingValueListeners == null || !this.m_onMappingValueListeners.TryGetValue(type, out valueEventHandlerList))
          return;
        foreach (OnMappingValueEventHandler valueEventHandler in valueEventHandlerList)
          valueEventHandler((object) null, new OnMappingValueEventArgs(context, mapping, key, value));
      }
      catch (Exception ex)
      {
        context.Error(value, ex);
        throw;
      }
    }

    internal void RaiseOnMappingEnd(TemplateContext context, string type, MappingToken mapping)
    {
      try
      {
        List<OnMappingEndEventHandler> mappingEndEventHandlerList;
        if (this.m_onMappingEndListeners == null || !this.m_onMappingEndListeners.TryGetValue(type, out mappingEndEventHandlerList))
          return;
        foreach (OnMappingEndEventHandler mappingEndEventHandler in mappingEndEventHandlerList)
          mappingEndEventHandler((object) null, new OnMappingEndEventArgs(context, mapping));
      }
      catch (Exception ex)
      {
        context.Error((TemplateToken) mapping, ex);
        throw;
      }
    }

    internal void RaiseOnSequenceStart(
      TemplateContext context,
      string type,
      SequenceToken sequence)
    {
      try
      {
        List<OnSequenceStartEventHandler> startEventHandlerList;
        if (this.m_onSequenceStartListeners == null || !this.m_onSequenceStartListeners.TryGetValue(type, out startEventHandlerList))
          return;
        foreach (OnSequenceStartEventHandler startEventHandler in startEventHandlerList)
          startEventHandler((object) null, new OnSequenceStartEventArgs(context, sequence));
      }
      catch (Exception ex)
      {
        context.Error((TemplateToken) sequence, ex);
        throw;
      }
    }

    internal void RaiseOnSequenceItem(
      TemplateContext context,
      string type,
      SequenceToken sequence,
      TemplateToken item)
    {
      try
      {
        List<OnSequenceItemEventHandler> itemEventHandlerList;
        if (this.m_onSequenceItemListeners == null || !this.m_onSequenceItemListeners.TryGetValue(type, out itemEventHandlerList))
          return;
        foreach (OnSequenceItemEventHandler itemEventHandler in itemEventHandlerList)
          itemEventHandler((object) null, new OnSequenceItemEventArgs(context, sequence, item));
      }
      catch (Exception ex)
      {
        context.Error(item, ex);
        throw;
      }
    }

    internal void RaiseOnSequenceEnd(TemplateContext context, string type, SequenceToken sequence)
    {
      try
      {
        List<OnSequenceEndEventHandler> sequenceEndEventHandlerList;
        if (this.m_onSequenceEndListeners == null || !this.m_onSequenceEndListeners.TryGetValue(type, out sequenceEndEventHandlerList))
          return;
        foreach (OnSequenceEndEventHandler sequenceEndEventHandler in sequenceEndEventHandlerList)
          sequenceEndEventHandler((object) null, new OnSequenceEndEventArgs(context, sequence));
      }
      catch (Exception ex)
      {
        context.Error((TemplateToken) sequence, ex);
        throw;
      }
    }

    private void Add<T>(ref Dictionary<string, List<T>> dictionary, string type, T listener)
    {
      if (dictionary == null)
        dictionary = new Dictionary<string, List<T>>((IEqualityComparer<string>) StringComparer.Ordinal);
      List<T> objList;
      if (!dictionary.TryGetValue(type, out objList))
      {
        objList = new List<T>();
        dictionary.Add(type, objList);
      }
      objList.Add(listener);
    }
  }
}
