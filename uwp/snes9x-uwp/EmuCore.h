#pragma once

#include "../../win32/CXAudio2.h"

ref class EmuCore sealed
{
public:
    EmuCore();

    Windows::Foundation::IAsyncOperation<bool>^ Init();
    void InitSettings();
    Windows::Foundation::IAsyncOperation<bool>^ LoadRom();

    Windows::Foundation::IAsyncOperation<bool>^ SaveState();
    Windows::Foundation::IAsyncOperation<bool>^ LoadState();

    property Platform::Array<byte>^ Screen;

    property Windows::Foundation::Size Size;
    property int Width;
    property int Height;

    property Platform::String^ RomName;

internal:
    CXAudio2 S9XAudio2;
};

