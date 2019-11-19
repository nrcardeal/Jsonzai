``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18362
Intel Core i7-8565U CPU 1.80GHz (Whiskey Lake), 1 CPU, 8 logical and 4 physical cores
  [Host] : .NET Framework 4.8 (4.8.4018.0), X86 LegacyJIT  [AttachedDebugger]

Job=InProcess  Toolchain=InProcessEmitToolchain  IterationCount=15  
LaunchCount=1  WarmupCount=10  

```
|                                    Method |       Mean |      Error |     StdDev | Rank |
|------------------------------------------ |-----------:|-----------:|-----------:|-----:|
|                       BenchStudentReflect |  11.664 us |  0.3105 us |  0.2905 us |    9 |
|                          BenchStudentEmit |   8.893 us |  0.2128 us |  0.1777 us |    8 |
|                  BenchStudentArrayReflect | 205.600 us | 13.2858 us | 12.4276 us |   19 |
|                     BenchStudentArrayEmit |  93.455 us |  4.6247 us |  4.0997 us |   16 |
|                     BenchClassroomReflect | 125.328 us |  4.1419 us |  3.8743 us |   18 |
|                        BenchClassroomEmit | 102.707 us |  1.3739 us |  1.1472 us |   17 |
|                       BenchAccountReflect |  40.882 us |  0.6643 us |  0.6214 us |   15 |
|                          BenchAccountEmit |  17.055 us |  0.9632 us |  0.8043 us |   12 |
|                      BenchPropertyReflect |   5.028 us |  0.6836 us |  0.6394 us |    5 |
|                         BenchPropertyEmit |   3.839 us |  0.4692 us |  0.4389 us |    4 |
|                      BenchSiblingsReflect |   3.949 us |  0.5870 us |  0.5490 us |    4 |
|                         BenchSiblingsEmit |   2.618 us |  0.1687 us |  0.1495 us |    1 |
|               BenchPersonWithBirthReflect |   5.738 us |  0.1477 us |  0.1153 us |    6 |
|                  BenchPersonWithBirthEmit |   4.875 us |  0.5731 us |  0.5360 us |    5 |
|                   BenchPersonArrayReflect |   3.119 us |  0.0915 us |  0.0812 us |    2 |
|                      BenchPersonArrayEmit |   2.788 us |  0.3955 us |  0.3699 us |    1 |
|                       BenchJsonUriReflect |   3.339 us |  0.0844 us |  0.0748 us |    3 |
|                          BenchJsonUriEmit |   3.280 us |  0.3055 us |  0.2858 us |    3 |
|                      BenchJsonGuidReflect |   6.084 us |  0.2162 us |  0.1805 us |    7 |
|                         BenchJsonGuidEmit |   5.371 us |  0.5562 us |  0.5203 us |    5 |
|                  BenchJsonDatetimeReflect |  22.636 us |  1.6662 us |  1.5586 us |   14 |
|                     BenchJsonDatetimeEmit |  15.920 us |  0.4524 us |  0.4231 us |   11 |
|                    BenchTestNumberReflect |  20.916 us |  0.9049 us |  0.8464 us |   14 |
|                       BenchTestNumberEmit |  14.946 us |  0.3907 us |  0.3655 us |   10 |
|             BenchStructArrayAgendaReflect |   6.561 us |  0.7029 us |  0.6575 us |    7 |
|                BenchStructArrayAgendaEmit |   6.618 us |  1.0593 us |  0.9909 us |    7 |
| BenchStructArrayTypeValueSpeedTestReflect |  22.121 us |  1.9451 us |  1.7243 us |   14 |
|    BenchStructArrayTypeValueSpeedTestEmit |  18.906 us |  0.5398 us |  0.4508 us |   13 |
