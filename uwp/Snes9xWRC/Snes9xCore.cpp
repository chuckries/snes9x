#include "pch.h"
#include "Snes9xCore.h"

#include "LibSnes9x.h"

namespace Snes9xWRC { namespace Core {

    void InitMemory()
    {
        Memory.Init();
    }

    bool InitGraphics(uint16_t* screen, uint32_t pitch)
    {
        GFX.Screen = screen;
        GFX.Pitch = pitch;
        return (bool)S9xGraphicsInit();
    }

    bool InitApu()
    {
        return (bool)S9xInitAPU();
    }

    bool InitSound(int buffer_ms, int lag_ms)
    {
        return (bool)S9xInitSound(buffer_ms, lag_ms);
    }

    bool LoadRomMem(const byte* source, unsigned int sourceSize)
    {
        return (bool)Memory.LoadROMMem(source, sourceSize);
    }

    void MainLoop()
    {
        S9xMainLoop();
    }

} }