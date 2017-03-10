#pragma once

namespace Snes9x { namespace Core
{

    public ref class CoreJoypad sealed
    {
    public:
        CoreJoypad(unsigned int joypadNumber);

#define DEF_REPORT(s) void Report##s(bool pressed)
        DEF_REPORT(A);
        DEF_REPORT(B);
        DEF_REPORT(X);
        DEF_REPORT(Y);
        DEF_REPORT(L);
        DEF_REPORT(R);
        DEF_REPORT(Start);
        DEF_REPORT(Select);
        DEF_REPORT(Up);
        DEF_REPORT(Down);
        DEF_REPORT(Left);
        DEF_REPORT(Right);

        property unsigned int JoypadNumber;
    };

} }