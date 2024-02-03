using System;

namespace SkipCutscene.Utilities;
internal static class ThrowIfArgument
{
    public static T IsNull<T>(T value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return value;
    }
}
