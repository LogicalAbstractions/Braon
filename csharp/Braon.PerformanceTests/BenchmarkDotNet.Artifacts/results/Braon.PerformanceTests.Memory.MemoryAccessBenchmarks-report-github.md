``` ini

BenchmarkDotNet=v0.10.10, OS=Windows 10 Redstone 2 [1703, Creators Update] (10.0.15063.674)
Processor=Intel Core i7-5960X CPU 3.00GHz (Broadwell), ProcessorCount=16
Frequency=2929686 Hz, Resolution=341.3335 ns, Timer=TSC
.NET Core SDK=2.0.2
  [Host]     : .NET Core 2.0.0 (Framework 4.6.00001.0), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.0 (Framework 4.6.00001.0), 64bit RyuJIT


```
|                          Method |     Mean |     Error |    StdDev |   Median |
|-------------------------------- |---------:|----------:|----------:|---------:|
|              ManagedArrayAccess | 1.381 ms | 0.0398 ms | 0.0353 ms | 1.365 ms |
|       ManagedArrayPointerAccess | 1.075 ms | 0.0210 ms | 0.0224 ms | 1.066 ms |
|          ManagedArraySpanAccess | 1.126 ms | 0.0223 ms | 0.0651 ms | 1.091 ms |
|   ManagedArrayPointerSpanAccess | 1.081 ms | 0.0132 ms | 0.0103 ms | 1.079 ms |
|     UnmanagedArrayPointerAccess | 1.066 ms | 0.0078 ms | 0.0069 ms | 1.066 ms |
| UnmanagedArrayPointerSpanAccess | 1.227 ms | 0.0260 ms | 0.0767 ms | 1.225 ms |
