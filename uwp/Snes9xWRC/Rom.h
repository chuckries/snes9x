#pragma once

namespace Snes9xWRC
{

    public interface class IRom
    {
        property Platform::String^ Name
        {
            Platform::String^ get();
        }

        property Platform::String^ Path
        {
            Platform::String^ get();
        }

        IAsyncOperation<Object^>^ GetBytesAsync();
    };

}