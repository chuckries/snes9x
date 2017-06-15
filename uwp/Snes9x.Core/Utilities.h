#pragma once

inline void ThrowIfFalse(bool condition)
{
    if (!condition)
    {
        throw ref new Platform::FailureException();
    }
}