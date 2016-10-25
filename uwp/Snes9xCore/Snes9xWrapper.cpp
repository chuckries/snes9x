#include "pch.h"
#include "Snes9xWrapper.h"
#include "LibSnes9x.h"

namespace Snes9xCore
{

    bool S9xWrapper::InitMemory()
    {
        return (bool)Memory.Init();
    }

    bool S9xWrapper::InitGraphics(uint16_t* screen, uint32_t pitch)
    {
        GFX.Screen = screen;
        GFX.Pitch = pitch;
        return (bool)S9xGraphicsInit();
    }

    bool S9xWrapper::InitApu()
    {
        return (bool)S9xInitAPU();
    }

    bool S9xWrapper::InitSound(int buffer_ms, int lag_ms)
    {
        return (bool)S9xInitSound(buffer_ms, lag_ms);
    }

    void S9xWrapper::InitControllers()
    {
        S9xSetController(0, CTL_JOYPAD, 0, 0, 0, 0);
        S9xSetController(1, CTL_JOYPAD, 1, 0, 0, 0);

        S9xUnmapAllControls();
        S9xSetupDefaultKeymap();
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

}