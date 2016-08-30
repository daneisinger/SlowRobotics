using Alea.CUDA;
using Alea.CUDA.Utilities;
using Alea.CUDA.IL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SlowRobotics.GPGPU
{
    public class GPUTest
    {

            //[parallelSquareCPU]
            public static void SquareCPU(double[] inputs)
            {
                var outputs = new double[inputs.Length];
                for (var i = 0; i < inputs.Length; i++)
                {
                    outputs[i] = inputs[i] * inputs[i];
                }
            }
            //[/parallelSquareCPU]

            //[parallelSquareKernel]
            [AOTCompile]
            static void SquareKernel(deviceptr<double> outputs, deviceptr<double> inputs, int n)
            {
                var start = blockIdx.x * blockDim.x + threadIdx.x;
                var stride = gridDim.x * blockDim.x;
                for (var i = start; i < n; i += stride)
                { 

                    double sum = 0;
                    
                    if (i-1 >= 0) sum += inputs[i - 1]; //left
                    if (i+1 < n) sum += inputs[i + 1]; //right
                    if (i-256 >=0) sum += inputs[i -256]; //up
                    if (i + 256 < n) sum += inputs[i + 256]; //down
                    if (sum == 0 && inputs[i]==0)
                    {
                        outputs[i] = 1;
                    } else if (sum == 2 && inputs[i] == 1)
                    {
                        outputs[i] = 0;
                    }else
                    {
                        outputs[i] = inputs[i];
                    }
                
                   // outputs[i] = inputs[i] * inputs[i];
                }
            }
            //[/parallelSquareKernel]

            //[parallelSquareLaunch]
            static double[] SquareGPU(double[] inputs)
            {
                var worker = Worker.Default;
                using (var dInputs = worker.Malloc(inputs))
                using (var dOutputs = worker.Malloc<double>(inputs.Length))
                {
                    const int blockSize = 256;
                    var numSm = worker.Device.Attributes.MULTIPROCESSOR_COUNT;
                    var gridSize = Math.Min(16 * numSm, Common.divup(inputs.Length, blockSize));
                    var lp = new LaunchParam(gridSize, blockSize);
                    worker.Launch(SquareKernel, lp, dOutputs.Ptr, dInputs.Ptr, inputs.Length);
                    return dOutputs.Gather();
                }
            }
            //[/parallelSquareLaunch]

            //[parallelSquareTest]
           // [Test]
           public static double[] createInputs()
        {
            Random r = new Random();
            int w = 256;
            int h = 256;
            var inputs = Enumerable.Range(0, w*h).Select(i => (r.NextDouble() > 0.5) ? 0.0 : 1.0).ToArray();
            return inputs;
        }
            public static double[] SquareTest(double[] inputs)
            {
            
                var outputs = SquareGPU(inputs);
                 return (outputs);
            }
            //[/parallelSquareTest]
        }
}
