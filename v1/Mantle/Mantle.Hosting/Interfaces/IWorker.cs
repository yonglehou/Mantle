﻿using System;

namespace Mantle.Hosting.Interfaces
{
    public interface IWorker
    {
        event Action<string> ErrorOccurred;
        event Action<string> MessageOccurred;

        void Start();
        void Stop();
    }
}