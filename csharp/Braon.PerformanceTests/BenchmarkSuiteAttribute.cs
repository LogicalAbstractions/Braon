using System;

namespace Braon.PerformanceTests
{
    [AttributeUsage(AttributeTargets.Class)]
    public class BenchmarkSuiteAttribute : Attribute 
    {
        public string Name { get; set; }
    }
}