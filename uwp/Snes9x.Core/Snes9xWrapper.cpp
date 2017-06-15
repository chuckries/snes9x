#include "pch.h"
#include "Snes9xWrapper.h"
#include "LibSnes9x.h"

namespace Snes9x { namespace Core
{

    bool S9xWrapper::LoadRom(const char* path)
    {
        return (bool)Memory.LoadROM(path);
    }

    bool S9xWrapper::LoadRomMem(const byte* source, unsigned int sourceSize)
    {
        return (bool)Memory.LoadROMMem(source, sourceSize);
    }

    void S9xWrapper::MainLoop()
    {
        S9xMainLoop();
    }

    bool S9xWrapper::SaveState(const char* path)
    {
        return (bool)S9xFreezeGame(path);
    }

    bool S9xWrapper::LoadState(const char* path)
    {
        return (bool)S9xUnfreezeGame(path);
    }

    bool S9xWrapper::SaveSRAM(const char* path)
    {
        return (bool)Memory.SaveSRAM(path);
    }

    bool S9xWrapper::LoadSRAM(const char* path)
    {
        return (bool)Memory.LoadSRAM(path);
    }

    void S9xWrapper::ReportButton(unsigned int id, bool pressed)
    {
        S9xReportButton(id, pressed);
    }

} }