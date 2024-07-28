// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.XpressManaged
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  [CLSCompliant(false)]
  public static class XpressManaged
  {
    private static readonly Pool<XpressManaged.XPRESS_LZ_WORKSPACE_MAX> ManagedWorkspacePool = new Pool<XpressManaged.XPRESS_LZ_WORKSPACE_MAX>((Func<XpressManaged.XPRESS_LZ_WORKSPACE_MAX>) (() => new XpressManaged.XPRESS_LZ_WORKSPACE_MAX()), (Action<XpressManaged.XPRESS_LZ_WORKSPACE_MAX>) (w => { }), 4 * Environment.ProcessorCount);
    private const uint STATUS_BUFFER_TOO_SMALL = 3221225507;
    private const uint STATUS_NOT_SUPPORTED = 3221225659;
    private const uint STATUS_SUCCESS = 0;
    private const uint STATUS_BAD_COMPRESSION_BUFFER = 3221226050;
    private const uint FIRST_HASH_COEFF = 4;
    private const uint FIRST_HASH_COEFF2 = 1;
    private const uint HASH_TABLE_SIZE = 2806;
    private const uint HASH_COEFF_HIGH = 8;
    private const uint HASH_COEFF_LOW = 2;
    private const uint HASH_TABLE_EX_SIZE = 4846;
    private const uint LZ_WINDOW_SIZE = 8192;
    private const uint LZ_SMALL_LEN_BITS = 3;
    private const uint LZ_MAX_SMALL_LEN = 7;
    private const uint Z_HASH_SIZE = 32768;
    private static readonly ushort[][] XpressHashFunction = new ushort[3][]
    {
      new ushort[256]
      {
        (ushort) 5732,
        (ushort) 14471,
        (ushort) 24297,
        (ushort) 25128,
        (ushort) 11502,
        (ushort) 22712,
        (ushort) 22856,
        (ushort) 21969,
        (ushort) 16029,
        (ushort) 23951,
        (ushort) 1785,
        (ushort) 13794,
        (ushort) 3705,
        (ushort) 1145,
        (ushort) 16537,
        (ushort) 7129,
        (ushort) 26156,
        (ushort) 31870,
        (ushort) 23418,
        (ushort) 20567,
        (ushort) 29626,
        (ushort) 218,
        (ushort) 7372,
        (ushort) 1940,
        (ushort) 17184,
        (ushort) 18323,
        (ushort) 10119,
        (ushort) 8604,
        (ushort) 23621,
        (ushort) 13384,
        (ushort) 29712,
        (ushort) 9146,
        (ushort) 20553,
        (ushort) 27100,
        (ushort) 5850,
        (ushort) 23503,
        (ushort) 23176,
        (ushort) 28142,
        (ushort) 4676,
        (ushort) 27280,
        (ushort) 31756,
        (ushort) 8706,
        (ushort) 15559,
        (ushort) 27626,
        (ushort) 18819,
        (ushort) 3281,
        (ushort) 10692,
        (ushort) 4048,
        (ushort) 20259,
        (ushort) 10493,
        (ushort) 27969,
        (ushort) 7260,
        (ushort) 24815,
        (ushort) 17549,
        (ushort) 18994,
        (ushort) 16693,
        (ushort) 31156,
        (ushort) 16887,
        (ushort) 8459,
        (ushort) 31718,
        (ushort) 32718,
        (ushort) 12366,
        (ushort) 7181,
        (ushort) 15493,
        (ushort) 24345,
        (ushort) 26476,
        (ushort) 7355,
        (ushort) 14175,
        (ushort) 13292,
        (ushort) 4822,
        (ushort) 22793,
        (ushort) 215,
        (ushort) 24723,
        (ushort) 706,
        (ushort) 21435,
        (ushort) 19075,
        (ushort) 25807,
        (ushort) 32292,
        (ushort) 7815,
        (ushort) 29338,
        (ushort) 10789,
        (ushort) 9301,
        (ushort) 21695,
        (ushort) 25084,
        (ushort) 25689,
        (ushort) 31272,
        (ushort) 17247,
        (ushort) 14211,
        (ushort) 9189,
        (ushort) 26756,
        (ushort) 18524,
        (ushort) 14732,
        (ushort) 6188,
        (ushort) 8560,
        (ushort) 13667,
        (ushort) 26189,
        (ushort) 32660,
        (ushort) 4089,
        (ushort) 3976,
        (ushort) 23040,
        (ushort) 29476,
        (ushort) 20495,
        (ushort) 7315,
        (ushort) 5110,
        (ushort) 28586,
        (ushort) 17798,
        (ushort) 29904,
        (ushort) 344,
        (ushort) 22387,
        (ushort) 27111,
        (ushort) 9782,
        (ushort) 6030,
        (ushort) 18509,
        (ushort) 7423,
        (ushort) 9197,
        (ushort) 31788,
        (ushort) 25654,
        (ushort) 20364,
        (ushort) 18354,
        (ushort) 5971,
        (ushort) 10938,
        (ushort) 366,
        (ushort) 15563,
        (ushort) 18371,
        (ushort) 10683,
        (ushort) 3657,
        (ushort) 18511,
        (ushort) 15438,
        (ushort) 5520,
        (ushort) 422,
        (ushort) 13442,
        (ushort) 29830,
        (ushort) 27837,
        (ushort) 15463,
        (ushort) 14806,
        (ushort) 12921,
        (ushort) 3883,
        (ushort) 22276,
        (ushort) 16387,
        (ushort) 31729,
        (ushort) 21655,
        (ushort) 19236,
        (ushort) 20470,
        (ushort) 20492,
        (ushort) 11751,
        (ushort) 24686,
        (ushort) 25844,
        (ushort) 2840,
        (ushort) 9189,
        (ushort) 9869,
        (ushort) 19238,
        (ushort) 24848,
        (ushort) 15480,
        (ushort) 25809,
        (ushort) 20681,
        (ushort) 11470,
        (ushort) 25986,
        (ushort) 9409,
        (ushort) 24755,
        (ushort) 16524,
        (ushort) 24107,
        (ushort) 21258,
        (ushort) 13853,
        (ushort) 28403,
        (ushort) 6006,
        (ushort) 21415,
        (ushort) 23280,
        (ushort) 2570,
        (ushort) 12055,
        (ushort) 20113,
        (ushort) 5235,
        (ushort) 3562,
        (ushort) 7055,
        (ushort) 733,
        (ushort) 30365,
        (ushort) 9470,
        (ushort) 24356,
        (ushort) 9178,
        (ushort) 21362,
        (ushort) 28496,
        (ushort) 11136,
        (ushort) 3731,
        (ushort) 3047,
        (ushort) 16021,
        (ushort) 11840,
        (ushort) 22838,
        (ushort) 907,
        (ushort) 18609,
        (ushort) 4721,
        (ushort) 18943,
        (ushort) 14439,
        (ushort) 26839,
        (ushort) 2422,
        (ushort) 15312,
        (ushort) 11641,
        (ushort) 25979,
        (ushort) 199,
        (ushort) 31769,
        (ushort) 5234,
        (ushort) 23588,
        (ushort) 23401,
        (ushort) 15374,
        (ushort) 1558,
        (ushort) 14750,
        (ushort) 24149,
        (ushort) 31127,
        (ushort) 13862,
        (ushort) 26020,
        (ushort) 31010,
        (ushort) 1888,
        (ushort) 11434,
        (ushort) 1688,
        (ushort) 5562,
        (ushort) 9959,
        (ushort) 2280,
        (ushort) 8742,
        (ushort) 1443,
        (ushort) 25709,
        (ushort) 10904,
        (ushort) 24657,
        (ushort) 23884,
        (ushort) 6380,
        (ushort) 4008,
        (ushort) 20069,
        (ushort) 28033,
        (ushort) 6303,
        (ushort) 19272,
        (ushort) 30233,
        (ushort) 17279,
        (ushort) 17817,
        (ushort) 24129,
        (ushort) 28759,
        (ushort) 935,
        (ushort) 6698,
        (ushort) 16287,
        (ushort) 32578,
        (ushort) 14420,
        (ushort) 17587,
        (ushort) 18645,
        (ushort) 17233,
        (ushort) 8481,
        (ushort) 15766,
        (ushort) 3346,
        (ushort) 4080,
        (ushort) 25415,
        (ushort) 11667,
        (ushort) 15445,
        (ushort) 3149,
        (ushort) 17290,
        (ushort) 14358,
        (ushort) 28128,
        (ushort) 29424,
        (ushort) 17465,
        (ushort) 20225,
        (ushort) 8897,
        (ushort) 19481
      },
      new ushort[256]
      {
        (ushort) 89,
        (ushort) 24313,
        (ushort) 14591,
        (ushort) 8306,
        (ushort) 22828,
        (ushort) 18884,
        (ushort) 7990,
        (ushort) 26457,
        (ushort) 24877,
        (ushort) 30705,
        (ushort) 24165,
        (ushort) 7973,
        (ushort) 16319,
        (ushort) 9286,
        (ushort) 22677,
        (ushort) 30021,
        (ushort) 7901,
        (ushort) 7880,
        (ushort) 18035,
        (ushort) 4008,
        (ushort) 2401,
        (ushort) 29585,
        (ushort) 21569,
        (ushort) 12793,
        (ushort) 23315,
        (ushort) 9286,
        (ushort) 10852,
        (ushort) 26539,
        (ushort) 13186,
        (ushort) 26156,
        (ushort) 26964,
        (ushort) 21604,
        (ushort) 8305,
        (ushort) 15916,
        (ushort) 17767,
        (ushort) 15170,
        (ushort) 26565,
        (ushort) 26259,
        (ushort) 11693,
        (ushort) 31316,
        (ushort) 28980,
        (ushort) 22665,
        (ushort) 496,
        (ushort) 16463,
        (ushort) 12261,
        (ushort) 5111,
        (ushort) 1943,
        (ushort) 1660,
        (ushort) 23930,
        (ushort) 13257,
        (ushort) 4757,
        (ushort) 31563,
        (ushort) 15469,
        (ushort) 18763,
        (ushort) 17906,
        (ushort) 21517,
        (ushort) 29723,
        (ushort) 18324,
        (ushort) 20062,
        (ushort) 23554,
        (ushort) 9039,
        (ushort) 24669,
        (ushort) 31775,
        (ushort) 16771,
        (ushort) 17119,
        (ushort) 31060,
        (ushort) 2587,
        (ushort) 8261,
        (ushort) 22033,
        (ushort) 17027,
        (ushort) 5593,
        (ushort) 1729,
        (ushort) 214,
        (ushort) 2914,
        (ushort) 21849,
        (ushort) 18432,
        (ushort) 7429,
        (ushort) 21490,
        (ushort) 26117,
        (ushort) 29863,
        (ushort) 22371,
        (ushort) 18408,
        (ushort) 792,
        (ushort) 19364,
        (ushort) 21668,
        (ushort) 1809,
        (ushort) 21594,
        (ushort) 4948,
        (ushort) 20456,
        (ushort) 2019,
        (ushort) 29290,
        (ushort) 22887,
        (ushort) 29078,
        (ushort) 22335,
        (ushort) 9187,
        (ushort) 5096,
        (ushort) 17146,
        (ushort) 1432,
        (ushort) 19366,
        (ushort) 22510,
        (ushort) 7422,
        (ushort) 25502,
        (ushort) 13009,
        (ushort) 11542,
        (ushort) 7875,
        (ushort) 2647,
        (ushort) 7784,
        (ushort) 12205,
        (ushort) 12243,
        (ushort) 19397,
        (ushort) 15740,
        (ushort) 5364,
        (ushort) 4113,
        (ushort) 26061,
        (ushort) 25497,
        (ushort) 7856,
        (ushort) 1925,
        (ushort) 32327,
        (ushort) 12210,
        (ushort) 6254,
        (ushort) 27122,
        (ushort) 27016,
        (ushort) 20121,
        (ushort) 12018,
        (ushort) 4980,
        (ushort) 9272,
        (ushort) 5017,
        (ushort) 10708,
        (ushort) 31072,
        (ushort) 25162,
        (ushort) 25851,
        (ushort) 13203,
        (ushort) 15909,
        (ushort) 30345,
        (ushort) 26602,
        (ushort) 19838,
        (ushort) 12429,
        (ushort) 4695,
        (ushort) 2383,
        (ushort) 17491,
        (ushort) 524,
        (ushort) 31579,
        (ushort) 1318,
        (ushort) 12371,
        (ushort) 19282,
        (ushort) 19629,
        (ushort) 1772,
        (ushort) 22021,
        (ushort) 12775,
        (ushort) 25516,
        (ushort) 2257,
        (ushort) 26301,
        (ushort) 10519,
        (ushort) 27741,
        (ushort) 5552,
        (ushort) 27266,
        (ushort) 13548,
        (ushort) 28363,
        (ushort) 18524,
        (ushort) 31245,
        (ushort) 5982,
        (ushort) 17294,
        (ushort) 21585,
        (ushort) 13591,
        (ushort) 31302,
        (ushort) 31804,
        (ushort) 12702,
        (ushort) 15533,
        (ushort) 29640,
        (ushort) 23889,
        (ushort) 24312,
        (ushort) 16503,
        (ushort) 15054,
        (ushort) 22097,
        (ushort) 17733,
        (ushort) 4557,
        (ushort) 22693,
        (ushort) 3477,
        (ushort) 16700,
        (ushort) 18113,
        (ushort) 11692,
        (ushort) 10926,
        (ushort) 2215,
        (ushort) 9617,
        (ushort) 24248,
        (ushort) 3956,
        (ushort) 22701,
        (ushort) 24952,
        (ushort) 938,
        (ushort) 13889,
        (ushort) 4191,
        (ushort) 24275,
        (ushort) 9101,
        (ushort) 15744,
        (ushort) 19304,
        (ushort) 12082,
        (ushort) 6459,
        (ushort) 17626,
        (ushort) 32298,
        (ushort) 2736,
        (ushort) 8529,
        (ushort) 28611,
        (ushort) 15671,
        (ushort) 3892,
        (ushort) 26773,
        (ushort) 25900,
        (ushort) 6541,
        (ushort) 24135,
        (ushort) 20603,
        (ushort) 24870,
        (ushort) 27926,
        (ushort) 4019,
        (ushort) 28502,
        (ushort) 28252,
        (ushort) 10220,
        (ushort) 5251,
        (ushort) 25639,
        (ushort) 26053,
        (ushort) 25351,
        (ushort) 9722,
        (ushort) 3020,
        (ushort) 4086,
        (ushort) 29133,
        (ushort) 25585,
        (ushort) 23781,
        (ushort) 19564,
        (ushort) 29020,
        (ushort) 23744,
        (ushort) 1752,
        (ushort) 30531,
        (ushort) 24484,
        (ushort) 30451,
        (ushort) 25913,
        (ushort) 10908,
        (ushort) 15852,
        (ushort) 19700,
        (ushort) 14122,
        (ushort) 26590,
        (ushort) 17988,
        (ushort) 5299,
        (ushort) 23511,
        (ushort) 22145,
        (ushort) 26960,
        (ushort) 9847,
        (ushort) 5119,
        (ushort) 18466,
        (ushort) 6431,
        (ushort) 3592,
        (ushort) 6992,
        (ushort) 7398,
        (ushort) 9792,
        (ushort) 24368,
        (ushort) 19780,
        (ushort) 27824,
        (ushort) 16766,
        (ushort) 770
      },
      new ushort[256]
      {
        (ushort) 29141,
        (ushort) 2944,
        (ushort) 21483,
        (ushort) 667,
        (ushort) 28990,
        (ushort) 23448,
        (ushort) 12644,
        (ushort) 7839,
        (ushort) 21929,
        (ushort) 19747,
        (ushort) 16616,
        (ushort) 17046,
        (ushort) 19188,
        (ushort) 32762,
        (ushort) 25138,
        (ushort) 25039,
        (ushort) 19337,
        (ushort) 724,
        (ushort) 29934,
        (ushort) 4914,
        (ushort) 22687,
        (ushort) 841,
        (ushort) 14193,
        (ushort) 22961,
        (ushort) 1775,
        (ushort) 6902,
        (ushort) 23188,
        (ushort) 19240,
        (ushort) 7069,
        (ushort) 25600,
        (ushort) 15642,
        (ushort) 4994,
        (ushort) 21651,
        (ushort) 3594,
        (ushort) 27731,
        (ushort) 19933,
        (ushort) 11672,
        (ushort) 20837,
        (ushort) 21867,
        (ushort) 2547,
        (ushort) 30691,
        (ushort) 5021,
        (ushort) 4084,
        (ushort) 3381,
        (ushort) 20986,
        (ushort) 2656,
        (ushort) 7110,
        (ushort) 13821,
        (ushort) 7795,
        (ushort) 758,
        (ushort) 20780,
        (ushort) 20822,
        (ushort) 32649,
        (ushort) 9811,
        (ushort) 2267,
        (ushort) 25889,
        (ushort) 11350,
        (ushort) 27423,
        (ushort) 2944,
        (ushort) 7104,
        (ushort) 22471,
        (ushort) 31485,
        (ushort) 31150,
        (ushort) 9359,
        (ushort) 30674,
        (ushort) 13639,
        (ushort) 31985,
        (ushort) 20817,
        (ushort) 11744,
        (ushort) 16516,
        (ushort) 11270,
        (ushort) 24524,
        (ushort) 3193,
        (ushort) 18291,
        (ushort) 5290,
        (ushort) 7973,
        (ushort) 25154,
        (ushort) 32008,
        (ushort) 17754,
        (ushort) 3315,
        (ushort) 27005,
        (ushort) 21741,
        (ushort) 15695,
        (ushort) 20415,
        (ushort) 8565,
        (ushort) 4083,
        (ushort) 23560,
        (ushort) 24858,
        (ushort) 24228,
        (ushort) 13255,
        (ushort) 14780,
        (ushort) 14373,
        (ushort) 22361,
        (ushort) 20804,
        (ushort) 2970,
        (ushort) 16847,
        (ushort) 8003,
        (ushort) 25347,
        (ushort) 6633,
        (ushort) 29140,
        (ushort) 25152,
        (ushort) 16751,
        (ushort) 10005,
        (ushort) 8413,
        (ushort) 31873,
        (ushort) 12712,
        (ushort) 28180,
        (ushort) 23299,
        (ushort) 16433,
        (ushort) 3658,
        (ushort) 7784,
        (ushort) 28886,
        (ushort) 19894,
        (ushort) 18771,
        (ushort) 675,
        (ushort) 588,
        (ushort) 901,
        (ushort) 24092,
        (ushort) 1755,
        (ushort) 30519,
        (ushort) 11912,
        (ushort) 15045,
        (ushort) 15684,
        (ushort) 9183,
        (ushort) 10056,
        (ushort) 16848,
        (ushort) 16248,
        (ushort) 32429,
        (ushort) 2555,
        (ushort) 11360,
        (ushort) 11926,
        (ushort) 32162,
        (ushort) 19499,
        (ushort) 10997,
        (ushort) 20341,
        (ushort) 5905,
        (ushort) 16620,
        (ushort) 32124,
        (ushort) 27807,
        (ushort) 19460,
        (ushort) 24198,
        (ushort) 905,
        (ushort) 4976,
        (ushort) 14495,
        (ushort) 17752,
        (ushort) 15076,
        (ushort) 31994,
        (ushort) 11620,
        (ushort) 27478,
        (ushort) 16025,
        (ushort) 31463,
        (ushort) 25965,
        (ushort) 28887,
        (ushort) 18086,
        (ushort) 3806,
        (ushort) 11346,
        (ushort) 6701,
        (ushort) 27480,
        (ushort) 30042,
        (ushort) 61,
        (ushort) 1846,
        (ushort) 16527,
        (ushort) 9096,
        (ushort) 5811,
        (ushort) 3284,
        (ushort) 1002,
        (ushort) 21170,
        (ushort) 16860,
        (ushort) 21152,
        (ushort) 4570,
        (ushort) 10196,
        (ushort) 32752,
        (ushort) 9201,
        (ushort) 22647,
        (ushort) 16755,
        (ushort) 32259,
        (ushort) 29729,
        (ushort) 23205,
        (ushort) 19906,
        (ushort) 20825,
        (ushort) 31181,
        (ushort) 3237,
        (ushort) 931,
        (ushort) 25156,
        (ushort) 20188,
        (ushort) 16427,
        (ushort) 14394,
        (ushort) 18993,
        (ushort) 7857,
        (ushort) 25179,
        (ushort) 26064,
        (ushort) 1679,
        (ushort) 23786,
        (ushort) 32761,
        (ushort) 10299,
        (ushort) 1891,
        (ushort) 14039,
        (ushort) 1035,
        (ushort) 19354,
        (ushort) 6436,
        (ushort) 15366,
        (ushort) 14679,
        (ushort) 26868,
        (ushort) 19947,
        (ushort) 4862,
        (ushort) 19105,
        (ushort) 7407,
        (ushort) 13039,
        (ushort) 4013,
        (ushort) 22970,
        (ushort) 16180,
        (ushort) 14412,
        (ushort) 3405,
        (ushort) 4984,
        (ushort) 26696,
        (ushort) 7035,
        (ushort) 5361,
        (ushort) 11923,
        (ushort) 20784,
        (ushort) 23477,
        (ushort) 9498,
        (ushort) 8836,
        (ushort) 25922,
        (ushort) 32629,
        (ushort) 27125,
        (ushort) 30994,
        (ushort) 18141,
        (ushort) 21981,
        (ushort) 27383,
        (ushort) 23834,
        (ushort) 24366,
        (ushort) 10855,
        (ushort) 6149,
        (ushort) 22048,
        (ushort) 11990,
        (ushort) 13549,
        (ushort) 4315,
        (ushort) 3591,
        (ushort) 1901,
        (ushort) 21868,
        (ushort) 23189,
        (ushort) 25251,
        (ushort) 28174,
        (ushort) 6620,
        (ushort) 11566,
        (ushort) 31561,
        (ushort) 5909,
        (ushort) 10506,
        (ushort) 5137,
        (ushort) 8212,
        (ushort) 20000,
        (ushort) 14345,
        (ushort) 17393,
        (ushort) 7349,
        (ushort) 17202,
        (ushort) 15562
      }
    };
    private static readonly byte[] XpressHighBitIndexTable = new byte[256]
    {
      (byte) 0,
      (byte) 0,
      (byte) 1,
      (byte) 1,
      (byte) 2,
      (byte) 2,
      (byte) 2,
      (byte) 2,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 4,
      (byte) 4,
      (byte) 4,
      (byte) 4,
      (byte) 4,
      (byte) 4,
      (byte) 4,
      (byte) 4,
      (byte) 4,
      (byte) 4,
      (byte) 4,
      (byte) 4,
      (byte) 4,
      (byte) 4,
      (byte) 4,
      (byte) 4,
      (byte) 5,
      (byte) 5,
      (byte) 5,
      (byte) 5,
      (byte) 5,
      (byte) 5,
      (byte) 5,
      (byte) 5,
      (byte) 5,
      (byte) 5,
      (byte) 5,
      (byte) 5,
      (byte) 5,
      (byte) 5,
      (byte) 5,
      (byte) 5,
      (byte) 5,
      (byte) 5,
      (byte) 5,
      (byte) 5,
      (byte) 5,
      (byte) 5,
      (byte) 5,
      (byte) 5,
      (byte) 5,
      (byte) 5,
      (byte) 5,
      (byte) 5,
      (byte) 5,
      (byte) 5,
      (byte) 5,
      (byte) 5,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 6,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7,
      (byte) 7
    };

    private static void NT_ASSERT(bool assert)
    {
      if (!assert)
        throw new InvalidOperationException();
    }

    private static unsafe void __movsb(byte* dst, byte* source, ulong count)
    {
      for (ulong index = 0; index < count; ++index)
        dst[index] = source[index];
    }

    public static uint TryCompressChunk(
      byte[] uncompressedChunk,
      uint uncompressedChunkSize,
      byte[] compressedChunk,
      out uint compressedChunkSize)
    {
      if (uncompressedChunkSize < 16U)
      {
        compressedChunkSize = 0U;
        return 3221225507;
      }
      using (Pool<XpressManaged.XPRESS_LZ_WORKSPACE_MAX>.PoolHandle poolHandle = XpressManaged.ManagedWorkspacePool.Get())
        return XpressManaged.RtlCompressBufferXpressLzMax(uncompressedChunk, uncompressedChunkSize, compressedChunk, (uint) compressedChunk.Length, out compressedChunkSize, poolHandle.Value);
    }

    private static unsafe uint RtlCompressBufferXpressLzMax(
      byte[] uncompressedBuffer,
      uint uncompressedBufferSize,
      byte[] compressedBuffer,
      uint compressedBufferSize,
      out uint finalCompressedSize,
      XpressManaged.XPRESS_LZ_WORKSPACE_MAX Workspace)
    {
      Workspace.Zero();
      fixed (byte* UncompressedBuffer = uncompressedBuffer)
        fixed (byte* CompressedBuffer = compressedBuffer)
          fixed (uint* FinalCompressedSize = &finalCompressedSize)
            fixed (byte** WorkspaceHashTable = Workspace.HashTable)
              fixed (byte** WorkspacePrevMatch = Workspace.PrevMatch)
                return XpressManaged.RtlCompressBufferXpressLzMaxInternal(UncompressedBuffer, uncompressedBufferSize, CompressedBuffer, compressedBufferSize, FinalCompressedSize, WorkspaceHashTable, WorkspacePrevMatch);
    }

    private static unsafe uint RtlCompressBufferXpressLzMaxInternal(
      byte* UncompressedBuffer,
      uint UncompressedBufferSize,
      byte* CompressedBuffer,
      uint CompressedBufferSize,
      uint* FinalCompressedSize,
      byte** WorkspaceHashTable,
      byte** WorkspacePrevMatch)
    {
      ulong num1 = 0;
      uint num2 = UncompressedBufferSize;
      bool flag1 = false;
      bool flag2 = false;
      byte* numPtr1 = UncompressedBuffer + UncompressedBufferSize;
      byte* numPtr2 = CompressedBuffer + CompressedBufferSize;
      if (CompressedBufferSize < 64U || UncompressedBufferSize < 8U)
        return 3221225507;
      if ((UIntPtr) UncompressedBuffer <= new UIntPtr(8193))
        return 3221225659;
      byte* numPtr3 = numPtr1 - 5;
      byte* numPtr4 = numPtr2 - 32 - 9;
      byte* numPtr5 = UncompressedBuffer;
      byte* numPtr6 = CompressedBuffer;
      byte* numPtr7 = (byte*) null;
      uint num3 = 2;
      uint* numPtr8 = (uint*) numPtr6;
      byte* numPtr9 = numPtr6 + 4;
      *numPtr9 = *numPtr5;
      byte* numPtr10 = numPtr9 + 1;
      byte* numPtr11 = numPtr5 + 1;
      ulong num4 = 3;
      ulong num5 = 0;
      ulong num6 = 0;
      byte* numPtr12 = UncompressedBuffer;
      do
      {
        byte* val1 = numPtr12 + 8192;
        if (val1 > numPtr3)
          val1 = numPtr3;
        byte* numPtr13 = (byte*) Math.Min((ulong) val1, (ulong) numPtr11 + (ulong) num2);
        num6 %= 16384UL;
        while (numPtr12 < val1)
        {
          ulong num7 = (ulong) XpressManaged.XpressHashFunction[0][(int) *numPtr12] ^ (ulong) XpressManaged.XpressHashFunction[1][(int) numPtr12[1]] ^ (ulong) XpressManaged.XpressHashFunction[2][(int) numPtr12[2]];
          XpressManaged.NT_ASSERT(num6 < 16384UL);
          *(IntPtr*) ((IntPtr) WorkspacePrevMatch + (IntPtr) (num6 * (ulong) sizeof (byte*))) = *(IntPtr*) ((IntPtr) WorkspaceHashTable + (IntPtr) (num7 * (ulong) sizeof (byte*)));
          *(IntPtr*) ((IntPtr) WorkspaceHashTable + (IntPtr) (num7 * (ulong) sizeof (byte*))) = (IntPtr) numPtr12;
          ++numPtr12;
          ++num6;
        }
        byte** numPtr14 = WorkspacePrevMatch;
        while (numPtr11 < numPtr13 || numPtr11 < val1)
        {
          byte* numPtr15 = (byte*) *(IntPtr*) ((IntPtr) numPtr14 + (IntPtr) ((ulong) (numPtr11 - UncompressedBuffer) % 16384UL * (ulong) sizeof (byte*)));
          uint num8 = *(uint*) numPtr11;
          if (numPtr15 >= numPtr11 - 8192)
          {
            byte* numPtr16 = numPtr11 - 8192;
            uint num9 = num8 ^ *(uint*) numPtr15;
            if (num9 == 0U)
            {
              flag1 = true;
            }
            else
            {
              if (((int) num9 & 16777215) != 0)
              {
                numPtr15 = (byte*) *(IntPtr*) ((IntPtr) numPtr14 + (IntPtr) ((ulong) (numPtr15 - UncompressedBuffer) % 16384UL * (ulong) sizeof (byte*)));
                if (numPtr15 >= numPtr16)
                {
                  uint num10 = num8 ^ *(uint*) numPtr15;
                  if (num10 == 0U)
                  {
                    flag1 = true;
                    goto label_28;
                  }
                  else if (((int) num10 & 16777215) != 0)
                  {
                    numPtr15 = (byte*) *(IntPtr*) ((IntPtr) numPtr14 + (IntPtr) ((ulong) (numPtr15 - UncompressedBuffer) % 16384UL * (ulong) sizeof (byte*)));
                    if (numPtr15 >= numPtr16)
                    {
                      uint num11 = num8 ^ *(uint*) numPtr15;
                      if (num11 == 0U)
                      {
                        flag1 = true;
                        goto label_28;
                      }
                      else if (((int) num11 & 16777215) != 0)
                        goto label_24;
                    }
                    else
                      goto label_24;
                  }
                }
                else
                  goto label_24;
              }
              XpressManaged.NT_ASSERT(num4 == 3UL);
              XpressManaged.NT_ASSERT(numPtr11 > numPtr15);
              num1 = (ulong) (numPtr11 - numPtr15);
              numPtr16 = numPtr11 - 8192;
            }
label_28:
            do
            {
              if (!flag1)
              {
                numPtr15 = (byte*) *(IntPtr*) ((IntPtr) numPtr14 + (IntPtr) ((ulong) (numPtr15 - UncompressedBuffer) % 16384UL * (ulong) sizeof (byte*)));
                if (numPtr15 < numPtr16)
                  break;
              }
              if (flag1 || (int) num8 == (int) *(uint*) numPtr15)
              {
                flag1 = false;
                byte* numPtr17 = numPtr11;
                byte* numPtr18 = numPtr11 + 4;
                byte* numPtr19 = numPtr15 + 4;
                uint* numPtr20 = (uint*) numPtr18;
                uint* numPtr21 = (uint*) numPtr19;
                while (numPtr18 + 32 < numPtr1)
                {
                  if ((int) *numPtr20 == (int) *numPtr21)
                  {
                    if ((int) numPtr20[1] != (int) numPtr21[1])
                    {
                      numPtr18 += 4;
                      numPtr19 += 4;
                    }
                    else if ((int) numPtr20[2] != (int) numPtr21[2])
                    {
                      numPtr18 += 8;
                      numPtr19 += 8;
                    }
                    else if ((int) numPtr20[3] != (int) numPtr21[3])
                    {
                      numPtr18 += 12;
                      numPtr19 += 12;
                    }
                    else if ((int) numPtr20[4] != (int) numPtr21[4])
                    {
                      numPtr18 += 16;
                      numPtr19 += 16;
                    }
                    else if ((int) numPtr20[5] != (int) numPtr21[5])
                    {
                      numPtr18 += 20;
                      numPtr19 += 20;
                    }
                    else if ((int) numPtr20[6] != (int) numPtr21[6])
                    {
                      numPtr18 += 24;
                      numPtr19 += 24;
                    }
                    else if ((int) numPtr20[7] != (int) numPtr21[7])
                    {
                      numPtr18 += 28;
                      numPtr19 += 28;
                    }
                    else
                    {
                      numPtr18 += 32;
                      numPtr19 += 32;
                      numPtr20 = (uint*) numPtr18;
                      numPtr21 = (uint*) numPtr19;
                      continue;
                    }
                  }
                  if ((int) *numPtr18 == (int) *numPtr19)
                  {
                    if ((int) numPtr18[1] != (int) numPtr19[1])
                    {
                      ++numPtr18;
                      ++numPtr19;
                      goto label_57;
                    }
                    else if ((int) numPtr18[2] != (int) numPtr19[2])
                    {
                      numPtr18 += 2;
                      numPtr19 += 2;
                      goto label_57;
                    }
                    else
                    {
                      numPtr18 += 3;
                      numPtr19 += 3;
                      goto label_57;
                    }
                  }
                  else
                    goto label_57;
                }
                for (; numPtr18 < numPtr1 && (int) *numPtr18 == (int) *numPtr19; ++numPtr19)
                  ++numPtr18;
label_57:
                XpressManaged.NT_ASSERT(numPtr18 > numPtr17);
                ulong num12 = (ulong) (numPtr18 - numPtr17);
                if (num12 > num4)
                {
                  num4 = num12;
                  XpressManaged.NT_ASSERT(numPtr18 > numPtr19);
                  num1 = (ulong) (numPtr18 - numPtr19);
                  if (numPtr19 > numPtr17)
                  {
                    numPtr11 = numPtr17;
                    break;
                  }
                }
                else
                  num5 += num12;
                numPtr15 = numPtr19 - num12;
                numPtr11 = numPtr17;
              }
              ++num5;
            }
            while (num5 < 24UL);
            num5 = 0UL;
            ulong num13 = num4;
            num4 = 3UL;
            numPtr11 += num13;
            XpressManaged.NT_ASSERT(num1 <= 8192UL);
            ulong num14 = num13 - 3UL;
            num1 = num1 - 1UL << 3;
            for (; flag2 || num14 >= 7UL; flag2 = true)
            {
              if (!flag2)
              {
                num1 |= 7UL;
                *(short*) numPtr10 = (short) (ushort) num1;
                numPtr10 += 2;
                num14 -= 7UL;
              }
              if (!flag2 && (IntPtr) numPtr7 == IntPtr.Zero)
              {
                numPtr7 = numPtr10;
                if (num14 < 15UL)
                {
                  *numPtr10 = (byte) num14;
                  ++numPtr10;
                  goto label_81;
                }
                else
                {
                  *numPtr10 = (byte) 15;
                  ++numPtr10;
                }
              }
              else if (!flag2 && num14 < 15UL)
              {
                byte* numPtr22 = numPtr7;
                int num15 = (int) (byte) ((uint) *numPtr22 | (uint) (byte) (num14 << 4));
                *numPtr22 = (byte) num15;
                numPtr7 = (byte*) null;
                goto label_81;
              }
              else
              {
                if (!flag2)
                {
                  byte* numPtr23 = numPtr7;
                  int num16 = (int) (byte) ((uint) *numPtr23 | 240U);
                  *numPtr23 = (byte) num16;
                  numPtr7 = (byte*) null;
                }
                flag2 = false;
                ulong num17 = num14 - 15UL;
                if (num17 < (ulong) byte.MaxValue)
                {
                  *numPtr10 = (byte) num17;
                  ++numPtr10;
                  goto label_81;
                }
                else
                {
                  *numPtr10 = byte.MaxValue;
                  byte* numPtr24 = numPtr10 + 1;
                  ulong num18 = num17 + 22UL;
                  if (num18 < 65536UL)
                  {
                    *(short*) numPtr24 = (short) (ushort) num18;
                    numPtr10 = numPtr24 + 2;
                    goto label_81;
                  }
                  else
                  {
                    *(short*) numPtr24 = (short) 0;
                    byte* numPtr25 = numPtr24 + 2;
                    *(int*) numPtr25 = (int) (uint) num18;
                    numPtr10 = numPtr25 + 4;
                    goto label_81;
                  }
                }
              }
            }
            num1 += num14;
            *(short*) numPtr10 = (short) (ushort) num1;
            numPtr10 += 2;
label_81:
            if ((int) num3 > 0)
            {
              num3 = (uint) ((int) num3 * 2 + 1);
            }
            else
            {
              uint num19 = (uint) ((int) num3 * 2 + 1);
              *numPtr8 = num19;
              num3 = 1U;
              numPtr8 = (uint*) numPtr10;
              numPtr10 += 4;
            }
            if (numPtr10 < numPtr4)
              continue;
            goto label_89;
          }
label_24:
          *numPtr10 = (byte) num8;
          ++numPtr10;
          ++numPtr11;
          if ((int) num3 > 0)
          {
            num3 *= 2U;
          }
          else
          {
            uint num20 = num3 * 2U;
            *numPtr8 = num20;
            num3 = 1U;
            numPtr8 = (uint*) numPtr10;
            numPtr10 += 4;
            if (numPtr10 >= numPtr4)
              goto label_89;
          }
        }
      }
      while (numPtr11 < numPtr3);
label_89:
      while (numPtr11 < numPtr1 && numPtr10 < numPtr2)
      {
        *numPtr10 = *numPtr11;
        ++numPtr10;
        ++numPtr11;
        if ((int) num3 > 0)
        {
          num3 *= 2U;
        }
        else
        {
          uint num21 = num3 * 2U;
          *numPtr8 = num21;
          num3 = 1U;
          numPtr8 = (uint*) numPtr10;
          numPtr10 += 4;
        }
      }
      if (numPtr10 >= numPtr2)
        return 3221225507;
      while ((int) num3 > 0)
        num3 = (uint) ((int) num3 * 2 + 1);
      uint num22 = (uint) ((int) num3 * 2 + 1);
      *numPtr8 = num22;
      *FinalCompressedSize = (uint) (numPtr10 - CompressedBuffer);
      if (*FinalCompressedSize < 8U)
        *FinalCompressedSize = 8U;
      return 0;
    }

    [CLSCompliant(false)]
    public static unsafe uint RtlDecompressBufferXpressLz(
      byte[] uncompressedBuffer,
      byte[] compressedBuffer,
      uint compressedBufferSize,
      out uint finalUncompressedSize)
    {
      fixed (byte* UncompressedBuffer = uncompressedBuffer)
        fixed (byte* CompressedBuffer = compressedBuffer)
          fixed (uint* FinalUncompressedSize = &finalUncompressedSize)
            return XpressManaged.RtlDecompressBufferXpressLz(UncompressedBuffer, (uint) uncompressedBuffer.Length, CompressedBuffer, compressedBufferSize, FinalUncompressedSize);
    }

    private static unsafe uint RtlDecompressBufferXpressLz(
      byte* UncompressedBuffer,
      uint UncompressedBufferSize,
      byte* CompressedBuffer,
      uint CompressedBufferSize,
      uint* FinalUncompressedSize)
    {
      long count = 0;
      ulong num1 = 0;
      int num2 = 0;
      if (CompressedBufferSize < 5U)
        return 3221226050;
      byte* numPtr1 = CompressedBuffer + CompressedBufferSize;
      byte* numPtr2 = UncompressedBuffer + UncompressedBufferSize;
      byte* numPtr3 = CompressedBuffer;
      byte* dst1 = UncompressedBuffer;
      byte* numPtr4 = numPtr1 - 82 - 4;
      byte* numPtr5 = numPtr2 - 352;
      byte* numPtr6 = (byte*) null;
      bool flag1 = false;
      bool flag2 = false;
      bool flag3 = false;
      bool flag4 = false;
      bool flag5 = true;
label_3:
      byte* numPtr7;
      while (true)
      {
        do
        {
          if (flag5)
            goto label_16;
label_4:
          if (!flag2)
          {
            if (num2 >= 0)
              num2 *= 2;
            else
              goto label_15;
          }
          while (true)
          {
            flag2 = false;
            if (num2 >= 0)
            {
              num2 *= 2;
              if (num2 >= 0)
              {
                num2 *= 2;
                if (num2 >= 0)
                {
                  num2 *= 2;
                  *(int*) dst1 = (int) *(uint*) numPtr3;
                  dst1 += 4;
                  numPtr3 += 4;
                  if (num2 >= 0)
                    num2 *= 2;
                  else
                    goto label_15;
                }
                else
                  goto label_12;
              }
              else
                goto label_10;
            }
            else
              break;
          }
          *dst1 = *numPtr3;
          ++dst1;
          ++numPtr3;
          goto label_15;
label_10:
          *(short*) dst1 = (short) *(ushort*) numPtr3;
          dst1 += 2;
          numPtr3 += 2;
          goto label_15;
label_12:
          *(int*) dst1 = (int) *(uint*) numPtr3;
          dst1 += 3;
          numPtr3 += 3;
label_15:
          num2 *= 2;
label_16:
          if (flag5 || num2 == 0)
          {
            flag5 = false;
            num2 = *(int*) numPtr3;
            numPtr3 += 4;
            if (numPtr3 >= numPtr4 || dst1 >= numPtr5)
            {
              flag1 = true;
              goto label_56;
            }
            else if (num2 < 0)
            {
              num2 = num2 * 2 + 1;
            }
            else
            {
              num2 = num2 * 2 + 1;
              flag2 = true;
              goto label_4;
            }
          }
          ulong num3 = (ulong) *(ushort*) numPtr3;
          numPtr3 += 2;
          long num4 = (long) num3 & 7L;
          num1 = (num3 >> 3) + 1UL;
          if (num4 == 7L)
          {
            if ((IntPtr) numPtr6 == IntPtr.Zero)
            {
              numPtr6 = numPtr3;
              ++numPtr3;
              count = (long) ((int) *numPtr6 & 15);
            }
            else
            {
              count = (long) ((int) *numPtr6 >> 4);
              numPtr6 = (byte*) null;
            }
            if (count == 15L)
            {
              if (numPtr3 + 7 >= numPtr4)
              {
                flag3 = true;
                goto label_56;
              }
              else
              {
                long num5 = (long) *numPtr3;
                ++numPtr3;
                if (num5 == (long) byte.MaxValue)
                {
                  long num6 = (long) *(ushort*) numPtr3;
                  numPtr3 += 2;
                  if (num6 == 0L)
                  {
                    num6 = (long) *(uint*) numPtr3;
                    numPtr3 += 4;
                  }
                  if (num6 < 22L || dst1 + num6 + 3 < dst1)
                    return 3221226050;
                  num5 = num6 - 22L;
                }
                count = num5 + 15L;
              }
            }
            num4 = count + 7L;
          }
          count = num4 + 3L;
          numPtr7 = dst1 - num1;
          if (numPtr7 < UncompressedBuffer)
            return 3221226050;
          if (num1 < 4UL)
          {
            switch ((long) num1 - 1L)
            {
              case 0:
                *dst1 = *numPtr7;
                dst1[1] = *numPtr7;
                dst1[2] = *numPtr7;
                count -= 3L;
                dst1 += 3;
                break;
              case 1:
                *dst1 = *numPtr7;
                dst1[1] = numPtr7[1];
                count -= 2L;
                dst1 += 2;
                break;
              case 2:
                *dst1 = *numPtr7;
                dst1[1] = numPtr7[1];
                dst1[2] = numPtr7[2];
                dst1 += 3;
                count -= 3L;
                break;
              default:
                throw new InvalidOperationException();
            }
          }
          else
            break;
        }
        while (count == 0L);
        *(int*) dst1 = (int) *(uint*) numPtr7;
        *(int*) (dst1 + 4) = (int) *(uint*) (numPtr7 + 4);
        if (count < 9L)
          dst1 += count;
        else
          break;
      }
      byte* dst2 = dst1 + 8;
      byte* source1 = numPtr7 + 8;
      count -= 8L;
      while (dst2 < numPtr5)
      {
        *(int*) dst2 = (int) *(uint*) source1;
        *(int*) (dst2 + 4) = (int) *(uint*) (source1 + 4);
        *(int*) (dst2 + 8) = (int) *(uint*) (source1 + 8);
        *(int*) (dst2 + 12) = (int) *(uint*) (source1 + 12);
        if (count < 17L)
        {
          dst1 = dst2 + count;
          goto label_3;
        }
        else
        {
          dst2 += 16;
          source1 += 16;
          count -= 16L;
        }
      }
      if (dst2 + count > numPtr2)
        return 3221226050;
      XpressManaged.__movsb(dst2, source1, (ulong) count);
      dst1 = dst2 + count;
label_56:
      while (true)
      {
        if (!flag1)
        {
          if (!flag3)
          {
            if (!flag4)
            {
              if (num2 >= 0)
                num2 *= 2;
              else
                goto label_69;
            }
            while (true)
            {
              flag4 = false;
              if (num2 >= 0)
              {
                num2 *= 2;
                if (numPtr3 + 2 <= numPtr1 && dst1 + 2 <= numPtr2)
                {
                  *(short*) dst1 = (short) *(ushort*) numPtr3;
                  dst1 += 2;
                  numPtr3 += 2;
                  if (num2 >= 0)
                    num2 *= 2;
                  else
                    goto label_69;
                }
                else
                  goto label_66;
              }
              else
                break;
            }
            if (numPtr3 < numPtr1 && dst1 < numPtr2)
            {
              *dst1 = *numPtr3;
              ++dst1;
              ++numPtr3;
            }
            else
              break;
label_69:
            num2 *= 2;
          }
          else
            goto label_83;
        }
        if (flag1 || num2 == 0)
        {
          if (!flag1)
          {
            if (numPtr3 + 3 < numPtr1)
            {
              num2 = *(int*) numPtr3;
              numPtr3 += 4;
            }
            else
              goto label_73;
          }
          flag1 = false;
          if (num2 < 0)
          {
            num2 = num2 * 2 + 1;
          }
          else
          {
            num2 = num2 * 2 + 1;
            flag4 = true;
            continue;
          }
        }
        if (numPtr3 != numPtr1)
        {
          if (numPtr3 + 1 < numPtr1)
          {
            ulong num7 = (ulong) *(ushort*) numPtr3;
            numPtr3 += 2;
            count = (long) num7 & 7L;
            num1 = (num7 >> 3) + 1UL;
          }
          else
            goto label_80;
        }
        else
          goto label_108;
label_83:
        if (flag3 || count == 7L)
        {
          if (!flag3)
          {
            if ((IntPtr) numPtr6 == IntPtr.Zero)
            {
              if (numPtr3 < numPtr1)
              {
                numPtr6 = numPtr3;
                ++numPtr3;
                count = (long) ((int) *numPtr6 & 15);
              }
              else
                goto label_87;
            }
            else
            {
              count = (long) ((int) *numPtr6 >> 4);
              numPtr6 = (byte*) null;
            }
          }
          if (flag3 || count == 15L)
          {
            flag3 = false;
            if (numPtr3 < numPtr1)
            {
              long num8 = (long) *numPtr3;
              ++numPtr3;
              if (num8 == (long) byte.MaxValue)
              {
                if (numPtr3 + 1 < numPtr1)
                {
                  long num9 = (long) *(ushort*) numPtr3;
                  numPtr3 += 2;
                  if (num9 == 0L)
                  {
                    if (numPtr3 + 3 < numPtr1)
                    {
                      num9 = (long) *(uint*) numPtr3;
                      numPtr3 += 4;
                    }
                    else
                      goto label_98;
                  }
                  if (num9 >= 22L && dst1 + num9 + 3 >= dst1)
                    num8 = num9 - 22L;
                  else
                    goto label_101;
                }
                else
                  goto label_95;
              }
              count = num8 + 15L;
            }
            else
              goto label_92;
          }
          count += 7L;
        }
        count += 3L;
        byte* source2 = dst1 - num1;
        if (source2 >= UncompressedBuffer && dst1 + count <= numPtr2)
        {
          XpressManaged.__movsb(dst1, source2, (ulong) count);
          dst1 += count;
        }
        else
          goto label_106;
      }
      return 3221226050;
label_66:
      return 3221226050;
label_73:
      return 3221226050;
label_80:
      if (dst1 < numPtr2)
        return 3221226050;
      goto label_108;
label_87:
      return 3221226050;
label_92:
      return 3221226050;
label_95:
      return 3221226050;
label_98:
      return 3221226050;
label_101:
      return 3221226050;
label_106:
      return 3221226050;
label_108:
      *FinalUncompressedSize = (uint) (dst1 - UncompressedBuffer);
      return 0;
    }

    private sealed class XPRESS_LZ_WORKSPACE_MAX
    {
      public readonly unsafe byte*[] HashTable = new byte*[32768];
      public readonly unsafe byte*[] PrevMatch = new byte*[16384];

      public unsafe void Zero()
      {
        for (int index = 0; index < this.HashTable.Length; ++index)
          this.HashTable[index] = (byte*) null;
        for (int index = 0; index < this.PrevMatch.Length; ++index)
          this.PrevMatch[index] = (byte*) null;
      }
    }
  }
}
