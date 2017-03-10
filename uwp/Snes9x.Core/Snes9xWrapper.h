#pragma once

namespace Snes9x { namespace Core
{
    class S9xWrapper
    {
    public:
        static bool InitMemory();
        static bool InitGraphics(uint16_t* screen, uint32_t pitch);
        static bool InitApu();
        static bool InitSound(int buffer_ms, int lag_ms);
        static void InitControllers();

        static bool LoadRomMem(const byte* source, unsigned int sourceSize);

        static void MainLoop();

        static bool SaveState(const char* path);
        static bool LoadState(const char* path);

        static bool SaveSRAM(const char* path);
        static bool LoadSRAM(const char* path);

        static void ReportButton(unsigned int id, bool pressed);
    };
} }