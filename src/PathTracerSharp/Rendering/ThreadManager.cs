using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace PathTracerSharp.Rendering
{
    public class ThreadManager : IDisposable
    {
        public ThreadManagerState State { get; private set; } = ThreadManagerState.Created;

        private ConcurrentQueue<Routine> _queue = new ConcurrentQueue<Routine>();
        private Thread[] _pool;
        private Action _onDone;
        private int _endedThreads = 0;

        private struct Routine
        {
            public Action Action { get; set; }
            public int Priority { get; set; }

            public Routine(Action action, int priority)
            {
                Action = action;
                Priority = priority;
            }
        }

        public void Start(int count, Action onDone) 
        {
            State = ThreadManagerState.Started;

            SortQueue();

            _onDone = onDone;
            _pool = Enumerable.Range(0, count)
                .Select(x => new Thread(ThreadProcess) { IsBackground = true })
                .ToArray();

            for (var i = 0; i < _pool.Length; i++)
            {
                if (_pool[i] != null) 
                {
                    _pool[i].Start();
                }
            }
        }

        private void Stop() 
        {
            _onDone?.Invoke();
            _onDone = null;
            State = ThreadManagerState.Stopped;
        }

        public void Push(Action action, int priority = 0)
        {
            if (State != ThreadManagerState.Created) 
                throw new InvalidOperationException($"This method can be called only when State = {ThreadManagerState.Created}");

            _queue.Enqueue(new Routine(action, priority));
        }

        private void SortQueue() 
        {
            var sortedQueue = new ConcurrentQueue<Routine>();
            foreach (var item in _queue.OrderBy(x => x.Priority))
            {
                sortedQueue.Enqueue(item);
            }
            _queue = sortedQueue;
        }

        private void Kill() 
        {
            if (_pool is null) return;

            for (var i = 0; i < _pool.Length; i++) 
            {
                if (_pool[i] != null) 
                {
                    _pool[i].Interrupt();
                    _pool[i] = null;
                }
            }

            _queue.Clear();
            _onDone = null;
        }

        private void ThreadProcess() 
        {
            try
            {
                while (true)
                {
                    if (_queue.Count > 0)
                    {
                        if (_queue.TryDequeue(out var routine)) 
                        {
                            routine.Action?.Invoke();
                        }

                        if (_queue.Count == 0)
                        {
                            break;
                        }
                    }
                    else
                    {
                        Thread.Sleep(1);
                    }

                    if (_queue.Count == 0)
                    {
                        break;
                    }
                }

                Interlocked.Increment(ref _endedThreads);

                if (_endedThreads == _pool.Length) 
                {
                    Stop();
                }
            }
            catch (ThreadInterruptedException)
            {
            }
        }

        public void Dispose()
        {
            Kill();
            GC.SuppressFinalize(this);
            GC.ReRegisterForFinalize(this);
        }
    }

    public enum ThreadManagerState 
    {
        Created = 0,
        Started = 1,
        Stopped = 2,
    }
}
