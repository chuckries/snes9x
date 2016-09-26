#pragma once

namespace Snes9xCore
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