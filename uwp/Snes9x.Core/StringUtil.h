#pragma once

#include "pch.h"

class WideToUtf8
{
public:
    WideToUtf8(const wchar_t* in)
    {
        int size = WideCharToMultiByte(CP_UTF8, 0, in, -1, out, 0, nullptr, nullptr);
        out = new char[size];
        WideCharToMultiByte(CP_UTF8, 0, in, -1, out, size, nullptr, nullptr);
    }

    ~WideToUtf8()
    {
        if (out != nullptr)
        {
            delete[] out;
            out = nullptr;
        }
    }

    operator char* ()
    {
        return out;
    }

private:
    char* out;
};