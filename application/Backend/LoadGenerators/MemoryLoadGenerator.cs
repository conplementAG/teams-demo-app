using System.Runtime.CompilerServices;
using System.Threading;
using System.Linq;
using System.Diagnostics;
using System.Runtime;

namespace Backend.Load
{
    public class MemoryLoadGenerator
    {
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public void GenerateLoad(int megabytes, int durationInSeconds)
        {
            GCLatencyMode oldMode = GCSettings.LatencyMode;
            RuntimeHelpers.PrepareConstrainedRegions();

            try
            {
                GCSettings.LatencyMode = GCLatencyMode.LowLatency;

                var bytesOfLong = 8;
                var number = megabytes * 1024 * 1024 / bytesOfLong;
                var allocatedMemory = new long[number];
                for (long i = 0; i < allocatedMemory.Length; i++)
                {
                    allocatedMemory[i] = i;
                }
                Thread.Sleep(durationInSeconds * 1000);
                var sum = allocatedMemory.Sum();
                Debug.WriteLine($"Sum: {sum}");
            }
            finally
            {
                GCSettings.LatencyMode = oldMode;
            }
        }
    }
}
