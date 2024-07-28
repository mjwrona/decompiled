// Decompiled with JetBrains decompiler
// Type: Nest.CatNodesRecord
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class CatNodesRecord : ICatRecord
  {
    public string Build => this._b ?? this._build;

    public string CompletionSize => this._completionSize ?? this._cs ?? this._completion_size;

    [DataMember(Name = "cpu")]
    public string CPU { get; internal set; }

    public string DiskAvailable => this._diskAvail ?? this._disk ?? this._d ?? this._disk_avail;

    public string FielddataEvictions => this._fielddataEvictions ?? this._fe ?? this._fielddata_evictions;

    public string FielddataMemory => this._fielddataMemory ?? this._fm ?? this._fielddata_memory_size;

    public int? FileDescriptorCurrent => this._fileDescriptorCurrent ?? this._fdc ?? this._file_desc_current;

    public int? FileDescriptorMax => this._fileDescriptorMax ?? this._fdm ?? this._file_desc_max;

    public int? FileDescriptorPercent => this._fileDescriptorPercent ?? this._fdp ?? this._file_desc_percent;

    public string FilterCacheEvictions => this._filterCacheEvictions ?? this._fce ?? this._filter_cache_evictions;

    public string FilterCacheMemory => this._filterCacheMemory ?? this._fcm ?? this._filter_cache_memory_size;

    public string FlushTotal => this._flushTotal ?? this._ft ?? this._flush_total;

    public string FlushTotalTime => this._flushTotalTime ?? this._ftt ?? this._flush_total_time;

    public string GetCurrent => this._getCurrent ?? this._gc ?? this._get_current;

    public string GetExistsTime => this._getExistsTime ?? this._geti ?? this._get_exists_time;

    public string GetExistsTotal => this._getExistsTotal ?? this._geto ?? this._get_exists_total;

    public string GetMissingTime => this._getMissingTime ?? this._gmti ?? this._get_missing_time;

    public string GetMissingTotal => this._getMissingTotal ?? this._gmto ?? this._get_missing_total;

    public string GetTime => this._getTime ?? this._gti ?? this._get_time;

    public string GetTotal => this._getTotal ?? this._gto ?? this._get_total;

    public string HeapCurrent => this._heapCurrent ?? this._hc ?? this._heap_current;

    public string HeapMax => this._heapMax ?? this._hm ?? this._heap_max;

    public string HeapPercent => this._heapPercent ?? this._hp ?? this._heap_percent;

    public string IdCacheMemory => this._idCacheMemory ?? this._im ?? this._id_cache_memory_size;

    public string IndexingDeleteCurrent => this._indexingDeleteCurrent ?? this._idcs ?? this._indexing_delete_current;

    public string IndexingDeleteTime => this._indexingDeleteTime ?? this._idti ?? this._indexing_delete_time;

    public string IndexingDeleteTotal => this._indexingDeleteTotal ?? this._idto ?? this._indexing_delete_total;

    public string IndexingIndexCurrent => this._indexingIndexCurrent ?? this._iic ?? this._indexing_index_current;

    public string IndexingIndexTime => this._indexingIndexTime ?? this._iiti ?? this._indexing_index_time;

    public string IndexingIndexTotal => this._indexingIndexTotal ?? this._iito ?? this._indexing_index_total;

    public string Ip => this._i ?? this._ip;

    public string Jdk => this._j ?? this._jdk;

    [DataMember(Name = "load_15m")]
    public string LoadFifteenMinute { get; internal set; }

    [DataMember(Name = "load_5m")]
    public string LoadFiveMinute { get; internal set; }

    [DataMember(Name = "load_1m")]
    public string LoadOneMinute { get; internal set; }

    public string Master => this._m ?? this._master;

    public string MergesCurrent => this._mergesCurrent ?? this._mc ?? this._merges_current;

    public string MergesCurrentDocs => this._mergesCurrentDocs ?? this._mcd ?? this._merges_current_docs;

    public string MergesCurrentSize => this._mergesCurrentSize ?? this._mcs ?? this._merges_current_size;

    public string MergesTotal => this._mergesTotal ?? this._mt ?? this._merges_total;

    public string MergesTotalDocs => this._mergesTotalDocs ?? this._mtd ?? this._merges_total_docs;

    public string MergesTotalTime => this._mergesTotalTime ?? this._mtt ?? this._merges_total_time;

    public string Name => this._n ?? this._name;

    public string NodeId => this._id ?? this._nodeId;

    public string NodeRole => this._nodeRole ?? this._data_client ?? this._dc ?? this._r ?? this._node_role;

    public string PercolateCurrent => this._percolateCurrent ?? this._pc ?? this._percolate_current;

    public string PercolateMemory => this._percolateMemory ?? this._pm ?? this._percolate_memory_size;

    public string PercolateQueries => this._percolate_queries ?? this._pq ?? this._percolate_queries;

    public string PercolateTime => this._percolateTime ?? this._pti ?? this._percolate_time;

    public string PercolateTotal => this._percolateTotal ?? this._pto ?? this._percolate_total;

    public string Pid => this._p ?? this._pid;

    public string Port => this._po ?? this._port;

    public string QueryCacheMemorySize => this._queryCacheMemory ?? this._qcm ?? this._query_cache_memory_size;

    public string QueryCacheEvictions => this._queryCacheEvictions ?? this._qce ?? this._query_cache_evictions;

    public string QueryCacheHitCount => this._queryCacheHitCount ?? this._qchc ?? this._query_cache_hit_count;

    public string QueryCacheMissCount => this._queryCacheMissCount ?? this._qcmc ?? this._query_cache_miss_count;

    public string RamCurrent => this._ramCurrent ?? this._rc ?? this._ram_current;

    public string RamMax => this._ramMax ?? this._rm ?? this._ram_max;

    public string RamPercent => this._ramPercent ?? this._rp ?? this._ram_percent;

    public string RefreshTime => this._refreshTime ?? this._rti ?? this._refreshTime;

    public string RefreshTotal => this._refreshTotal ?? this._rto ?? this._refresh_total;

    public string SearchFetchCurrent => this._searchFetchCurrent ?? this._sfc ?? this._search_fetch_current;

    public string SearchFetchTime => this._searchFetchTime ?? this._sfti ?? this._search_fetch_time;

    public string SearchFetchTotal => this._searchFetchTotal ?? this._sfto ?? this._searchFetchTotal;

    public string SearchOpenContexts => this._searchOpenContexts ?? this._so ?? this._search_open_contexts;

    public string SearchQueryCurrent => this._searchQueryCurrent ?? this._sqc ?? this._search_query_current;

    public string SearchQueryTime => this._searchQueryTime ?? this._sqti ?? this._search_query_time;

    public string SearchQueryTotal => this._searchQueryTotal ?? this._sqto ?? this._search_query_total;

    public string SegmentsCount => this._segmentsCount ?? this._sc ?? this._segmentsCount;

    public string SegmentsIndexWriterMaxMemory => this._segmentsIndexWriterMaxMemory ?? this._siwmx ?? this._segments_index_writer_max_memory;

    public string SegmentsIndexWriterMemory => this._segmentsIndexWriterMemory ?? this._siwm ?? this._segments_index_writer_memory;

    public string SegmentsMemory => this._segmentsMemory ?? this._sm ?? this._segments_memory;

    public string SegmentsVersionMapMemory => this._segmentsVersionMapMemory ?? this._svmm ?? this._segments_version_map_memory;

    public string Uptime => this._u ?? this._uptime;

    public string Version => this._v ?? this._version;

    [DataMember(Name = "b")]
    internal string _b { get; set; }

    [DataMember(Name = "build")]
    internal string _build { get; set; }

    [DataMember(Name = "completion.size")]
    internal string _completion_size { get; set; }

    [DataMember(Name = "completionSize")]
    internal string _completionSize { get; set; }

    [DataMember(Name = "cs")]
    internal string _cs { get; set; }

    [DataMember(Name = "d")]
    internal string _d { get; set; }

    [DataMember(Name = "data/client")]
    internal string _data_client { get; set; }

    [DataMember(Name = "dc")]
    internal string _dc { get; set; }

    [DataMember(Name = "disk")]
    internal string _disk { get; set; }

    [DataMember(Name = "disk.avail")]
    internal string _disk_avail { get; set; }

    [DataMember(Name = "diskAvail")]
    internal string _diskAvail { get; set; }

    [DataMember(Name = "fce")]
    internal string _fce { get; set; }

    [DataMember(Name = "fcm")]
    internal string _fcm { get; set; }

    [DataMember(Name = "fdc")]
    internal int? _fdc { get; set; }

    [DataMember(Name = "fdm")]
    internal int? _fdm { get; set; }

    [DataMember(Name = "fdp")]
    internal int? _fdp { get; set; }

    [DataMember(Name = "fe")]
    internal string _fe { get; set; }

    [DataMember(Name = "fielddata.evictions")]
    internal string _fielddata_evictions { get; set; }

    [DataMember(Name = "fielddata.memory_size")]
    internal string _fielddata_memory_size { get; set; }

    [DataMember(Name = "fielddataEvictions")]
    internal string _fielddataEvictions { get; set; }

    [DataMember(Name = "fielddataMemory")]
    internal string _fielddataMemory { get; set; }

    [DataMember(Name = "file_desc.current")]
    internal int? _file_desc_current { get; set; }

    [DataMember(Name = "file_desc.max")]
    internal int? _file_desc_max { get; set; }

    [DataMember(Name = "file_desc.percent")]
    internal int? _file_desc_percent { get; set; }

    [DataMember(Name = "fileDescriptorCurrent")]
    internal int? _fileDescriptorCurrent { get; set; }

    [DataMember(Name = "fileDescriptorMax")]
    internal int? _fileDescriptorMax { get; set; }

    [DataMember(Name = "fileDescriptorPercent")]
    internal int? _fileDescriptorPercent { get; set; }

    [DataMember(Name = "filter_cache.evictions")]
    internal string _filter_cache_evictions { get; set; }

    [DataMember(Name = "filter_cache.memory_size")]
    internal string _filter_cache_memory_size { get; set; }

    [DataMember(Name = "filterCacheEvictions")]
    internal string _filterCacheEvictions { get; set; }

    [DataMember(Name = "filterCacheMemory")]
    internal string _filterCacheMemory { get; set; }

    [DataMember(Name = "flush.total")]
    internal string _flush_total { get; set; }

    [DataMember(Name = "flush.total_time")]
    internal string _flush_total_time { get; set; }

    [DataMember(Name = "flushTotal")]
    internal string _flushTotal { get; set; }

    [DataMember(Name = "flushTotalTime")]
    internal string _flushTotalTime { get; set; }

    [DataMember(Name = "fm")]
    internal string _fm { get; set; }

    [DataMember(Name = "ft")]
    internal string _ft { get; set; }

    [DataMember(Name = "ftt")]
    internal string _ftt { get; set; }

    [DataMember(Name = "gc")]
    internal string _gc { get; set; }

    [DataMember(Name = "get.current")]
    internal string _get_current { get; set; }

    [DataMember(Name = "get.exists_time")]
    internal string _get_exists_time { get; set; }

    [DataMember(Name = "get.exists_total")]
    internal string _get_exists_total { get; set; }

    [DataMember(Name = "get.missing_time")]
    internal string _get_missing_time { get; set; }

    [DataMember(Name = "get.missing_total")]
    internal string _get_missing_total { get; set; }

    [DataMember(Name = "get.time")]
    internal string _get_time { get; set; }

    [DataMember(Name = "get.total")]
    internal string _get_total { get; set; }

    [DataMember(Name = "getCurrent")]
    internal string _getCurrent { get; set; }

    [DataMember(Name = "getExistsTime")]
    internal string _getExistsTime { get; set; }

    [DataMember(Name = "getExistsTotal")]
    internal string _getExistsTotal { get; set; }

    [DataMember(Name = "geti")]
    internal string _geti { get; set; }

    [DataMember(Name = "getMissingTime")]
    internal string _getMissingTime { get; set; }

    [DataMember(Name = "getMissingTotal")]
    internal string _getMissingTotal { get; set; }

    [DataMember(Name = "geto")]
    internal string _geto { get; set; }

    [DataMember(Name = "getTime")]
    internal string _getTime { get; set; }

    [DataMember(Name = "getTotal")]
    internal string _getTotal { get; set; }

    [DataMember(Name = "gmti")]
    internal string _gmti { get; set; }

    [DataMember(Name = "gmto")]
    internal string _gmto { get; set; }

    [DataMember(Name = "gti")]
    internal string _gti { get; set; }

    [DataMember(Name = "gto")]
    internal string _gto { get; set; }

    [DataMember(Name = "hc")]
    internal string _hc { get; set; }

    [DataMember(Name = "heap.current")]
    internal string _heap_current { get; set; }

    [DataMember(Name = "heap.max")]
    internal string _heap_max { get; set; }

    [DataMember(Name = "heap.percent")]
    internal string _heap_percent { get; set; }

    [DataMember(Name = "heapCurrent")]
    internal string _heapCurrent { get; set; }

    [DataMember(Name = "heapMax")]
    internal string _heapMax { get; set; }

    [DataMember(Name = "heapPercent")]
    internal string _heapPercent { get; set; }

    [DataMember(Name = "hm")]
    internal string _hm { get; set; }

    [DataMember(Name = "hp")]
    internal string _hp { get; set; }

    [DataMember(Name = "i")]
    internal string _i { get; set; }

    [DataMember(Name = "id")]
    internal string _id { get; set; }

    [DataMember(Name = "id_cache.memory_size")]
    internal string _id_cache_memory_size { get; set; }

    [DataMember(Name = "idCacheMemory")]
    internal string _idCacheMemory { get; set; }

    [DataMember(Name = "idc")]
    internal string _idcs { get; set; }

    [DataMember(Name = "idti")]
    internal string _idti { get; set; }

    [DataMember(Name = "idto")]
    internal string _idto { get; set; }

    [DataMember(Name = "iic")]
    internal string _iic { get; set; }

    [DataMember(Name = "iiti")]
    internal string _iiti { get; set; }

    [DataMember(Name = "iito")]
    internal string _iito { get; set; }

    [DataMember(Name = "im")]
    internal string _im { get; set; }

    [DataMember(Name = "indexing.delete_current")]
    internal string _indexing_delete_current { get; set; }

    [DataMember(Name = "indexing.delete_time")]
    internal string _indexing_delete_time { get; set; }

    [DataMember(Name = "indexing.delete_total")]
    internal string _indexing_delete_total { get; set; }

    [DataMember(Name = "indexing.index_current")]
    internal string _indexing_index_current { get; set; }

    [DataMember(Name = "indexing.index_time")]
    internal string _indexing_index_time { get; set; }

    [DataMember(Name = "indexing.index_total")]
    internal string _indexing_index_total { get; set; }

    [DataMember(Name = "indexingDeleteCurrent")]
    internal string _indexingDeleteCurrent { get; set; }

    [DataMember(Name = "indexingDeleteTime")]
    internal string _indexingDeleteTime { get; set; }

    [DataMember(Name = "indexingDeleteTotal")]
    internal string _indexingDeleteTotal { get; set; }

    [DataMember(Name = "indexingIndexCurrent")]
    internal string _indexingIndexCurrent { get; set; }

    [DataMember(Name = "indexingIndexTime")]
    internal string _indexingIndexTime { get; set; }

    [DataMember(Name = "indexingIndexTotal")]
    internal string _indexingIndexTotal { get; set; }

    [DataMember(Name = "ip")]
    internal string _ip { get; set; }

    [DataMember(Name = "j")]
    internal string _j { get; set; }

    [DataMember(Name = "jdk")]
    internal string _jdk { get; set; }

    [DataMember(Name = "m")]
    internal string _m { get; set; }

    [DataMember(Name = "master")]
    internal string _master { get; set; }

    [DataMember(Name = "mc")]
    internal string _mc { get; set; }

    [DataMember(Name = "mcd")]
    internal string _mcd { get; set; }

    [DataMember(Name = "mcs")]
    internal string _mcs { get; set; }

    [DataMember(Name = "merges.current")]
    internal string _merges_current { get; set; }

    [DataMember(Name = "merges.current_docs")]
    internal string _merges_current_docs { get; set; }

    [DataMember(Name = "merges.current_size")]
    internal string _merges_current_size { get; set; }

    [DataMember(Name = "merges.total")]
    internal string _merges_total { get; set; }

    [DataMember(Name = "merges.total_docs")]
    internal string _merges_total_docs { get; set; }

    [DataMember(Name = "merges.total_time")]
    internal string _merges_total_time { get; set; }

    [DataMember(Name = "mergesCurrent")]
    internal string _mergesCurrent { get; set; }

    [DataMember(Name = "mergesCurrentDocs")]
    internal string _mergesCurrentDocs { get; set; }

    [DataMember(Name = "mergesCurrentSize")]
    internal string _mergesCurrentSize { get; set; }

    [DataMember(Name = "mergesTotal")]
    internal string _mergesTotal { get; set; }

    [DataMember(Name = "mergesTotalDocs")]
    internal string _mergesTotalDocs { get; set; }

    [DataMember(Name = "mergesTotalTime")]
    internal string _mergesTotalTime { get; set; }

    [DataMember(Name = "mt")]
    internal string _mt { get; set; }

    [DataMember(Name = "mtd")]
    internal string _mtd { get; set; }

    [DataMember(Name = "mtt")]
    internal string _mtt { get; set; }

    [DataMember(Name = "n")]
    internal string _n { get; set; }

    [DataMember(Name = "name")]
    internal string _name { get; set; }

    [DataMember(Name = "node.role")]
    internal string _node_role { get; set; }

    [DataMember(Name = "nodeId")]
    internal string _nodeId { get; set; }

    [DataMember(Name = "nodeRole")]
    internal string _nodeRole { get; set; }

    [DataMember(Name = "p")]
    internal string _p { get; set; }

    [DataMember(Name = "pc")]
    internal string _pc { get; set; }

    [DataMember(Name = "percolate.current")]
    internal string _percolate_current { get; set; }

    [DataMember(Name = "percolate.memory_size")]
    internal string _percolate_memory_size { get; set; }

    [DataMember(Name = "percolate.queries")]
    internal string _percolate_queries { get; set; }

    [DataMember(Name = "percolate.time")]
    internal string _percolate_time { get; set; }

    [DataMember(Name = "percolate.total")]
    internal string _percolate_total { get; set; }

    [DataMember(Name = "percolateCurrent")]
    internal string _percolateCurrent { get; set; }

    [DataMember(Name = "percolateMemory")]
    internal string _percolateMemory { get; set; }

    [DataMember(Name = "percolateQueries")]
    internal string _percolateQueries { get; set; }

    [DataMember(Name = "percolateTime")]
    internal string _percolateTime { get; set; }

    [DataMember(Name = "percolateTotal")]
    internal string _percolateTotal { get; set; }

    [DataMember(Name = "pid")]
    internal string _pid { get; set; }

    [DataMember(Name = "pm")]
    internal string _pm { get; set; }

    [DataMember(Name = "po")]
    internal string _po { get; set; }

    [DataMember(Name = "port")]
    internal string _port { get; set; }

    [DataMember(Name = "pq")]
    internal string _pq { get; set; }

    [DataMember(Name = "pti")]
    internal string _pti { get; set; }

    [DataMember(Name = "pto")]
    internal string _pto { get; set; }

    [DataMember(Name = "query_cache.memory_size")]
    internal string _query_cache_memory_size { get; set; }

    [DataMember(Name = "qcm")]
    internal string _qcm { get; set; }

    [DataMember(Name = "queryCacheMemory")]
    internal string _queryCacheMemory { get; set; }

    [DataMember(Name = "query_cache.evictions")]
    internal string _query_cache_evictions { get; set; }

    [DataMember(Name = "qce")]
    internal string _qce { get; set; }

    [DataMember(Name = "queryCacheEvictions")]
    internal string _queryCacheEvictions { get; set; }

    [DataMember(Name = "query_cache.hit_count")]
    internal string _query_cache_hit_count { get; set; }

    [DataMember(Name = "qchc")]
    internal string _qchc { get; set; }

    [DataMember(Name = "queryCacheHitCount")]
    internal string _queryCacheHitCount { get; set; }

    [DataMember(Name = "query_cache.miss_count")]
    internal string _query_cache_miss_count { get; set; }

    [DataMember(Name = "qcmc")]
    internal string _qcmc { get; set; }

    [DataMember(Name = "queryCacheMissCount")]
    internal string _queryCacheMissCount { get; set; }

    [DataMember(Name = "r")]
    internal string _r { get; set; }

    [DataMember(Name = "ram.current")]
    internal string _ram_current { get; set; }

    [DataMember(Name = "ram.max")]
    internal string _ram_max { get; set; }

    [DataMember(Name = "ram.percent")]
    internal string _ram_percent { get; set; }

    [DataMember(Name = "ramCurrent")]
    internal string _ramCurrent { get; set; }

    [DataMember(Name = "ramMax")]
    internal string _ramMax { get; set; }

    [DataMember(Name = "ramPercent")]
    internal string _ramPercent { get; set; }

    [DataMember(Name = "rc")]
    internal string _rc { get; set; }

    [DataMember(Name = "refresh.time")]
    internal string _refresh_time { get; set; }

    [DataMember(Name = "refresh.total")]
    internal string _refresh_total { get; set; }

    [DataMember(Name = "refreshTime")]
    internal string _refreshTime { get; set; }

    [DataMember(Name = "refreshTotal")]
    internal string _refreshTotal { get; set; }

    [DataMember(Name = "rm")]
    internal string _rm { get; set; }

    [DataMember(Name = "rp")]
    internal string _rp { get; set; }

    [DataMember(Name = "rti")]
    internal string _rti { get; set; }

    [DataMember(Name = "rto")]
    internal string _rto { get; set; }

    [DataMember(Name = "sc")]
    internal string _sc { get; set; }

    [DataMember(Name = "search.fetch_current")]
    internal string _search_fetch_current { get; set; }

    [DataMember(Name = "search.fetch_time")]
    internal string _search_fetch_time { get; set; }

    [DataMember(Name = "search.fetch_total")]
    internal string _search_fetch_total { get; set; }

    [DataMember(Name = "search.open_contexts")]
    internal string _search_open_contexts { get; set; }

    [DataMember(Name = "search.query_current")]
    internal string _search_query_current { get; set; }

    [DataMember(Name = "search.query_time")]
    internal string _search_query_time { get; set; }

    [DataMember(Name = "search.query_total")]
    internal string _search_query_total { get; set; }

    [DataMember(Name = "searchFetchCurrent")]
    internal string _searchFetchCurrent { get; set; }

    [DataMember(Name = "searchFetchTime")]
    internal string _searchFetchTime { get; set; }

    [DataMember(Name = "searchFetchTotal")]
    internal string _searchFetchTotal { get; set; }

    [DataMember(Name = "searchOpenContexts")]
    internal string _searchOpenContexts { get; set; }

    [DataMember(Name = "searchQueryCurrent")]
    internal string _searchQueryCurrent { get; set; }

    [DataMember(Name = "searchQueryTime")]
    internal string _searchQueryTime { get; set; }

    [DataMember(Name = "searchQueryTotal")]
    internal string _searchQueryTotal { get; set; }

    [DataMember(Name = "segments.count")]
    internal string _segments_count { get; set; }

    [DataMember(Name = "segments.index_writer_max_memory")]
    internal string _segments_index_writer_max_memory { get; set; }

    [DataMember(Name = "segments.index_writer_memory")]
    internal string _segments_index_writer_memory { get; set; }

    [DataMember(Name = "segments.memory")]
    internal string _segments_memory { get; set; }

    [DataMember(Name = "segments.version_map_memory")]
    internal string _segments_version_map_memory { get; set; }

    [DataMember(Name = "segmentsCount")]
    internal string _segmentsCount { get; set; }

    [DataMember(Name = "segmentsIndexWriterMaxMemory")]
    internal string _segmentsIndexWriterMaxMemory { get; set; }

    [DataMember(Name = "segmentsIndexWriterMemory")]
    internal string _segmentsIndexWriterMemory { get; set; }

    [DataMember(Name = "segmentsMemory")]
    internal string _segmentsMemory { get; set; }

    [DataMember(Name = "segmentsVersionMapMemory")]
    internal string _segmentsVersionMapMemory { get; set; }

    [DataMember(Name = "sfc")]
    internal string _sfc { get; set; }

    [DataMember(Name = "sfti")]
    internal string _sfti { get; set; }

    [DataMember(Name = "sfto")]
    internal string _sfto { get; set; }

    [DataMember(Name = "siwm")]
    internal string _siwm { get; set; }

    [DataMember(Name = "siwmx")]
    internal string _siwmx { get; set; }

    [DataMember(Name = "sm")]
    internal string _sm { get; set; }

    [DataMember(Name = "so")]
    internal string _so { get; set; }

    [DataMember(Name = "sqc")]
    internal string _sqc { get; set; }

    [DataMember(Name = "sqti")]
    internal string _sqti { get; set; }

    [DataMember(Name = "sqto")]
    internal string _sqto { get; set; }

    [DataMember(Name = "svmm")]
    internal string _svmm { get; set; }

    [DataMember(Name = "u")]
    internal string _u { get; set; }

    [DataMember(Name = "uptime")]
    internal string _uptime { get; set; }

    [DataMember(Name = "v")]
    internal string _v { get; set; }

    [DataMember(Name = "version")]
    internal string _version { get; set; }
  }
}
