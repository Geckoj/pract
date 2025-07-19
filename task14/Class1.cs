using System;
using System.Threading;

public class DefiniteIntegral
{
    public static double Solve(double a, double b, Func<double, double> function, double step, int threadsNumber)
    {
        int steps = (int)((b - a) / step);
        double result = 0.0;
        var barrier = new Barrier(threadsNumber);
        object resultLock = new object();

        void CalculateIntegral(int threadIndex)
        {
            int start = (steps / threadsNumber) * threadIndex;
            int end = (threadIndex == threadsNumber - 1) ? steps : (steps / threadsNumber) * (threadIndex + 1);

            double partialResult = 0.0;

            for (int i = start; i < end; i++)
            {
                double x1 = a + i * step;
                double x2 = a + (i + 1) * step;
                partialResult += (function(x1) + function(x2)) * step / 2.0;
            }

            barrier.SignalAndWait();

            lock (resultLock)
            {
                result += partialResult;
            }
        }

        Thread[] threads = new Thread[threadsNumber];
        for (int i = 0; i < threadsNumber; i++)
        {
            int index = i;
            threads[i] = new Thread(() => CalculateIntegral(index));
            threads[i].Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        return result;
    }
}
