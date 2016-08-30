#pragma once

namespace Snes9xWRC
{

    public ref class CoreSettings sealed
    {
    public:
        CoreSettings();

        // Properties
    public:
        property bool DisplayFrameRate
        {
            bool get();
            void set(bool value);
        }
    };

}